(function () {
    window.SS = window.SS || {};
    // js配置项
    SS.config = {
        // 调试模式
        debug: true,
        // 客户Logo上传路径
        customerLogoPath: bootPATH + "res/customer/",
        // LPA问题图片上传路径
        lPAProbPicPath: bootPATH + "res/lpa/",
        // TPMAM问题图片上传路径
        tpmamProbPicPath: bootPATH + "res/tpmam/",
        // 员工头像
        empHeaderPath: bootPATH + "res/emp_header/",
        // EHS图片上传路径
        ehsPicPath: bootPATH + "res/ehs/",

        // 临时文件上传路径
        tempPath: bootPATH + "temp/",
        // 短信重新发送时间(秒)
        smsSendPeriod: 60,
        // 公众号appid
        wxappid: "wxb515ce295a97516a"
    };
})();