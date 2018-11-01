function insertAtCursor(input, textToInsert) {
	// get current text of the input
	const value = input.val();

	// save selection start and end position
	const start = input[0].selectionStart;
	const end = input[0].selectionEnd;

	if (value && value.length > 0) {
		if (start && end) {
			// update the value with our text inserted
			const newValue = value.slice(0, start) + textToInsert + value.slice(end);
			input.val(newValue);

			// update cursor to be at the end of insertion
			input[0].selectionStart = input[0].selectionEnd = start + textToInsert.length;
		}
		else {
			input.val(input.val() + textToInsert);
		}
	} else {
		input.val(textToInsert);
	}
}

$("div.userKeywordButtons").on("click", "a", function () {
	var keyword = $(this).attr("data-keyword");
	var textarea = $(this).closest("div.message-part").find("textarea");

	insertAtCursor(textarea, keyword);
});

$("div.linksKeywordButtons").on("click", "a", function () {
	var keyword = $(this).attr("data-keyword");
	var textarea = $(this).closest("div.message-part").find("textarea");
	var linkKeyword = "%SHORTLINK:" + keyword + "%"
	insertAtCursor(textarea, linkKeyword);
});

$('#btnGroupLinks').on("click", function () {
	var userKeywordButtons = $("div.linksKeywordButtons");
	userKeywordButtons.empty();
	$.ajax({
		url: "/ShortUrls/GetList",
		contentType: "application/json; charset=utf-8",
		type: "POST",
		success: function (data) {
			for (var key in data) {
				var newItem = $('<a class="dropdown-item" href="#" data-keyword="' + key + '">' + data[key] + '</a>');
				userKeywordButtons.append(newItem);
			}
		}
	});
});

$("div.uploadedFilesList").on("click", "button.close", function (evt) {
	var table = $("div.uploadedFilesList");
	var div = $(this).closest("div.list-group-item");
	var idFile = div.attr("data-idFile");
	$.ajax({
		url: "/Messaging/Delete",
		contentType: "application/json; charset=utf-8",
		data: JSON.stringify(idFile),
		type: "POST",
		success: function (data) {
			if (data.state == 0) {
				div.remove();
				var rowsCount = table.find("div.list-group-item").length;
				if (rowsCount == 0)
					table.parent().prop("hidden", true);
			}
		}
	});
});

$("button.upload-button").click(function (evt) {
	var messagePartBlock = $(this).closest("div.message-part");
	var formData = new FormData();
	var input = messagePartBlock.find("input[name=file]");
	var file = input[0].files[0];
	formData.append('file', file);

	var uploadedFilesCount = messagePartBlock.find("div.uploadedFilesList div.list-group-item").length;

	var progressEle = messagePartBlock.find("progress-bar");
	progressEle.removeClass();
	progressEle.addClass("progress-bar");
	progressEle.parent().prop("hidden", false);
	progressEle.parent("div").fadeIn();


	var prefix = messagePartBlock.attr("data-dataPrefix");
	if (prefix == undefined)
		prefix = "";
	$.ajax({
		url: "/Messaging/UploadFileAjax?idx=" + uploadedFilesCount + "&prefix=" + prefix,
		data: formData,
		processData: false,
		contentType: false,
		type: "POST",
		xhr: function () {
			var xhr = new window.XMLHttpRequest();
			xhr.upload.addEventListener("progress", function (evt) {
				if (evt.lengthComputable) {
					var progress = Math.round((evt.loaded / evt.total) * 100);
					progressEle.html(progress + "%");
					progressEle.width(progress + "%");
					progressEle.attr("aria-valuenow", progress);
				}
			}, false);
			return xhr;
		},
		success: function (html) {
			progressEle.removeClass("bg-danger bg-success");
			if (html == "") {

				progressEle.addClass("bg-danger");
				return;
			}

			progressEle.addClass("bg-success").parent("div").fadeOut("slow");

			var successAlert = $("<div></div>");
			successAlert.addClass("alert alert-success alert-dismissible fade show");
			successAlert.append("Файл успешно загружен.");
			successAlert.insertAfter(messagePartBlock.find("div.file-input-group"));

			successAlert.fadeOut(1600, "swing", function (successAlert) { successAlert.remove(); }.bind(this, successAlert));

			var table = messagePartBlock.find("div.uploadedFilesList");
			table.append(html);
			table.parent().prop("hidden", false);
			input.val('');
			input.closest('label').get(0).lastChild.nodeValue = "Выберите файл";
			messagePartBlock.find("button.upload-button").prop("disabled", true);
		},
		error: function (data) {
			progressEle.addClass("bg-danger");
		}
	});
});

$("input.custom-file-input").change(function (e) {
	$(this).parent("label.selected-file-label").get(0).lastChild.nodeValue = this.files[0].name;
	var files = e.originalEvent.target.files;
	var fileSize = 0;
	for (var i = 0, len = files.length; i < len; i++) {
		fileSize += Number(files[i].size);
	}

	var messagePartBlock = $(this).closest("div.message-part");
	var uploadButton = messagePartBlock.find(".upload-button");
	var fileTooBigAlert = messagePartBlock.find(".alert-fileTooBig");
	if (fileSize > MaxRequestBodySize) {
		uploadButton.prop("disabled", true);
		fileTooBigAlert.prop("hidden", false);
	}
	else {
		uploadButton.prop("disabled", false);
		fileTooBigAlert.prop("hidden", true);
	}
});

$("button.messageKeyboadAppendColumnButton").click(function (evt) {
	var clickedButton = $(this).parent();
	var currentRow = $(this).parents("div.row");
	var currentRowId = currentRow.attr("data-idRow");
	var columnsInRow = currentRow.children("div.column");
	var table = $("#messageKeyboad");
	var prefix = table.attr("data-prefix");
	var rowsInTable = table.children("div.row");
	var newRowId = currentRowId;
	var newColumnId = columnsInRow.length - 2;
	if (newColumnId == 3) {
		clickedButton.prop("hidden", true);
	}

	$.ajax({
		url: "/Messaging/AddKeyboardButton?prefix=" + prefix + "&row=" + newRowId + "&column=" + newColumnId,
		contentType: "application/json; charset=utf-8",
		type: "POST",
		success: function (html) {
			var newColumn = $("<div></div>");
			newColumn.addClass("column col-3");
			newColumn.attr("data-idColumn", newColumnId);
			newColumn.append(html);
			newColumn.find("button.close").prop("hidden", false);
			newColumn.insertBefore(clickedButton);
			var prevColumn = newColumn.prev("div.column");
			prevColumn.find("button.close").prop("hidden", true);
			prevColumn.find("input[name*='CanDelete']").val(false);
		}
	});
});

$("button.messageKeyboadAppendRowButton").click(function (evt) {
	var clickedButton = $(this).parent();
	var table = $("#messageKeyboad");

	var rowsInTable = table.children("div.row");
	if (rowsInTable > 10) {
		clickedButton.prop("hidden", true);
	}

	var currentRow = $(this).parents("div.row");
	var newRow = currentRow.clone(true);
	var addColumnButton = newRow.find("button.messageKeyboadAppendColumnButton").parent();
	var addRowButton = currentRow.find("button.messageKeyboadAppendRowButton").parent();
	addColumnButton.prop("hidden", false);
	addRowButton.prop("hidden", true);
	newRow.attr("data-idRow", rowsInTable.length);
	newRow.find("div[data-idColumn]").remove();
	newRow.insertAfter(currentRow);
});

$("#messageKeyboad").on("click", "button.close", function (evt) {
	var removingColumn = $(this).parents("div.column");
	var prevColumn = removingColumn.prev("div.column");
	prevColumn.find("button.close").prop("hidden", false);
	removingColumn.remove();
});