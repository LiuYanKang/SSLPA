
var datagrid, cboProblemRegion, cboProblemType, cboState, txtProgress;

$.page_load = function () {
    datagrid = mini.get("datagrid");
    cboProblemRegion = mini.get("cboProblemRegion");
    cboProblemType = mini.get("cboProblemType");
    cboState = mini.get("cboState");

    //$.dic("1025", function (e) {        //问题区域
    //    cboProblemRegion.set({ data: e });
    //})
    $.srv("LPA", "AuditAreaGet", {
        success: function (data) {
            cboProblemRegion.set({ data: data.Data });
        }
    });

    $.dic("1026", function (e) {        //问题类型
        cboProblemType.set({ data: e });
    })

    $.dic("1027", function (e) {        //问题状态
        cboState.set({ data: e });
    })

    txtProgress = mini.get("txtProgress");
    txtProgress.set({ data: [{ Name: "0%", Code: 0 }, { Name: "25%", Code: 25 }, { Name: "50%", Code: 50 }, { Name: "75%", Code: 75 }, { Name: "已完成", Code: 100 }, { Name: "所有未完成", Code: -1 }] });
    mini.get("btnUpdate").setVisible(false);
    search();
}


function datagrid_BeforeLoad(e) {
    e.cancel = true;
    loadProblemList(e.params);
}


function loadProblemList(e) {
    datagrid.loading();

    $.srv("LPA", "ProblemList", {
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

function searchKey() {
    var key = mini.get("keyword").getValue();
    var progress = txtProgress.getValue() == "" ? null : txtProgress.getValue();
    var problemRegionState = cboProblemRegion.getValue() == "" ? null : cboProblemRegion.getValue();
    var problemTypeState = cboProblemType.getValue() == "" ? null : cboProblemType.getValue();
    var state = cboState.getValue() == "" ? null : cboState.getValue();

    var submitBeginDate = mini.get("txtSubmitBeginDate").getValue();
    submitBeginDate = submitBeginDate == "" ? submitBeginDate = null : $.jsDate2WcfDate(submitBeginDate);
    var submitEndDate = mini.get("txtSubmitEndDate").getValue();
    submitEndDate = submitEndDate == "" ? submitEndDate = null : $.jsDate2WcfDate(submitEndDate);

    return { Keyword: key, ProblemRegionState: problemRegionState, ProblemTypeState: problemTypeState, State: state, SubmitBeginDate: submitBeginDate, SubmitEndDate: submitEndDate, Progress: progress };
}


//查询
function search() {
    datagrid.load(searchKey());
}



//编辑
function ProblemEdit() {
    var row = datagrid.getSelected();
    if (!row) {
        mini.showTips({ content: "请选择要编辑的问题记录", state: "danger" });
        return;
    }

    mini.open({
        title: "编辑问题记录",
        url: bootPATH + "LPA/ProblemInfo.html?probID=" + row.ProbID,
        width: 710,
        height: 610,
        ondestroy: function (aciton) {
            if (aciton != "save") return;
            mini.showTips({ content: "修改成功", state: "success" });
            datagrid.reload();
        }
    });
};


//删除
function ProblemRemove() {
    var row = datagrid.getSelected();
    if (!row) {
        mini.showTips({ content: "请选择要删除的问题记录", state: "danger" });
        return;
    };
    mini.confirm("确定删除 ？", "确定？",
        function (action) {
            if (action != "ok") return;
            $.srv("LPA", "ProblemDel", {
                data: { id: row.ProbID },
                success: function (result) {
                    if (result.Data) {
                        mini.showTips({ content: "删除成功", state: "success" });
                        datagrid.reload();
                    }
                }
            });
        });
};


//更新问题
function ProblemUpdate() {
    var row = datagrid.getSelected();
    if (!row) {
        mini.showTips({ content: "请选择要更新的问题记录", state: "danger" });
        return;
    }

    mini.open({
        title: "更新问题记录",
        url: bootPATH + "LPA/ProblemInfoUpdate.html?probID=" + row.ProbID,
        width: 710,
        height: 610,
        ondestroy: function (aciton) {
            if (aciton != "save") return;
            mini.showTips({ content: "修改成功", state: "success" });
            datagrid.reload();
        }
    });
};

function onProgressRender(e) {
    return "<div class='progressbar'>"
                + "<div class='progressbar-percent percent" + e.value + "' style='width:" + e.value + "%;'></div>"
                + "<div class='progressbar-label'>" + e.value + "%</div>"
            + "</div>";
}

function onProblemDescRender(e) {
    return "<a href='javascript:onOpen(" + e.row.ProbID + ")'>" + e.row.ProblemDesc + "</a>"
}

function onOpen(id) {
    mini.open({
        title: "问题记录详情",
        url: bootPATH + "LPA/ProblemView.html?probID=" + id,
        width: 655,
        height: 530,
        ondestroy: function (action) {
            if (action != "save") return;
        }
    });
}


//导出
function ExportExcel() {
    mini.mask({
        el: document.body,
        cls: 'mini-mask-loading',
        html: '数据导出中……'
    });

    $.srv("LPA", "ProblemExport", {
        data: { param: searchKey() },
        success: function (result) {
            mini.unmask(document.body);
            document.location = bootPATH + "temp/" + result.Data;
        }
        , exception: function (data) {
            mini.unmask(document.body);
        }
    });
}

function ProblemFinish() {
    var row = datagrid.getSelected();
    if (!row) {
        mini.showTips({ content: "请选择要关闭的问题", state: "danger" });
        return;
    }

    mini.open({
        title: "关闭问题",
        url: bootPATH + "LPA/ProblemFinish.html?probID=" + row.ProbID,
        width: 710,
        height: 580,
        ondestroy: function (aciton) {
            if (aciton != "save") return;
            mini.showTips({ content: "操作成功", state: "success" });
            datagrid.reload();
        }
    });
}


function datagrid_selectionchanged() {
    var row = datagrid.getSelected();
    if (row.Progress==100) {  // 库位有货
        mini.get("btnClose").setVisible(false);
    } else {
        mini.get("btnClose").setVisible(true);
    }
    if (row.UserId == row.Responsible) { // 库位有货
        mini.get("btnUpdate").setVisible(true);
    } else {
        mini.get("btnUpdate").setVisible(false);
    }
}