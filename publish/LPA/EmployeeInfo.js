var form;
var reqStr;
var txtEmpID, cblArea, cboAuditType;

$.page_load = function () {

    form = new mini.Form("form1");
    txtEmpID = mini.get("txtEmpID");
    //txtSuperiorID = mini.get("txtSuperiorID");
    cblArea = mini.get("cblArea");
    cboAuditType = mini.get("cboAuditType");
   
    reqStr = $.request("empID");
    if (reqStr) {       //修改
        setData(reqStr);
        txtEmpID.disable();
    }
    if (!reqStr) {
        reqStr = 0;
    }
    // 审核区域
    $.srv("LPA", "AuditAreaGet", {
        data: { id: reqStr },
        success: function (data) {
            cblArea.set({ data: data.Data });
        }
    });
    $.dic("1024", function (e) {        //审核类型
        cboAuditType.set({ data: e });
    });
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

    $.srv("LPA", "EmployeeSave", {
        data: { model: obj },
        success: function (data) {
            CloseWindow("save");
        }
    });
}

function getData() {
    var obj = form.getData();
    if (obj.SuperiorID == "") {
        obj.SuperiorID = null;
    }

    obj.AuditArea = cblArea.getValue().split(',');

    return obj;
}


function setData(id) {
    $.srv("LPA", "EmployeeGet", {
        data: { id: id },
        success: function (result) {
            var obj = result.Data;
            form.setData(obj);
            txtEmpID.setText(obj.EmpName);
            txtEmpID.setValue(obj.EmpID);
            //txtSuperiorID.setText(obj.SuperiorName);
            //txtSuperiorID.setValue(obj.SuperiorID); 
            
            form.setChanged(false);

            cblArea.setValue(obj.AuditArea.join(','));

        }
    });
}


function onChooseEmp(e) {
    var txt = e.sender;

    mini.open({
        title: "请选择员工...",
        url: bootPATH + "LPA/ChooseEmployee.html?multiSelect=0",
        width: 500,
        height: 425,
        onload: function () {
            var iframe = this.getIFrameEl();
            var selected = { EmpID: txt.getValue(), Name: txt.getText() };
            iframe.contentWindow.setData(selected);
        },
        ondestroy: function (action) {
            if (action == "ok") {
                var iframe = this.getIFrameEl();
                var data = iframe.contentWindow.getData();
                if (data.length < 1) return false;
                data = mini.clone(data[0]);
                txt.setValue(data.EmpID);
                txt.setText(data.Name);
                txt.doValueChanged(e);
                $.srv("LPA", "AuditAreaGet", {
                    data: { id: data.EmpID },
                    success: function (data) {
                        cblArea.set({ data: data.Data });
                    }
                });
            }
        }
    });
}

