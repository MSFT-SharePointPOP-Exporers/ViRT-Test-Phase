<%@ Page Language="C#" MasterPageFile="~/Views/Shared/ViRT.Master" Inherits="System.Web.Mvc.ViewPage" %>

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

            if (window.location.search != "") {
                $("#rendering h1").append($.QueryString("datacen").substring(0, 3));
                $.ajax({
                    data: sessionStorage["query"],
                    url: '<%= Url.Action("getNetworkFarm", "ViRT_Query") %>',
                    dataType: "json",
                    success: function (data) {
                        $(".dchm").append('<ul class="small-block-grid-2 medium-block-grid-3 large-block-grid-4">');
                        if (data != null) {
                            for (var x = 0; x < data.length; x++) {
                                $(".dchm ul").append("<li id ='" + x + "'>");
                                $("#" + x).append("<h2>Network " + data[x].NetworkID + "<br/><a href='#' onClick='setNetwork(" + data[x].NetworkID + ")'>" + data[x].Percent + "%</a></h2>");
                                $("#" + x).append("<div class ='network-box' id =" + data[x].NetworkID + ">");
                                $("select").append("<option value=" + data[x].NetworkID + ">" + data[x].NetworkID + "</option>");
                                for (var y = 0; y < data[x].Farms.length; y++) {
                                    if (data[x].Farms[y].Percent <= 100.00 && data[x].Farms[y].Percent >= 99.9) {
                                        $("#" + data[x].NetworkID).append("<div class ='farm-box green' onClick='setFarm(this.id)' id=" + data[x].Farms[y].Farms + ">Farm " + data[x].Farms[y].Farms.toString() + "<br/>" + data[x].Farms[y].Percent + "%</div>");
                                    } else if (data[x].Farms[y].Percent < 99.9 & data[x].Farms[y].Percent >= 99.0) {
                                        $("#" + data[x].NetworkID).append("<div class ='farm-box yellow' onClick='setFarm(this.id)' id=" + data[x].Farms[y].Farms + ">Farm " + data[x].Farms[y].Farms.toString() + "<br/>" + data[x].Farms[y].Percent + "%</div>");
                                    } else if (data[x].Farms[y].Percent < 99.0 && data[x].Farms[y].Percent >= 95.0) {
                                        $("#" + data[x].networkID).append("<div class ='farm-box red1' onClick='setFarm(this.id)' id=" + data[x].Farms[y].Farms + ">Farm " + data[x].Farms[y].Farms.toString() + "<br/>" + data[x].Farms[y].Percent + "%</div>");
                                    } else if (data[x].Farms[y].Percent < 95.0 && data[x].Farms[y].Percent >= 85.0) {
                                        $("#" + data[x].NetworkID).append("<div class ='farm-box red2' onClick='setFarm(this.id)' id=" + data[x].Farms[y].Farms + ">Farm " + data[x].Farms[y].Farms.toString() + "<br/>" + data[x].Farms[y].Percent + "%</div>");
                                    } else {
                                        $("#" + data[x].NetworkID).append("<div class ='farm-box red3' onClick='setFarm(this.id)' id=" + data[x].Farms[y].Farms + ">Farm " + data[x].Farms[y].Farms.toString() + "<br/>" + data[x].Farms[y].Percent + "%</div>");
                                    }
                                }
                            }
                        }
                    }
                });
            }
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
        <label for="networks">Jump To Network</label>
        <select id="networks" onchange="window.location.hash = this.value">
        </select>
    </div>
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">
   <div class = "dchm">
    </div>
</asp:Content>