
var dgleft, dgRight;
var materialtype, materialID, materialCode;
var procID, procName;


$.page_load = function () {
    dgleft = mini.get("dgleft");
    dgRight = mini.get("dgRight");

    dglSearch();
}


function dgl_BeforeLoad(e) {
    e.cancel = true;
    loadLeftList(e.params);
}


function loadLeftList(e) {                        //加载左边的DataGrid 菜单
    dgleft.loading();

    $.srv("Base", "EmployeeQuery", {
        data: { pagerParams: e },
        success: function (result) {
            dgleft.set({
                pageIndex: result.Data.PageIndex
                , pageSize: result.Data.PageSize
                , totalCount: result.Data.TotalCount
                , data: result.Data.Data
            });

            if (result.Data.Data.length > 0) {
                dgleft.setSelected(dgleft.data[0]);       //默认选择左边DataGird 第一行
            }
        }
    });
}

//查询
function dglSearch() {
    dgleft.load(searchKey());
}


function searchKey() {
    var key = mini.get("leftKeyword").getValue();
    return {
        KeyWord: key,
        Status: '0'
    }
}


function dgleft_selectionchanged(e) {
    var row = e.selected;

    dgRight.load({ empid: row.EmpID });
}


//加载列表
function dgRight_BeforeLoad(e) {
    e.cancel = true;
    loadWorkProcessList(e.params);
};


function loadWorkProcessList(params) {
    dgRight.loading();
    $.srv("BaseInfo", "EmpSkillQuery", {
        data: { empid: params.empid },
        success: function (result) {
            dgRight.set({
                data: result.Data
            })
        }
    });
};





//新增
function onAdd() {
    var row = dgleft.getSelected();
    if (!row) {
        mini.showTips({ content: "请选择职员", state: "danger" });
        return;
    }
    mini.open({
        title: "新增",
        url: bootPATH + "BaseInfo/EmpSkillInfo.html",
        width: 380,
        height: 330,
        onload: function () {
            var iframe = this.getIFrameEl();
            iframe.contentWindow.setData({ EmpID: row.EmpID, EmpName: row.Name });

        },
        ondestroy: function (aciton) {
            if (aciton != "save") return;
            mini.showTips({ content: "新增成功", state: "success" });
            dgRight.reload();
        }
    });
};


//编辑
function onEdit() {
    var row = dgRight.getSelected();
    if (!row) {
        mini.showTips({ content: "请选择要编辑的项目", state: "danger" });
        return;
    }
    var rowEmp = dgleft.getSelected();
    row.EmpName = rowEmp.Name;

    mini.open({
        title: "编辑",
        url: bootPATH + "BaseInfo/EmpSkillInfo.html",
        width: 380,
        height: 330,
        onload: function () {
            var iframe = this.getIFrameEl();
            iframe.contentWindow.setData(row);

        },
        ondestroy: function (aciton) {
            if (aciton != "save") return;
            mini.showTips({ content: "修改成功", state: "success" });
            dgRight.reload();
        }
    });
};


//删除
function onRemove() {
    var row = dgRight.getSelected();
    if (!row) {
        mini.showTips({ content: "请选择要删除的项", state: "danger" });
        return;
    };
    mini.confirm("确定删除 <i class='icon-quote-left'></i> <span class='red'>" + row.MachineName + "</span> <i class='icon-quote-right'></i> ？", "确定？",
        function (action) {
            if (action != "ok") return;
            $.srv("BaseInfo", "EmpSkillDel", {
                data: { empid: row.EmpID, machineid: row.MachineID },
                success: function (result) {
                    if (result.Data) {
                        mini.showTips({ content: "删除成功", state: "success" });
                        dgRight.reload();
                    }
                }
            });
        });
};

function onLevelRender(e) {
    return '<span style="display:inline-block;width:15px;height:15px;marin-right:10px;background:#' + e.row.LevelRemark + '"></span> ' + e.value;
}