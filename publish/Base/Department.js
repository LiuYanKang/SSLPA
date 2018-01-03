var form;
var txtManagerIdID;

$.page_load = function () {
    mini.parse();
    form = new mini.Form("form1");
    txtManagerIdID = mini.get("txtManagerIdID");
    var pid = $.request("pid");
    var pname = unescape($.request("pname"));
    var deptId = $.request("DeptID");
    if (deptId) {
        setData(deptId);
    }
    if (pid) {
        mini.get("txtParentName").disable();
        mini.get("txtParentName").setValue(pname);
        mini.get("PID").setValue(pid);       
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
    txtManagerIdID.disable();
    $.srv("Base", $.request("DeptID") ? "DepartmentSave" : "DepartmentAdd", {
        data: { model: obj },
        success: function (data) {
            CloseWindow("save");
        }
    });
}

function getData() {
    var obj = form.getData();
    if (obj.DeptID == "") obj.DeptID = null;
    if (obj.ManagerId == "") obj.ManagerId = null;
    if (obj.PID == "") obj.PID = null;
    return obj;
}



function setData(id) {
    $.srv("Base", "DepartmentGet", {
        data: { id: id },
        success: function (result) {
            var obj = result.Data;
            form.setData(obj);
            mini.get("txtParentName").disable();
            txtManagerIdID.setValue(obj.ManagerId);
            txtManagerIdID.setText(obj.ManagerName);
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
            }
        }
    });
}