using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace SpaceStrategy
{
	public abstract class SpaceshipWeapon
	{
		internal int normalPower;
		protected int power;
		public SpaceshipWeapon(GothicSpaceship owner, Side side, float minRange, float range, int power, WeaponType type)
		{
			OwnerSpaceship = owner;
			SpaceshipSide = side;
			LineColor = GetColor(type);
			MinRange = minRange;
			Range = range;
			normalPower = power;
			Power = power;
			IsUsed = false;
			Name = type.ToString();
		}
		public SpaceshipWeapon(GothicSpaceship owner, GameDataSet.SpaceshipClassWeaponryRow data) :
			this(owner, GetSide(data.SpaceshipSide), data.MinRange, data.Range, data.Power, GetWeaponType(data.WeaponType))
		{ }
		public string Name { get; private set; }
		public Game Game { get { return OwnerSpaceship.Game; } }		
		public bool IsUsed { get; set; }
		public GothicSpaceship OwnerSpaceship { get; private set; }
		public Color LineColor { get; private set; }
		public Side SpaceshipSide { get; private set; }
		public virtual int Power
		{
			get { return !OwnerSpaceship.IsCrippled ? power : GeometryHelper.RoundUp((double)power / 2.0); }
			set { power = value; }
		}
		public float Range { get; private set; }
		public float MinRange { get; private set; }
		internal abstract void Attack(GothicSpaceshipBase attackedSpaceship, IEnumerable<SpaceshipWeapon> allAttackingWeapons);
		public static WeaponType GetWeaponType(string weaponTypeName)
		{
			switch (weaponTypeName.ToLower()) {
				case "lance": return WeaponType.Lance;
				case "cannon": return WeaponType.Battary;
				case "nova": return WeaponType.Nova;
				case "torpedo": return WeaponType.Torpedo;
				default:
					throw new ArgumentException("Неверный тип оружия.");
			}
		}
		private static Side GetSide(string sideName)
		{
			switch (sideName.ToLower()) {
				case "left": return Side.Left;
				case "right": return Side.Right;
				case "front": return Side.Front;
				case "lfr": return Side.LFR;
				case "all": return Side.All;
				default:
					throw new ArgumentException("Неверный тип стороны корабля.");
			}
		}
		private static Color GetColor(WeaponType Type)
		{
			switch (Type) {
				case WeaponType.Lance:
					return Color.Red;
				case WeaponType.Battary:
					return Color.Orange;
				case WeaponType.Nova:
					return Color.Purple;
				case WeaponType.Torpedo:
					return Color.Green;
				case WeaponType.Turret:
					return Color.Yellow;
				default:
					return Color.White;
			}
		}
	}
	[Flags]
	public enum Side
	{
		Left=1,
		Front=2,
		Right=4,
		Back=8,
		LFR = Left | Front | Right,
		All = Left | Front | Right | Back
	}
	public enum WeaponType
	{
		Lance,
		Battary,
		Nova,
		Torpedo,
		Turret
	}
}
