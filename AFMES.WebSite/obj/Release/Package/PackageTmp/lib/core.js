jQuery.extend({
    // WCF时间转换为JS时间
    wcfDate2JsDate: function (value) {
        if (typeof value == "string" && value.indexOf("\/Date(") == 0) {
            var s = value.substring(6, value.length - 2);
            return new Date(parseInt(s));
        }
        return value;
    },
    // JS时间转换为WCF时间
    jsDate2WcfDate: function (jsDate) {
        if (!jsDate || !jsDate.getTime) return null;
        return "\/Date(" + jsDate.getTime() + "+0800)\/";
    },
    // 获取RequestString
    request: function (name) {
        var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)");
        var r = window.location.search.substr(1).match(reg);
        if (r != null) return unescape(r[2]); return null;
    },
    // Cookie操作
    cookie: function (name, value, options) {
        if (typeof value != 'undefined') {
            options = options || { expires: 3, path: '/' };
            if (value === null) {
                value = '';
                options = $.extend({}, options);
                options.expires = -1;
            }
            var expires = '';
            if (options.expires && (typeof options.expires == 'number' || options.expires.toUTCString)) {
                var date;
                if (typeof options.expires == 'number') {
                    date = new Date();
                    date.setTime(date.getTime() + (options.expires * 24 * 60 * 60 * 1000));
                } else {
                    date = options.expires;
                }
                expires = '; expires=' + date.toUTCString();
            }
            var path = options.path ? '; path=' + (options.path) : '';
            var domain = options.domain ? '; domain=' + (options.domain) : '';
            var secure = options.secure ? '; secure' : '';
            document.cookie = [name, '=', encodeURIComponent(value), expires, path, domain, secure].join('');
        } else {
            var cookieValue = null;
            if (document.cookie && document.cookie != '') {
                var cookies = document.cookie.split(';');
                for (var i = 0; i < cookies.length; i++) {
                    var cookie = jQuery.trim(cookies[i]);
                    if (cookie.substring(0, name.length + 1) == (name + '=')) {
                        cookieValue = decodeURIComponent(cookie.substring(name.length + 1));
                        break;
                    }
                }
            }
            return cookieValue;
        }
    },
    // 计算反色
    colorInvert: function (a) {
        a = a.replace('#', '');
        var c16, c10, max16 = 15, b = [];
        for (var i = 0; i < a.length; i++) {
            c16 = parseInt(a.charAt(i), 16);//  to 16进制 
            c10 = parseInt(max16 - c16, 10);// 10进制计算 
            b.push(c10.toString(16)); // to 16进制 
        }
        return '#' + b.join('');
    },
    // 页面初始化
    page_init: function () { return true },
    // 页面载入
    page_load: function () { return true },
    // 用户信息缓存
    __userinfo: null,
    // 用户信息
    userinfo: function (value) {
        if (typeof value != 'undefined') {
            this.__userinfo = value;
            this.cookie("userinfo", value ? JSON.stringify(value) : null);
        } else {
            if (!this.__userinfo) {
                this.__userinfo = JSON.parse(this.cookie("userinfo"));
            }
            return this.__userinfo;
        }
    },
    // 获取服务URL
    getSrvURL: function (srvName, methodName) {
        return bootPATH + "Srv/" + srvName + ".svc/" + methodName;
    },
    // 获取ajax设置
    getAjaxSetting: function (srvName, methodName, ajaxSetting) {
        ajaxSetting = ajaxSetting || {};
        ajaxSetting.data = ajaxSetting.data || {};
        // 自动带ticket
        if (!ajaxSetting.data.ticket && this.userinfo()) {
            ajaxSetting.data.ticket = this.userinfo().Ticket;
        }
        // ajax默认配置
        var ajax = {
            type: "POST"
            , url: this.getSrvURL(srvName, methodName)
            , dataType: "json"
            , contentType: "application/json; charset=utf-8"
            , cache: false
            , async: true
            , timeout: 30000
        };
        // 支持自定义部分ajax设置
        $.extend(ajax, ajaxSetting);
        // 框架优先控制的函数
        ajax.data = ajax.type.toUpperCase() == "GET" ? ajaxSetting.data : JSON.stringify(ajaxSetting.data);
        ajax.success = function (e) {
            if (e.d == undefined) return ajaxSetting.success(e);
            if (e.d.Status != 1) return;

            if (ajaxSetting.success)
                ajaxSetting.success(e.d);
        };
        ajax.complete = function (e) {
            var data;
            try {
                data = $.parseJSON(e.responseText);
            } catch (ex) {
                console.log(e);
                //alert(e.responseText);
                return;
            }
            if (!data) {
                if (ajaxSetting.exception
                    && ajaxSetting.exception({ Msg: e.responseText }) === false)
                    return;
                alert(e.responseText);
                return;
            }
            if (data.Message) {// 处理500错误
                if (ajaxSetting.exception
                    && ajaxSetting.exception({ Msg: data.Message }) === false)
                    return;
                alert(data.Message);
                return;
            }

            // 处理 正常业务
            data = data.d;
            // 优先调用自定义的complete
            if (ajaxSetting.complete
                && ajaxSetting.complete(data) === false) return;

            switch (data.Status) {
                case -2: // -2 没有权限        
                    alert("没有权限");
                    break;
                case -1:    // -1 未登入        
                    $.userinfo(null);
                    console.log("登陆超时");
                    $.goLogin(true);
                    break;
                case 0: // 0  处理业务异常
                    if (ajaxSetting.exception
                        && ajaxSetting.exception(data) === false)
                        return;
                    var msg = data.Msg || data;
                    if (msg.length > 500)
                        alert(msg);
                    else
                        alert(msg);
                    break;
                default: // 1  成功       
                    break;
            };
        };

        return ajax;
    },
    // 调用服务（服务名称，方法名称，自定义ajax配置{type,data,success,exception}）
    srv: function (srvName, methodName, ajaxSetting) {
        var ajaxSetting = this.getAjaxSetting(srvName, methodName, ajaxSetting);
        $.ajax(ajaxSetting);
    },
    // 用户校验
    userValidate: function () {
        // 匿名页面不需要校验
        // 根据BODY标签 是否有 anonymous 标签来决定
        if ($("body").attr("anonymous")) return true;
        var userinfo = this.userinfo();
        // 校验本地cookie
        if (!userinfo) {
            this.goLogin(true);
            return false;
        }
        var isLogin = true;
        // 校验本地的用户信息在服务端是否依然有效
        //$.srv("Base", "HasLogin"
        //    , {
        //        type: "GET"
        //        , async: false
        //        , success: function (data) {
        //            if (!data.Data) {// 未登录
        //                $.userinfo(null);
        //                $.goLogin(true);
        //                isLogin = false;
        //            }
        //            isLogin = true;
        //        }
        //    });
        // 控制界面按钮显示
        $("*[functioncode]").each(function () {
            var ele = $(this);
            var fcode = ele.attr("functioncode");
            if ($.inArray(fcode, userinfo.AuthList) == -1)
                ele.hide();
        });
        return isLogin;
    },
    // 进入登录页面
    goLogin: function (isBack) {
        var url = "login.html";
        if (isBack) {
            url += "?refurl=" + escape(location.href);
        }
        location.href = bootPATH + url;
    },
    // 注销登录
    logout: function () {
        if (!this.__userinfo) {
            $.goLogin();
        } else {
            this.srv("Base", "Logout", {
                success: function () {
                    $.userinfo(null);
                    $.goLogin();
                }
            });
        }
    },
    // 获取字典选项
    dic: function (code, callback) {
        $.srv("Base", "QueryDicItems", {
            data: { dicCode: code },
            success: function (result) {
                callback(result.Data);
            }
        });
    }
});

var SS = SS || {};
$.extend(SS, {
    // 判断是否手机客户端
    isMobile: function () {
        var u = navigator.userAgent;
        if (/iPad/i.test(u)) {
            return false;
        }
        return u.indexOf('Mobile') > -1;
    },
    // 判断当前浏览器是否为微信浏览器
    isWX: function () {
        return /MicroMessenger/i.test(navigator.userAgent);
    },
    // 微信OpenID
    __wxOpenID: null,
    // 根据微信OAuth身份获取OpenID
    wxOpenID: function (openCode, callback) {
        if (typeof openCode != 'undefined') {
            $.srv("Member", "GetOpenID"
                , {
                    data: { openCode: openCode }
                    , success: function (data) {
                        SS.__wxOpenID = data.Data;
                        $.cookie("wxopenid", data.Data);
                        if (callback) callback(data.Data);
                    }
                });
        } else {
            if (!this.__wxOpenID) {
                this.__wxOpenID = $.cookie("wxopenid");
            }
            return this.__wxOpenID;
        }
    },
    // 格式化文件大小
    formateFileSize: function (filesize) {
        var size = parseFloat(filesize);

        var fileSizeKB = Math.round(size / 1024 * 100) / 100.0;
        var fileSizeMB = Math.round(size / 1024 / 1024 * 100) / 100.0;
        var fileSizeGB = Math.round(size / 1024 / 1024 / 1024 * 100) / 100.0;

        if (size >= 1024 * 1024 * 1024) {
            return fileSizeGB + "GB";
        }
        else if (size >= 1024 * 1024) {
            return fileSizeMB + "MB";
        }
        else if (size >= 1024) {
            return fileSizeKB + "KB";
        } else {
            return size + "B";
        }
    },
    // 查看条码
    showBarCode: function (data) {
        mini.open({
            title: "查看条码",
            url: bootPATH + "BaseInfo/ShowBarCode.html?data=" + escape(data),
            width: 300,
            height: 200
        });
    },
    // 查看二维码
    showQrCode: function (data) {
        mini.open({
            title: "查看二维码",
            url: bootPATH + "BaseInfo/ShowQrCode.html?data=" + escape(data),
            width: 500,
            height: 500
        });
    }
});

(function () {
    Date.prototype.format = function (format) {
        var o = {
            "M+": this.getMonth() + 1, //month 
            "d+": this.getDate(), //day 
            "h+": this.getHours(), //hour 
            "m+": this.getMinutes(), //minute 
            "s+": this.getSeconds(), //second 
            "q+": Math.floor((this.getMonth() + 3) / 3), //quarter 
            "S": this.getMilliseconds() //millisecond 
        }

        if (/(y+)/.test(format)) {
            format = format.replace(RegExp.$1, (this.getFullYear() + "").substr(4 - RegExp.$1.length));
        }

        for (var k in o) {
            if (new RegExp("(" + k + ")").test(format)) {
                format = format.replace(RegExp.$1, RegExp.$1.length == 1 ? o[k] : ("00" + o[k]).substr(("" + o[k]).length));
            }
        }
        return format;
    }


    function cacl(arr, callback) {
        var ret;
        for (var i = 0; i < arr.length; i++) {
            ret = callback(arr[i], ret);
        }
        return ret;
    }

    Array.prototype.max = function () {
        return cacl(this, function (item, max) {
            if (!(max > item)) {
                return item;
            }
            else {
                return max;
            }
        });
    };
    Array.prototype.min = function () {
        return cacl(this, function (item, min) {
            if (!(min < item)) {
                return item;
            }
            else {
                return min;
            }
        });
    };
    Array.prototype.sum = function () {
        return cacl(this, function (item, sum) {
            if (typeof (sum) == 'undefined') {
                return item;
            }
            else {
                return sum += item;
            }
        });
    };
    Array.prototype.avg = function () {
        if (this.length == 0) {
            return 0;
        }
        return this.sum(this) / this.length;
    };
    Array.prototype.insert = function (index, item) {
        this.splice(index, 0, item);
        return this;
    };
    Array.prototype.repeat = function (count) {
        for (var i = 1; i < count; i++) {
            this.push(this[0]);
        }
        return this;
    };

    Number.prototype.toFixed = function (s) {
        return parseInt(this * Math.pow(10, s) + 0.5) / Math.pow(10, s);
    }
})();

// 页面初始化
$(function () {
    // 页面初始化自定义函数
    $.page_init();
    // 权限校验
    if (!$.userValidate()) return false;

    // miniui初始化
    if (typeof (mini) != "undefined") mini.parse();
    // 页面载入自定义函数
    $.page_load();
});