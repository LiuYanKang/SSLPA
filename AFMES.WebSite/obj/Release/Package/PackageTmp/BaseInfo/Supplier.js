var datagrid;

$.page_load = function () {
    datagrid = mini.get("datagrid");
    search();
};

//加载列表
function datagrid_BeforeLoad(e) {
    e.cancel = true;
    loadStationList(e.params);
}

function loadStationList(e) {
    datagrid.loading();

    $.srv("BaseInfo", "SupplierList", {
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


function OnAdd() {                                          //新增
    mini.open({
        title: "新增供应商",
        url: bootPATH + "BaseInfo/SupplierInfo.html",
        width: 400,
        height: 400,
        ondestroy: function (aciton) {
            if (aciton != "save") return;
            mini.showTips({ content: "新增成功", state: "success" });
            search();
        }
    });
};

function OnSee() {
    var row = datagrid.getSelected();
    if (!row) {
        mini.showTips({ content: "请选择要编辑的供应商", state: "danger" });
        return;
    }
    mini.open({
        title: "编辑供应商",
        url: bootPATH + "BaseInfo/SupplierInfo.html?id=" + row.SupplierID,
        width: 400,
        height: 400,
        ondestroy: function (aciton) {
            if (aciton != "save") return;
            mini.showTips({ content: "修改成功", state: "success" });
            search();
        }
    });
}

function OnRemove(e) {
    var row = datagrid.getSelected();
    if (!row) {
        mini.showTips({ content: "请选择要删除的供应商", state: "danger" });
        return;
    };
    mini.confirm("确定删除 <i class='icon-quote-left'></i> <span class='red'>" + row.Code + "</span> <i class='icon-quote-right'></i> ？", "确定？",
       function (action) {
           if (action != "ok") return;
           $.srv("BaseInfo", "SupplierDel", {
               data: { id: row.SupplierID },
               success: function (result) {
                   if (result.Data) {
                       mini.showTips({ content: "删除成功", state: "success" });
                       search();
                   }
               }
           });
       });
}