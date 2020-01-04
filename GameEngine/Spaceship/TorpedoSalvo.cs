using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceStrategy
{
	public class TorpedoSalvo: GothicSpaceshipBase
	{
		private List<Torpedo> torpedos = new List<Torpedo>();
		private int power;
		private TorpedoSalvoState torpedoState;
		private Color aimZoneColor;
		private static SpaceshipClass torpedoSalvoClass = new SpaceshipClass("Torpedo","",SpaceshipCategory.Ordnance,"Weapon",1,0,0,6,6,6,6,0,0,0,0,0);
        private TorpedoWeapon launcherWeapon;
        double speed;
        public TorpedoSalvo(Game game, Player owner, TorpedoWeapon launcherWeapon, Position position, int power, double speed, Color zoneColor)
			: base(game, position, torpedoSalvoClass, owner)
		{
			Power = power;
			this.speed = speed;
			MinRunBeforeTurn = 2 * speed;
			torpedoState = TorpedoSalvoState.Aiming;
			aimZoneColor = zoneColor;
            this.launcherWeapon = launcherWeapon;
			
			double halfWidth = torpedoWidth / 2;
			double maxTorpedoShift = Size / 2 - halfWidth;
			
			int fullRowsCount = power / salvoWidthCount;
			int lastRowCount = power - fullRowsCount;
			for (int row = 0; row < fullRowsCount; row++) {
				for (int column = 0; column < salvoWidthCount; column++) {
					torpedos.Add(new Torpedo(this, new Point2d(-(row - 1) * torpedoLength, column * torpedoWidth - maxTorpedoShift), (float)torpedoLength, (float)torpedoWidth));
				}
			}
			if (lastRowCount == 1) {
				torpedos.Add(new Torpedo(this, new Point2d(-(fullRowsCount - 1) * torpedoLength, 0), (float)torpedoLength, (float)torpedoWidth));
			}
			else if (lastRowCount == 2) {
				torpedos.Add(new Torpedo(this, new Point2d(-(fullRowsCount - 1) * torpedoLength, maxTorpedoShift), (float)torpedoLength, (float)torpedoWidth));
				torpedos.Add(new Torpedo(this, new Point2d(-(fullRowsCount - 1) * torpedoLength, -maxTorpedoShift), (float)torpedoLength, (float)torpedoWidth));
			}
            IsCollisionObject = false;                
		}
        public override double Speed { get { return speed; } }
        public Spaceship LauncherSpaceship { get { return launcherWeapon.OwnerSpaceship; } }
		int salvoWidthCount { get { return 3; } }
		double torpedoWidth { get { return Size / salvoWidthCount; } }
		double torpedoLength { get { return Size / 2; } }
		public int Power
		{
			get { return power; }
			set { power = value; }
		}
		public override void Draw(System.Drawing.Graphics dc)
		{
			var oldDC = dc.Save();
			dc.TranslateTransform((float)Position.Location.X, (float)Position.Location.Y);
			dc.RotateTransform((float)Position.Degree);

			switch (torpedoState) {
				case TorpedoSalvoState.Aiming:
					int alphaChannelIntensity = 64;
					Brush areaBrush = new SolidBrush(Color.FromArgb(alphaChannelIntensity, aimZoneColor));
					dc.FillRectangle(areaBrush, 0, -(float)Size / 2, Game.Size.Width*2, (float)Size);
					Pen areaPen = new Pen(aimZoneColor, Game.Params.TrajectoryLineThickness);
					dc.DrawLine(areaPen, 0, (float)Size / 2, Game.Size.Width * 2, (float)Size / 2);
					dc.DrawLine(areaPen, 0, -(float)Size / 2, Game.Size.Width * 2, -(float)Size / 2);
					break;
				case TorpedoSalvoState.Flying:
                case TorpedoSalvoState.Destroyed:
					foreach (var torpedo in torpedos) {
						torpedo.Draw(dc, Player.Color);
					}
					dc.DrawEllipse(new Pen(aimZoneColor, Game.Params.TrajectoryLineThickness/4), -(float)Size / 2, -(float)Size / 2, (float)Size, (float)Size);
					break;
			}
			dc.Restore(oldDC);
		}
		
		internal new double Size { get { return Game.Params.TorpedoeSize; } }
		internal void Launch(TorpedoWeapon w){
            IsCollisionObject = true;
            BlastMarkersAtBase.SetCurBlastMarkers(launcherWeapon.OwnerSpaceship.BlastMarkersAtBase);  
			torpedoState = TorpedoSalvoState.Flying;
		}
        public override void OnTime(TimeSpan dt)
        {
            foreach (var torpedo in torpedos)
            {
                torpedo.TryDestruction(dt);
            }
            torpedos.RemoveAll(a => a.State == TorpedoState.Destroyed);
            if (torpedos.Count == 0)
            {
                State = SpaceshipState.Removed;
                //Game.RemoveSpaceship(this);
            }
        }
        internal override void Attack(GothicSpaceshipBase attackedGothicSpaceship)
        {
            if (torpedoState == TorpedoSalvoState.Flying)
            {
                if (attackedGothicSpaceship is GothicSpaceship)
                {
                    if (attackedGothicSpaceship.BlastMarkersAtBase.Any())
                    {
                        OnNewBlastMarkerContact(this, new NewBlastMarkersContactEventArgs(attackedGothicSpaceship.BlastMarkersAtBase));
                    }
                    Side attackedSide = GetAttackedSide(this, attackedGothicSpaceship);
                    int armor = GetArmor(attackedSide);
                    int hitTorpedoCount = Dice.RolledDicesCount(6, Power, armor, string.Format("Торпедная атака по борту {0}", attackedSide.ToString()));
                    DestroyTorpedos(hitTorpedoCount, new TimeSpan());
                    attackedGothicSpaceship.Attacked(launcherWeapon, hitTorpedoCount, new TimeSpan());
                }
                else if (attackedGothicSpaceship is TorpedoSalvo)
                {
                    var attackedTorpedo = (attackedGothicSpaceship as TorpedoSalvo);
                    Attacked(attackedTorpedo.launcherWeapon, HitPoints, new TimeSpan());
                    attackedTorpedo.Attacked(this.launcherWeapon, attackedTorpedo.HitPoints, new TimeSpan());
                }
            }
        }
        internal override void Attacked(SpaceshipWeapon attacker, int damage, TimeSpan timeBeforeAttackImpact)
        {
            if (attacker is TurretWeapon)
            {
                if (damage > Power)
                {
                    Power = 0;
                    Game.ScriptManager.AddEvent(new InflictDamageToTorpedo(this, attacker as TurretWeapon, HitPoints, timeBeforeAttackImpact, maxTorpedoDestructionDelay));
                    //InflictDamage(HitPoints);
                }
                else
                {
                    Power = Power - damage;
                    DestroyTorpedos(damage, maxTorpedoDestructionDelay);
                    maxTorpedoDestructionDelay = new TimeSpan();
                }
            }
            else if (attacker is TorpedoWeapon)
            {
                SetMaxTorpedoDestructionDelay(DefaultDestructionMaxDelay);
                this.InflictDamage(HitPoints);
            }
            else if (damage >= HitPoints)
            {
                Game.ScriptManager.AddEvent(new InflictDamage(this, attacker, HitPoints, timeBeforeAttackImpact)); 
                //InflictDamage(HitPoints);                
            }
        }
        /// <summary>
        /// Задает время, в течении котого в случайные моменты будут взрываться торпеды.
        /// Вызывать перед вызовом TorpedoSalvo.Attacked, после чего время автоматически установится равным 0.
        /// </summary>
        /// <param name="delay"></param>
        public void SetMaxTorpedoDestructionDelay(TimeSpan delay)
        {
            maxTorpedoDestructionDelay = delay;
        }
        private TimeSpan maxTorpedoDestructionDelay;
        public override void InflictDamage(int damagePoints)
        {
            base.InflictDamage(damagePoints);
            if (hitPoints == 0)
            {
                DestroyTorpedos(Power, maxTorpedoDestructionDelay);
                IsCollisionObject = false;
                torpedoState = TorpedoSalvoState.Destroyed;
            }
            maxTorpedoDestructionDelay = new TimeSpan();
        }
        private void DestroyTorpedos(int destroyedTorpedoCount, TimeSpan torpedoDestructionMaxDelay)
        {            
            var aliveTorpedos = torpedos.Where(a => a.State == TorpedoState.Normal).ToList();
            for (int i = 0; i < destroyedTorpedoCount; i++)
            {
                if (aliveTorpedos.Count > 0)
                {
                    int destroyedTorpedoIndex = Game.rand.Next(aliveTorpedos.Count);
                    aliveTorpedos[destroyedTorpedoIndex].Destroy(new TimeSpan(0, 0, 0, 0, Game.rand.Next((int)torpedoDestructionMaxDelay.TotalMilliseconds)));
                    aliveTorpedos.Remove(aliveTorpedos[destroyedTorpedoIndex]);
                }
            }
        }
        protected TimeSpan DefaultDestructionMaxDelay { get { return new TimeSpan(0, 0, 0, 0, 50); } }
        protected override void PassingBlastMarkerEffect()
        {
            if (Dice.RollDices(6, 1, "Пролет торпедного залпа через маркеры взрыва.") >= 6)
            {
                SetMaxTorpedoDestructionDelay(DefaultDestructionMaxDelay);
                InflictDamage(hitPoints);
            }
        }
        private enum TorpedoSalvoState
        {
            Aiming,
            Flying,
            Destroyed
        }
		private enum TorpedoState
		{
			Normal,
			MarkedForDestruction,
			Destroyed
		}
		private class Torpedo
		{
			public Point2d position;
			public TimeSpan ExplodeAfter;
			public TorpedoState State;
			private Spaceship torpedoSalvo;
			public Torpedo(TorpedoSalvo torpedoSalvo, Point2d pos, float length, float width)
			{
				position = pos;
				State = TorpedoState.Normal;
				Length = length;
				Width = width;
				this.torpedoSalvo = torpedoSalvo;
			}
			private float Length { get; set; }
			private float Width { get; set; }
			public void Destroy(TimeSpan explodeAfter)
			{
				State = TorpedoState.MarkedForDestruction;
				ExplodeAfter = explodeAfter;
			}
			public bool TryDestruction(TimeSpan dt)
			{
				if (State == TorpedoState.MarkedForDestruction) {
					if (ExplodeAfter > dt) {
						ExplodeAfter -= dt;
						return false;
					}
					else {
						ExplodeAfter = new TimeSpan();
						State = TorpedoState.Destroyed;
                        AnimationHelper.CreateAnimation(new RoundExplosionAnimation(new Position(this.position + this.torpedoSalvo.Position.Location.ToVector() + new Vector(-Length / 2, 0)), new TimeSpan(0, 0, 0, 1), Length / 2, Length * 4));
						//AnimationHelper.CreateAnimation(new SpaceshipRoundExplosionAnimation(this.torpedoSalvo, new Position(this.position+new Vector(-Length/2,0)), new TimeSpan(0, 0, 0, 1), Length / 2, Length * 4));
						return true;
					}
				}
				else return false;
			}
			internal void Draw(System.Drawing.Graphics dc, Color normalColor)
			{
				switch (State) {
					case TorpedoState.Normal:
					case TorpedoState.MarkedForDestruction:
						var path = new System.Drawing.Drawing2D.GraphicsPath(new PointF[]{
								position,
								position + new Vector(-Length,+Width/2),
								position + new Vector(-Length,-Width/2)},
								new Byte[]{
								(byte)PathPointType.Start,
								(byte)PathPointType.Line,
								(byte)PathPointType.Line});
						dc.FillPath(new SolidBrush(normalColor), path);
						break;
					default:
						break;
				}
			}

		}
        
	}
}
