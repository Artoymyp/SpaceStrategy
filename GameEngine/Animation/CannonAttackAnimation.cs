using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SpaceStrategy.Animation
{
    internal class CannonAttackAnimation : AnimationObject
    {
        public CannonAttackAnimation(CannonWeapon source, Spaceship target)//, int damage)
            : base(target.Game, new TimeSpan(), false)
        {
            this.target = target;
            this.source = source;
            List<int> weaponIndexes = new List<int>();

            int totalPower = source.Power;
            if (totalPower == 0)
                return;
            for (int i = 0; i < totalPower; i++)
			{
                weaponIndexes.Add(i);
			}
            workingWeapons = GeometryHelper.GetRandomItems(weaponIndexes, totalPower);
            List<int> hitWeapons = GeometryHelper.GetRandomItems(workingWeapons, workingWeapons.Count);//damage);

            TimeSpan blobStartTime = new TimeSpan();
            TimeSpan maxBlobLifeTime = new TimeSpan();
            for (int i = 0; i < totalPower; i++)
            {
                if (!workingWeapons.Contains(i))
                {
                    allFireBlobs.Add(null);
                    continue;
                }
                var weaponPos = source.OwnerSpaceship.weaponPlacements[source][i];
                Point2d globalWeaponPos = weaponPos.Location.TransformBy(source.OwnerSpaceship.Position);
                double randX=(Game.rand.NextDouble()-0.5) * target.Diameter;
                double randY=(Game.rand.NextDouble()-0.5) * target.Diameter;
                Vector targetShift = new Vector(randX, randY);
                Vector attackDir = globalWeaponPos.VectorTo(target.Position.Location + targetShift);
                double attackDist = attackDir.Length;
                attackDir.Normalize();
                allFireBlobs.Add(new FireBlob(new Position(globalWeaponPos, attackDir), attackDist,hitWeapons.Contains(i)));
                
                //Calculating animation duration;
                double curBlobLifeTimeMs = attackDist / FireBlob.UnitsPerSecSpeed*1000;
                TimeSpan curBlobLifeTime = blobStartTime + new TimeSpan(0,0,0,0,(int)curBlobLifeTimeMs);
                if (curBlobLifeTime > maxBlobLifeTime) {
                    maxBlobLifeTime = curBlobLifeTime;
                }
                blobStartTime += FireSpan;
            }
            TimeFromLastFire = FireSpan;
            AnimationDuration = maxBlobLifeTime;
        }
        List<FireBlob> allFireBlobs = new List<FireBlob>();
        List<FireBlob> fireBlobs = new List<FireBlob>();
        TimeSpan TimeFromLastFire;
        static TimeSpan FireSpan { get { return new TimeSpan(0, 0, 0, 0, 30); } }
        List<Vector> randWeaponAttackDistComponent = new List<Vector>();
        List<int> workingWeapons = new List<int>();
        static Color color { get { return Color.DeepSkyBlue; } }
        Spaceship target;
        CannonWeapon source;
        internal override void OnTime(TimeSpan dt)
        {
            fireBlobs.RemoveAll(a => a.Complete);
            foreach (var blob in fireBlobs)
            {
                blob.Move(dt);
            }
            if (TimeFromLastFire >= FireSpan)
            {
                if (allFireBlobs.Count > 0)
                {
                    var curBlob = allFireBlobs.First();
                    allFireBlobs.RemoveAt(0);
                    if (curBlob != null)
                    {
                        fireBlobs.Add(curBlob);
                    }
                }
                TimeFromLastFire = new TimeSpan();
            }
            TimeFromLastFire += dt;
            if (allFireBlobs.Count == 0 && fireBlobs.Count == 0)
                Drop();
        }
        internal override void Draw(Graphics dc)
        {
            foreach (var blob in fireBlobs)
            {
                blob.Draw(dc);
            }
        }
        private void DrawLaserLine(Graphics dc, Point2d source, Point2d target)
        {
            Pen p = new Pen(color, 0.05F);
            dc.DrawLine(p, source, target);            
        }
        private void DrawLaserBlob(Graphics dc, RectangleF blobRect, Point2d point)
        {
            if (blobRect != null)
                dc.FillEllipse(new SolidBrush(color), blobRect);

        }
        private class FireBlob
        {
            public FireBlob(Position startPos, double maxDistance, bool placeExplosion)
            {
                Position = startPos;
                this.maxDist = maxDistance;
                this.placeExplosion = placeExplosion;
            }
            bool placeExplosion;
            Position Position;
            double distance=0;
            double maxDist;
            public bool Complete = false;
            public void Move(TimeSpan dt)
            {
                if (Complete)
                    return;
                var distIncrement = (float)dt.Milliseconds / (float)1000 * UnitsPerSecSpeed;
                Position.Location = Position.Location + Position.Direction * distIncrement;
                distance+=distIncrement;
                if (distance >= maxDist)
                {
                    Complete = true;
                    AnimationHelper.CreateAnimation(new RoundExplosionAnimation(Position, new TimeSpan(0, 0, 0, 0, 1000), 0, 1.5F));
                }
            }
            public static double UnitsPerSecSpeed { get { return 40; } }
            static float Radius { get { return 0.4F; } }
            public void Draw(Graphics dc)
            {

                RectangleF rect = new RectangleF((float)Position.Location.X - Radius, (float)Position.Location.Y - Radius, Radius * 2, Radius * 2);
                dc.FillEllipse(new SolidBrush(CannonAttackAnimation.color), rect);
            }
        }
    }
}
