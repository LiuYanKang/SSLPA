var form;
var reqStr;
var datagrid;
var sPDCode;

$.page_load = function () {

    form = new mini.Form("form1");
    datagrid = mini.get("datagrid");
    reqStr = $.request("proLineId");
    sPDCode = mini.get("sPDCode");

    // 产品部门
    $.srv("BaseInfo", "ProDept", {
        success: function (data) {
            sPDCode.set({ data: data.Data });
        }
    });
    if (reqStr) {       //修改
        GetAllDate(reqStr);
        sPDCode.disable();
    }


};

function GetAllDate(reqStr) {
    $.srv("BaseInfo", "ProductLineGet", {
        data: { id: reqStr },
        success: function (result) {
            var obj = result.Data;
            form.setData(obj);
            form.setChanged(false);
            datagrid.set({
                data: obj.MachineList
            });


        }
    });
}

function onOk(e) {
    saveData();
}

function onCancel(e) {
    CloseWindow("cancel");
}

//保存
function saveData() {
    var obj = getData();
    obj.MachineList = datagrid.getData();
    form.validate();
    if (form.isValid() == false) return;

    $.srv("BaseInfo", "ProductLineSave", {
        data: { model: obj },
        success: function (data) {
            CloseWindow("save");
        }
    });
}

function getData() {
    var obj = form.getData();
    if (obj.ProLineId == "") {
        obj.ProLineId = null;
    }
    return obj;
}


function onMachine() {
    var obj = form.getData();
    if (obj.PDCode == "") {
        obj.PDCode = null;
    }
    mini.open({
        title: "请选择设备...",
        url: bootPATH + "BaseInfo/MachineSel.html?pdCode=" + obj.PDCode,
        width: 660,
        height: 490,
        onload: function() {
            var iframe = this.getIFrameEl();
        },
        ondestroy: function(action) {
            if (action == "ok") {
                var iframe = this.getIFrameEl();
                var data = iframe.contentWindow.getData();
                var material = datagrid.getData();
                if (material.length > 0) {
                    for (var i = 0; i < data.length; i++) {
                        var isAdd = true;
                        for (var j = 0; j < material.length; j++) {
                            if (material[j].MachineID == data[i].MachineID) {
                                isAdd = false;
                            }
                        }
                        if (isAdd) {
                            datagrid.addRow({ MachineID: data[i].MachineID, Name: data[i].Name, Code: data[i].Code, WorkProcessName: data[i].WorkProcessName });
                        }
                    }
                } else {
                    datagrid.setData(data);
                }

            }
            return true;
        }
    });
}

function onDeleteRenderer() {
    return "<a href='javascript:onMaterialRemove()'>删除</a>";
}

function onMaterialRemove() {
    var row = datagrid.getSelected();
    datagrid.removeRow(row, true);
};
//拖拽后保存
function datagrid_drop() {
    var idList = [];
    var obj = datagrid.getData();
    for (var i = 0; i < obj.length; i++) {
        idList.push(obj[i].MachineID);
    }

    $.srv("BaseInfo", "ProductLineSnSave", {
        data: { idList: idList },
        success: function (data) {
            mini.showTips({ content: "排序保存成功", state: "success" });
        }
    });
}
