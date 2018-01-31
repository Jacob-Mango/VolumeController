using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using VolumeController.Networking;
using VolumeController.AudioCore;
using VolumeController.Serialization;

namespace VolumeController.Forms {
	public partial class MainForm : Form {

		public Dictionary<int, VolumeControl> Applications;
		public Dictionary<int, GroupControl> Groups;

		public MNetworking Networking;

		public MainForm() {
			InitializeComponent();

			Applications = new Dictionary<int, VolumeControl>();
			Groups = new Dictionary<int, GroupControl>();
		}

		private void OnLoad(object sender, EventArgs e) {
			string message = "Login";
			string caption = "Do you wish to login?";
			MessageBoxButtons buttons = MessageBoxButtons.YesNo;
			DialogResult result = System.Windows.Forms.DialogResult.Yes;
			
			// result = MessageBox.Show(message, caption, buttons);

			if (result == System.Windows.Forms.DialogResult.Yes) {
				Networking = new MNetworking(NetworkHandle, "127.0.0.1", 3000);
				Networking.Send(new RCDatabase("Login"));
			} else {
				this.Close();
			}
		}

		private void OnResize(object sender, EventArgs e) {
			foreach (var group in Groups) {
				GroupControl control = group.Value;
				control.Height = this.Size.Height;
				Size size = control.Size;
				size.Height = this.Size.Height;
				control.Size = size;
			}
		}

		private void AddGroup(GroupControl control) {
			tbpnMain.ColumnCount += 1;
			tbpnMain.Controls.Add(control);
			tbpnMain.SetColumn(control, tbpnMain.ColumnCount);
		}

		private void NetworkHandle(RCDatabase database, IPEndPoint endPoint) {
			switch (database.GetName()) {
				case "Data":
					UpdateData(database);
					break;
				default:
					break;
			}
		}

		private void UpdateData(RCDatabase database) {
			RCDatabase updatedData = new RCDatabase("UpdateData");
			foreach (var obj in database.Objects) {
				string type = obj.GetName();
				if (type.Equals("Group")) {
					int groupID = obj.FindField("gid").GetValue();
					string name = obj.FindString("name").GetString();
					float volume = obj.FindField("volume").GetValue();

					MethodInvoker mi = delegate () {
						bool NewGroup = false;
						if (!Groups.ContainsKey(groupID)) {
							GroupControl control = new GroupControl();
							control.Group = new VolumeGroup(groupID, name, volume);
							Groups[groupID] = control;

							AddGroup(Groups[groupID]);
							NewGroup = true;
						}

						updatedData.AddObject(Groups[groupID].UpdateControl());

						if (NewGroup)
							Groups[groupID].RefreshControl();

					};
					this.Invoke(mi);
				} else if (type.Equals("Application")) {
					int groupID = obj.FindField("gid").GetValue();
					int processID = obj.FindField("pid").GetValue();

					MethodInvoker mi = delegate () {
						bool NewApp = false;

						if (!Applications.ContainsKey(processID)) {
							Applications[processID] = new VolumeControl();
							Applications[processID].Application = new VolumeApplication(processID, groupID);

							Groups[groupID].AddControl(Applications[processID]);

							NewApp = true;
						}

						updatedData.AddObject(Applications[processID].UpdateControl());

						Applications[processID].Application.Volume = obj.FindField("volume").GetValue();
						Applications[processID].Application.Muted = obj.FindField("muted").GetValue();
						Applications[processID].Application.Name = obj.FindString("name").GetString();
						Applications[processID].Application.GroupID = obj.FindField("gid").GetValue();

						if (NewApp)
							Applications[processID].RefreshControl();
					};
					this.Invoke(mi);
				}
			}
			Networking.Send(updatedData);
		}
	}
}
