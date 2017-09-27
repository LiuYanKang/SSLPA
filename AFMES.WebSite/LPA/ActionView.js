
var datagrid, dgProblem;
var id;
var cboAuditArea;

$.page_load = function () {
    datagrid = mini.get("datagrid");
    dgProblem = mini.get("dgProblem");
    id = $.request("actionID");
    cboAuditArea = mini.get("cboAuditArea");
    //$.dic("1025", function (e) {        //审核区域
    //    cboAuditArea.set({ data: e });
    //});

    $.srv("LPA", "AuditAreaGet", {
        success: function (data) {
            cboAuditArea.set({ data: data.Data });
        }
    });
    
    if (id) {
        setData(id);
        search();
    }
}


function setData(id) {

    $.srv("LPA", "LPAActionGet", {
        data: { id: id },
        success: function (result) {
            var obj = result.Data;
            $("#lblName").html(obj.EmpName);
            var auditDate = obj.AuditDate == null ? null : $.wcfDate2JsDate(obj.AuditDate).format("yyyy-MM-dd hh:mm");
            $("#lblAuditDate").html(auditDate);
            datagrid.setData(obj.LPAActionResultList);
            dgProblem.setData(obj.ProblemList);
        }
    });
}


function onResult(e) {
    switch (e.value) {
        case 1:
            return "Y";
        case 2:
            return "N";
        case 3:
            return "NA";
        case 4:
            return "NC";
        default:
            return e.value;
    }
}


function search() {
    var auditArea = mini.get("cboAuditArea").getValue();
    var keyword = mini.get("txtKeyword").getValue();

    $.srv("LPA", "LPAActionResultGet", {
        data: { id: id, area: auditArea, keyword: keyword },
        success: function (result) {
            datagrid.setData(result.Data);
        }
    });
}