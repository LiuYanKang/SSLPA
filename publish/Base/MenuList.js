
var grid = null;
var params = fw.fwUrl.FWUrlHelper.getParams();
//加载页面
$.page_load = function () {
    grid = mini.get("treegrid1");
    //判断选择类型有值
    if (fw.fwObject.FWObjectHelper.hasValue(params.selectType)) {
        //判断是单选
        if (params.selectType == fw.fwData.FWSelectType.Single) {
           grid.setMultiSelect(false);
        } else {
           grid.setMultiSelect(true);
        };
        //判断选择回调有值
        if (fw.fwObject.FWObjectHelper.hasValue(params.selectCallback)) {
            //显示选择按钮
            $("#select").show();
        };
        //判断选择清除有值
        if (fw.fwObject.FWObjectHelper.hasValue(params.selectClearCallback)) {
            //显示清空按钮
            $("#selectClear").show();
        };
        //隐藏功能按钮
        $("#functionList").hide();
    };
    grid.load();
    search();
};

function search() {
    treegrid1_Load();
};

//单元格渲染事件
function treegrid1_Renderer(e) {
    var html = "";
    switch (e.field) {
        case "Name":
            html = "<a href=\"javascript:openInfo('query','" + e.record.MenuCode + "')\" style=\"color:blue;\">" + e.value + "</a>";
            break;
        default:
            break;
    };
    return html;
};

//加载数据
function treegrid1_Load() {
    grid.loading();
    $.srv("Base", "QueryMenuItems", {
        //data: {  },
        success: function (result) {
            grid.loadList(result.Data);
        }
    });
};

function Del(e) {
    var row = grid.getSelected();
    if (!row) {
        mini.showTips({ content: "请选择要删除的对象", state: "success" });
    }
    mini.confirm("确定删除 <i class='icon-quote-left'></i><span class='red'>" + row.Name + "</span> <i class='icon-quote-right'></i> ？", "确定？",
        function (action) {
            if (action != "ok") return;
            $.srv("Base", "MenuDel", {
                data: { id: row.MenuCode },
                success: function (result) {
                    if (result.Data) {
                        mini.showTips({ content: "删除成功", state: "success" });
                        search();
                    } else {
                        mini.showTips({ content: "删除失败", state: "danger" });
                    }
                }
            });
        });
}

   
//打开信息窗口
function openInfo(action, PCode, MenuCode) {
    var windon = {};
    if (action == "insert") {
        windon.url = bootPATH + "Base/MenuInfo.html?PCode=" + PCode + "&action=" + action + "";
        windon.width = "600";
        windon.height = "300";
    };
    if (action == "update") {
        var row = grid.getSelected();
        if (row) {
            windon.url = bootPATH + "Base/MenuInfo.html?PCode=" + row.MenuCode + "&MenuCode=" + row.MenuCode + "&action=" + action + "";
            windon.width = "600";
            windon.height = "300";
        } else {
            mini.showTips({ content: "请选择根节点", state: "success" });
            return;
        };
    }
    if (action == "query") {
        var row = grid.getSelected();
        if (row) {
            windon.url = bootPATH + "Base/MenuInfo.html?MenuCode=" + row.MenuCode + "&action=" + action + "";
            windon.width = "600";
            windon.height = "300";
        } else {
            mini.showTips({ content: "请选择要操作的对象", state: "success" });
            return;
        };
    }
    mini.open({
        title: "新增菜单",
        url: windon.url,
        width: windon.width,
        height: windon.height,
        ondestroy: function (action) {
            //if (action != "save") return;
            search();
        }
    });
};

//选中行前事件
function treegrid1_BeforeSelect(e) {
    //被排除的行不能选中
    if (fw.fwObject.FWObjectHelper.hasValue(params.exceptMMenuCode) && fw.fwObject.FWObjectHelper.hasValue(exceptNodes)) {
        if (exceptNodes.indexOf(e.record) > -1) {
            e.cancel = true;
        };
    };
};


var lastSelectedNode = null;
//行选择改变时
function datagrid1_SelectionChanged(e) {
    lastSelectedNode = e.selected;
    var childControls = mini.getChildControls($("#functionList")[0]);
    for (var i = 0; i < childControls.length; i++) {
        var isEnabled = true;
        if (fw.fwObject.FWObjectHelper.hasValue(childControls[i].minSelectedCount)) {
            if (isEnabled && childControls[i].minSelectedCount <= e.selecteds.length) {
                isEnabled = true;
            } else {
                isEnabled = false;
            };
        };
        if (fw.fwObject.FWObjectHelper.hasValue(childControls[i].maxSelectedCount)) {
            if (isEnabled && e.selecteds.length <= childControls[i].minSelectedCount) {
                isEnabled = true;
            } else {
                isEnabled = false;
            };
        };
        childControls[i].set({ enabled: isEnabled });
    };
    if (e.selected) {
        grid.lastSelectedRowIndex = grid.indexOf(e.selected);
    };
};

//选择选中项(提供给父页面调用)
function select() {
    var params = fw.fwUrl.FWUrlHelper.getParams();
    //判断选择类型以及选择回调方法有值
    if (fw.fwObject.FWObjectHelper.hasValue(params.selectType) && fw.fwObject.FWObjectHelper.hasValue(params.selectCallback)) {
        //判断是单选
        if (params.selectType == fw.fwData.FWSelectType.Single) {
            //获取选中项对象
            var entity = getSelectedEntity();
            //判断选中项对象有值
            if (fw.fwObject.FWObjectHelper.hasValue(entity)) {
                //调用回调方法
                fw.callFunction(fw.openWindow(), params.selectCallback, [entity]);
                //关闭窗口
                fw.closeWindow();
            };
        } else if (params.selectType == fw.fwData.FWSelectType.Multi) {
            //获取选中项对象集合
            var entityList = getSelectedEntityList();
            //判断选中项对象集合有值
            if (fw.fwObject.FWObjectHelper.hasValue(entityList)) {
                //调用回调方法
                fw.callFunction(fw.openWindow(), params.selectCallback, [entityList]);
                //关闭窗口
                fw.closeWindow();
            };
        } else if (params.selectType == fw.fwData.FWSelectType.All) {
            var entityList = grid.findRows();
            for (var i = 0; i < entityList.length; i++) {
                if (grid.isSelected(entityList[i])) {
                    entityList[i].checked = true;
                };
            };
            //调用回调方法
            fw.callFunction(fw.openWindow(), params.selectCallback, [entityList]);
            //关闭窗口
            fw.closeWindow();
        };
    };
};

function selectClear() {
    //判断选择类型以及选择回调方法有值
    if (fw.fwObject.FWObjectHelper.hasValue(params.selectType) && fw.fwObject.FWObjectHelper.hasValue(params.selectClearCallback)) {
        //调用选择清除回调方法
        fw.callFunction(fw.openWindow(), params.selectClearCallback, []);
        //关闭窗口
        fw.closeWindow();
    };
};


//获取选中项对象
function getSelectedEntity() {
    //获取选中项对象
    var entity = $.page.idM.treegrid1.getSelected();
    //判断对象没有值
    if (!fw.fwObject.FWObjectHelper.hasValue(entity)) {
        mini.alert("请选择一项！");
        entity = undefined;
    };
    return entity;
};

//获取选中项对象集合
function getSelectedEntityList() {
    //获取选中项对象集合
    var entityList = $.page.idM.treegrid1.getSelecteds();
    //判断对象集合没有值
    if (!fw.fwObject.FWObjectHelper.hasValue(entityList) || entityList.length < 1) {
        mini.alert("请选择需要操作项！");
        entityList = undefined;
    };
    return entityList;
};

//获得选中项编码
function getMMenuCode() {
    var mMenuCode = undefined;
    //获取选中项对象
    var entity = getSelectedEntity();
    //判断对象有值
    if (entity && fw.fwObject.FWObjectHelper.hasValue(entity)) {
        mMenuCode = entity.MenuCode;
    };
    return mMenuCode;
};

//获得选中项编码集合
function getMMenuCodeList() {
    var mMenuCodeList = undefined;
    //获取选中项对象集合
    var entityList = getSelectedEntityList();
    //判断对象集合有值
    if (fw.fwObject.FWObjectHelper.hasValue(entityList) && entityList.length > 0) {
        mMenuCodeList = [];
        for (var i = 0; i < entityList.length; i++) {
            mMenuCodeList.push(entityList[i].MenuCode);
        };
    };
    return mMenuCodeList;
};
