using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceStrategy
{
	public static class GunneryTable
	{
		public static int GetDiceCount(int firepower, SpaceshipCategory targetCategory, TargetOrientation targetOrientation, double distanceToTarget, double targetMoveDistance, bool blastMarkersOnLineOfFire)
		{
			if (firepower <= 0)
				return 0;

			if (targetMoveDistance < 5)
				targetCategory = SpaceshipCategory.Defense;
			
			int columnIndex = 0;
			switch (targetCategory) {
				case SpaceshipCategory.CapitalShip:
					columnIndex = 1;
					break;
				case SpaceshipCategory.Escort:
					columnIndex = 2;
					break;
				case SpaceshipCategory.Ordnance:
					columnIndex = 4;
					break;
				case SpaceshipCategory.Defense:
					columnIndex = 0;
					break;
				default:
					break;
			}
			if (targetCategory!= SpaceshipCategory.Defense && targetCategory != SpaceshipCategory.Ordnance) {
				if (targetOrientation == TargetOrientation.MovingAway) {
					columnIndex += 1;
				}
				else if (targetOrientation == TargetOrientation.Abeam){
					columnIndex += 2;
				}
			}
			if (blastMarkersOnLineOfFire)
				columnIndex += 1;
			if (distanceToTarget < 15)
				columnIndex -= 1;
			if (distanceToTarget > 30)
				columnIndex += 1;
			if (columnIndex < 0)
				columnIndex = 0;
			if (columnIndex > 4)
				columnIndex = 4;
			return GetDiceCount(firepower, columnIndex);
		}
		static Dictionary<int, Row> items = new Dictionary<int, Row>();
		static GunneryTable()
		{
			items.Add(1,	new Row() { DamageDiceCount = new int[] { 1, 1, 1, 0, 0 } });
			items.Add(2,	new Row() { DamageDiceCount = new int[] { 2, 1, 1, 1, 0 } });
			items.Add(3,	new Row() { DamageDiceCount = new int[] { 3, 2, 2, 1, 1 } });
			items.Add(4,	new Row() { DamageDiceCount = new int[] { 4, 3, 2, 1, 1 } });
			items.Add(5,	new Row() { DamageDiceCount = new int[] { 5, 4, 3, 2, 1 } });
			items.Add(6,	new Row() { DamageDiceCount = new int[] { 5, 4, 3, 2, 1 } });
			items.Add(7,	new Row() { DamageDiceCount = new int[] { 6, 5, 4, 2, 1 } });
			items.Add(8,	new Row() { DamageDiceCount = new int[] { 7, 6, 4, 3, 2 } });
			items.Add(9,	new Row() { DamageDiceCount = new int[] { 8, 6, 5, 3, 2 } });
			items.Add(10,	new Row() { DamageDiceCount = new int[] { 9, 7, 5, 4, 2 } });
			items.Add(11,	new Row() { DamageDiceCount = new int[] { 10, 8, 6, 4, 2 } });
			items.Add(12,	new Row() { DamageDiceCount = new int[] { 11, 8, 6, 4, 2 } });
			items.Add(13,	new Row() { DamageDiceCount = new int[] { 12, 9, 7, 5, 3 } });
			items.Add(14,	new Row() { DamageDiceCount = new int[] { 13, 10, 7, 5, 3 } });
			items.Add(15,	new Row() { DamageDiceCount = new int[] { 14, 11, 8, 5, 3 } });
			items.Add(16,	new Row() { DamageDiceCount = new int[] { 14, 11, 8, 6, 3 } });
			items.Add(17,	new Row() { DamageDiceCount = new int[] { 15, 12, 9, 6, 3 } });
			items.Add(18,	new Row() { DamageDiceCount = new int[] { 16, 13, 9, 6, 4 } });
			items.Add(19,	new Row() { DamageDiceCount = new int[] { 17, 13, 10, 7, 4 } });
			items.Add(20,	new Row() { DamageDiceCount = new int[] { 18, 14, 10, 7, 4 } });
			
		}
		private static int GetDiceCount(int firepower,int columnIndex)
		{
			int result = 0;
			int maxFirepower = items.Count;
			int remainingFirepower = firepower;
			while (remainingFirepower > maxFirepower)
			{
				result += items[maxFirepower].DamageDiceCount[columnIndex];
				remainingFirepower -= maxFirepower;
			}

			result += items[remainingFirepower].DamageDiceCount[columnIndex];
			return result;
		}
		internal static bool GetBlastMarkersOnLine(IEnumerable<BlastMarker> markers, Point2d startPoint, Point2d endPoint){
			if (markers.Any(bm=>GeometryHelper.PerpendicularDistance(bm.Position.Location, startPoint, endPoint)<BlastMarker.CollisionRadius))
				return true;
			else return false;
		}
		private class Row
		{
			public int[] DamageDiceCount;
		}
	}
	public enum TargetOrientation{
		Stationary=0,
		Closing,
		MovingAway,
		Abeam
	}
}
