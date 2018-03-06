'use strict';

class Socket {
	constructor(hosts) {
		var path;

		for (var i in hosts) {
			path = 'ws://' + hosts[i] + ':80/';
			console.log('===> Tested path :: ', path);
			try {
				this.socket = new WebSocket(path);
				break;
			} catch (e) {
				console.error('===> WebSocket creation error :: ', e);
			}
		}
	}

	OnMessage(f) {
		this.socket.addEventListener('message', function (event) {
			f(event);
		});
	}

	SendData(d) {
		this.socket.send(JSON.stringify(d));
	}
};