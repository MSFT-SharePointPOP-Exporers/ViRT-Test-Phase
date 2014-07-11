﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace MvcApplication1.Models
{
	public class MSRreliability
    {
		private SqlConnection dbConnect = new SqlConnection(ConfigurationManager.ConnectionStrings["ViRT"].ConnectionString);

/// <summary>
		/// Constructor for a MSR_Reliability Object
		/// </summary>
		public MSRreliability()
		{

		}

		/// <summary>
		/// Calculates the reliability percentage for a given month
		/// </summary>
		/// <param name="monthYear">The month to calculate the percent reliability</param>
		/// <returns>Percent reliability</returns>
		public decimal MonthReliabilityPercent(DateTime monthYear)
		{
			dbConnect.Open();
			DataTable tagTable = tags();
			String successTag = (String)tagTable.Rows[0]["SuccessTag"];
			String failureTag = (String)tagTable.Rows[0]["FailureTag"];
			String userErrTag = (String)tagTable.Rows[0]["UserErrorTag"];
			DateTime start = monthYear;
			DateTime end = (monthYear.AddMonths(1)).AddDays(-1);

			int successCount = CalculateTagTotal(successTag, start, end);
			int failureCount = CalculateTagTotal(failureTag, start, end);
			int userErrCount = CalculateTagTotal(userErrTag, start, end);

			if (successCount + failureCount == 0) return 0;

			decimal total = (decimal)(successCount + userErrCount) * 100 / (successCount + failureCount);

			dbConnect.Close();
			return Math.Round(total, 4);
		}

		/// <summary>
		/// Helper method which retrieves the tags to calculate reliability
		/// </summary>
		/// <returns>DataTable with SuccessTag, FailureTag, and UserErrorTag for Component STS</returns>
		private DataTable tags()
		{
			String queryTags = "SELECT SuccessTag, FailureTag, UserErrorTag FROM Component WHERE Component = 'STS'";
			SqlCommand queryCommand = new SqlCommand(queryTags, dbConnect);
			SqlDataReader queryCommandReader = queryCommand.ExecuteReader();
			DataTable tagTable = new DataTable();
			tagTable.Load(queryCommandReader);

			return tagTable;
		}

		/// <summary>
		/// Calculates the number of hits for a specific tag for a month time period
		/// </summary>
		/// <param name="tag">Tag to calculate the sum</param>
		/// <param name="start">Start of the month</param>
		/// <param name="end">End of the month</param>
		/// <returns>Total number of hits</returns>
		private int CalculateTagTotal(String tag, DateTime start, DateTime end)
		{
			String sumQuery = "SELECT SUM(NumberOfHits) AS TokenIssuances FROM ProdDollar_TagAggregationCopy WHERE Tag = '" + 
				tag + "' AND Date BETWEEN '" + start.ToString() + "' AND '" + end.ToString() + "'";

			SqlCommand queryCommand = new SqlCommand(sumQuery, dbConnect);
			SqlDataReader queryCommandReader = queryCommand.ExecuteReader();
			DataTable sumTag = new DataTable();
			sumTag.Load(queryCommandReader);

			if (sumTag.Rows[0].IsNull(0)) return 0;
			return (int)sumTag.Rows[0]["TokenIssuances"];
		}

		/// <summary>
		/// Creates a data table with daily data for a specified month
		/// If no data is available for a time period, there are no entires
		/// in the table
		/// </summary>
		/// <param name="monthYear">Month and Year which the data will represent</param>
		/// <returns>DataTable with columns Date and Percent for the month</returns>
		public DataTable ReliaiblityDailyTable(DateTime monthYear)
		{
			dbConnect.Open();
			DataTable tagTable = tags();
			String successTag = (String)tagTable.Rows[0]["SuccessTag"];
			String failureTag = (String)tagTable.Rows[0]["FailureTag"];
			String userErrTag = (String)tagTable.Rows[0]["UserErrorTag"];
			DateTime start = monthYear;
			DateTime end = (monthYear.AddMonths(1)).AddDays(-1);

			//Make this into a method
			DataTable successTable = CalculateDailyTagForMonth(successTag, start, end);
			DataTable failureTable = CalculateDailyTagForMonth(failureTag, start, end);
			DataTable userErrTable = CalculateDailyTagForMonth(userErrTag, start, end);

			DataTable datePercent = CalculateDatePercent(successTable, failureTable, userErrTable, start, end
);
			//End of method

			dbConnect.Close();
			return datePercent;
		}

		/// <summary>
		/// Condenses hourly tag data into daily tag data
		/// </summary>
		/// <param name="tag">Tag to retrieve data</param>
		/// <param name="start">Start of month</param>
		/// <param name="end">End of month</param>
		/// <returns>Table with condensed Tag number of hits for a day</returns>
		private DataTable CalculateDailyTagForMonth(String tag, DateTime start, DateTime end)
		{
			String hitDailyMonthCount = "SELECT Date, SUM(NumberOfHits) AS NumberOfHits FROM ProdDollar_TagAggregationCopy WHERE Tag = '" +
				tag + "' AND Date BETWEEN '" + start.ToString() + "' AND '" + end.ToString() + "' GROUP BY Date";

			SqlCommand queryCommand = new SqlCommand(hitDailyMonthCount, dbConnect);
			SqlDataReader queryCommandReader = queryCommand.ExecuteReader();
			DataTable tagCountsByDates = new DataTable();
			tagCountsByDates.Load(queryCommandReader);

			return tagCountsByDates;
		}

		/// <summary>
		/// Calculates the reliability percentage for the entire month
		/// </summary>
		/// <param name="successTable">Success Hits</param>
		/// <param name="failureTable">Failure Hits</param>
		/// <param name="userErrTable">User Errors Hits</param>
		/// <param name="start">Start of month</param>
		/// <param name="end">End of month</param>
		/// <returns>DataTable with Date and Percent</returns>
		private DataTable CalculateDatePercent(DataTable successTable, DataTable failureTable, DataTable userErrTable, DateTime start, DateTime end)
		{
			DataTable datePercent = new DataTable();
			datePercent.Columns.Add("Date", typeof(DateTime));
			datePercent.Columns.Add("Percent", typeof(decimal));
			DataRow toAdd = datePercent.NewRow();

			int succ = 0;
			int fail = 0;
			int erro = 0;

			for (DateTime i = start; i <= end; i = i.AddDays(1))
			{
				

				for (int j = 0; j < successTable.Rows.Count; j++)
				{
					if ((DateTime)successTable.Rows[j]["Date"] == i)
					{
						succ = (int)successTable.Rows[j]["NumberOfHits"];
						//Console.WriteLine("succ " + succ);
						j = successTable.Rows.Count;
					}
				}
				for (int j = 0; j < failureTable.Rows.Count; j++)
				{
					if ((DateTime)failureTable.Rows[j]["Date"] == i)
					{
						fail = (int)failureTable.Rows[j]["NumberOfHits"];
						//Console.WriteLine("fail " + fail);
						j = successTable.Rows.Count;
					}
				}
				for (int j = 0; j < userErrTable.Rows.Count; j++)
				{
					if ((DateTime)userErrTable.Rows[j]["Date"] == i)
					{
						erro = (int)successTable.Rows[j]["NumberOfHits"];
						//Console.WriteLine("erro " + erro);
						j = successTable.Rows.Count;
					}
				}

				if (succ + fail != 0)
				{
					toAdd["Percent"] = Math.Round((decimal)(succ + erro) * 100 / (succ + fail), 4);
					toAdd["Date"] = i;
					datePercent.Rows.Add(toAdd);
				}
				
				toAdd = datePercent.NewRow();
				succ = 0;
				fail = 0;
				erro = 0;
			}

			return datePercent;
		}
	}
}