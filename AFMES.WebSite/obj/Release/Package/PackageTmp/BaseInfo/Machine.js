var datagrid;
var txtProcID, cboProductType, sPDCode;

$.page_load = function () {
    mini.parse();
    datagrid = mini.get("datagrid");
    txtProcID = mini.get("txtProcID");
    cboProductType = mini.get("cboProductType");
    sPDCode = mini.get("sPDCode");

    // 产品部门
    $.srv("BaseInfo", "ProDept", {
        success: function (data) {
            sPDCode.set({ data: data.Data });
        }
    });
    //$.dic("1030", function (e) {
    //    cboProductType.set({ data: e });
    //});

    search();
};

//加载列表
function datagrid_BeforeLoad(e) {
    e.cancel = true;
    loadMachineList(e.params);
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
    var key = mini.get("keyword").getValue();
    var procID = txtProcID.getValue();
    procID = procID == "" ? null : procID;
    var productType = sPDCode.getValue();
    return {
        Keyword: mini.get("keyword").getValue(),
        ProcID: procID,
        ProductType: productType
    };
}
//查询
function search() {
    datagrid.load(searchKey());
}


function OnAdd() {                                          //新增
    mini.open({
        title: "新增设备",
        url: bootPATH + "BaseInfo/MachineInfo.html",
        width: 400,
        height: 350,
        ondestroy: function (aciton) {
            if (aciton != "save") return;
            mini.showTips({ content: "新增成功", state: "success" });
            search();
        }
    });
};

function onEdit() {
    var row = datagrid.getSelected();
    if (!row) {
        mini.showTips({ content: "请选择要编辑的设备", state: "danger" });
        return;
    }
    mini.open({
        title: "编辑设备",
        url: bootPATH + "BaseInfo/MachineInfo.html?id=" + row.MachineID,
        width: 400,
        height: 350,
        ondestroy: function (aciton) {
            if (aciton != "save") return;
            mini.showTips({ content: "修改成功", state: "success" });
            search();
        }
    });
}

function OnRemove(e) {
    var row = datagrid.getSelected();
    if (!row) {
        mini.showTips({ content: "请选择要删除的设备", state: "danger" });
        return;
    };
    mini.confirm("确定删除 <i class='icon-quote-left'></i> <span class='red'>" + row.Code + "</span> <i class='icon-quote-right'></i> ？", "确定？",
       function (action) {
           if (action != "ok") return;
           $.srv("BaseInfo", "MachineDel", {
               data: { id: row.MachineID },
               success: function (result) {
                   if (result.Data) {
                       mini.showTips({ content: "删除成功", state: "success" });
                       search();
                   }
               }
           });
       });
}

function onChooseWorkProcess(e) {
    var txt = e.sender;

    mini.open({
        title: "请选择工序...",
        url: bootPATH + "BaseInfo/WorkProcessChoose.html?ProcID=" + txtProcID.getValue(),
        width: 520,
        height: 360,
        onload: function () {
            var iframe = this.getIFrameEl();
        },
        ondestroy: function (action) {
            if (action == "ok") {
                var iframe = this.getIFrameEl();
                var data = iframe.contentWindow.getData();
                if (data.length < 1) return false;
                data = mini.clone(data);
                txt.setValue(data.ProcID);
                txt.setText(data.Name);
                txt.doValueChanged(e);
            }
        }
    });
}


// 显示二维码
function showQrCode() {

    var row = datagrid.getSelected();
    if (!row) {
        mini.showTips({ content: "请选择要查看的项", state: "danger" });
        return;
    };

    var data = { Type: "Machine", Value: row.Code };
    SS.showQrCode(JSON.stringify(data));
}

function editEHS() {
    var row = datagrid.getSelected();
    if (!row) {
        mini.showTips({ content: "请选择要查看的项", state: "danger" });
        return;
    };

    mini.open({
        title: "编辑设备EHS",
        url: bootPATH + "BaseInfo/MachineEHS.html?id=" + row.MachineID,
        width: 600,
        height: 500,
        ondestroy: function (aciton) {
            if (aciton != "save") return;
            mini.showTips({ content: "保存成功", state: "success" });
        }
    });
}