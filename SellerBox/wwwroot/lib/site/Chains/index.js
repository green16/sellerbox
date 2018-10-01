$("table input[type=checkbox]").click(function (evt) {
	var idChain = $(this).closest("tr").attr("data-idChain");
	$.ajax({
		type: "POST",
		url: "/Chains/ToogleChain?idChain=" + idChain,
		contentType: "application/json; charset=utf-8"
	});
});

$("a.removeChain").click(function (evt) {
	var container = $(this).closest("tr");
	var idChain = container.attr("data-idChain");
	$('#warningRemoveModal').attr("data-idRemovingChain", idChain);
	$('#warningRemoveModal').modal("show");
});

$('#warningRemoveModal').on('hide.bs.modal', function (evt) {
	$(this).removeAttr("data-idRemovingChain");
});

$("#warningRemoveModalYesButton").click(function (evt) {
	var idChain = $('#warningRemoveModal').attr("data-idRemovingChain");
	var removingContainer = $("[data-idChain=" + idChain + "]");
	$.ajax({
		url: "/Chains/Delete?idChain=" + idChain,
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
