// Copyright 2016 Google Inc.
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//      http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.


(function () {
	'use strict';

	var socket, path;
	var hosts = ['127.0.0.1', 'localhost'];

	for (var i in hosts) {
		path = 'ws://' + hosts[i] + ':80/';
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

			var ht = $('.templates .application').clone();
			ht.attr('id', i);
			ht.find(".slider").attr('id', i);
			ht.find(".slider").attr('value', app.Volume * 100);
			ht.find(".mute").attr('id', i);
			ht.find(".cgroup").attr('id', i);
			ht.find(".app-name").text(app.Name);

			$(".applications").append($(ht));
		} else {
			$(".slider#" + i).val(app.Volume * 100);
			// $("#" + i + ".text").val(app.Volume * 100);
		}

		Applications[i] = app;
	}

	function SetGroup(i, group) {
		if (Groups[i] === undefined) {
			//var c = $("<div class='group' id='group" + group.GroupID + "'><h3>" + group.Name +
			//	"</h3><div class='applications'></div></div>");
			//$("#groups").append(c);
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
})();