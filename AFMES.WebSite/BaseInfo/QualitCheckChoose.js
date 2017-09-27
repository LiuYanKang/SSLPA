
var dgleft, dgCenter, dgRight;
var cboMaterialType;
var materialtype, materialID, materialCode;
var procID, procName;
var selectedItems;

$.page_load = function () {
    dgleft = mini.get("dgleft");
    dgCenter = mini.get("dgCenter");
    dgRight = mini.get("dgRight");
    cboMaterialType = mini.get("cboMaterialType");
    selectedItems = [];

    $.dic("1007", function (e) {        // 物料类型
        var items = [];
        for (var i = 0; i < e.length; i++) {
            if (e[i].Code == "1") continue;
            items.push({ Code: e[i].Code, Name: e[i].Name });
            cboMaterialType.set({ data: items });
        }
    });

    dglSearch();
}


function dgl_BeforeLoad(e) {
    e.cancel = true;
    loadLeftList(e.params);
}


function loadLeftList(e) {                        //加载左边的DataGrid 菜单
    dgleft.loading();

    $.srv("BaseInfo", "SemiProductOrProductQuery", {
        data: { pagerParams: e },
        success: function (result) {
            dgleft.set({
                pageIndex: result.Data.PageIndex
                    , pageSize: result.Data.PageSize
                    , totalCount: result.Data.TotalCount
                , data: result.Data.Data
            });

            if (result.Data.Data.length > 0) {
                dgleft.setSelected(dgleft.data[0]);       //默认选择左边DataGird 第一行
            }
        }
    });
}

//查询
function dglSearch() {
    dgleft.load(searchKey());
}


function searchKey() {
    var key = mini.get("leftKeyword").getValue();
    var materialType = cboMaterialType.getValue();
    materialType = materialType == "" ? null : materialType;
    return {
        Keyword: key,
        MaterialType: materialType
    }
}


function onMaterialType(e) {
    if (e.row.MaterialType == "2") {
        return "半成品";
    } else {
        return "产品";
    }
}



//以下为工序模块内容
function dgleft_selectionchanged(e) {
    var row = e.selected;
    materialtype = row.MaterialType;         //传物料类型 选择相应的工序

    materialID = row.MaterialID;        //获取当前选中行的物料ID
    materialCode = row.Code;            //获取当前选中行的物料号
    dgCenter.load({ type: materialtype });
}


//加载列表
function dgCenter_BeforeLoad(e) {
    e.cancel = true;
    loadWorkProcessList(e.params);
};


function loadWorkProcessList(params) {
    dgCenter.loading();
    $.srv("BaseInfo", "WorkProcessListByMaterialID", {
        data: { type: params.type },
        success: function (result) {

            dgCenter.set({
                data: result.Data
            });
            if (result.Data.length > 0)
                dgCenter.setSelected(dgCenter.data[0]);       //默认选择第一行
        }
    });
};


//查询
function dgCenter_selectionchanged(e) {
    var row = e.selected;
    procID = row.ProcID;        //工序ID
    procName = row.Name;        //工序名称

    dgRight.load({ ItemID: materialID, ItemType: materialtype, ProcessID: procID });
}




//加载列表
function dgRight_BeforeLoad(e) {
    e.cancel = true;
    loadQualitCheckList(e.params);
};


function loadQualitCheckList(params) {
    dgRight.loading();
    $.srv("BaseInfo", "QualitCheckQuery", {
        data: { pagerParams: params },
        success: function (result) {

            dgRight.set({
                data: result.Data
            });
        }
    });
};


//查询质量释放项目数据
function searchright() {
    dgRight.load(dgrSearchKey());
}


function dgrSearchKey() {
    var key = mini.get("rightKeyword").getValue();

    return {
        Keyword: key,
        ItemID: materialID,
        ItemType: materialtype,
        ProcessID: procID
    }
}



function onInfoTypeName(e) {
    if (e.row.InfoType == "1") {    //数值
        return e.row.MinValue + "~" + e.row.MaxValue;
    }

    if (e.row.InfoType == "2") {    //布尔
        return "布尔值";
    }
}






function CloseWindow(action) {
    if (window.CloseOwnerWindow) return window.CloseOwnerWindow(action);
    else window.close();
};

function onOk(e) {
    CloseWindow("ok");
};
function onCancel() {
    CloseWindow("cancel");
};


//选中后点击确认  会将数据返回到上一个页面中
function getData() {
    return selectedItems;
};


function setData(data) {
    if (multiSelect == "0") {
        selectedItems.push(data);
    } else {
        for (var j = 0; j < data.length; j++) {
            selectedItems.push(data[j]);
        }
    }
}
//保存所有选中的数据
function onSelectoinChanged(e) {
    if (e.select) {
        if (multiSelect == "0") {
            selectedItems = [];
        }
        for (var i = 0; i < e.records.length; i++) {
            var hasValue = false;
            for (var li = 0; li < selectedItems.length; li++) {
                if (selectedItems[li].QCInfoID == e.records[i].QCInfoID) {
                    hasValue = true;
                    break;
                }
            }
            if (!hasValue)
                selectedItems.push(e.records[i]);
        }
    } else {
        for (var i = 0; i < e.records.length; i++) {
            for (var li = 0; li < selectedItems.length; li++) {
                if (selectedItems[li].QCInfoID == e.records[i].QCInfoID) {
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
            if (this.QCInfoID == source[i].QCInfoID) {
                curSelected.push(source[i]);
                return false;
            }
        });
    }
    if (curSelected.length)
        e.sender.selects(curSelected);
}


