$("#actionTypes input").change(function () {
	var idAction = $(this).attr("data-id");
	$("#action").prop("hidden", idAction == "0");
	switch (idAction) {
		case "1":
			{
				$("#chain1").prop("hidden", false);
				$("#chain2").prop("hidden", true);
				break;
			}
		case "2":
			{
				$("#chain1").prop("hidden", false);
				$("#chain2").prop("hidden", false);
				break;
			}
		case "3":
			{
				$("#chain1").prop("hidden", true);
				$("#chain2").prop("hidden", false);
				break;
			}
	}
});