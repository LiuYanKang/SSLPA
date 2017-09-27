var reqStr;

$.page_load = function () {
    reqStr = $.request("RawMaterialID");
    if (reqStr) {
        setData(reqStr);
    }
}


function setData(id) {

    $.srv("BaseInfo", "RawMaterialGet", {
        data: { id: id },
        success: function (result) {
            var obj = result.Data;
            $("#lblCode").html(obj.Code);
            $("#lblSupplierName").html(obj.SupplierName);
            $("#lblMinStorage").html(obj.MinStorage);
            $("#lblMaxStorage").html(obj.MaxStorage);
            $("#lblDiameter").html(obj.Diameter);
            $("#lblRemark").html(obj.Remark);
        }
    });
}