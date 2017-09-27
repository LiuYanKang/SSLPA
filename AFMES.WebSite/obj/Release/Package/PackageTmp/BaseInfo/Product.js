
var datagrid;

$.page_load = function () {
    datagrid = mini.get("datagrid");
    search();
}



function datagrid_BeforeLoad(e) {
    e.cancel = true;
    loadProductList(e.params);
}


function loadProductList(e) {
    datagrid.loading();

    $.srv("BaseInfo", "ProductQuery", {
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
    var key = mini.get("keyword").getValue();
    datagrid.load({ Keyword: key });
}


//新增
function onAdd() {
    mini.open({
        title: "新增产品",
        url: bootPATH + "BaseInfo/ProductInfo.html",
        width: 690,
        height: 350,
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
        mini.showTips({ content: "请选择要编辑的产品", state: "danger" });
        return;
    }

    mini.open({
        title: "编辑产品",
        url: bootPATH + "BaseInfo/ProductInfo.html?id=" + row.ProdID,
        width: 690,
        height: 350,
        ondestroy: function (aciton) {
            if (aciton != "save") return;
            mini.showTips({ content: "修改成功", state: "success" });
            search();
        }
    });
};
//删除
function onRemove() {
    var row = datagrid.getSelected();
    if (!row) {
        mini.showTips({ content: "请选择要删除的产品", state: "danger" });
        return;
    };
    mini.confirm("确定删除 <i class='icon-quote-left'></i> <span class='red'>" + row.Code + "</span> <i class='icon-quote-right'></i> ？", "确定？",
        function (action) {
            if (action != "ok") return;
            $.srv("BaseInfo", "ProductDel", {
                data: { id: row.ProdID },
                success: function (result) {
                    if (result.Data) {
                        mini.showTips({ content: "删除成功", state: "success" });
                        search();
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

function onProductRenderer(e) {
    return "<a href='javascript:onQuery(" + e.row.ProdID + ")' title='点击查看产品'>" + e.row.Code + "</a>";
}

function onQuery(id) {
    mini.open({
        title: "产品详情",
        url: bootPATH + "BaseInfo/ProductView.html?prodID=" + id,
        width: 700,
        height: 360,
        ondestroy: function (action) {
            if (action != "save") return;
            search();
        }
    });
}
