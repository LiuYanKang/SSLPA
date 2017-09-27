var treegrid;

$.page_load = function () {
    treegrid = mini.get("treegrid1");
    TreegridLoadList();
}


function TreegridLoadList(e) {

    $.srv("BaseInfo", "DownCodeQuery", {
        data: { pagerParams: e },
        success: function (result) {
            var menuData = result.Data;

            // 加载停机树菜单
            $(menuData).each(function () {
                this.text = this.Summary;
            });
            treegrid.loadList(menuData);
        }
    });
}


//新增
function onAdd() {
    var nodeID;
    var node = treegrid.getSelectedNode();
    if (!node) {
        nodeID = -1;
    } else {
        nodeID = node.DownCodeID;
    }

    mini.open({
        title: "新增停机",
        url: bootPATH + "BaseInfo/DownCodeInfo.html?pid=" + nodeID,
        width: 645,
        height: 270,
        ondestroy: function (aciton) {
            if (aciton != "save") return;
            mini.showTips({ content: "新增成功", state: "success" });
            TreegridLoadList();
        }
    });
};

//编辑
function onEdit() {
    var node = treegrid.getSelectedNode();
    if (!node) {
        mini.showTips({ content: "请选择要编辑的停机", state: "danger" });
        return;
    }

    mini.open({
        title: "编辑停机",
        url: bootPATH + "BaseInfo/DownCodeInfo.html?id=" + node.DownCodeID,
        width: 645,
        height: 270,
        ondestroy: function (aciton) {
            if (aciton != "save") return;
            mini.showTips({ content: "修改成功", state: "success" });
            TreegridLoadList();
        }
    });
};

//删除
function onRemove() {
    var node = treegrid.getSelectedNode();
    if (!node) {
        mini.showTips({ content: "请选择要删除的任务名称", state: "danger" });
        return;
    };

    mini.confirm("确定删除 <i class='icon-quote-left'></i> <span class='red'>" + node.Summary + "</span> <i class='icon-quote-right'></i> ？", "确定？",
        function (action) {
            if (action != "ok") return;
            $.srv("BaseInfo", "DownCodeDel", {
                data: { id: node.DownCodeID },
                success: function (result) {
                    if (result.Data) {
                        mini.showTips({ content: "删除成功", state: "success" });
                        TreegridLoadList();
                    }
                }
            });
        });
};

