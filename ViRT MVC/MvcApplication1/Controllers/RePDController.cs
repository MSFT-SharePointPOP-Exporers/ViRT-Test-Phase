using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcApplication1.Models;
using Newtonsoft.Json;
using System.Data;

namespace MvcApplication1.Controllers
{
    public class RePDController : Controller
    {

        public ActionResult Index()
        {
            MSRreliability reliability = new MSRreliability();
            DateTime newdate = new DateTime(2013, 7, 1);
            DataTable MSRreliabilityChart = reliability.ReliaiblityDailyTable(Convert.ToDateTime(Request.QueryString["start"]));//Convert.ToDateTime(Request.QueryString["start"]));
            var json = JsonConvert.SerializeObject(MSRreliabilityChart, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            ViewBag.MSRreliabilityChart = json;

            MSRperformance performance = new MSRperformance();
            DataTable MSRPerfPercentileChart = performance.Percentile95Table(Convert.ToDateTime(Request.QueryString["start"]));
            var perfJson = JsonConvert.SerializeObject(MSRPerfPercentileChart, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            ViewBag.MSRPerfPercentileChart = perfJson;

            return View();
        }

    }
}
