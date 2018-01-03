
var form;

$.page_load = function () {

    mini.parse();

    form = new mini.Form("form1");

    var reqStr = $.request("id");
    if (reqStr) {       //修改
        setData(reqStr);
    }

};

function onOk(e) {
    saveData();
}
function onCancel(e) {
    CloseWindow("cancel");
}

//保存
function saveData() {
    var obj = getData();
    var beginTime = new Date("2016-1-1 " + obj.BeginTime);
    obj.BeginTime = $.jsDate2WcfDate(beginTime);
    var endTime = new Date("2016-1-1 " + obj.EndTime + ":00");
    obj.EndTime = $.jsDate2WcfDate(endTime);

    form.validate();
    if (form.isValid() == false) return;
    $.srv("BaseInfo", $.request("id") ? "WorkingShiftSave" : "WorkingShiftAdd", {
        data: { model: obj },
        success: function (data) {
            CloseWindow("save");
        }
    });
}

function getData() {

    var obj = form.getData(true);
    if (obj.ShiftID == "") {
        obj.ShiftID = null;
    }
    return obj;
}


function setData(id) {
    $.srv("BaseInfo", "WorkingShiftGet", {
        data: { id: id },
        success: function (result) {
            var obj = result.Data;
            form.setData(obj);
            var beginTime = $.wcfDate2JsDate(obj.BeginTime).format("hh:mm");
            mini.get("txtBeginTime").setValue(beginTime);
            var endTime = $.wcfDate2JsDate(obj.EndTime).format("hh:mm");
            mini.get("txtEndTime").setValue(endTime);
            form.setChanged(false);
        }
    });
}

