using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using VolumeController.Service;
using VolumeController.AudioCore;
using VolumeController.Networking;
using VolumeController.Serialization;
using System.Net;

namespace VolumeControllerService {
	partial class MainService : ServiceBase {

		public Dictionary<int, VolumeApplication> Applications;
		public Dictionary<int, VolumeGroup> Groups;

		private Thread m_Worker;
		private AutoResetEvent m_StopRequest;

		private MNetworking m_Networking;

		private List<IPEndPoint> endPoints = new List<IPEndPoint>();

		public MainService() {
			m_Networking = new MNetworking(NetworkHandle, "", 3000);

			Applications = new Dictionary<int, VolumeApplication>();
			Groups = new Dictionary<int, VolumeGroup>();
			m_StopRequest = new AutoResetEvent(false);

			Groups[0] = new VolumeGroup(0, "Group 1", 1.0f);
			// Groups[1] = new VolumeGroup(1, "Group 2", 1.0f);

			InitializeComponent();
		}

		internal void TestStartupAndStop(string[] args) {
			this.OnStart(args);
			Console.ReadLine();
			this.OnStop();
		}

		protected override void OnStart(string[] args) {
			m_Worker = new Thread(UpdateData);
			m_Worker.Start();
		}

		protected override void OnStop() {
			m_StopRequest.Set();
			m_Worker.Join();
		}

		private void NetworkHandle(RCDatabase database, IPEndPoint endPoint) {
			switch (database.GetName()) {
				case "Login":
					Login(database, endPoint);
					break;
				case "UpdateData":
					UpdateData(database);
					break;
				default:
					break;
			}
		}

		private void Login(RCDatabase database, IPEndPoint endPoint) {
			endPoints.Add(endPoint);
			Console.WriteLine("New login!");
		}

		private void UpdateData(RCDatabase database) {
			foreach (var obj in database.Objects) {
				Console.WriteLine("Data updated!");
				string type = obj.GetName();
				int groupID = obj.FindField("gid").GetValue();
				if (type.Equals("Group")) {
					Console.WriteLine("\t- Group");
					Groups[groupID].Volume = obj.FindField("volume").GetValue();
					Groups[groupID].Name = obj.FindString("name").GetString();

					List<ProcessInformation> pinfos = AudioManager.GetAudioApplications();
					foreach (ProcessInformation p in pinfos)
						if (Applications.ContainsKey(p.processID) && Applications[p.processID].Valid) {
							float newVolume = Applications[p.processID].Volume * Groups[Applications[p.processID].GroupID].Volume;

							AudioManager.SetApplicationVolume(p.processID, newVolume);
						}
				} else if (type.Equals("Application")) {
					Console.WriteLine("\t- Application");
					int processID = obj.FindField("pid").GetValue();

					Applications[processID].Volume = obj.FindField("volume").GetValue();
					Applications[processID].Muted = obj.FindField("muted").GetValue();
					Applications[processID].Name = obj.FindString("name").GetString();
					Applications[processID].GroupID = groupID;

					AudioManager.SetApplicationVolume(processID, Applications[processID].Volume * Groups[groupID].Volume);
				}
			}
		}

		private void UpdateData() {
			while (true) {
				RCDatabase data = new RCDatabase("Data");
				foreach (var dicGroup in Groups) {
					VolumeGroup group = dicGroup.Value;

					RCObject application = new RCObject("Group");
					application.AddField(RCField.Integer("gid", group.GroupID));
					application.AddString(RCString.Create("name", group.Name));
					application.AddField(RCField.Float("volume", group.Volume));
					data.AddObject(application);
				}


				List<ProcessInformation> pinfos = AudioManager.GetAudioApplications();
				foreach (ProcessInformation p in pinfos) {
					if (!Applications.ContainsKey(p.processID)) {
						Applications[p.processID] = new VolumeApplication(p.processID, 0);
						Applications[p.processID].Volume = 1;
					}
					if (Applications[p.processID].Valid) {
						AudioManager.SetApplicationVolume(p.processID, Applications[p.processID].Volume * Groups[Applications[p.processID].GroupID].Volume);

						try {
							Process process = Process.GetProcessById(p.processID);
							Applications[p.processID].Name = process.ProcessName;
						} catch (ArgumentException) {
							Applications[p.processID].Name = p.name;
						}

						RCObject application = new RCObject("Application");
						application.AddField(RCField.Integer("pid", Applications[p.processID].ProcessID));
						application.AddField(RCField.Integer("gid", Applications[p.processID].GroupID));
						application.AddField(RCField.Float("volume", Applications[p.processID].Volume));
						application.AddField(RCField.Boolean("muted", Applications[p.processID].Muted));
						application.AddString(RCString.Create("name", Applications[p.processID].Name));
						data.AddObject(application);
					}
				}

				foreach (var endPoint in endPoints) {
					m_Networking.Send(data, endPoint);
				}

				if (m_StopRequest.WaitOne(500)) return;
			}
		}
	}
}
