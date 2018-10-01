$("#checkLastPosts input").change(function () {
	var idAction = $(this).attr("data-id");
	switch (idAction) {
		case "0":
			{
				$("#postList").prop("hidden", false);
				$("#postsRange").prop("hidden", true);
				break;
			}
		case "1":
			{
				$("#postList").prop("hidden", true);
				$("#postsRange").prop("hidden", false);
				$("#lastPostsCount").val(1);
				break;
			}
	}

});

$("#postsRange input").change(function () {
	var idAction = $(this).attr("data-id");
	switch (idAction) {
		case "0":
			{
				$("#lastPostsCount").prop("disabled", true);
				$("#lastPostsCount").val(null);
				break;
			}
		case "1":
			{
				$("#lastPostsCount").prop("disabled", false);
				$("#lastPostsCount").val(1);
				break;
			}
	}
});

$("#IdCheckingChain").change(function () {
	var idChain = $("#IdCheckingChain").val();
	$("#IdCheckingChainContent").prop("disabled", idChain == "");

	var removingChainContents = $("#IdCheckingChainContent").find("option").slice(1);
	$.ajax({
		url: "/Reposts/GetChainContents",
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

$("#refreshPostsButton").click(function (evt) {
	var removingPosts = $("#IdPost").find("option").slice(1);
	$.ajax({
		url: "/Reposts/RefreshPosts",
		contentType: "application/json; charset=utf-8",
		type: "POST",
		success: function (data) {
			if (data.error == 1) {
				window.location.replace(data.redirectUrl);
				return;
			}
			removingPosts.remove();

			if (!$.isPlainObject(data.posts))
				return;

			for (var key in data.posts) {
				$("#IdPost").append($("<option></option>").attr("value", key).text(data.posts[key]));
			}
		},
		error: function () {
		}
	});
});