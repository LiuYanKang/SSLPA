var form;
var txtCustID, txtSemiProdID, txtProdAbr;
var arry = [];
var ss = "";
var codeList = [];

$.page_load = function () {

    mini.parse();

    form = new mini.Form("form1");
    txtCustID = mini.get("txtCustID");
    txtSemiProdID = mini.get("txtSemiProdID");
    txtProdAbr = mini.get("txtProdAbr");

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
    $.srv("BaseInfo", $.request("id") ? "ProductSave" : "ProductAdd", {
        data: { model: obj },
        success: function (data) {
            CloseWindow("save");
        }
    });
}

function getData() {

    var obj = form.getData();
    if (txtProdAbr.getValue() == "") {
        obj.AbrasivesID = [];
    } else {
        obj.AbrasivesID = txtProdAbr.getValue().split(',');
    }
    if (obj.ProdID == "") {
        obj.ProdID = null;
    }
    return obj;
}


function setData(id) {
    $.srv("BaseInfo", "ProductGet", {
        data: { id: id },
        success: function (result) {
            var obj = result.Data;
            form.setData(obj);
            txtCustID.setText(obj.CustomerName);
            txtCustID.setValue(obj.CustID);
            for (var i = 0; i < obj.SemiProduct.length; i++) {
                arry.push(obj.SemiProduct[i].SemiProdID);
                codeList.push(obj.SemiProduct[i].Code);
                ss += obj.SemiProduct[i].Code;
                if (i < obj.SemiProduct.length - 1) {
                    ss += ",";
                }
            }

            txtSemiProdID.setText(ss);
            txtSemiProdID.setValue(arry);
            // 工装
            var strAbrArr = "", strAbrIdArr = "";
            for (var i = 0; i < obj.Abrasives.length; i++) {
                strAbrArr += obj.Abrasives[i].Name;
                strAbrIdArr += obj.Abrasives[i].ABRID;
                if (i < obj.Abrasives.length - 1) {
                    strAbrArr += ",";
                    strAbrIdArr += ",";
                }
            }
            txtProdAbr.setValue(strAbrIdArr);
            txtProdAbr.setText(strAbrArr);
            form.setChanged(false);
        }
    });
}



function onChooseCustomer(e) {
    var txt = e.sender;

    mini.open({
        title: "请选择客户...",
        url: bootPATH + "BaseInfo/CustomerChoose.html?customerID=" + txtCustID.getValue(),
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
                txt.setValue(data.CustID);
                txt.setText(data.Name);
                txt.doValueChanged(e);
            }
        }
    });
}

function onChooseSemiProd(e) {
    var txt = e.sender;

    mini.open({
        title: "请选择半成品...",
        url: bootPATH + "BaseInfo/SemiProductChoose.html?multiSelect=1",
        width: 520,
        height: 440,
        onload: function () {
            var iframe = this.getIFrameEl();
            var selecteds = [];
            for (var i = 0; i < arry.length; i++) {
                selecteds.push({ SemiProdID: arry[i], Code: codeList[i] });
            }
            iframe.contentWindow.setData(selecteds);
        },
        ondestroy: function (action) {
            if (action == "ok") {
                var iframe = this.getIFrameEl();
                var data = iframe.contentWindow.getData();
                if (data.length < 1) return false;
                data = mini.clone(data);
                arry = [];
                codeList = [];
                ss = "";
                for (var i = 0; i < data.length; i++) {
                    arry.push(data[i].SemiProdID);
                    codeList.push(data[i].Code);
                    ss += data[i].Code;
                    if (i < data.length - 1) {
                        ss += ",";
                    }
                }

                txt.setValue(arry);
                txt.setText(ss);
                txt.doValueChanged(e);
            }
        }
    });
}
