$("#actionTypes input").change(function () {
	var idAction = $(this).attr("data-id");
	switch (idAction) {
		case "0":
			{
				$("#onlyReply").prop("hidden", false);
				$("#chain1").prop("hidden", true);
				$("#chain2").prop("hidden", true);
				$("#onError").prop("hidden", true);
				break;
			}
		case "1":
			{
				$("#onlyReply").prop("hidden", true);
				$("#chain1").prop("hidden", false);
				$("#chain2").prop("hidden", true);
				$("#onError").prop("hidden", false);
				break;
			}
		case "2":
			{
				$("#onlyReply").prop("hidden", true);
				$("#chain1").prop("hidden", false);
				$("#chain2").prop("hidden", false);
				$("#onError").prop("hidden", false);
				break;
			}
		case "3":
			{
				$("#onlyReply").prop("hidden", true);
				$("#chain1").prop("hidden", true);
				$("#chain2").prop("hidden", false);
				$("#onError").prop("hidden", false);
				break;
			}
	}
});