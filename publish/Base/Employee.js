
var cboEmployeeStatus;
var datagrid;
var tree;
var statusRecovery, statusSet;

$.page_load = function () {
    datagrid = mini.get("datagrid");
    statusRecovery = mini.get("statusRecovery");    //重新入职
    statusSet = mini.get("statusSet");              //离职
    tree = mini.get("tree");

    cboEmployeeStatus = mini.get("cboEmployeeStatus");
    $.dic("1014", function (e) {
        cboEmployeeStatus.set({ data: e });         //绑定员工状态
    });
    loadTree();
    search();
}

//加载部门
function loadTree() {
    $.srv("Base", "MDepartmentAll", {
        data: { id: id },
        success: function (result) {
            tree.loadList(result.Data);
        }
    });

}

//部门节点右击
function onBeforeOpen(e) {
    var menu = e.sender;

    var node = tree.getSelectedNode();
    if (!node) {
        e.cancel = true;
        return;
    }
    if (node && node.text == "Base") {
        e.cancel = true;
        //阻止浏览器默认右键菜单
        e.htmlEvent.preventDefault();
        return;
    }

    var addItem = mini.getbyName("add", menu);
    var editItem = mini.getbyName("edit", menu);
    var removeItem = mini.getbyName("remove", menu);
    addItem.show();
    editItem.show();
    removeItem.show();

}

//部门节点单击
function onTreeNodeClick(e) {
    search();
}

//查询
function search() {
    var deptId = "";
    if (tree.getSelectedNode()) {
        deptId = tree.getSelectedNode().DeptID;
    }
    var key = mini.get("keyword").getValue();
    var EmployeeStatus = cboEmployeeStatus.getValue();
    EmployeeStatus = EmployeeStatus == "" ? null : EmployeeStatus;

    datagrid.load({ KeyWord: key, Status: EmployeeStatus, DeptID: deptId });
}


function onAddNode() {
    var node = tree.getSelectedNode();
    mini.open({
        title: "新增部门",
        url: bootPATH + "Base/Department.html?pid=" + node.DeptID + "&pname=" + escape(node.Name),
        width: 350,
        height: 350,
        ondestroy: function (aciton) {
            if (aciton != "save") return;
            mini.showTips({ content: "新增成功", state: "success" });
            loadTree();
            search();
        }
    });
}

function onEditNode() {
    var node = tree.getSelectedNode();
    mini.open({
        title: "编辑部门",
        url: bootPATH + "Base/Department.html?DeptID=" + node.DeptID,
        width: 350,
        height: 350,
        ondestroy: function (aciton) {
            if (aciton != "save") return;
            mini.showTips({ content: "修改成功", state: "success" });
            loadTree();
            search();
        }
    });
}

function onRemoveNode() {
    var node = tree.getSelectedNode();
    if (!node) return;

    $.srv("Base", "DepartmentCanDel", {
        data: { fulldeptId: node.FullDeptID },
        success: function (result) {
            if (result.Data == true) {

                mini.confirm("确定删除<i class='icon-quote-left'></i> <span class='red'>" + node.Name + "</span> <i class='icon-quote-right'></i> ？", "确定？",
                    function (action) {
                        if (action != "ok") return;

                        $.srv("Base", "DepartmentDel", {
                            data: { deptId: node.DeptID },
                            success: function (result) {
                                if (result.Data) {
                                    tree.removeNode(node);
                                    mini.showTips({ content: "删除成功", state: "success" });
                                    loadTree();

                                } else {
                                    mini.showTips({ content: "删除失败", state: "danger" });
                                }
                            }
                        });
                    }
                );

            } else {
                mini.showTips({ content: "该部门存在员工或者子部门无法删除", state: "danger" });
            }
        }
    });


}


//加载列表
function datagrid_BeforeLoad(e) {
    e.cancel = true;
    loadEmployeeList(e.params);
};


function loadEmployeeList(pagerParams) {
    datagrid.loading();
    $.srv("Base", "EmployeeQuery", {
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


function datagrid_selectionchanged() {
    var row = datagrid.getSelected();
    if (row.Status == "0") {                                    //在职
        mini.get("statusSet").setVisible(true);
        mini.get("statusRecovery").setVisible(false);

        if (row.LoginName == "" || row.LoginName == null) {
            mini.get("addLoginUser").setVisible(true);
        } else {
            mini.get("addLoginUser").setVisible(false);
        }
    }

    if (row.Status == "1") {
        mini.get("statusSet").setVisible(false);
        mini.get("statusRecovery").setVisible(true);
        mini.get("addLoginUser").setVisible(false);
    }

    if (row.LoginName == "" || row.LoginName == null) {
        mini.get("setRole").setVisible(false);
    } else {
        mini.get("setRole").setVisible(true);
    }
}

function onAdd() {                                          //新增
    mini.open({
        title: "新增职员",
        url: bootPATH + "Base/EmployeeInfo.html",
        width: 720,
        height: 500,
        ondestroy: function (aciton) {
            if (aciton != "save") return;
            mini.showTips({ content: "新增成功", state: "success" });
            search();
        }
    });
}

function onEdit() {                                         //编辑
    var row = datagrid.getSelected();
    if (!row) {
        mini.showTips({ content: "请选择编辑的职员", state: "danger" });
        return;
    }
    mini.open({
        title: "职员详情",
        url: bootPATH + "Base/EmployeeInfo.html?empID=" + row.EmpID,
        width: 720,
        height: 500,
        ondestroy: function (aciton) {
            if (aciton != "save") return;
            mini.showTips({ content: "修改成功", state: "success" });
            search();
        }
    });
}



//创建账号
function onAddLoginUser() {
    var row = datagrid.getSelected();
    if (!row) {
        mini.showTips({ content: "请选择要创建账号的职员", state: "danger" });
        return;
    }
    mini.open({
        title: "创建账号",
        url: bootPATH + "Base/LoginUser.html?empId=" + row.EmpID,
        width: 600,
        height: 170,
        ondestroy: function (aciton) {
            if (aciton != "save") return;
            mini.showTips({ content: "创建成功", state: "success" });
            search();
        }
    });
}


function SetStatus() {

    var row = datagrid.getSelected();
    var isDisabled = row.Status == "1";
    var info;
    if (row.Status == "1") {
        info = "重新入职";
    } else {
        info = "离职";
    }
    mini.confirm("确定<i class='icon-quote-left'></i> <span class='red'>" + row.Name + "</span><i class='icon-quote-right'></i>？" + info, "确定？",
    function (action) {
        if (action != "ok") return;

        $.srv("Base", "SetDisabledEmployee", {
            data: { id: row.EmpID, isDisabled: isDisabled },
            success: function (result) {
                if (result.Data) {
                    mini.showTips({ content: "操作成功", state: "success" });
                    search();
                } else {
                    mini.showTips({ content: "操作失败", state: "danger" });
                }
            }
        });
    });
}



function onColorRenderer(e) {
    if (e.row.Status == "0") {
        return "在职";
    } else {
        return "<a style='color:#ff0000;'>离职</a>";
    }
}


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

function onHeaderRenderer(e) {
    if (!e.row.EmpCode || e.row.EmpCode == "") {
        return '请补充工号';
    }

    var imgsrc = SS.config.empHeaderPath + e.row.EmpCode + '.jpg?t=' + new Date().getMilliseconds();
    var defaultImg = SS.config.empHeaderPath + 'default.jpg';

    return '<img src="' + imgsrc + '" style="width:100%" onerror="javascript:this.src=\'' + defaultImg + '\'" />'
}