using SeekerSoft.Core.Config;
using System;
using System.Collections.Generic;
using System.Web;

namespace SeekerSoft.Core.WXPay
{
    /**
    * 	配置账号信息
    */
    public static class WxPayConfig
    {
        //=======【基本信息设置】=====================================
        /* 微信公众号信息配置
        * APPID：绑定支付的APPID（必须配置）
        * MCHID：商户号（必须配置）
        * KEY：商户支付密钥，参考开户邮件设置（必须配置）
        * APPSECRET：公众帐号secert（仅JSAPI支付的时候需要配置）
        */
        public static readonly string APPID = ConfigHelper.Get("WXAppID");
        public static readonly string APPSECRET = ConfigHelper.Get("WXSecretCode");
        public static readonly string MCHID = ConfigHelper.Get("WXPayMCHID");
        public static readonly string KEY = ConfigHelper.Get("WXPayKEY");

        // 订单有效期（分钟）
        public static readonly int OrderExpire = ConfigHelper.GetInt("WXPayOrderExpire", 45);

        //=======【证书路径设置】===================================== 
        /* 证书路径,注意应该填写绝对路径（仅退款、撤销订单时需要）
        */
        public static readonly string SSLCERT_PATH = "cert/apiclient_cert.p12";
        public static readonly string SSLCERT_PASSWORD = "1272610901";



        //=======【支付结果通知url】===================================== 
        /* 支付结果通知回调url，用于商户接收支付结果
        */
        public static readonly string NOTIFY_URL = ConfigHelper.Get("WXPayNOTIFY_URL");

        //=======【商户系统后台机器IP】===================================== 
        /* 此参数可手动配置也可在程序中自动获取
        */
        public static readonly string IP = ConfigHelper.Get("ServerIP");


        //=======【代理服务器设置】===================================
        /* 默认IP和端口号分别为0.0.0.0和0，此时不开启代理（如有需要才设置）
        */
        public static readonly string PROXY_URL = "";

        //=======【上报信息配置】===================================
        /* 测速上报等级，0.关闭上报; 1.仅错误时上报; 2.全量上报
        */
        public static readonly int REPORT_LEVENL = 1;

    }
}