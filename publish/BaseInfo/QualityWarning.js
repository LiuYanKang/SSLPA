var datagrid;
var  cboProductType;
var form;
var ue = null;
$.page_load = function () {
    mini.parse();
    form = new mini.Form("form1");
    datagrid = mini.get("datagrid");
    cboProductType = mini.get("cboProductType");
    $.dic("1030", function (e) {
        cboProductType.set({ data: e });
    });

    search();
    InitializeUE();
};

function InitializeUE() {
    ue = UE.getEditor('editor', {
        initialFrameHeight: $("#parenttd").height()-150
    });
}

//加载列表
function datagrid_BeforeLoad(e) {
    e.cancel = true;
    loadMachineList(e.params);
}

function datagrid_selectionchanged(e) {
    setData(e.selected);
}


function loadMachineList(e) {
    datagrid.loading();
    $.srv("BaseInfo", "MachineQuery", {
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

function searchKey() {
    var productType = cboProductType.getValue();
    return {
        Keyword: mini.get("keyword").getValue(),
        ProcID: null,
        ProductType: productType
    };
}
//查询
function search() {
    datagrid.load(searchKey());
}


function setData(data) {
    form.setData(data);
    ue.setContent(data.QualityWarning);
}


function onOk() {
    var obj = form.getData(true);
    obj.QualityWarning = ue.getContent();
    $.srv("BaseInfo", "MachineQualityWarningSave", {
        data: { model: obj },
        success: function (data) {
            mini.showTips({ content: "保存成功", state: "success" });
            search();
        }
    });
}

