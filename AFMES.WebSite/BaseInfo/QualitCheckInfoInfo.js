var form;
var cboQualitCheckType;
var materialID, procID, materialtype;
var txtMaxValue, txtMinValue;

$.page_load = function () {

    mini.parse();

    form = new mini.Form("form1");
    txtMinValue = mini.get("txtMinValue");
    txtMaxValue = mini.get("txtMaxValue");

    var pn = $.request("materialCode");     //物料号
    if (pn) {
        $("#lblPN").html(pn);
    }
    var pName = $.request("procName");
    if (pName) {
        $("#lblProcName").html(pName);
    }

    materialtype = $.request("materialtype");

    var reqStr = $.request("qCInfoID");
    if (reqStr) {       //修改
        setData(reqStr);
    } else {            //新增
        materialID = $.request("materialID");         //接收的物料ID
        if (!materialID) { alert("必须传入materialID"); return; }
        mini.get("hidItemID").setValue(materialID);     //物料ID

        procID = $.request("procID");
        mini.get("hidProcessID").setValue(procID);             //工序ID
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
    obj.ItemType = materialtype;    //物料类型

    form.validate();
    if (form.isValid() == false) return;

    if (txtMinValue.getValue() != "" || txtMinValue.getValue() != null || txtMaxValue.getValue() != "" || txtMaxValue.getValue() != null) {
        if (parseFloat(txtMinValue.getValue()) > parseFloat(txtMaxValue.getValue())) {
            mini.alert("最小值不能超过最大值，请重新输入!")
            return;
        }
    }

    $.srv("BaseInfo", $.request("qCInfoID") ? "QualitCheckSave" : "QualitCheckAdd", {
        data: { model: obj },
        success: function (data) {
            CloseWindow("save");
        }
    });
}

function getData() {

    var obj = form.getData();
    if (obj.QCInfoID == "") {
        obj.QCInfoID = null;
    }
    if (obj.MinValue == "") {       //最小值
        obj.MinValue = null;
    }
    if (obj.MaxValue == "") {       //最大值
        obj.MaxValue = null;
    }
    return obj;
}


function setData(id) {
    $.srv("BaseInfo", "QualitCheckGet", {
        data: { id: id },
        success: function (result) {
            var obj = result.Data;
            if (obj.InfoType == "2") {
                $("#hidZhi").hide();
            }
            form.setData(obj);
            form.setChanged(false);
        }
    });
}

function onValuechanged(e) {
    if (e.value == 2) {
        $("#hidZhi").hide();
    } else {
        $("#hidZhi").show();
    }
}


function onMinMaxChanged(e) {
    if (txtMinValue.getValue() != "" || txtMinValue.getValue() != null || txtMaxValue.getValue() != "" || txtMaxValue.getValue() != null) {
        if (parseFloat(txtMinValue.getValue()) > parseFloat(txtMaxValue.getValue())) {
            mini.alert("最小值不能超过最大值，请重新输入!")
            return;
        }
    }
}