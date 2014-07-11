﻿<%@ Page Language="C#" MasterPageFile="~/Views/Shared/RePD.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    MSR Report
</asp:Content>

<asp:Content ID="Head" ContentPlaceHolderID="Head" runat="server">
    <style>
    #chartdiv {
            width: 100%;
            height: 700px;
        }
        </style>
    <script>
        $(document).ready(function () {
            $("#rendering h1").append($.QueryString("team"));

            $.ajax({
                url: '<%= Url.Action("getReliability", "RePD_Query") %>',
                success: function (data) {
                    $(".reliability").append(reliability + "%");
                    if (reliability > upper) {
                        $(".reliability").addClass("green");
                    } else if (reliability > lower && reliability < upper) {
                        $(".reliability").addClass("yellow");
                    } else if (reliability < lower) {
                        $(".reliability").addClass("red");
                    }
                }
            });

            $.ajax({
                url: '<%= Url.Action("getPerformance", "RePD_Query") %>',
                success: function (data) {
                    $(".performance").append(performance + "%");
                    if (performance > upper) {
                        $(".performance").addClass("green");
                    } else if (performance > lower && performance < upper) {
                        $(".performance").addClass("yellow");
                    } else if (performance < lower) {
                        $(".performance").addClass("red");
                    }
                }
            });

            $.ajax({
                url: '<%= Url.Action("getQoS", "RePD_Query") %>',
                success: function (data) {
                    $(".qos").append(QoS + "%");
                    if (QoS > upper) {
                        $(".qos").addClass("green");
                    } else if (QoS > lower && QoS < upper) {
                        $(".qos").addClass("yellow");
                    } else if (QoS < lower) {
                        $(".qos").addClass("red");
                    }
                }
            });

            //It just so happens that it works. But fix it.
            $.ajax({
                url: '<%= Url.Action("getLatency", "RePD_Query") %>',
                success: function (data) {
                    $(".latency").append(latency + "ms");
                    if (latency > upper) {
                        $(".latency").addClass("green");
                    } else if (latency > lower && latency < upper) {
                        $(".latency").addClass("yellow");
                    } else if (latency < lower) {
                        $(".latency").addClass("red");
                    }
                }
            });


        });
    </script>
    <script type="text/javascript" src="http://www.amcharts.com/lib/3/amcharts.js"></script>
    <script type="text/javascript" src="http://www.amcharts.com/lib/3/serial.js"></script>
    <script type="text/javascript" src="http://www.amcharts.com/lib/3/themes/dark.js"></script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div id="overallstats">
        <h2>Overall Statistics</h2>
    <table>
  <tr>
    <td rowspan="2">QoS</td>
    <td>Reliability</td>
    <td class="text reliability"></td><!--Reliability-->
    <td class="text qos" rowspan="2"></td><!--QoS-->
  </tr>
  <tr>
    <td class="secondary">Performance</td>
    <td class="text performance"></td><!--Performance-->
  </tr>
  <tr>
    <td class="text" colspan="3">
     95th percentile server latency*<br />
     *Goal is < 200 ms
    </td>
    <td class="text latency"></td>
  </tr>
</table>
        </div>
    <div id="chartdiv"></div>
    <script>
        var bullets = ["round", "square", "triangleUp", "triangleDown", "triangleLeft", "triangleRight", "diamond", "xError", "yError"];
        var data = <%= Html.Raw(ViewBag.MSRreliabilityChart)%>;//generateChartData();

        create(data);

        function create(chartData) {
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
                    "categoryBalloonDateFormat": "MMM DD, YYYY"
                },
                "dataDateFormat": "YYYY-MM-DD",
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
