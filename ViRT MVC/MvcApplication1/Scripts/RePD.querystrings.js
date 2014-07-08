function setDefaults() {
    sessionStorage["team"] = "Authentication";
    sessionStorage["month"] = "June";
    sessionStorage["year"] = 2014;
}

function updateQueryString() {
    if (sessionStorage["team"] == undefined) {
        setDefaults();
    }
    sessionStorage["query"] = "?team=" + sessionStorage["team"] + "&month=" + sessionStorage["month"] + "&year=" + sessionStorage["year"];
    window.location.search = sessionStorage["query"];
}

function setSessionStorage() {
    sessionStorage["team"] = $.QueryString("team");
    sessionStorage["month"] = $.QueryString("month");
    sessionStorage["year"] = $.QueryString("year");
}