$('#ElementType').change(function () {
	var elementType = $(this).val();

	$.ajax({
		url: "/Notifications/GetElements?elementType=" + elementType,
		contentType: "application/json; charset=utf-8",
		type: "POST",
		success: function (data) {
			$('#IdElement').empty();
			$('#IdElement').append($('<option value="@null" selected>Выберите ...</option>'));

			if (data.error == 1 || !$.isPlainObject(data)) {
				$('#IdElement').parent().prop("hidden", true);
				return;
			}
			$.each(data, function (key, value) {
				$('#IdElement').append($('<option value="' + key + '">' + value + '</option>'));
			});

			$('#IdElement').parent().prop("hidden", false);
		}
	});
});

$('#NotificationType').change(function () {
	var notificationType = $(this).val();
	if (notificationType != 0)
		$('#emailInput').find('input').val("");
	$('#emailInput').prop("hidden", notificationType == 0);
});