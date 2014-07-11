<%@ Page Language="C#" MasterPageFile="~/Views/Shared/RePD.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    View1
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<h2>View1</h2>

<div id="chartdiv"></div>
    <script>
        var data = <%= Html.Raw(ViewBag.PercentData)%>;//generateChartData();

		createReliability(data);

		function createReliability(chartData) {
		    var chart = AmCharts.makeChart("chartdiv", {
		        "type": "serial",
		        "theme": "dark",
		        "pathToImages": "http://www.amcharts.com/lib/3/images/",
		        "dataProvider": chartData,
		        "valueAxes": [{
		            "dashLength": 1,
		            "position": "left"
		        }],
		        "chartScrollbar": {
		            "autoGridCount": true,
		            "scrollbarHeight": 40
		        },
		        "chartCursor": {
		            "cursorPosition": "mouse",
		            "categoryBalloonDateFormat": "MMM DD, YYYY \nJJ:NN"
		        },
		        "dataDateFormat": "YYYY-MM-DDTJJ:NN:SS",
		        "categoryField": "Date",
		        "categoryAxis": {
		            "parseDates": true
		        }
		    });

		    var i = 0;
		    for (var propertyName in chartData[0]) {
		        if (propertyName != 'Date') {
		            if (i == 9)
		                i = 0;
		            var graph1 = new AmCharts.AmGraph();
		            graph1.type = "line";
		            graph1.valueField = propertyName;
		            graph1.balloonText = "<b><span style='font-size:14px;'>[[title]]</span></b><br />[[category]]<br /><span style='font-size:14px;'>Reliability: [[value]]</span>";
		            graph1.title = propertyName;
		            graph1.bullet = bullets[i];
		            graph1.bulletSize = 10;
		            graph1.hideBulletsCount = 30;
		            graph1.connect = false;
		            chart.addGraph(graph1);
		            i++;
		        }
		    }


		    return chart;
		}

    </script>
</asp:Content>
