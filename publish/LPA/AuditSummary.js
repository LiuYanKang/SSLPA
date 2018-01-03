
var datagrid, cboArea;

$.page_load = function () {
    datagrid = mini.get("datagrid");
    cboArea = mini.get("cboArea");

    $.srv("LPA", "AuditAreaGet", {
        success: function (data) {
            cboArea.set({ data: data.Data });
        }
    });

    search();
}


function datagrid_BeforeLoad(e) {
    e.cancel = true;
    loadAuditSummaryList(e.params);
}


function loadAuditSummaryList(e) {
    datagrid.loading();

    $.srv("LPA", "AuditSummaryList", {
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
    var area = cboArea.getValue() == "" ? null : cboArea.getValue();
    datagrid.load({ Keyword: key,Area:area});
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