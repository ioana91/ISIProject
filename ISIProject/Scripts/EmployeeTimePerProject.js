google.load("visualization", "1", { packages: ["corechart"] });

$(document).ready(function () {
    $("#startDatepicker").datepicker({ firstDay: 1, autoSize: true, defaultDate: 0, showOtherMonths: true, dateFormat: "dd/mm/yy" });
    $("#endDatepicker").datepicker({ firstDay: 1, autoSize: true, defaultDate: 0, showOtherMonths: true, dateFormat: "dd/mm/yy" });

    $("#btnDisplay").click(function () {
        var startDate = $("#startDatepicker").datepicker("getDate");
        var endDate = $("#endDatepicker").datepicker("getDate");
        var selectedEmployee = $("select[name='SelectedEmployee']").val()

        $.ajax({
            type: "POST",
            async: true,
            data: JSON.stringify({ selectedEmployee: selectedEmployee, startDate: startDate, endDate: endDate }),
            dataType: "json",
            url: "/EmployeeTimePerProjectReport/SelectOptions",
            contentType: "application/json; charset=utf-8",
            success: function (x) {
                for (var i = 1; i < x.length; i++) {
                    x[i][1] = parseFloat(x[i][1]);
                }
                
                var data = google.visualization.arrayToDataTable(x);

                var options = {
                    title: 'Employee Working Hours per Project',
                    backgroundColor: '#efeeef'
                };

                var chart = new google.visualization.PieChart(document.getElementById('piechart'));
                chart.draw(data, options);
            },
            failure: function (errMsg) {
                alert(errMsg);
            },
        });
    });
});