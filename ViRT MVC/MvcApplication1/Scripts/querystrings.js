function setDefaults() {
	sessionStorage["start"] = new Date.parse("t - 8 d").toString("yyyy-MM-dd");
	sessionStorage["end"] = new Date.parse("t - 1 d").toString("yyyy-MM-dd");
	sessionStorage["pipeline"] = "Overview";
	sessionStorage["datacen"] = "All";
	sessionStorage["network"] = -1;
	sessionStorage["farm"] = -1;
	sessionStorage["query"] = "?start=" + sessionStorage["start"] + "&end=" + sessionStorage["end"] + "&pipeline=" + sessionStorage["pipeline"] + "&datacen=" + sessionStorage["datacen"] + "&network=" + sessionStorage["network"] + "&farm=" + sessionStorage["farm"];
}

function setHomeDeafults() {
	sessionStorage["datacen"] = "All";
	sessionStorage["network"] = -1;
	sessionStorage["farm"] = -1;
	sessionStorage["query"] = "?start=" + sessionStorage["start"] + "&end=" + sessionStorage["end"] + "&pipeline=" + sessionStorage["pipeline"] + "&datacen=" + sessionStorage["datacen"] + "&network=" + sessionStorage["network"] + "&farm=" + sessionStorage["farm"];
}

function updateQueryString() {
	if (sessionStorage["start"] == undefined) {
		setDefaults();
	}
	sessionStorage["query"] = "?start=" + sessionStorage["start"] + "&end=" + sessionStorage["end"] + "&pipeline=" + sessionStorage["pipeline"] + "&datacen=" + sessionStorage["datacen"] + "&network=" + sessionStorage["network"] + "&farm=" + sessionStorage["farm"];
	window.location.search = sessionStorage["query"];
}

function setSessionStorage() {
	sessionStorage["start"] = $.QueryString("start");
	sessionStorage["end"] = $.QueryString("end");
	sessionStorage["pipeline"] = $.QueryString("pipeline");
	sessionStorage["datacen"] = $.QueryString("datacen");
	sessionStorage["network"] = $.QueryString("network");
	sessionStorage["farm"] = $.QueryString("farm");
}

function setFields() {
	$(".from").val(sessionStorage["start"]);
	$(".to").val(sessionStorage["end"]);
	$("#FeaturedContent_Datacenter").val(sessionStorage["datacen"]);
	$("#FeaturedContent_Network").val(sessionStorage["network"]);
	$("#FeaturedContent_Farm").val(sessionStorage["farm"]);
}

function setPipeline(id) {
    $("#loading").fadeIn();
	sessionStorage["pipeline"] = id;
	updateQueryString();
}


function setDatacenter(id) {
	sessionStorage["datacen"] = id;
	sessionStorage["network"] = -1;
	sessionStorage["farm"] = -1;
	window.location.href = "DCHM";

}

function setBreadcrumbs() {
	console.log("hi");
	if (sessionStorage["datacen"] != "All" && sessionStorage["network"] != -1 && sessionStorage["farm"] != -1) {
		$(".breadcrumbs").append("<li><a href='../Home/DCHM'>Datacenter " + sessionStorage["datacen"] + "</a></li>");
		$(".breadcrumbs").append("<li><a href='../Home/DCHM'>Network " + sessionStorage["network"] + "</a></li>");
		$(".breadcrumbs").append("<li class='current'>Farm " + sessionStorage["farm"] + "</li>");
	} else if (sessionStorage["datacen"] != "All" && sessionStorage["network"] != -1 && sessionStorage["farm"] == -1) {
		console.log("hi");
		$(".breadcrumbs").append("<li onclick='setDefaults()'><a href='../Home/DCHM'>Datacenter " + sessionStorage["datacen"] + "</a></li>");
		$(".breadcrumbs").append("<li onclick='setDefaults()' class='current'>Network " + sessionStorage["network"] + "</li>");
	} else if (sessionStorage["datacen"] != "All" && sessionStorage["network"] == -1 && sessionStorage["farm"] == -1) {
		console.log("hi");
		$(".breadcrumbs").append("<li class='current'>Datacenter " + sessionStorage["datacen"] + "</li>");
	}
}

/*
*Set a farm and it's network is found and set in the url too!
*/
function setFarm(id) {
	sessionStorage["farm"] = id;
	sessionStorage["network"] = $("#" + id).parent().attr('id');
	window.location.href = "PercentData";
}

/*
*Set a network and it'll reset the farm!
*/
function setNetwork(id) {
	sessionStorage["network"] = id;
	sessionStorage["farm"] = -1;
	window.location.href = "PercentData";
}


$(document).ready(function () {
    //$("#loading").fadeIn();
	$(document).foundation();
	if (window.location.search == "") {
		updateQueryString();
	} else {
		setSessionStorage();
		setFields();
		setBreadcrumbs();
	}
	//$("#loading").fadeOut("slow");
});

//Move Elsewhere!
$(document).ready(function () {
    $("#loading").fadeIn();
	$(".from").datepicker({
		defaultDate: sessionStorage["start"],
		changeMonth: true,
		changeYear: true,
		dateFormat: 'yy-mm-dd',
		numberOfMonths: 1,
		onClose: function (selectedDate) {
			$(".to").datepicker("option", "minDate", selectedDate);
			$('.from').val(selectedDate);
		}
	});
	$(".to").datepicker({
		defaultDate: sessionStorage["end"],
		changeMonth: true,
		changeYear: true,
		maxDate: "+0D",
		dateFormat: 'yy-mm-dd',
		numberOfMonths: 1,
		onClose: function (selectedDate) {
			$(".from").datepicker("option", "maxDate", selectedDate);
			$('.to').val(selectedDate);
		}
	});

	//$("#loading").fadeOut("slow");
});