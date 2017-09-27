
var datagrid, cboStatus;


$.page_load = function () {
    datagrid = mini.get("datagrid");
    cboStatus = mini.get("cboStatus");
    $.dic("1003", function (e) {// 订单状态
        cboStatus.set({ data: e });
    });
    search();
}



function datagrid_BeforeLoad(e) {
    e.cancel = true;
    loadStockLocationList(e.params);
}


function loadStockLocationList(e) {
    datagrid.loading();

    $.srv("BaseInfo", "StockLocationQuery", {
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
    datagrid.load(searchKey());
}


function searchKey() {
    var key = mini.get("keyword").getValue();
    var status = cboStatus.getValue();
    status = status == "" ? null : status;
    return {
        Keyword: key,
        Status: status
    }
}


//新增
function onAdd() {
    mini.open({
        title: "新增库位",
        url: bootPATH + "BaseInfo/StockLocationInfo.html",
        width: 665,
        height: 310,
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
        mini.showTips({ content: "请选择要编辑的库位", state: "danger" });
        return;
    }

    mini.open({
        title: "编辑库位",
        url: bootPATH + "BaseInfo/StockLocationInfo.html?id=" + row.LocID,
        width: 665,
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
        mini.showTips({ content: "请选择要删除的库位", state: "danger" });
        return;
    };
    mini.confirm("确定删除 <i class='icon-quote-left'></i> <span class='red'>" + row.Name + "</span> <i class='icon-quote-right'></i> ？", "确定？",
        function (action) {
            if (action != "ok") return;
            $.srv("BaseInfo", "StockLocationDel", {
                data: { id: row.LocID },
                success: function (result) {
                    if (result.Data) {
                        mini.showTips({ content: "删除成功", state: "success" });
                        datagrid.reload();
                    }
                }
            });
        });
};

// 显示二维码
function showQrCode() {

    var row = datagrid.getSelected();
    if (!row) {
        mini.showTips({ content: "请选择要查看的项", state: "danger" });
        return;
    };

    var stockType = {
        "1": "StockLocRM",
        "2": "StockLocSM",
        "3": "StockLocMP"
    }
    var data = { Type: stockType[row.StockType], Value: row.Code };
    SS.showQrCode(JSON.stringify(data));
}
