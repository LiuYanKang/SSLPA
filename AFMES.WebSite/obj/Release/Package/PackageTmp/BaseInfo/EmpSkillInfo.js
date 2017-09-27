var form;
var cboMachine, cboSkillLevel;

$.page_load = function () {
    form = new mini.Form("form1");


    cboSkillLevel = mini.get("cboSkillLevel");
    $.dic("1033", function (e) {
        cboSkillLevel.set({ data: e });
    });

    //绑定设备下拉框里的数据
    cboMachine = mini.get("cboMachine");
    $.srv("BaseInfo", "MachineAllS", {
        success: function (result) {
            cboMachine.set({ data: result.Data });
        }
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

    $.srv("BaseInfo", "EmpSkillSave", {
        data: { model: obj },
        success: function (data) {
            CloseWindow("save");
        }
    });
}

function getData() {
    var obj = form.getData();
    return obj;
}


function setData(data) {
    $('#lblEmpName').text(data.EmpName);
    form.setData(data);
    if (data.MachineID) {
        cboMachine.setReadOnly(true);
    }
    form.setChanged(false);
}
