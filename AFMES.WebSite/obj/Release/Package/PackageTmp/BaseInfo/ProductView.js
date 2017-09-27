var reqStr;

$.page_load = function () {
    reqStr = $.request("prodID");
    if (reqStr) {
        setData(reqStr);
    }
}


function setData(id) {

    $.srv("BaseInfo", "ProductGet", {
        data: { id: id },
        success: function (result) {
            var obj = result.Data;
            $("#lblCode").html(obj.Code);
            $("#lblCustCode").html(obj.CustCode);
            $("#lblCustomerName").html(obj.CustomerName);
            $("#lblWeight").html(obj.Weight);
            $("#lblLocCode").html(obj.LocCode);
            $("#lblRemark").html(obj.Remark);
        }
    });
}