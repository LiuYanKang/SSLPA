
var datagrid;
var txtSupplierID;

$.page_load = function () {
    mini.parse();

    datagrid = mini.get("datagrid");
    txtSupplierID = mini.get("txtSupplierID");
    search();
}


function datagrid_BeforeLoad(e) {
    e.cancel = true;
    loadSemiProductList(e.params);
}


function loadSemiProductList(e) {
    datagrid.loading();

    $.srv("BaseInfo", "SemiProductQuery", {
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
    var supplierID = txtSupplierID.getValue();
    supplierID = supplierID == "" ? null : supplierID;
    return {
        Keyword: mini.get("keyword").getValue(),
        SupplierID: supplierID
    };
}
//查询
function search() {
    datagrid.load(searchKey());
}


//新增
function onAdd() {
    mini.open({
        title: "新增半成品",
        url: bootPATH + "BaseInfo/SemiProductInfo.html",
        width: 690,
        height: 395,
        ondestroy: function (aciton) {
            if (aciton != "save") return;
            mini.showTips({ content: "新增成功", state: "success" });
            search();
        }
    });
};

//编辑
function onEdit() {
    var row = datagrid.getSelected();
    if (!row) {
        mini.showTips({ content: "请选择要编辑的半成品", state: "danger" });
        return;
    }

    mini.open({
        title: "编辑半成品",
        url: bootPATH + "BaseInfo/SemiProductInfo.html?id=" + row.SemiProdID,
        width: 690,
        height: 395,
        ondestroy: function (aciton) {
            if (aciton != "save") return;
            mini.showTips({ content: "修改成功", state: "success" });
            datagrid.reload();
        }
    });
};
//删除
function onRemove() {
    var row = datagrid.getSelected();
    if (!row) {
        mini.showTips({ content: "请选择要删除的半成品", state: "danger" });
        return;
    };
    mini.confirm("确定删除 <i class='icon-quote-left'></i> <span class='red'>" + row.Code + "</span> <i class='icon-quote-right'></i> ？", "确定？",
        function (action) {
            if (action != "ok") return;
            $.srv("BaseInfo", "SemiProductDel", {
                data: { id: row.SemiProdID },
                success: function (result) {
                    if (result.Data) {
                        mini.showTips({ content: "删除成功", state: "success" });
                        datagrid.reload();
                    }
                }
            });
        });
};


// 查看条码
function showBarCode() {
    var row = datagrid.getSelected();
    if (!row) {
        mini.showTips({ content: "请选择要查看的项", state: "danger" });
        return;
    }

    SS.showBarCode(row.Code);
}

function onSemiProdRenderer(e) {
    return "<a href='javascript:onQuery(" + e.row.SemiProdID + ")' title='点击查看半成品'>" + e.row.Code + "</a>";
}

function onQuery(id) {
    mini.open({
        title: "半成品详情",
        url: bootPATH + "BaseInfo/SemiProductView.html?SemiProdID=" + id,
        width: 700,
        height: 360,
        ondestroy: function (action) {
            if (action != "save") return;
            search();
        }
    });
}


function onChooseSupplier(e) {
    var txt = e.sender;

    mini.open({
        title: "请选择供应商...",
        url: bootPATH + "BaseInfo/SupplierChoose.html?supplierID=" + txtSupplierID.getValue(),
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
                txt.setValue(data.SupplierID);
                txt.setText(data.Name);
                txt.doValueChanged(e);
            }
        }
    });
}