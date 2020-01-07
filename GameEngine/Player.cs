using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace SpaceStrategy
{
	public class Player
	{
		bool _specialOrderFail;

		public Player(Game game, string name, string race, Color color)
		{
			Game = game;
			Name = name;
			Race = race;
			Color = color;
			SpecialOrderFail = false;
			Points = 0;
		}

		public Color Color { get; }

		public Game Game { get; set; }

		public string Name { get; }

		public int Points { get; internal set; }

		public PositioningZone PositioningZone { get; internal set; }

		public string Race { get; }

		public IEnumerable<GothicSpaceship> Spaceships
		{
			get { return Game.GothicSpaceships.Where(a => a.Player == this); }
		}

		public bool SpecialOrderFail
		{
			get { return _specialOrderFail; }
			internal set
			{
				if (_specialOrderFail != value) {
					_specialOrderFail = value;

					if (Game.SelectedSpaceship != null) {
						Game.SelectedSpaceship.NotifyPropertyChanged("AvailableOrders");
					}
				}
			}
		}

		public override string ToString()
		{
			return Name;
		}
	}
}