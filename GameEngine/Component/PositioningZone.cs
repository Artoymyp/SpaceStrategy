using System;
using System.Drawing;
using System.Linq;

namespace SpaceStrategy
{
	public class PositioningZone
	{
		Point2d _bottomRightPoint;
		bool _creatingSpaceship;
		int _maxSpaceshipsCount;
		Point2d _topLeftPoint;

		public PositioningZone(Game game, Player player, Point2d topLeftPoint, Point2d bottomRightPoint)
		{
			Player = player;
			Player.PositioningZone = this;
			Game = game;
			_maxSpaceshipsCount = 4; // game.Scenario.MaxPlayerSpaceshipCount;

			// TODO: Complete member initialization
			_topLeftPoint = topLeftPoint;
			_bottomRightPoint = bottomRightPoint;
		}

		public GothicSpaceship CreatedSpaceship { get; private set; }

		public SpaceshipClass SelectedSpaceshipClass { get; private set; }

		internal Game Game { get; }

		internal Player Player { get; }

		bool CanCreateMoreSpaceships
		{
			get { return _maxSpaceshipsCount > 0; }
		}

		public event EventHandler PositioningBegin;

		public event EventHandler PositioningComplete;

		public void BeginSpaceshipCreation(SpaceshipClass spaceshipClass)
		{
			if (CanCreateMoreSpaceships) {
				// && SelectedSpaceshipClass.Id != spaceshipClass.Id) {
				SelectedSpaceshipClass = spaceshipClass;
				Game.RemoveSpaceship(CreatedSpaceship);
				CreatedSpaceship = new GothicSpaceship(Game, new Position(), SelectedSpaceshipClass, Player);
				Game.SelectedSpaceship = CreatedSpaceship;
			}
		}

		internal void Draw(Graphics dc)
		{
			int width = (int)Math.Abs(_bottomRightPoint.X - _topLeftPoint.X);
			int height = (int)Math.Abs(_bottomRightPoint.Y - _topLeftPoint.Y);

			//width = (int)(width * Game.DistanceCoef);
			//height = (int)(height * Game.DistanceCoef);

			//Rectangle rect = new Rectangle((int)(TopLeftCorner.X * Game.DistanceCoef), (int)(TopLeftCorner.Y * Game.DistanceCoef), width, height);
			var rect = new Rectangle((int)_topLeftPoint.X, (int)_topLeftPoint.Y, width, height);
			var pen = new Pen(Game.Params.PositioningZoneColor);
			var brush = new SolidBrush(Color.FromArgb(128, Game.Params.PositioningZoneColor));
			dc.FillRectangle(brush, rect);
			dc.DrawRectangle(pen, rect);
		}

		internal void OnMouseClick(Point2d point)
		{
			if (CreatedSpaceship != null && IsInside(CreatedSpaceship.Position.Location)) {
				if (CreatedSpaceship.State == SpaceshipState.DeterminingPosition) {
					CreatedSpaceship.State = SpaceshipState.DeterminingDirection;
				}
				else if (CreatedSpaceship.State == SpaceshipState.DeterminingDirection) {
					EndSpaceshipCreation();
				}
			}
		}

		internal void OnMouseMove(Point2d point)
		{
			if (SelectedSpaceshipClass != null) {
				if (CreatedSpaceship.State == SpaceshipState.DeterminingPosition) {
					CreatedSpaceship.Position = new Position(point);
				}

				if (!Game.GothicSpaceships.Contains(CreatedSpaceship)) {
					Game.AddSpaceship(CreatedSpaceship);
				}
				else if (CreatedSpaceship.State == SpaceshipState.DeterminingDirection) {
					Point2d curPos = CreatedSpaceship.Position;
					var newDir = new Vector(point.X - curPos.X, point.Y - curPos.Y);
					if (newDir == new Vector(0, 0)) {
						newDir = new Vector(1, 0);
					}

					newDir.Normalize();
					CreatedSpaceship.Position = new Position(curPos, newDir);
				}
			}
		}

		internal void StartPositioning()
		{
			if (CanCreateMoreSpaceships) {
				_creatingSpaceship = true;
				OnPositioningBegin(this, new EventArgs());
			}
		}

		protected void OnPositioningBegin(object sender, EventArgs e)
		{
			if (PositioningBegin != null) {
				PositioningBegin(sender, e);
			}
		}

		protected void OnPositioningComplete(object sender, EventArgs e)
		{
			if (PositioningComplete != null) {
				PositioningComplete(sender, e);
			}
		}

		void EndSpaceshipCreation()
		{
			CreatedSpaceship.FinishSpaceshipCreation();
			CreatedSpaceship.State = SpaceshipState.Standing;
			CreatedSpaceship.GenerateLeadership();

			if (SelectedSpaceshipClass != null && CanCreateMoreSpaceships) {
				CreatedSpaceship = new GothicSpaceship(Game, new Position(), SelectedSpaceshipClass, Player);
			}

			_maxSpaceshipsCount--;
			if (!CanCreateMoreSpaceships) {
				_creatingSpaceship = false;
				OnPositioningComplete(this, new EventArgs());
			}
		}

		bool IsInside(Point2d point)
		{
			//if (point.X < (int)(TopLeftCorner.X * Game.DistanceCoef) || point.X > (int)(BottomRightCorner.X * Game.DistanceCoef)) return false;
			//if (point.Y < (int)(TopLeftCorner.Y * Game.DistanceCoef) || point.Y > (int)(BottomRightCorner.Y * Game.DistanceCoef)) return false;
			if (point.X < (int)_topLeftPoint.X || point.X > (int)_bottomRightPoint.X) {
				return false;
			}

			if (point.Y < (int)_topLeftPoint.Y || point.Y > (int)_bottomRightPoint.Y) {
				return false;
			}

			return true;
		}
	}
}