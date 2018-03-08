function main() {
	var c = new Controller();
	$(".ccgroup").click(function () {
		c.SelectGroup($("#groups-select").find(":selected").val());
	});

	setTimeout(function () {
		c.SelectGroup(0);
	}, 100);
}

$(document).ready(function () {
	main();
});