var myChart;
var cboYear, cboAuditArea;

$.page_load = function () {
    cboYear = mini.get("cboYear");
    cboAuditArea = mini.get("cboAuditArea");

    $.dic("1021", function (e) {        //年份
        cboYear.set({ data: e });
    });
    cboYear.setValue(new Date().getFullYear());//当前年
    //$.dic("1022", function (e) {        //月份
    //    cboMonth.set({ data: e });
    //});
    //cboMonth.setValue(new Date().getMonth() + 1);//当前月

    $.srv("LPA", "AuditAreaGet", {
        success: function (data) {
            cboAuditArea.set({ data: data.Data });           
        }
    });
    cboAuditArea.setValue(1);
    myChart = echarts.init(document.getElementById('divChart'));
    search();
};

function search() {
    var param = {};
    param.ByYear = cboYear.getValue();
    param.AreaId = cboAuditArea.getValue()==""?null:cboAuditArea.getValue();
    myChart.showLoading();
    $.srv("LPA", "LpaProblemArea", {
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
            text: 'LPA Area (Monthly) 分层审核问题区域(月)',
            subtext: '当前区域',
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
            top:'97%'
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
                data: ['1月', '2月', '3月', '4月', '5月', '6月', '7月', '8月', '9月', '10月', '11月', '12月']
            }
        ],
        yAxis: [
            {
                type: 'value'
            }
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
            markPoint: {
                data: [
                    { type: 'max', name: '最大值' },
                    { type: 'min', name: '最小值' }
                ]
            },
            markLine: {
                data: [
                    { type: 'average', name: '平均值' }
                ]
            }
        };

        for (var m = 1; m <= 12; m++) {
            var val = getValue(data, m);
            obj.data.push(val);
        }
        result.push(obj);
    

        var close = {
            name: "Close关闭",
            type: 'bar',
            data: [],
            markPoint: {
                data: [
                    { type: 'max', name: '最大值' },
                    { type: 'min', name: '最小值' }
                ]
            },
            markLine: {
                data: [
                    { type: 'average', name: '平均值' }
                ]
            }
        };

        for (var f = 1; f <= 12; f++) {
            var clo = getCloseValue(data, f);
            close.data.push(clo);
        }
        result.push(close);


    return result;
}

function getValue(data, m) {
    for (var li in data) {
        var d = data[li];
        if (d.AreaMonth == m)
            return d.ProblemCount;
    }
    return "";
}

function getCloseValue(data, m) {
    for (var li in data) {
        var d = data[li];
        if (d.AreaMonth == m)
            return d.CloseCount;
    }
    return "";
}