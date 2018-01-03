var datagrid;
var machineid;
var selectedItems = [];

$.page_load = function () {
    machineid = $.request("id");
    getMachineEHS();

    datagrid = mini.get("datagrid");
    search();

};

//加载列表
function datagrid_BeforeLoad(e) {
    e.cancel = true;
    loadList(e.params);
};
//查询
function search() {

    datagrid.load();
};

function loadList(e) {
    datagrid.loading();
    $.srv("BaseInfo", "EHSQuery", {
        data: { keyword: mini.get("keyword").getValue(), param: e },
        success: function (result) {
            datagrid.set({
                pageIndex: result.Data.PageIndex
                , pageSize: result.Data.PageSize
                , totalCount: result.Data.TotalCount
                , data: result.Data.Data
            });
        }
    });
};


//显示小图标
function onImg(e) {
    if (!e.value) return "未上传图片";
    return "<img src=\"" + SS.config.ehsPicPath + e.value + "\" style=\"height:100px; \"> ";
}


function onOk() {
    var btn = mini.get("btnOk");
    btn.disable();

    $.srv("BaseInfo", "SaveMachineEHS", {
        data: { machineid: machineid, ids: selectedItems },
        success: function (data) {
            CloseWindow("save");
        },
        exception: function (e) {
            btn.enable();
        }
    });
};
function onCancel() {
    CloseWindow("cancel");
};


//保存所有选中的数据
function onSelectoinChanged(e) {
    if (e.select) {
        for (var i = 0; i < e.records.length; i++) {
            var hasValue = false;
            for (var li = 0; li < selectedItems.length; li++) {
                if (selectedItems[li] == e.records[i].EHSID) {
                    hasValue = true;
                    break;
                }
            }
            if (!hasValue)
                selectedItems.push(e.records[i].EHSID);
        }
    } else {
        for (var i = 0; i < e.records.length; i++) {
            for (var li = 0; li < selectedItems.length; li++) {
                if (selectedItems[li] == e.records[i].EHSID) {
                    selectedItems.removeAt(li);
                    break;
                }
            }
        }
    }
};

//表格渲染事件完毕
function onGridUpdate(e) {
    var source = e.source.data;
    var selected = $(selectedItems);
    var curSelected = [];
    for (var i in source) {
        selected.each(function () {
            if (this == source[i].EHSID) {
                curSelected.push(source[i]);
                return false;
            }
        });
    }
    if (curSelected.length)
        e.sender.selects(curSelected);
}

function getMachineEHS() {
    $.srv("BaseInfo", "GetMachineEHS", {
        data: { machineid: machineid },
        type: "GET",
        success: function (e) {
            var data = e.Data;
            for (var i = 0; i < data.length; i++) {
                selectedItems.push(data[i].EHSID);
            }
        }
    });
}