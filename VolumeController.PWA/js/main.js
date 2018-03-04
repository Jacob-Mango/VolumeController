var socket, path;
var hosts = ['127.0.0.1', 'localhost'];

for (var i in hosts) {
	path = 'ws://' + hosts[i] + ':3820/';
	console.log('===> Tested path :: ', path);
	try {
		socket = new WebSocket(path);
		break;
	} catch (e) {
		// !!! Never shown !!!
		console.error('===> WebSocket creation error :: ', e);
	}
}

// Connection opened
socket.addEventListener('open', function (event) {
	// socket.send('login');
});

var Applications = [];
var Groups = [];

// Listen for messages
socket.addEventListener('message', function (event) {
	var data = JSON.parse(event.data);

	for (var i = 0; i < data.Groups.length; i++)
		SetGroup(i, data.Groups[i]);

	for (var i = 0; i < data.Applications.length; i++)
		SetApplication(i, data.Applications[i]);

	for (var i = 0; i < data.Icons; i++)
		SetIcon(i, data.Icons[i]);

	console.log(data);
});

function SetApplication(i, app) {
	if (Applications[i] === undefined) {

		var ht = "";
		ht += "<div class='application' id='" + i + "'>";
		ht += "<div class='left'>";
		ht += "<img src='app.png' />";
		ht += "</div>";
		ht += "<div class='right'>";
		ht += "<div class='dropdown'>";
		ht += "<img src='more.png' />";
		ht += "<div class='dropdown-content'>";
		ht += "<img src='more.png' />";
		ht += "<input type='button' class='button' value='Move Group'/>";
		ht += "</div>";
		ht += "</div>";
		ht += "<h4>" + app.Name + "</h4>";
		ht += "<input type='range' min='0' max='100' value='" + app.Volume * 100 + "' class='slider' id='" + i + "' />";
		ht += "<input type='text' class='text' id='" + i + "' />";
		ht += "</div>";
		ht += "</div>";

		$("#group" + app.GroupID + " .applications").append($(ht));
	} else {
		$("#" + i + ".slider").val(app.Volume * 100);
		// $("#" + i + ".text").val(app.Volume * 100);
	}

	Applications[i] = app;
}

function SetGroup(i, group) {
	if (Groups[i] === undefined) {
		var c = $("<div class='group' id='group" + group.GroupID + "'><h3>" + group.Name +
			"</h3><div class='applications'></div></div>");
		$(".groups").append(c);
	}
	Groups[i] = group;
}

function SetIcon(i, icon) {}

setInterval(function () {
	for (var i = 0; i < Applications.length; i++) {
		Applications[i].Volume = $("#" + i + ".slider").val() / 100;
	}

	socket.send(JSON.stringify({
		"Groups": Groups,
		"Applications": Applications
	}));
}, 250);

function GetLocalIP() {
	window.RTCPeerConnection = window.RTCPeerConnection || window.mozRTCPeerConnection || window.webkitRTCPeerConnection;
	var pc = new RTCPeerConnection({
			iceServers: []
		}),
		noop = function () {};

	pc.createDataChannel('');
	pc.createOffer(pc.setLocalDescription.bind(pc), noop);
	pc.onicecandidate = function (ice) {
		if (ice && ice.candidate && ice.candidate.candidate) {
			var ip = /([0-9]{1,3}(\.[0-9]{1,3}){3}|[a-f0-9]{1,4}(:[a-f0-9]{1,4}){7})/.exec(ice.candidate.candidate)[1];
			pc.onicecandidate = noop;
		}
	};
}