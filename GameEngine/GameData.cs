using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using SpaceStrategy.GameDataSetTableAdapters;
using SpaceStrategy.Properties;

namespace SpaceStrategy
{
	public class GameDataAdapter
	{
		public GameDataAdapter()
		{
			Races = new GameDataSet.RaceDataTable();
			var raceTableAdapter = new RaceTableAdapter();
			raceTableAdapter.Fill(Races);
		}

		public GameDataSet.RaceDataTable Races { get; }

		public static IEnumerable<GothicSpaceshipBonus> GetBonusesByClassName(string className)
		{
			string conn = Settings.Default.GameDataConnectionString;
			var ds = new DataSet();
			var da = new SqlDataAdapter();
			string q = @"
				SELECT	Bonuses.Name, Bonuses.Description, SpaceshipClassBonuses.SpaceshipClass, SpaceshipClassBonuses.BonusId, SpaceshipClassBonuses.Id
				FROM	SpaceshipClassBonuses INNER JOIN Bonuses
				ON		SpaceshipClassBonuses.BonusId = Bonuses.Id
				WHERE	(SpaceshipClassBonuses.SpaceshipClass = @SpaceshipClass)";
			da.SelectCommand = new SqlCommand(q, new SqlConnection(conn));
			da.SelectCommand.Parameters.Add(new SqlParameter("@SpaceshipClass", className));
			da.Fill(ds);

			var bonuses = new List<GothicSpaceshipBonus>();
			if (ds != null) {
				foreach (DataRow bonusDataRow in ds.Tables[0].Rows) bonuses.Add(new GothicSpaceshipBonus((string)bonusDataRow.ItemArray[0], (string)bonusDataRow.ItemArray[1]));
			}

			return bonuses;
		}

		public GameDataSet.SpaceshipClassDataTable GetSpaceshipClassesByRaceId(string raceId)
		{
			var ta = new SpaceshipClassTableAdapter();
			return ta.GetDataByRaceId(raceId);
		}

		public List<GameDataSet.SpaceshipClassRow> GetSpaceshipClassesByRaceId(string raceId, int maxSpaceshipPoints)
		{
			var ta = new SpaceshipClassTableAdapter();

			//var result = new SpaceStrategy.GameDataSet.SpaceshipClassDataTable();
			//foreach (var row in ta.GetDataByRaceId(raceId)) {
			//	if (row.Points<=maxSpaceshipPoints)

			//}
			return ta.GetDataByRaceId(raceId).Where(a => a.Points < maxSpaceshipPoints).ToList();
		}
	}
}