var form;
var reqStr;
var PDCode;

$.page_load = function () {

    form = new mini.Form("form1");
    PDCode =mini.get("PDCode");
    reqStr = $.request("itemID");
    if (reqStr) {       //修改
        setData(reqStr);
        PDCode.disable();
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

    $.srv("BaseInfo", "ProductDeptSave", {
        data: { model: obj },
        success: function (data) {
            CloseWindow("save");
        }
    });
}

function getData() {
    var obj = form.getData();
    if (obj.PDCode == "") {
        obj.PDCode = null;
    }
    return obj;
}

function setData(id) {
    $.srv("BaseInfo", "ProductDeptGet", {
        data: { pdcode: id },
        success: function (result) {
            var obj = result.Data;
            form.setData(obj);
            form.setChanged(false);
        }
    });
}