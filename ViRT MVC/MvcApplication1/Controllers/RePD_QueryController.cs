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
        public string getRange()
        {
            return "testetsess";
        }

        public string getTeams()
        {
            return "teststetsa";
        }

        public string getReliability()
        {
            return "yetestst";
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
