namespace SpaceStrategy
{
	public class NextBattlePhaseEventArgs
	{
		public GamePhase BattlePhase;
		public Player CurrentPlayer;

		public NextBattlePhaseEventArgs(Player currentPlayer, GamePhase battlePhase)
		{
			CurrentPlayer = currentPlayer;
			BattlePhase = battlePhase;
		}
	}
}