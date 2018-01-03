var datagrid;
var form;
var machineID;
var pdCode;
var sPDCode;
$.page_load = function () {

    datagrid = mini.get("datagrid");
    form = new mini.Form("form1");
    machineID = $.request("machineID");
    pdCode = $.request("pdCode");
    sPDCode = mini.get("sPDCode");
    // 产品部门
    $.srv("BaseInfo", "ProDept", {
        success: function (data) {
            sPDCode.set({ data: data.Data });                        
        }
    });
    sPDCode.setValue(pdCode);
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
    var proType = mini.get("sPDCode").getValue();
    datagrid.load({ Keyword: key, ProductType: proType });
};

function loadSupplierList(pagerParams) {
    datagrid.loading();
    $.srv("BaseInfo", "MachineQuery", {
        data: { pagerParams: pagerParams },
        success: function (result) {
            datagrid.set({
                pageIndex: result.Data.PageIndex
                    , pageSize: result.Data.PageSize
                    , totalCount: result.Data.TotalCount
                , data: result.Data.Data
            });
            var obj = result.Data.Data;
            if (machineID) {
                for (var i = 0; i < obj.length; i++) {
                    if (obj[i].MachineID == machineID) {
                        datagrid.setSelected(obj[i]);
                    }
                }
            }
        }
    });
};

function getData() {
    var row = datagrid.getSelecteds();
    if (!row) {
        mini.showTips({ content: "请选择设备", state: "danger" });
        return;
    }

    //var obj = {};
    //obj.MachineID = row.MachineID;
    //obj.Code = row.Code;
    //obj.Name = row.Name;
    //obj.WorkProcessName = row.WorkProcessName;
    return row;
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

