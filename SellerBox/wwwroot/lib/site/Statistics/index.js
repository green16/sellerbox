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