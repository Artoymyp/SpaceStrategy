using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpaceStrategy
{
	public class NextBattlePhaseEventArgs
	{
		public Player CurrentPlayer;
		public GamePhase battlePhase;

		public NextBattlePhaseEventArgs(Player CurrentPlayer, GamePhase battlePhase)
		{
			this.CurrentPlayer = CurrentPlayer;
			this.battlePhase = battlePhase;
		}
	}
}
