var datagrid;

$.page_load = function () {

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

//新增
function onAdd() {
    mini.open({
        title: "新增EHS",
        url: bootPATH + "BaseInfo/EHSInfo.html",
        width: 600,
        height: 350,
        ondestroy: function (aciton) {
            if (aciton != "save") return;
            search();
        }
    });
};

//编辑
function onEdit() {
    var row = datagrid.getSelected();
    if (!row) {
        mini.showTips({ content: "请选择要编辑的项", state: "danger" });
        return;
    }

    mini.open({
        title: "编辑EHS",
        url: bootPATH + "BaseInfo/EHSInfo.html?id=" + row.EHSID,
        width: 600,
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
    var rows = getMServiceIDList();
    if (!rows) {
        mini.showTips({ content: "请选择要删除的内容", state: "danger" });
        return;
    };
    mini.confirm("确定删除？", "确定？",
        function (action) {
            if (action != "ok") return;
            $.srv("BaseInfo", "EHSDel", {
                data: { ids: rows },
                success: function (result) {
                    mini.showTips({ content: "删除成功", state: "success" });
                    search();
                }
            });
        });
};
//获得选中项编码集合
function getMServiceIDList() {
    var mServiceID = undefined;
    //获取选中项对象集合
    var entityList = datagrid.getSelecteds();
    //判断对象集合有值
    if (entityList && entityList.length > 0) {
        mServiceID = [];
        for (var i = 0; i < entityList.length; i++) {
            mServiceID.push(entityList[i].EHSID);
        };
    };
    return mServiceID;
};


//显示小图标
function onImg(e) {
    if (!e.value) return "未上传图片";
    return "<img src=\"" + SS.config.ehsPicPath + e.value + "\" style=\"height:100px; \"> ";
}



