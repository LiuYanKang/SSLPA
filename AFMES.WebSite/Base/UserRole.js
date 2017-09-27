
var grid;
var selectedItems;
var multiSelect;

$(function () {
    mini.regESC();
    selectedItems = [];

    mini.parse();
    grid = mini.get("datagrid1");

    mini.get('txtKeyword').focus();

    getUserRole();
});


function onGridUpdate(e) {
    var source = e.source.data;
    var selected = $(selectedItems);
    var curSelected = [];
    for (var i = 0; i < source.length; i++) {
        selected.each(function () {
            if (this == source[i].RoleID) {
                curSelected.push(source[i]);
                return false;
            }
        });
    }
    if (curSelected.length)
        e.sender.selects(curSelected);
}


function datagrid1_BeforeLoad(e) {
    e.cancel = true;
    datagrid1_Load(e.params);
};


function datagrid1_Load(e) {
    grid.loading();

    $.srv("Base", "QueryRoleItems", {
        data: { param: e },
        success: function (result) {
            grid.set({
                pageIndex: result.Data.PageIndex
              , pageSize: result.Data.PageSize
              , totalCount: result.Data.TotalCount
              , data: result.Data.Data
            });
        }
    });
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

function search() {
    var key = mini.get("txtKeyword").getValue();
    grid.load({ Keyword: key });
};
//输入框的回车的方法 
function onKeyEnter(e) {
    search();
};
//////////////////////////////////

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
                if (selectedItems[li] == e.records[i].RoleID) {
                    hasValue = true;
                    break;
                }
            }
            if (!hasValue)
                selectedItems.push(e.records[i].RoleID);
        }
    } else {
        for (var i = 0; i < e.records.length; i++) {
            for (var li = 0; li < selectedItems.length; li++) {
                if (selectedItems[li] == e.records[i].RoleID) {
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
    var userid = $.request('userid');
    $.srv("Base", "SaveUserRole", {
        data: { userID: userid, roleList: selectedItems }
        , success: function (result) {
            CloseWindow("ok");
        }
    });



};
function onCancel() {
    CloseWindow("cancel");
};

function getUserRole() {
    var userid = $.request('userid');
    $.srv("Base", "GetUserRole", {
        data: { userID: userid }
        , success: function (result) {
            selectedItems = result.Data;

            search();
        }
    });
}