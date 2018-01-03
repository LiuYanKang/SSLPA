var form;
var cboStockType;

$.page_load = function () {

    mini.parse();

    form = new mini.Form("form1");

    cboStockType = mini.get("cboStockType");
    $.dic("1003", function (e) {
        cboStockType.set({ data: e });
    });

    var reqStr = $.request("id");
    if (reqStr) {       //修改
        setData(reqStr);
    }

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
    $.srv("BaseInfo", $.request("id") ? "StockLocationSave" : "StockLocationAdd", {
        data: { model: obj },
        success: function (data) {
            CloseWindow("save");
        }
    });
}

function getData() {

    var obj = form.getData();
    if (obj.LocID == "") {
        obj.LocID = null;
    }
    return obj;
}


function setData(id) {
    $.srv("BaseInfo", "StockLocationGet", {
        data: { id: id },
        success: function (result) {
            var obj = result.Data;
            form.setData(obj);
            form.setChanged(false);
        }
    });
}

