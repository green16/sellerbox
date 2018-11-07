$("#AddToChain").change(function () {
	$("#IdChain").prop("disabled", !this.checked);
	$("#IdChain").val(null);
});