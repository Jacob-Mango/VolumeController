using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using VolumeController.AudioCore;
using VolumeController.Networking;
using VolumeController.Serialization;
using VolumeController.WPF.Controls;

namespace VolumeController.WPF.Controls {
	/// <summary>
	/// Interaction logic for VolumeControl.xaml
	/// </summary>
	public partial class VolumeControl : UserControl {

		public VolumeApplication Application;

		private bool m_ShouldUpdate;
		private bool m_RequestRefresh;

		public VolumeControl(int processID, int groupID) {
			InitializeComponent();
			m_RequestRefresh = true;

			Application = new VolumeApplication(processID, groupID);
			Application.Volume = 1;
			Application.Name = "Application";

			Loaded += GroupControl_Loaded;
		}

		private void GroupControl_Loaded(object sender, RoutedEventArgs e) {
			ApplicationName.Text = Application.Name;
			ApplicationSlider.Value = (int)(Application.Volume * 1000);
		}

		public void RefreshControl() {
			if (!m_RequestRefresh) return;
			m_RequestRefresh = false;
			ApplicationName.Text = Application.Name;
			ApplicationSlider.Value = (int)(Application.Volume * 1000);
		}

		public RCObject UpdateControl() {
			ApplicationName.Text = Application.Name;
			if (m_ShouldUpdate) {
				m_ShouldUpdate = false;

				RCObject application = new RCObject("Application");
				application.AddField(RCField.Integer("pid", Application.ProcessID));
				application.AddField(RCField.Integer("gid", Application.GroupID));
				application.AddField(RCField.Float("volume", Application.Volume));
				application.AddField(RCField.Boolean("muted", Application.Muted));
				application.AddString(RCString.Create("name", Application.Name));
				return application;
			}
			return null;
		}
	}
}
