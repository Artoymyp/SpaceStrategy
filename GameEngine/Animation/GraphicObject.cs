using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace SpaceStrategy
{
	public abstract class GraphicObject
	{
		public Position Position { get; set; }
		private bool _isCollisionObject = true;
		public virtual bool IsCollisionObject { get { return _isCollisionObject; } set { _isCollisionObject = value; } }
		public abstract void Draw(Graphics dc);
		bool _isVisible = true;
		public bool IsVisible { get { return _isVisible; } internal set { _isVisible=value;} }

	}
}