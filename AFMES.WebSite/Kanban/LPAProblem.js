var tbody;

$.page_load = function() {
	mui.init();
	//tick();

	tbody = $("#tb tbody");
	tbody.empty();
	loadData();
};

function tick() {
	$("#lblColok").html(new Date().format("yyyy/MM/dd<br>hh:mm:ss"));
	window.setTimeout("tick()", 1000);
}

function loadData() {
	$.srv("LPA", "KanBanProblemList", {
		type: "GET",
		success: function(result) {
			var data = result.Data;
			if(data.length > 0) {
				var tbodyHtml = "";
				for(var i = 0; i < data.length; i++) {
					var li = data[i];
					var html = "";
					html += "<tr>";
					//html += "<td>" + li.CreateByName + "</td>";
					html += "<td>" + li.ProblemRegionName + "</td>";
					html += "<td>" + li.ProblemTypeName + "</td>";
					html += "<td>" + li.ProblemDesc + "</td>";
					//html += "<td>" + (li.Measure || "") + "</td>";
					html += "<td>" + li.ResponsibleName + "</td>";

					var planEndDat = li.PlanEndDate == null ? "" : $.wcfDate2JsDate(li.PlanEndDate).format("MM-dd");
					//html += "<td>" + planEndDat + "</td>";

					var actualEndDate = li.ActualEndDate == null ? "" : $.wcfDate2JsDate(li.ActualEndDate).format("MM-dd");
					html += "<td>" + actualEndDate + "</td>";
					html += "<td class='status " + getProgress(li.Progress) + "'>" + li.Progress + "%</td>";
					html += "</tr>";
					tbodyHtml += html;
				}
				
				tbody.empty().append(tbodyHtml);
				window.setTimeout("loadData()", 3 * 60 * 1000);
			}
		},
		exception: function(data) {
			mui.toast(data.Msg);
			console.log(data.Msg);
			window.setTimeout("loadData()", 3000);
			return false;
		}
	});
}

function getProgress(prog) {
	if(prog > 0 && prog < 100)
		return "processing";
	if(prog == 100)
		return "end";

	return "";
}