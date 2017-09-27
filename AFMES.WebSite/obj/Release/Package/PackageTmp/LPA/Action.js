
var datagrid, cboState;

$.page_load = function () {
    datagrid = mini.get("datagrid");
    cboState = mini.get("cboState");

    $.dic("1027", function (e) {
        cboState.set({ data: e });
    })

    search();
}


function datagrid_BeforeLoad(e) {
    e.cancel = true;
    loadActionList(e.params);
}


function loadActionList(e) {
    datagrid.loading();

    $.srv("LPA", "ActionList", {
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
    var state = cboState.getValue() == "" ? null : cboState.getValue();
    datagrid.load({ Keyword: key, State: state });
}


//删除
function ActionRemove() {
    var row = datagrid.getSelected();
    if (!row) {
        mini.showTips({ content: "请选择要删除的LPA审核记录", state: "danger" });
        return;
    };
    mini.confirm("确定删除 ？", "确定？",
        function (action) {
            if (action != "ok") return;
            $.srv("LPA", "ActionDel", {
                data: { id: row.ActionID },
                success: function (result) {
                    if (result.Data) {
                        mini.showTips({ content: "删除成功", state: "success" });
                        datagrid.reload();
                    }
                }
            });
        });
};


function onDetailLink(e) {
    return "<a href='javascript:onOpen(" + e.row.ActionID + ")'>" + mini.formatDate($.wcfDate2JsDate(e.value), "yyyy-MM-dd HH:mm") + "</a>"
}


function onOpen(id) {
    mini.open({
        title: "审核记录详情",
        url: bootPATH + "LPA/ActionView.html?actionID=" + id,
        width: 650,
        height: 410,
        ondestroy: function (action) {
            if (action != "save") return;
            search();
        }
    });
}
