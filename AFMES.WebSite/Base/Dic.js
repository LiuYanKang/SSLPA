
var dgl, dgr;

$.page_load = function () {
    dgl = mini.get("dgleft");
    dgr = mini.get("dgright");
    search();
}

function dg_BeforeLoad(e) {
    e.cancel = true;
    loadLeftList(e.params);
}


function loadLeftList(e) {                        //加载左边的DataGrid 菜单
    dgl.loading();

    $.srv("Base", "DicAll", {
        data: { pagerParams: e },
        success: function (result) {
            dgl.set({
                data: result.Data                   //如果没有分页效果的话，Data后面就不用加 .Data 
            });

            if (result.Data.length > 0)
                dgl.setSelected(dgl.data[0]);       //默认选择左边DataGird 第一行
        }
    });
}


function dgl_selectionchanged() {                       //点击左边的DataGrid 显示右边的DataGrid 菜单
    var row = dgl.getSelected();
    var id = row.DicCode;                       //此DicCode为数据库中的ID值

    dgr.load({ dicCode: id });
}

function dgr_BeforeLoad(e) {
    e.cancel = true;
    loadRightList(e.params);
}

function loadRightList(e) {
    dgr.loading();

    $.srv("Base", "QueryDicItems", {
        data: e,                                     //如果是没有分页的，直接传 e 
        success: function (result) {
            dgr.set({
                data: result.Data
            });
        }
    });
}

function onRemove(e) {
    var row = dgr.getSelected();
    if (!row) {
        mini.showTips({ content: "请选择要删除的内容", state: "danger" });
        return;
    }

    var state = row._state;                           //此_state 为调试中获取到的  _state              如果是新增入的则直接删除页面上的行，而不是删除数据库里的行
    if (state == "added") {                           //此added也为调试中 获取到的   added
        dgr.removeRow(row, true);
        return;
    }

    mini.confirm("确定删除 <i class='icon-quote-left'></i> <span class='red'>" + row.Name + "</span> <i class='icon-quote-right'></i> ？", "确定？",
        function (action) {
            if (action != "ok") return;

            $.srv("Base", "DelDicItem", {
                data: { dicCode: row.DicCode, code: row.Code },
                success: function (result) {
                    mini.showTips({ content: "删除成功", state: "success" });

                    dgr.load({ dicCode: row.DicCode });
                }
            });
        });
}


function onIsSysRender(e) {
    if (e.value == true) {
        return "√";
    }
    else {
        return "";
    }
}


function onAdd() {                              //新增
    var row = dgl.getSelected();
    if (!row) return;
    var newRow = { DicCode: row.DicCode, Code: "", Name: "", IsSys: false, SN: 99, Remark: "" };                //新增一行之后，首先得获取主表的ID，此处为DicCOde
    dgr.addRow(newRow, 0);

    dgr.beginEditCell(newRow, "code");
}


function dgr_cellendedit(e) {
    var data = dgr.getChanges();                 //获取所有编辑过的行
    if (data.length == 0) return;

    var code = data[0].Code;
    var name = data[0].Name;
    if (code == "") {
        mini.showTips({ content: "代码不能为空", state: "danger" });
        return;
    }
    else if (name == "") {
        mini.showTips({ content: "名称不能为空", state: "danger" });
        return;
    }


    $.srv("Base", "SaveDicItem", {
        data: { model: data[0] },                                                             // SaveDicItem 返回的是一个bool类型的，用数组data[0]
        success: function (result) {
            mini.showTips({ content: "操作成功", state: "success" });

            var row = dgl.getSelected();
            dgr.load({ dicCode: row.DicCode });
        }
    });
}


function OnCellBeginEdit(e) {
    var record = e.record, field = e.field;

    if (field == "Code") {
        var state = record._state;
        if (state == "added") {
            e.cancel = false;
        }
        else {
            e.cancel = true;
        }
    }

}


function search() {
    var key = mini.get("keyword").getValue();
    dgl.load({ Keyword: key });
}