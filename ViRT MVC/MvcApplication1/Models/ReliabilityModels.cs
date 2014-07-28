﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace MvcApplication1.Models
{
	public class Reliability
	{
		private String dataCenter;
		private int networkID;
		private int farmID;
		private String pipeline;
		private DateTime start;
		private DateTime end;
		private SqlConnection dbConnect = new SqlConnection(ConfigurationManager.ConnectionStrings["ViRTConnection"].ConnectionString);
		Random random = new Random();


		/*Constructors*/

		/// <summary>
		/// Creates a new Reliability object with default values
		/// Every DC, Overview, and timespan of 7 days
		/// </summary>
		public Reliability()
		{
			dataCenter = "All";
			networkID = -1;
			farmID = -1;
			pipeline = "Overview";

			start = DateTime.Today.AddDays(-8);
			end = DateTime.Today.AddDays(-1);
			//Time span is 7 days.
		}

		/// <summary>
		/// Creates a Reliability object with all values initialized
		/// </summary>
		/// <param name="pDataCenter">Data Center</param>
		/// <param name="pNetworkID">Network ID</param>
		/// <param name="pFarmID">Farm ID</param>
		/// <param name="pPipeline">Selected Pipeline</param>
		/// <param name="pStart">Start Date</param>
		/// <param name="pEnd">End Date</param>
		public Reliability(String pDataCenter, int pNetworkID, int pFarmID, String pPipeline,
			DateTime pStart, DateTime pEnd)
		{
			dataCenter = pDataCenter;
			networkID = pNetworkID;
			farmID = pFarmID;
			pipeline = pPipeline;
			start = pStart;
			end = pEnd;
		}


		/*Public Methods*/


		/// <summary>
		/// Calculates the reliability percentage for a single pipeline in the overview bar
		/// Gets every component in the pipeline and calculates the percentage for every component
		/// </summary>
		/// <param name="pPipeline">Pipeline to calculate values</param>
		/// <returns>DataTable with Component Column and Percent Column</returns>
		public DataTable CalculateOverviewBar(String pPipeline)
		{
			//Retrieves all the components for the specified pipeline
			dbConnect.Open();
			String query = "SELECT Component FROM PipelineComponent WHERE Pipeline = '" + pPipeline + "'";
			SqlCommand queryCommand = new SqlCommand(query, dbConnect);
			SqlDataReader queryCommandReader = queryCommand.ExecuteReader();
			DataTable componentTable = new DataTable();
			componentTable.Load(queryCommandReader);

			//Create new DataTable with two columns named Pipeline and Percent
			DataTable retTable = new DataTable();
			retTable.Columns.Add("Component", typeof(String));
			retTable.Columns.Add("Percent", typeof(decimal));

			//Create new variables needed to fill retTable
			DataRow toAdd = retTable.NewRow();
			DataTable temp;
			decimal total = 0;

			//Iterate the pipeline table
			for (int i = 0; i < componentTable.Rows.Count; i++)
			{
				//Temp holds the values of a single component in the pipeline
				temp = CalculateComponent((String)componentTable.Rows[i]["Component"]);

				//Check for divide by 0
				if (temp.Rows.Count != 0)
				{
					//Iterate through temp and add the values
					for (int j = 0; j < temp.Rows.Count; j++)
					{
						total = total + (decimal)temp.Rows[j]["Percentage"];
					}

					total = total / temp.Rows.Count;
				}
				else
				{
					total = 0;
				}

				//Add the Component name and the totalAverage to the return table
				toAdd["Component"] = (string)componentTable.Rows[i]["Component"];
				toAdd["Percent"] = Math.Round(total, 4);
				retTable.Rows.Add(toAdd);

				toAdd = retTable.NewRow();
				total = 0;
			}

			//Close connection and return the table
			dbConnect.Close();
			return retTable;
		}

		/// <summary>
		/// Gets the raw numbers (success hits and failure hits) for a component
		/// </summary>
		/// <param name="pComponent">Component to retrieve the raw numbers</param>
		/// <returns>DataTable with Date Column, SuccessHits Column and FailureHits Column</returns>
		public DataTable RawDataGraphTable(String pComponent)
		{
			//Query for tags of component
			dbConnect.Open();
			String query = "SELECT SuccessTag, FailureTag FROM Component WHERE Component = '" + pComponent + "'";
			SqlCommand queryCommand = new SqlCommand(query, dbConnect);
			SqlDataReader queryCommandReader = queryCommand.ExecuteReader();

			//Get success and failure tags into a table
			DataTable twoTags = new DataTable();
			twoTags.Load(queryCommandReader);

			//Get the names of the two tags
			DataColumn col = twoTags.Columns[0];
			String successTag = (String)twoTags.Rows[0][col.ColumnName];
			col = twoTags.Columns[1];
			String failureTag = (String)twoTags.Rows[0][col.ColumnName];

			//Fill DataTables with the hits for that tag
			DataTable dt = ComponentRawHitsDataTable(successTag, failureTag);

			//Close and return the DataTable
			dbConnect.Close();
			return dt;
		}

		/// <summary>
		/// Calculates the reliability for a pipeline by component
		/// </summary>
		/// <param name="pPipeline">Specified pipeline</param>
		/// <returns>DataTable containing Date Column and column for each component</returns>
		public DataTable PipelineGraphTable(String pPipeline)
		{
			//connect to DB and query for 
			dbConnect.Open();

			//Get all components from pipeline
			String query = "SELECT Component FROM PipelineComponent WHERE Pipeline = '" + pPipeline + "'";
			SqlCommand queryCommand = new SqlCommand(query, dbConnect);
			SqlDataReader queryCommandReader = queryCommand.ExecuteReader();

			//componentsTable has all the components from the pipeline
			DataTable componentsTable = new DataTable();
			componentsTable.Load(queryCommandReader);

			//The number of components the pipeline has and stores the names into an array of strings
			int length = componentsTable.Rows.Count;
			string[] comps = new string[length];
			for (int i = 0; i < length; i++)
			{
				comps[i] = (string)componentsTable.Rows[i]["Component"];
			}

			//datePercent is an array which will hold all the tables with the calculated % for every component
			DataTable[] datePercents = new DataTable[length];

			//DataTable which hold the date and percent of all the components
			DataTable dt = new DataTable();
			dt.Columns.Add("Date", typeof(DateTime));

			//Goes through every component and pulls the percents
			//Also creates new columns for the date and percents table with all the components
			for (int i = 0; i < length; i++)
			{
				datePercents[i] = CalculateComponent(comps[i]);
				dt.Columns.Add(comps[i], typeof(decimal));
			}
			DataRow toAdd = dt.NewRow();

			//Iterate throught the entire time period
			for (DateTime i = start; i < end; i = i.AddHours(1))
			{
				toAdd["Date"] = i;
				//Iterate through all the datePercent components tables
				for (int j = 0; j < datePercents.Length; j++)
				{
					//Iterate through a datePercent table
					for (int k = 0; k < datePercents[j].Rows.Count; k++)
					{
						//Check if there are entries with the time i
						if ((DateTime)datePercents[j].Rows[k]["Date"] == i)
						{
							toAdd[comps[j]] = datePercents[j].Rows[k]["Percent"];
						}
					}
				}
				//Add the entries to the return table
				dt.Rows.Add(toAdd);
				toAdd = dt.NewRow();
			}

			//Close db and return table
			dbConnect.Close();
			return dt;
		}

		/// <summary>
		/// Calculates the percentage shown on the world map circles
		/// </summary>
		/// <returns>A single percentage for a pipeline</returns>
		public decimal CalculateWorldMapCircle()
		{
			DataTable pipePercentTable = CalculateOverviewBar(pipeline);
			decimal total = 0;

			for (int i = 0; i < pipePercentTable.Rows.Count; i++)
			{
				total = total + (decimal)pipePercentTable.Rows[i]["Percent"];
			}

			if (pipePercentTable.Rows.Count == 0) return 0;

			return Math.Round(total / pipePercentTable.Rows.Count, 4);
		}

		/// <summary>
		/// Creates a DataTable that has all the information for the DataCenter Heat Map
		/// </summary>
		/// <returns>DataTable containing a Date Column, Percent Column, and a DataTable with Farms and their Percents</returns>
		public DataTable CalculateDataCenterHeatMap()
		{
			dbConnect.Open();
			//Create a DataTable which has NetworkIDs, Percents (average of farms), and a DataTable of the Farms
			DataTable retTable = new DataTable();
			retTable.Columns.Add("NetworkID", typeof(int));
			retTable.Columns.Add("Percent", typeof(decimal));
			retTable.Columns.Add("Farms", typeof(DataTable));

			//DataRow to add to the return table
			DataRow toAdd = retTable.NewRow();

			//All the NetworkIds in the current DataCenter
			DataTable allNetsinDC = GetNetworks(dataCenter);


			if (pipeline != "Overview")
			{

				for (int i = 0; i < allNetsinDC.Rows.Count; i++)
				{
					//Console.WriteLine("Makes it here 1");
					ChangeNetworkID((int)allNetsinDC.Rows[i][0]);
					//Console.WriteLine("Makes it here 2");
					toAdd["NetworkID"] = networkID;
					//Console.WriteLine("Makes it here 3");
					toAdd["Percent"] = NetworkOnePercent();
					//Console.WriteLine("Makes it here 4");
					toAdd["Farms"] = CalculatePercentByFarms();
					//Console.WriteLine("Makes it here 5");

					retTable.Rows.Add(toAdd);
					toAdd = retTable.NewRow();
				}
			}
			else
			{
				ChangePipeline("UserLogin");
				for (int i = 0; i < allNetsinDC.Rows.Count; i++)
				{
					//Console.WriteLine("Makes it here 1");
					ChangeNetworkID((int)allNetsinDC.Rows[i][0]);
					//Console.WriteLine("Makes it here 2");
					toAdd["NetworkID"] = networkID;
					//Console.WriteLine("Makes it here 3");
					toAdd["Percent"] = NetworkOnePercent();
					//Console.WriteLine("Makes it here 4");
					toAdd["Farms"] = CalculatePercentByFarms();
					//Console.WriteLine("Makes it here 5");

					retTable.Rows.Add(toAdd);
					toAdd = retTable.NewRow();
				}
				ChangePipeline("Overview");
			}

			ChangeNetworkID(-1);
			dbConnect.Close();
			return retTable;

		}


		private String[] Tags(String component)
		{
			String query = "SELECT SuccessTag, FailureTag FROM Component WHERE Component = '" + component + "'";
			SqlCommand queryCommand = new SqlCommand(query, dbConnect);
			SqlDataReader queryCommandReader = queryCommand.ExecuteReader();
			DataTable twoTags = new DataTable();
			twoTags.Load(queryCommandReader);

			String[] tags = { (String)twoTags.Rows[0][0], (String)twoTags.Rows[0][1] };

			return tags;
		}



		private DataTable CalculatePercentByFarms()
		{
			String[] tags = Tags(pipeline);
			String successTag = tags[0];
			String failureTag = tags[1];

			String query = "SELECT FarmId, CAST(SUM(CASE when Tag = '" + successTag + "' then Hits else 0 END) AS DECIMAL)" +
				"/(SUM(CASE when Tag = '" + successTag + "' then Hits else 0 END)+SUM(CASE when Tag = '" + failureTag + "' then Hits else 0 END ) + .000000001) * 100" +
				" AS Percentage FROM Prod_Reliability";
			String where = " WHERE Date >= '" + start.ToString() + "' AND Date < '" + end.ToString() + "' AND NetworkId = " + networkID;
			String groupBy = " GROUP BY Date, FarmId";

			query = query + where + groupBy;

			SqlCommand queryCommand = new SqlCommand(query, dbConnect);
			SqlDataReader queryCommandReader = queryCommand.ExecuteReader();
			DataTable farmPercent = new DataTable();
			farmPercent.Load(queryCommandReader);

			return farmPercent;
		}



		private decimal NetworkOnePercent()
		{
			String[] tags = Tags(pipeline);
			String successTag = tags[0];
			String failureTag = tags[1];

			String query = "SELECT CAST(SUM(CASE when Tag = '" + successTag + "' then Hits else 0 END) AS DECIMAL)" +
				"/(SUM(CASE when Tag = '" + successTag + "' then Hits else 0 END)+SUM(CASE when Tag = '" + failureTag + "' then Hits else 0 END ) + .000000001) * 100" +
				" AS Percentage FROM Prod_Reliability";
			String where = " WHERE Date >= '" + start.ToString() + "' AND Date < '" + end.ToString() + "' AND NetworkId = " + networkID;
			String groupBy = " GROUP BY NetworkId";

			query = query + where + groupBy;

			SqlCommand queryCommand = new SqlCommand(query, dbConnect);
			SqlDataReader queryCommandReader = queryCommand.ExecuteReader();
			DataTable onePer = new DataTable();
			onePer.Load(queryCommandReader);

			return (decimal)onePer.Rows[0][0];
		}


		/*Everything after this is private methods or helper methods*/

		/// <summary>
		/// Calculates the reliability of a single component
		/// Retrieves 
		/// TODO: query and calculate everything in this method
		/// </summary>
		/// <param name="pComponent"></param>
		/// <returns></returns>
		private DataTable CalculateComponent(String pComponent)
		{
			String[] tags = Tags(pComponent);
			String successTag = tags[0];
			String failureTag = tags[1];

			return ComponentPercentageCalculate(successTag, failureTag);
		}

		/// <summary>
		/// Calculates the reliability of a component through its tags
		/// </summary>
		/// <param name="sTag">Success Tag of component</param>
		/// <param name="fTag">Failure Tag of component</param>
		/// <returns>DataTable with Date Column and Percentage Column</returns>
		private DataTable ComponentPercentageCalculate(String sTag, String fTag)
		{
			//Strings that create the query
			String query = "SELECT DATEADD(HOUR, Hour, Date) AS Date, CAST(SUM(CASE when Tag = '" + sTag + "' then Hits else 0 END) AS DECIMAL)" +
				"/(SUM(CASE when Tag = '" + sTag + "' then Hits else 0 END)+SUM(CASE when Tag = '" + fTag + "' then Hits else 0 END ) + .000000001) * 100" +
				" AS Percentage FROM Prod_Reliability";
			String where = " WHERE Date >= '" + start.ToString() + "' AND Date < '" + end.ToString() + "'";
			String groupBy = " GROUP BY Date, Hour ORDER BY Date";

			//Creates the remainer of the where portion of the query
			if (!dataCenter.Equals("All"))
			{
				if (networkID == -1) where = where + " AND DataCenter = '" + dataCenter + "'";
				else
				{
					if (farmID == -1) where = where + " AND NetworkId = '" + networkID + "'";
					else where = where + " AND FarmId = '" + farmID + "'";
				}
			}

			//Query DB
			query = query + where + groupBy;
			SqlCommand queryCommand = new SqlCommand(query, dbConnect);
			SqlDataReader queryCommandReader = queryCommand.ExecuteReader();
			DataTable tagTable = new DataTable();
			tagTable.Load(queryCommandReader);

			//Return the table
			return tagTable;
		}

		private DataTable ComponentRawHitsDataTable(String sTag, String fTag)
		{
			//Strings that create the query
			String query = "SELECT DATEADD(HOUR, Hour, Date) AS Date, SUM(CASE when Tag = '" + sTag + "' then Hits else 0 END) AS " + sTag +
				", SUM(CASE when Tag = '" + fTag + "' then Hits else 0 END ) AS " + fTag + " FROM Prod_Reliability";
			String where = " WHERE Date >= '" + start.ToString() + "' AND Date < '" + end.ToString() + "'";
			String groupBy = " GROUP BY Date, Hour ORDER BY Date";

			//Creates the remainer of the where portion of the query
			if (!dataCenter.Equals("All"))
			{
				if (networkID == -1) where = where + " AND DataCenter = '" + dataCenter + "'";
				else
				{
					if (farmID == -1) where = where + " AND NetworkId = '" + networkID + "'";
					else where = where + " AND FarmId = '" + farmID + "'";
				}
			}

			//Query DB
			query = query + where + groupBy;
			SqlCommand queryCommand = new SqlCommand(query, dbConnect);
			SqlDataReader queryCommandReader = queryCommand.ExecuteReader();
			DataTable tagTable = new DataTable();
			tagTable.Load(queryCommandReader);

			//Return the table
			return tagTable;
		}









		/*
		 * Retrieves the names of the components for a pipeline
		 * 
		 * @param pPipeline		The pipeline whose components will be returned
		 * @return		Array of componets for the pipeline
		 */
		public String[] GetComponents(String pPipeline)
		{
			dbConnect.Open();
			String query = "SELECT Component FROM PipelineComponent WHERE Pipeline = '" + pPipeline + "'";
			SqlCommand queryCommand = new SqlCommand(query, dbConnect);
			SqlDataReader queryCommandReader = queryCommand.ExecuteReader();
			DataTable componentsForPipeline = new DataTable();
			componentsForPipeline.Load(queryCommandReader);


			String[] compsArray = new String[componentsForPipeline.Rows.Count];

			for (int i = 0; i < componentsForPipeline.Rows.Count; i++)
			{
				compsArray[i] = (String)componentsForPipeline.Rows[i]["Component"];
			}

			dbConnect.Close();
			return compsArray;
		}

		/*
		 * Retrieves all the pipeline names 
		 * 
		 * @return a String array of all the pipelines
		 */
		public DataTable GetAllPipelines()
		{
			dbConnect.Open();
			String query = "SELECT * FROM Pipeline";
			SqlCommand queryCommand = new SqlCommand(query, dbConnect);
			SqlDataReader queryCommandReader = queryCommand.ExecuteReader();
			DataTable pipelines = new DataTable();
			pipelines.Load(queryCommandReader);

			dbConnect.Close();
			return pipelines;
		}

		/*
		 * 
		 * 
		 */
		public String[] GetAllDataCentersArray()
		{
			dbConnect.Open();
			String query = "SELECT DataCenter FROM DataCenter";
			SqlCommand queryCommand = new SqlCommand(query, dbConnect);
			SqlDataReader queryCommandReader = queryCommand.ExecuteReader();
			DataTable dataCenters = new DataTable();
			dataCenters.Load(queryCommandReader);


			String[] dcArray = new String[dataCenters.Rows.Count];

			for (int i = 0; i < dataCenters.Rows.Count; i++)
			{
				dcArray[i] = (String)dataCenters.Rows[i]["DataCenter"];
			}

			dbConnect.Close();
			return dcArray;
		}

		/*
		 * 
		 * 
		 * 
		 */
		public int[] GetAllFarmsInNetworkArray()
		{
			dbConnect.Open();
			String query = "SELECT FarmId FROM NetworkIdFarmId WHERE NetworkId = " + networkID;
			SqlCommand queryCommand = new SqlCommand(query, dbConnect);
			SqlDataReader queryCommandReader = queryCommand.ExecuteReader();
			DataTable farmIDTable = new DataTable();
			farmIDTable.Load(queryCommandReader);

			int[] farmArray = new int[farmIDTable.Rows.Count];

			for (int i = 0; i < farmIDTable.Rows.Count; i++)
			{
				farmArray[i] = (int)farmIDTable.Rows[i]["FarmId"];
			}

			dbConnect.Close();

			return farmArray;
		}

		/*
		 * Retrieves the datacenter with the lat and long
		 * 
		 * @return		DataTable with latitude and longitude
		 */
		public DataTable GetDataCenterLatLong()
		{
			dbConnect.Open();
			String query = "SELECT DataCenter, latitude, longitude FROM DataCenter";
			SqlCommand queryCommand = new SqlCommand(query, dbConnect);
			SqlDataReader queryCommandReader = queryCommand.ExecuteReader();
			DataTable dclatlong = new DataTable();
			dclatlong.Load(queryCommandReader);
			dbConnect.Close();
			return dclatlong;
		}


		/*
		 * Retrieves all the NetworkID's for a specific dataCenter
		 * 
		 * @return a DataTable of the NetworkID's
		 */
		public DataTable GetNetworks(String dataCenter)
		{
			String query = "SELECT DISTINCT NetworkID FROM DataCenterNetworkId WHERE DataCenter = '" + dataCenter + "'";
			SqlCommand queryCommand = new SqlCommand(query, dbConnect);
			SqlDataReader queryCommandReader = queryCommand.ExecuteReader();
			DataTable networks = new DataTable();
			networks.Load(queryCommandReader);
			return networks;
		}

		/*
		* Retrieves all the FarmID for the NetworkID
		* 
		* @return a DataTable of the FarmID's
		*/
		public DataTable getFarms(int NetworkID)
		{
			//dbConnect.Open();
			//String query = "SELECT NetworkID FROM DataCenterNetworkId WHERE DataCenter = '" + dataCenter + "'";
			//SqlCommand queryCommand = new SqlCommand(query, dbConnect);
			//SqlDataReader queryCommandReader = queryCommand.ExecuteReader();
			//DataTable networks = new DataTable();
			//networks.Load(queryCommandReader);
			//dbConnect.Close();
			DataTable farms = new DataTable();
			DataColumn id = new DataColumn("FarmID", typeof(int));
			DataColumn percent = new DataColumn("Percentage", typeof(double));
			farms.Columns.Add(id);
			farms.Columns.Add(percent);
			for (int i = 0; i < random.Next(1, 30); i++)
			{
				DataRow newRow = farms.NewRow();
				newRow["FarmID"] = random.Next(1000, 10000);
				newRow["Percentage"] = Convert.ToDouble(String.Format("{0:0.0000}", random.NextDouble() * 100));
				farms.Rows.Add(newRow);
			}
			return farms;
		}



		/*
		* Retrieves all of the available networks
		* 
		* @return		A DataTable of All Networks
		*/
		public DataTable GetAllNetworks()
		{
			dbConnect.Open();
			String query = "SELECT DISTINCT NetworkID FROM DataCenterNetworkId";
			SqlCommand queryCommand = new SqlCommand(query, dbConnect);
			SqlDataReader queryCommandReader = queryCommand.ExecuteReader();
			DataTable allNetworks = new DataTable();
			allNetworks.Load(queryCommandReader);
			dbConnect.Close();
			return allNetworks;
		}

		/*
		* Retrieves all of the available farms
		* 
		* @return	A DataTable of All Farms
		*/
		public DataTable GetAllFarms()
		{
			dbConnect.Open();
			String query = "SELECT DISTINCT FarmID FROM NetworkIdFarmId";
			SqlCommand queryCommand = new SqlCommand(query, dbConnect);
			SqlDataReader queryCommandReader = queryCommand.ExecuteReader();
			DataTable allFarms = new DataTable();
			allFarms.Load(queryCommandReader);
			dbConnect.Close();
			return allFarms;
		}

		/*
		 * Changes the start and end date
		 * 
		 * @param pStart		New start date
		 * @param pEnd			New end date
		 */
		public void ChangeDate(DateTime pStart, DateTime pEnd)
		{
			start = pStart;
			end = pEnd.AddDays(1);
		}

		/*
		 * Changes the Data Center and changes network and farm to default
		 * 
		 * @param pDataCenter		Desired new data center
		 */
		public void ChangeDataCenter(String pDataCenter)
		{
			dataCenter = pDataCenter;
			networkID = -1;
			farmID = -1;
		}

		/*
		 * Changes the Network and changes the farm to the default
		 * 
		 * @param pNetworkID		Desired new networkID
		 */
		public void ChangeNetworkID(int pNetworkID)
		{
			networkID = pNetworkID;
			farmID = -1;
		}

		/*
		 * Change the farmID
		 * 
		 * @param pFarmID		Desired new farmID
		 */
		public void ChangeFarmID(int pFarmID)
		{
			farmID = pFarmID;
		}

		/*
		 * Change both the Data Center and NetworkId and make farm default
		 * 
		 * @param pDataCenter		New DataCenter
		 * @param pNetworkID		New NetworkID
		 */
		public void ChangeDataCenterNetworkID(String pDataCenter, int pNetworkID)
		{
			dataCenter = pDataCenter;
			networkID = pNetworkID;
			farmID = -1;
		}

		/*
		 * Changes the NetworkID and the FarmID
		 * 
		 * @param pNetworkID		New NetworkID
		 * @param pFarmID			New FarmID
		 */
		public void ChangeNetworkIDFarmID(int pNetworkID, int pFarmID)
		{
			networkID = pNetworkID;
			farmID = pFarmID;
		}

		/*
		 * Change all location filters
		 * 
		 * @param pDataCenter		New DataCenter
		 * @param pNetworkID		New NetworkID
		 * @param pFarmID			New FarmID
		 */
		public void ChangeLocationFilter(String pDataCenter, int pNetworkID, int pFarmID)
		{
			dataCenter = pDataCenter;
			networkID = pNetworkID;
			farmID = pFarmID;
		}

		/*
		 * Changes all filters
		 * 
		 * @param pDataCenter		New DataCenter
		 * @param pNetworkID		New NetworkID
		 * @param pFarmID			New FarmID
		 * @param pPipeline			New Pipeline
		 * @param pStart			New start time
		 * @param pEnd				New End time
		 */
		public void ChangeAllFilters(String pDataCenter, int pNetworkID, int pFarmID, String pPipeline,
			DateTime pStart, DateTime pEnd)
		{
			dataCenter = pDataCenter;
			networkID = pNetworkID;
			farmID = pFarmID;
			pipeline = pPipeline;
			start = pStart;
			end = pEnd.AddDays(1);
		}

		/*
		 * Changes the pipeline
		 * 
		 * @param pPipeline		New Pipeline
		 */
		public void ChangePipeline(String pPipeline)
		{
			pipeline = pPipeline;
		}

	}
}