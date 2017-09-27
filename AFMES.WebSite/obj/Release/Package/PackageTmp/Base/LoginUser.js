var form;

$.page_load = function () {

    mini.parse();

    form = new mini.Form("form1");

    var reqStr = $.request("empId");
    mini.get("empID").setValue(reqStr);

};

function onOk(e) {
    saveData();
}

function onCancel(e) {
    CloseWindow("cancel");
}

//保存
function saveData() {
    var obj = getData();
    form.validate();
    if (form.isValid() == false) return;

    $.srv("Base", "LoginUserAdd", {
        data: { model: obj },
        success: function (data) {
            CloseWindow("save");
        }
    });
}

function getData() {

    var obj = form.getData();
    if (obj.UserID == "") obj.UserID = null;

    return obj;
}
