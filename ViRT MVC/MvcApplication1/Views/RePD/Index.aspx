<%@ Page Language="C#" MasterPageFile="~/Views/Shared/RePD.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    MSR Report
</asp:Content>

<asp:Content ID="Head" ContentPlaceHolderID="Head" runat="server">
    <script>
        $(document).ready(function () {
            $("#rendering h1").append($.QueryString("team"));
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
    <td class="text yellow">99.54%</td><!--Reliability-->
    <td class="text yellow" rowspan="2">99.18%</td><!--QoS-->
  </tr>
  <tr>
    <td class="secondary">Performance</td>
    <td class="text yellow">99.68%</td><!--Performance-->
  </tr>
  <tr>
    <td class="text" colspan="3">
     95th percentile server latency*<br />
     *Goal is < 200 ms
    </td>
    <td class="text green">127 ms</td>
  </tr>
</table>
        </div>
</asp:Content>
