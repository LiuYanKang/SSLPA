var myChart;
var cboYear, cboAuditArea;

$.page_load = function () {
    cboYear = mini.get("cboYear");
    cboAuditArea = mini.get("cboAuditArea");

    $.dic("1021", function (e) {
        cboYear.set({data:e})
    });
    cboYear.setValue(new Date().getFullYear());

    $.srv("LPA", "AuditAreaGet", {
        success: function (data) {
            cboAuditArea.set({ data: data.Data });
        }
    });
    cboAuditArea.setValue(1);

    myChart = echarts.init(document.getElementById('divChart'));
    search();
}



function search() {
    var param = {};
    param.ByYear = cboYear.getValue();
    param.AreaId = cboAuditArea.getValue() == "" ? null : cboAuditArea.getValue();
    myChart.showLoading();
    $.srv("LPA", "GetLPAAreaWeekly", {
        data: { param: param },
        success: function (result) {
            var chartData = buildData(result.Data);
            initChart(result.Data, chartData);
        },
        exception: function () {
            myChart.hideLoading();
        }
    });
}

function initChart(data, chartData) {
    // 指定图表的配置项和数据
    var option = {
        title: {
            text: 'LPA Area (Weekly) 分层审核问题区域(周)',
            subtext: '',
            x: 'center',
            textStyle: {
                fontSize: 26
            }
        },
        tooltip: {
            trigger: 'axis'
        },
        legend: {
            data: ['Problem问题', 'Close关闭'],
            top: '97%'
        },
        toolbox: {
            show: true,
            feature: {
                dataView: { show: true, readOnly: false },
                magicType: { show: true, type: ['line', 'bar'] },
                restore: { show: true },
                saveAsImage: { show: true }
            }
        },
        calculable: true,
        xAxis: [
            {
                type: 'category',
                name: '周',
                data:data.Weeks
            }
        ],
        yAxis: [
            {
                type: 'value',
                name: '问题数量',
                axisLabel: {
                    formatter: '{value}'
                }
            },
            
],
    series: chartData
};
myChart.hideLoading();
myChart.clear();
myChart.setOption(option);
}

function buildData(data) {
    var result = [];
    if (data == null) {
        return result;
    }
    var obj = {
        name: "Problem问题",
        type: 'bar',
        data: [],
       
    };

    for (var m = 1; m <= data.Weeks.length; m++) {
        var val = getValue(data.Data, m);
        obj.data.push(val);
    }
    result.push(obj);


    var close = {
        name: "Close关闭",
        type: 'bar',
        data: [],
       
    };

    for (var f = 1; f <= data.Weeks.length; f++) {
        var clo = getCloseValue(data.Data, f);
        close.data.push(clo);
    }
    result.push(close);


    return result;
}

function getValue(data, m) {
    for (var li in data) {
        var d = data[li];
        if (d.Weeks == m)
            return d.ProblemCount;
    }
    return "";
}

function getCloseValue(data, m) {
    for (var li in data) {
        var d = data[li];
        if (d.Weeks == m)
            return d.CloseCount;
    }
    return "";
}