using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceStrategy
{
	internal class GothicTrajectory : LinearTrajectory
	{
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
		public Game Game { get { return owner.Spaceship.Game; } }
		public bool SpecialOrderIsPossible { get; private set; }
		public double MaxDistance { get; private set; }
		public double MinRunBeforeTurn { get; private set; }
		public int MaxTurnCount { get; private set; }
		private double MaxTurnAngle { get; set; }
		public double MandatoryDistance { get; private set; }
        bool specialOrderIsSet = false;
		internal GothicOrder SpecialOrder { get; set; }
        public void CutDistanceToMove(double distanceToCut)
        {
            MaxDistance = Math.Max(0, MaxDistance - distanceToCut);
        }
		public override void Draw(Graphics dc)
		{
            if (!IsVisible)
                return;
			GothicTrajectoryParams p = GetTrajectoryParams(previewedOrder);

			if (this.owner.Spaceship.State == SpaceshipState.Standing) {
				var oldDc = dc.Save();
				dc.TranslateTransform((float)GetCurrentPosition().Location.X, (float)GetCurrentPosition().Location.Y);
				dc.RotateTransform((float)GetCurrentPosition().Degree);

				if (this.MaxTurnCount > 0 && p.turnIsPossibleAtDistance < p.distanceToMove && p.distanceToMove > 0.1) {
					if (p.turnIsPossibleAtDistance < p.mandatoryDistance + p.optionalDistance) {
						Color maxDistColor = highlightedParts.HasFlag(GothicTrajectoryParts.MaxDistance) ? Game.Params.SelectedTrajectoryColor : Game.Params.TrajectoryColor;
						Color leftSideColor = highlightedParts.HasFlag(GothicTrajectoryParts.AreaLeftSide) ? Game.Params.SelectedTrajectoryColor : Game.Params.TrajectoryColor;
						Color rightSideColor = highlightedParts.HasFlag(GothicTrajectoryParts.AreaRightSide) ? Game.Params.SelectedTrajectoryColor : Game.Params.TrajectoryColor;
						Color fillColor = highlightedParts.HasFlag(GothicTrajectoryParts.Area) ? Game.Params.SelectedTrajectoryColor : Game.Params.TrajectoryColor;

						DrawMovementZone(dc, rightSideColor, leftSideColor, maxDistColor, fillColor, p.mandatoryDistance + p.optionalDistance - p.turnIsPossibleAtDistance, p.turnIsPossibleAtDistance);
					}
					if (p.turnIsPossibleAtDistance < p.mandatoryDistance) {
						Color maxDistColor = highlightedParts.HasFlag(GothicTrajectoryParts.MandatoryPartEnd) ? Game.Params.SelectedMandatoryTrajectoryColor : Game.Params.MandatoryTrajectoryColor;
						Color leftSideColor = highlightedParts.HasFlag(GothicTrajectoryParts.AreaLeftSide) ? Game.Params.SelectedMandatoryTrajectoryColor : Game.Params.MandatoryTrajectoryColor;
						Color rightSideColor = highlightedParts.HasFlag(GothicTrajectoryParts.AreaRightSide) ? Game.Params.SelectedMandatoryTrajectoryColor : Game.Params.MandatoryTrajectoryColor;
						Color fillColor = highlightedParts.HasFlag(GothicTrajectoryParts.Area) ? Game.Params.SelectedMandatoryTrajectoryColor : Game.Params.MandatoryTrajectoryColor;

						DrawMovementZone(dc, rightSideColor, leftSideColor, maxDistColor, fillColor, p.mandatoryDistance - p.turnIsPossibleAtDistance, p.turnIsPossibleAtDistance);
					}
				}

				if (p.mandatoryDistance > 0) {
					Color lineColor = highlightedParts.HasFlag(GothicTrajectoryParts.CenterLine) ? Game.Params.SelectedMandatoryTrajectoryColor : Game.Params.MandatoryTrajectoryColor;
					dc.DrawLine(new Pen(lineColor, Game.Params.TrajectoryLineThickness), 0, 0, p.mandatoryDistance, 0);
				}
				if (p.optionalDistance > 0) {
					Color lineColor = highlightedParts.HasFlag(GothicTrajectoryParts.CenterLine) ? Game.Params.SelectedTrajectoryColor : Game.Params.TrajectoryColor;
					dc.DrawLine(new Pen(lineColor, Game.Params.TrajectoryLineThickness), p.mandatoryDistance, 0, p.mandatoryDistance + p.optionalDistance, 0);
				}
				if (p.randomDistance > 0) {
					Pen randomDistancePen = new Pen(Game.Params.MandatoryTrajectoryColor, Game.Params.TrajectoryLineThickness);
					randomDistancePen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
					dc.DrawLine(randomDistancePen, p.mandatoryDistance + p.optionalDistance, 0, p.mandatoryDistance + p.optionalDistance + p.randomDistance, 0);
				}

				if (highlightPoint.HasValue) {
					Point2d globalCSPoint = highlightPoint.Value.TransformBy(Position);
					if (highlightedParts.HasFlag(GothicTrajectoryParts.MandatoryPart))
						Game.Cursor.SetForm(CursorForm.TrajectoryMandatory);
					else
						Game.Cursor.SetForm(CursorForm.TrajectoryOptional);
					Game.Cursor.SetLocation(Game.CoordConverter.GetWinCoord(globalCSPoint));
				}
				dc.Restore(oldDc); 
			}
			else if (this.owner.Spaceship.State == SpaceshipState.Moving) {
				Pen pen = new Pen(Game.Params.ActiveTrajectoryColor, Game.Params.ActiveTrajectoryThicknes);
				for (int i = 0; i < trajectoryPoints.Count-1; i++) {
					PointF startP = new PointF((float)trajectoryPoints[i].X, (float)trajectoryPoints[i].Y);
					PointF endP = new PointF((float)trajectoryPoints[i + 1].X, (float)trajectoryPoints[i + 1].Y);
					dc.DrawLine(pen, startP, endP);
				}
				highlightPoint = null;
			}
		}
		internal override void AddToCurrentDistance(double distance, out double unusedDistance)
		{
            if (distanceFromStart > MaxDistance)
            {
                Length = distanceFromStart;
            }
			base.AddToCurrentDistance(distance, out unusedDistance);
			if (unusedDistance > 0) {
				owner.Spaceship.State = SpaceshipState.Standing;
				this.Position = this.GetCurrentPosition();
				this.StartPoint = Position.Location;
				this.Direction = Position.Direction;
				this.MaxDistance = MaxDistance - distanceFromStart;
				this.MandatoryDistance = Math.Max(0, MandatoryDistance - distanceFromStart);
				this.distanceFromStart = 0;
				this.trajectoryPoints.Clear();
			}
		}
		internal override void OnMouseMove(Point2d coord)
		{
			var hitResult = HitTest(coord);
			highlightPoint = hitResult.AnchoredPoint;
			highlightedParts = hitResult.HighlightedParts;
		}
		internal override bool IsOnCourse(Point2d coord, double maxDistFromCourse)
		{
			GothicTrajectoryParams p = GetTrajectoryParams();
			Point2d spaceshipCSCoord = coord.UnTransformBy(owner.Spaceship.Position);
			//Center line
			if ((Math.Abs(spaceshipCSCoord.Y) < maxDistFromCourse) &&
				(spaceshipCSCoord.X >= 0 && spaceshipCSCoord.X <= p.distanceToMove)) {
				return true;
			}
			return false;
		}
		private HitTestResult HitTest(Point2d coord)
		{
			GothicTrajectoryParts highlightedParts = GothicTrajectoryParts.None;

			GothicTrajectoryParams p = GetTrajectoryParams();
			Point2d spaceshipCSCoord = coord.UnTransformBy(Position);
			//Center line
			if ((Math.Abs(spaceshipCSCoord.Y) <= Game.Params.TrajectoryAnchorDistance) &&
				(spaceshipCSCoord.X >= 0 && spaceshipCSCoord.X <= p.distanceToMove)) {
				highlightedParts |= GothicTrajectoryParts.CenterLine;
			}
			//Mandatory distance end
			if (p.mandatoryDistance <= p.turnIsPossibleAtDistance) {
				if (Math.Abs(spaceshipCSCoord.X - p.mandatoryDistance) < Game.Params.TrajectoryAnchorDistance &&
					Math.Abs(spaceshipCSCoord.Y) < Game.Params.TrajectoryAnchorDistance)
					highlightedParts |= GothicTrajectoryParts.MandatoryPartEnd;
			}

			double distanceAfterTurn = p.distanceToMove - p.turnIsPossibleAtDistance;
			double maxAngleRadian = MaxTurnAngle * GeometryHelper.Pi / 180;
			double xMax = distanceAfterTurn * GeometryHelper.Cos(maxAngleRadian);
			double yMax = distanceAfterTurn * GeometryHelper.Sin(maxAngleRadian);
			Point2d turnOriginCSCoord = new Point2d(spaceshipCSCoord.X - p.turnIsPossibleAtDistance, spaceshipCSCoord.Y);
			double radCur = GeometryHelper.ToRadian(new Vector(turnOriginCSCoord.X, turnOriginCSCoord.Y));
			double xCur = distanceAfterTurn * GeometryHelper.Cos(radCur);
			double yCur = distanceAfterTurn * GeometryHelper.Sin(radCur);
			double mandatoryDistanceAfterTurn = p.mandatoryDistance - p.turnIsPossibleAtDistance;
			double distanceAfterTurnToPoint = turnOriginCSCoord.ToVector().Length;
					
			if (p.turnIsPossibleAtDistance <= p.distanceToMove) {

				//First turn distance
				if (Math.Abs(spaceshipCSCoord.X) < Game.Params.TrajectoryAnchorDistance &&
					Math.Abs(spaceshipCSCoord.Y) < Game.Params.TrajectoryAnchorDistance)
					highlightedParts |= GothicTrajectoryParts.FirstTurnDistance;


				//Mandatory distance end arc
				if (mandatoryDistanceAfterTurn > 0) {
					if (Math.Abs(Math.Sqrt(turnOriginCSCoord.X * turnOriginCSCoord.X + turnOriginCSCoord.Y * turnOriginCSCoord.Y) - mandatoryDistanceAfterTurn) < Game.Params.TrajectoryAnchorDistance) {
						if (Math.Abs(GeometryHelper.ToRadian(new Vector(turnOriginCSCoord.X, turnOriginCSCoord.Y))) < maxAngleRadian) {
							highlightedParts |= GothicTrajectoryParts.MandatoryPartEnd;
						}
					}
				}

				if (Math.Abs(GeometryHelper.ToRadian(new Vector(turnOriginCSCoord.X, turnOriginCSCoord.Y))) <= maxAngleRadian) {
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
				if ((0 <= turnOriginCSCoord.X && turnOriginCSCoord.X <= xMax) ||
					(Math.Abs(turnOriginCSCoord.X) < Game.Params.TrajectoryAnchorDistance)) {
					if (MaxTurnAngle != 90) {
						double k = Math.Sign(turnOriginCSCoord.Y) * Math.Tan(maxAngleRadian);
						double y = k * turnOriginCSCoord.X;
						if (Math.Abs(turnOriginCSCoord.Y - y) < Game.Params.TrajectoryAnchorDistance) {
							if (k > 0) highlightedParts |= GothicTrajectoryParts.AreaRightSide;
							else if (k < 0) highlightedParts |= GothicTrajectoryParts.AreaLeftSide;
						}
					}
					else {
						if (-yMax <= turnOriginCSCoord.Y && turnOriginCSCoord.Y <= yMax
							) {
							if (turnOriginCSCoord.Y > 0) highlightedParts |= GothicTrajectoryParts.AreaRightSide;
							else if (turnOriginCSCoord.Y < 0) highlightedParts |= GothicTrajectoryParts.AreaLeftSide;
						}
					}
				}
			}
			if (!highlightedParts.HasFlag(GothicTrajectoryParts.Area)) {
				if (highlightedParts.HasFlag(GothicTrajectoryParts.AreaLeftSide)||
					highlightedParts.HasFlag(GothicTrajectoryParts.AreaRightSide))
					highlightedParts|=GothicTrajectoryParts.Area;
				if (highlightedParts.HasFlag(GothicTrajectoryParts.MaxDistance) &&
					distanceAfterTurn > 0)
					highlightedParts |= GothicTrajectoryParts.Area;
			}


			//Mandatory part
			if (p.mandatoryDistance > 0 &&
				0 < spaceshipCSCoord.X && spaceshipCSCoord.X < p.mandatoryDistance + Game.Params.TrajectoryAnchorDistance) {
				if (highlightedParts.HasFlag(GothicTrajectoryParts.CenterLine)) {
					highlightedParts |= GothicTrajectoryParts.MandatoryPart;
				}
				else {
					if (distanceAfterTurnToPoint < mandatoryDistanceAfterTurn)
						highlightedParts |= GothicTrajectoryParts.MandatoryPart;
				}
			}
			if (highlightedParts.HasFlag(GothicTrajectoryParts.MandatoryPartEnd))
				highlightedParts |= GothicTrajectoryParts.MandatoryPart;

			Point2d? anchoredPoint = null;
			if (highlightedParts.HasFlag(GothicTrajectoryParts.CenterLine)) {
				if (highlightedParts.HasFlag(GothicTrajectoryParts.MaxDistance))
					anchoredPoint = new Point2d(p.distanceToMove, 0);
				else if (highlightedParts.HasFlag(GothicTrajectoryParts.MandatoryPartEnd))
					anchoredPoint = new Point2d(p.mandatoryDistance, 0);
				else
					anchoredPoint = new Point2d(spaceshipCSCoord.X, 0);
			}
			else if (highlightedParts.HasFlag(GothicTrajectoryParts.MaxDistance)) {
				if (highlightedParts.HasFlag(GothicTrajectoryParts.AreaLeftSide) ||
					highlightedParts.HasFlag(GothicTrajectoryParts.AreaRightSide)) {
					anchoredPoint = new Point2d(p.turnIsPossibleAtDistance + xMax, Math.Sign(spaceshipCSCoord.Y) * yMax);
				}
				else {
					anchoredPoint = new Point2d(p.turnIsPossibleAtDistance + xCur, yCur);
				}
			}
			else if (highlightedParts.HasFlag(GothicTrajectoryParts.FirstTurnDistance)) {
				anchoredPoint = new Point2d(p.turnIsPossibleAtDistance, 0);
			}
			else if (highlightedParts.HasFlag(GothicTrajectoryParts.MandatoryPartEnd)) {
				if (mandatoryDistanceAfterTurn > 0) {
					double xMandatoryCur = mandatoryDistanceAfterTurn * GeometryHelper.Cos(radCur);
					double yMandatoryCur = mandatoryDistanceAfterTurn * GeometryHelper.Sin(radCur);
					anchoredPoint = new Point2d(p.turnIsPossibleAtDistance + xMandatoryCur, yMandatoryCur);
				}
				else {
					anchoredPoint = new Point2d(p.mandatoryDistance, 0);
				}
			}
			else if (highlightedParts.HasFlag(GothicTrajectoryParts.AreaLeftSide) ||
					  highlightedParts.HasFlag(GothicTrajectoryParts.AreaRightSide)
				  ) {
				if (MaxTurnAngle != 90) {
					double k = Math.Sign(turnOriginCSCoord.Y) * Math.Tan(maxAngleRadian);
					double y = k * turnOriginCSCoord.X;
					anchoredPoint = new Point2d(spaceshipCSCoord.X, y);
				}
				else {
					if (-yMax <= turnOriginCSCoord.Y && turnOriginCSCoord.Y <= yMax) {
						anchoredPoint = new Point2d(p.turnIsPossibleAtDistance, spaceshipCSCoord.Y);
					}
				}
			}
			else if (highlightedParts.HasFlag(GothicTrajectoryParts.Area))
				anchoredPoint = spaceshipCSCoord;


			return new HitTestResult(anchoredPoint, highlightedParts);
		}
		private void DrawMovementZone(Graphics dc, Color rightSideLineColor, Color leftSideLineColor, Color maxDistanceColor, Color fillColor, float zoneDistance, float turnIsPossibleAtDistance)
		{
            if (zoneDistance > 0.001)
            {
                float maxY = zoneDistance * (float)GeometryHelper.Sin(MaxTurnAngle * GeometryHelper.Pi / 180);
                float maxX = zoneDistance * (float)GeometryHelper.Cos(MaxTurnAngle * GeometryHelper.Pi / 180);
                Pen leftSidePen = new Pen(leftSideLineColor, Game.Params.TrajectoryLineThickness);
                Pen rightSidePen = new Pen(rightSideLineColor, Game.Params.TrajectoryLineThickness);
                Pen maxDistPen = new Pen(maxDistanceColor, Game.Params.TrajectoryLineThickness);
                int alphaChannelIntensity = 64;
                Brush transparentBrush = new SolidBrush(Color.FromArgb(alphaChannelIntensity, fillColor));
                var dcState = dc.Save();
                dc.TranslateTransform(turnIsPossibleAtDistance, 0);
                dc.FillPie(transparentBrush, -zoneDistance, -zoneDistance, 2 * zoneDistance, 2 * zoneDistance, -(float)MaxTurnAngle, (float)(2 * MaxTurnAngle));
                dc.DrawLine(rightSidePen, 0, 0, maxX, maxY);
                dc.DrawLine(leftSidePen, 0, 0, maxX, -maxY);

                dc.DrawArc(maxDistPen, -zoneDistance, -zoneDistance, 2 * zoneDistance, 2 * zoneDistance, -(float)MaxTurnAngle, (float)(2 * MaxTurnAngle));
                dc.Restore(dcState);
            }
		}
		public void SetSpecialOrder(GothicOrder order)
		{
            if (specialOrderIsSet)
                return;

            specialOrderIsSet = true;
			//PreviewSpecialOrder(GothicOrder.NormalMove);

            switch (order)
            {
                case GothicOrder.None:
                case GothicOrder.ReloadOrdnance:
                case GothicOrder.BraceForImpact:
                    break;
                case GothicOrder.AllAheadFull:
                    GothicSpaceship ownerSpaceship = owner.Spaceship as GothicSpaceship;
                    //MaxDistance = (float)(MaxDistance + (float)Game.DistanceCoef * Game.RollDices(6, ownerSpaceship.Class.AllAheadFullCoef, "Дополнительная дальность"));
                    MaxDistance = (float)(MaxDistance + Dice.RollDices(6, ownerSpaceship.Class.AllAheadFullCoef, "Дополнительная дальность"));
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
                default:
                    break;
            }
            SpecialOrder = order;
            previewedOrder = order;
		}
        GothicOrder previewedOrder;
		public void PreviewSpecialOrder(GothicOrder order)
		{
            if (!specialOrderIsSet)
                previewedOrder = order;
		}
        private GothicTrajectoryParams GetTrajectoryParams()
        {
            return GetTrajectoryParams(SpecialOrder);
        }
		private GothicTrajectoryParams GetTrajectoryParams(GothicOrder specialOrder)
		{
			float mandatoryDistance;
			float distanceToMove;
			float optionalDistance;
			float randomDistance;			
			float turnIsPossibleAtDistance;

                switch (specialOrder) {
				case GothicOrder.AllAheadFull:
					mandatoryDistance = (float)(MaxDistance - distanceFromStart);
					if (mandatoryDistance < 0) mandatoryDistance = 0;
					distanceToMove = (float)(MaxDistance - distanceFromStart);
					optionalDistance = 0;
                    randomDistance = (float)(specialOrderIsSet ? 0 : 24);
					turnIsPossibleAtDistance = float.PositiveInfinity;//turn is not possible
					break;
				case GothicOrder.BurnRetros:
					mandatoryDistance = 0;

                    distanceToMove = (float)((specialOrderIsSet ? MaxDistance : MaxDistance / 2) - distanceFromStart);
					optionalDistance = distanceToMove;
					randomDistance = 0;
					turnIsPossibleAtDistance = 0;
					break;
				case GothicOrder.LockOn:
					mandatoryDistance = (float)(MandatoryDistance - distanceFromStart);
					distanceToMove = (float)(MaxDistance - distanceFromStart);
					optionalDistance = distanceToMove - mandatoryDistance;
					randomDistance = 0;
					turnIsPossibleAtDistance = float.PositiveInfinity;//turn is not possible
					break;
				case GothicOrder.LaunchOrdnance:
					mandatoryDistance = (float)5000;
					distanceToMove = mandatoryDistance = (float)5000;
					optionalDistance = 0;
					randomDistance = 0;
					turnIsPossibleAtDistance = 0;
					break;
				default:
				case GothicOrder.ComeToNewDirection:
				case GothicOrder.BraceForImpact:
				case GothicOrder.ReloadOrdnance:
					mandatoryDistance = (float)(MandatoryDistance - distanceFromStart);
					distanceToMove = (float)(MaxDistance - distanceFromStart);
					optionalDistance = distanceToMove - mandatoryDistance;
					randomDistance = 0;
					turnIsPossibleAtDistance = (float)Math.Max(MinRunBeforeTurn - owner.DistanceAfterLastTurn, 0);
					break;
			}
			return new GothicTrajectoryParams(mandatoryDistance,distanceToMove,optionalDistance,randomDistance,turnIsPossibleAtDistance);
		}
		Point2d? highlightPoint;
		GothicTrajectoryParts highlightedParts;
		List<Point2d> trajectoryPoints = new List<Point2d>();
		public override Position GetPositionAt(double distance)
		{
			double curDist = 0;
			for (int i = 0; i < trajectoryPoints.Count-1; i++)
			{
				curDist += trajectoryPoints[i].DistanceTo(trajectoryPoints[i+1]);
				if (curDist > distance){
					Vector curDir = trajectoryPoints[i].VectorTo(trajectoryPoints[i+1]);
					curDir.Normalize();
					var resultPoint = trajectoryPoints[i+1] - curDir*(curDist - distance);
					return new Position(resultPoint, curDir);
				}
			}
			if (trajectoryPoints.Count == 0)
				return Position;
			Vector lasDir = trajectoryPoints[trajectoryPoints.Count - 2].VectorTo(trajectoryPoints[trajectoryPoints.Count - 1]);
			lasDir.Normalize();
			return new Position(trajectoryPoints.Last(), lasDir);
		}
		internal bool MoveTo(Point2d coord)
		{
            var hitResult = HitTest(coord);
            if (hitResult.AnchoredPoint.HasValue)
            {
                SpecialOrderIsPossible = false;
                trajectoryPoints.Clear();

                trajectoryPoints.Add(StartPoint);
                Point2d endPoint = hitResult.AnchoredPoint.Value.TransformBy(Position);
                trajectoryPoints.Add(endPoint);

                if (highlightedParts.HasFlag(GothicTrajectoryParts.Area))
                {
                    var p = GetTrajectoryParams();

                    if (p.turnIsPossibleAtDistance >= 0)
                    {
                        Point2d startOfAfterTurnPart = new Point2d(p.turnIsPossibleAtDistance, 0).TransformBy(Position);
                        if (startOfAfterTurnPart != StartPoint && startOfAfterTurnPart != endPoint)
                        {
                            trajectoryPoints.Insert(1, startOfAfterTurnPart);
                            Vector part1Dir = trajectoryPoints[0].VectorTo(trajectoryPoints[1]);
                            Vector part2Dir = trajectoryPoints[1].VectorTo(trajectoryPoints[2]);
                            part1Dir.Normalize();
                            part2Dir.Normalize();
                            if (part1Dir != part2Dir)
                                MaxTurnCount--;
                        }
                        else
                        {
                            Vector part1Dir = trajectoryPoints[0].VectorTo(trajectoryPoints[1]);
                            part1Dir.Normalize();
                            if (part1Dir != Position.Direction)
                                MaxTurnCount--;
                        }
                    }
                }

                double totalTrajectoryLength = 0;
                for (int i = 0; i < trajectoryPoints.Count - 1; i++)
                {
                    totalTrajectoryLength += trajectoryPoints[i].DistanceTo(trajectoryPoints[i + 1]);
                }
                Length = totalTrajectoryLength;
                return true;
            }
            else
                return false;
		}
		private struct GothicTrajectoryParams
		{
			public float mandatoryDistance;
			public float distanceToMove;
			public float optionalDistance;
			public float randomDistance;
			public float turnIsPossibleAtDistance;
			public GothicTrajectoryParams(float mandatoryDistance, float distanceToMove, float optionalDistance, float randomDistance, float turnIsPossibleAtDistance)
			{
				this.mandatoryDistance = mandatoryDistance;
				this.distanceToMove = distanceToMove;
				this.optionalDistance = optionalDistance;
				this.randomDistance = randomDistance;
				this.turnIsPossibleAtDistance = turnIsPossibleAtDistance;
			}
		}
		private struct HitTestResult
		{
			public Point2d? AnchoredPoint;
			public GothicTrajectoryParts HighlightedParts;
			public HitTestResult(Point2d? anchoredPoint, GothicTrajectoryParts highlightedParts)
			{
				AnchoredPoint = anchoredPoint;
				HighlightedParts = highlightedParts;
			}
		}
		
		internal bool Autocomplete()
		{
            GothicSpaceshipBase gothicSpaceshipBase = owner.Spaceship as GothicSpaceshipBase;
            if (gothicSpaceshipBase!=null)
            {
                var p = GetTrajectoryParams();
                if (p.mandatoryDistance > 0.001)
                {
                    double distanceToMove = Math.Min(p.mandatoryDistance, p.distanceToMove);
                    Position curPos = owner.Spaceship.Position;
                    Point2d newPos = curPos.Location + curPos.Direction * distanceToMove;
                    gothicSpaceshipBase.MoveTo(newPos);
                    return true;
                }
            }
			return false;
		}
	}
	[Flags]
	internal enum GothicTrajectoryParts:int
	{
		None=0,
		Area=1,
		CenterLine=2,
		MandatoryPartEnd=4,
		MaxDistance=8,
		FirstTurnDistance=16,
		SecondTurnDistance=32,
		AreaLeftSide=64,
		AreaRightSide=128,
		MandatoryPart=256
	}
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

}
