using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;

namespace SpaceStrategy
{
	public enum GothicOrder
	{
		None,
		AllAheadFull,
		ComeToNewDirection,
		BurnRetros,
		LockOn,
		BraceForImpact,
		ReloadOrdnance,
		LaunchOrdnance
	}


	[Flags]
	enum GothicTrajectoryParts
	{
		None = 0,
		Area = 1,
		CenterLine = 2,
		MandatoryPartEnd = 4,
		MaxDistance = 8,
		FirstTurnDistance = 16,
		SecondTurnDistance = 32,
		AreaLeftSide = 64,
		AreaRightSide = 128,
		MandatoryPart = 256
	}


	class GothicTrajectory : LinearTrajectory
	{
		readonly List<Point2d> _trajectoryPoints = new List<Point2d>();
		GothicTrajectoryParts _highlightedParts;

		Point2d? _highlightPoint;

		GothicOrder _previewedOrder;

		bool _specialOrderIsSet;

		public GothicTrajectory(TrajectoryCollection owner, Position startPosition, double maxDistance, double minRunBeforeTurn, double maxTurnAngle)
			: base(owner, startPosition.Location, startPosition.Location + startPosition.Direction * maxDistance)
		{
			MaxDistance = maxDistance;
			MinRunBeforeTurn = minRunBeforeTurn;
			MaxTurnAngle = maxTurnAngle;
			MaxTurnCount = 1;
			MandatoryDistance = MaxDistance / 2;
			SpecialOrderIsPossible = true;
		}

		public GothicTrajectory(TrajectoryCollection owner, Position startPosition, double maxDistance, double minRunBeforeTurn, double maxTurnAngle, double mandatoryDistance)
			: this(owner, startPosition, maxDistance, minRunBeforeTurn, maxTurnAngle)
		{
			MandatoryDistance = mandatoryDistance;
		}

		public Game Game
		{
			get { return Owner.Spaceship.Game; }
		}

		public double MandatoryDistance { get; private set; }

		public double MaxDistance { get; private set; }

		public int MaxTurnCount { get; private set; }

		public double MinRunBeforeTurn { get; private set; }

		public bool SpecialOrderIsPossible { get; private set; }

		internal GothicOrder SpecialOrder { get; set; }

		double MaxTurnAngle { get; }

		public void CutDistanceToMove(double distanceToCut)
		{
			MaxDistance = Math.Max(0, MaxDistance - distanceToCut);
		}

		public override void Draw(Graphics dc)
		{
			if (!IsVisible) {
				return;
			}

			GothicTrajectoryParams p = GetTrajectoryParams(_previewedOrder);

			if (Owner.Spaceship.State == SpaceshipState.Standing) {
				GraphicsState oldDc = dc.Save();
				dc.TranslateTransform((float)GetCurrentPosition().Location.X, (float)GetCurrentPosition().Location.Y);
				dc.RotateTransform((float)GetCurrentPosition().Degree);

				if (MaxTurnCount > 0 && p.TurnIsPossibleAtDistance < p.DistanceToMove && p.DistanceToMove > 0.1) {
					if (p.TurnIsPossibleAtDistance < p.MandatoryDistance + p.OptionalDistance) {
						Color maxDistColor = _highlightedParts.HasFlag(GothicTrajectoryParts.MaxDistance) ? Game.Params.SelectedTrajectoryColor : Game.Params.TrajectoryColor;
						Color leftSideColor = _highlightedParts.HasFlag(GothicTrajectoryParts.AreaLeftSide) ? Game.Params.SelectedTrajectoryColor : Game.Params.TrajectoryColor;
						Color rightSideColor = _highlightedParts.HasFlag(GothicTrajectoryParts.AreaRightSide) ? Game.Params.SelectedTrajectoryColor : Game.Params.TrajectoryColor;
						Color fillColor = _highlightedParts.HasFlag(GothicTrajectoryParts.Area) ? Game.Params.SelectedTrajectoryColor : Game.Params.TrajectoryColor;

						DrawMovementZone(dc, rightSideColor, leftSideColor, maxDistColor, fillColor, p.MandatoryDistance + p.OptionalDistance - p.TurnIsPossibleAtDistance, p.TurnIsPossibleAtDistance);
					}

					if (p.TurnIsPossibleAtDistance < p.MandatoryDistance) {
						Color maxDistColor = _highlightedParts.HasFlag(GothicTrajectoryParts.MandatoryPartEnd) ? Game.Params.SelectedMandatoryTrajectoryColor : Game.Params.MandatoryTrajectoryColor;
						Color leftSideColor = _highlightedParts.HasFlag(GothicTrajectoryParts.AreaLeftSide) ? Game.Params.SelectedMandatoryTrajectoryColor : Game.Params.MandatoryTrajectoryColor;
						Color rightSideColor = _highlightedParts.HasFlag(GothicTrajectoryParts.AreaRightSide) ? Game.Params.SelectedMandatoryTrajectoryColor : Game.Params.MandatoryTrajectoryColor;
						Color fillColor = _highlightedParts.HasFlag(GothicTrajectoryParts.Area) ? Game.Params.SelectedMandatoryTrajectoryColor : Game.Params.MandatoryTrajectoryColor;

						DrawMovementZone(dc, rightSideColor, leftSideColor, maxDistColor, fillColor, p.MandatoryDistance - p.TurnIsPossibleAtDistance, p.TurnIsPossibleAtDistance);
					}
				}

				if (p.MandatoryDistance > 0) {
					Color lineColor = _highlightedParts.HasFlag(GothicTrajectoryParts.CenterLine) ? Game.Params.SelectedMandatoryTrajectoryColor : Game.Params.MandatoryTrajectoryColor;
					dc.DrawLine(new Pen(lineColor, Game.Params.TrajectoryLineThickness), 0, 0, p.MandatoryDistance, 0);
				}

				if (p.OptionalDistance > 0) {
					Color lineColor = _highlightedParts.HasFlag(GothicTrajectoryParts.CenterLine) ? Game.Params.SelectedTrajectoryColor : Game.Params.TrajectoryColor;
					dc.DrawLine(new Pen(lineColor, Game.Params.TrajectoryLineThickness), p.MandatoryDistance, 0, p.MandatoryDistance + p.OptionalDistance, 0);
				}

				if (p.RandomDistance > 0) {
					var randomDistancePen = new Pen(Game.Params.MandatoryTrajectoryColor, Game.Params.TrajectoryLineThickness);
					randomDistancePen.DashStyle = DashStyle.Dash;
					dc.DrawLine(randomDistancePen, p.MandatoryDistance + p.OptionalDistance, 0, p.MandatoryDistance + p.OptionalDistance + p.RandomDistance, 0);
				}

				if (_highlightPoint.HasValue) {
					Point2d globalCsPoint = _highlightPoint.Value.TransformBy(Position);
					if (_highlightedParts.HasFlag(GothicTrajectoryParts.MandatoryPart)) {
						Game.Cursor.SetForm(CursorForm.TrajectoryMandatory);
					}
					else {
						Game.Cursor.SetForm(CursorForm.TrajectoryOptional);
					}

					Game.Cursor.SetLocation(Game.CoordinateConverter.GetWinCsPoint(globalCsPoint));
				}

				dc.Restore(oldDc);
			}
			else if (Owner.Spaceship.State == SpaceshipState.Moving) {
				var pen = new Pen(Game.Params.ActiveTrajectoryColor, Game.Params.ActiveTrajectoryThickness);
				for (int i = 0; i < _trajectoryPoints.Count - 1; i++) {
					var startP = new PointF((float)_trajectoryPoints[i].X, (float)_trajectoryPoints[i].Y);
					var endP = new PointF((float)_trajectoryPoints[i + 1].X, (float)_trajectoryPoints[i + 1].Y);
					dc.DrawLine(pen, startP, endP);
				}

				_highlightPoint = null;
			}
		}

		public override Position GetPositionAt(double distance)
		{
			double curDist = 0;
			for (int i = 0; i < _trajectoryPoints.Count - 1; i++) {
				curDist += _trajectoryPoints[i].DistanceTo(_trajectoryPoints[i + 1]);
				if (curDist > distance) {
					Vector curDir = _trajectoryPoints[i].VectorTo(_trajectoryPoints[i + 1]);
					curDir.Normalize();
					Point2d resultPoint = _trajectoryPoints[i + 1] - curDir * (curDist - distance);
					return new Position(resultPoint, curDir);
				}
			}

			if (_trajectoryPoints.Count == 0) {
				return Position;
			}

			Vector lasDir = _trajectoryPoints[_trajectoryPoints.Count - 2].VectorTo(_trajectoryPoints[_trajectoryPoints.Count - 1]);
			lasDir.Normalize();
			return new Position(_trajectoryPoints.Last(), lasDir);
		}

		public void PreviewSpecialOrder(GothicOrder order)
		{
			if (!_specialOrderIsSet) {
				_previewedOrder = order;
			}
		}

		public void SetSpecialOrder(GothicOrder order)
		{
			if (_specialOrderIsSet) {
				return;
			}

			_specialOrderIsSet = true;

			//PreviewSpecialOrder(GothicOrder.NormalMove);

			switch (order) {
				case GothicOrder.None:
				case GothicOrder.ReloadOrdnance:
				case GothicOrder.BraceForImpact:
					break;

				case GothicOrder.AllAheadFull:
					var ownerSpaceship = Owner.Spaceship as GothicSpaceship;

					MaxDistance = (float)(MaxDistance + Dice.RollDices(6, ownerSpaceship.Class.AllAheadFullMultiplier, "Дополнительная дальность"));
					MaxTurnCount = 0;
					MandatoryDistance = MaxDistance;
					break;

				case GothicOrder.ComeToNewDirection:
					MaxTurnCount = 2;
					break;

				case GothicOrder.BurnRetros:
					MandatoryDistance = 0;
					MaxDistance = MaxDistance / 2;
					MinRunBeforeTurn = 0;
					break;

				case GothicOrder.LockOn:
					MaxTurnCount = 0;
					break;
			}

			SpecialOrder = order;
			_previewedOrder = order;
		}

		internal override void AddToCurrentDistance(double distance, out double unusedDistance)
		{
			if (DistanceFromStart > MaxDistance) {
				Length = DistanceFromStart;
			}

			base.AddToCurrentDistance(distance, out unusedDistance);
			if (unusedDistance > 0) {
				Owner.Spaceship.State = SpaceshipState.Standing;
				Position = GetCurrentPosition();
				StartPoint = Position.Location;
				Direction = Position.Direction;
				MaxDistance = MaxDistance - DistanceFromStart;
				MandatoryDistance = Math.Max(0, MandatoryDistance - DistanceFromStart);
				DistanceFromStart = 0;
				_trajectoryPoints.Clear();
			}
		}

		internal bool Autocomplete()
		{
			var gothicSpaceshipBase = Owner.Spaceship as GothicSpaceshipBase;
			if (gothicSpaceshipBase != null) {
				GothicTrajectoryParams p = GetTrajectoryParams();
				if (p.MandatoryDistance > 0.001) {
					double distanceToMove = Math.Min(p.MandatoryDistance, p.DistanceToMove);
					Position curPos = Owner.Spaceship.Position;
					Point2d newPos = curPos.Location + curPos.Direction * distanceToMove;
					gothicSpaceshipBase.MoveTo(newPos);
					return true;
				}
			}

			return false;
		}

		internal override bool IsOnCourse(Point2d point, double maxDistFromCourse)
		{
			GothicTrajectoryParams p = GetTrajectoryParams();
			Point2d spaceshipCsPoint = point.UnTransformBy(Owner.Spaceship.Position);

			//Center line
			if (Math.Abs(spaceshipCsPoint.Y) < maxDistFromCourse && spaceshipCsPoint.X >= 0 && spaceshipCsPoint.X <= p.DistanceToMove) {
				return true;
			}

			return false;
		}

		internal bool MoveTo(Point2d point)
		{
			HitTestResult hitResult = HitTest(point);
			if (hitResult.AnchoredPoint.HasValue) {
				SpecialOrderIsPossible = false;
				_trajectoryPoints.Clear();

				_trajectoryPoints.Add(StartPoint);
				Point2d endPoint = hitResult.AnchoredPoint.Value.TransformBy(Position);
				_trajectoryPoints.Add(endPoint);

				if (_highlightedParts.HasFlag(GothicTrajectoryParts.Area)) {
					GothicTrajectoryParams p = GetTrajectoryParams();

					if (p.TurnIsPossibleAtDistance >= 0) {
						Point2d startOfAfterTurnPart = new Point2d(p.TurnIsPossibleAtDistance, 0).TransformBy(Position);
						if (startOfAfterTurnPart != StartPoint && startOfAfterTurnPart != endPoint) {
							_trajectoryPoints.Insert(1, startOfAfterTurnPart);
							Vector part1Dir = _trajectoryPoints[0].VectorTo(_trajectoryPoints[1]);
							Vector part2Dir = _trajectoryPoints[1].VectorTo(_trajectoryPoints[2]);
							part1Dir.Normalize();
							part2Dir.Normalize();
							if (part1Dir != part2Dir) {
								MaxTurnCount--;
							}
						}
						else {
							Vector part1Dir = _trajectoryPoints[0].VectorTo(_trajectoryPoints[1]);
							part1Dir.Normalize();
							if (part1Dir != Position.Direction) {
								MaxTurnCount--;
							}
						}
					}
				}

				double totalTrajectoryLength = 0;
				for (int i = 0; i < _trajectoryPoints.Count - 1; i++) totalTrajectoryLength += _trajectoryPoints[i].DistanceTo(_trajectoryPoints[i + 1]);
				Length = totalTrajectoryLength;
				return true;
			}

			return false;
		}

		internal override void OnMouseMove(Point2d point)
		{
			HitTestResult hitResult = HitTest(point);
			_highlightPoint = hitResult.AnchoredPoint;
			_highlightedParts = hitResult.HighlightedParts;
		}

		void DrawMovementZone(Graphics dc, Color rightSideLineColor, Color leftSideLineColor, Color maxDistanceColor, Color fillColor, float zoneDistance, float turnIsPossibleAtDistance)
		{
			if (zoneDistance > 0.001) {
				float maxY = zoneDistance * (float)GeometryHelper.Sin(MaxTurnAngle * GeometryHelper.Pi / 180);
				float maxX = zoneDistance * (float)GeometryHelper.Cos(MaxTurnAngle * GeometryHelper.Pi / 180);
				var leftSidePen = new Pen(leftSideLineColor, Game.Params.TrajectoryLineThickness);
				var rightSidePen = new Pen(rightSideLineColor, Game.Params.TrajectoryLineThickness);
				var maxDistPen = new Pen(maxDistanceColor, Game.Params.TrajectoryLineThickness);
				int alphaChannelIntensity = 64;
				Brush transparentBrush = new SolidBrush(Color.FromArgb(alphaChannelIntensity, fillColor));
				GraphicsState dcState = dc.Save();
				dc.TranslateTransform(turnIsPossibleAtDistance, 0);
				dc.FillPie(transparentBrush, -zoneDistance, -zoneDistance, 2 * zoneDistance, 2 * zoneDistance, -(float)MaxTurnAngle, (float)(2 * MaxTurnAngle));
				dc.DrawLine(rightSidePen, 0, 0, maxX, maxY);
				dc.DrawLine(leftSidePen, 0, 0, maxX, -maxY);

				dc.DrawArc(maxDistPen, -zoneDistance, -zoneDistance, 2 * zoneDistance, 2 * zoneDistance, -(float)MaxTurnAngle, (float)(2 * MaxTurnAngle));
				dc.Restore(dcState);
			}
		}

		GothicTrajectoryParams GetTrajectoryParams()
		{
			return GetTrajectoryParams(SpecialOrder);
		}

		GothicTrajectoryParams GetTrajectoryParams(GothicOrder specialOrder)
		{
			float mandatoryDistance;
			float distanceToMove;
			float optionalDistance;
			float randomDistance;
			float turnIsPossibleAtDistance;

			switch (specialOrder) {
				case GothicOrder.AllAheadFull:
					mandatoryDistance = (float)(MaxDistance - DistanceFromStart);
					if (mandatoryDistance < 0) {
						mandatoryDistance = 0;
					}

					distanceToMove = (float)(MaxDistance - DistanceFromStart);
					optionalDistance = 0;
					randomDistance = _specialOrderIsSet ? 0 : 24;
					turnIsPossibleAtDistance = float.PositiveInfinity; //turn is not possible
					break;

				case GothicOrder.BurnRetros:
					mandatoryDistance = 0;

					distanceToMove = (float)((_specialOrderIsSet ? MaxDistance : MaxDistance / 2) - DistanceFromStart);
					optionalDistance = distanceToMove;
					randomDistance = 0;
					turnIsPossibleAtDistance = 0;
					break;

				case GothicOrder.LockOn:
					mandatoryDistance = (float)(MandatoryDistance - DistanceFromStart);
					distanceToMove = (float)(MaxDistance - DistanceFromStart);
					optionalDistance = distanceToMove - mandatoryDistance;
					randomDistance = 0;
					turnIsPossibleAtDistance = float.PositiveInfinity; //turn is not possible
					break;

				case GothicOrder.LaunchOrdnance:
					mandatoryDistance = 5000;
					distanceToMove = mandatoryDistance = 5000;
					optionalDistance = 0;
					randomDistance = 0;
					turnIsPossibleAtDistance = 0;
					break;

				default:
				case GothicOrder.ComeToNewDirection:
				case GothicOrder.BraceForImpact:
				case GothicOrder.ReloadOrdnance:
					mandatoryDistance = (float)(MandatoryDistance - DistanceFromStart);
					distanceToMove = (float)(MaxDistance - DistanceFromStart);
					optionalDistance = distanceToMove - mandatoryDistance;
					randomDistance = 0;
					turnIsPossibleAtDistance = (float)Math.Max(MinRunBeforeTurn - Owner.DistanceAfterLastTurn, 0);
					break;
			}

			return new GothicTrajectoryParams(mandatoryDistance, distanceToMove, optionalDistance, randomDistance, turnIsPossibleAtDistance);
		}

		HitTestResult HitTest(Point2d point)
		{
			var highlightedParts = GothicTrajectoryParts.None;

			GothicTrajectoryParams p = GetTrajectoryParams();
			Point2d spaceshipCsPoint = point.UnTransformBy(Position);

			//Center line
			if (Math.Abs(spaceshipCsPoint.Y) <= Game.Params.TrajectoryAnchorDistance && spaceshipCsPoint.X >= 0 && spaceshipCsPoint.X <= p.DistanceToMove) {
				highlightedParts |= GothicTrajectoryParts.CenterLine;
			}

			//Mandatory distance end
			if (p.MandatoryDistance <= p.TurnIsPossibleAtDistance) {
				if (Math.Abs(spaceshipCsPoint.X - p.MandatoryDistance) < Game.Params.TrajectoryAnchorDistance &&
					Math.Abs(spaceshipCsPoint.Y) < Game.Params.TrajectoryAnchorDistance) {
					highlightedParts |= GothicTrajectoryParts.MandatoryPartEnd;
				}
			}

			double distanceAfterTurn = p.DistanceToMove - p.TurnIsPossibleAtDistance;
			double maxAngleRadian = MaxTurnAngle * GeometryHelper.Pi / 180;
			double xMax = distanceAfterTurn * GeometryHelper.Cos(maxAngleRadian);
			double yMax = distanceAfterTurn * GeometryHelper.Sin(maxAngleRadian);
			var turnOriginCsPoint = new Point2d(spaceshipCsPoint.X - p.TurnIsPossibleAtDistance, spaceshipCsPoint.Y);
			double radCur = new Vector(turnOriginCsPoint.X, turnOriginCsPoint.Y).ToRadian();
			double xCur = distanceAfterTurn * GeometryHelper.Cos(radCur);
			double yCur = distanceAfterTurn * GeometryHelper.Sin(radCur);
			double mandatoryDistanceAfterTurn = p.MandatoryDistance - p.TurnIsPossibleAtDistance;
			double distanceAfterTurnToPoint = turnOriginCsPoint.ToVector().Length;

			if (p.TurnIsPossibleAtDistance <= p.DistanceToMove) {
				//First turn distance
				if (Math.Abs(spaceshipCsPoint.X) < Game.Params.TrajectoryAnchorDistance &&
					Math.Abs(spaceshipCsPoint.Y) < Game.Params.TrajectoryAnchorDistance) {
					highlightedParts |= GothicTrajectoryParts.FirstTurnDistance;
				}

				//Mandatory distance end arc
				if (mandatoryDistanceAfterTurn > 0) {
					if (Math.Abs(Math.Sqrt(turnOriginCsPoint.X * turnOriginCsPoint.X + turnOriginCsPoint.Y * turnOriginCsPoint.Y) - mandatoryDistanceAfterTurn) < Game.Params.TrajectoryAnchorDistance) {
						if (Math.Abs(new Vector(turnOriginCsPoint.X, turnOriginCsPoint.Y).ToRadian()) < maxAngleRadian) {
							highlightedParts |= GothicTrajectoryParts.MandatoryPartEnd;
						}
					}
				}

				if (Math.Abs(new Vector(turnOriginCsPoint.X, turnOriginCsPoint.Y).ToRadian()) <= maxAngleRadian) {
					//Area
					if (distanceAfterTurnToPoint < distanceAfterTurn - Game.Params.TrajectoryAnchorDistance) {
						highlightedParts |= GothicTrajectoryParts.Area;
					}

					//Max distance line
					else if (Math.Abs(distanceAfterTurnToPoint - distanceAfterTurn) < Game.Params.TrajectoryAnchorDistance) {
						highlightedParts |= GothicTrajectoryParts.MaxDistance;
					}
				}

				//SideLines
				if (0 <= turnOriginCsPoint.X && turnOriginCsPoint.X <= xMax ||
					Math.Abs(turnOriginCsPoint.X) < Game.Params.TrajectoryAnchorDistance) {
					if (MaxTurnAngle != 90) {
						double k = Math.Sign(turnOriginCsPoint.Y) * Math.Tan(maxAngleRadian);
						double y = k * turnOriginCsPoint.X;
						if (Math.Abs(turnOriginCsPoint.Y - y) < Game.Params.TrajectoryAnchorDistance) {
							if (k > 0) {
								highlightedParts |= GothicTrajectoryParts.AreaRightSide;
							}
							else if (k < 0) {
								highlightedParts |= GothicTrajectoryParts.AreaLeftSide;
							}
						}
					}
					else {
						if (-yMax <= turnOriginCsPoint.Y && turnOriginCsPoint.Y <= yMax
						) {
							if (turnOriginCsPoint.Y > 0) {
								highlightedParts |= GothicTrajectoryParts.AreaRightSide;
							}
							else if (turnOriginCsPoint.Y < 0) {
								highlightedParts |= GothicTrajectoryParts.AreaLeftSide;
							}
						}
					}
				}
			}

			if (!highlightedParts.HasFlag(GothicTrajectoryParts.Area)) {
				if (highlightedParts.HasFlag(GothicTrajectoryParts.AreaLeftSide) ||
					highlightedParts.HasFlag(GothicTrajectoryParts.AreaRightSide)) {
					highlightedParts |= GothicTrajectoryParts.Area;
				}

				if (highlightedParts.HasFlag(GothicTrajectoryParts.MaxDistance) &&
					distanceAfterTurn > 0) {
					highlightedParts |= GothicTrajectoryParts.Area;
				}
			}

			//Mandatory part
			if (p.MandatoryDistance > 0 &&
				0 < spaceshipCsPoint.X && spaceshipCsPoint.X < p.MandatoryDistance + Game.Params.TrajectoryAnchorDistance) {
				if (highlightedParts.HasFlag(GothicTrajectoryParts.CenterLine)) {
					highlightedParts |= GothicTrajectoryParts.MandatoryPart;
				}
				else {
					if (distanceAfterTurnToPoint < mandatoryDistanceAfterTurn) {
						highlightedParts |= GothicTrajectoryParts.MandatoryPart;
					}
				}
			}

			if (highlightedParts.HasFlag(GothicTrajectoryParts.MandatoryPartEnd)) {
				highlightedParts |= GothicTrajectoryParts.MandatoryPart;
			}

			Point2d? anchoredPoint = null;
			if (highlightedParts.HasFlag(GothicTrajectoryParts.CenterLine)) {
				if (highlightedParts.HasFlag(GothicTrajectoryParts.MaxDistance)) {
					anchoredPoint = new Point2d(p.DistanceToMove, 0);
				}
				else if (highlightedParts.HasFlag(GothicTrajectoryParts.MandatoryPartEnd)) {
					anchoredPoint = new Point2d(p.MandatoryDistance, 0);
				}
				else {
					anchoredPoint = new Point2d(spaceshipCsPoint.X, 0);
				}
			}
			else if (highlightedParts.HasFlag(GothicTrajectoryParts.MaxDistance)) {
				if (highlightedParts.HasFlag(GothicTrajectoryParts.AreaLeftSide) ||
					highlightedParts.HasFlag(GothicTrajectoryParts.AreaRightSide)) {
					anchoredPoint = new Point2d(p.TurnIsPossibleAtDistance + xMax, Math.Sign(spaceshipCsPoint.Y) * yMax);
				}
				else {
					anchoredPoint = new Point2d(p.TurnIsPossibleAtDistance + xCur, yCur);
				}
			}
			else if (highlightedParts.HasFlag(GothicTrajectoryParts.FirstTurnDistance)) {
				anchoredPoint = new Point2d(p.TurnIsPossibleAtDistance, 0);
			}
			else if (highlightedParts.HasFlag(GothicTrajectoryParts.MandatoryPartEnd)) {
				if (mandatoryDistanceAfterTurn > 0) {
					double xMandatoryCur = mandatoryDistanceAfterTurn * GeometryHelper.Cos(radCur);
					double yMandatoryCur = mandatoryDistanceAfterTurn * GeometryHelper.Sin(radCur);
					anchoredPoint = new Point2d(p.TurnIsPossibleAtDistance + xMandatoryCur, yMandatoryCur);
				}
				else {
					anchoredPoint = new Point2d(p.MandatoryDistance, 0);
				}
			}
			else if (highlightedParts.HasFlag(GothicTrajectoryParts.AreaLeftSide) ||
					 highlightedParts.HasFlag(GothicTrajectoryParts.AreaRightSide)
			) {
				if (MaxTurnAngle != 90) {
					double k = Math.Sign(turnOriginCsPoint.Y) * Math.Tan(maxAngleRadian);
					double y = k * turnOriginCsPoint.X;
					anchoredPoint = new Point2d(spaceshipCsPoint.X, y);
				}
				else {
					if (-yMax <= turnOriginCsPoint.Y && turnOriginCsPoint.Y <= yMax) {
						anchoredPoint = new Point2d(p.TurnIsPossibleAtDistance, spaceshipCsPoint.Y);
					}
				}
			}
			else if (highlightedParts.HasFlag(GothicTrajectoryParts.Area)) {
				anchoredPoint = spaceshipCsPoint;
			}

			return new HitTestResult(anchoredPoint, highlightedParts);
		}


		struct GothicTrajectoryParams
		{
			public readonly float DistanceToMove;
			public readonly float MandatoryDistance;
			public readonly float OptionalDistance;
			public readonly float RandomDistance;
			public readonly float TurnIsPossibleAtDistance;

			public GothicTrajectoryParams(float mandatoryDistance, float distanceToMove, float optionalDistance, float randomDistance, float turnIsPossibleAtDistance)
			{
				MandatoryDistance = mandatoryDistance;
				DistanceToMove = distanceToMove;
				OptionalDistance = optionalDistance;
				RandomDistance = randomDistance;
				TurnIsPossibleAtDistance = turnIsPossibleAtDistance;
			}
		}


		struct HitTestResult
		{
			public readonly GothicTrajectoryParts HighlightedParts;
			public Point2d? AnchoredPoint;

			public HitTestResult(Point2d? anchoredPoint, GothicTrajectoryParts highlightedParts)
			{
				AnchoredPoint = anchoredPoint;
				HighlightedParts = highlightedParts;
			}
		}
	}
}