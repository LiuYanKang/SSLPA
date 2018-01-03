var reqStr;

$.page_load = function () {
    reqStr = $.request("closeDownID");
    if (reqStr) {
        setData(reqStr);
    }
}

function setData(id) {

    $.srv("BaseInfo", "RepairRecordGet", {
        data: { id: id },
        success: function (result) {
            var obj = result.Data;
            $("#lblMachineID").html(obj.MachineName);
            var closedowntime = obj.CloseDownTime == "" ? null : $.wcfDate2JsDate(obj.CloseDownTime).format("yyyy-MM-dd hh:mm");
            $("#lblCloseDownTime").html(closedowntime);
            $("#lblSubmitPerson").html(obj.SubmitPersonName);
            $("#lblTroubleType").html(obj.TroubleTypeName);
            $("#lblTroubleCode").html(obj.TroubleCode);
            $("#lblMemo").html(obj.Memo);
            $("#lblRepairResult").html(obj.RepairResult);
            var repairstarttime = obj.RepairStartTime == "" ? null : $.wcfDate2JsDate(obj.RepairStartTime).format("yyyy-MM-dd hh:mm");
            $("#lblRepairStartTime").html(repairstarttime);
            var repairfinishtime = obj.RepairFinishTime == "" ? null : $.wcfDate2JsDate(obj.RepairFinishTime).format("yyyy-MM-dd hh:mm");
            $("#lblRepairFinishTime").html(repairfinishtime);
            $("#lblRepairPerson").html(obj.RepairPersonName);
            $("#lblConfirmPerson").html(obj.ConfirmPersonName);
        }
    });
}