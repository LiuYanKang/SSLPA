var form;
var reqStr;
var txtStartPlanDate, txtEndPlanDate, txtEmpID, cboAuditType, cblArea, cboBanCi;

$.page_load = function () {

    form = new mini.Form("form1");
    txtEmpID = mini.get("txtEmpID");
    txtStartPlanDate = mini.get("txtStartPlanDate");
    txtEndPlanDate = mini.get("txtEndPlanDate");
    cboAuditType = mini.get("cboAuditType");
    cboBanCi = mini.get("cboBanCi");
    cblArea = mini.get("cblArea");
    //reqStr = $.request("planID");
    //if (reqStr) {       //修改
    //    setData(reqStr);
    //}
    $.dic("1030", function (e) {
        cboBanCi.set({ data: e });
    });

    $.dic("1024", function (e) {        //审核类型
        cboAuditType.set({ data: e });
    });

    // 审核区域
    $.srv("LPA", "AuditAreaGet", {
        success: function (data) {
            cblArea.set({ data: data.Data });
        }
    });

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
    var startDate = obj.StartPlanDate.format("yyyy-MM-dd");
    var endDate = obj.EndPlanDate.format("yyyy-MM-dd");
    if (startDate > endDate) {
        mini.showTips({ content: "计划开始时间不能大于计划结束时间！", state: "danger" });
        return;
    }

    var dayCount = DateDiff(startDate, endDate);        //获取计划开始时间 跟 结束时间的天数差
    obj.dayCount = dayCount + 1;

    form.validate();
    if (form.isValid() == false) return;

    obj.StartPlanDate = $.jsDate2WcfDate(obj.StartPlanDate);
    obj.EndPlanDate = $.jsDate2WcfDate(obj.EndPlanDate);
    obj.BanCi = cboBanCi.getValue();
    obj.AuditArea = cblArea.getValue().split(',');
    $.srv("LPA", "ActionPlanAdd", {
        data: { model: obj },
        success: function (data) {
            CloseWindow("save");
        }
    });
}

function getData() {
    var obj = form.getData();
    if (obj.PlanID == "") {
        obj.PlanID = null;
    }
    return obj;
}


//function setData(id) {
//    $.srv("LPA", "ActionPlanGet", {
//        data: { id: id },
//        success: function (result) {
//            var obj = result.Data;
//            form.setData(obj);
//            txtEmpID.setText(obj.EmpName);
//            txtEmpID.setValue(obj.EmpID);
//            var startPlanDate = $.wcfDate2JsDate(obj.StartPlanDate).format("yyyy-MM-dd");
//            txtStartPlanDate.setValue(startPlanDate);
//            var endPlanDate = $.wcfDate2JsDate(obj.EndPlanDate).format("yyyy-MM-dd");
//            txtEndPlanDate.setValue(endPlanDate);
//            form.setChanged(false);
//        }
//    });
//}


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
                $.srv("LPA", "AuditAreaGet", {
                    data: { id: data.EmpID },
                    success: function (data) {
                        cblArea.set({ data: data.Data });
                    }
                });
                //选择员工默认加载选择对应区域和类型
                $.srv("LPA", "SetActionPlan", {
                    data: { id: data.EmpID },
                    success: function (result) {
                        var obj = result.Data;
                        cblArea.setValue(obj.AuditArea.join(','));
                        cboAuditType.setValue(obj.AuditType);
                        //cboBanCi.setValue(obj.BanCi);

                    }
                });
            }
        }
    });
}


//计算天数差的函数，通用  
function DateDiff(sDate1, sDate2) {    //sDate1和sDate2是2006-12-18格式  
    var aDate, oDate1, oDate2, iDays
    aDate = sDate1.split("-")
    oDate1 = new Date(aDate[1] + '-' + aDate[2] + '-' + aDate[0])    //转换为12-18-2006格式  
    aDate = sDate2.split("-")
    oDate2 = new Date(aDate[1] + '-' + aDate[2] + '-' + aDate[0])
    iDays = parseInt(Math.abs(oDate1 - oDate2) / 1000 / 60 / 60 / 24)    //把相差的毫秒数转换为天数  
    return iDays;
}

