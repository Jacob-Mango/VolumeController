using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NET {
	public partial class VolumeControl : UserControl {
		private int m_PID;
		private int m_Volume;

		public VolumeControl(int pid, int initVol, string name) {
			InitializeComponent();
			m_PID = pid;
			m_Volume = initVol;

			tkbrVolume.Value = m_Volume;
			lblVolume.Text = m_Volume.ToString();

			lblName.Text = name;
		}

		private void OnScroll(object sender, EventArgs e) {
			m_Volume = tkbrVolume.Value;
			lblVolume.Text = m_Volume.ToString();

			UpdateVolume();
		}

		private void UpdateVolume() {
			float vol = m_Volume / 100.0f;
			if (m_PID < 0)
				AudioManager.SetMasterVolume(vol);
			else
				AudioManager.SetApplicationVolume(m_PID, vol);
		}

		public void SetName(string name) {
			lblName.Name = name;
		}

		public void SetVolume(int volume) {
			m_Volume = volume;

			UpdateVolume();
		}
	}
}
