var datagrid;
var form;
var selectedItems;

$.page_load = function () {

    datagrid = mini.get("datagrid");

    selectedItems = [];
    form = new mini.Form("form1");
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
    $.srv("BaseInfo", "SemiProductQuery", {
        data: { pagerParams: pagerParams },
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


//获取所有选中的行
function GetSelecteds() {
    var rows = grid.getSelecteds();
    return rows;
};

//选中后点击确认  会将数据返回到上一个页面中
function getData() {
    return selectedItems;
};


function setData(data) {
    if (multiSelect == "0") {
        selectedItems.push(data);
    } else {
        for (var j = 0; j < data.length; j++) {
            selectedItems.push(data[j]);
        }
    }
}
//保存所有选中的数据
function onSelectoinChanged(e) {
    if (e.select) {
        if (multiSelect == "0") {
            selectedItems = [];
        }
        for (var i = 0; i < e.records.length; i++) {
            var hasValue = false;
            for (var li = 0; li < selectedItems.length; li++) {
                if (selectedItems[li].SemiProdID == e.records[i].SemiProdID) {
                    hasValue = true;
                    break;
                }
            }
            if (!hasValue)
                selectedItems.push({ SemiProdID: e.records[i].SemiProdID, Code: e.records[i].Code });
        }
    } else {
        for (var i = 0; i < e.records.length; i++) {
            for (var li = 0; li < selectedItems.length; li++) {
                if (selectedItems[li].SemiProdID == e.records[i].SemiProdID) {
                    selectedItems.removeAt(li);
                    break;
                }
            }
        }
    }
};

//表格渲染事件完毕
function onGridUpdate(e) {
    var source = e.source.data;
    var selected = $(selectedItems);
    var curSelected = [];
    for (var i in source) {
        selected.each(function () {
            if (this.SemiProdID == source[i].SemiProdID) {
                curSelected.push(source[i]);
                return false;
            }
        });
    }
    if (curSelected.length)
        e.sender.selects(curSelected);
}
