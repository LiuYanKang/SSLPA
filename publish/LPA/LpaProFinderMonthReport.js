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
    $.srv("LPA", "GetProFinderMonthReport", {
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
    var bcBySeriesIndex = [];
    for (var a = 0; a < data.NameList.length; a++) {
        bcBySeriesIndex.push("A");
        bcBySeriesIndex.push("B");
    }
    var option = {
        title: {
            text: 'LPA Problem Category Report (Monthly) 分层审核问题发现人(月)',
            subtext: '',
            x: 'center',
            textStyle: {
                fontSize: 26
            }
        },
        tooltip: {
            trigger: 'axis',
            axisPointer: {            // 坐标轴指示器，坐标轴触发有效
                type: 'shadow'        // 默认为直线，可选为：'line' | 'shadow'
            },
            formatter: function (params) {
                var html = [];

                var category = {};
                echarts.util.each(params, function (param) {
                    var catName = param.seriesName;
                    var bc = bcBySeriesIndex[param.seriesIndex];
                    if (!category[catName]) {
                        category[catName] = {};
                    }
                    category[catName][bc] = param.value;
                });
                console.log(category);
                echarts.util.each(category, function (cate, key) {
                    html.push(
                        '<tr>',
                        '<td>', key, '</td>',
                        '<td>', cate.A, '</td>',
                        '<td>', cate.B, '</td>',
                        '</tr>'
                    );
                });

                return '<table border=1><tbody>'
                    + '<tr><td></td><td>A(问题)</td><td>B(关闭)</td></tr>'
                    + html.join('')
                    + '</tbody></table>';
            }
        },
        legend: {
            data: data.NameList,
            top: '97%'
        },
        grid: {
            left: '3%',
            right: '4%',
            bottom: '3%',
            containLabel: true
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

    for (var i = 0; i < data.NameList.length; i++) {
        var name = data.NameList[i];
        var obj = {
            name: name,
            type: 'bar',
            barWidth: 40,
            stack: 'A',
            data: [],
            label: {
                normal: {
                    show: true,
                    position: 'top',
                    formatter: ' ',
                    textStyle: { color: '#333' }
                }
            }
        };

        for (var w = 1; w <= 12; w++) {
            var val = getValue(data.Data, w, name);
            obj.data.push(val);
        }
        result.push(obj);

        var clo = {
            name: name,
            type: 'bar',
            barWidth: 40,
            stack: 'B',
            data: [],
            label: {
                normal: {
                    show: true,
                    position: 'top',
                    formatter: ' ',
                    textStyle: { color: '#333' }
                }
            }
        };
        for (var z = 1; z <= 12; z++) {
            var proc = getCloseValue(data.Data, z, name);
            clo.data.push(proc);
        }
        result.push(clo);
    }
    return result;
}

function getValue(data, w, name) {
    for (var li in data) {
        var d = data[li];

        if (d.AreaMonth == w && d.Name == name)
            return d.ProblemCount;
    }
    return "";
}

function getCloseValue(data, w, name) {
    for (var li in data) {
        var d = data[li];
        if (d.AreaMonth == w && d.Name == name)
            return d.CloseCount;
    }
    return "";
}
