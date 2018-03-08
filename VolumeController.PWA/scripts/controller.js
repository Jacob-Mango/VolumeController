'use strict';

class Controller {
	constructor() {
		var self = this;
		this.Applications = [];
		this.Groups = [];

		// Internal IP's are the best :)
		this.Socket = new Socket(['192.168.0.65']);
		this.Socket.OnMessage(function (event) {
			self.Update(JSON.parse(event.data));
		});

		setInterval(function () {
			try {
				for (var i = 0; i < self.Applications.length; i++) {
					$("#" + self.Applications[i].ProcessID + " .app-name").css({
						"fontSize": 20
					});
					$("#" + self.Applications[i].ProcessID + " .app-name").autoshrink();
				}
			} catch (e) {}
		}, 400);
	}

	UpdateApplication(app) {
		var self = this;
		var id = this.Applications.length + 1;
		var index;
		var found = this.Applications.some(function (ele, i) {
			return ele.ProcessID === app.ProcessID ? (index = i, true) : false;
		});
		if (!found) {
			var ht = $('.templates .application').clone();
			ht.attr('id', app.ProcessID);
			ht.find(".slider-wrapper").attr('id', app.ProcessID);
			ht.find(".mute").attr('id', app.ProcessID);
			ht.find(".cgroup").attr('id', app.ProcessID);
			ht.find(".app-name").text(app.Name);

			$("#" + app.GroupID + ".application-group").append($(ht));

			new Slider(app.ProcessID);

			this.Applications.push(app);
		} else {
			this.Applications[index].Volume = app.Volume;

			$(".application #" + this.Applications[index].ProcessID + ".slider").val(this.Applications[index].Volume * 100);
		}
	}

	UpdateGroup(group) {
		if (group.Volume === undefined) return;

		var id = this.Applications.length + 1;
		var index;
		var found = this.Applications.some(function (ele, i) {
			return ele.GroupID === group.GroupID ? (index = i, true) : false;
		});
		if (!found) {
			$("#groups-select").append($("<option value='" + group.GroupID + "'>" + group.Name + "</option>"));


			var ht = $('.templates .application-group').clone();
			ht.attr('id', group.GroupID);
			ht.attr('class', ht.attr('class') + ' invisible');

			$(".groups").append($(ht));

			this.Groups.push(group);
		} else {
			this.Groups[index].Volume = group.Volume;
			$(".group #" + this.Groups[index].GroupID + ".slider").val(this.Groups[index].Volume * 100);
		}
	}

	SelectGroup(select) {
		try {
			if (this.Groups.length > 0) {
				var c = "invisible";

				for (var i = 0; i < this.Groups.length; i++)
					$("#" + this.Groups[i].GroupID + ".application-group").addClass(c);


				$("#" + select + ".application-group").removeClass(c);

				$(".group").attr('id', select);
				$(".group .slider-group").attr('id', select);
				$(".group .ccgroup").attr('id', select);
				$(".group .mute").attr('id', select);
				$(".group .name").text(this.Groups[select].Name);
			}
		} catch (e) {
			console.log(e);
		}
	}

	Update(data) {
		for (var i = 0; i < data.Groups.length; i++)
			this.UpdateGroup(data.Groups[i]);

		for (var i = 0; i < data.Applications.length; i++)
			this.UpdateApplication(data.Applications[i]);

		for (var i = 0; i < this.Applications.length; i++) {
			this.Applications[i].Volume = $(".application #" + this.Applications[i].ProcessID + ".slider").val() / 100;
		}

		for (var i = 0; i < this.Groups.length; i++) {
			this.Groups[i].Volume = $(".group #" + this.Groups[i].GroupID + ".slider").val() / 100;
		}
		this.Socket.SendData({
			"Groups": this.Groups,
			"Applications": this.Applications
		});
	}
};