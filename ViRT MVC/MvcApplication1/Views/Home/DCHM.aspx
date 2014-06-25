<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Head" ContentPlaceHolderID="Head" runat="server">
    <link href="../../Content/DCHM.css" rel="stylesheet" />
    <script>
        $(document).ready(function () {
            $(document).ajaxSend(function () {
                $("#loading").fadeIn();
            });

            $(document).ajaxComplete(function () {
                $("#loading").fadeOut("slow");
            });
            $("#rendering h1").append($.QueryString("datacen").substring(0,3));
            $.ajax({
                data: sessionStorage["query"],
                url: '<%= Url.Action("getNetworkFarm", "Query") %>',
                dataType: "json",
                success: function (data) {
                    $(".dchm").append('<ul class="small-block-grid-2 medium-block-grid-3 large-block-grid-4">');
                    for (var x = 0; x < data.length;x++) {
                        $(".dchm ul").append("<li id ='" + x +"'>");
                        $("#"+x).append("<h2>Network " + data[x].NetworkID + "<br/><a href='#' onClick='setNetwork(" + data[x].NetworkID + ")'>" + data[x].Percentage + "%</a></h2>");
                        $("#"+x).append("<div class ='network-box' id =" + data[x].NetworkID + ">");
                        $("select").append("<option value=" + data[x].NetworkID + ">" + data[x].NetworkID + "</option>");
                        for (var y = 0; y < data[x].Farms.length;y++) {
                            console.log(data[x].Farms[y].FarmID.toString() + " " + data[x].Farms[y].Percentage + "%");
                            if (data[x].Farms[y].Percentage <= 100.00 && data[x].Farms[y].Percentage >= 99.9) {
                                $("#" + data[x].NetworkID).append("<div class ='farm-box green' onClick='setFarm(this.id)' id=" + data[x].Farms[y].FarmID + ">Farm " + data[x].Farms[y].FarmID.toString() + "<br/>" + data[x].Farms[y].Percentage + "%</div>");
                            } else if (data[x].Farms[y].Percentage < 99.9 & data[x].Farms[y].Percentage >= 99.0) {
                                $("#" + data[x].NetworkID).append("<div class ='farm-box yellow' onClick='setFarm(this.id)' id=" + data[x].Farms[y].FarmID + ">Farm " + data[x].Farms[y].FarmID.toString() + "<br/>" + data[x].Farms[y].Percentage + "%</div>");
                            } else if (data[x].Farms[y].Percentage < 99.0 && data[x].Farms[y].Percentage >= 95.0) {
                                $("#" + data[x].networkID).append("<div class ='farm-box red1' onClick='setFarm(this.id)' id=" + data[x].Farms[y].FarmID + ">Farm " + data[x].Farms[y].FarmID.toString() + "<br/>" + data[x].Farms[y].Percentage + "%</div>");
                            } else if (data[x].Farms[y].Percentage < 95.0 && data[x].Farms[y].percentage >= 85.0) {
                                $("#" + data[x].NetworkID).append("<div class ='farm-box red2' onClick='setFarm(this.id)' id=" + data[x].Farms[y].FarmID + ">Farm " + data[x].Farms[y].FarmID.toString() + "<br/>" + data[x].Farms[y].Percentage + "%</div>");
                            } else {
                                $("#" + data[x].NetworkID).append("<div class ='farm-box red3' onClick='setFarm(this.id)' id=" + data[x].Farms[y].FarmID + ">Farm " + data[x].Farms[y].FarmID.toString() + "<br/>" + data[x].Farms[y].Percentage + "%</div>");
                            }
                        }
                    }
                }
            });
        });
    </script>
</asp:Content>

<asp:Content ID="contactTitle" ContentPlaceHolderID="TitleContent" runat="server">
    Data Center Heat Map
</asp:Content>

<asp:Content ID="Content" ContentPlaceHolderID="FeaturedContent" runat="server">
    <h1></h1> <!-- Data Center will go here -->
    <h2>Select a Network of Farm to filter reliability data.</h2>
    <div class="" id="jumptonetwork">
        <label for="networks">Jump To</label>
        <select id="networks" onchange="window.location.hash = this.value">
        </select>
    </div>
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">
   <div class = "dchm">
    </div>
</asp:Content>