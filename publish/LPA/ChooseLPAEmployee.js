
var datagrid;
var multiSelect, selectedItems;

$.page_load = function () {

    selectedItems = [];
    datagrid = mini.get("datagrid");
    search();
}


//加载列表
function datagrid_BeforeLoad(e) {
    e.cancel = true;
    loadChooseLPAEmployee(e.params);
};


function loadChooseLPAEmployee(pagerParams) {
    datagrid.loading();
    $.srv("LPA", "EmployeeList", {
        data: { param: pagerParams },
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

    datagrid.load({ KeyWord: key, Status: null });
}



//表格渲染事件完毕
function onGridUpdate(e) {
    var source = e.source.data;
    var selected = $(selectedItems);
    var curSelected = [];
    for (var i in source) {
        selected.each(function () {
            if (this.EmpID == source[i].EmpID) {
                curSelected.push(source[i]);
                return false;
            }
        });
    }
    if (curSelected.length)
        e.sender.selects(curSelected);
}


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
            for (var li in selectedItems) {
                if (selectedItems[li].EmpID == e.records[i].EmpID) {
                    hasValue = true;
                    break;
                }
            }
            if (!hasValue)
                selectedItems.push(e.records[i]);
        }
    } else {
        for (var i = 0; i < e.records.length; i++) {
            for (var li in selectedItems) {
                if (selectedItems[li].EmpID == e.records[i].EmpID) {
                    selectedItems.removeAt(li);
                    break;
                }
            }
        }
    }
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


