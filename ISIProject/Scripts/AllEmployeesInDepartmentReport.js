google.load('visualization', '1', { packages: ['table'] });

$(document).ready(function () {
    $("#btnDisplay").click(function () {
        var selectedDepartment = $("select[name='SelectedDepartment']").val()

        $.ajax({
            type: "POST",
            async: true,
            data: JSON.stringify({ selectedDepartment: selectedDepartment }),
            dataType: "json",
            url: "/AllEmployeesInDepartmentReport/SelectOptions",
            contentType: "application/json; charset=utf-8",
            success: function (x) {
                var data = new google.visualization.DataTable();
                data.addColumn('string', 'Name');
                data.addColumn('string', 'Email');
                data.addColumn('string', 'Role');
                data.addRows(x);

                var table = new google.visualization.Table(document.getElementById('table_div'));
                table.draw(data, { showRowNumber: true });
            },
            failure: function (errMsg) {
                alert(errMsg);
            },
        });
    });
});