﻿var myChart;
var cboYear ;

$.page_load = function () {
    cboYear = mini.get("cboYear");

    $.dic("1021", function (e) {        //年份
        cboYear.set({ data: e });
    });
    cboYear.setValue(new Date().getFullYear());//当前年

    myChart = echarts.init(document.getElementById('divChart'));
    search();
};

function search() {
    var param = {};
    param.ByYear = cboYear.getValue();
    param.AreaId = null;
    myChart.showLoading();
    $.srv("LPA", "GetLpaCpWeekRate", {
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
            text: 'LPA Closed Problem Rate (Weekly) 分层审核问题关闭率(周)',
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
            data: ['Problem问题', 'Close关闭', '问题关闭率'],
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
                name: '      周',
                data: data.Weeks
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
        {
            type: 'value',
            name: '问题关闭率',
            min: 0,
            max: 100,
            interval: 5,
            axisLabel: {
                formatter: '{value} %'
            }
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
        data: []
    };

    for (var m = 1; m <= data.Weeks.length; m++) {
        var val = getValue(data.Data, m);
        obj.data.push(val);
    }
    result.push(obj);


    var close = {
        name: "Close关闭",
        type: 'bar',
        data: []
    };

    for (var f = 1; f <= data.Weeks.length; f++) {
        var clo = getCloseValue(data.Data, f);
        close.data.push(clo);
    }
    result.push(close);

    var weeks = {
        name: '问题关闭率',
        type: 'line',
        yAxisIndex: 1,
        data: []
    }
    for (var r = 1; r <= data.Weeks.length; r++) {
        var rat = getCloseRate(data.Data, r);
        weeks.data.push(rat);
    }
    result.push(weeks);

    return result;
}

function getValue(data, m) {
    for (var li in data) {
        var d = data[li];
        if (d.Weeks == m)
            return d.ProblemCount;
    }
    return 0;
}

function getCloseValue(data, m) {
    for (var li in data) {
        var d = data[li];
        if (d.Weeks == m)
            return d.CloseCount;
    }
    return 0;
}

function getCloseRate(data, m) {
    for (var li in data) {
        var d = data[li];
        if (d.Weeks == m)
            return d.CloseRate;
    }
    return 0;
}