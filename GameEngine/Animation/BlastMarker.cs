using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceStrategy
{
    public class BlastMarker : GraphicObject
    {
        BlastMarkerAnimation animation;
        internal BlastMarker(Game game, Position position, TimeSpan showBMAfter)
            : base()
        {
            Position = position;
            animation = new BlastMarkerAnimation(position);
            game.ScriptManager.AddEvent(new ShowAnimationEvent(animation, showBMAfter));
            //AnimationHelper.CreateAnimation(animation);
        }
        public override void Draw(System.Drawing.Graphics dc)
        {}
        public void Dispose()
        {
            if (animation!=null)
                animation.Drop();
        }
        public static double CollisionRadius { get { return 1.35; } }
    }
}
