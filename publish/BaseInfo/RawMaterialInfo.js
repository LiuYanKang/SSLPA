var form;
var txtSupplierID;

$.page_load = function () {

    mini.parse();

    form = new mini.Form("form1");
    txtSupplierID = mini.get("txtSupplierID");

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
    if (parseFloat(obj.MinStorage) > parseFloat(obj.MaxStorage)) {
        mini.showTips({ content: "最小库存不能大于最大库存！", state: "danger" });
        return;
    }
    form.validate();
    if (form.isValid() == false) return;
    $.srv("BaseInfo", $.request("id") ? "RawMaterialSave" : "RawMaterialAdd", {
        data: { model: obj },
        success: function (data) {
            CloseWindow("save");
        }
    });
}

function getData() {

    var obj = form.getData();
    if (obj.RawMaterialID == "") {
        obj.RawMaterialID = null;
    }
    return obj;
}


function setData(id) {
    $.srv("BaseInfo", "RawMaterialGet", {
        data: { id: id },
        success: function (result) {
            var obj = result.Data;
            form.setData(obj);
            txtSupplierID.setText(obj.SupplierName);
            txtSupplierID.setValue(obj.SupplierID);
            form.setChanged(false);
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
