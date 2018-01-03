var tree = null;
var key = {};
var treeFunc, treeMenu;


//加载页面
$.page_load = function () {
    tree = mini.get("tree");
    treeFunc = mini.get('treeFunc');
    treeMenu = mini.get('treeMenu');
    queryFuncAll();
    queryTreeMenu();
    loadTree();
};

//加载部门
function loadTree() {
    $.srv("Base", "MDepartmentAll", {
        data: { id: id },
        success: function (result) {
            tree.loadList(result.Data);
        }
    });

}


//查询
function search() {
    key = mini.get("key").getValue();

};



//选择左侧管理员显示权限菜单
function onTreeNodeClick(e) {
    if (!tree.getSelectedNode()) return;
    treeFunc.uncheckAllNodes();
    treeMenu.uncheckAllNodes();
    $.srv("Base", "GetFuncByDepartmentId", {
        data: { deptId: tree.getSelectedNode().DeptID },
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
    $.srv("Base", "GetMenuByDepartmentId", {
        data: { deptId: tree.getSelectedNode().DeptID },
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



//加载右边tree菜单分配
function queryTreeMenu() {
    $.srv("Base", "QueryMenuAll", {
        success: function (reslut) {
            treeMenu.loadList(reslut.Data);

        }
    });

}


function onFuncSave() {
    var row = tree.getSelectedNode();
    if (!row) return;

    var funcNodes = treeFunc.getCheckedNodes(true);
    var selectedFunc = [];
    $(funcNodes).each(function () {
        selectedFunc.push(this.FuncCode);
    });
    $.srv("Base", "SaveDepartmentFunc", {
        data: { departmentId: row.DeptID, funcList: selectedFunc },
        success: function (result) {
            if (result.Data) {
                mini.showTips({ content: "保存成功", state: "success" });
            } else {
                mini.showTips({ content: "保存失败", state: "danger" });
            }
        }
    });
}

function onMenuSave() {
    var row = tree.getSelectedNode();
    if (!row) return;

    var menuNodes = treeMenu.getCheckedNodes(true);
    var selectmenuNodes = [];
    $(menuNodes).each(function () {
        selectmenuNodes.push(this.MenuCode);
    });
    $.srv("Base", "SaveDepartmentMenu", {
        data: { departmentId: row.DeptID, menuList: selectmenuNodes },
        success: function (result) {
            if (result.Data) {
                mini.showTips({ content: "保存成功", state: "success" });
            } else {
                mini.showTips({ content: "保存失败", state: "danger" });
            }
        }

    });
}