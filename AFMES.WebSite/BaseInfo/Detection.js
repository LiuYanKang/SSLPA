var datagrid;

$.page_load = function () {
    datagrid = mini.get("datagrid");
};

//加载列表
function datagrid_BeforeLoad(e) {
    e.cancel = true;
    loadWorkProcessList(e.params);
}

function loadWorkProcessList(e) {
    datagrid.loading();

    $.srv("BaseInfo", "WorkProcessList", {
        data: { pagerParams: e },
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

function OnAdd() {                                          //新增
    mini.open({
        title: "新增检查项",
        url: bootPATH + "BaseInfo/DetectionInfo.html",
        width: 400,
        height: 500,
        ondestroy: function (aciton) {
            if (aciton != "save") return;
            mini.showTips({ content: "新增成功", state: "success" });
            search();
        }
    });
};
