using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace SpaceStrategy
{
	public class PositioningZone
	{
		private bool _creatingSpaceship = false;
		public SpaceshipClass SelectedSpaceshipClass { get; private set; }
		public GothicSpaceship CreatedSpaceship { get; private set; }

		private Point2d _topLeftPoint;
		private Point2d _bottomRightPoint;
		private int _maxSpaceshipsCount = 0;
		public PositioningZone(Game game, Player player, Point2d topLeftPoint, Point2d bottomRightPoint)
		{
			this.Player = player;
			Player.PositioningZone = this;
			Game = game;
			_maxSpaceshipsCount = 4;// game.Scenario.MaxPlayerSpaceshipCount;
			// TODO: Complete member initialization
			this._topLeftPoint = topLeftPoint;
			this._bottomRightPoint = bottomRightPoint;
		}
		internal Game Game { get; private set; }
		internal Player Player { get; private set; }
		internal void Draw(System.Drawing.Graphics dc)
		{
			int width = (int)Math.Abs(_bottomRightPoint.X - _topLeftPoint.X);
			int height = (int)Math.Abs(_bottomRightPoint.Y - _topLeftPoint.Y);

			//width = (int)(width * Game.DistanceCoef);
			//height = (int)(height * Game.DistanceCoef);

			//Rectangle rect = new Rectangle((int)(TopLeftCorner.X * Game.DistanceCoef), (int)(TopLeftCorner.Y * Game.DistanceCoef), width, height);
			Rectangle rect = new Rectangle((int)(_topLeftPoint.X), (int)(_topLeftPoint.Y), width, height);
			Pen pen = new Pen(Game.Params.PositioningZoneColor);
			SolidBrush brush = new SolidBrush(Color.FromArgb(128, Game.Params.PositioningZoneColor));
			dc.FillRectangle(brush, rect);
			dc.DrawRectangle(pen, rect); 
		}
		private bool CanCreateMoreSpaceships
		{
			get
			{
				return _maxSpaceshipsCount > 0;
			}
		}
		internal void OnMouseMove(Point2d point)
		{
			if (SelectedSpaceshipClass != null) {
				if (CreatedSpaceship.State == SpaceshipState.DeterminingPosition) {
					CreatedSpaceship.Position = new Position(point);
				}
				
				if (!Game.GothicSpaceships.Contains(CreatedSpaceship))
					Game.AddSpaceship(CreatedSpaceship);
			
				else if (CreatedSpaceship.State == SpaceshipState.DeterminingDirection) {
					Point2d curPos = CreatedSpaceship.Position;
					Vector newDir = new Vector(point.X - curPos.X, point.Y - curPos.Y);
					if (newDir == new Vector(0, 0))
						newDir = new Vector(1, 0);
					newDir.Normalize();
					CreatedSpaceship.Position = new Position(curPos, newDir);
				}
			}
		}
		internal void OnMouseClick(Point2d point)
		{
			if (CreatedSpaceship != null && IsInside(CreatedSpaceship.Position.Location)) {
				if (CreatedSpaceship.State == SpaceshipState.DeterminingPosition)
					CreatedSpaceship.State = SpaceshipState.DeterminingDirection;
				else if (CreatedSpaceship.State == SpaceshipState.DeterminingDirection) {
					EndSpaceshipCreation();
				}
			}
		}
		private bool IsInside(Point2d point){
			//if (point.X < (int)(TopLeftCorner.X * Game.DistanceCoef) || point.X > (int)(BottomRightCorner.X * Game.DistanceCoef)) return false;
			//if (point.Y < (int)(TopLeftCorner.Y * Game.DistanceCoef) || point.Y > (int)(BottomRightCorner.Y * Game.DistanceCoef)) return false;
			if (point.X < (int)(_topLeftPoint.X) || point.X > (int)(_bottomRightPoint.X)) return false;
			if (point.Y < (int)(_topLeftPoint.Y) || point.Y > (int)(_bottomRightPoint.Y)) return false;
			return true;
		}
		public void BeginSpaceshipCreation(SpaceshipClass spaceshipClass)
		{
			if (CanCreateMoreSpaceships){// && SelectedSpaceshipClass.Id != spaceshipClass.Id) {
				SelectedSpaceshipClass = spaceshipClass;
				Game.RemoveSpaceship(CreatedSpaceship);
				CreatedSpaceship = new GothicSpaceship(Game, new Position(), SelectedSpaceshipClass, Player);
				Game.SelectedSpaceship = CreatedSpaceship;
			}
		}
		
		private void EndSpaceshipCreation()
		{
			CreatedSpaceship.FinishSpaceshipCreation();
			CreatedSpaceship.State = SpaceshipState.Standing;
			CreatedSpaceship.GenerateLeadership();

			if (SelectedSpaceshipClass!=null && CanCreateMoreSpaceships)
				CreatedSpaceship = new GothicSpaceship(Game, new Position(), SelectedSpaceshipClass, Player);
			_maxSpaceshipsCount--;
			if (!CanCreateMoreSpaceships) {
				_creatingSpaceship = false;
				OnPositioningComplete(this, new EventArgs());
			}
		}
		public event EventHandler PositioningBegin;
		protected void OnPositioningBegin(object sender, EventArgs e)
		{
			if (PositioningBegin != null) {
				PositioningBegin(sender, e);
			}
		}
		
		public event EventHandler PositioningComplete;
		protected void OnPositioningComplete(object sender, EventArgs e)
		{
			if (PositioningComplete != null) {
				PositioningComplete(sender, e);
			}
		}

		internal void StartPositioning()
		{
			if (CanCreateMoreSpaceships) {
				_creatingSpaceship = true;
				OnPositioningBegin(this, new EventArgs());
			}
		}
	}
}
