using C2ExplorerDB;
using C2ExplorerServiceStack.Logic.Interfaces;
using LinqToDB;
using System.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using C2ChartsLibrary.Implementation;
using C2ExplorerServiceStack.Logic.Implementation;
using Deedle;
using C2ExplorerServiceStack.Logic.Interfaces.C2Api;
using C2ChartsLibrary.Enums;
using System.Diagnostics;
using C2ChartsLibrary.Interfaces;

namespace ConsoleApp1
{
	public class MyCode : C2ExplorerProxy
	{
		public MyCode():base(){ }

		/// <summary>
		/// This code is based on the code developed by Chris Bayley
		/// </summary>
		public void Example01()
		{
			Console.WriteLine("Running Example01...");

			// Systems we want to see in the chart
			Tuple<long, String, Color>[] systemIds = {
				Tuple.Create(113004400L, "COREX", Color.Red),
				Tuple.Create(106901765L, "VIXTrader", Color.LightBlue),
				Tuple.Create(106600099L, "VIXTrader Pro", Color.LightGreen)};

			// Set a preliminary Start Date
			DateTime StartDate = DateTime.Parse("1-feb-2008");

			// Find earliest start date common to all systems
			foreach (var id in systemIds)
			{
				DateTime sysStartDate = (from s in C2SYSTEMS
										 where s.SystemId == id.Item1
										 select s.Started).First();

				if (DateTime.Compare(sysStartDate, StartDate) > 0)
				{
					StartDate = sysStartDate;
				}
			}

			// Get the Monthly equity data
			ITimeSheet timeSheet = TimeSheetFactory(systemIds.Select(id => id.Item1), TimeInterval.Month, EquityType.Equity);

			// ===================================================================
			// Here, a developer writes the exactly same code as on the web page.
			// ===================================================================
			// Say - a developer wants to play with timeSheet.
			// IntelliSense in action:
			var Developer_Trying_To_Do_Something = timeSheet.Commissions.CalcCommissions(systemIds[0].Item1);

			TABLE = timeSheet.GetEquitiesSheet();

			//// Create a chart object
			//ITimeSeriesChart chart = new TimeSeriesChart();
			//chart.Name = String.Format("Chris Bayley Chart");

			//// Add data
			//foreach (Tuple<long, String, Color> system in systemIds)
			//{
			//	chart.Add(timeSheet.GetColumn(system.Item1, EquityType.Equity), system.Item2, system.Item3);
			//}

			//// Enjoy!
			//CHART = chart;

			// return the EquitySheet as a Deedle.Frame
			var sparseEquityFrame = timeSheet.DataFrame;

			// Drop the rows from the frame which dont have data for all systems 
			var denseEquityFrame = FrameModule.DropSparseRows(sparseEquityFrame);

			// ??? QUESTION 1:
			// Now I have the data I want in denseEquityFrame and I wish to convert it to IEnumerable for display by TABLE=
			IEnumerable<string> rowKeyNames = new string[] { "KeyName" };
			var equityDataTable = denseEquityFrame.ToDataTable(rowKeyNames);
			// Next line yields this error:
			// *** Error: Error CS0266: 139:7 Cannot implicitly convert type 'System.Data.DataTable' to 
			// 'System.Collections.Generic.IEnumerable<object>'. An explicit conversion exists (are you missing a cast?)
			// If an explict conversion exits, what is it ??
			//TABLE=equityDataTable;

			// ??? QUESTION 2:
			// return Equity Sheet as IEnumerable to use with TABLE=
			// I can do this but it includes Dates I don't want 
			var equityTable = timeSheet.GetEquitiesSheet();
			// So I'd like to do this:
			// equityTable =  timeSheet.GetEquitiesSheet().Where(z => z.Item1 >= StartDate);
			// but that yields:
			// *** Error: Error CS1061: 146:62 'object' does not contain a definition for 'Item1' and no extension method 'Item1'
			// accepting a first argument of type 'object' could be found (are you missing a using directive or an assembly reference?)
			TABLE = equityTable;


			// ============ LOCAL DEBUGGING IN VISUAL STUDIO ========
			FrameExtensions.Print(denseEquityFrame);
			//FrameExtensions.Print(((Frame<DateTime, string>)equityTable.ElementAt(1)).GetFrameData());
			FrameExtensions.Print<DateTime, string>(timeSheet.DataFrame);

			Console.WriteLine("Example01 done. Press ENTER");
			Console.ReadLine();
		}

		/// <summary>
		/// This code is based on the code developed by Chris Bayley
		/// </summary>
		public void Example02()
		{

			Console.WriteLine("Running Example02...");

			// Setup
			var statName = "jSharpe";
			var low = 1.5M;
			var high = 2.5M;
			var howManySystemsInResult = 20;

			H1 = String.Format("Best by {0} of 2015. ( {1} < {0} < {2} )", statName, low, high);

			// Select systems according our setup
			var selectedStats = from stat in C2STATS
								where stat.StatName == statName
									  //                  && stat.CalcedWhen.Year == 2015
									  && stat.StatValueVal > low
									  && stat.StatValueVal < high
								select stat;

			// See tmp result:
			TABLE = selectedStats;

			Console.WriteLine("------ selectedStats --------");
			foreach (var stat in selectedStats) {
				Console.WriteLine($" StatName: {stat.StatName}, Value: {stat.StatValueVal}");
			}

			H3 = String.Format("We have {0} systems in the range", selectedStats.Count());

			// Sort DESC and take first highest values
			var takeBest = (from stat in selectedStats
							orderby stat.StatValueVal descending
							select stat).Take(howManySystemsInResult);

			// See tmp result:
			TABLE = takeBest; HR();

			Console.WriteLine("------ takeBest --------");
			foreach (var stat in takeBest)
			{
				Console.WriteLine($" StatName: {stat.StatName}, Value: {stat.StatValueVal}");
			}

			// Add names 
			var withNames = from statItem in takeBest
							join syst in C2SYSTEMS on statItem.SystemId equals syst.SystemId
							select new
							{
								Name = syst.SystemName,
								Statistics = statItem.StatValueVal,
								ID = statItem.SystemId,
								Added = syst.Added
							};
			H2 = "The best";
			TABLE = withNames; HR();

			// Get just systems ids
			var systemsIds = from item in withNames.ToList() select item.ID;

			systemsIds = systemsIds.ToList();


			// Equities in one chart:
			H1 = "A common chart";

			ITimeSeriesChart commonChart = new TimeSeriesChart();
			commonChart.Name = "Equities";
			commonChart.Add(GetEquities(systemsIds));
			TSCHART = commonChart; HR();

			// Stop ("return") here for now and look at results.
			// Select some good systems and look at them then.
			// return; 

			// =======================================================
			// Continuing after the first run.
			// =======================================================
			/*
			 It seems good systems are:
			 - "ASCENDANT TY" - 90325773 
			 - "Super Model II" - 91758928 
			 - "Midcap Daytrader ITF"  - 84939785
			 - "DJ Profit" - 92914206

			 So look at them:

			*/
			H1 = "Selected systems";



			Console.WriteLine("------ creating charts --------");
			foreach (var id in systemsIds.ToArray())
			{
				var sysChart = GetC2SYSTEM(id).SystemChart();
				sysChart.Height = 1000;
				sysChart.Width = 1000;
				CHART = sysChart;

				Console.WriteLine($" System: {id}, sysChart.Name: {sysChart.Name}");

				/*
				  HR();
				  FRAME = new Frame(){
					  Src = "https://collective2.com/system" + id.ToString(),
					  Height = 600,
					  Width = 800
				  };
				  HR();
				  */
			}

			Console.WriteLine("Example02 done. Press ENTER");
			Console.ReadLine();

		}
	}
}