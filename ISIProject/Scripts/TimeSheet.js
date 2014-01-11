var activities;
var clients;
var projects;
var activityType = function () { return $("input[name='activityType']:radio") }
var activityTypeValue = activityType().val();

var selectActivityValue = function () { return $("select[name='activity']").val(); }
var selectActivityText = function () { return $("select[name='activity'] :selected").text(); }

var selectClientValue = function () { return $("select[name='client']").val(); }
var selectClientText = function () { return $("select[name='client'] :selected").text(); }

var selectProjectValue = function () { return $("select[name='project']").val(); }
var selectProjectText = function () { return $("select[name='project'] :selected").text(); }

$(document).ready(function () {
    var id = 10;
    var $calendar = $('#calendar');

    $("#btnDuplicate").click(function () {
        var date = $("#datepicker").datepicker("getDate");

        $.ajax({
            type: "POST",
            async: true,
            data: JSON.stringify({ date: date }),
            dataType: "json",
            url: "/Timesheet/DuplicateEvents",
            contentType: "application/json; charset=utf-8",
            success: function () {
                location.reload(true);
            },
            failure: function (errMsg) {
                alert(errMsg);
            },
        });

        $('#calendar').weekCalendar("today");
        $('#calendar').weekCalendar("refresh");

    });

    $('#lblError').hide();

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
            if (calEvent.end.getYear() < new Date().getYear() || calEvent.end.getMonth() < new Date().getMonth()) {
                $event.css("backgroundColor", "#aaa");
                $event.find(".wc-time").css({
                    "backgroundColor": "#999",
                    "border": "1px solid #888"
                });
            }
        },
        draggable: function (calEvent, $event) {
            return calEvent.readOnly != true;
        },
        resizable: function (calEvent, $event) {
            return calEvent.readOnly != true;
        },
        eventNew: function (calEvent, $event) {
            var $dialogContent = $("#event_edit_container");
            resetForm($dialogContent);
            var startField = $dialogContent.find("select[name='start']").val(calEvent.start);
            var endField = $dialogContent.find("select[name='end']").val(calEvent.end);
            //var activityField = $dialogContent.find("select[name='activity']");
            //var bodyField = $dialogContent.find("select[name='project']");


            $dialogContent.dialog({
                modal: true,
                title: "New Calendar Event",
                close: function () {
                    $dialogContent.dialog("destroy");
                    $dialogContent.hide();
                    $('#calendar').weekCalendar("removeUnsavedEvents");
                },
                buttons: {
                    save: function () {
                        calEvent.id = id;
                        id++;
                        calEvent.start = new Date(startField.val());
                        calEvent.end = new Date(endField.val());
                        calEvent.title = selectActivityText();
                        calEvent.body = selectProjectText();
                        calEvent.activityID = selectActivityValue();
                        calEvent.clientId = selectClientValue();
                        calEvent.projectId = selectProjectValue();
                        calEvent.state = 0;

                        var calendarEvent = ({
                            start: calEvent.start,
                            end: calEvent.end,
                            clientId: selectClientValue(),
                            projectId: selectProjectValue(),
                            activityId: selectActivityValue(),
                            state : 0
                        });

                        var dataToBeSent = JSON.stringify({ NewEvent: calendarEvent });
                        if (validate()) {
                            $.ajax({
                                type: "POST",
                                async: true,
                                data: JSON.stringify({ NewEvent: calendarEvent }),
                                dataType: "json",
                                url: "/Timesheet/AddEvent",
                                contentType: "application/json; charset=utf-8",
                                success: function (newId) {
                                    calEvent.id = newId;
                                },
                                failure: function (errMsg) {
                                    alert(errMsg);
                                }
                            });
                            $calendar.weekCalendar("removeUnsavedEvents");
                            $calendar.weekCalendar("updateEvent", calEvent);
                            resetDropDownListValues()
                            $dialogContent.dialog("close");
                            $('#lblError').hide();
                        } else {
                            $('#lblError').show();
                        }
                    },
                    cancel: function () {
                        $dialogContent.dialog("close");
                        resetDropDownListValues();
                        $('#lblError').hide();
                    }
                }
            }).show();

            $dialogContent.find(".date_holder").text($calendar.weekCalendar("formatDate", calEvent.start));
            setupStartAndEndTimeFields(startField, endField, calEvent, $calendar.weekCalendar("getTimeslotTimes", calEvent.start));

        },
        eventDrop: function (calEvent, $event) {
            var calendarEvent = ({
                id: calEvent.id,
                start: calEvent.start,
                end: calEvent.end,
                clientId: calEvent.clientId,
                projectId: calEvent.projectId,
                activityId: calEvent.activityId,
                state : calEvent.state,
            });

            $.ajax({
                type: "POST",
                async: true,
                data: JSON.stringify({ NewEvent: calendarEvent }),
                dataType: "json",
                url: "/Timesheet/UpdateEvent",
                contentType: "application/json; charset=utf-8",
                failure: function (errMsg) {
                    alert(errMsg);
                }
            });
        },
        eventResize: function (calEvent, $event) {

            var calendarEvent = ({
                id: calEvent.id,
                start: calEvent.start,
                end: calEvent.end,
                clientId: calEvent.clientId,
                projectId: calEvent.projectId,
                activityId: calEvent.activityId,
                state: calEvent.state,
            });

            $.ajax({
                type: "POST",
                async: true,
                data: JSON.stringify({ NewEvent: calendarEvent }),
                dataType: "json",
                url: "/Timesheet/UpdateEvent",
                contentType: "application/json; charset=utf-8",
                failure: function (errMsg) {
                    alert(errMsg);
                }
            });

            //$calendar.weekCalendar("updateEvent", calEvent);
            //$dialogContent.dialog("close");
        },
        eventClick: function (calEvent, $event) {

            if (calEvent.readOnly) {
                return;
            }

            var $dialogContent = $("#event_edit_container");
            resetForm($dialogContent);
            var startField = $dialogContent.find("select[name='start']").val(calEvent.start);
            var endField = $dialogContent.find("select[name='end']").val(calEvent.end);
            var activityField = $dialogContent.find("input[name='title']").val(calEvent.title);
            var bodyField = $dialogContent.find("textarea[name='body']");
            bodyField.val(calEvent.body);

            $dialogContent.dialog({
                modal: true,
                title: "Edit - " + calEvent.title,
                close: function () {
                    $dialogContent.dialog("destroy");
                    $dialogContent.hide();
                    $('#calendar').weekCalendar("removeUnsavedEvents");
                },
                buttons: {
                    save: function () {

                        calEvent.start = new Date(startField.val());
                        calEvent.end = new Date(endField.val());
                        calEvent.title = selectActivityText();
                        calEvent.body = selectProjectText();
                        calEvent.activityId = selectActivityValue();
                        calEvent.projectId = selectProjectValue();
                        calEvent.clientId = selectClientValue();

                        var calendarEvent = ({
                            id: calEvent.id,
                            start: calEvent.start,
                            end: calEvent.end,
                            clientId: calEvent.clientId,
                            projectId: calEvent.projectId,
                            activityId: calEvent.activityId,
                            state : calEvent.state
                        });
                        if (validate()) {
                            $.ajax({
                                type: "POST",
                                async: true,
                                data: JSON.stringify({ NewEvent: calendarEvent }),
                                dataType: "json",
                                url: "/Timesheet/UpdateEvent",
                                contentType: "application/json; charset=utf-8",
                                failure: function (errMsg) {
                                    alert(errMsg);
                                }
                            });
                            $('#lblError').hide();
                            $calendar.weekCalendar("updateEvent", calEvent);
                            $dialogContent.dialog("close");
                            resetDropDownListValues();
                        }
                        else {
                            $('#lblError').show();
                        }
                    },
                    "delete": function () {
                        $('#lblError').hide();
                        var calendarEvent = ({
                            id: calEvent.id
                        });

                        $.ajax({
                            type: "POST",
                            async: true,
                            data: JSON.stringify({ NewEvent: calendarEvent }),
                            dataType: "json",
                            url: "/Timesheet/RemoveEvent",
                            contentType: "application/json; charset=utf-8",
                            failure: function (errMsg) {
                                alert(errMsg);
                            }
                        });
                        $calendar.weekCalendar("removeEvent", calEvent.id);
                        $dialogContent.dialog("close");
                        resetDropDownListValues();
                    },
                    cancel: function () {
                        $dialogContent.dialog("close");
                        resetDropDownListValues();
                        $('#lblError').hide();
                    }
                }
            }).show();

            var startField = $dialogContent.find("select[name='start']").val(calEvent.start);
            var endField = $dialogContent.find("select[name='end']").val(calEvent.end);
            $dialogContent.find(".date_holder").text($calendar.weekCalendar("formatDate", calEvent.start));
            setupStartAndEndTimeFields(startField, endField, calEvent, $calendar.weekCalendar("getTimeslotTimes", calEvent.start));
            $(window).resize().resize(); //fixes a bug in modal overlay size ??

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

    function resetForm($dialogContent) {
        $dialogContent.find("input").val("");
        $dialogContent.find("textarea").val("");
    }

    function getEventData() {
        var year = new Date().getFullYear();
        var month = new Date().getMonth();
        var day = new Date().getDate();

        var superEvents;

        $.ajax({
            type: "POST",
            async: false,
            dataType: "json",
            url: "/Timesheet/GetEvents",
            contentType: "application/json; charset=utf-8",
            success: function (recieved) {
                superEvents = recieved;
                for (i = 0; i < superEvents.length; i++) {
                    superEvents[i].start = new Date(superEvents[i].start);
                    superEvents[i].end = new Date(superEvents[i].end);

                }
            },
            error: function (errMsg) {
                alert(errMsg);
                return;
            }
        });

        return superEvents;
    }


    /*
     * Sets up the start and end time fields in the calendar event
     * form for editing based on the calendar event being edited
     */
    function setupStartAndEndTimeFields($startTimeField, $endTimeField, calEvent, timeslotTimes) {

        for (var i = 0; i < timeslotTimes.length; i++) {
            var startTime = timeslotTimes[i].start;
            var endTime = timeslotTimes[i].end;
            var startSelected = "";
            if (startTime.getTime() === calEvent.start.getTime()) {
                startSelected = "selected=\"selected\"";
            }
            var endSelected = "";
            if (endTime.getTime() === calEvent.end.getTime()) {
                endSelected = "selected=\"selected\"";
            }
            $startTimeField.append("<option value=\"" + startTime + "\" " + startSelected + ">" + timeslotTimes[i].startFormatted + "</option>");
            $endTimeField.append("<option value=\"" + endTime + "\" " + endSelected + ">" + timeslotTimes[i].endFormatted + "</option>");

        }
        $endTimeOptions = $endTimeField.find("option");
        $startTimeField.trigger("change");
    }

    var $endTimeField = $("select[name='end']");
    var $endTimeOptions = $endTimeField.find("option");

    //reduces the end time options to be only after the start time options.
    $("select[name='start']").change(function () {
        var startTime = $(this).find(":selected").val();
        var currentEndTime = $endTimeField.find("option:selected").val();
        $endTimeField.html(
              $endTimeOptions.filter(function () {
                  return startTime < $(this).val();
              })
              );

        var endTimeSelected = false;
        $endTimeField.find("option").each(function () {
            if ($(this).val() === currentEndTime) {
                $(this).attr("selected", "selected");
                endTimeSelected = true;
                return false;
            }
        });

        if (!endTimeSelected) {
            //automatically select an end date 2 slots away.
            $endTimeField.find("option:eq(1)").attr("selected", "selected");
        }

    });

    $("input[name=activityType]:radio").change(function () {
        if ($('input:radio:checked').attr('id') === "rdb1") {
            $("#clientLi").show();
            $("#projectLi").show();
            populateActivityDropDownList(true);
        } else {
            $("#clientLi").hide();
            $("#projectLi").hide();
            populateActivityDropDownList(false);
        }
    });

    $("select[name='client']").change(function () {
        if (selectClientValue() !== "-1") {
            populateProjectDropDownList(selectClientValue());
        } else {

        }
    });

    populateActivityDropDownList(true);
    populateClientDropDownList();
});

function populateActivityDropDownList(activeOnes) {
    var $dialogContent = $("#event_edit_container");

    if (typeof activities == "undefined") {
        $.ajax({
            type: "POST",
            async: false,
            dataType: "json",
            url: "/Timesheet/GetActivities",
            contentType: "application/json; charset=utf-8",
            success: function (activitiesList) {
                activities = activitiesList;
            },
            failure: function (errMsg) {
                alert(errMsg);
            },
        });
    }
    var activityField = $dialogContent.find("select[name='activity']");

    activityField.empty();
    activityField.append("<option value='" + "-1" + "'>" + "" + "</option>");

    for (var i = 0; i < activities.length; i++) {
        if (activities[i].IsActive == activeOnes) {
            var htmlString = "<option value='" + activities[i].ActivityId + "'>" + activities[i].Name + "</option>";
            activityField.append(htmlString);
        }
    }
}

function populateClientDropDownList() {
    var $dialogContent = $("#event_edit_container");

    if (typeof clients == "undefined") {
        $.ajax({
            type: "POST",
            async: false,
            dataType: "json",
            url: "/Timesheet/GetClients",
            contentType: "application/json; charset=utf-8",
            success: function (clientList) {
                clients = clientList;
            },
            failure: function (errMsg) {
                alert(errMsg);
            },
        });
    }

    var clientField = $dialogContent.find("select[name='client']");

    clientField.empty();
    clientField.append("<option value='" + "-1" + "'>" + "" + "</option>");

    for (var i = 0; i < clients.length; i++) {
        var htmlString = "<option value='" + clients[i].ClientId + "'>" + clients[i].Name + "</option>";
        clientField.append(htmlString);
    }
}

function populateProjectDropDownList(clientID) {
    var $dialogContent = $("#event_edit_container");
   
    
        $.ajax({
            type: "POST",
            async: false,
            data: JSON.stringify({ clientId: clientID }),
            dataType: "json",
            url: "/Timesheet/GetProjects",
            contentType: "application/json; charset=utf-8",
            success: function (projectList) {
                projects = projectList;
            },
            failure: function (errMsg) {
                alert(errMsg);
            },
        });
    var projectField = $dialogContent.find("select[name='project']");

    projectField.empty();
    projectField.append("<option value='" + "-1" + "'>" + "" + "</option>");

    for (var i = 0; i < projects.length; i++) {
        var htmlString = "<option value='" + projects[i].ProjectId + "'>" + projects[i].Name + "</option>";
        projectField.append(htmlString);
    }
}

function validate() {
    if ($('input:radio:checked').attr('id') === "rdb1") {
        if (selectActivityValue() != "-1"
            && selectClientValue() != "-1"
            && selectProjectValue() != "-1") {
            return true;
        }
    } else {
        if (selectActivityValue() != "-1") {
            return true;
        }
    }
    return false;
}

function resetDropDownListValues(){
    $("select[name='activity']").val('-1');
    $("select[name='project']").val('-1');
    $("select[name='client']").val('-1');
}

$(function () {
    $("#datepicker").datepicker({ firstDay: 1, autoSize: true, defaultDate: -1, showOtherMonths: true, dateFormat: "dd/mm/yy" });
    $("#datepicker").datepicker("setDate", "-1");

});