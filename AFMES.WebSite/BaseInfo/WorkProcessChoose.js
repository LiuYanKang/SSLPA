var datagrid;
var form;
var procID;

$.page_load = function () {

    datagrid = mini.get("datagrid");
    form = new mini.Form("form1");
    procID = $.request("procID");

    search();
};

//加载列表
function datagrid_BeforeLoad(e) {
    e.cancel = true;
    loadWorkProcessList(e.params);
};

//查询
function search() {
    var key = mini.get("keyword").getValue();
    datagrid.load({ Keyword: key });
};

function loadWorkProcessList(pagerParams) {
    datagrid.loading();
    $.srv("BaseInfo", "WorkProcessList", {
        data: { pagerParams: pagerParams },
        success: function (result) {
            datagrid.set({
                pageIndex: result.Data.PageIndex
                    , pageSize: result.Data.PageSize
                    , totalCount: result.Data.TotalCount
                , data: result.Data.Data
            });
            var obj = result.Data.Data;
            if (procID) {
                for (var i = 0; i < obj.length; i++) {
                    if (obj[i].ProcID == procID) {
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
        mini.showTips({ content: "请选择工序", state: "danger" });
        return;
    }

    var obj = {};
    obj.ProcID = row.ProcID;
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
