
var datagrid;

$.page_load = function () {
    datagrid = mini.get("datagrid");
    search();
}



function datagrid_BeforeLoad(e) {
    e.cancel = true;
    loadWorkingShiftList(e.params);
}


function loadWorkingShiftList(e) {
    datagrid.loading();

    $.srv("BaseInfo", "WorkingShiftQuery", {
        data: { pagerParams: e },
        success: function (result) {
            datagrid.set({
                data: result.Data
            });
        }
    });
}


//查询
function search() {
    datagrid.load();
}


//新增
function onAdd() {
    mini.open({
        title: "新增班次",
        url: bootPATH + "BaseInfo/WorkingShiftInfo.html",
        width: 585,
        height: 310,
        ondestroy: function (aciton) {
            if (aciton != "save") return;
            mini.showTips({ content: "新增成功", state: "success" });
            search();
        }
    });
};

//编辑
function onEdit() {
    var row = datagrid.getSelected();
    if (!row) {
        mini.showTips({ content: "请选择要编辑的班次", state: "danger" });
        return;
    }

    mini.open({
        title: "编辑班次",
        url: bootPATH + "BaseInfo/WorkingShiftInfo.html?id=" + row.ShiftID,
        width: 585,
        height: 310,
        ondestroy: function (aciton) {
            if (aciton != "save") return;
            mini.showTips({ content: "修改成功", state: "success" });
            search();
        }
    });
};

//删除
function onRemove() {
    var row = datagrid.getSelected();
    if (!row) {
        mini.showTips({ content: "请选择要删除的班次", state: "danger" });
        return;
    };
    mini.confirm("确定删除 <i class='icon-quote-left'></i> <span class='red'>" + row.Name + "</span> <i class='icon-quote-right'></i> ？", "确定？",
        function (action) {
            if (action != "ok") return;
            $.srv("BaseInfo", "WorkingShiftDel", {
                data: { id: row.ShiftID },
                success: function (result) {
                    if (result.Data) {
                        mini.showTips({ content: "删除成功", state: "success" });
                        search();
                    }
                }
            });
        });
};

