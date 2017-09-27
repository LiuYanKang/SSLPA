// 扩展mini对象
jQuery.extend(mini, {
    // 获取TextBoxList数据
    getTextBoxListData: function (txtLst) {
        var selected = [];
        var selVal = txtLst.getValue().split(',');
        var selText = txtLst.getText().split(',');
        for (var i = 0; i < selVal.length; i++) {
            if (!selVal[i]) break; // 跳过空值
            var obj = {};
            obj[txtLst.textField] = selText[i];
            obj[txtLst.valueField] = selVal[i];
            selected.push(obj);
        }
        return selected;
    },
    // 获取TextBoxList数据
    getTextBoxListDistinctData: function (txtLst) {
        var selected = [];
        var selVal = txtLst.getValue().split(',');
        var selText = txtLst.getText().split(',');
        for (var i = 0; i < selVal.length; i++) {
            if (!selVal[i]) break; // 跳过空值
            var isExists = false;
            // 判断是否已存在
            for (var idx = 0; idx < selected.length; idx++) {
                if (selected[idx][txtLst.valueField] == selVal[i]) {
                    isExists = true; break;
                }
            }
            if (isExists) continue;
            var obj = {};
            obj[txtLst.textField] = selText[i];
            obj[txtLst.valueField] = selVal[i];
            selected.push(obj);
        }
        return selected;
    },
    // 设置TextBoxList数据源
    setTextBoxListData: function (txtLst, selected) {
        var name = '', id = '';
        $(selected).each(function () {
            name += this[txtLst.textField] + ',';
            id += this[txtLst.valueField] + ',';
        });
        name = name.substr(0, name.length - 1);
        id = id.substr(0, id.length - 1);
        txtLst.setValue(id);
        txtLst.setText(name);
    },
    // 文件上传
    onFileSelect: function (e) {
        if (e.file.size > (30 * 1024 * 1024)) {
            mini.alert("文件最大30MB");
            return;
        }
        this.startUpload();
    },
    // 文件上传报错
    onUploadError: function (e) {
        mini.alert("错误信息:" + e.message + "( " + e.code + " )", "上传失败");
    },
    // 文件上传完成
    onUploadComplete: function (e) {
        //alert(e);
    },
    // 渲染日期
    onDateRender: function (e) {
        return mini.formatDate($.wcfDate2JsDate(e.value), "yyyy-MM-dd");
    },
    // 渲染日期时间
    onDateTimeRender: function (e) {
        return mini.formatDate($.wcfDate2JsDate(e.value), "yyyy-MM-dd HH:mm:ss");
    },
    // 渲染时间
    onTimeRender: function (e) {
        return mini.formatDate($.wcfDate2JsDate(e.value), "HH:mm:ss");
    },
    // 渲染时间,不要秒
    onTime2Render: function (e) {
        return mini.formatDate($.wcfDate2JsDate(e.value), "HH:mm");
    },
    // 渲染日期时间不要秒
    onDateTime2Render: function (e) {
        return mini.formatDate($.wcfDate2JsDate(e.value), "yyyy-MM-dd HH:mm");
    },
    //转换文件大小
    onFileSizeRender: function (e) {
        return SS.formateFileSize(e.value);
    },
    // 文件上传报错
    onUploadError: function (e) {
        mini.alert("错误信息:" + e.message + "( " + e.code + " )", "上传失败");
    },
    // 弹出 部门选择框 //<input id="txtPos" name="PosID" class="mini-buttonedit" required="true" style="width: 350px;" emptytext="请选择部门..." onbuttonclick="mini.onSelPos" selectonfocus="true" allowinput="false" />
    onSelOrg: function (e) {
        var txt = e.sender;
        mini.open({
            title: "请选择部门...",
            url: bootPATH + "BaseInfo/OrgSel.html?id=" + txt.getValue(),
            width: 300,
            height: 400,
            ondestroy: function (action) {
                if (action != "ok") return;

                var iframe = this.getIFrameEl();
                var data = iframe.contentWindow.getData();
                if (data.length < 1) return false;
                data = mini.clone(data);
                txt.setValue(data.OrgCode);
                txt.setText(data.Name);
                txt.data = data;
                txt.doValueChanged();
            }
        });
    },
    // 弹出 项目选择框 //<input id="txtProj" class="mini-buttonedit" emptytext="Select project name..." style="width: 200px;" onbuttonclick="mini.onSelCust" selectonfocus="true" allowinput="false" />
    onSelProj: function (e) {
        var txt = e.sender;
        mini.open({
            title: "请选择项目名称",
            url: bootPATH + "Houses/ProjSel.html?multiSelect=0",
            width: 500,
            height: 400,
            onload: function () {
                var iframe = this.getIFrameEl();
                var selected = { ProjID: txt.getValue(), Name: txt.getText() };
                iframe.contentWindow.setData(selected);
            },
            ondestroy: function (action) {
                if (action == "ok") {
                    var iframe = this.getIFrameEl();
                    var data = iframe.contentWindow.getData();
                    if (data.length < 1) return false;
                    data = mini.clone(data[0]);
                    txt.setValue(data.ProjID);
                    txt.setText(data.Name);
                    txt.doValueChanged(e);
                }
            }
        });
    },

    // 点击关闭按钮时
    onCloseClick: function (e) {
        var txt = e.sender;
        txt.setValue(null);
        txt.setText(null);
        txt.doValueChanged(e);
    },
    // 注册ESC按键
    regESC: function () {
        document.onkeyup = function (e) {
            var currKey = 0, e = e || event;
            currKey = e.keyCode || e.which || e.charCode;
            if (currKey == 27) {
                CloseWindow("close");
            }
        };
    }
});

// 关闭窗口
function CloseWindow(action) {
    if (action == "close" && form.isChanged()) {
        mini.confirm("数据已经被修改，是否要保存？", "确认退出",
            function (action) {
                if (action != "ok") {
                    if (window.CloseOwnerWindow) return window.CloseOwnerWindow(action);
                    else window.close();
                }
            });
        return false;
    }
    if (window.CloseOwnerWindow)
        return window.CloseOwnerWindow(action);
    else
        window.close();
}