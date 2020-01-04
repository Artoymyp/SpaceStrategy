using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceStrategy
{
	public class Player
	{
		public Player(Game game, string name, string race, Color color)
		{
			this.Game = game;
			Name = name;
			Race = race;
			Color = color;
			SpecialOrderFail = false;
			Points = 0;
		}
		public int Points { get; internal set; }
		public PositioningZone PositioningZone { get; internal set; }
		public string Name { get; private set; }
		public Color Color { get; private set; }
		public string Race { get; private set; }
		public IEnumerable<GothicSpaceship> Spaceships { get { return Game.GothicSpaceships.Where(a => a.Player == this); } }
		bool specialOrderFail;
		public bool SpecialOrderFail
		{
			get { return specialOrderFail; }
			internal set
			{
				if (specialOrderFail != value)
				{
					specialOrderFail = value;
					
					if (Game.SelectedSpaceship != null)
					{
						Game.SelectedSpaceship.NotifyPropertyChanged("AvailableOrders");
					}
				}
			}
		}
		public override string ToString()
		{
			return Name;
		}

		public Game Game { get; set; }
	}
}
