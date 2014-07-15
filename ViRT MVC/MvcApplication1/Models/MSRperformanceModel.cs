using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace MvcApplication1.Models
{
	class MSRperformance
	{
		private SqlConnection dbConnect = new SqlConnection(ConfigurationManager.ConnectionStrings["ViRT"].ConnectionString);

		public MSRperformance()
		{

		}

		/*Perfomance does not work yet! Need data stuff!
		public decimal MonthPerformancePercent(DateTime monthYear)
		{
			dbConnect.Open();
			DateTime start = monthYear;
			DateTime end = (monthYear.AddMonths(1)).AddDays(-1);

			DataTable countTotal = GetMonitoredScopes(start, end);

			dbConnect.Close();
			return 0;
		}

		private DataTable GetMonitoredScopes(DateTime start, DateTime end)
		{
			String queryCountTotal = "SELECT LogDate AS Date, SampleCount, SumExecutionTimeMilliseconds FROM Probes WHERE LogDate BETWEEN" +
				start.ToString() + "' AND '" + end.ToString() + "'";
			SqlCommand queryCommand = new SqlCommand(queryCountTotal, dbConnect);
			SqlDataReader queryCommandReader = queryCommand.ExecuteReader();
			DataTable countTotalTable = new DataTable();
			countTotalTable.Load(queryCommandReader);

			return countTotalTable;
		}
		
		private decimal CalculateMonitored()
		{
			return 0;
		}
		*/

		/// <summary>
		/// Calculates the 95th percentile for SPClaimsCounterScope for a complete month
		/// </summary>
		/// <param name="monthYear">Start date of the month to calculate 95th percentile</param>
		/// <returns>Integer with 95th percentile in milliseconds</returns>
		public int Percentile95th(DateTime monthYear)
		{
			dbConnect.Open();
			DateTime start = monthYear;
			DateTime end = (monthYear.AddMonths(1)).AddDays(-1);

			DataTable tileDaily = PercentileDailyTable(start, end);
			int time = Calculate95th(tileDaily);
			Console.WriteLine(time);

			dbConnect.Close();
			return time;
		}

		/// <summary>
		/// Pulls data from DB and formats it into a date view
		/// </summary>
		/// <param name="start">Start date to pull</param>
		/// <param name="end">End date to include</param>
		/// <returns>DateTable with Date, Sample Count for day, and the sum of the 95th percentiles</returns>
		private DataTable PercentileDailyTable(DateTime start, DateTime end)
		{
			String query = "SELECT CAST(LogDate AS DATE) AS Date, SUM(SampleCount) AS SampleCount, SUM(SampleCount * ExecutionTimeMilliseconds_95) AS Sum95 FROM Prod_MonitoredScopes WHERE LogDate BETWEEN '" +
				start.ToString() + "' AND '" + end.ToString() + "' AND MonitoredScopeName = 'SPClaimsCounterScope' " + "GROUP BY CAST(LogDate AS DATE) ORDER BY Date";
			//Console.WriteLine(query);
			SqlCommand queryCommand = new SqlCommand(query, dbConnect);
			//queryCommand.CommandTimeout = 1000;
			SqlDataReader queryCommandReader = queryCommand.ExecuteReader();
			DataTable dateExecutionCount = new DataTable();
			dateExecutionCount.Load(queryCommandReader);

			return dateExecutionCount;
		}

		/// <summary>
		/// Gets the time in millisecond
		/// </summary>
		/// <param name="tileDaily">Table with daily data</param>
		/// <returns>95th percentile in milliseconds</returns>
		private int Calculate95th(DataTable tileDaily)
		{
			long total = 0;

			for (int i = 0; i < tileDaily.Rows.Count; i++)
			{
				total += (long)(tileDaily.Rows[i]["SampleCount"]);
			}

			int sum = 0;

			for (int i = 0; i < tileDaily.Rows.Count; i++)
			{
				sum += (int)((long)tileDaily.Rows[i]["Sum95"] / total);
			}

			return sum;
		}

		/// <summary>
		/// Creates a datatable which has the percentile for each day in the specified month
		/// </summary>
		/// <param name="monthYear">Start of the month to pull data from</param>
		/// <returns>Data table with Date and Percentile</returns>
		public DataTable Percentile95Table(DateTime monthYear)
		{
			dbConnect.Open();
			DateTime start = monthYear;
			DateTime end = (monthYear.AddMonths(1)).AddDays(-1);

			DataTable tileDaily = PercentileDailyTable(start, end);
			DataTable dailyPercentile = Format(tileDaily);

			dbConnect.Close();
			return dailyPercentile;
		}

		/// <summary>
		/// Formats the output for Percentile95
		/// </summary>
		/// <param name="tileDaily">Table to format</param>
		/// <returns>Formatted table with Date and Percentile</returns>
		private DataTable Format(DataTable tileDaily)
		{
			DataTable retTable = new DataTable();
			retTable.Columns.Add("Date", typeof(DateTime));
			retTable.Columns.Add("Percentile", typeof(int));
			DataRow toAdd = retTable.NewRow();

			for (int i = 0; i < tileDaily.Rows.Count; i++)
			{
				toAdd["Date"] = tileDaily.Rows[i]["Date"];
				toAdd["Percentile"] = (int)((long)tileDaily.Rows[i]["Sum95"] / (long)tileDaily.Rows[i]["SampleCount"]);
				retTable.Rows.Add(toAdd);
				toAdd = retTable.NewRow();
			}

			return retTable;
		}

	}
}
