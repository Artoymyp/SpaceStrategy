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
        private bool isCollisionObject = true;
        public virtual bool IsCollisionObject { get { return isCollisionObject; } set { isCollisionObject = value; } }
		public abstract void Draw(Graphics dc);
        bool isVisible = true;
        public bool IsVisible { get { return isVisible; } internal set { isVisible=value;} }

	}
}