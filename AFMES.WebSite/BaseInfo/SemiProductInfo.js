var form;
var txtSupplierID, txtRawMaterialID;
var cboStrength;

$.page_load = function () {

    mini.parse();

    form = new mini.Form("form1");
    txtSupplierID = mini.get("txtSupplierID");
    txtRawMaterialID = mini.get("txtRawMaterialID");

    cboStrength = mini.get("cboStrength");
    $.dic("1006", function (e) {
        cboStrength.set({ data: e });
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
    if (parseInt(obj.MinStorage) >= parseInt(obj.MaxStorage)) {
        mini.showTips({ content: "最小库存不能大于最大库存！", state: "danger" });
        return;
    }
    form.validate();
    if (form.isValid() == false) return;
    $.srv("BaseInfo", $.request("id") ? "SemiProductSave" : "SemiProductAdd", {
        data: { model: obj },
        success: function (data) {
            CloseWindow("save");
        }
    });
}

function getData() {

    var obj = form.getData();
    if (obj.SemiProdID == "") {
        obj.SemiProdID = null;
    }
    if (obj.RawMaterialID == "") {
        obj.RawMaterialID = null;
    }
    return obj;
}


function setData(id) {
    $.srv("BaseInfo", "SemiProductGet", {
        data: { id: id },
        success: function (result) {
            var obj = result.Data;
            form.setData(obj);
            txtSupplierID.setText(obj.SupplierName);
            txtSupplierID.setValue(obj.SupplierID);
            txtRawMaterialID.setText(obj.MaterialCode);
            txtRawMaterialID.setValue(obj.RawMaterialID);
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


function onChooseRawMaterial(e) {
    var txt = e.sender;

    mini.open({
        title: "请选择原材料...",
        url: bootPATH + "BaseInfo/RawMaterialChoose.html?rawMaterialID=" + txt.getValue(),
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
                txt.setValue(data.RawMaterialID);
                txt.setText(data.Code);
                txt.doValueChanged(e);
            }
        }
    });
}
