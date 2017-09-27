
var datagrid;

$.page_load = function () {
    datagrid = mini.get("datagrid");
    search();
}
function datagrid_BeforeLoad(e) {
    e.cancel = true;
    loadAuditItemList(e.params);
}

function loadAuditItemList(e) {
    datagrid.loading();
    $.srv("BaseInfo", "ProductLineList", {
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
    datagrid.load({ Keyword: key });
}



//新增
function ProductLineAdd() {
    mini.open({
        title: "新增产线",
        url: bootPATH + "BaseInfo/ProductLineInfo.html",
        width: 660,
        height: 500,
        ondestroy: function (aciton) {
            if (aciton != "save") return;
            mini.showTips({ content: "新增成功", state: "success" });
            search();
        }
    });
};

//编辑
function ProductLineEdit() {
    var row = datagrid.getSelected();
    if (!row) {
        mini.showTips({ content: "请选择要编辑的产线", state: "danger" });
        return;
    }

    mini.open({
        title: "编辑产线",
        url: bootPATH + "BaseInfo/ProductLineInfo.html?proLineId=" + row.ProLineId,
        width: 660,
        height: 500,
        ondestroy: function (aciton) {
            if (aciton != "save") return;
            mini.showTips({ content: "修改成功", state: "success" });
            datagrid.reload();
        }
    });
};
//删除
function ProductLineRemove() {
    var row = datagrid.getSelected();
    if (!row) {
        mini.showTips({ content: "请选择要删除的产线", state: "danger" });
        return;
    };
    mini.confirm("确定删除？", "确定？",
        function (action) {
            if (action != "ok") return;
            $.srv("BaseInfo", "ProductLineDel", {
                data: { id: row.ProLineId },
                success: function (result) {
                    if (result.Data) {
                        mini.showTips({ content: "删除成功", state: "success" });
                        datagrid.reload();
                    }
                }
            });
        });
};

function onDescRender(e) {
    return e.value;
}

