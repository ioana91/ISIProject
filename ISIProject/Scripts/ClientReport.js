$(document).ready(function () {
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
            url: "/ClientReport/SelectOptions",
            contentType: "application/json; charset=utf-8",
            success: function (x) {
                
            },
            failure: function (errMsg) {
                alert(errMsg);
            },
        });
    });
});