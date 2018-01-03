var form;
var reqStr;
var cboItemRegion, cboItemType, auditType, setMachineOther, AuditArea, cboIsInputData;

$.page_load = function () {

    form = new mini.Form("form1");
    cboItemRegion = mini.get("cboItemRegion");
    cboItemType = mini.get("cboItemType");
    
    setMachineOther = mini.get("setMachineOther");

    //cboIsInputData = mini.get("cboIsInputData")
   
    //isInputData = $.request("IsInputData");

    reqStr = $.request("itemID");
    auditType = $.request("AuditType");
    AuditArea = $.request("AuditArea");
    if (reqStr) {       //修改
        setData(reqStr);
    }

    //$.dic("1025", function (e) {        //区域
    //    cboItemRegion.set({ data: e });
    //});
    // 审核区域
    $.srv("LPA", "AuditAreaGet", {
        success: function (data) {
            cboItemRegion.set({ data: data.Data });
            if (AuditArea) {
                cboItemRegion.setValue(AuditArea);
                $.srv("LPA", "GetAuditMachine", {
                    data: { id: AuditArea },
                    success: function (data) {
                        setMachineOther.set({ data: data.Data });
                    }
                });
            }           
        }
    });

    $.dic("1026", function (e) {        //分类
        cboItemType.set({ data: e });
    });

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

    $.srv("LPA", reqStr ? "AuditItemSave" : "AuditItemAdd", {
        data: { model: obj },
        success: function (data) {
            CloseWindow("save");
        }
    });
}

function getData() {

    var obj = form.getData();
   // obj.cboIsInputData = cboIsInputData.getValue();
  // obj.IsInputData = isInputData;
    obj.AuditType = auditType;
    if (obj.ItemID == "") {
        obj.ItemID = null;
    }
    return obj;
}


function setData(id) {
    $.srv("LPA", "AuditItemGet", {
        data: { id: id },
        success: function (result) {
            var obj = result.Data;
            form.setData(obj);
            form.setChanged(false);
            $.srv("LPA", "GetAuditMachine", {
                data: { id: obj.ItemRegion },
                success: function (data) {
                    setMachineOther.set({ data: data.Data });
                }
            });
        }
    });
}

function GetAuditMachine() {
    var areaId = cboItemRegion.getValue();
    $.srv("LPA", "GetAuditMachine", {
        data: { id: areaId },
        success: function (data) {
            setMachineOther.set({ data: data.Data });
        }
    });
}

//function onIsInputData(e) {
//    if (e.value) {
//        return true;
//    }
//}