using System.Drawing;

namespace SpaceStrategy
{
	public abstract class GraphicObject
	{
		public virtual bool IsCollisionObject { get; set; } = true;

		public bool IsVisible { get; internal set; } = true;

		public Position Position { get; set; }

		public abstract void Draw(Graphics dc);
	}
}