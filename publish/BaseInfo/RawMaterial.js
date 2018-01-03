
var datagrid;

$.page_load = function () {
    datagrid = mini.get("datagrid");
    search();
}



function datagrid_BeforeLoad(e) {
    e.cancel = true;
    loadRawMaterialList(e.params);
}


function loadRawMaterialList(e) {
    datagrid.loading();

    $.srv("BaseInfo", "RawMaterialQuery", {
        data: { pagerParams: e },
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
function onAdd() {
    mini.open({
        title: "新增原材料",
        url: bootPATH + "BaseInfo/RawMaterialInfo.html",
        width: 550,
        height: 350,
        ondestroy: function (aciton) {
            if (aciton != "save") return;
            mini.showTips({ content: "新增成功", state: "success" });
            search();
        }
    });
};

//编辑
function onEdit() {
    var row = datagrid.getSelected();
    if (!row) {
        mini.showTips({ content: "请选择要编辑的原材料", state: "danger" });
        return;
    }

    mini.open({
        title: "编辑原材料",
        url: bootPATH + "BaseInfo/RawMaterialInfo.html?id=" + row.RawMaterialID,
        width: 550,
        height: 350,
        ondestroy: function (aciton) {
            if (aciton != "save") return;
            mini.showTips({ content: "修改成功", state: "success" });
            search();
        }
    });
};
//删除
function onRemove() {
    var row = datagrid.getSelected();
    if (!row) {
        mini.showTips({ content: "请选择要删除的原材料", state: "danger" });
        return;
    };
    mini.confirm("确定删除 <i class='icon-quote-left'></i> <span class='red'>" + row.Code + "</span> <i class='icon-quote-right'></i> ？", "确定？",
        function (action) {
            if (action != "ok") return;
            $.srv("BaseInfo", "RawMaterialDel", {
                data: { id: row.RawMaterialID },
                success: function (result) {
                    if (result.Data) {
                        mini.showTips({ content: "删除成功", state: "success" });
                        search();
                    }
                }
            });
        });
};

// 查看条码
function showBarCode() {
    var row = datagrid.getSelected();
    if (!row) {
        mini.showTips({ content: "请选择要查看的项", state: "danger" });
        return;
    }

    SS.showBarCode(row.Code);
}

function onRawMaterialRenderer(e) {
    return "<a href='javascript:onQuery(" + e.row.RawMaterialID + ")' title='点击查看原材料'>" + e.row.Code + "</a>";
}

function onQuery(id) {
    mini.open({
        title: "原材料详情",
        url: bootPATH + "BaseInfo/RawMaterialView.html?RawMaterialID=" + id,
        width: 700,
        height: 360,
        ondestroy: function (action) {
            if (action != "save") return;
            search();
        }
    });
}


