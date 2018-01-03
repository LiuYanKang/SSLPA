var grid = null;
var key = {};
var treeFunc, treeMenu;


//加载页面
$.page_load = function () {
    grid = mini.get("datagrid1");
    treeFunc = mini.get('treeFunc');
    treeMenu = mini.get('treeMenu');
    grid.load();
    queryFuncAll();
    queryTreeMenu();
    ///search();
};


//加载列表
function datagrid1_BeforeLoad(e) {
    e.cancel = true;
    var pageIndex = e.data.pageIndex + 1;
    var pageSize = e.data.pageSize;
    //datagrid1_Load(pageIndex, pageSize);
    datagrid1_Load(e.params);
};

//查询
function search() {
    key = mini.get("key").getValue();
    grid.load({ Keyword: key });
    //datagrid1_Load(1);
};

//增加一行
function addRow() {
    var newRow = {};
    grid.addRow(newRow, 0);
    grid.beginEditCell(newRow, "Name");
};

//加载数据
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

//保存
function saveData() {
    var data = grid.getChanges();
    if (data.length == 0) return;
    $.srv("Base", "SaveRoleItem", {
        data: { models: data },
        success: function (result) {
            mini.showTips({ content: "操作成功", state: "success" });
            search();
        }
    });
};

//删除
function removeRow() {
    var rows = getRoleIDList();
    if (!rows) {
        mini.showTips({ content: "请选择要删除的内容", state: "danger" });
        return;
    };
    mini.confirm("确定删除？", "确定？",
        function (action) {
            if (action != "ok") return;
            $.srv("Base", "DelRoleItem", {
                data: { roleID: rows },
                success: function (result) {
                    mini.showTips({ content: "删除成功", state: "success" });
                    search();
                }
            });
        });
};

//分配菜单
function setMenu() {
    //获得选中项编码集合
    var roleIDList = getSelectedCodeList();
    //判断选中了项
    if (roleIDList) {
        var data = {
            selectType: fw.fwData.FWSelectType.All
            , selectCallback: "openSetMenuCallback"
            , roleIDList: fw.fwJson.FWJsonHelper.serializeObject(roleIDList)
        };
        //获得传入的参数字符串
        var params = fw.fwUrl.FWUrlHelper.param(data);
        //打开窗口
        mini.open({
            url: "Base/MenuList.html?" + params
            , title: "选择菜单"
            , width: 800
            , height: 560
            , onload: function () {
                //var iframe = this.getIFrameEl();
                //iframe.contentWindow;
            }
            , ondestroy: function (action) {
                //判断非（关闭和取消）窗口
                if (action != "close" && action != "cancel") {
                };
            }
        });
    };
};

//分配角色
function openSetMenuCallback(entityList) {
    var mRoleIDList = getSelectedCodeList();
    if (mRoleIDList) {
        var insertMenuCodeList = [];
        var deleteMenuCodeList = [];
        if (entityList != null && entityList.length > 0) {
            for (var i = 0; i < entityList.length; i++) {
                entityList[i].isChecked = entityList[i].checked ? 1 : 0;
                if (entityList[i].isChecked == 1) {
                    insertMenuCodeList.push(entityList[i].MenuCode);
                } else {
                    deleteMenuCodeList.push(entityList[i].MenuCode);
                };
            };
        };
        //分配菜单
        $.srv("Base", "SetMenu", {
            data: {
                mRoleIDList: mRoleIDList
                , insertMMenuIDList: insertMenuCodeList
                , deleteMMenuIDList: deleteMenuCodeList
            },
            success: function (result) {
                mini.showTips({ content: "分配成功", state: "success" });
                //search();
            }
        });
    };
};


//获得选中项编码集合
function getRoleIDList() {
    var roleIDList = undefined;
    //获取选中项对象集合
    var entityList = grid.getSelecteds();
    //判断对象集合有值
    if (entityList && entityList.length > 0) {
        roleIDList = [];
        for (var i = 0; i < entityList.length; i++) {
            roleIDList.push(entityList[i].RoleID);
        };
    };
    return roleIDList;
};


//加载右边tree菜单
function queryFuncAll() {
    $.srv("Base", "FuncAll", {
        success: function (result) {
            for (var i = 0; i < result.Data.length; i++) {
                result.Data[i].Name = result.Data[i].FuncCode + " - " + result.Data[i].Name;
            }
            treeFunc.loadList(result.Data);
        }
    });
}


function onFuncSave() {
    var noRow = grid.getSelected();
    if (!noRow) return;

    var funcNodes = treeFunc.getCheckedNodes(true);
    var selectedFunc = [];
    $(funcNodes).each(function () {
        selectedFunc.push(this.FuncCode);
    });
    $.srv("Base", "SaveRoleFunc", {
        data: { roleID: noRow.RoleID, funcList: selectedFunc },
        success: function (result) {
            if (result.Data) {
                mini.showTips({ content: "保存成功", state: "success" });
            } else {
                mini.showTips({ content: "保存失败", state: "danger" });
            }
        }
    });
}

//选择左侧管理员显示权限菜单
function dg_selectionchanged(e) {
    var row = grid.getSelected();
    var nodes = treeFunc.getSelectedNode();
    if (!row) return;
    treeFunc.uncheckAllNodes();
    treeMenu.uncheckAllNodes();
    $.srv("Base", "GetFuncByRole", {
        data: { id: row.RoleID },
        success: function (result) {
            var nodes = treeFunc.getList();//获取节点数组
            var checkedList = [];   //new 数组
            $(nodes).each(function () { //循环遍历如果有的话就添加进new 数组
                if ($.inArray(this.FuncCode, result.Data) < 0) return;
                checkedList.push(this);

            });
            treeFunc.checkNodes(checkedList);             //Check多选多个节点
        }
    });
    $.srv("Base", "GetMenuByRole", {
        data: { id: row.RoleID },
        success: function (result) {
            var nodes = treeMenu.getList(); //获取节点数组
            var checkedList = [];          //new 数组
            $(nodes).each(function () {    //循环遍历如果有的话就添加进new 数组
                if ($.inArray(this.MenuCode, result.Data) < 0) return;
                checkedList.push(this);

            });
            treeMenu.checkNodes(checkedList);             //Check多选多个节点
        }
    });
}

//加载右边tree菜单分配
function queryTreeMenu() {
    $.srv("Base", "QueryMenuAll", {
        success: function (reslut) {
            treeMenu.loadList(reslut.Data);

        }
    });

}

function onMenuSave() {
    var row = grid.getSelected();
    if (!row) return;

    var menuNodes = treeMenu.getCheckedNodes(true);
    var selectmenuNodes = [];
    $(menuNodes).each(function () {
        selectmenuNodes.push(this.MenuCode);
    });
    $.srv("Base", "SaveRoleMenu", {
        data: { roleID: row.RoleID, menuList: selectmenuNodes },
        success: function (result) {
            if (result.Data) {
                mini.showTips({ content: "保存成功", state: "success" });
            } else {
                mini.showTips({ content: "保存失败", state: "danger" });
            }
        }

    });
}