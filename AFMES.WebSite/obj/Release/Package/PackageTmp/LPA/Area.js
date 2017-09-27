
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
    $.srv("LPA", "AreaList", {
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
function AreaAdd() {
    mini.open({
        title: "新增区域",
        url: bootPATH + "LPA/AreaInfo.html",
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
function AreaEdit() {
    var row = datagrid.getSelected();
    if (!row) {
        mini.showTips({ content: "请选择要编辑的区域", state: "danger" });
        return;
    }

    mini.open({
        title: "编辑区域",
        url: bootPATH + "LPA/AreaInfo.html?areaId=" + row.AreaId,
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
function AreaRemove() {
    var row = datagrid.getSelected();
    if (!row) {
        mini.showTips({ content: "请选择要删除的区域", state: "danger" });
        return;
    };
    mini.confirm("确定删除？", "确定？",
        function (action) {
            if (action != "ok") return;
            $.srv("LPA", "AreaDel", {
                data: { id: row.AreaId },
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

