var form;
var pid;

$.page_load = function () {

    mini.parse();

    form = new mini.Form("form1");

    var reqStr = $.request("id");
    if (reqStr) {       //修改
        setData(reqStr);
    } else {          //新增
        pid = $.request("pid");

        mini.get("hidPID").setValue(pid);
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
    form.validate();
    if (form.isValid() == false) return;
    $.srv("BaseInfo", $.request("id") ? "DownCodeSave" : "DownCodeAdd", {
        data: { model: obj },
        success: function (data) {
            CloseWindow("save");
        }
    });
}

function getData() {

    var obj = form.getData();
    if (obj.DownCodeID == "") {
        obj.DownCodeID = null;
    }
    return obj;
}


function setData(id) {
    $.srv("BaseInfo", "DownCodeGet", {
        data: { id: id },
        success: function (result) {
            var obj = result.Data;
            form.setData(obj);
            form.setChanged(false);
        }
    });
}