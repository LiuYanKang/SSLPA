var tbody;
var machineid = 0;
var mask;

//加载弹出层页面
function loadPopUpLayer() {

    var pid = this.getAttribute('ProbID');

    mask = mui.createMask(function () {
        $("#popover").hide();
    });
    mask.show();//显示遮罩
    $("#popover").show();

    $.srv("LPA", "KanBanProblemDetail", {
        type: "GET",
        data: { pid: pid },
        success: function (result) {
            var obj = result.Data;
            $("#lblProblemRegion").html(obj.ProblemRegionName);
            $("#lblCreateByName").html(obj.CreateByName);
            $("#lblSort").html(obj.ProblemTypeName);
            $("#lblResponsible").html(obj.ResponsibleName);
            //var actualEndDate = obj.ActualEndDate == null ? "" : $.wcfDate2JsDate(obj.ActualEndDate).format("MM-dd");
            //$("#lblDate").html(actualEndDate);

            var submitTime = obj.SubmitDate == null ? null : $.wcfDate2JsDate(obj.SubmitDate).format("yyyy-MM-dd hh:mm");
            $("#lblSubmitTime").html(submitTime);

            var planStartDate = obj.PlanStartDate == null ? "" : $.wcfDate2JsDate(obj.PlanStartDate).format("yyyy-MM-dd");
            $("#lblPlanStartDate").html(planStartDate);
            var planEndDate = obj.PlanEndDate == null ? "" : $.wcfDate2JsDate(obj.PlanEndDate).format("yyyy-MM-dd");
            $("#lblPlanEndDate").html(planEndDate);
            var actualEndDate = obj.ActualEndDate == null ? "" : $.wcfDate2JsDate(obj.ActualEndDate).format("yyyy-MM-dd");
            $("#lblActualEndDate").html(actualEndDate);

            if (!obj.ProblemDesc || obj.ProblemDesc == "") obj.ProblemDesc = "无";
            $("#lblDesc").html(obj.ProblemDesc);
            if (!obj.Measure || obj.Measure == "") obj.Measure = "无";
            $("#lblMeasure").html(obj.Measure);
            if (!obj.Remark || obj.Remark == "") obj.Remark = "无";
            $("#lblRemark").html(obj.Remark);
            //dgProbPicBefore.setData(obj.BeforeProbPicList);
            //dgProbPicAfter.setData(obj.AfterProbPicList);
            //dgProbLog.setData(obj.ProbLogList);
            //发现时照片
            var discoverPhoto = obj.BeforeProbPicList;
            //alert(obj.BeforeProbPicList);
            //整改后照片
            var updatePhoto = obj.AfterProbPicList;
            ////修改日志
            //var log = obj.ProbLogList;

            //填充发现时照片折叠面板
            var dishtml = "";
            if (discoverPhoto.length > 0) {
                for (var i = 0; i < discoverPhoto.length; i++) {
                    var li = discoverPhoto[i];
                    dishtml += "<p>";
                    dishtml += "<img src='" + SS.config.lPAProbPicPath + li.FileName + "' data-preview-src='' data-preview-group='1'/>";
                    dishtml += "</p>";

                    //dishtml += "<img src='" + SS.config.lPAProbPicPath + li.FileName + "' />";
                }
            } else {
                dishtml = "<p>无照片</p>";
            }
            $("#dgProbPicBefore").html(dishtml);

            //填充整改后照片折叠面板
            if (updatePhoto.length > 0) {
                var updhtml = "";
                for (var i = 0; i < updatePhoto.length; i++) {
                    var li = updatePhoto[i];
                    updhtml += "<p>";
                    updhtml += "<img src='" + SS.config.lPAProbPicPath + li.FileName + "' data-preview-src='' data-preview-group='2'/>";
                    updhtml += "</p>";

                    //updhtml += "<img src='" + SS.config.lPAProbPicPath + li.FileName + "' />";
                }
            } else {
                updhtml = "<p>无照片</p>";
            }
            $("#dgProbPicAfter").html(updhtml);
        }
    });
}

$.page_load = function () {
    mui.init();

    machineid = $.request('machineid');
    if (!machineid || machineid == "") machineid = 0;
    tbody = $("#tb tbody");
    tbody.empty();
    loadData();

    mui("#tb tbody").on('tap', 'tr', loadPopUpLayer);

};

function loadData() {
    $.srv("LPA", "KanBanProblemList", {
        type: "GET",
        data: { machineid: machineid },
        success: function (result) {
            var data = result.Data;
            if (data.length > 0) {
                var tbodyHtml = "";
                for (var i = 0; i < data.length; i++) {
                    var li = data[i];
                    var html = "";
                    html += "<tr ProbID='" + li.ProbID + "'  >";
                    //html += "<td>" + li.CreateByName + "</td>";
                    html += "<td>" + li.ProblemRegionName + "</td>";
                    html += "<td>" + li.ProblemTypeName + "</td>";
                    html += "<td>" + li.ProblemDesc + "</td>";
                    //html += "<td>" + (li.Measure || "") + "</td>";
                    html += "<td>" + li.ResponsibleName + "</td>";

                    var planEndDat = li.PlanEndDate == null ? "" : $.wcfDate2JsDate(li.PlanEndDate).format("MM-dd");
                    //html += "<td>" + planEndDat + "</td>";

                    var actualEndDate = li.ActualEndDate == null ? "" : $.wcfDate2JsDate(li.ActualEndDate).format("MM-dd");
                    html += "<td>" + actualEndDate + "</td>";
                    html += "<td class='status " + getProgress(li.Progress) + "'>" + li.Progress + "%</td>";
                    html += "</tr>";
                    tbodyHtml += html;
                }

                tbody.empty().append(tbodyHtml);
                window.setTimeout("loadData()", 3 * 60 * 1000);
            }
        },
        exception: function (data) {
            mui.toast(data.Msg);
            console.log(data.Msg);
            window.setTimeout("loadData()", 3000);
            return false;
        }
    });
}

function getProgress(prog) {
    if (prog > 0 && prog < 100)
        return "processing";
    if (prog == 100)
        return "end";

    return "";
}

