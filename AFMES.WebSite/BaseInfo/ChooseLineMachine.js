
var datagrid;
var multiSelect, selectedItems, pdCode;

$.page_load = function () {

    selectedItems = [];
    datagrid = mini.get("datagrid");
    pdCode = $.request("pdCode");
    search();
}


//加载列表
function datagrid_BeforeLoad(e) {
    e.cancel = true;
    loadChooseLPAEmployee(e.params);
};


function loadChooseLPAEmployee(pagerParams) {
    datagrid.loading();
    $.srv("BaseInfo", "GetProductLineList", {
        data: { param: pagerParams, pDCode: pdCode },
        success: function (result) {
            datagrid.set({
                pageIndex: result.Data.PageIndex
                    , pageSize: result.Data.PageSize
                    , totalCount: result.Data.TotalCount
                , data: result.Data.Data
            });
        }
    });
};


function search() {
    var key = mini.get("keyword").getValue();

    datagrid.load({ Keyword: key });
}


//选中后点击确认  会将数据返回到上一个页面中
function getData() {
    var row = datagrid.getSelecteds();
    if (!row) {
        mini.showTips({ content: "请选择设备", state: "danger" });
        return;
    }
    return row;
};


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


