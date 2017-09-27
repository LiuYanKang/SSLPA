var form;
var cboGender, cboMgrType, cboStation;
var txtDepartmentId;
var sPDCode;

$.page_load = function () {

    mini.parse();

    form = new mini.Form("form1");
    cboGender = mini.get("cboGender");
    txtDepartmentId = mini.get("txtDepartmentId");
    // 获取性别字典项目
    $.dic("1015", function (e) {
        cboGender.set({ data: e });
    });
    sPDCode = mini.get("sPDCode");

    // 产品部门
    $.srv("BaseInfo", "ProDept", {
        success: function (data) {
            sPDCode.set({ data: data.Data });
        }
    });


    var reqStr = $.request("empID");
    if (reqStr) {       //修改
        setData(reqStr);
    }


    initUploadImg();
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
    obj.PhotoFile = $('#hidImg').val();
    form.validate();
    if (form.isValid() == false) return;

    $.srv("Base", $.request("empID") ? "EmployeeSave" : "EmployeeAdd", {
        data: { model: obj },
        success: function (data) {
            CloseWindow("save");
        }
    });
}

function getData() {

    var obj = form.getData();
    if (obj.EmpID == "") obj.EmpID = null;
    if (obj.DeptID == "") obj.DeptID = null;
    return obj;
}



function setData(id) {
    $.srv("Base", "EmployeeGet", {
        data: { id: id },
        success: function (result) {
            var obj = result.Data;
            form.setData(obj);
            txtDepartmentId.setValue(obj.DeptID);
            txtDepartmentId.setText(obj.DepartmentName);
            if (!obj.EmpCode || obj.EmpCode == "") {
                $('#imgUpload').attr('src', SS.config.empHeaderPath + 'default.jpg');
            } else {
                $('#imgUpload').attr('src', SS.config.empHeaderPath + obj.EmpCode + '.jpg?t=' + new Date().getMilliseconds());
            }
            form.setChanged(false);
        }
    });
}

function onDepartmentId(e) {
    var txt = e.sender;
    mini.open({
        title: "请选择所属部门...",
        url: bootPATH + "Base/DepartmentChoose.html",
        width: 400,
        height: 450,
        onload: function () {
            var iframe = this.getIFrameEl();
            var deptId = txtDepartmentId.getValue();
            iframe.contentWindow.setData(deptId);
        },
        ondestroy: function (action) {
            if (action == "ok") {
                var iframe = this.getIFrameEl();
                var data = iframe.contentWindow.getData();
                data = mini.clone(data);
                if (data) {
                    txt.setValue(data.DeptID);
                    txt.setText(data.Name);
                }
            }
        }
    });
}




// 初始化上传组件
function initUploadImg() {

    // 初始化Web Uploader
    var uploader = WebUploader.create({
        // 选完文件后，是否自动上传。
        auto: true,
        fileNumLimit: 1,
        // swf文件路径

        swf: '../lib/webloader/Uploader.swf',
        // 文件接收服务端。
        server: '../Upload/upload.ashx',
        // 选择文件的按钮。可选。
        // 内部根据当前运行是创建，可能是input元素，也可能是flash.
        pick: {
            id: '#filePicker',
            multiple: false
        },
        // 只允许选择图片文件。
        accept: {
            title: 'Images',
            extensions: 'jpg,jpeg',
            mimeTypes: 'image/*'
        }
    });
    // 当有文件添加进来的时候
    uploader.on('fileQueued', function (file) {
        var $img = $('#imgUpload');

        // 创建缩略图
        // 如果为非图片文件，可以不用调用此方法。
        // thumbnailWidth x thumbnailHeight 为 100 x 100
        uploader.makeThumb(file, function (error, src) {
            if (error) {
                $img.replaceWith('<span>不能预览</span>');
                return;
            }

            $img.attr('src', src);
        }, 260, 390);
    });

    // 文件上传失败，显示上传出错。
    uploader.on('uploadError', function (file) {
        mini.alert("上传失败");
    });

    // 完成一个文件的上传，得到服务器返回的服务器ID
    uploader.on('uploadSuccess', function (file, response) {
        $('#hidImg').val(response._raw);
        mini.alert('图片上传成功');
    });
}