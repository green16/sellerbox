$(function () {
	var now = new Date();
	var year = now.getFullYear();
	var month = now.getMonth() + 1;
	var day = now.getDate();
	var hour = now.getHours();
	var minute = now.getMinutes();
	if (month.toString().length == 1) {
		month = '0' + month;
	}
	if (day.toString().length == 1) {
		day = '0' + day;
	}
	if (hour.toString().length == 1) {
		hour = '0' + hour;
	}
	if (minute.toString().length == 1) {
		minute = '0' + minute;
	}

	$('#DtStart').parent().flatpickr({
		maxDate: year + '-' + month,
		maxTime: hour + ':' + minute,
		locale: "ru",
		time_24hr: true,
		dateFormat: 'd.m.Y H:i',
		enableTime: true,
		wrap: true
	});

	$('#DtEnd').parent().flatpickr({
		maxDate: year + '-' + month,
		maxTime: hour + ':' + minute,
		locale: "ru",
		time_24hr: true,
		dateFormat: 'd.m.Y H:i',
		enableTime: true,
		wrap: true
	});

});

$("form").on("submit", function (event) {
	event.preventDefault();

	var $this = $(this);
	var frmValues = $this.serialize();
	$.ajax({
		type: $this.attr('method'),
		url: $this.attr('action'),
		data: frmValues
	}).done(function (e) {
		if (e == null) {
			$('#chartBlock').prop('hidden', true);
			return;
		}
		UpdateChartBlock(e);
	})
});

function RenewCanvas(ctx) {
	var ctxContainer = ctx.parent();
	ctx.remove(); // this is my <canvas> element
	var newCtx = $('<canvas id="chart" style="width:100%; height:500px"></canvas>');
	ctxContainer.append(newCtx);
	return newCtx;
};

function UpdateChartBlock(data) {
	$('#title').text(data.title);

	var legendBlock = $('#legend');
	legendBlock.empty();
	data.legend.forEach(function (legendElement) {
		var newLegendElement = $('<div style="display: flex; margin-left: 40px"></div>');
		var newSquareElement = $('<div style="height: 16px; width: 16px; background-color: ' + legendElement.color + '; margin-right: 5px; margin-top: 4px"></div>');
		var newTextElement = $('<div>' + legendElement.text + ': ' + legendElement.value + '</div>');
		newTextElement.text(legendElement.text + ': ' + legendElement.value);

		newLegendElement.append(newSquareElement);
		newLegendElement.append(newTextElement);

		legendBlock.append(newLegendElement);
	});

	var ctx = RenewCanvas($("#chart"));
	$('#chartBlock').prop('hidden', false);

	window.myChart = new Chart(ctx, {
		type: 'bar',
		data: {
			labels: data.yLabels,
			datasets: data.dataset
		},
		options: {
			tooltips: {
				mode: 'index',
				intersect: false
			},
			responsive: true,
			scales: {
				xAxes: [{
					stacked: true,
				}],
				yAxes: [{
					stacked: true
				}]
			}
		}
	});
}