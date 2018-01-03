var datagrid;
$.page_load = function () {
    datagrid = mini.get("datagrid");
    datagrid.load();
};

//加载列表
function datagrid_BeforeLoad(e) {
    e.cancel = true;
    loadUserList(e.params);       //e.params 固定写法
};

//查询
function search() {
    datagrid.load();
};

function loadUserList(pagerParams) {
    datagrid.loading();
    $.srv("Base", "GetUserList", {
        data: { loginName: mini.get("keyword").getValue(), pagerParams: pagerParams },
        success: function (result) {
            datagrid.set({
                pageIndex: result.Data.PageIndex
                    , pageSize: result.Data.PageSize
                    , totalCount: result.Data.TotalCount
                , data: result.Data.Data
            });

            //if (result.Data.length > 0) {
            //    datagrid.setSelected(dgl.data[0]);
            //};
        }
    });
};

//新增
function onAdd() {
    var newRow = { name: "New Row" };
    datagrid.addRow(newRow, 0);
    datagrid.beginEditCell(newRow, "UserID");
};

//保存
function onSave() {
    //获取所有编辑过的行
    var data = datagrid.getChanges();
    if (data.length == 0) return;

    $.srv("Base", "SaveUser", {
        data: { models: data },
        success: function (result) {
            mini.showTips({ content: "操作成功", state: "success" });
            search();
        }
    });
};

function onRemove() {
    var rows = getMUserIDList();
    if (!rows) {
        mini.showTips({ content: "请选择要删除的内容", state: "danger" });
        return;
    };
    mini.confirm("确定删除？", "确定？",
        function (action) {
            if (action != "ok") return;
            $.srv("Base", "DelUserItem", {
                data: { userID: rows },
                success: function (result) {
                    mini.showTips({ content: "删除成功", state: "success" });
                    search();
                }
            });
        });
};

function onPasswordInit() {
    var rows = getMUserIDList();
    if (!rows) {
        mini.showTips({ content: "请选择需要初始化密码的用户", state: "danger" });
        return;
    };
    mini.confirm("确定要初始化密码吗？", "确定？",
        function (action) {
            if (action != "ok") return;
            $.srv("Base", "UserPasswordInit", {
                data: { userID: rows },
                success: function (result) {
                    mini.showTips({ content: "操作成功", state: "success" });
                    search();
                }
            });
        });
};

//分配角色
function openSetRole() {
    var row = datagrid.getSelected();
    if (!row) {
        mini.showTips({ content: "请选择用户", state: "warning" });
        return;
    }

    //打开窗口
    mini.open({
        url: "Base/UserRole.html?multiSelect=1&userid=" + row.UserID
        , title: "配置用户角色"
        , width: 500
        , height: 480
        , ondestroy: function (action) {
            if (action != "ok") return;

            mini.showTips({ content: "用户角色配置成功", state: "success" });
        }
    });
};

//分配角色
function setRoleCallback(entityList) {
    var mUserIDList = getMUserIDList();
    if (mUserIDList) {
        var insertMRoleCodeList = [];
        var deleteMRoleCodeList = [];
        if (entityList != null && entityList.length > 0) {
            for (var i = 0; i < entityList.length; i++) {
                entityList[i].isChecked = entityList[i].checked ? 1 : 0;
                if (entityList[i].isChecked == 1) {
                    insertMRoleCodeList.push(entityList[i].RoleID);
                } else {
                    deleteMRoleCodeList.push(entityList[i].RoleID);
                };
            };
        };
        //分配角色
        $.srv("Base", "SetRole", {
            data: {
                mUserIDList: mUserIDList
                , insertMRoleIDList: insertMRoleCodeList
                , deleteMRoleIDList: deleteMRoleCodeList
            },
            success: function (result) {
                mini.showTips({ content: "分配成功", state: "success" });
                //search();
            }
        });
    };
};

//获得选中项编码集合
function getMUserIDList() {
    var mUserID = undefined;
    //获取选中项对象集合
    var entityList = datagrid.getSelecteds();
    //判断对象集合有值
    if (entityList && entityList.length > 0) {
        mUserID = [];
        for (var i = 0; i < entityList.length; i++) {
            mUserID.push(entityList[i].UserID);
        };
    };
    return mUserID;
};
