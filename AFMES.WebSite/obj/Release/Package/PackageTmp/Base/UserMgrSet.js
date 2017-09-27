
var cbl, form;
var mgrType, stationID;
var userID, userName, loginName;

$.page_load = function () {
    form = new mini.Form("form1");
    cbl = mini.get("cbl");
    userID = mini.get("userID");
    var empID = $.request("EmpID");
    stationID = $.request("StationID");
    load(empID);
}

function load(e) {

    $.srv("Base", "EmployeeGet", {
        data: { id: e },
        success: function (result) {
            var obj = result.Data;
            mgrType = obj.MgrType;
            userID.setValue(obj.UserID);
            $("#userName").html(obj.Name);
            $("#loginName").html(obj.LoginName);
            if (mgrType == "2") {
                loadStation();
            } else {
                loadRoom(stationID);
            }
        }
    });

}


function loadStation() {
    cbl.setValueField("StationID");
    $.srv("BaseInfo", "StationAll", {
        data: {
            param: {
                Status: null,
                sortField: "Name",
                sortOrder: "ASC"
            }
        },
        success: function (result) {
            cbl.set({
                data: result.Data
            });

            $.srv("Base", "GetUserMgrStationByUserID", {
                data: { userID: userID.getValue() },
                success: function (result) {
                    var checkedList = "";
                    var obj = result.Data;
                    for (var i = 0; i < obj.length; i++) {
                        if (i == obj.length - 1)
                            checkedList += obj[i];
                        else
                            checkedList += obj[i] + ",";
                    }
                    cbl.setValue(checkedList);
                }
            });
        }
    });



}

function loadRoom(stationID) {
    cbl.setValueField("RoomID");
    $.srv("BaseInfo", "GetRoomByStationID", {
        data: { id: stationID },
        success: function (result) {
            cbl.set({
                data: result.Data
            });

            $.srv("Base", "GetUserMgrRoomByUserID", {
                data: { userID: userID.getValue() },
                success: function (result) {
                    var checkedList = "";
                    var obj = result.Data;
                    if (obj.length == 0) return;
                    for (var i = 0; i < obj.length; i++) {
                        if (i == obj.length - 1)
                            checkedList += obj[i];
                        else
                            checkedList += obj[i] + ",";
                    }
                    cbl.setValue(checkedList);
                }
            });

        }
    });


}

function onCancel(e) {
    CloseWindow("cancel");
}

function save() {
    if (mgrType == "2") saveStation();
    else saveRoom();
}

function saveStation() {
    var obj = form.getData();
    var stationID = cbl.getValue()
    if (stationID == "" || stationID == null) {
        obj.StationID = null;
    } else {
        obj.StationID = stationID.split(",");
    }
    $.srv("Base", "SaveUserMgrStation", {
        data: { model: obj },
        success: function (data) {
            CloseWindow("save");
        }
    });
}

function saveRoom() {
    var obj = form.getData();
    var roomID = cbl.getValue()
    if (roomID == "" || roomID == null) {
        obj.RoomID = null;
    } else {
        obj.RoomID = roomID.split(",");
    }
    $.srv("Base", "SaveQUserMgrRoom", {
        data: { model: obj },
        success: function (data) {
            CloseWindow("save");
        }
    });
}

function CheckAll() {
    var chkAll = mini.get("chkAll").getValue();
    if (chkAll == "false") {
        cbl.deselectAll();
    } else {
        cbl.selectAll();
    }

}
