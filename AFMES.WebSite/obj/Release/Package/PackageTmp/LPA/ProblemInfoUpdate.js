var form, dgProbBefore, dgProbAfter;
var reqStr;
var txtResponsible, cboProblemRegion, cboMachine, cboProblemType;
var txtSubmitDate, txtPlanStartDate, txtPlanEndDate, txtActualEndDate, txtProgress;

var oldPlanEndDate;
var isAdd = false;

$.page_load = function () {

    form = new mini.Form("form1");
    dgProbBefore = mini.get("dgProbBefore");
    dgProbAfter = mini.get("dgProbAfter");
    txtSubmitDate = mini.get("txtSubmitDate");
    txtPlanStartDate = mini.get("txtPlanStartDate");
    txtPlanEndDate = mini.get("txtPlanEndDate");
    txtActualEndDate = mini.get("txtActualEndDate");
   
    txtResponsible = mini.get("txtResponsible");
    cboProblemRegion = mini.get("cboProblemRegion");
    cboMachine = mini.get("cboMachine");
    cboProblemType = mini.get("cboProblemType");
    txtProgress = mini.get("txtProgress");
    reqStr = $.request("probID");
    if (reqStr) {       //修改
        setData(reqStr);
    }

    //$.dic("1025", function (e) {        //问题区域
    //    cboProblemRegion.set({ data: e });
    //});

    $.srv("LPA", "AuditAreaGet", {
        success: function (data) {
            cboProblemRegion.set({ data: data.Data });
        }
    });

    $.dic("1026", function (e) {        //问题类型
        cboProblemType.set({ data: e });
    });

    // 设备清单
    $.srv("BaseInfo", "MachineAllS", {
        success: function (data) {
            cboMachine.set({ data: data.Data });
        }
    });

    txtProgress.set({ data: [{ Name: "0", Code: 0 }, { Name: "25", Code: 25 }, { Name: "50", Code: 50 }, { Name: "75", Code: 75 }] });
};

function onOk(e) {
    saveData();
}

function onCancel(e) {
    CloseWindow("cancel");
}

//保存
function saveData() {
    var obj = getData();
    var submitDate = obj.SubmitDate.format("yyyy-MM-dd");
    if (obj.PlanStartDate) {
        var planStartDate = obj.PlanStartDate.format("yyyy-MM-dd");
        if (submitDate > planStartDate) {
            mini.showTips({ content: "计划开始日期不能大于问题提交日期！", state: "danger" });
            return;
        }
    }
    form.validate();
    if (form.isValid() == false) return;

    obj.SubmitDate = $.jsDate2WcfDate(obj.SubmitDate);
    obj.PlanStartDate = $.jsDate2WcfDate(obj.PlanStartDate);
    obj.PlanEndDate = $.jsDate2WcfDate(obj.PlanEndDate);

    if (oldPlanEndDate) {
        obj.OldPlanEndDate = $.jsDate2WcfDate(new Date(oldPlanEndDate));    //将原计划完成日期存入变量
    }
    obj.ActualEndDate = $.jsDate2WcfDate(obj.ActualEndDate);

    $.srv("LPA", "ProblemSave", {
        data: { model: obj },
        success: function (data) {
            CloseWindow("save");
        }
    });
}

function getData() {
    var obj = form.getData();
    //获取图片列表数据
    obj.BeforeProbPicList = dgProbBefore.getData();
    obj.AfterProbPicList = dgProbAfter.getData();
    obj.ProbID = reqStr;
    if (obj.MachineID == "")
        obj.MachineID = null;
    return obj;
}


function setData(id) {
    $.srv("LPA", "ProblemGet", {
        data: { id: id },
        success: function (result) {
            var obj = result.Data;
            form.setData(obj);
            txtResponsible.setText(obj.ResponsibleName);
            txtResponsible.setValue(obj.Responsible);
            var submitDate = $.wcfDate2JsDate(obj.SubmitDate);
            txtSubmitDate.setValue(submitDate);
            var planStartDate = obj.PlanStartDate == null ? "" : $.wcfDate2JsDate(obj.PlanStartDate).format("yyyy-MM-dd");
            txtPlanStartDate.setValue(planStartDate);
            var planEndDate = obj.PlanEndDate == null ? "" : $.wcfDate2JsDate(obj.PlanEndDate).format("yyyy-MM-dd");
            txtPlanEndDate.setValue(planEndDate);

            oldPlanEndDate = planEndDate;

            var actualEndDate = obj.ActualEndDate == null ? "" : $.wcfDate2JsDate(obj.ActualEndDate).format("yyyy-MM-dd");
            txtActualEndDate.setValue(actualEndDate);

            form.setChanged(false);
            dgProbBefore.set({ data: obj.BeforeProbPicList });
            dgProbAfter.set({ data: obj.AfterProbPicList });
            //cboPicType.setValue(obj.ProbPicList[0].PicType);
            $("#lblCreateby").html(obj.CreateByName);
        }
    });
}


function onChooseEmp(e) {
    var txt = e.sender;

    mini.open({
        title: "请选择员工...",
        url: bootPATH + "LPA/ChooseLPAEmployee.html?multiSelect=0",
        width: 500,
        height: 425,
        onload: function () {
            var iframe = this.getIFrameEl();
            var selected = { EmpID: txt.getValue(), EmpName: txt.getText() };
            iframe.contentWindow.setData(selected);
        },
        ondestroy: function (action) {
            if (action == "ok") {
                var iframe = this.getIFrameEl();
                var data = iframe.contentWindow.getData();
                if (data.length < 1) return false;
                data = mini.clone(data[0]);
                txt.setValue(data.EmpID);
                txt.setText(data.EmpName);
                txt.doValueChanged(e);
            }
        }
    });
}



function onFileNameRender(e) {
    if (isAdd) {
        return "<img src=\"" + SS.config.tempPath + e.row.FileName + "\" style=\"height:96px; width:180px;\"> ";
    } else {
        return "<img src=\"" + SS.config.lPAProbPicPath + e.row.FileName + "\" style=\"height:96px; width:180px;\"> ";
    }
}


//新增发现时照片
function onAddProbPicBefore() {
    mini.open({
        title: "新增LPA问题图片",
        url: bootPATH + "LPA/ProbPicInfo.html",
        width: 700,
        height: 410,
        ondestroy: function (aciton) {
            if (aciton != "save") return;

            var iframe = this.getIFrameEl();
            var data = mini.clone(iframe.contentWindow.returnData);        //这里的returnData 是获取子页面的setData 中的returnData
            isAdd = true;
            // 新增
            dgProbBefore.addRow(data);
            return true;
        }
    });
};


//删除发现时照片
function onRemoveProbPicBefore() {
    var row = dgProbBefore.getSelected();
    if (!row) {
        mini.showTips({ content: "请选择要删除的问题图片", state: "danger" });
        return;
    };
    mini.confirm("确定删除 <i class='icon-quote-left'></i> <span class='red'>" + row.FileName + "</span> <i class='icon-quote-right'></i> ？", "确定？",
        function (action) {
            if (action != "ok") return;
            dgProbBefore.removeRow(row, true);
        });
};



//新增整改后照片
function onAddProbPicAfter() {
    mini.open({
        title: "新增LPA问题图片",
        url: bootPATH + "LPA/ProbPicInfo.html",
        width: 700,
        height: 410,
        ondestroy: function (aciton) {
            if (aciton != "save") return;

            var iframe = this.getIFrameEl();
            var data = mini.clone(iframe.contentWindow.returnData);        //这里的returnData 是获取子页面的setData 中的returnData
            isAdd = true;
            // 新增
            dgProbAfter.addRow(data);
            return true;
        }
    });
};


//删除整改后照片
function onRemoveProbPicAfter() {
    var row = dgProbAfter.getSelected();
    if (!row) {
        mini.showTips({ content: "请选择要删除的问题图片", state: "danger" });
        return;
    };
    mini.confirm("确定删除 <i class='icon-quote-left'></i> <span class='red'>" + row.FileName + "</span> <i class='icon-quote-right'></i> ？", "确定？",
        function (action) {
            if (action != "ok") return;
            dgProbAfter.removeRow(row, true);
        });
};

