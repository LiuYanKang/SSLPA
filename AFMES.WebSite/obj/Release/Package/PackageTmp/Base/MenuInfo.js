
var form;
var PCode = $.request("PCode");
var MenuCode = $.request("MenuCode");
var action = $.request("action");
$.page_load = function () {
    mini.parse();
    form = new mini.Form("editform");
    if (action == "query") {
        query(MenuCode);
    };
};

function onInsertOrUpdate() {
    var obj = getData();
    var NameMenth;
    if (action == "insert") {
        obj.PCode = PCode;
        NameMenth = "MenuSave";
    };
    if (action == "update") {
        obj.PCode = PCode;
        obj.MenuCode = MenuCode;
        NameMenth = "MenuAdd";
    };
    if (action == "query") {
        NameMenth = "MenuUpdate";
    };
    $.srv("Base", NameMenth, {
        data: { model: obj },
        success: function (data) {
            CloseWindow("save");
        }
    });
}

function query(id) {
    $.srv("Base", "MentGetEntity", {
        data: { MenuCode: id },
        success: function (result) {
            var obj = result.Data;        
            form.setData(obj);
        }
    });
}

function getData() {
    var obj = form.getData();
    return obj;
}

function onCancel(e) {
    CloseWindow("cancel");
}