using System;
using System.Windows.Forms;
using SpaceStrategy;

namespace UI
{
	public partial class StarfieldControl : UserControl
	{
		public StarfieldControl()
		{
			InitializeComponent();
			DoubleBuffered = true;
		}

		public Game GameEngine { get; set; }

		void StarfieldControl_MouseEnter(object sender, EventArgs e)
		{
			Cursor.Hide();
		}

		void StarfieldControl_MouseLeave(object sender, EventArgs e)
		{
			Cursor.Show();
		}

		void StarfieldControl_Paint(object sender, PaintEventArgs e)
		{
			if (GameEngine != null) {
				GameEngine.OnDraw(e.Graphics);
				Update();
			}
		}
	}
}