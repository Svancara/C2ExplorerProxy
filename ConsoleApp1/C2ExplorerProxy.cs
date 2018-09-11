using C2ChartsLibrary.Enums;
using C2ChartsLibrary.Interfaces;
using C2ExplorerDB;
using C2ExplorerServiceStack.Logic.Implementation;
using C2ExplorerServiceStack.Logic.Interfaces;
using C2ExplorerServiceStack.Logic.Interfaces.C2Api;
using LinqToDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
	public class C2ExplorerProxy
	{
		// This simulates variables used in C2Explorer scripts.
		protected IC2HighChart CHART;
		protected IHtmlFrame FRAME;
		protected String TEXT;
		protected String H1;
		protected String H2;
		protected String H3;
		protected String H4;
		protected String H5;
		protected String HTML;
		protected IEnumerable<Object> TABLE;
		protected IEnumerable<long> NUMTABLE;
		protected ITimeSheet TIMESHEET;
		protected ITimeLine C2TIMELINE;
		protected ITimeSeriesChart TSCHART;
		protected ITable<c2ex_publicsystems> C2SYSTEMS;
		protected ITable<c2ex_closedtrades> C2TRADES;
		protected ITable<c2ex_closedsignals> C2SIGNALS;
		protected ITable<c2ex_stats> C2STATS;
		protected ITable<c2ex_accountequity> C2EQUITY;

		IQueryRunner queryRunner = new QueryRunner();

		public C2ExplorerProxy()
		{
			queryRunner.InitDb();

			C2SYSTEMS = queryRunner.C2SYSTEMS;
			C2TRADES = queryRunner.C2TRADES;
			C2SIGNALS = queryRunner.C2SIGNALS;
			C2EQUITY = queryRunner.C2EQUITY;
			C2STATS = queryRunner.C2STATS;
		}

		protected ICommissions CommissionsFactory(String planId = C2ExplorerServiceStack.Logic.Implementation.Globals.DefaultCommissPlan)
		{
			return queryRunner.CommissionsFactory(planId);
		}

		protected IEnumerable<IAccountEquityPoint> GetAccountEquity(int ID)
		{
			return queryRunner.GetAccountEquity(ID);
		}

		protected IC2TradingSystem GetC2SYSTEM(long ID)
		{
			return queryRunner.GetC2SYSTEM(ID);
		}

		protected IEnumerable<IChartTimeSeries> GetEquities(IEnumerable<long> systemsIds)
		{
			return queryRunner.GetEquities(systemsIds);
		}

		protected IEnumerable<object> GetEquitiesSheet(long[] systems, TimeInterval timeInterval, EquityType equityType)
		{
			ITimeSheet timeSheet = queryRunner.TimeSheetFactory(systems, timeInterval, equityType);
			return timeSheet.GetEquitiesSheet();
		}

		protected IEnumerable<object> GetEquitiesSheet(long system, TimeInterval timeInterval, EquityType equityType)
		{
			return this.GetEquitiesSheet(new long[] { system }, timeInterval, equityType);
		}

		protected IC2HighChart ColumnChart(string titleText, string categoriesSeriesName, string dataSeriesName, string[] categories, Decimal[] series, string subtitleText = "", Boolean spline = true, C2ExplorerThemeName themeName = C2ExplorerThemeName.DarkUnica, bool navigator = false)
		{
			return queryRunner.ColumnChart(titleText, categoriesSeriesName, dataSeriesName, categories, series, subtitleText, spline, themeName, navigator);
		}

		protected ITimeSheet TimeSheetFactory(
			IEnumerable<long> systemsIds,
			TimeInterval timeInterval = TimeInterval.Month,
			EquityType equityType = EquityType.Equity,
			String planId = C2ExplorerServiceStack.Logic.Implementation.Globals.DefaultCommissPlan)
		{
			return queryRunner.TimeSheetFactory(systemsIds, timeInterval, equityType, planId);
		}

		protected void HR()
		{
			queryRunner.HR();
		}

	}
}
