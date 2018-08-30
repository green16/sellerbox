const connection = new signalR.HubConnectionBuilder()
	.withUrl("/messaginghub")
	.build();

$(function () {
	connection.start().then(function () {
		connection.invoke("Subscribe", idGroup).then(function (state) {
			console.log(state);
			if (state == null) {
				$("form").prop("hidden", false);
				$("#sendingProgress").parent().prop("hidden", true);
			}
			else {
				$("form").prop("hidden", true);
				$("#sendingProgress").parent().prop("hidden", false);
				ProgressChanged(state.item1, state.item2);
			}
		});
	});
});

$("form").on("submit", function (event) {
	var $this = $(this);
	var frmValues = $this.serialize();
	$.ajax({
		type: $this.attr('method'),
		url: $this.attr('action'),
		data: frmValues
	})
	.done(function (e) {
		connection.invoke("Start", idGroup, e.idMessage, e.ids).catch(err => console.log(err.toString()));
	})
	event.preventDefault();
});

connection.on("ProgressStarted", () => {
	var progressBar = $("#sendingProgress");
	progressBar.removeClass();
	progressBar.addClass("progress-bar progress-bar-striped progress-bar-animated bg-warning");
	progressBar.attr("aria-valuenow", 0);
	progressBar.attr("aria-valuemax", 0);
	progressBar.attr("style", "width: 100%");
	progressBar.text("Запуск...");
	progressBar.parent().prop("hidden", false);
});

function ProgressChanged(total, progress) {
	var progressBar = $("#sendingProgress");
	if (progressBar.hasClass("bg-warning")) {
		progressBar.removeClass("bg-warning");
	}
	progressBar.attr("aria-valuemax", total);
	progressBar.attr("aria-valuenow", progress);
	var percent = Math.trunc(progress / total * 100);
	progressBar.attr("style", "width: " + percent + "%");
	progressBar.text(percent + "%");
};

connection.on("ProgressChanged", (total, progress) => ProgressChanged(total, progress));

connection.on("ProgressFinished", () => {
	var progressBar = $("#sendingProgress");
	progressBar.addClass("bg-success");
	var total = progressBar.attr("aria-valuemax");
	progressBar.attr("aria-valuenow", total);
	progressBar.attr("style", "width: 100%");
	progressBar.text("100%");
	progressBar.removeClass("progress-bar-striped progress-bar-animated");
	$("#sync").prop("hidden", false);
	window.location.reload(true);
});
