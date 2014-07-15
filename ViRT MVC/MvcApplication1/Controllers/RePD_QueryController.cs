using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using Newtonsoft.Json;
using MvcApplication1.Models;

namespace MvcApplication1.Controllers
{
    public class RePD_QueryController : Controller
    {
        MSRreliability rel = new MSRreliability();

        public decimal getRange()
        {
            return 487.1846846846M;
        }

        public string getTeams()
        {
            return "teststetsa";
        }

        public decimal getReliability()
        {
            return rel.MonthReliabilityPercent(Convert.ToDateTime(Request.QueryString["start"]));
        }

        public string getPerformance()
        {
            return "testetss";
        }

        public string getQOS()
        {
            return "testetstt";
        }

        public string getLatency()
        {
            return "testetstt";
        }
    }
}
