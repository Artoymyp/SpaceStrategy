using System;
using System.Drawing;

namespace SpaceStrategy
{
	class AnimationHelper
	{
		internal static Game Game { get; set; }

		internal static Color AnimateColor(double phase, Color startColor, Color endColor)
		{
			return AnimateColor(phase, startColor, endColor, AnimationMode.Linear);
		}

		internal static Color AnimateColor(double phase, Color startColor, Color endColor, AnimationMode mode)
		{
			if (phase > 1) {
				phase = 1;
			}

			if (phase < 0) {
				phase = 0;
			}

			return Color.FromArgb(
				AnimateInt(phase, startColor.A, endColor.A, mode),
				AnimateInt(phase, startColor.R, endColor.R, mode),
				AnimateInt(phase, startColor.G, endColor.G, mode),
				AnimateInt(phase, startColor.B, endColor.B, mode));
		}

		internal static float AnimateFloat(double phase, double startValue, double endValue)
		{
			return (float)(startValue + (endValue - startValue) * phase);
		}

		internal static float AnimateFloat(double phase, double startValue, double endValue, AnimationMode mode)
		{
			switch (mode) {
				case AnimationMode.Linear:
					return AnimateFloat(phase, startValue, endValue);

				case AnimationMode.Sin:
					//var curValue = (startValue + (endValue - startValue) * phase);
					double val = 1 - Math.Sqrt(1 - phase * phase);
					return (float)(startValue + (endValue - startValue) * val);

				case AnimationMode.Cos:
					double val1 = Math.Sqrt(phase);
					return (float)(startValue + (endValue - startValue) * val1);

				case AnimationMode.Pow8:
					double pow8 = Math.Pow(phase, 8);
					return (float)(startValue + (endValue - startValue) * pow8);

				default:
					throw new NotImplementedException();
			}
		}

		internal static int AnimateInt(double phase, double startValue, double endValue)
		{
			return (int)AnimateFloat(phase, startValue, endValue, AnimationMode.Linear);
		}

		internal static int AnimateInt(double phase, double startValue, double endValue, AnimationMode mode)
		{
			return (int)AnimateFloat(phase, startValue, endValue, mode);
		}

		internal static void CreateAnimation(AnimationObject animation)
		{
			Game.AnimationManager.AddAnimation(animation);
		}
	}
}