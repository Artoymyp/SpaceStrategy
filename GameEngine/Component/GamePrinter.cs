using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpaceStrategy
{
	public static class GamePrinter
	{
		static List<string> lines = new List<string>();
		static GamePrinter() {
			TopLeftCorner = new Point2d(0, 50);
			MaxRowsCount = 10;
			Font = new Font("CurierNew", 8, FontStyle.Regular);
            Color = Color.Lime;
			Brush = new SolidBrush(Color);
		}
		static public void AddLine(string newLine)
		{
			lines.Insert(0, newLine);
            lines = lines.Take(MaxRowsCount).ToList();
		}
		static public Point2d TopLeftCorner { get; set; }
		static public int MaxRowsCount {get; set;}
		static public Font Font { get; set; }
        static public Color Color { get; set; }
		static public Brush Brush { get; set; }
		static public void Draw(Graphics dc)
		{
            for (int i = 0; i < Math.Min(MaxRowsCount, lines.Count); i++) {
                var p = TopLeftCorner + new Vector(0, (float)(Font.SizeInPoints * i) * 1.5);
                Point linePosition = new Point((int)p.X, (int)p.Y);
				TextRenderer.DrawText(dc, lines[i], Font, linePosition, Color);
                //PointF linePosition = TopLeftCorner + new Vector(0, (float)(Font.SizeInPoints * i) * 1.5);
                //dc.DrawString(lines[i], Font, Brush, linePosition);
			}
		}
        static public void Draw(Graphics dc, string text, PointF position, Font f, Brush b)
        {
            //dc.DrawString(text, f, b, position);
            Point linePosition = new Point((int)position.X, (int)position.Y);
            TextRenderer.DrawText(dc, text, Font, linePosition, Color);                
        }
	}
}
