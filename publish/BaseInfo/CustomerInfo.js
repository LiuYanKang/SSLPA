var form;

$.page_load = function () {
    mini.regESC();
    mini.parse();

    form = new mini.Form("form1");

    var reqStr = $.request("id");
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
    form.validate();
    if (form.isValid() == false) return;
    $.srv("BaseInfo", $.request("id") ? "CustomerSave" : "CustomerAdd", {
        data: { model: obj },
        success: function (data) {
            CloseWindow("save");
        },
        exception: function (e) {
            mini.get("btnOk").enable();
        }
    });
}

function getData() {

    var obj = form.getData();

    obj.Logo = $('#hidImg').val();
    if (obj.Logo == "") obj.Logo = null;
    if (obj.CustID == "") {
        obj.CustID = null;

    }
    return obj;
}


function setData(id) {
    $.srv("BaseInfo", "CustomerGet", {
        data: { id: id },
        success: function (result) {
            var obj = result.Data;

            $("#imgUpload").attr("src", SS.config.customerLogoPath + obj.Logo);

            form.setData(obj);
            form.setChanged(false);
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
        server: 'upload.ashx?type=1',
        // 选择文件的按钮。可选。
        // 内部根据当前运行是创建，可能是input元素，也可能是flash.
        pick: {
            id: "#filePicker",
            multiple: false
        },
        // 只允许选择图片文件。
        accept: {
            title: 'Images',
            extensions: 'gif,jpg,jpeg,bmp,png',
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
        }, 100, 100);
    });

    // 文件上传失败，显示上传出错。
    uploader.on('uploadError', function (file) {
        alert("上传失败");
    });

    // 完成一个文件的上传，得到服务器返回的服务器ID
    uploader.on('uploadSuccess', function (file, response) {
        $('#hidImg').val(response._raw);
    });
}