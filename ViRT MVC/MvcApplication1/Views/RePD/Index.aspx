<%@ Page Language="C#" MasterPageFile="~/Views/Shared/RePD.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    MSR Report
</asp:Content>

<asp:Content ID="Head" ContentPlaceHolderID="Head" runat="server">
    <script>
        $(document).ready(function () {
            $("#rendering h1").append($.QueryString("team"));

            $.ajax({
                url: '<%= Url.Action("getReliability", "RePD_Query") %>',
                success: function (data) {
                    $(".reliability").append(reliability.toFixed(2) + "%");
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
                    $(".performance").append(performance.toFixed(2) + "%");
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
                    $(".qos").append(QoS.toFixed(2) + "%");
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
</asp:Content>
