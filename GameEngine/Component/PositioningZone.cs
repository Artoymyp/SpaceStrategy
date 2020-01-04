using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace SpaceStrategy
{
	public class PositioningZone
	{
		private bool creatingSpaceship = false;
		public SpaceshipClass SelectedSpaceshipClass { get; private set; }
		public GothicSpaceship CreatedSpaceship { get; private set; }

		private Point2d TopLeftCorner;
		private Point2d BottomRightCorner;
		private int MaxPoints = 0;
		public PositioningZone(Game game, Player player, Point2d topLeftCorner, Point2d bottomRightCorner)
		{
			this.Player = player;
			Player.PositioningZone = this;
			Game = game;
			MaxPoints = 4;// game.Scenario.MaxPlayerSpaceshipCount;
			// TODO: Complete member initialization
			this.TopLeftCorner = topLeftCorner;
			this.BottomRightCorner = bottomRightCorner;
		}
		internal Game Game { get; private set; }
		internal Player Player { get; private set; }
		internal void Draw(System.Drawing.Graphics dc)
		{
			int width = (int)Math.Abs(BottomRightCorner.X - TopLeftCorner.X);
			int height = (int)Math.Abs(BottomRightCorner.Y - TopLeftCorner.Y);

			//width = (int)(width * Game.DistanceCoef);
			//height = (int)(height * Game.DistanceCoef);

			//Rectangle rect = new Rectangle((int)(TopLeftCorner.X * Game.DistanceCoef), (int)(TopLeftCorner.Y * Game.DistanceCoef), width, height);
			Rectangle rect = new Rectangle((int)(TopLeftCorner.X), (int)(TopLeftCorner.Y), width, height);
			Pen pen = new Pen(Game.Params.PositioningZoneColor);
			SolidBrush brush = new SolidBrush(Color.FromArgb(128, Game.Params.PositioningZoneColor));
			dc.FillRectangle(brush, rect);
			dc.DrawRectangle(pen, rect); 
		}
		private bool CanCreateMoreShips
		{
			get
			{
				return MaxPoints > 0;
			}
		}
		internal void OnMouseMove(Point2d coord)
		{
			if (SelectedSpaceshipClass != null) {
				if (CreatedSpaceship.State == SpaceshipState.DeterminingPosition) {
					CreatedSpaceship.Position = new Position(coord);
				}
				
				if (!Game.GothicSpaceships.Contains(CreatedSpaceship))
					Game.AddSpaceship(CreatedSpaceship);
			
				else if (CreatedSpaceship.State == SpaceshipState.DeterminingDirection) {
					Point2d curPos = CreatedSpaceship.Position;
					Vector newDir = new Vector(coord.X - curPos.X, coord.Y - curPos.Y);
					if (newDir == new Vector(0, 0))
						newDir = new Vector(1, 0);
					newDir.Normalize();
					CreatedSpaceship.Position = new Position(curPos, newDir);
				}
			}
		}
		internal void OnMouseClick(Point2d coord)
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
			if (point.X < (int)(TopLeftCorner.X) || point.X > (int)(BottomRightCorner.X)) return false;
			if (point.Y < (int)(TopLeftCorner.Y) || point.Y > (int)(BottomRightCorner.Y)) return false;
			return true;
		}
		public void BeginSpaceshipCreation(SpaceshipClass spaceshipClass)
		{
			if (CanCreateMoreShips){// && SelectedSpaceshipClass.Id != spaceshipClass.Id) {
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

			if (SelectedSpaceshipClass!=null && CanCreateMoreShips)
				CreatedSpaceship = new GothicSpaceship(Game, new Position(), SelectedSpaceshipClass, Player);
			MaxPoints--;
			if (!CanCreateMoreShips) {
				creatingSpaceship = false;
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
			if (CanCreateMoreShips) {
				creatingSpaceship = true;
				OnPositioningBegin(this, new EventArgs());
			}
		}
	}
}
