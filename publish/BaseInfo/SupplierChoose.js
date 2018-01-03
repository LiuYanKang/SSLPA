var datagrid;
var form;
var supplierID;

$.page_load = function () {

    datagrid = mini.get("datagrid");
    form = new mini.Form("form1");
    supplierID = $.request("supplierID");

    search();
};

//加载列表
function datagrid_BeforeLoad(e) {
    e.cancel = true;
    loadSupplierList(e.params);
};

//查询
function search() {
    var key = mini.get("keyword").getValue();
    datagrid.load({ Keyword: key });
};

function loadSupplierList(pagerParams) {
    datagrid.loading();
    $.srv("BaseInfo", "SupplierQuery", {
        data: { pagerParams: pagerParams },
        success: function (result) {
            datagrid.set({
                pageIndex: result.Data.PageIndex
                    , pageSize: result.Data.PageSize
                    , totalCount: result.Data.TotalCount
                , data: result.Data.Data
            });
            var obj = result.Data.Data;
            if (supplierID) {
                for (var i = 0; i < obj.length; i++) {
                    if (obj[i].SupplierID == supplierID) {
                        datagrid.setSelected(obj[i]);
                    }
                }
            }
        }
    });
};

function getData() {
    var row = datagrid.getSelected();
    if (!row) {
        mini.showTips({ content: "请选择供应商", state: "danger" });
        return;
    }

    var obj = {};
    obj.SupplierID = row.SupplierID;
    obj.Code = row.Code;
    obj.Name = row.Name;
    return obj;
}

function CloseWindow(action) {
    if (window.CloseOwnerWindow) return window.CloseOwnerWindow(action);
    else window.close();
};
function onOk() {
    CloseWindow("ok");
};
function onCancel() {
    CloseWindow("cancel");
};

