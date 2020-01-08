using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using SpaceStrategy.Properties;

namespace SpaceStrategy
{
	public class Game : INotifyPropertyChanged
	{
		public Scenario Scenario;
		public TimeSpan TimerStep = new TimeSpan(4 * 10000);
		internal static Random Rand = new Random(DateTime.Now.Millisecond);

		/// <summary>
		///     Attacker - Defender.
		/// </summary>
		internal List<Tuple<Spaceship, Spaceship>> CommittedAttacks = new List<Tuple<Spaceship, Spaceship>>();

		internal CoordinateConverter CoordinateConverter;
		internal GameCursor Cursor;
		internal List<AnimationObject> DroppedAnimations = new List<AnimationObject>();
		readonly List<AnimationObject> _animations = new List<AnimationObject>();
		readonly AttackCompass _attackCompass;
		readonly Bitmap _background = Resources.background;
		readonly List<GothicSpaceship> _destroyedSpaceships = new List<GothicSpaceship>();
		readonly List<GraphicObject> _graphicObjects = new List<GraphicObject>();

		readonly Stopwatch _stopwatch = new Stopwatch();

		readonly List<PositioningZone> _zones;

		//10000 ticks = 1 ms
		bool _allShipsStand = true;

		GamePhase _battlePhase = GamePhase.Ending;
		double _curAntiTorpedoDist;
		int _curBlastMarkersAtBase;
		int _curObjectsOnCourseCount;
		Player _currentPlayer;
		double _curTorpedoDist;
		int _fps;
		int _frameCounter;
		GameState _gameState;
		bool _isAutoCompleting;
		int _positioningCompletedPlayersCount;
		GameState _savedState;
		GothicSpaceship _selectedSpaceship;

		public Game()
		{
			Size = Screen.GetWorkingArea(new Point(0, 0)).Size;

			GameData = new GameDataAdapter();
			Players = new PlayerList();

			int firstPlayerIndex = Dice.RollDices(2, 1, "Определение первого игрока.");
			Players.Add(new Player(this, "Игрок 1", "Imperial", Color.Aqua));
			if (firstPlayerIndex == 1) {
				Players.Add(new Player(this, "Игрок 2", "Chaos", Color.Red));
			}
			else {
				Players.Insert(0, new Player(this, "Игрок 2", "Chaos", Color.Red));
			}

			CurrentPlayer = Players[0];
			_zones = new List<PositioningZone>();

			int positioningZoneWidth = 90;
			int positioningZoneHeight = 30;
			int distanceBetweenZones = 60;
			int centerX = 0;
			int centerY = 0;
			_zones.Add(new PositioningZone(this, Players[0],
				new Point2d(centerX - positioningZoneWidth / 2, centerY - distanceBetweenZones / 2 - positioningZoneHeight),
				new Point2d(centerX + positioningZoneWidth / 2, centerY - distanceBetweenZones / 2)));
			_zones.Add(new PositioningZone(this, Players[1],
				new Point2d(centerX - positioningZoneWidth / 2, centerY + distanceBetweenZones / 2),
				new Point2d(centerX + positioningZoneWidth / 2, centerY + distanceBetweenZones / 2 + positioningZoneHeight)));

			foreach (PositioningZone zone in _zones) {
				zone.PositioningBegin += zone_PositioningBegin;
				zone.PositioningComplete += zone_PositioningComplete;
			}

			var scenarioNeeds = new Size(90, distanceBetweenZones + positioningZoneHeight * 2);
			float maxScale = Math.Min(Size.Height / scenarioNeeds.Height, Size.Width / scenarioNeeds.Width);
			CoordinateConverter = new CoordinateConverter(this, maxScale);

			AllShipsStopped += Game_AllShipsStopped;

			Paused = false;
			CompletedTurnsCount = 0;
			GameState = GameState.Menu;
			Cursor = new GameCursor(this);
			_attackCompass = new AttackCompass(this);
			Params = new GothicGameParams(this);
			AnimationHelper.Game = this;
			MoveDistanceInLastTurn = new Dictionary<GothicSpaceshipBase, double>();
			PassedBlastMarkerSpaceships = new List<GothicSpaceshipBase>();
			ScriptManager = new ScriptManager();
			Scenario = new BasicScenario(this);
		}

		public event PropertyChangedEventHandler PropertyChanged;

		public event EventHandler<AllShipsStoppedEventArgs> AllShipsStopped;

		/// <summary>
		///     Event of advancing from spaceships positioning to battle cycle.
		/// </summary>
		public event EventHandler BattleStarted;

		public event EventHandler<CursorEventArgs> CursorMove;

		public event EventHandler<NextBattlePhaseEventArgs> NextBattlePhase;

		public event EventHandler<NextPlayerEventArgs> NextTurn;

		public event EventHandler<Point2dSelectEventArgs> PointSelected;

		public event EventHandler<NextPlayerEventArgs> StartPositioningPhase;

		public event EventHandler<NextPlayerEventArgs> StartPositioningShips;

		//public IEnumerable<GothicTrajectorySpecialOrder> AvailableOrders
		//{
		//	get
		//	{
		//		var result = new List<GothicTrajectorySpecialOrder>();
		//		if (SelectedSpaceship == null || SelectedSpaceship.Player!=CurrentPlayer) {
		//			return new List<GothicTrajectorySpecialOrder>();
		//		}
		//		if (CurrentPlayer.SpecialOrderFail) {
		//			return new List<GothicTrajectorySpecialOrder>();
		//		}
		//		switch (BattlePhase) {
		//			case GamePhase.Movement:
		//				if (SelectedSpaceship.SpecialOrder == GothicTrajectorySpecialOrder.NormalMove ||
		//					SelectedSpaceship.SpecialOrder == GothicTrajectorySpecialOrder.None)
		//				{
		//					if (SelectedSpaceship.Weapons.OfType<TorpedoWeapon>().Any(a => a.Power > a.LoadedTorpedoCount))
		//					{
		//						result.Add(GothicTrajectorySpecialOrder.ReloadOrdnance);
		//					}
		//				}
		//				if (SelectedSpaceship.SpecialOrder == GothicTrajectorySpecialOrder.NormalMove)
		//				{
		//					result.Add(GothicTrajectorySpecialOrder.BraceForImpact);
		//				}
		//				else if (SelectedSpaceship.SpecialOrder== GothicTrajectorySpecialOrder.None) {
		//					result.Add(GothicTrajectorySpecialOrder.AllAheadFull);
		//					result.Add(GothicTrajectorySpecialOrder.BurnRetros);
		//					result.Add(GothicTrajectorySpecialOrder.ComeToNewDirection);
		//					result.Add(GothicTrajectorySpecialOrder.LockOn);
		//					result.Add(GothicTrajectorySpecialOrder.BraceForImpact);
		//				}
		//				break;
		//			case GamePhase.Attack:
		//				if (SelectedSpaceship.Weapons.OfType<TorpedoWeapon>().Any(a => a.LoadedTorpedoCount > 0))
		//				{
		//					result.Add(GothicTrajectorySpecialOrder.LaunchOrdnance);
		//				}
		//				break;
		//			case GamePhase.Ordnance:
		//				result.Add(GothicTrajectorySpecialOrder.BraceForImpact);
		//			break;
		//			case GamePhase.Ending:
		//				break;
		//			default:
		//				break;
		//		}
		//		return result;
		//	}
		//}
		internal enum TurnPhase
		{
			Movement,
			Attack,
			Ordnance
		}

		enum Races
		{
			ImperialMarine = 1,
			ChaosMarine = 2
		}

		public int CompletedTurnsCount { get; set; }

		public Player CurrentPlayer
		{
			get { return _currentPlayer; }
			private set
			{
				if (_currentPlayer != value) {
					_currentPlayer = value;
					NotifyPropertyChanged("CurrentPlayer");
					if (_selectedSpaceship != null) {
						_selectedSpaceship.NotifyPropertyChanged("AvailableOrders");
					}
				}
			}
		}

		public IReadOnlyCollection<GothicSpaceship> DestroyedSpaceships
		{
			get { return _destroyedSpaceships.AsReadOnly(); }
		}

		public GameDataAdapter GameData { get; }

		public GameState GameState
		{
			get { return _gameState; }
			set
			{
				if (_gameState != value) {
					_gameState = value;
					NotifyPropertyChanged("GameState");
				}
			}
		}

		public bool Paused { get; set; }

		public PlayerList Players { get; }

		public GothicSpaceship SelectedSpaceship
		{
			get { return _selectedSpaceship; }
			internal set
			{
				if (_selectedSpaceship != value) {
					_selectedSpaceship = value;
					NotifyPropertyChanged("SelectedSpaceship");
				}
			}
		}

		public Size Size { get; }

		internal IEnumerable<AnimationObject> Animations
		{
			get { return _animations; }
		}

		internal GamePhase BattlePhase
		{
			get { return _battlePhase; }
			set
			{
				if (_battlePhase != value) {
					_battlePhase = value;
					NotifyPropertyChanged("BattlePhase");
					if (_selectedSpaceship != null) {
						_selectedSpaceship.NotifyPropertyChanged("AvailableOrders");
					}
				}
			}
		}

		internal IEnumerable<BlastMarker> BlastMarkers
		{
			get { return _graphicObjects.OfType<BlastMarker>(); }
		}

		internal TurnPhase CurrentPhase { get; private set; }

		internal IEnumerable<GothicSpaceship> GothicSpaceships
		{
			get { return _graphicObjects.OfType<GothicSpaceship>(); }
		}

		internal IEnumerable<GraphicObject> GraphicObjects
		{
			get { return _graphicObjects; }
		}

		internal Dictionary<GothicSpaceshipBase, double> MoveDistanceInLastTurn { get; }

		internal GothicGameParams Params { get; }

		internal List<GothicSpaceshipBase> PassedBlastMarkerSpaceships { get; private set; }

		internal ScriptManager ScriptManager { get; }

		internal IEnumerable<GothicSpaceshipBase> SpaceBodies
		{
			get { return _graphicObjects.OfType<GothicSpaceshipBase>(); }
		}

		internal IEnumerable<TorpedoSalvo> TorpedoSalvos
		{
			get { return _graphicObjects.OfType<TorpedoSalvo>(); }
		}

		public void EndBattlePhase()
		{
			if (BattlePhase == GamePhase.Movement) {
				//Unselect spaceships.
				_isAutoCompleting = false;
				foreach (GothicSpaceship spaceship in CurrentPlayer.Spaceships)
					if (spaceship.State == SpaceshipState.Moving) {
						_isAutoCompleting = true;
						spaceship.MovementStarted = true;

						//break;
					}
					else if (spaceship.TryAutoMoveMandatoryDistance()) {
						_isAutoCompleting = true;
					}

				if (!_isAutoCompleting) {
					foreach (GothicSpaceship spaceship in CurrentPlayer.Spaceships) spaceship.Trajectory = null;
				}
			}
			else if (BattlePhase == GamePhase.Attack) { }
			else if (BattlePhase == GamePhase.Ordnance) {
				_isAutoCompleting = false;
				Player curOrdnancePlayer = CurrentPlayer;
				do {
					foreach (TorpedoSalvo ordnance in TorpedoSalvos.Where(a => a.Player == curOrdnancePlayer))
						if (ordnance.State == SpaceshipState.Moving || ordnance.TryAutoMoveMandatoryDistance()) {
							_isAutoCompleting = true;

							//break;
						}

					if (!_isAutoCompleting) {
						curOrdnancePlayer = NextPlayer(curOrdnancePlayer);
					}
					else {
						break;
					}
				} while (curOrdnancePlayer != CurrentPlayer);

				if (!_isAutoCompleting) {
					foreach (TorpedoSalvo ordnance in TorpedoSalvos) ordnance.Trajectory = null;
				}
			}
			else if (BattlePhase == GamePhase.Ending) { }
			else {
				throw new InvalidOperationException("Недопустимое состояние игры");
			}

			if (!_isAutoCompleting) {
				AdvanceGame();
			}
		}

		public void MouseMove(Point2d point, MouseButtons button)
		{
			CoordinateConverter.CurWinPoint = point;
			if (button == MouseButtons.None) {
				Point2d gameCsPoint = CoordinateConverter.GetGameCsPoint(point);
				switch (GameState) {
					case GameState.Battle:
						if (SelectedSpaceship != null &&
							SelectedSpaceship.Trajectory != null) {
							SelectedSpaceship.Trajectory.OnMouseMove(gameCsPoint);
						}

						Cursor.SetForm(CursorForm.Default);
						break;

					case GameState.Attack:
						Cursor.SetForm(CursorForm.Attack);
						break;

					case GameState.SelectingPoint:
						Cursor.SetForm(CursorForm.Attack);
						break;

					case GameState.SelectingShips:
						Cursor.SetForm(CursorForm.Default);
						break;

					case GameState.CreatingSpaceship:
						CurrentPlayer.PositioningZone.OnMouseMove(gameCsPoint);
						Cursor.SetForm(CursorForm.Default);
						break;

					default:
						Cursor.SetForm(CursorForm.Default);
						break;
				}

				Cursor.SetLocation(point);
				OnCursorMove(this, new CursorEventArgs(gameCsPoint));
			}
			else if (button == MouseButtons.Right) {
				CoordinateConverter.FinishFieldDrag(point);
			}
		}

		public Player NextPlayer(Player currentPlayer)
		{
			int currentPlayerIndex = currentPlayer != null ? Players.IndexOf(currentPlayer) : -1;
			return Players[(currentPlayerIndex + 1) % Players.Count];
		}

		public void OnDraw(Graphics dc)
		{
			DrawBackground(dc);
			dc.ScaleTransform(CoordinateConverter.Scale, CoordinateConverter.Scale);
			dc.TranslateTransform((float)CoordinateConverter.Translation.X, (float)CoordinateConverter.Translation.Y);

			if (GameState == GameState.CreatingSpaceship) {
				CurrentPlayer.PositioningZone.Draw(dc);
			}

			foreach (GraphicObject graphicObject in GraphicObjects) {
				if (graphicObject is GothicSpaceship) {
					var spaceship = graphicObject as GothicSpaceship;

					if (spaceship.Player == CurrentPlayer || spaceship.IsSelected) {
						if (spaceship.IsDestroyed == CatastrophicDamage.None) {
							if (BattlePhase == GamePhase.Movement) {
								spaceship.Trajectory.Draw(dc);
							}
						}
					}

					if (spaceship.IsSelected) {
						//if (BattlePhase == GamePhase.Attack)
						{
							foreach (SpaceshipWeapon weapon in spaceship.Weapons) {
								if (weapon.IsUsed) {
									continue;
								}

								_attackCompass.Draw(dc, weapon.LineColor, spaceship.Position, weapon.SpaceshipSide, weapon.Range);
							}
						}
						if (spaceship.IsDestroyed == CatastrophicDamage.None) {
							if (BattlePhase == GamePhase.Movement) {
								spaceship.Trajectory.Draw(dc);
							}
						}
					}
				}

				graphicObject.Draw(dc);
			}

			for (int i = 0; i < _animations.Count(); i++) _animations[i].Draw(dc);
			dc.ResetTransform();

			//int pointsFontSize=10;
			//Font f1 = new Font("CurierNew", pointsFontSize, FontStyle.Bold);
			//Brush b1 = new SolidBrush(Color.White);
			//PointF curPointsPos = new PointF(0,0);
			//dc.DrawString("Очки", f1, b1, curPointsPos);
			//foreach (var player in Players)
			//{
			//	curPointsPos = new PointF(curPointsPos.X, curPointsPos.Y + (int)(pointsFontSize*1.2));
			//	Brush b2 = new SolidBrush(player.Color);
			//	dc.DrawString(player.Name + ": " + player.Points, f1, b2, curPointsPos);
			//}

			bool debug = true;
			if (debug) {
				//Cursor block
				var f = new Font("CurierNew", 8, FontStyle.Regular);
				Brush b = new SolidBrush(Color.Lime);

				//dc.DrawString(CoordinateConverter.CurWinCsPoint.ToString() + " WinCursor", f, b, new PointF(10, 320));
				//dc.DrawString(CoordinateConverter.CurGameCsPoint.ToString() + " GameCursor", f, b, new PointF(10, 335));
				//dc.DrawString(CoordinateConverter.LastSelectedShipGameCsPoint.ToString() + " ShipCoordinate", f, b, new PointF(10, 350));
				//dc.DrawString(CoordinateConverter.Translation.ToString() + " Translation", f, b, new PointF(10, 365));

				//dc.DrawString(curTorpedoDist.ToString() + " torpedo dist to target", f, b, new PointF(10, 365));
				//dc.DrawString(curAntiTorpedoDist.ToString() + " anti-torpedo dist", f, b, new PointF(10, 380));
				//dc.DrawString(curObjectsOnCourseCount.ToString() + " objects on course count", f, b, new PointF(10, 395));

				GamePrinter.Draw(dc, _curBlastMarkersAtBase + " blastMarkers at base", new PointF(10, 410), f, b);

				GamePrinter.Draw(dc);
				_frameCounter = (_frameCounter + 1) % 30;
				if (_frameCounter == 0) {
					_fps = (int)(1000.0 / TimerStep.TotalMilliseconds);
				}

				GamePrinter.Draw(dc, "fps: " + _fps, new PointF(500, 0), f, b);
			}

			Cursor.Draw(dc);
			if (GameState != GameState.SelectingPoint) {
				Cursor.SetForm(CursorForm.Default);
			}
		}

		public void OnKeyDown(KeyEventArgs key)
		{
			switch (key.KeyCode) {
				case Keys.D1: break;
			}
		}

		public void OnMouseClick(Point2d point, MouseButtons button)
		{
			Point2d gameCsPoint = CoordinateConverter.GetGameCsPoint(point);
			switch (GameState) {
				case GameState.Battle:
					if (button == MouseButtons.Left) {
						if (TrySelectSpaceship(gameCsPoint, out GothicSpaceshipBase newSelectedSpaceship)) {
							if (newSelectedSpaceship is GothicSpaceship) {
								if (SelectedSpaceship == null) {
									SelectedSpaceship = newSelectedSpaceship as GothicSpaceship;
									SelectedSpaceship.IsSelected = true;
								}
								else if (SelectedSpaceship != newSelectedSpaceship) {
									SelectedSpaceship.IsSelected = false;
									SelectedSpaceship = newSelectedSpaceship as GothicSpaceship;
									SelectedSpaceship.IsSelected = true;
								}
							}

							CoordinateConverter.LastSelectedShipGameCsPoint = SelectedSpaceship.Position.Location;
						}
						else {
							if (SelectedSpaceship != null) {
								SelectedSpaceship.IsSelected = false;
								SelectedSpaceship = null;
							}
						}

						break;
					}

					switch (BattlePhase) {
						case GamePhase.Movement:
							if (button == MouseButtons.Right) {
								if (CoordinateConverter.IsDraggingField) {
									if (SelectedSpaceship != null) {
										if (SelectedSpaceship.State == SpaceshipState.Standing) {
											if (SelectedSpaceship.Player == CurrentPlayer) {
												SelectedSpaceship.MoveTo(gameCsPoint);
											}
										}
									}
								}
							}

							break;

						case GamePhase.Attack:
							if (button == MouseButtons.Right) {
								if (SelectedSpaceship != null && SelectedSpaceship.Player == CurrentPlayer) {
									if (TrySelectSpaceship(gameCsPoint, out GothicSpaceshipBase attackedSpaceship)) {
										if (attackedSpaceship != SelectedSpaceship && (Params.FriendlyFire || attackedSpaceship.Player != CurrentPlayer)) {
											SelectedSpaceship.Attack(attackedSpaceship);
										}
									}
								}
							}

							break;

						case GamePhase.Ordnance:
							break;

						case GamePhase.Ending:
							break;
					}

					break;

				case GameState.SelectingShips:
					break;

				case GameState.CreatingSpaceship:
					CurrentPlayer.PositioningZone.OnMouseClick(gameCsPoint);
					break;

				case GameState.SelectingPoint:
					EndPointSelect(gameCsPoint);
					break;
			}
		}

		public void OnMouseDown(Point2d point, MouseButtons button)
		{
			if (button == MouseButtons.Right) {
				CoordinateConverter.StartFieldDrag(point);
			}
		}

		public void OnTimer()
		{
			TimerStep = _stopwatch.Elapsed;
			_stopwatch.Reset();
			_stopwatch.Start();
			bool debug = true;
			if (debug) {
				_curBlastMarkersAtBase = SelectedSpaceship != null ? SelectedSpaceship.BlastMarkersAtBase.Count : 0;
			}

			if (!Paused) {
				bool newAllShipsStand = true;
				List<GothicSpaceshipBase> spaceshipsToRemove = SpaceBodies.Where(a => a.State == SpaceshipState.Removed).ToList();
				foreach (GothicSpaceshipBase spaceship in spaceshipsToRemove) RemoveSpaceship(spaceship);
				foreach (GothicSpaceshipBase spaceship in SpaceBodies) spaceship.OnTime(TimerStep);

				foreach (GothicSpaceshipBase spaceship in SpaceBodies) {
					var curBlastMarkersAtBase = new List<BlastMarker>();
					foreach (BlastMarker bm in BlastMarkers.Where(a => a.IsCollisionObject))
						if (bm.Position.DistanceTo(spaceship.Position) <= BlastMarker.CollisionRadius + spaceship.Diameter / 2) {
							curBlastMarkersAtBase.Add(bm);
						}

					if (spaceship.IsCollisionObject) {
						spaceship.BlastMarkersAtBase.SetCurBlastMarkers(curBlastMarkersAtBase);
					}

					if (spaceship.TryMove(TimerStep, out double movedDistance)) {
						newAllShipsStand = false;

						var objectsOnCourse = new List<GraphicObject>();
						Trajectory trajectory = spaceship.Trajectory.First();
						foreach (GraphicObject possiblyCollidedObject in GraphicObjects.Where(a => a != spaceship && a.IsCollisionObject))
							if (possiblyCollidedObject is Spaceship) {
								if (trajectory.IsOnCourse(possiblyCollidedObject.Position.Location, (possiblyCollidedObject as Spaceship).Diameter / 2 + spaceship.Diameter / 2)) {
									objectsOnCourse.Add(possiblyCollidedObject);
								}
							}

						if (debug) {
							_curObjectsOnCourseCount = objectsOnCourse.Count;
						}

						CheckSpaceshipCollision(spaceship, objectsOnCourse);
					}
				}

				ScriptManager.OnTime(TimerStep);

				if (!_allShipsStand && newAllShipsStand) {
					if (_isAutoCompleting) {
						OnAllShipsStopped(this, new AllShipsStoppedEventArgs(ShipsStopCause.AutoCompletion));
					}
				}

				_allShipsStand = newAllShipsStand;

				foreach (AnimationObject droppedAnimation in DroppedAnimations) RemoveAnimation(droppedAnimation);
				DroppedAnimations.Clear();
				for (int i = 0; i < _animations.Count(); i++) _animations[i].OnTime(TimerStep);
			}
		}

		public void SelectPoint()
		{
			_savedState = GameState;
			GameState = GameState.SelectingPoint;
		}

		public void StartBattle()
		{
			GameState = GameState.Battle;
			EndBattlePhase();
		}

		public void StartFleetPositioning()
		{
			GameState = GameState.CreatingSpaceship;
			OnStartPositioningPhase(this, new NextPlayerEventArgs(CurrentPlayer));
			CurrentPlayer.PositioningZone.StartPositioning();
		}

		public void StartGame()
		{
			StartFleetPositioning();
		}

		internal void AddAnimation(AnimationObject animation)
		{
			_animations.Add(animation);
		}

		internal void AddGraphicObject(GraphicObject obj)
		{
			_graphicObjects.Add(obj);
		}

		internal void AddSpaceship(Spaceship spaceship)
		{
			_graphicObjects.Add(spaceship);
		}

		internal void AddSpaceshipMoveDistanceInLastTurn(GothicSpaceshipBase spaceship, double distance)
		{
			if (MoveDistanceInLastTurn.ContainsKey(spaceship)) {
				MoveDistanceInLastTurn[spaceship] += distance;
			}
			else {
				MoveDistanceInLastTurn[spaceship] = distance;
			}
		}

		internal void AddTorpedoSalvo(TorpedoSalvo torpedoSalve)
		{
			_graphicObjects.Add(torpedoSalve);
		}

		internal double GetSpaceshipMoveDistanceInLastTurn(GothicSpaceshipBase spaceship)
		{
			if (MoveDistanceInLastTurn.TryGetValue(spaceship, out double distance)) {
				return distance;
			}

			return 0;
		}

		internal void RemoveAnimation(AnimationObject animation)
		{
			_animations.Remove(animation);
		}

		internal void RemoveSpaceship(Spaceship spaceship)
		{
			_graphicObjects.Remove(spaceship);
			var gss = spaceship as GothicSpaceship;
			if (gss != null) {
				if (!_destroyedSpaceships.Contains(gss) && gss.IsDestroyed != CatastrophicDamage.None) {
					_destroyedSpaceships.Add(gss);
				}
			}
		}

		protected virtual void NotifyPropertyChanged(string propertyName)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null) {
				handler(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		protected void OnAllShipsStopped(object sender, AllShipsStoppedEventArgs e)
		{
			EventHandler<AllShipsStoppedEventArgs> handler = AllShipsStopped;
			if (handler != null) {
				handler(sender, e);
			}
		}

		protected void OnBattleStarted(object sender, EventArgs e)
		{
			if (BattleStarted != null) {
				BattleStarted(sender, e);
			}
		}

		protected virtual void OnCursorMove(object sender, CursorEventArgs e)
		{
			if (CursorMove != null) {
				CursorMove(sender, e);
			}
		}

		protected virtual void OnNextBattlePhase(object sender, NextBattlePhaseEventArgs e)
		{
			if (NextBattlePhase != null) {
				NextBattlePhase(sender, e);
			}
		}

		protected void OnNextTurn(object sender, NextPlayerEventArgs e)
		{
			if (NextTurn != null) {
				NextTurn(sender, e);
			}
		}

		protected virtual void OnPointSelected(object sender, Point2dSelectEventArgs e)
		{
			if (PointSelected != null) {
				PointSelected(sender, e);
			}
		}

		protected virtual void OnStartPositioningPhase(object sender, NextPlayerEventArgs e)
		{
			if (StartPositioningPhase != null) {
				StartPositioningPhase(sender, e);
			}
		}

		protected virtual void OnStartPositioningShips(object sender, NextPlayerEventArgs e)
		{
			if (StartPositioningShips != null) {
				StartPositioningShips(sender, e);
			}
		}

		void AdvanceGame()
		{
			switch (BattlePhase) {
				case GamePhase.Movement:
					foreach (GothicSpaceship ss in CurrentPlayer.Spaceships.ToList())
						if (ss.IsDestroyed == CatastrophicDamage.BlazingHulk) {
							Point2d bmsPos = new Point2d(ss.Position.Angle + GeometryHelper.Pi, ss.Diameter / 2).ToEuclidCs(ss.Position);
							var bm = new BlastMarker(this, new Position(bmsPos, GeometryHelper.Pi), new TimeSpan());
							ss.BlastMarkersAtBase.AddBlastMarker(bm);
							AddGraphicObject(bm);
						}

					BattlePhase = GamePhase.Attack;
					break;

				case GamePhase.Attack:
					BattlePhase = GamePhase.Ordnance;
					foreach (TorpedoSalvo torpedo in TorpedoSalvos) {
						torpedo.Trajectory = new TrajectoryCollection(torpedo);
						var trajectory = new GothicTrajectory(
							torpedo.Trajectory,
							torpedo.Position,
							torpedo.Speed,
							double.PositiveInfinity,
							0,
							torpedo.Speed);
						torpedo.Trajectory.Add(trajectory);
						torpedo.State = SpaceshipState.Standing;
					}

					EndBattlePhase();
					return;

				case GamePhase.Ordnance:
					BattlePhase = GamePhase.Ending;
					AdvanceGame();
					return;

				case GamePhase.Ending:
					FixSpaceships();
					RemoveOldBlastMarkers();

					//Столкновения с blast marker'ами надо обновлять до инициализации траекторий, т.к. они могут заполнять этот массив, если у основания коробля есть маркеры взрывов.
					PassedBlastMarkerSpaceships = new List<GothicSpaceshipBase>();
					DisposeUselessTorpedoes();

					UpdateSpaceshipsTrajectories();

					ResetWeaponUsage();

					CurrentPlayer = NextPlayer(CurrentPlayer);
					CurrentPlayer.SpecialOrderFail = false;
					foreach (GothicSpaceship ss in GothicSpaceships.Where(a => a.Player == CurrentPlayer)) ss.MovementStarted = false;
					CommittedAttacks.Clear();

					foreach (KeyValuePair<GothicSpaceshipBase, double> kvp in MoveDistanceInLastTurn.Where(a => a.Key.Player == CurrentPlayer)) MoveDistanceInLastTurn[kvp.Key] = 0;

					ResetSpaceshipSelection();
					CompletedTurnsCount++;
					if (Scenario.IsComplete()) {
						GameState = GameState.End;
					}

					OnNextTurn(this, new NextPlayerEventArgs(CurrentPlayer));
					break;
			}

			OnNextBattlePhase(this, new NextBattlePhaseEventArgs(CurrentPlayer, BattlePhase));
		}

		void CheckSpaceshipCollision(GothicSpaceshipBase spaceship, List<GraphicObject> objectsOnCourse)
		{
			foreach (GraphicObject possibleCollision in objectsOnCourse) {
				if (CommittedAttacks.Any(a => a.Item1 == possibleCollision && a.Item2 == spaceship) &&
					CommittedAttacks.Any(a => a.Item1 == spaceship && a.Item2 == possibleCollision)) {
					continue;
				}

				if (spaceship is TorpedoSalvo || possibleCollision is TorpedoSalvo) {
					TorpedoSalvo torpedo = spaceship is TorpedoSalvo ? spaceship as TorpedoSalvo : possibleCollision is TorpedoSalvo ? possibleCollision as TorpedoSalvo : null;
					GraphicObject collisionObject = torpedo == spaceship ? possibleCollision : spaceship;

					double distance = collisionObject.Position.Location.DistanceTo(torpedo.Position.Location);
					if (!CommittedAttacks.Any(a => a.Item1 == collisionObject && a.Item2 == torpedo)) {
						if (collisionObject is GothicSpaceship) {
							var gss = collisionObject as GothicSpaceship;
							double antiTorpedoAttackDistance = torpedo.Size + gss.Diameter * 3;
							_curTorpedoDist = distance;
							_curAntiTorpedoDist = antiTorpedoAttackDistance;
							if (distance < antiTorpedoAttackDistance) {
								TurretWeapon turret = gss.Weapons.OfType<TurretWeapon>().FirstOrDefault();
								if (turret != null) {
									turret.Attack(torpedo, new List<SpaceshipWeapon> { turret });
								}

								CommittedAttacks.Add(new Tuple<Spaceship, Spaceship>(gss, torpedo));
							}
						}
					}

					if (!CommittedAttacks.Any(a => a.Item1 == torpedo && a.Item2 == collisionObject)) {
						if (collisionObject is GothicSpaceshipBase) {
							var spaceshipBase = collisionObject as GothicSpaceshipBase;
							double torpedoAttackDistance = torpedo.Size / 2 + spaceshipBase.Diameter / 2;
							if (distance < torpedoAttackDistance) {
								torpedo.Attack(spaceshipBase);
								CommittedAttacks.Add(new Tuple<Spaceship, Spaceship>(torpedo, spaceshipBase));
							}
						}
					}
				}
			}
		}

		void DisposeUselessTorpedoes()
		{
			foreach (TorpedoSalvo torpedo in TorpedoSalvos) {
				double minDistToSpaceship = double.PositiveInfinity;
				foreach (GothicSpaceshipBase ss in SpaceBodies) {
					if (ss == torpedo || ss is TorpedoSalvo) {
						continue;
					}

					double dist = torpedo.Position.Location.DistanceTo(ss.Position.Location);
					if (dist < minDistToSpaceship) {
						minDistToSpaceship = dist;
					}
				}

				if (minDistToSpaceship > 60) {
					torpedo.State = SpaceshipState.Removed;
				}
			}
		}

		void DrawBackground(Graphics dc)
		{
			dc.DrawImage(_background, 0, 0, _background.Width, _background.Height);

			//dc.DrawImageUnscaled(b, 0, 0, b.Width, b.Height);
		}

		void EndPointSelect(Point2d selectedPoint)
		{
			GameState = _savedState;
			OnPointSelected(this, new Point2dSelectEventArgs(selectedPoint));
		}

		void FixSpaceships()
		{
			foreach (GothicSpaceship spaceship in GothicSpaceships.ToList()) {
				spaceship.RestoreCriticalDamage();
				spaceship.ApplyCriticalDamageConsequences();
			}
		}

		void Game_AllShipsStopped(object sender, AllShipsStoppedEventArgs e)
		{
			if (e.StopCause == ShipsStopCause.AutoCompletion) {
				EndBattlePhase();
			}
		}

		void RemoveOldBlastMarkers()
		{
			var possiblyRemovedBms = new List<BlastMarker>();
			foreach (BlastMarker blastMarker in BlastMarkers) {
				bool isAtSpaceshipBase = false;
				foreach (GothicSpaceshipBase ss in SpaceBodies.Where(a => a.State == SpaceshipState.Moving || a.State == SpaceshipState.Standing))
					if (ss.BlastMarkersAtBase.Contains(blastMarker)) {
						isAtSpaceshipBase = true;
						break;
					}

				if (!isAtSpaceshipBase) {
					possiblyRemovedBms.Add(blastMarker);
				}
			}

			int removedBmsCount = Dice.RollDices(6, 1, "Максимальное количество удаленных маркеров взрыва.");
			List<BlastMarker> bmsToRemove = possiblyRemovedBms.GetRandomItems(removedBmsCount);
			foreach (BlastMarker blastMarker in bmsToRemove) {
				_graphicObjects.Remove(blastMarker);
				blastMarker.Dispose();
			}
		}

		void ResetSpaceshipSelection()
		{
			if (SelectedSpaceship != null) {
				SelectedSpaceship.IsSelected = false;
				SelectedSpaceship = null;
			}
		}

		void ResetWeaponUsage()
		{
			foreach (GothicSpaceship spaceship in GothicSpaceships) {
				if (spaceship.Player != CurrentPlayer) {
					continue;
				}

				foreach (SpaceshipWeapon weapon in spaceship.Weapons)
					if (!(weapon is TorpedoWeapon)) {
						weapon.IsUsed = false;
					}
			}
		}

		bool TrySelectSpaceship(Point2d point, out GothicSpaceshipBase selectedSpaceship)
		{
			double spaceshipRadiusSqr = Params.SpaceshipDiameter * Params.SpaceshipDiameter / 4;
			var possiblySelectedSpaceships = new List<Tuple<GothicSpaceshipBase, double>>();
			foreach (GothicSpaceshipBase spaceship in SpaceBodies) {
				double distanceSqrToSpaceship = point.DistanceSqrTo(spaceship.Position.Location);
				if (distanceSqrToSpaceship <= spaceshipRadiusSqr) {
					possiblySelectedSpaceships.Add(new Tuple<GothicSpaceshipBase, double>(spaceship, distanceSqrToSpaceship));
				}
			}

			if (possiblySelectedSpaceships.Count == 0) {
				selectedSpaceship = null;
				return false;
			}

			Tuple<GothicSpaceshipBase, double> minDistanceSpaceship = possiblySelectedSpaceships.First();
			foreach (Tuple<GothicSpaceshipBase, double> spaceship in possiblySelectedSpaceships)
				if (minDistanceSpaceship.Item2 > spaceship.Item2) {
					minDistanceSpaceship = spaceship;
				}

			selectedSpaceship = minDistanceSpaceship.Item1;
			return true;
		}

		void UpdateSpaceshipsTrajectories()
		{
			BattlePhase = GamePhase.Movement;
			foreach (GothicSpaceship spaceship in GothicSpaceships) {
				spaceship.Trajectory = new TrajectoryCollection(spaceship);

				var curTrajectory = new GothicTrajectory(
					spaceship.Trajectory,
					spaceship.Position,
					spaceship.Speed,
					spaceship.MinRunBeforeTurn,
					spaceship.MaxTurnAngle);
				if (spaceship.BlastMarkersAtBase.Any()) {
					curTrajectory.CutDistanceToMove(Params.BlastMarkerSpeedDamage);
				}

				spaceship.Trajectory.Add(curTrajectory);

				spaceship.State = SpaceshipState.Standing;
				spaceship.SpecialOrder = GothicOrder.None;
			}
		}

		void zone_PositioningBegin(object sender, EventArgs e)
		{
			OnStartPositioningShips(this, new NextPlayerEventArgs(CurrentPlayer));
		}

		void zone_PositioningComplete(object sender, EventArgs e)
		{
			_positioningCompletedPlayersCount++;
			if (_positioningCompletedPlayersCount == Players.Count) {
				StartBattle();
				OnBattleStarted(this, new EventArgs());
			}
			else {
				CurrentPlayer = NextPlayer(CurrentPlayer);
				CurrentPlayer.PositioningZone.StartPositioning();
			}
		}

		internal class GothicGameParams
		{
			internal GothicGameParams(Game game)
			{
				Game = game;
			}

			public double BlastMarkerSpeedDamage
			{
				get { return 5; }
			}

			public bool Debug
			{
				get { return false; }
			}

			public Color HitPointsGaugeColor
			{
				get { return Color.Red; }
			}

			public Color ShieldGaugeColor
			{
				get { return Color.Cyan; }
			}

			internal Color ActiveTrajectoryColor
			{
				get { return Color.Orange; }
			}

			internal float ActiveTrajectoryThickness
			{
				get { return 3 / Game.CoordinateConverter.Scale; }
			}

			internal Color AttackCompassColor
			{
				get { return Color.Orange; }
			}

			internal float AttackCompassThickness
			{
				get { return 2 / Game.CoordinateConverter.Scale; }
			}

			internal float BlastMarkerDiameter
			{
				get { return SpaceshipDiameter * 0.9F; }
			}

			internal bool FriendlyFire
			{
				get { return true; }
			}

			internal Game Game { get; }

			internal Color MandatoryTrajectoryColor
			{
				get { return Color.Firebrick; }
			}

			internal Color PositioningZoneColor
			{
				get { return Color.Lime; }
			}

			internal Color SelectedMandatoryTrajectoryColor
			{
				get { return Color.Red; }
			}

			internal Color SelectedTrajectoryColor
			{
				get { return Color.Lime; }
			}

			internal float SelectionThickness
			{
				get { return 3 / Game.CoordinateConverter.Scale; }
			}

			internal int SpaceshipDiameter
			{
				get { return 3; }
			}

			internal Color SpaceshipSelectionColor
			{
				get { return Color.Gold; }
			}

			internal float TorpedoSize
			{
				get { return 2.5F; }
			}

			internal float TrajectoryAnchorDistance
			{
				get { return 3 / Game.CoordinateConverter.Scale; }
			}

			internal int TrajectoryAnchorPointRadius
			{
				get { return 4; }
			}

			internal Color TrajectoryColor
			{
				get { return Color.Green; }
			}

			internal float TrajectoryLineThickness
			{
				get { return 3 / Game.CoordinateConverter.Scale; }
			}
		}
	}


	public class NextPlayerEventArgs : EventArgs
	{
		public Player Player;

		public NextPlayerEventArgs(Player p)
		{
			Player = p;
		}
	}
}