using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SpaceStrategy;

namespace UI
{
	public partial class StarfieldControl : UserControl
	{
		public Game GameEngine { get; set; }
		public StarfieldControl()
		{
			InitializeComponent();
			this.DoubleBuffered = true;
		}

		private void StarfieldControl_Paint(object sender, PaintEventArgs e)
		{
			if (GameEngine != null) {
				GameEngine.OnDraw(e.Graphics);
				this.Update();
			}
		}

		private void StarfieldControl_MouseEnter(object sender, EventArgs e)
		{
			Cursor.Hide();
		}

		private void StarfieldControl_MouseLeave(object sender, EventArgs e)
		{
			Cursor.Show();
		}
	}
}
