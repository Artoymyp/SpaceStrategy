using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceStrategy
{
	internal class AnimationHelper
	{
		internal static Game Game { get; set; }
		internal static void CreateAnimation(AnimationObject animation)
		{
			Game.AddAnimation(animation);
		}
		internal static Color AnimateColor(double phase, Color startColor, Color endColor)
		{
			return AnimateColor(phase, startColor, endColor, AnimationMode.Linear);
		}
		internal static Color AnimateColor(double phase, Color startColor, Color endColor, AnimationMode mode)
		{
			if (phase > 1)
				phase = 1;
			if (phase < 0)
				phase = 0;
			return Color.FromArgb(
				AnimateInt(phase, startColor.A, endColor.A, mode),
				AnimateInt(phase, startColor.R, endColor.R, mode),
				AnimateInt(phase, startColor.G, endColor.G, mode),
				AnimateInt(phase, startColor.B, endColor.B, mode));
		}
		internal static int AnimateInt(double phase, double startValue, double endValue)
		{
			return (int)AnimateFloat(phase, startValue, endValue, AnimationMode.Linear);
		}
		internal static int AnimateInt(double phase, double startValue, double endValue, AnimationMode mode)
		{
			return (int)AnimateFloat(phase, startValue,endValue,mode);
		}
		internal static float AnimateFloat(double phase, double startValue, double endValue)
		{
			return (float)(startValue + (endValue - startValue) * phase);
		}
		internal static float AnimateFloat(double phase, double startValue, double endValue, AnimationMode mode)
		{
			switch (mode)
			{
				case AnimationMode.Linear:
					return AnimateFloat(phase, startValue, endValue);
				case AnimationMode.Sin:
					//var curValue = (startValue + (endValue - startValue) * phase);
					var val =1- Math.Sqrt(1 - phase * phase);
					return (float)(startValue + (endValue - startValue) * val);
				case AnimationMode.Cos:
					var val1 = Math.Sqrt(phase);
					return (float)(startValue + (endValue - startValue) * val1);
				case AnimationMode.x8:
					var valx8 = Math.Pow(phase, 8);
					return (float)(startValue + (endValue - startValue) * valx8);
				default:
					throw new NotImplementedException();
			}
		}
	}
	public enum AnimationMode
	{
		Linear,
		Sin,
		Cos,
		x8
	}
}
