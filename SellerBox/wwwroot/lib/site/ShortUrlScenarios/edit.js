$("#CheckIsSubscriber").change(function () {
	var checkIsSubscriber = this.checked;
	$("#IdGoToErrorChain2").parent().prop("hidden", !checkIsSubscriber);
	$("#IdGoToErrorChain3").parent().prop("hidden", !checkIsSubscriber);

	$("#IdGoToChain").parent().find("div label").text("Переход есть" + (checkIsSubscriber ? " + вступил" : ""));
	$("#IdGoToErrorChain1").parent().find("div label").text("Нет перехода" + (checkIsSubscriber ? " + не вступил" : ""));
});

$("#IdCheckingChain").change(function () {
	var idChain = $("#IdCheckingChain").val();
	$("#IdCheckingChainContent").prop("disabled", idChain == "");

	var removingChainContents = $("#IdCheckingChainContent").find("option").slice(1);
	$.ajax({
		url: "/ShortUrlsScenarios/GetChainContents",
		contentType: "application/json; charset=utf-8",
		data: JSON.stringify(idChain),
		type: "POST",
		success: function (data) {
			removingChainContents.remove();

			if (!$.isPlainObject(data))
				return;

			for (var key in data) {
				$("#IdCheckingChainContent").append($("<option></option>").attr("value", key).text(data[key]));
			}
		},
		error: function () {
			$("#IdCheckingChainContent").prop("disabled", true);
		}
	});
});
