using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpaceStrategy
{
	public class NextBattlePhaseEventArgs
	{
		public Player CurrentPlayer;
		public GamePhase BattlePhase;

		public NextBattlePhaseEventArgs(Player currentPlayer, GamePhase battlePhase)
		{
			this.CurrentPlayer = currentPlayer;
			this.BattlePhase = battlePhase;
		}
	}
}
