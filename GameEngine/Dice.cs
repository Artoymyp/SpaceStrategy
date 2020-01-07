using System;

namespace SpaceStrategy
{
	public class Dice
	{
		static Random Rand
		{
			get { return Game.Rand; }
		}

		internal static int RollDices(int sides, int count, string description)
		{
			int result = 0;
			string text = string.Format("({0}d{1}: ", count, sides);
			for (int i = 0; i < count; i++) {
				int value = Rand.Next(sides) + 1;
				result += value;
				text += value + (i == count - 1 ? ")" : ", ");
			}

			text = string.Format("{0} Результат = {1} {2}", description, result, text);
			if (description != string.Empty) {
				GamePrinter.AddLine(text);
			}

			return result;
		}

		internal static int RolledDicesCount(int sides, int count, int needAtLeast, string description)
		{
			int result = 0;
			if (count <= 0) {
				return 0;
			}

			string text = string.Format("({0}d{1} {2}+: ", count, sides, needAtLeast);
			for (int i = 0; i < count; i++) {
				int value = RollDices(6, 1, string.Empty);
				if (value >= needAtLeast) {
					result++;
				}

				text += value + (i == count - 1 ? ")" : ", ");
			}

			text = string.Format("{0} Результат = {1} {2}", description, result, text);
			GamePrinter.AddLine(text);
			return result;
		}
	}
}