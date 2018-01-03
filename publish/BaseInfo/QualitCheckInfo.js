
var dgleft, dgCenter, dgRight;
var cboMaterialType;
var materialtype, materialID, materialCode;
var procID, procName;


$.page_load = function () {
    dgleft = mini.get("dgleft");
    dgCenter = mini.get("dgCenter");
    dgRight = mini.get("dgRight");
    cboMaterialType = mini.get("cboMaterialType");


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



//新增
function onAdd() {
    mini.open({
        title: "新增质量释放项目",
        url: bootPATH + "BaseInfo/QualitCheckInfoInfo.html?materialID=" + materialID + "&materialCode=" + materialCode + "&procID=" + procID + "&procName=" + escape(procName) + "&materialtype=" + materialtype,
        width: 380,
        height: 330,
        ondestroy: function (aciton) {
            if (aciton != "save") return;
            mini.showTips({ content: "新增成功", state: "success" });
            searchright();
        }
    });
};


//编辑
function onEdit() {
    var row = dgRight.getSelected();
    if (!row) {
        mini.showTips({ content: "请选择要编辑的质量释放项目", state: "danger" });
        return;
    }

    mini.open({
        title: "编辑质量释放项目",
        url: bootPATH + "BaseInfo/QualitCheckInfoInfo.html?materialID=" + materialID + "&materialCode=" + materialCode + "&procID=" + procID + "&procName=" + escape(procName) + "&materialtype=" + materialtype + "&qCInfoID=" + row.QCInfoID,
        width: 380,
        height: 330,
        ondestroy: function (aciton) {
            if (aciton != "save") return;
            mini.showTips({ content: "修改成功", state: "success" });
            searchright();
        }
    });
};


//删除
function onRemove() {
    var row = dgRight.getSelected();
    if (!row) {
        mini.showTips({ content: "请选择要删除的质量释放项目", state: "danger" });
        return;
    };
    mini.confirm("确定删除 <i class='icon-quote-left'></i> <span class='red'>" + row.Content + "</span> <i class='icon-quote-right'></i> ？", "确定？",
        function (action) {
            if (action != "ok") return;
            $.srv("BaseInfo", "QualitCheckDel", {
                data: { id: row.QCInfoID },
                success: function (result) {
                    if (result.Data) {
                        mini.showTips({ content: "删除成功", state: "success" });
                        searchright();
                    }
                }
            });
        });
};


//批量删除
function onRemoveForBatch() {
    var rows = getRoleIDList();
    if (!rows) {
        mini.showTips({ content: "请选择要删除的质量释放项目", state: "danger" });
        return;
    };
    mini.confirm("确定删除选中的质量释放项目？", "确定？",
        function (action) {
            if (action != "ok") return;
            $.srv("BaseInfo", "QualitCheckDelForBatch", {
                data: { rowsID: rows },
                success: function (result) {
                    if (result.Data) {
                        mini.showTips({ content: "删除成功", state: "success" });
                        searchright();
                    }
                }
            });
        });
};



//获得选中项编码集合
function getRoleIDList() {
    var rowIDList = [];
    //获取选中项对象集合
    var entityList = dgRight.getSelecteds();
    //判断对象集合有值
    if (entityList && entityList.length > 0) {
        for (var i = 0; i < entityList.length; i++) {
            rowIDList.push(entityList[i].QCInfoID);
        };
    };
    return rowIDList;
};


function onCopy(e) {
    var txt = e.sender;

    mini.open({
        title: "请选择质量释放项目...",
        url: bootPATH + "BaseInfo/QualitCheckChoose.html",
        width: 1000,
        height: 500,
        onload: function () {
            var iframe = this.getIFrameEl();
        },
        ondestroy: function (action) {
            if (action == "ok") {
                var iframe = this.getIFrameEl();
                var data = iframe.contentWindow.getData();
                if (data.length < 1) return false;

                var objs = [];
                for (i = 0; i < data.length; i++) {
                    objs.push({ ProcessID: procID, ItemID: materialID, ItemType: materialtype, QCInfoID: data[i].QCInfoID });
                }

                $.srv("BaseInfo", "QualitCheckAddForBatch", {
                    data: { models: objs },
                    success: function (result) {
                        if (result.Data) {
                            mini.showTips({ content: "拷贝成功", state: "success" });
                            searchright();
                        }
                    }
                });
            }
        }
    });
}



function onInfoTypeName(e) {
    if (e.row.InfoType == "1") {    //数值
        return e.row.MinValue + "~" + e.row.MaxValue;
    }

    if (e.row.InfoType == "2") {    //布尔
        return "布尔值";
    }
}