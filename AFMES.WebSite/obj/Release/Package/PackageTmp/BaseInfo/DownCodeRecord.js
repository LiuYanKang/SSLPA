var datagrid;
var cboMachineName, cboEquipStatusName;

$.page_load = function () {
    datagrid = mini.get("datagrid");

    cboMachineName = mini.get("cboMachineName");
    $.srv("BaseInfo", "MachineAllS", {
        success: function (result) {
            cboMachineName.set({
                data: result.Data
            })
        }
    });

    cboEquipStatusName = mini.get("cboEquipStatusName");
    $.dic("1004", function (e) {
        var status = [];
        for (var i = 0; i < e.length; i++) {
            if (e[i].Code != "1") {
                status.push({
                    Code: e[i].Code,
                    Name: e[i].Name
                });
            }
        }
        cboEquipStatusName.set({ data: status });
    })
    search();
}



function datagrid_BeforeLoad(e) {
    e.cancel = true;
    loadDownCodeRecordList(e.params);
}


function loadDownCodeRecordList(e) {
    datagrid.loading();

    $.srv("BaseInfo", "DownCodeRecordQuery", {
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

//查询
function search() {
    datagrid.load(searchKey());
}

function searchKey() {
    var key = mini.get("keyword").getValue();
    var machineName = cboMachineName.getValue();
    machineName = machineName == "" ? null : machineName;
    var equipStatusName = cboEquipStatusName.getValue();
    equipStatusName = equipStatusName == "" ? null : equipStatusName;
    return {
        Keyword: key,
        MachineID: machineName,
        Code: equipStatusName
    }
}

function onTroTypRenderer(e) {
    if (e.row.TroubleTypeName == null) {
        e.row.TroubleTypeName = ""
    }
    else {
        return "<a href='javascript:onQuery(" + e.row.CloseDownID + ")' title='点击查看维修记录'>" + e.row.TroubleTypeName + "</a>";
    }
}

function onQuery(id) {
    mini.open({
        title: "维修记录详情",
        url: bootPATH + "BaseInfo/RepairRecord.html?closeDownID=" + id,
        width: 700,
        height: 360,
        ondestroy: function (action) {
            if (action != "save") return;
            search();
        }
    });
}