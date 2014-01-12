google.load("visualization", "1", { packages: ["corechart"] });

$(document).ready(function () {
    $('#btnUp').hide();
    $('#btnDown').hide();
    
    $("#startDatepicker").datepicker({ firstDay: 1, autoSize: true, defaultDate: 0, showOtherMonths: true, dateFormat: "dd/mm/yy" });
    $("#endDatepicker").datepicker({ firstDay: 1, autoSize: true, defaultDate: 0, showOtherMonths: true, dateFormat: "dd/mm/yy" });

    $("#btnDisplay").click(function () {
        var startDate = $("#startDatepicker").datepicker("getDate");
        var endDate = $("#endDatepicker").datepicker("getDate");

        $.ajax({
            type: "POST",
            async: true,
            data: JSON.stringify({ startDate: startDate, endDate: endDate }),
            dataType: "json",
            url: "/WorkingHoursPerProjectReport/SelectOptions",
            contentType: "application/json; charset=utf-8",
            success: function (x) {
                draw_the_chart(x)
                $('#btnUp').show();
                $('#btnDown').show();
            },
            failure: function (errMsg) {
                alert(errMsg);
            },
        });
    });

    $("#btnUp").click(function () {
        var startDate = $("#startDatepicker").datepicker("getDate");
        var endDate = $("#endDatepicker").datepicker("getDate");

        $.ajax({
            type: "POST",
            async: true,
            data: JSON.stringify({ startDate: startDate, endDate: endDate }),
            dataType: "json",
            url: "/WorkingHoursPerProjectReport/OrderAscending",
            contentType: "application/json; charset=utf-8",
            success: function (x) {
                draw_the_chart(x)
            },
            failure: function (errMsg) {
                alert(errMsg);
            },
        });
    });

    $("#btnDown").click(function () {
        var startDate = $("#startDatepicker").datepicker("getDate");
        var endDate = $("#endDatepicker").datepicker("getDate");

        $.ajax({
            type: "POST",
            async: true,
            data: JSON.stringify({ startDate: startDate, endDate: endDate }),
            dataType: "json",
            url: "/WorkingHoursPerProjectReport/OrderDescending",
            contentType: "application/json; charset=utf-8",
            success: function (x) {
                draw_the_chart(x)
            },
            failure: function (errMsg) {
                alert(errMsg);
            },
        });
    });
});

function draw_the_chart(x) {
    for (var i = 1; i < x.length; i++) {
        x[i][1] = parseFloat(x[i][1]);
    }

    var data = google.visualization.arrayToDataTable(x);

    var options = {
        title: 'Working Hours per Project',
        backgroundColor: '#efeeef'
    };

    var chart = new google.visualization.ColumnChart(document.getElementById('chart_div'));
    chart.draw(data, options);
}