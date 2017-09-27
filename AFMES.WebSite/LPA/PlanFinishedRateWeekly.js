var myChart;
var cboYear;

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
    $.srv("LPA", "GetPlanFinishedRateWeekly", {
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
            text: '执行计划关闭率(周)',
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
            data: ['执行计划总数量', '执行计划完成数', '执行计划完成率'],
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
            name: '执行计划数量',
            axisLabel: {
                formatter: '{value}'
            }
        },
        {
            type: 'value',
            name: '执行计划完成率',
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
        name: "执行计划总数量",
        type: 'bar',
        data: []
    };

    for (var m = 1; m <= data.Weeks.length; m++) {
        var val = getValue(data.Data, m);
        obj.data.push(val);
    }
    result.push(obj);


    var close = {
        name: "执行计划完成数",
        type: 'bar',
        data: []
    };

    for (var f = 1; f <= data.Weeks.length; f++) {
        var clo = getCloseValue(data.Data, f);
        close.data.push(clo);
    }
    result.push(close);

    var weeks = {
        name: '执行计划完成率',
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
            return d.PlanCountAll;
    }
    return 0;
}

function getCloseValue(data, m) {
    for (var li in data) {
        var d = data[li];
        if (d.Weeks == m)
            return d.PlanCountFinished;
    }
    return 0;
}

function getCloseRate(data, m) {
    for (var li in data) {
        var d = data[li];
        if (d.Weeks == m)
            return d.PlanFinishedRate;
    }
    return 0;
}