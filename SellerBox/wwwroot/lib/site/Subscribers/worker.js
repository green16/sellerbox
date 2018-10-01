const connection = new signalR.HubConnectionBuilder()
	.withUrl("/subscribersynchub")
	.build();

$(function () {
	connection.start().then(function () {
		connection.invoke("Subscribe", idGroup, syncType).then(function (state) {
			console.log(state);
			if (state == null) {
				$("#sync").prop("hidden", false);
				$("#syncProgress").parent().prop("hidden", true);
			}
			else {
				$("#sync").prop("hidden", true);
				$("#syncProgress").parent().prop("hidden", false);
				ProgressChanged(state.item1, state.item2);
			}
		});
	});
});

$("#sync").click(function (evt) {
	connection.invoke("StartProcess", idGroup, syncType).catch(err => console.log(err.toString()));
	evt.preventDefault();
});

connection.on("ProgressStarted", () => {
	$("#sync").prop("hidden", true);
	var progressBar = $("#syncProgress");
	progressBar.removeClass();
	progressBar.addClass("progress-bar progress-bar-striped progress-bar-animated bg-warning");
	progressBar.attr("aria-valuenow", 0);
	progressBar.attr("aria-valuemax", 0);
	progressBar.attr("style", "width: 100%");
	progressBar.text("Запуск...");
	progressBar.parent().prop("hidden", false);
});

function ProgressChanged(total, progress) {
	var progressBar = $("#syncProgress");
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
	var progressBar = $("#syncProgress");
	progressBar.addClass("bg-success");
	var total = progressBar.attr("aria-valuemax");
	progressBar.attr("aria-valuenow", total);
	progressBar.attr("style", "width: 100%");
	progressBar.text("100%");
	progressBar.removeClass("progress-bar-striped progress-bar-animated");
	$("#sync").prop("hidden", false);
	window.location.reload(true); 
});
