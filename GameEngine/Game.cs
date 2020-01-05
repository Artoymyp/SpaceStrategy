using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceStrategy
{
	public class Game : INotifyPropertyChanged
	{
		GameState _gameState;
		GameState _savedState;
		int _positioningCompletedPlayersCount = 0;
		Player _currentPlayer;
		AttackCompass _attackCompass;
		List<AnimationObject> _animations = new List<AnimationObject>();
		List<GraphicObject> _graphicObjects = new List<GraphicObject>();
		List<GothicSpaceship> _destroyedSpaceships = new List<GothicSpaceship>();
		public Game()
		{
			Size = System.Windows.Forms.Screen.GetWorkingArea(new Point(0,0)).Size;

			GameData = new GameDataAdapter();
			Players = new PlayerList();

			int firstPlayerIndex = Dice.RollDices(2, 1, "Определение первого игрока.");
			Players.Add(new Player(this, "Игрок 1", "Imperial", Color.Aqua));
			if (firstPlayerIndex == 1)
				Players.Add(new Player(this, "Игрок 2", "Chaos", Color.Red));
			else
				Players.Insert(0, new Player(this, "Игрок 2", "Chaos", Color.Red));

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

			foreach (var zone in _zones) {
				zone.PositioningBegin += zone_PositioningBegin;
				zone.PositioningComplete += zone_PositioningComplete;
			}

			Size scenarioNeeds = new Size(90, distanceBetweenZones + positioningZoneHeight * 2);
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
		public Size Size {get;private set;}
		internal ScriptManager ScriptManager { get; private set; }		
		internal Dictionary<GothicSpaceshipBase, double> MoveDistanceInLastTurn { get; private set; }
		internal List<GothicSpaceshipBase> PassedBlastMarkerSpaceships { get; private set; }
		internal IEnumerable<AnimationObject> Animations { get { return _animations; } }
		internal IEnumerable<GothicSpaceship> GothicSpaceships { get { return _graphicObjects.OfType<GothicSpaceship>(); } }
		internal IEnumerable<GothicSpaceshipBase> SpaceBodies { get { return _graphicObjects.OfType<GothicSpaceshipBase>(); } }
		internal IEnumerable<BlastMarker> BlastMarkers { get { return _graphicObjects.OfType<BlastMarker>(); } }
		internal IEnumerable<TorpedoSalvo> TorpedoSalvos { get { return _graphicObjects.OfType<TorpedoSalvo>(); } }
		internal IEnumerable<GraphicObject> GraphicObjects { get { return _graphicObjects; } }
		internal GothicGameParams Params { get; private set; }
		public IReadOnlyCollection<GothicSpaceship> DestroyedSpaceships { get { return _destroyedSpaceships.AsReadOnly(); } }
		public PlayerList Players { get; private set; }
		public Player CurrentPlayer
		{
			get { return _currentPlayer; }
			private set
			{
				if (_currentPlayer != value)
				{
					_currentPlayer = value;
					NotifyPropertyChanged("CurrentPlayer");
					if (_selectedSpaceship != null)
					{
						_selectedSpaceship.NotifyPropertyChanged("AvailableOrders");
					}
				}
			}
		}
		internal TurnPhase CurrentPhase { get; private set; }
		public GameDataAdapter GameData { get; private set; }
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
		internal double GetSpaceshipMoveDistanceInLastTurn(GothicSpaceshipBase spaceship)
		{
			if (MoveDistanceInLastTurn.TryGetValue(spaceship, out var distance))
				return distance;
			else return 0;
		}
		internal void AddSpaceshipMoveDistanceInLastTurn(GothicSpaceshipBase spaceship, double distance)
		{
			if (MoveDistanceInLastTurn.ContainsKey(spaceship))
			{
				MoveDistanceInLastTurn[spaceship] += distance;
			}
			else
			{
				MoveDistanceInLastTurn[spaceship] = distance;
			}
		}

		internal void AddSpaceship(Spaceship spaceship) { 
			_graphicObjects.Add(spaceship);
		}

		internal void RemoveSpaceship(Spaceship spaceship) { 
			_graphicObjects.Remove(spaceship);
			var gss = spaceship as GothicSpaceship;
			if (gss != null)
			{
				if (!_destroyedSpaceships.Contains(gss) && gss.IsDestroyed!=CatastrophycDamage.None)
					_destroyedSpaceships.Add(gss);
			}
		}
		internal void AddTorpedoSalvo(TorpedoSalvo torpedoSalve) { _graphicObjects.Add(torpedoSalve); }
		internal void AddAnimation(AnimationObject animation) { _animations.Add(animation); }
		internal void RemoveAnimation(AnimationObject animation) { _animations.Remove(animation); }
		internal void AddGraphicObject(GraphicObject obj) { _graphicObjects.Add(obj); }
		public void SelectPoint()
		{
			_savedState = GameState;
			GameState = GameState.SelectingPoint;
		}
		private void EndPointSelect(Point2d selectedPoint)
		{
			GameState = _savedState;
			OnPointSelected(this, new Point2dSelectEventArgs(selectedPoint));
		}
		void Game_AllShipsStopped(object sender, AllShipsStoppedEventArgs e)
		{
			if (e.StopCause == ShipsStopCause.Autocompletion)
				EndBattlePhase();
		}
		public event EventHandler<CursorEventArgs> CursorMove;
		protected virtual void OnCursorMove(object sender, CursorEventArgs e)
		{
			if (CursorMove != null) {
				CursorMove(sender, e);
			}
		}
		public event EventHandler<Point2dSelectEventArgs> PointSelected;
		protected virtual void OnPointSelected(object sender, Point2dSelectEventArgs e)
		{
			if (PointSelected != null) {
				PointSelected(sender, e);
			}
		}
		public event EventHandler<NextBattlePhaseEventArgs> NextBattlePhase;
		protected virtual void OnNextBattlePhase(object sender, NextBattlePhaseEventArgs e)
		{
			if (NextBattlePhase != null) {
				NextBattlePhase(sender, e);
			}
		}
		public event EventHandler<NextPlayerEventArgs> StartPositioningPhase;
		protected virtual void OnStartPositioningPhase(object sender, NextPlayerEventArgs e)
		{
			if (StartPositioningPhase != null) {
				StartPositioningPhase(sender, e);
			}
		}


		public event EventHandler<NextPlayerEventArgs> StartPositioningShips;
		protected virtual void OnStartPositioningShips(object sender, NextPlayerEventArgs e)
		{
			if (StartPositioningShips != null) {
				StartPositioningShips(sender, e);
			}
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
		void zone_PositioningBegin(object sender, EventArgs e)
		{
			OnStartPositioningShips(this, new NextPlayerEventArgs(CurrentPlayer));
		}
		public event EventHandler<AllShipsStoppedEventArgs> AllShipsStopped;
		protected void OnAllShipsStopped(object sender, AllShipsStoppedEventArgs e)
		{
			var handler = AllShipsStopped;
			if (handler != null)
			{
				handler(sender, e);
			}
		}
		/// <summary>
		/// Event of advancing from spaceships positioning to battle cylce.
		/// </summary>
		public event EventHandler BattleStarted;
		protected void OnBattleStarted(object sender, EventArgs e)
		{
			if (BattleStarted != null)
				BattleStarted(sender, e);
		}
		public TimeSpan TimerStep = new TimeSpan(4 * 10000);//10000 ticks = 1 ms
		bool _allShipsStand = true;
		internal List<AnimationObject> DroppedAnimations = new List<AnimationObject>();
		Stopwatch _stopwatch = new Stopwatch();
		public void OnTimer()
		{			
			TimerStep = _stopwatch.Elapsed;
			_stopwatch.Reset();
			_stopwatch.Start();
			bool debug = true;
			if (debug)
			{
				_curBlastMarkersAtBase = SelectedSpaceship != null ? SelectedSpaceship.BlastMarkersAtBase.Count : 0;
			}
			if (!Paused) {
				bool newAllShipsStand = true;
				var spaceshipsToRemove = SpaceBodies.Where(a=>a.State== SpaceshipState.Removed).ToList();
				foreach (var spaceship in spaceshipsToRemove)
				{
					RemoveSpaceship(spaceship);
				}
				foreach (var spaceship in SpaceBodies)
				{
					spaceship.OnTime(TimerStep);
				}

				
				
				foreach (var spaceship in SpaceBodies) {
					List<BlastMarker> curBlastMarkersAtBase=new List<BlastMarker>();
					foreach (var bm in BlastMarkers.Where(a=>a.IsCollisionObject))
					{
						if (bm.Position.DistanceTo(spaceship.Position) <= BlastMarker.CollisionRadius+spaceship.Diameter/2)
							curBlastMarkersAtBase.Add(bm);
					}
					if (spaceship.IsCollisionObject)
						spaceship.BlastMarkersAtBase.SetCurBlastMarkers(curBlastMarkersAtBase);

					if (spaceship.TryMove(TimerStep, out var movedDistance))
					{
						newAllShipsStand = false;

						List<GraphicObject> objectsOnCourse = new List<GraphicObject>();
						Trajectory trajectory = spaceship.Trajectory.First();
						foreach (var possiblyCollidedObject in GraphicObjects.Where(a => a != spaceship && a.IsCollisionObject))
						{
							if (possiblyCollidedObject is Spaceship)
							{
								if (trajectory.IsOnCourse(possiblyCollidedObject.Position.Location, (possiblyCollidedObject as Spaceship).Diameter / 2 + spaceship.Diameter / 2))
								{
									objectsOnCourse.Add(possiblyCollidedObject);
								}
							}
						}
						if (debug)
						{
							_curObjectsOnCourseCount = objectsOnCourse.Count;
						}
						CheckSpaceshipCollision(spaceship, objectsOnCourse);
					}
				}
				ScriptManager.OnTime(TimerStep);
			 
				if (!_allShipsStand&&newAllShipsStand) {
					if (_isAutoCompleting)
					OnAllShipsStopped(this, new AllShipsStoppedEventArgs(ShipsStopCause.Autocompletion));
				}
				_allShipsStand = newAllShipsStand;

				foreach (var droppedAnimation in DroppedAnimations)
				{
					RemoveAnimation(droppedAnimation);
				}
				DroppedAnimations.Clear();
				for (int i = 0; i < _animations.Count(); i++)
				{
					_animations[i].OnTime(TimerStep);
				}
			}
		}
		double _curTorpedoDist;
		double _curAntiTorpedoDist;
		int _curObjectsOnCourseCount;
		int _curBlastMarkersAtBase;
		private void CheckSpaceshipCollision(GothicSpaceshipBase spaceship, List<GraphicObject> objectsOnCourse)
		{
			foreach (var possibleCollision in objectsOnCourse) {
				if (CommittedAttacks.Any(a => a.Item1 == possibleCollision && a.Item2 == spaceship) &&
					CommittedAttacks.Any(a => a.Item1 == spaceship && a.Item2 == possibleCollision)) continue;
				
				if (spaceship is TorpedoSalvo || possibleCollision is TorpedoSalvo) {
					TorpedoSalvo torpedo = (spaceship is TorpedoSalvo ? spaceship as TorpedoSalvo : (possibleCollision is TorpedoSalvo ? possibleCollision as TorpedoSalvo : null));
					GraphicObject collisionObject = torpedo == spaceship ? possibleCollision : spaceship;

					double distance = collisionObject.Position.Location.DistanceTo(torpedo.Position.Location);
					if (!CommittedAttacks.Any(a => a.Item1 == collisionObject && a.Item2 == torpedo)) {
						if (collisionObject is GothicSpaceship)
						{
							GothicSpaceship gss = collisionObject as GothicSpaceship;
							double antiTorpedoAttackDistance = torpedo.Size + gss.Diameter * 3;
							_curTorpedoDist = distance;
							_curAntiTorpedoDist = antiTorpedoAttackDistance;
							if (distance < antiTorpedoAttackDistance)
							{
								TurretWeapon turret = gss.Weapons.OfType<TurretWeapon>().FirstOrDefault();
								if (turret != null)
									turret.Attack(torpedo, new List<SpaceshipWeapon>(){turret});
								CommittedAttacks.Add(new Tuple<Spaceship, Spaceship>(gss, torpedo));
							}
						}
					}
					if (!CommittedAttacks.Any(a => a.Item1 == torpedo && a.Item2 == collisionObject)) {
						if (collisionObject is GothicSpaceshipBase)
						{
							GothicSpaceshipBase gssb = collisionObject as GothicSpaceshipBase;
							double torpedoAttackDistance = torpedo.Size / 2 + gssb.Diameter / 2;
							if (distance < torpedoAttackDistance)
							{
								torpedo.Attack(gssb);
								CommittedAttacks.Add(new Tuple<Spaceship, Spaceship>(torpedo, gssb));
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// Attacker - Defender.
		/// </summary>
		internal List<Tuple<Spaceship, Spaceship>> CommittedAttacks = new List<Tuple<Spaceship, Spaceship>>();
		private List<PositioningZone> _zones;
		public void OnDraw(Graphics dc)
		{
			DrawBackground(dc);
			dc.ScaleTransform(CoordinateConverter.Scale, CoordinateConverter.Scale);
			dc.TranslateTransform((float)CoordinateConverter.Translation.X, (float)CoordinateConverter.Translation.Y);
			
			if (this.GameState == GameState.CreatingSpaceship) {
				CurrentPlayer.PositioningZone.Draw(dc);
			}

			//foreach (var torpedo in torpedos) {
			//	torpedo.Draw(dc);
			//}
			
			foreach (var graphicObject in GraphicObjects) {

				if (graphicObject is GothicSpaceship)
				{
					var spaceship = graphicObject as GothicSpaceship;
					//if (spaceship.IsSelected)
					if (spaceship.Player == CurrentPlayer || spaceship.IsSelected)
					{
						if(spaceship.IsDestroyed==CatastrophycDamage.None)
							if (BattlePhase == GamePhase.Movement)
								spaceship.Trajectory.Draw(dc);
					}
					if ( spaceship.IsSelected) {
						//if (BattlePhase == GamePhase.Attack) 
						{
							foreach (var weapon in spaceship.Weapons) {
								if (weapon.IsUsed)
									continue;
								_attackCompass.Draw(dc, weapon.LineColor, spaceship.Position, weapon.SpaceshipSide, weapon.Range);
							}
						}
						if (spaceship.IsDestroyed == CatastrophycDamage.None)
							if (BattlePhase == GamePhase.Movement)
								spaceship.Trajectory.Draw(dc);
					}
				}
				graphicObject.Draw(dc);
			}
			for (int i = 0; i < _animations.Count(); i++)
			{
				_animations[i].Draw(dc);
			}
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
				Font f = new Font("CurierNew", 8, FontStyle.Regular);
				Brush b = new SolidBrush(Color.Lime);
				//dc.DrawString(CoordConverter.CurWinCoord.ToString() + " WinCursor", f, b, new PointF(10, 320));
				//dc.DrawString(CoordConverter.CurGameCoord.ToString() + " GameCursor", f, b, new PointF(10, 335));
				//dc.DrawString(CoordConverter.LastSelectedShipGameCoord.ToString() + " ShipCoord", f, b, new PointF(10, 350));
				//dc.DrawString(CoordConverter.Translation.ToString() + " Translation", f, b, new PointF(10, 365));
				
				//dc.DrawString(curTorpedoDist.ToString() + " torpedo dist to target", f, b, new PointF(10, 365));
				//dc.DrawString(curAntiTorpedoDist.ToString() + " anti-torpedo dist", f, b, new PointF(10, 380));
				//dc.DrawString(curObjectsOnCourseCount.ToString() + " objects on course count", f, b, new PointF(10, 395));
			 
				GamePrinter.Draw(dc, _curBlastMarkersAtBase.ToString() + " blastMarkers at base", new PointF(10, 410), f, b);

				GamePrinter.Draw(dc);
				_frameCounter = (_frameCounter + 1) % 30;
				if (_frameCounter == 0)
				{
					_fps = (int)(1000.0 / TimerStep.TotalMilliseconds);
				}
				GamePrinter.Draw(dc, "fps: " + _fps.ToString(), new PointF(500, 0), f, b);
			}
			

			Cursor.Draw(dc); 
			if (GameState != GameState.SelectingPoint)
				Cursor.SetForm(CursorForm.Default);
		}
		int _fps = 0;
		int _frameCounter = 0;
		internal GameCursor Cursor;
		internal CoordinateConverter CoordinateConverter;
		Bitmap _background = Properties.Resources.background;
		private void DrawBackground(Graphics dc)
		{
			dc.DrawImage(_background, 0, 0, _background.Width, _background.Height);
			//dc.DrawImageUnscaled(b, 0, 0, b.Width, b.Height);
			
		}
		public void MouseMove(Point2d point, System.Windows.Forms.MouseButtons button)
		{
			CoordinateConverter.CurWinCoord = point;
			if (button == System.Windows.Forms.MouseButtons.None) {
				var gameCsPoint = CoordinateConverter.GetGameCoord(point);
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
			else if (button == System.Windows.Forms.MouseButtons.Right) {
				CoordinateConverter.FinishFieldDrag(point);
			}
		}
		public void OnKeyDown(System.Windows.Forms.KeyEventArgs key)
		{
			switch (key.KeyCode)
			{
				case System.Windows.Forms.Keys.D1: break;
			}
		}
		public void OnMouseDown(Point2d coord, System.Windows.Forms.MouseButtons button)
		{
			if (button == System.Windows.Forms.MouseButtons.Right) {
				CoordinateConverter.StartFieldDrag(coord);
			}
		}
		public void OnMouseClick(Point2d coord, System.Windows.Forms.MouseButtons button)
		{
			var gameCoord = CoordinateConverter.GetGameCoord(coord);
			switch (GameState) {
				case GameState.Battle:
					if (button == System.Windows.Forms.MouseButtons.Left) {
						if (TrySelectSpaceship(gameCoord, out var newSelectedSpaceship)) {
							if (newSelectedSpaceship is GothicSpaceship)
							{
								if (SelectedSpaceship == null)
								{
									SelectedSpaceship = newSelectedSpaceship as GothicSpaceship;
									SelectedSpaceship.IsSelected = true;
								}
								else if (SelectedSpaceship != newSelectedSpaceship)
								{
									SelectedSpaceship.IsSelected = false;
									SelectedSpaceship = newSelectedSpaceship as GothicSpaceship;
									SelectedSpaceship.IsSelected = true;
								}
							}
							CoordinateConverter.LastSelectedShipGameCoord = SelectedSpaceship.Position.Location;
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
							if (button == System.Windows.Forms.MouseButtons.Right) {
								if (CoordinateConverter.IsDraggingField) {
									if (SelectedSpaceship != null) {
										if (SelectedSpaceship.State == SpaceshipState.Standing) {
											if (SelectedSpaceship.Player == CurrentPlayer)
												SelectedSpaceship.MoveTo(gameCoord);
										}
									}
								}
							}
							break;
						case GamePhase.Attack:
							if (button == System.Windows.Forms.MouseButtons.Right) {
								if (SelectedSpaceship != null && SelectedSpaceship.Player == CurrentPlayer) {
									if (TrySelectSpaceship(gameCoord, out var attackedSpaceship)) {
										if (attackedSpaceship!= SelectedSpaceship && (Params.FriendlyFire || attackedSpaceship.Player != CurrentPlayer)) {
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
						default:
							break;
					}
					break;
				case GameState.SelectingShips:
					break;
				case GameState.CreatingSpaceship:
					CurrentPlayer.PositioningZone.OnMouseClick(gameCoord);
					break;
				case GameState.SelectingPoint:
					EndPointSelect(gameCoord);
					break;
				default:
					break;
			}
		}
		private enum Races
		{
			ImperialMarine = 1,
			ChaoseMarine = 2
		}

		public void StartFleetPositioning()
		{
			this.GameState = GameState.CreatingSpaceship;
			OnStartPositioningPhase(this,new NextPlayerEventArgs(CurrentPlayer));
			CurrentPlayer.PositioningZone.StartPositioning();
		}
		public void StartGame()
		{
			StartFleetPositioning();
		}
		public void StartBattle()
		{
			this.GameState = GameState.Battle;
			EndBattlePhase();
		}
		public Player NextPlayer(Player currentPlayer)
		{
			int currentPlayerIndex = currentPlayer != null ? Players.IndexOf(currentPlayer) : -1;
			return Players[(currentPlayerIndex + 1) % Players.Count];
		}

		bool _isAutoCompleting = false;
		GamePhase _battlePhase = GamePhase.Ending;
		internal GamePhase BattlePhase
		{
			get { return _battlePhase; }
			set
			{
				if (_battlePhase != value)
				{
					_battlePhase = value;
					NotifyPropertyChanged("BattlePhase");
					if (_selectedSpaceship != null)
						_selectedSpaceship.NotifyPropertyChanged("AvailableOrders");
				}
			}
		}
		private void AdvanceGame()
		{
			switch (BattlePhase) {
				case GamePhase.Movement:
					foreach (var ss in CurrentPlayer.Spaceships.ToList())
					{
						if (ss.IsDestroyed== CatastrophycDamage.BlazingHulk)
						{
							Point2d bmsPos = new Point2d(ss.Position.Angle + GeometryHelper.Pi, ss.Diameter / 2).ToEuclidCs(ss.Position);
							BlastMarker bm = new BlastMarker(this, new Position(bmsPos, GeometryHelper.Pi), new TimeSpan());
							ss.BlastMarkersAtBase.AddBlastMarker(bm);
							this.AddGraphicObject(bm);
						}
					}
					BattlePhase = GamePhase.Attack;
					break;
				case GamePhase.Attack:
					BattlePhase = GamePhase.Ordnance;
					foreach (var torpedo in TorpedoSalvos) {
						torpedo.Trajectory = new TrajectoryCollection(torpedo);
						GothicTrajectory trajectory = new GothicTrajectory(
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
					DisposeUselessTorpedos();

					UpdateSpaceshipsTrajectories();

					ResetWeaponUsage();

					CurrentPlayer = NextPlayer(CurrentPlayer);
					CurrentPlayer.SpecialOrderFail = false;
					foreach (var ss in GothicSpaceships.Where(a=>a.Player==CurrentPlayer)) {
						ss.MovementStarted = false;
					}
					CommittedAttacks.Clear();

					foreach (var kvp in MoveDistanceInLastTurn.Where(a => a.Key.Player == CurrentPlayer)) {
						MoveDistanceInLastTurn[kvp.Key] = 0;
					}

					ResetSpaceshipSelection();
					CompletedTurnsCount++;
					if (Scenario.IsComplete())
						GameState = GameState.End;
					OnNextTurn(this, new NextPlayerEventArgs(CurrentPlayer));
					break;
				default:
					break;
			}
			OnNextBattlePhase(this, new NextBattlePhaseEventArgs(CurrentPlayer, BattlePhase));
		}
		public Scenario Scenario;
		private void DisposeUselessTorpedos()
		{
			foreach (var torpedo in TorpedoSalvos)
			{
				double minDistToSpaceship = double.PositiveInfinity;
				foreach (var ss in SpaceBodies)
				{
					if (ss == torpedo || ss is TorpedoSalvo)
						continue;
					double dist = torpedo.Position.Location.DistanceTo(ss.Position.Location);
					if (dist < minDistToSpaceship)
						minDistToSpaceship = dist;
				}
				if (minDistToSpaceship > 60)
					torpedo.State = SpaceshipState.Removed;
			}
		}
	
		private void ResetSpaceshipSelection()
		{
			if (SelectedSpaceship != null)
			{
				SelectedSpaceship.IsSelected = false;
				SelectedSpaceship = null;
			}
		}

		private void ResetWeaponUsage()
		{
			foreach (var spaceship in GothicSpaceships)
			{
				if (spaceship.Player != CurrentPlayer)
					continue;
				foreach (var weapon in spaceship.Weapons)
				{
					if (!(weapon is TorpedoWeapon))
						weapon.IsUsed = false;
				}
			}
		}

		private void UpdateSpaceshipsTrajectories()
		{
			BattlePhase = GamePhase.Movement;
			foreach (var spaceship in GothicSpaceships)
			{
				spaceship.Trajectory = new TrajectoryCollection(spaceship);

				var curTrajectory = new GothicTrajectory(
						spaceship.Trajectory,
						spaceship.Position,
						spaceship.Speed,
						spaceship.MinRunBeforeTurn,
						spaceship.MaxTurnAngle);
				if (spaceship.BlastMarkersAtBase.Any())
				{
					curTrajectory.CutDistanceToMove(Params.BlastMarkerSpeedDamage);
				}
				spaceship.Trajectory.Add(curTrajectory);

				spaceship.State = SpaceshipState.Standing;
				spaceship.SpecialOrder = GothicOrder.None;				
			}
		}

		private void FixSpaceships()
		{
			foreach (var spaceship in GothicSpaceships.ToList())
			{
				spaceship.RestoreCriticalDamage();
				spaceship.ApplyCriticalDamageConsequences();
			}
		}

		private void RemoveOldBlastMarkers()
		{
			var possiblyRemovedBms = new List<BlastMarker>();
			foreach (var blastMarker in BlastMarkers)
			{
				bool isAtSpaceshipBase = false;
				foreach (var ss in SpaceBodies.Where(a=>a.State== SpaceshipState.Moving || a.State== SpaceshipState.Standing))
				{
					if (ss.BlastMarkersAtBase.Contains(blastMarker))
					{
						isAtSpaceshipBase = true;
						break;
					}
				}
				if (!isAtSpaceshipBase) { possiblyRemovedBms.Add(blastMarker); }
			}

			int removedBmsCount = Dice.RollDices(6, 1, "Максимальное количество удаленных маркеров взрыва.");
			var bmsToRemove = possiblyRemovedBms.GetRandomItems(removedBmsCount);
			foreach (var blastMarker in bmsToRemove)
			{
				_graphicObjects.Remove(blastMarker);
				blastMarker.Dispose();
			}
		}

		public void EndBattlePhase()
		{

			if (BattlePhase == GamePhase.Movement) {
				//Unselect spaceships.
				_isAutoCompleting = false;
				foreach (var spaceship in CurrentPlayer.Spaceships) {
					if (spaceship.State == SpaceshipState.Moving){
						_isAutoCompleting = true;
						spaceship.MovementStarted = true;
						//break;
					}
					else if (spaceship.TryAutoMoveMandatoryDistance()) {
						_isAutoCompleting = true;
					}
				}
				if (!_isAutoCompleting) {
					foreach (var spaceship in CurrentPlayer.Spaceships)
					{
						spaceship.Trajectory=null;
					}
				}
			}
			else if (BattlePhase == GamePhase.Attack) {
			}
			else if (BattlePhase == GamePhase.Ordnance) {
				_isAutoCompleting = false;
				Player curOrdnancePlayer = CurrentPlayer;
				do {
					foreach (var ordnance in TorpedoSalvos.Where(a => a.Player == curOrdnancePlayer)) {
						if (ordnance.State == SpaceshipState.Moving || ordnance.TryAutoMoveMandatoryDistance()) {
							_isAutoCompleting = true;
							//break;
						}
					}
					if (!_isAutoCompleting) {
						curOrdnancePlayer = NextPlayer(curOrdnancePlayer);
					}
					else break;
				} while (curOrdnancePlayer!=CurrentPlayer);
				if (!_isAutoCompleting) {
					foreach (var ordnance in TorpedoSalvos) {
						ordnance.Trajectory = null;
					}
				}
			}
			else if (BattlePhase == GamePhase.Ending) {
			}
			else
				throw new InvalidOperationException("Недопустимое состояние игры");
			
			if (!_isAutoCompleting) {
				AdvanceGame();
			}
		}
		public event EventHandler<NextPlayerEventArgs> NextTurn;
		protected void OnNextTurn(object sender, NextPlayerEventArgs e)
		{
			if (NextTurn != null) {
				NextTurn(sender, e);
			}
		}
		internal static Random Rand = new Random(DateTime.Now.Millisecond);

		
		private bool TrySelectSpaceship(Point2d coord, out GothicSpaceshipBase selectedSpaceship)
		{
			//double spaceshipRadiusSqr = SpaceshipDiameter * SpaceshipDiameter * Game.DistanceCoef * Game.DistanceCoef / 4;
			double spaceshipRadiusSqr = Params.SpaceshipDiameter * Params.SpaceshipDiameter / 4;
			List<Tuple<GothicSpaceshipBase, double>> possiblySelectedSpaceships = new List<Tuple<GothicSpaceshipBase, double>>();
			foreach (var spaceship in SpaceBodies)
			{
				double distanceSqrToSpaceship = coord.DistanceSqrTo(spaceship.Position.Location);
				if (distanceSqrToSpaceship <= spaceshipRadiusSqr)
					possiblySelectedSpaceships.Add(new Tuple<GothicSpaceshipBase, double>(spaceship, distanceSqrToSpaceship));
			}
			if (possiblySelectedSpaceships.Count == 0) {
				selectedSpaceship = null;
				return false;
			}
			else {
				Tuple<GothicSpaceshipBase, double> minDistanceSpaceship = possiblySelectedSpaceships.First();
				foreach (var spaceship in possiblySelectedSpaceships) {
					if (minDistanceSpaceship.Item2 > spaceship.Item2) minDistanceSpaceship = spaceship;
				}
				selectedSpaceship = minDistanceSpaceship.Item1;
				return true;
			}
		}
		GothicSpaceship _selectedSpaceship;
		public GothicSpaceship SelectedSpaceship
		{
			get { return _selectedSpaceship; }
			internal set
			{
				if (_selectedSpaceship != value)
				{
					_selectedSpaceship = value;
					NotifyPropertyChanged("SelectedSpaceship");
				}
			}
		}
		public event PropertyChangedEventHandler PropertyChanged;
		protected virtual void NotifyPropertyChanged(string propertyName)
		{
			var handler = PropertyChanged;
			if (handler != null)
			{
				handler(this, new PropertyChangedEventArgs(propertyName));
			}
		}
		internal class GothicGameParams
		{
			internal bool FriendlyFire { get { return true; } }
			internal GothicGameParams(Game game)
			{
				Game = game;
			}
			internal Game Game { get; private set; }
			internal int SpaceshipDiameter { get { return 3; } }
			internal float TorpedoeSize { get { return 2.5F; } }
			internal int TrajectorycAnchorPointRadius { get { return 4; } }
			internal float SelectionThickness { get { return 3 / Game.CoordinateConverter.Scale; } }
			internal float TrajectoryLineThickness { get { return 3 / Game.CoordinateConverter.Scale; } }
			internal float TrajectoryAnchorDistance { get { return 3 / Game.CoordinateConverter.Scale; } }
			internal float ActiveTrajectoryThicknes { get { return 3 / Game.CoordinateConverter.Scale; } }
			internal float AttackCompassThickness { get { return 2 / Game.CoordinateConverter.Scale; } }
			internal Color AttackCompassColor { get { return Color.Orange; } }
			internal Color PositioningZoneColor { get { return Color.Lime; } }
			internal Color ActiveTrajectoryColor { get { return Color.Orange; } }
			internal Color SpaceshipSelectionColor { get { return Color.Gold; } }
			internal Color TrajectoryColor { get { return Color.Green; } }
			internal Color SelectedTrajectoryColor { get { return Color.Lime; } }
			internal Color MandatoryTrajectoryColor { get { return Color.Firebrick; } }
			internal Color SelectedMandatoryTrajectoryColor { get { return Color.Red; } }
			internal float BlastMarkerDiameter { get { return (float)SpaceshipDiameter * 0.9F; } }

			public Color HitPointsGaugeColor { get { return Color.Red; } }
			public Color ShieldGaugeColor { get { return Color.Cyan; } }

			public bool Debug { get { return false; } }

			public double BlastMarkerSpeedDamage { get { return 5; } }
		}

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



		public int CompletedTurnsCount { get; set; }
	}
	public enum GameState
	{
		//Paused,
		Battle,
		Attack,
		SelectingShips,
		CreatingSpaceship,
		Menu,
		SelectingPoint,
		End
	}
		
	public class NextPlayerEventArgs : EventArgs
	{
		public Player Player;
		public NextPlayerEventArgs(Player p)
		{
			Player = p;
		}
	}
	public class AllShipsStoppedEventArgs : EventArgs
	{
		public ShipsStopCause StopCause;
		public AllShipsStoppedEventArgs(ShipsStopCause p)
		{
			StopCause = p;
		}
	}

	public enum ShipsStopCause
	{
		Autocompletion
	}
	public enum GamePhase
	{
		Movement,
		Attack,
		Ordnance,
		Ending
	}
}

