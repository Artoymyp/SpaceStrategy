using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace SpaceStrategy
{
	public static class GamePrinter
	{
		static List<string> _Lines = new List<string>();

		static GamePrinter()
		{
			TopLeftCorner = new Point2d(0, 50);
			MaxRowsCount = 10;
			Font = new Font("CurierNew", 8, FontStyle.Regular);
			Color = Color.Lime;
			Brush = new SolidBrush(Color);
		}

		public static Brush Brush { get; set; }

		public static Color Color { get; set; }

		public static Font Font { get; set; }

		public static int MaxRowsCount { get; set; }

		public static Point2d TopLeftCorner { get; set; }

		public static void AddLine(string newLine)
		{
			_Lines.Insert(0, newLine);
			_Lines = _Lines.Take(MaxRowsCount).ToList();
		}

		public static void Draw(Graphics dc)
		{
			for (int i = 0; i < Math.Min(MaxRowsCount, _Lines.Count); i++) {
				Point2d p = TopLeftCorner + new Vector(0, Font.SizeInPoints * i * 1.5);
				var linePosition = new Point((int)p.X, (int)p.Y);
				TextRenderer.DrawText(dc, _Lines[i], Font, linePosition, Color);

				//PointF linePosition = TopLeftCorner + new Vector(0, (float)(Font.SizeInPoints * i) * 1.5);
				//dc.DrawString(lines[i], Font, Brush, linePosition);
			}
		}

		public static void Draw(Graphics dc, string text, PointF position, Font f, Brush b)
		{
			//dc.DrawString(text, f, b, position);
			var linePosition = new Point((int)position.X, (int)position.Y);
			TextRenderer.DrawText(dc, text, Font, linePosition, Color);
		}
	}
}