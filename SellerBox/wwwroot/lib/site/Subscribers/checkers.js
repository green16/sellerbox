$("a.updateIsChatAllowed").click(function () {
	var tag = $(this);
	var idVkUser = $(this).attr("data-idVk");
	$.ajax({
		url: "/Subscribers/CheckIsChatAllowed",
		contentType: "application/json; charset=utf-8",
		data: JSON.stringify(idVkUser),
		type: "POST",
		success: function (data) {
			var tagParent = tag.parent();
			tag.remove();
			if (data.isAllowed == 0)
				tagParent.text("нет");
			else
				tagParent.text("да");
		}
	});
});

$("a.updateIsSubscriber").click(function () {
	var tag = $(this);
	var idVkUser = $(this).attr("data-idVk");
	$.ajax({
		url: "/Subscribers/CheckIsSubscriber",
		contentType: "application/json; charset=utf-8",
		data: JSON.stringify(idVkUser),
		type: "POST",
		success: function (data) {
			var tagParent = tag.parent();
			tag.remove();
			if (data.isSubscribed == 0)
				tagParent.text("нет");
			else
				tagParent.text("да");
		}
	});
});