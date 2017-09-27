var myChart;
var cboType, txtBeginDate, txtEndDate;

$.page_load = function () {
    cboType = mini.get("cboType");
    cboType.setData([{ Name: "发现人", Code: "finder" }, { Name: "区域", Code: "region" }, { Name: "责任人", Code: "resp" }, { Name: "提交日期", Code: "date" }]);
    cboType.setValue("finder");

    txtBeginDate = mini.get("txtBeginDate");
    // 默认当月一号
    txtBeginDate.setValue(new Date((new Date()).setDate(1)));

    txtEndDate = mini.get("txtEndDate");
    //默认当月最后一天
    txtEndDate.setValue(new Date());


    myChart = echarts.init(document.getElementById('divChart'));
    search();
};

function search() {
    var param = {};
    param.ByType = cboType.getValue();
    var beginDate = txtBeginDate.getValue();
    param.BeginDate = beginDate == "" ? beginDate = null : $.jsDate2WcfDate(beginDate);
    var endDate = txtEndDate.getValue();
    param.EndDate = endDate == "" ? endDate = null : $.jsDate2WcfDate(endDate);


    myChart.showLoading();
    $.srv("LPA", "ReportQuery", {
        data: { param: param },
        success: function (result) {
            var data = buildData(result.Data);
            initChart(data);
        },
        exception: function () {
            myChart.hideLoading();
        }
    });
}

function initChart(data) {
    // 指定图表的配置项和数据
    var option = {
        title: {
            text: 'LPA报表 - ' + cboType.getText(),
            x: 'center'
        },
        color: ['#5b9bd5'],
        tooltip: {
            trigger: 'axis',
            axisPointer: {            // 坐标轴指示器，坐标轴触发有效
                type: 'shadow'        // 默认为直线，可选为：'line' | 'shadow'
            }
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
        xAxis: {
            data: data.x,
            axisLabel: {
                interval: 0,//强制显示所有水平标签
                rotate: 45// 倾斜
            }
        },
        yAxis: {},
        series: [{
            name: '问题数量',
            type: 'bar',
            data: data.y,
            barWidth: 15, //柱图宽度
            markLine: {
                data: [
                    { type: 'average', name: '平均值' }
                ]
            }
        }],
        //dataZoom: [{
        //    type: 'slider',
        //    start: 0,
        //    end: 100
        //}]
    };
    myChart.hideLoading();
    myChart.setOption(option);
}

function buildData(data) {
    var result = { x: [], y: [] };
    for (var i = 0; i < data.length; i++) {
        result.x.push(data[i].Name);
        result.y.push(data[i].ProbCount);
    }
    return result;
}