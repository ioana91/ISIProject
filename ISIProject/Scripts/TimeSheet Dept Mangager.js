$(document).ready(function () {
    
    var $dialogContent = $('#reject_pop_up');
    $('#calendar').hide();

    $dialogContent.hide();

    populateEmployeeList();

    $("#btnSelectEmployee").click(function () {
        var startDate = new Date();
        startDate.setMonth(startDate.getMonth() - 1);
        startDate.setDate(1);

        var employeeId = $("select[name='employee']").val();
        $("#hdnEmployeeId").val(employeeId);

        if (employeeId != "-1") {
            $('#btnAccept').show();
            $('#btnReject').show();
            initializeCalendar();
            $('#calendar').weekCalendar("gotoWeek", startDate);
            $('#calendar').weekCalendar("refresh");
            $('#calendar').show();
        }
        else {
            $('#btnAccept').hide();
            $('#btnReject').hide();
            $('#calendar').hide();
        }
    });

    $('#btnAccept').click(btnAcceptClicked);
    $('#btnReject').click(btnRejectClicked);

    $('#btnAccept').hide();
    $('#btnReject').hide();

});

function initializeCalendar() {
    var $calendar = $('#calendar');    

    $calendar.weekCalendar({
        timeslotsPerHour: 4,
        allowCalEventOverlap: true,
        overlapEventsSeparate: true,
        firstDayOfWeek: 1,
        businessHours: { start: 9, end: 18, limitDisplay: false },
        daysToShow: 7,
        height: function ($calendar) {
            return 500;
        },
        eventRender: function (calEvent, $event) {
            if (calEvent.state != 0) {
                $event.css("backgroundColor", computeBackgroundColor(calEvent));
                $event.find(".wc-time").css({
                    "backgroundColor": computeTitleColor(calEvent),
                    "border": "1px solid " + computeBorderColor(calEvent)
                });
                calEvent.readOnly = true;
            }
        },
        draggable: function (calEvent, $event) {
            return calEvent.readOnly != true;
        },
        resizable: function (calEvent, $event) {
            return calEvent.readOnly != true;
        },
        eventNew: function (calEvent, $event) {
            $calendar.weekCalendar("removeUnsavedEvents");
        },
        eventDrop: function (calEvent, $event) {
            if (calEvent.readOnly) {
                return;
            }
        },
        eventResize: function (calEvent, $event) {
            if (calEvent.readOnly) {
                return;
            }
        },
        eventClick: function (calEvent, $event) {
            if (calEvent.readOnly) {
                return;
            }
        },
        eventMouseover: function (calEvent, $event) {
        },
        eventMouseout: function (calEvent, $event) {
        },
        noEvents: function () {
        },
        data: function (start, end, callback) {
            callback(getEventData());
        }
    });

    function getEventData() {
        var superEvents;

        var employeeId = $("select[name='employee']").val();

        if (employeeId == "-1") {
            return;
        }

        $.ajax({
            type: "POST",
            async: false,
            data: JSON.stringify({ employeeId: employeeId }),
            dataType: "json",
            url: "/DeptManagerTimesheet/GetEventsForEmployee",
            contentType: "application/json; charset=utf-8",
            success: function (recieved) {
                superEvents = recieved;
                for (i = 0; i < superEvents.length; i++) {
                    superEvents[i].start = new Date(superEvents[i].start);
                    superEvents[i].end = new Date(superEvents[i].end);
                    superEvents[i].readOnly = true;
                }
            },
            failure: function (errMsg) {
                alert(errMsg);
            },
        });
        return superEvents;
    }
}

function populateEmployeeList() {
    $.ajax({
        type: "POST",
        async: false,
        dataType: "json",
        url: "/DeptManagerTimesheet/GetEmployees",
        contentType: "application/json; charset=utf-8",
        success: function (recieved) {
            var employeeDDL = $("select[name='employee']");

            employeeDDL.empty();
            employeeDDL.append("<option value='" + "-1" + "'>" + "" + "</option>");

            for (var i = 0; i < recieved.length; i++) {
                var htmlString = "<option value='" + recieved[i].EmployeeId + "'>" + recieved[i].Name + "</option>";
                employeeDDL.append(htmlString);
            }
        },
        error: function (errMsg) {
            alert(errMsg.error);
            return;
        }
    });
}

function btnAcceptClicked() {
    var employeeId = $("#hdnEmployeeId").val();

    $.ajax({
        type: "POST",
        async: false,
        data: JSON.stringify({ employeeId: employeeId }),
        dataType: "json",
        url: "/DeptManagerTimesheet/AproveTimesheet",
        contentType: "application/json; charset=utf-8",
    });

    $('#calendar').weekCalendar("today");
    $('#calendar').weekCalendar("refresh");
}
function btnRejectClicked() {
    var employeeId = $("#hdnEmployeeId").val();

    var $dialogContent = $("#reject_pop_up");
    $dialogContent.dialog({
        modal: true,
        title: "Reason for rejecting",
        close: function () {
            $dialogContent.dialog("destroy");
            $dialogContent.hide();
        },
        buttons: {
            send: function () {
                var message = $('#txtMessage').text();

                $.ajax({
                    type: "POST",
                    async: false,
                    data: JSON.stringify({ employeeId: employeeId, message: message }),
                    dataType: "json",
                    url: "/DeptManagerTimesheet/RejectTimesheet",
                    contentType: "application/json; charset=utf-8",
                });

                $('#calendar').weekCalendar("today");
                $('#calendar').weekCalendar("refresh");
                $dialogContent.dialog("close");
            },
            cancel: function () {
                $dialogContent.dialog("close");
            }
        }
    }).show();
}

function computeBackgroundColor(calEvent) {
    if (calEvent.state == 1) {
        return "#aaa"
    }
    if (calEvent.state == 2) {
        return "#6dcc62"
    }
    if (calEvent.state == 3) {
        return "#db5359"
    }
}

function computeBorderColor(calEvent) {
    if (calEvent.state == 1) {
        return "#888"
    }
    if (calEvent.state == 2) {
        return "#2b5127"
    }
    if (calEvent.state == 3) {
        return "#41181a"
    }
}

function computeTitleColor(calEvent) {
    if (calEvent.state == 1) {
        return "#999"
    }
    if (calEvent.state == 2) {
        return "#4c8e44"
    }
    if (calEvent.state == 3) {
        return "#993a3e"
    }
}