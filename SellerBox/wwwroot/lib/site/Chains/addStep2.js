$("button.removeChainContentButton").click(function (evt) {
	var container = $(this).closest("div.container");
	var idChainContent = container.attr("data-idChainContent");
	$('#warningRemoveModal').attr("data-idRemovingChainContent", idChainContent);
	$('#warningRemoveModal').modal("show");
});

$('#warningRemoveModal').on('hide.bs.modal', function (evt) {
	$(this).removeAttr("data-idRemovingChainContent");
})

$("#warningRemoveModalYesButton").click(function (evt) {
	var idChainContent = $('#warningRemoveModal').attr("data-idRemovingChainContent");
	var removingContainer = $("div.container [data-idChainContent=" + idChainContent + "]");
	$.ajax({
		url: "/Chains/RemoveMessage?idChainContent=" + idChainContent,
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
