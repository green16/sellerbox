$("a.removeShortLink").click(function (evt) {
	var container = $(this).closest("tr");
	var idScenario = container.attr("data-idShortLink");
	$('#warningRemoveModal').attr("data-idRemovingShortLink", idScenario);
	$('#warningRemoveModal').modal("show");
});

$('#warningRemoveModal').on('hide.bs.modal', function (evt) {
	$(this).removeAttr("data-idRemovingShortLink");
});

$("#warningRemoveModalYesButton").click(function (evt) {
	var idShortLink = $('#warningRemoveModal').attr("data-idRemovingShortLink");
	var removingContainer = $("[data-idShortLink=" + idShortLink + "]");
	$.ajax({
		url: "/ShortUrls/Delete?idShortLink=" + idShortLink,
		contentType: "application/json; charset=utf-8",
		type: "POST",
		success: function (e) {
			$('#warningRemoveModal').modal("hide");
			if (e.state != 0)
				return;
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
