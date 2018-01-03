var datagrid;
var form;
var customerID;

$.page_load = function () {

    datagrid = mini.get("datagrid");
    form = new mini.Form("form1");
    customerID = $.request("customerID");

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
    $.srv("BaseInfo", "CustomerList", {
        data: { pagerParams: pagerParams },
        success: function (result) {
            datagrid.set({
                pageIndex: result.Data.PageIndex
                    , pageSize: result.Data.PageSize
                    , totalCount: result.Data.TotalCount
                , data: result.Data.Data
            });
            var obj = result.Data.Data;
            if (customerID) {
                for (var i = 0; i < obj.length; i++) {
                    if (obj[i].CustID == customerID) {
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
        mini.showTips({ content: "请选择客户", state: "danger" });
        return;
    }

    var obj = {};
    obj.CustID = row.CustID;
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


//显示小图标
function onImg(e) {
    return "<img src=\"" + SS.config.customerLogoPath + e.value + "\" style=\"heigh:32px; width:32px;\"> ";
}