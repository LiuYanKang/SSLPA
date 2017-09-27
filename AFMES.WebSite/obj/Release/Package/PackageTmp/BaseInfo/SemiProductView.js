var reqStr;

$.page_load = function () {
    reqStr = $.request("SemiProdID");
    if (reqStr) {
        setData(reqStr);
    }
}


function setData(id) {

    $.srv("BaseInfo", "SemiProductGet", {
        data: { id: id },
        success: function (result) {
            var obj = result.Data;
            $("#lblCode").html(obj.Code);
            $("#lblMaterialCode").html(obj.MaterialCode);
            $("#lblStrengthName").html(obj.StrengthName);
            $("#lblDiameter").html(obj.Diameter);
            $("#lblMinStorage").html(obj.MinStorage);
            $("#lblMaxStorage").html(obj.MaxStorage);
            $("#lblSupplierName").html(obj.SupplierName);
            $("#lblRemark").html(obj.Remark);
        }
    });
}