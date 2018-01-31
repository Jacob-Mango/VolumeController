using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using CSCore.CoreAudioAPI;
using System.Runtime.InteropServices;

namespace NET {
	public partial class frmMain : Form {
		private Dictionary<int, VolumeControl> volumeControls = new Dictionary<int, VolumeControl>();

		private List<int> ignoreProccess = new List<int>();

		private Thread processThread;

		public frmMain() {
			InitializeComponent();
		}

		private void OnLoad(object sender, EventArgs e) {
			CreateControl(-1, 30, "Master Volume");

			processThread = new System.Threading.Thread(UpdateControls);
			processThread.Start();
		}

		private void OnClosing(object sender, FormClosingEventArgs e) {
			processThread.Abort();
		}

		private void CreateControl(int pid, int volume, string name) {
			try {
				VolumeControl volControl = new VolumeControl(pid, volume, name);

				volControl.Location = new System.Drawing.Point(268, 12);
				volControl.MaximumSize = new System.Drawing.Size(122, 188);
				volControl.MinimumSize = new System.Drawing.Size(122, 188);
				volControl.Size = new System.Drawing.Size(122, 188);
				volControl.Name = "vcId" + pid;

				MethodInvoker mi = delegate () {
					volumeControls.Add(pid, volControl);

					tblPanel.Controls.Add(volumeControls[pid]);
					tblPanel.SetColumn(volumeControls[pid], tblPanel.ColumnCount + 1);
				};
				this.Invoke(mi);
			} catch (InvalidComObjectException) {
				ignoreProccess.Add(pid);
			}
		}

		private void UpdateControl(int pid, string name) {
			if (volumeControls.ContainsKey(pid)) {
				int volume = (int) (100.0f * AudioManager.GetVolume(pid));

				if (pid < 0)
					Console.WriteLine(volume);

				volumeControls[pid].SetName(name);
				volumeControls[pid].SetVolume(volume);
			} else {
				int volume = (int) (100.0f * AudioManager.GetVolume(pid));
				CreateControl(pid, volume, name);
			}
		}

		private void UpdateControls() {
			while (true) {
				List<ProcessInformation> pinfos = AudioManager.GetAudioApplications();
				foreach (ProcessInformation p in pinfos)
					if (!ignoreProccess.Contains(p.processID)) {
						string name = p.name;
						try {
							name = Process.GetProcessById(p.processID).ProcessName;
						} catch (Exception) { }
						UpdateControl(p.processID, name);
					}
			}
		}
	}
}
