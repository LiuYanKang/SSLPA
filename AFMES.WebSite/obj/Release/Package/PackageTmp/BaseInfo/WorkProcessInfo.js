var form;
var cboProcType;
var sPDCode;

$.page_load = function () {

    mini.parse();

    form = new mini.Form("form1");
    //cboProcType = mini.get("cboProcType");
    sPDCode = mini.get("sPDCode");
    //$.dic("1007", function (e) {
    //    cboProcType.set({ data: e });
    //});
    // 产品部门
    $.srv("BaseInfo", "ProDept", {
        success: function (data) {
            sPDCode.set({ data: data.Data });
        }
    });
    var reqStr = $.request("id");
    if (reqStr) {       //修改
        setData(reqStr);
        sPDCode.disable();
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
    $.srv("BaseInfo", $.request("id") ? "WorkProcessSave" : "WorkProcessAdd", {
        data: { model: obj },
        success: function (data) {
            CloseWindow("save");
        }
    });
}

function getData() {

    var obj = form.getData();
    if (obj.ProcID == "") {
        obj.ProcID = null;
    }
    return obj;
}


function setData(id) {
    $.srv("BaseInfo", "WorkProcessGet", {
        data: { id: id },
        success: function (result) {
            var obj = result.Data;
            form.setData(obj);
            form.setChanged(false);
        }
    });
}