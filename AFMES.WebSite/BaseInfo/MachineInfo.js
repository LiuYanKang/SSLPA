var form;
var cboWorkProcess, cboProductType, sPDCode;

$.page_load = function () {

    mini.parse();

    form = new mini.Form("form1");
    //cboProductType = mini.get("cboProductType");
    //$.dic("1030", function (e) {
    //    cboProductType.set({ data: e });
    //});
    sPDCode = mini.get("sPDCode");

    // 产品部门
    $.srv("BaseInfo", "ProDept", {
        success: function (data) {
            sPDCode.set({ data: data.Data });
        }
    });
    cboWorkProcess = mini.get("cboWorkProcess");
    $.srv("BaseInfo", "WorkProcessAllS", {
        success: function (result) {
            cboWorkProcess.set({ data: result.Data });
            if (result.Data.length > 0) {
                cboWorkProcess.doValueChanged();
            }
        }
    });
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
    form.validate();
    if (form.isValid() == false) return;
    $.srv("BaseInfo", $.request("id") ? "MachineSave" : "MachineAdd", {
        data: { model: obj },
        success: function (data) {
            CloseWindow("save");
        }
    });
}

function getData() {

    var obj = form.getData();
    if (obj.MachineID == "") {
        obj.MachineID = null;
    }
    return obj;
}


function setData(id) {
    $.srv("BaseInfo", "MachineGet", {
        data: { id: id },
        success: function (result) {
            var obj = result.Data;
            form.setData(obj);
            form.setChanged(false);
        }
    });
}