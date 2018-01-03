
var datagrid, cboAuditType;

$.page_load = function () {
    datagrid = mini.get("datagrid");
    cboAuditType = mini.get("cboAuditType");

    $.dic("1024", function (e) {
        cboAuditType.set({ data: e });
    })

    search();
}


function datagrid_BeforeLoad(e) {
    e.cancel = true;
    loadActionPlanList(e.params);
}


function loadActionPlanList(e) {
    datagrid.loading();

    $.srv("LPA", "ActionPlanList", {
        data: { param: e },
        success: function (result) {
            datagrid.set({
                pageIndex: result.Data.PageIndex
               , pageSize: result.Data.PageSize
               , totalCount: result.Data.TotalCount
                , data: result.Data.Data
            });
        }
    });
}


//查询
function search() {
    var key = mini.get("keyword").getValue();
    var auditTypeStatus = cboAuditType.getValue() == "" ? null : cboAuditType.getValue();
    var planBeginDate = mini.get("txtPlanBeginDate").getValue();
    planBeginDate = planBeginDate == "" ? planBeginDate = null : $.jsDate2WcfDate(planBeginDate);
    var planEndDate = mini.get("txtPlanEndDate").getValue();
    planEndDate = planEndDate == "" ? planEndDate = null : $.jsDate2WcfDate(planEndDate);

    datagrid.load({ Keyword: key, Status: auditTypeStatus, PlanBeginDate: planBeginDate, PlanEndDate: planEndDate });
}


//新增
function ActionPlanAdd() {
    mini.open({
        title: "新增LPA执行计划表",
        url: bootPATH + "LPA/ActionPlanInfo.html",
        width: 700,
        height: 450,
        ondestroy: function (aciton) {
            if (aciton != "save") return;
            mini.showTips({ content: "新增成功", state: "success" });
            search();
        }
    });
};

//删除
function ActionPlanRemove() {
    var row = datagrid.getSelected();
    if (!row) {
        mini.showTips({ content: "请选择要删除的LPA执行计划表", state: "danger" });
        return;
    };
    mini.confirm("确定删除 ？", "确定？",
        function (action) {
            if (action != "ok") return;
            $.srv("LPA", "ActionPlanDel", {
                data: { id: row.PlanID },
                success: function (result) {
                    if (result.Data) {
                        mini.showTips({ content: "删除成功", state: "success" });
                        datagrid.reload();
                    }
                }
            });
        });
};


//批量删除
function onRemoveForBatch() {
    var rows = getRoleIDList();
    if (rows.length == 0) {
        mini.showTips({ content: "请选择要批量删除的执行计划", state: "danger" });
        return;
    };
    mini.confirm("确定批量删除选中的执行计划？", "确定？",
        function (action) {
            if (action != "ok") return;
            $.srv("LPA", "ActionPlanDelForBatch", {
                data: { rowsID: rows },
                success: function (result) {
                    if (result.Data) {
                        mini.showTips({ content: "批量删除成功", state: "success" });
                        datagrid.reload();
                    }
                }
            });
        });
};



//获得选中项编码集合
function getRoleIDList() {
    var rowIDList = [];
    //获取选中项对象集合
    var entityList = datagrid.getSelecteds();
    //判断对象集合有值
    if (entityList && entityList.length > 0) {
        for (var i = 0; i < entityList.length; i++) {
            rowIDList.push(entityList[i].PlanID);
        };
    };
    return rowIDList;
};
