__CreateJSPath = function (js) {
    var scripts = document.getElementsByTagName("script");
    var path = "";
    for (var i = 0, l = scripts.length; i < l; i++) {
        var src = scripts[i].src;
        if (src.indexOf(js) != -1) {
            var ss = src.split(js);
            path = ss[0];
            break;
        }
    }
    var href = location.href;
    href = href.split("#")[0];
    href = href.split("?")[0];
    var ss = href.split("/");
    ss.length = ss.length - 1;
    href = ss.join("/");
    if (path.indexOf("https:") == -1 && path.indexOf("http:") == -1 && path.indexOf("file:") == -1 && path.indexOf("\/") != 0) {
        path = href + "/" + path;
    }
    return path;
}
var bootPATH = __CreateJSPath("lib/boot.js");

//debugger
mini_debugger = false;

document.write('<script src="' + bootPATH + 'lib/jquery-1.10.2.min.js" type="text/javascript" ></sc' + 'ript>');
document.write('<script src="' + bootPATH + 'lib/json2.js" type="text/javascript" ></sc' + 'ript>');
//miniui
document.write('<link href="' + bootPATH + 'lib/miniui/themes/default/miniui.css" rel="stylesheet" type="text/css" />');
document.write('<link href="' + bootPATH + 'lib/miniui/themes/icons.css" rel="stylesheet" type="text/css" />');
document.write('<link href="' + bootPATH + 'lib/miniui/themes/metro/skin.css" rel="stylesheet" type="text/css" />');
document.write('<link href="' + bootPATH + 'css/global.css" rel="stylesheet" type="text/css" />');
document.write('<script src="' + bootPATH + 'lib/miniui/miniui.js" type="text/javascript" ></sc' + 'ript>');

//font
document.write('<link href="' + bootPATH + 'lib/FontAwesome/font-awesome.min.css" rel="stylesheet" type="text/css" />');
// 兼容IE7
if (navigator.appName == "Microsoft Internet Explorer" && navigator.appVersion.match(/7./i) == "7.") {
    document.write('<link href="' + bootPATH + 'lib/FontAwesome/font-awesome-ie7.min.css" rel="stylesheet" type="text/css" />');
}

document.write('<script src="' + bootPATH + 'lib/core.js" type="text/javascript" ></sc' + 'ript>');
document.write('<script src="' + bootPATH + 'lib/miniExt.js" type="text/javascript" ></sc' + 'ript>');
document.write('<script src="' + bootPATH + 'lib/config.js" type="text/javascript" ></sc' + 'ript>');