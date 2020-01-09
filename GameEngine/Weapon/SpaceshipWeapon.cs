using System;
using System.Collections.Generic;
using System.Drawing;

namespace SpaceStrategy
{
	[Flags]
	public enum Side
	{
		Left = 1,
		Front = 2,
		Right = 4,
		Back = 8,
		LeftFrontRight = Left | Front | Right,
		All = Left | Front | Right | Back
	}


	public enum WeaponType
	{
		Lance,
		Battery,
		Nova,
		Torpedo,
		Turret
	}


	public abstract class SpaceshipWeapon
	{
		internal int NormalPower;
		protected int power;

		protected SpaceshipWeapon(GothicSpaceship owner, Side side, float minRange, float range, int power, WeaponType type)
		{
			OwnerSpaceship = owner;
			SpaceshipSide = side;
			LineColor = GetColor(type);
			MinRange = minRange;
			Range = range;
			NormalPower = power;
			Power = power;
			IsUsed = false;
			Name = type.ToString();
		}

		protected SpaceshipWeapon(GothicSpaceship owner, GameDataSet.SpaceshipClassWeaponryRow data) :
			this(owner, GetSide(data.SpaceshipSide), data.MinRange, data.Range, data.Power, GetWeaponType(data.WeaponType)) { }

		public Game Game
		{
			get { return OwnerSpaceship.Game; }
		}

		public bool IsUsed { get; set; }

		public Color LineColor { get; }

		public float MinRange { get; }

		public string Name { get; }

		public GothicSpaceship OwnerSpaceship { get; }

		public virtual int Power
		{
			get { return !OwnerSpaceship.IsCrippled ? power : GeometryHelper.RoundUp(power / 2.0); }
			set { power = value; }
		}

		public float Range { get; }

		public Side SpaceshipSide { get; }

		public static WeaponType GetWeaponType(string weaponTypeName)
		{
			switch (weaponTypeName.ToLower()) {
				case "lance": return WeaponType.Lance;
				case "cannon": return WeaponType.Battery;
				case "nova": return WeaponType.Nova;
				case "torpedo": return WeaponType.Torpedo;
				default:
					throw new ArgumentException("Неверный тип оружия.");
			}
		}

		internal abstract void Attack(GothicSpaceshipBase attackedSpaceship, IEnumerable<SpaceshipWeapon> allAttackingWeapons);

		static Color GetColor(WeaponType type)
		{
			switch (type) {
				case WeaponType.Lance:
					return Color.Red;

				case WeaponType.Battery:
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

		static Side GetSide(string sideName)
		{
			switch (sideName.ToLower()) {
				case "left": return Side.Left;
				case "right": return Side.Right;
				case "front": return Side.Front;
				case "lfr": return Side.LeftFrontRight;
				case "all": return Side.All;
				default:
					throw new ArgumentException("Неверный тип стороны корабля.");
			}
		}
	}
}