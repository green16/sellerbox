$("#AddToChain").change(function () {
	$("#IdChain").prop("disabled", !this.checked);
	$("#IdChain").val(null);
});

$("#IsSubscriberRequired").change(function () {
	$("#SubscriberRequiredRegion").prop("hidden", !this.checked);
	$("#IdChain").val(null);
	$("#AddToChain").prop("checked", false);
});