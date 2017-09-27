
var datagrid, cboIsResponsible;

$.page_load = function () {
    datagrid = mini.get("datagrid");
    cboIsResponsible = mini.get("cboIsResponsible");

    search();
}


function datagrid_BeforeLoad(e) {
    e.cancel = true;
    loadEmployeeList(e.params);
}


function loadEmployeeList(e) {
    datagrid.loading();

    $.srv("LPA", "EmployeeList", {
        data: { param: e },
        success: function (result) {
            datagrid.set({
                pageIndex: result.Data.PageIndex
               , pageSize: result.Data.PageSize
               , totalCount: result.Data.TotalCount
                , data: result.Data.Data
            });
        }
    });
}


//查询
function search() {
    var key = mini.get("keyword").getValue();
    //var status = cboIsResponsible.getValue();
    //status = status == "" ? null : status;
    datagrid.load({ Keyword: key });
}


//新增
function EmployeeAdd() {
    mini.open({
        title: "新增LPA员工",
        url: bootPATH + "LPA/EmployeeInfo.html",
        width: 650,
        height: 450,
        ondestroy: function (aciton) {
            if (aciton != "save") return;
            mini.showTips({ content: "新增成功", state: "success" });
            search();
        }
    });
};

//编辑
function EmployeeEdit() {
    var row = datagrid.getSelected();
    if (!row) {
        mini.showTips({ content: "请选择要编辑的LPA员工", state: "danger" });
        return;
    }

    mini.open({
        title: "编辑LPA员工",
        url: bootPATH + "LPA/EmployeeInfo.html?empID=" + row.EmpID,
        width: 650,
        height: 450,
        ondestroy: function (aciton) {
            if (aciton != "save") return;
            mini.showTips({ content: "修改成功", state: "success" });
            datagrid.reload();
        }
    });
};
//删除
function EmployeeRemove() {
    var row = datagrid.getSelected();
    if (!row) {
        mini.showTips({ content: "请选择要删除的LPA员工", state: "danger" });
        return;
    };
    mini.confirm("确定删除 ？", "确定？",
        function (action) {
            if (action != "ok") return;
            $.srv("LPA", "EmployeeDel", {
                data: { id: row.EmpID },
                success: function (result) {
                    if (result.Data) {
                        mini.showTips({ content: "删除成功", state: "success" });
                        datagrid.reload();
                    }
                }
            });
        });
};


function onIsResponsible(e) {
    if (e.value) {
        return "√";
    }
}