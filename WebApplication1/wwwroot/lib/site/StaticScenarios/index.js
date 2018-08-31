$("a.toogleBirthdayIsEnabled").click(function () {
	var link = $(this);
	$.ajax({
		url: "/StaticScenarios/ToogleBirthdayIsEnabled",
		contentType: "application/json; charset=utf-8",
		type: "POST",
		success: function (data) {
			if (data.isEnabled == 0)
				link.text("Отключен");
			else
				link.text("Активен");
		}
	});
});

$("a.toogleBirthdayWallIsEnabled").click(function () {
	var link = $(this);
	$.ajax({
		url: "/StaticScenarios/ToogleBirthdayWallIsEnabled",
		contentType: "application/json; charset=utf-8",
		type: "POST",
		success: function (data) {
			if (data.isEnabled == 0)
				link.text("Отключен");
			else
				link.text("Активен");
		}
	});
});