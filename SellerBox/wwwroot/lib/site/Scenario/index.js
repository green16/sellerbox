﻿$("table input[type=checkbox]").click(function (evt) {
	var idScenario = $(this).closest("tr").attr("data-idScenario");

	var checkboxType = $(this).attr("data-type");
	switch (checkboxType) {
		case "IsStrictMatch": {
			$.ajax({
				type: "POST",
				url: "/Scenario/ToogleIsStrictMatch?idScenario=" + idScenario,
				contentType: "application/json; charset=utf-8"
			});
			break;
		}
		case "IsEnabled":{
			$.ajax({
				type: "POST",
				url: "/Scenario/ToogleIsEnabled?idScenario=" + idScenario,
				contentType: "application/json; charset=utf-8",
			});
			break;
		}
	}
});

$("a.removeScenario").click(function (evt) {
	var container = $(this).closest("tr");
	var idScenario = container.attr("data-idScenario");
	$('#warningRemoveModal').attr("data-idRemovingScenario", idScenario);
	$('#warningRemoveModal').modal("show");
});

$('#warningRemoveModal').on('hide.bs.modal', function (evt) {
	$(this).removeAttr("data-idRemovingScenario");
});

$("#warningRemoveModalYesButton").click(function (evt) {
	var idScenario = $('#warningRemoveModal').attr("data-idRemovingScenario");
	var removingContainer = $("[data-idScenario=" + idScenario + "]");
	$.ajax({
		url: "/Scenario/Delete?idScenario=" + idScenario,
		contentType: "application/json; charset=utf-8",
		type: "POST",
		success: function () {
			$('#warningRemoveModal').modal("hide");
			removingContainer.remove();

			var parentRemovingContainer = $("table.table").parent();
			var h5 = parentRemovingContainer.find("h5");

			if (parentRemovingContainer.find("tr").length == 1) {
				$("table.table").remove();
				h5.attr("hidden", false);
			}
			else
				h5.attr("hidden", true);
		}
	});
});
