const connection = new signalR.HubConnectionBuilder()
	.withUrl("/messaginghub")
	.build();

$(function () {
	connection.start().then(function () {
		connection.invoke("Subscribe", idGroup).then(function (state) {
			if (state == null) {
				$("div.sendingProgress").parent().prop("hidden", true);
			}
			else {
				state.forEach(function (sendingState) {
					ProgressChanged(sendingState);
				});
			}
		});
	});
});

$("button.removeMessagingButton").click(function (evt) {
	var container = $(this).closest("div.container");
	var idMessaging = container.attr("data-idMessaging");
	$('#warningRemoveModal').attr("data-idRemovingMessaging", idMessaging);
	$('#warningRemoveModal').modal("show");
});

$('#warningRemoveModal').on('hide.bs.modal', function (evt) {
	$(this).removeAttr("data-idRemovingMessaging");
})

$("#warningRemoveModalYesButton").click(function (evt) {
	var idMessaging = $('#warningRemoveModal').attr("data-idRemovingMessaging");
	var removingContainer = $("div.container [data-idMessaging=" + idMessaging + "]");
	$.ajax({
		url: "/Messaging/DeleteMessaging?idMessaging=" + idMessaging,
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

connection.on("ProgressStarted", (sendingState) => {
	var progressBar = $("div[data-idMessaging='" + sendingState.idMessaging + "'] div.progress div.sendingProgress");
	progressBar.removeClass();
	progressBar.addClass("progress-bar progress-bar-striped progress-bar-animated bg-warning sendingProgress");
	progressBar.attr("aria-valuenow", 0);
	progressBar.attr("aria-valuemax", 0);
	progressBar.attr("style", "width: 100%");
	progressBar.text("Запуск...");
	progressBar.parent().prop("hidden", false);
});

function ProgressChanged(sendingState) {
	var progressBar = $("div[data-idMessaging='" + sendingState.idMessaging + "'] div.progress div.sendingProgress");
	progressBar.parent().prop("hidden", false);
	if (progressBar.hasClass("bg-warning")) {
		progressBar.removeClass("bg-warning");
	}
	progressBar.attr("aria-valuemax", sendingState.total);
	progressBar.attr("aria-valuenow", sendingState.progress);
	var percent = 0;
	if (sendingState.total != 0)
		percent = Math.trunc(sendingState.progress / sendingState.total * 100);
	progressBar.attr("style", "width: " + percent + "%");
	progressBar.text(percent + "%");
};

connection.on("ProgressChanged", (sendingState) => ProgressChanged(sendingState));

connection.on("ProgressFinished", (idMessaging) => {
	var progressBar = $("div[data-idMessaging='" + idMessaging + "'] div.progress div.sendingProgress");
	progressBar.addClass("bg-success");
	var total = progressBar.attr("aria-valuemax");
	progressBar.attr("aria-valuenow", total);
	progressBar.attr("style", "width: 100%");
	progressBar.text("100%");
	progressBar.removeClass("progress-bar-striped progress-bar-animated");

	var progressDivBlock = progressBar.parent();
	progressDivBlock.fadeOut(1600, "swing", function (successAlert) { progressDivBlock.prop("hidden", true); }.bind(this, progressDivBlock));

	var messagingStatusSpan = progressDivBlock.prev().children("b").children("span");
	messagingStatusSpan.text("завершено");
	messagingStatusSpan.prop("style", "color: green");
});