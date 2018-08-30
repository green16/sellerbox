$("button.removeRepostScenarioButton").click(function (evt) {
	var container = $(this).closest("div.container");
	var idChainContent = container.attr("data-idRepostScenario");
	$('#warningRemoveModal').attr("data-idRemovingRepostScenario", idChainContent);
	$('#warningRemoveModal').modal("show");
});

$('#warningRemoveModal').on('hide.bs.modal', function (evt) {
	$(this).removeAttr("data-idRemovingRepostScenario");
})

$("#warningRemoveModalYesButton").click(function (evt) {
	var idRepostScenario = $('#warningRemoveModal').attr("data-idRemovingRepostScenario");
	var removingContainer = $("div.container [data-idRepostScenario=" + idRepostScenario + "]");
	$.ajax({
		url: "/Reposts/Delete?idRepostScenario=" + idRepostScenario,
		contentType: "application/json; charset=utf-8",
		type: "POST",
		success: function (data) {
			if (data.error === 1) {
				alert(data.message);
				return;
			}
			$('#warningRemoveModal').modal("hide");
			var parentRemovingContainer = removingContainer.parent();
			removingContainer.remove();

			if (parentRemovingContainer.find("div.container").length == 0)
				parentRemovingContainer.children("h5").attr("hidden", false);
			else
				parentRemovingContainer.children("h5").attr("hidden", true);
		}
	});
});
