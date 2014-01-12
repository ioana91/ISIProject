google.load("visualization", "1", { packages: ["corechart"] });

$(document).ready(function () {
    $('#lblError').hide();

    $("#btnDisplay").click(function () {
        var searchString = $("[name='SearchString']").val()

        $.ajax({
            type: "POST",
            async: true,
            data: JSON.stringify({ searchString: searchString }),
            dataType: "json",
            url: "/EmployeesPerProjectReport/SelectOptions",
            contentType: "application/json; charset=utf-8",
            success: function (x) {
                if (x == "error") {
                    $('#lblError').show();
                }
                else {
                    for (var i = 1; i < x.length; i++) {
                        x[i][1] = parseFloat(x[i][1]);
                    }

                    var data = google.visualization.arrayToDataTable(x);
                    
                    var options = {
                        title: "Employees' Working Hours for Specified Project",
                        backgroundColor: '#efeeef'
                    };

                    var chart = new google.visualization.ColumnChart(document.getElementById('chart_div'));
                    chart.draw(data, options);
                }
            },
            failure: function (errMsg) {
                alert(errMsg);
            },
        });
    });
});