
var datagrid, cboAuditType, cboAuditArea,sPDCode;

$.page_load = function () {
    datagrid = mini.get("datagrid");
    cboAuditType = mini.get("cboAuditType");
    cboAuditArea = mini.get("cboAuditArea");
    sPDCode = mini.get("sPDCode");

    $.dic("1024", function (e) {        //审核类型
        cboAuditType.set({ data: e });
    });
    // 审核区域
    $.srv("LPA", "AuditAreaGet", {
        success: function (data) {
            cboAuditArea.set({ data: data.Data });
        }
    });
    // 产品部门
    $.srv("BaseInfo", "ProDept", {
        success: function (data) {
            sPDCode.set({ data: data.Data });
        }
    });
    search();
}


function datagrid_BeforeLoad(e) {
    e.cancel = true;
    loadAuditItemList(e.params);
}


function loadAuditItemList(e) {
    datagrid.loading();

    $.srv("LPA", "AuditItemList", {
        data: { param: e },
        success: function (result) {
            datagrid.set({
                data: result.Data
            });
        }
    });
}


//查询
function search() {
    var key = mini.get("keyword").getValue();
    var auditType = cboAuditType.getValue();
    var auditArea = cboAuditArea.getValue();
    var pdCode=sPDCode.getValue();
    datagrid.load({ Keyword: key, AuditType: auditType, AuditArea: auditArea, PDCode: pdCode });
}


//新增
function AuditItemAdd() {
    mini.open({
        title: "新增审核项目",
        url: bootPATH + "LPA/AuditItemInfo.html?AuditType=" + cboAuditType.getValue() + "&AuditArea=" + cboAuditArea.getValue(),
        width: 500,
        height: 340,
        ondestroy: function (aciton) {
            if (aciton != "save") return;
            mini.showTips({ content: "新增成功", state: "success" });
            search();
        }
    });
};

//编辑
function AuditItemEdit() {
    var row = datagrid.getSelected();
    if (!row) {
        mini.showTips({ content: "请选择要编辑的审核项目", state: "danger" });
        return;
    }

    mini.open({
        title: "编辑审核项目",
        url: bootPATH + "LPA/AuditItemInfo.html?itemID=" + row.ItemID,
        width: 500,
        height: 340,
        ondestroy: function (aciton) {
            if (aciton != "save") return;
            mini.showTips({ content: "修改成功", state: "success" });
            datagrid.reload();
        }
    });
};
//删除
function AuditItemRemove() {
    var row = datagrid.getSelected();
    if (!row) {
        mini.showTips({ content: "请选择要删除的审核项目", state: "danger" });
        return;
    };
    mini.confirm("确定删除？", "确定？",
        function (action) {
            if (action != "ok") return;
            $.srv("LPA", "AuditItemDel", {
                data: { id: row.ItemID },
                success: function (result) {
                    if (result.Data) {
                        mini.showTips({ content: "删除成功", state: "success" });
                        datagrid.reload();
                    }
                }
            });
        });
};


//拖拽后保存
function datagrid_drop() {
    var idList = [];
    var obj = datagrid.getData();
    for (var i = 0; i < obj.length; i++) {
        idList.push(obj[i].ItemID);
    }

    $.srv("LPA", "AuditItemSNSave", {
        data: { idList: idList },
        success: function (data) {
            mini.showTips({ content: "排序保存成功", state: "success" });
        }
    });
}

function onDescRender(e) {
    return e.value;
}