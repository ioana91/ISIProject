google.load('visualization', '1', { packages: ['corechart'] })

$(document).ready(function () {
    $("#startDatepicker").datepicker({ firstDay: 1, autoSize: true, defaultDate: 0, showOtherMonths: true, dateFormat: "dd/mm/yy" });
    $("#endDatepicker").datepicker({ firstDay: 1, autoSize: true, defaultDate: 0, showOtherMonths: true, dateFormat: "dd/mm/yy" });

    $("#btnDisplay").click(function () {
        var startDate = $("#startDatepicker").datepicker("getDate");
        var endDate = $("#endDatepicker").datepicker("getDate");
        var selectedDepartment = $("select[name='SelectedDivision']").val()

        $.ajax({
            type: "POST",
            async: true,
            data: JSON.stringify({ startDate: startDate, endDate: endDate, selectedDepartment: selectedDepartment }),
            dataType: "json",
            url: "/AllProjectsInDivisionReport/SelectOptions",
            contentType: "application/json; charset=utf-8",
            success: function (x) {
                for (var i = 1; i < x.length; i++) {
                    for (var j = 1; j < x[0].length; j++) {
                        x[i][j] = parseFloat(x[i][j]);
                    }
                }

                var data = google.visualization.arrayToDataTable(x);

                var options = {
                    title: 'All Projects in Division',
                    vAxis: { title: "Working Hours" },
                    hAxis: { title: "Project" },
                    seriesType: "bars",
                    backgroundColor: '#efeeef'
                };

                var chart = new google.visualization.ComboChart(document.getElementById('chart_div'));
                chart.draw(data, options);
            },
            failure: function (errMsg) {
                alert(errMsg);
            },
        });
    });
});