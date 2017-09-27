var tree;

$.page_load = function () {
    tree = mini.get("tree1");
    loadTree();
};


function getData() {
    var node = tree.getSelectedNode();
    return node;
}function setData(deptId) {
    tree.findNodes(function (node) {
        if (node.DeptID == deptId) {
            tree.selectNode(node);
        }
    });
}

//加载部门
function loadTree(key) {
    $.srv("Base", "MDepartmentAll", {
        data: { key: key },
        success: function (result) {
            tree.loadList(result.Data);
        }
    });

}

//查询
function search() {
    var key = mini.get("keyword").getValue();
    loadTree(key);
};



function CloseWindow(action) {
    if (window.CloseOwnerWindow) return window.CloseOwnerWindow(action);
    else window.close();
};

function onOk() {
    CloseWindow("ok");
};

function onCancel() {
    CloseWindow("cancel");
};
