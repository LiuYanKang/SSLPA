var datagrid;
var form;
var rawMaterialID;

$.page_load = function () {

    datagrid = mini.get("datagrid");
    form = new mini.Form("form1");
    rawMaterialID = $.request("rawMaterialID");

    search();
};

//加载列表
function datagrid_BeforeLoad(e) {
    e.cancel = true;
    loadRawMaterialList(e.params);
};

//查询
function search() {
    var key = mini.get("keyword").getValue();
    datagrid.load({ Keyword: key });
};

function loadRawMaterialList(pagerParams) {
    datagrid.loading();
    $.srv("BaseInfo", "RawMaterialQuery", {
        data: { pagerParams: pagerParams },
        success: function (result) {
            datagrid.set({
                pageIndex: result.Data.PageIndex
                    , pageSize: result.Data.PageSize
                    , totalCount: result.Data.TotalCount
                , data: result.Data.Data
            });
            var obj = result.Data.Data;
            if (rawMaterialID) {
                for (var i = 0; i < obj.length; i++) {
                    if (obj[i].RawMaterialID == rawMaterialID) {
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
        mini.showTips({ content: "请选择原材料", state: "danger" });
        return;
    }

    var obj = {};
    obj.RawMaterialID = row.RawMaterialID;
    obj.Code = row.Code;
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

