ymaps.ready(init);

function init() {
	var myMap = new ymaps.Map("map", {
		center: [61.663201, 50.838296],
		zoom: 16
	});

	var myPlacemark = new ymaps.Placemark([61.663201, 50.838296], {
		hintContent: 'ООО "Информационные технологии"'
	});

	myMap.geoObjects.add(myPlacemark);
}