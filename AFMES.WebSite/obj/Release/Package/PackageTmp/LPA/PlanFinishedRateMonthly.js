var myChart;
var cboYear;

$.page_load = function () {
    cboYear = mini.get("cboYear");

    $.dic("1021", function (e) {
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
    $.srv("LPA", "GetPlanFinishedRate", {
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
    var option = {
        title: {
            text: '计划执行关闭率（月）',
            subtext: '',
            x: 'center',
            textStyle: {
                fontSize: 26
            }
        },
        tooltip: {

            trigger:'axis'
        },
        legend: {
            data: ['执行计划总数量','执行计划完成数','执行计划完成率'],
            top:'97%'
        },
        toolbox: {
            show: true,
            feature: {
                dataView: { show: true, readOnly: false },
                magicType: { show: true, type: ['line', 'bar'] },
                restore: { show: true },
                saveAsImage: {show:true}
            }
        },

        calculable: true,
        xAxis: [{
            type: 'category',
            data: ['1月', '2月', '3月', '4月', '5月', '6月', '7月', '8月', '9月', '10月', '11月', '12月']

        }],
        yAxis: [
        {
            type: 'value',
            name: '执行计划数量',
            axisLabel: {
                formatter:'{value}'
            }
        },
        {
            type: 'value',
            name: '执行计划完成率',
            min:0,
            max: 100,
            interval: 5,
            axisLabel: {
                formatter:'{value}%'
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
    for (var i =1; i <=12; i++) {
        var val = getValue(data, i);
        obj.data.push(val);
    }

    result.push(obj);

    var close = {
        name:"执行计划完成数",
        type:'bar',
        data:[]
    };

    for (var j = 1; j <=12; j++) {
        var clo = getCloseValue(data, j);
        close.data.push(clo);

    }
    result.push(close);

    var rate = {
        name: "执行计划完成率",
        type: 'line',
        yAxisIndex: 1,
        data:[]
    }
    for (var k = 1; k <=12; k++) {
        var rat = getCloseRate(data, k);
        rate.data.push(rat);
    }
    result.push(rate);
    return result;

}

function getValue(data, m) {
    for (var li in data) {
        var d = data[li];
        if (d.AreaMonth == m) {
            return d.PlanCountAll;
        }
    }
    return 0;
}

function getCloseValue(data, m) {

    for (var li in data) {
        var d = data[li];
        if (d.AreaMonth == m)
            return d.PlanCountFinished;
    }
    return 0;

}

function getCloseRate(data, m) {

    for (var li in data) {
        var d = data[li];
        if (d.AreaMonth == m)
            return d.PlanFinishedRate;
    }
    return 0;

}