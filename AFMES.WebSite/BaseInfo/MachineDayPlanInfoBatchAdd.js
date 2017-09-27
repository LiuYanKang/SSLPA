var form;
var txtPlanDateBegin, txtMachine;
var machineID, planDate;

$.page_load = function () {

    mini.parse();

    form = new mini.Form("form1");
    txtPlanDateBegin = mini.get("txtPlanDateBegin");
    txtMachine = mini.get("txtMachine");

    machineID = $.request("machineID");
    planDate = $.request("planDate");

    if (machineID && planDate) {       //修改
        txtPlanDateBegin.disable();          //如果是编辑，则日期、设备不可选
        $("#lblDateEnd").hide();
        mini.get("txtPlanDateEnd").setVisible();
        mini.get("txtMachine").disable();
        setData();
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
    var obj = form.getData();
    obj.PlanDateBegin = $.jsDate2WcfDate(obj.PlanDateBegin);
    obj.PlanDateEnd = obj.PlanDateEnd == null ? null : $.jsDate2WcfDate(obj.PlanDateEnd);
    form.validate();
    if (form.isValid() == false) return;
    $.srv("BaseInfo", machineID && planDate ? "MachineDayPlanSave" : "MachineDayPlanAdd", {
        data: { model: obj },
        success: function (data) {
            CloseWindow("save");
        }
    });
}


function setData() {
    $.srv("BaseInfo", "MachineDayPlanGet", {
        data: { machineID: machineID, planDate: planDate },
        success: function (result) {
            var obj = result.Data;
            txtMachine.setText(obj.MachineName);
            txtMachine.setValue(obj.MachineID);
            form.setData(obj);
            var planDate = $.wcfDate2JsDate(obj.PlanDate).format("yyyy-MM-dd");
            txtPlanDateBegin.setValue(planDate);
            form.setChanged(false);
        }
    });
}

function onMachine(e) {
    var txt = e.sender;

    mini.open({
        title: "请选择设备...",
        url: bootPATH + "BaseInfo/MachineSel.html?machineID=" + txtMachine.getValue(),
        width: 640,
        height: 490,
        onload: function () {
            var iframe = this.getIFrameEl();
        },
        ondestroy: function (action) {
            if (action == "ok") {
                var iframe = this.getIFrameEl();
                var data = iframe.contentWindow.getData();
                if (data.length < 1) return false;
                data = mini.clone(data);
                txt.setValue(data[0].MachineID);
                txt.setText(data[0].Name);
                txt.doValueChanged(e);
            }
        }
    });
}