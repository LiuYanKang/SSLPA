
var dgProbPicBefore, dgProbPicAfter, dgProbLog;
var reqStr;

$.page_load = function () {
    dgProbPicBefore = mini.get("dgProbPicBefore");
    dgProbPicAfter = mini.get("dgProbPicAfter");
    dgProbLog = mini.get("dgProbLog");
    reqStr = $.request("probID");
    if (reqStr) {
        setData(reqStr);
    }
}


function setData(id) {

    $.srv("LPA", "ProblemGet", {
        data: { id: id },
        success: function (result) {
            var obj = result.Data;
            $("#lblCreateByName").html(obj.CreateByName);
            $("#lblResponsibleName").html(obj.ResponsibleName);
            $("#lblProblemRegion").html(obj.ProblemRegionName);
            $("#lblProblemType").html(obj.ProblemTypeName);
            $("#lblMeasure").html(obj.Measure);
            $("#lblRemark").html(obj.Remark);
            $("#lblProblemDesc").html(obj.ProblemDesc);
            var submitTime = obj.SubmitDate == null ? null : $.wcfDate2JsDate(obj.SubmitDate).format("yyyy-MM-dd hh:mm");
            $("#lblSubmitTime").html(submitTime);

            var planStartDate = obj.PlanStartDate == null ? "" : $.wcfDate2JsDate(obj.PlanStartDate).format("yyyy-MM-dd");
            $("#lblPlanStartDate").html(planStartDate);
            var planEndDate = obj.PlanEndDate == null ? "" : $.wcfDate2JsDate(obj.PlanEndDate).format("yyyy-MM-dd");
            $("#lblPlanEndDate").html(planEndDate);
            var actualEndDate = obj.ActualEndDate == null ? "" : $.wcfDate2JsDate(obj.ActualEndDate).format("yyyy-MM-dd");
            $("#lblActualEndDate").html(actualEndDate);

            dgProbPicBefore.setData(obj.BeforeProbPicList);
            dgProbPicAfter.setData(obj.AfterProbPicList);
            dgProbLog.setData(obj.ProbLogList);
        }
    });
}


function onFileNameRender(e) {
    return "<img src=\"" + SS.config.lPAProbPicPath + e.row.FileName + "\" style=\"height:96px; width:180px;cursor:pointer;\" onclick='onClickPic(this)'> ";
}

function onClickPic(e) {
    mini.open({
        title: "查看问题图片",
        url: e.src,
        width: 1000,
        height: 500
    });
}