'use strict';

class Controller {
	constructor() {
		var self = this;
		this.Applications = [];
		this.Groups = [];

		this.Socket = new Socket(['127.0.0.1', 'localhost']);
		this.Socket.OnMessage(function (event) {
			self.Update(JSON.parse(event.data));
		});
	}

	UpdateApplication(app) {
		var id = this.Applications.length + 1;
		var index;
		var found = this.Applications.some(function (ele, i) {
			return ele.ProcessID === app.ProcessID ? (index = i, true) : false;
		});
		if (!found) {
			var ht = $('.templates .application').clone();
			ht.attr('id', app.ProcessID);
			ht.find(".slider").attr('id', app.ProcessID);
			ht.find(".slider").attr('value', app.Volume * 100);
			ht.find(".mute").attr('id', app.ProcessID);
			ht.find(".cgroup").attr('id', app.ProcessID);
			ht.find(".app-name").text(app.Name);

			$("#" + app.GroupID + ".application-group").append($(ht));

			this.Applications.push(app);
		} else {
			this.Applications[index].Volume = app.Volume;

			$(".application #" + this.Applications[index].ProcessID + ".slider").val(this.Applications[index].Volume * 100);
		}
	}

	UpdateGroup(group) {
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

			console.log(this.Groups[index].Volume);
			$(".group #" + this.Groups[index].GroupID + ".slider").val(this.Groups[index].Volume * 100);
		}
	}

	SelectGroup(select) {
		var c = "invisible";

		for (var i = 0; i < this.Groups.length; i++)
			$("#" + this.Groups[i].GroupID + ".application-group").addClass(c);

		$("#" + select + ".application-group").removeClass(c);

		$(".group").attr('id', select);
		$(".group .slider").attr('id', select);
		$(".group .ccgroup").attr('id', select);
		$(".group .mute").attr('id', select);
		$(".group .name").text(this.Groups[select].Name);
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