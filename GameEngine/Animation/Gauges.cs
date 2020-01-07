using System.Drawing;

namespace SpaceStrategy
{
	static class Gauges
	{
		internal static float Height
		{
			get { return 0.5F; }
		}

		static Brush EmptyGaugeBrush
		{
			get { return Brushes.Gray; }
		}

		public static void Draw(Graphics dc, Color color, int maxVal, int curVal, float width)
		{
			if (maxVal > 0) {
				float segmentWidth = width / maxVal;
				Brush gaugeBrush = new SolidBrush(color);

				float gaugeStart = -width * 0.5F;
				float filledSegmentWidth = segmentWidth * curVal;
				if (filledSegmentWidth > 0.001) {
					dc.FillRectangle(gaugeBrush, gaugeStart, 0, filledSegmentWidth, Height);
				}

				float emptySegmentWidth = width - filledSegmentWidth;
				if (emptySegmentWidth > 0.001) {
					dc.FillRectangle(EmptyGaugeBrush, gaugeStart + filledSegmentWidth, 0, emptySegmentWidth, Height);
				}

				if (maxVal > 1) {
					var separatorPen = new Pen(Brushes.Black, 0);
					float curSeparatorX = gaugeStart + segmentWidth;
					for (int i = 1; i < maxVal; i++) {
						dc.DrawLine(separatorPen, curSeparatorX, 0, curSeparatorX, Height);
						curSeparatorX += segmentWidth;
					}
				}
			}
		}
	}
}