
var form;
var returnData;// 用于返回的数据对象

$.page_load = function () {

    form = new mini.Form("form1");

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
    obj.FileName = $('#hidImg').val();
    returnData = obj;

    CloseWindow("save");
}

function getData() {

    var obj = form.getData();
    if (obj.PicID == "") {
        obj.PicID = null;
    }
    return obj;
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
        server: '../BaseInfo/upload.ashx',
        // 选择文件的按钮。可选。
        // 内部根据当前运行是创建，可能是input元素，也可能是flash.
        pick: {
            id: '#filePicker',
            multiple: true
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
        }, 720, 385);
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