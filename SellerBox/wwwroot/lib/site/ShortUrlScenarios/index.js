$("table input[type=checkbox]").click(function (evt) {
	var idShortUrlScenario = $(this).closest("tr").attr("data-idShortUrlScenario");

	var checkboxType = $(this).attr("data-type");
	switch (checkboxType) {
		case "IsEnabled": {
			$.ajax({
				type: "POST",
				url: "/ShortUrlsScenarios/ToogleIsEnabled?idShortUrlScenario=" + idShortUrlScenario,
				contentType: "application/json; charset=utf-8",
			});
			break;
		}
	}
});

$("a.removeShortUrlsScenarios").click(function (evt) {
	var container = $(this).closest("tr");
	var idShortUrlScenario = container.attr("data-idShortUrlScenario");
	$('#warningRemoveModal').attr("data-idRemovingShortUrlScenario", idShortUrlScenario);
	$('#warningRemoveModal').modal("show");
});

$('#warningRemoveModal').on('hide.bs.modal', function (evt) {
	$(this).removeAttr("data-idRemovingShortUrlScenario");
});

$("#warningRemoveModalYesButton").click(function (evt) {
	var idRemovingShortUrlScenario = $('#warningRemoveModal').attr("data-idRemovingShortUrlScenario");
	var removingContainer = $("[data-idShortUrlScenario=" + idRemovingShortUrlScenario + "]");
	$.ajax({
		url: "/ShortUrlsScenarios/Delete?idShortUrlScenario=" + idRemovingShortUrlScenario,
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
