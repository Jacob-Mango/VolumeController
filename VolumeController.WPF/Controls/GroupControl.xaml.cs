using System;
using System.Collections.Generic;
using System.Diagnostics;
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
	/// Interaction logic for GroupControl.xaml
	/// </summary>
	public partial class GroupControl : UserControl {

		public VolumeGroup Group;

		private bool m_ShouldUpdate;
		private bool m_RequestRefresh;

		public double MWidth;

		public GroupControl(int groupID, string name, float volume = 1.0f) {
			InitializeComponent();
			m_RequestRefresh = true;

			Group = new VolumeGroup(groupID, name, volume);

			Loaded += GroupControl_Loaded;
		}

		private void GroupControl_Loaded(object sender, RoutedEventArgs e) {
			GroupName.Text = Group.Name;
			GroupSlider.Value = (int)(Group.Volume * 1000);
		}

		private void OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
			Group.Volume = (float)(GroupSlider.Value) / 1000F;
			m_ShouldUpdate = true;
		}

		public void RefreshControl() {
			if (!m_RequestRefresh) return;

			m_RequestRefresh = false;
			GroupName.Text = Group.Name;
			GroupSlider.Value = (int)(Group.Volume * 1000);
		}

		public RCObject UpdateControl() {
			if (m_ShouldUpdate) {
				m_ShouldUpdate = false;

				RCObject group = new RCObject("Group");
				group.AddField(RCField.Integer("gid", Group.GroupID));
				group.AddField(RCField.Float("volume", Group.Volume));
				group.AddString(RCString.Create("name", Group.Name));
				
				return group;
			}
			return null;
		}
	}
}
