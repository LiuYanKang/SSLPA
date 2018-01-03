
var datagrid;
var cboMachine;
var key, machineID;

$.page_load = function () {
    datagrid = mini.get("datagrid");
    cboMachine = mini.get("cboMachine");

    // 默认当月一号
    mini.get("txtBeginDate").setValue(new Date((new Date()).setDate(1)));

    //默认当月最后一天
    mini.get("txtEndDate").setValue(new Date());

    //绑定设备下拉框里的数据
    $.srv("BaseInfo", "MachineAllS", {
        success: function (result) {
            cboMachine.set({ data: result.Data });
            if (result.Data.length > 0) {
                //cboMachine.doValueChanged();
            }
        }
    });

    search();
}


function datagrid_BeforeLoad(e) {
    e.cancel = true;
    loadMachineDayPlanList(e.params);
}


function loadMachineDayPlanList(e) {
    datagrid.loading();

    $.srv("BaseInfo", "MachineDayPlanQuery", {
        data: { pagerParams: e },
        success: function (result) {
            datagrid.set({
                data: result.Data
            });
        }
    });
}


function search() {
    key = mini.get("keyword").getValue();
    machineID = cboMachine.getValue() == "" ? null : cboMachine.getValue();
    var BeginDate = mini.get("txtBeginDate").getValue();
    BeginDate = BeginDate == "" ? BeginDate = null : $.jsDate2WcfDate(BeginDate);
    var EndDate = mini.get("txtEndDate").getValue();
    EndDate = EndDate == "" ? EndDate = null : $.jsDate2WcfDate(EndDate);
    datagrid.load({ Keyword: key, MachineID: machineID, StartTime: BeginDate, EndTime: EndDate });
}

//新增
function onAdd() {
    mini.open({
        title: "新增计划日产量",
        url: bootPATH + "BaseInfo/MachineDayPlanInfo.html",
        width: 650,
        height: 310,
        ondestroy: function (aciton) {
            if (aciton != "save") return;
            mini.showTips({ content: "新增成功", state: "success" });
            datagrid.reload();
        }
    });
};

//新增
function onBatchAdd() {
    mini.open({
        title: "新增计划日产量",
        url: bootPATH + "BaseInfo/MachineDayPlanInfoBatchAdd.html",
        width: 650,
        height: 310,
        ondestroy: function (aciton) {
            if (aciton != "save") return;
            mini.showTips({ content: "新增成功", state: "success" });
            datagrid.reload();
        }
    });
};
//编辑
function onEdit() {
    var row = datagrid.getSelected();
    if (!row) {
        mini.showTips({ content: "请选择要编辑的设备日产量计划", state: "danger" });
        return;
    }

    mini.open({
        title: "编辑设备日产量计划",
        url: bootPATH + "BaseInfo/MachineDayPlanInfo.html?machineID=" + row.MachineID + "&planDate=" + row.PlanDate,
        width: 650,
        height: 310,
        ondestroy: function (aciton) {
            if (aciton != "save") return;
            mini.showTips({ content: "修改成功", state: "success" });
            datagrid.reload();
        }
    });
};
//删除
function onRemove() {
    var row = datagrid.getSelected();
    if (!row) {
        mini.showTips({ content: "请选择要删除的设备日产量计划", state: "danger" });
        return;
    };
    mini.confirm("确定删除 ？", "确定？",
        function (action) {
            if (action != "ok") return;
            $.srv("BaseInfo", "MachineDayPlanDel", {
                data: { machineID: row.MachineID, planDate: row.PlanDate },
                success: function (result) {
                    if (result.Data) {
                        mini.showTips({ content: "删除成功", state: "success" });
                        datagrid.reload();
                    }
                }
            });
        });
};

