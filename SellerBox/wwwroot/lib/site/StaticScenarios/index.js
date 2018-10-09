$("table input[type=checkbox]").click(function (evt) {
	var checkboxType = $(this).attr("data-type");
	switch (checkboxType) {
		case "IsMaleBirthdayEnabled": {
			$.ajax({
				type: "POST",
				url: "/StaticScenarios/ToogleBirthdayIsEnabled?isMale=true",
				contentType: "application/json; charset=utf-8"
			});
			break;
		}
		case "IsFemaleBirthdayEnabled": {
			$.ajax({
				type: "POST",
				url: "/StaticScenarios/ToogleBirthdayIsEnabled?isMale=false",
				contentType: "application/json; charset=utf-8",
			});
			break;
		}
		case "IsBirthdayWallEnabled": {
			$.ajax({
				type: "POST",
				url: "/StaticScenarios/ToogleBirthdayWallIsEnabled",
				contentType: "application/json; charset=utf-8"
			});
			break;
		}
	}
});