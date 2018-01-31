using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using VolumeController.AudioCore;
using VolumeController.Networking;
using VolumeController.Serialization;
using VolumeController.WPF.Controls;

namespace VolumeController.WPF {

	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window {

		public Dictionary<int, VolumeControl> Applications;
		public Dictionary<int, GroupControl> Groups;

		public MNetworking Networking;

		public MainWindow() {
			InitializeComponent();

			Applications = new Dictionary<int, VolumeControl>();
			Groups = new Dictionary<int, GroupControl>();
		}

		private void OnLoad(object sender, RoutedEventArgs e) {
			//Connect();


			CreateGroup(0, "Group 1", 1);
			CreateVolume(0, 0);
			CreateVolume(1, 0);
			CreateVolume(2, 0);
			CreateVolume(3, 0);
			//CreateGroup(1, "Group 2", 1);
			//CreateGroup(2, "Group 3", 1);
		}

		private void Connect() {
			Networking = new MNetworking(NetworkHandle, "127.0.0.1", 3000);
			Networking.Send(new RCDatabase("Login"));
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

		public void CreateGroup(int groupID, string name, float volume) {
			GroupControl control = new GroupControl(groupID, name, volume);

			Grid.SetColumn(control, MainGrid.ColumnDefinitions.Count);
			Grid.SetRow(control, 0);
			MainGrid.Children.Add(control);

			Groups[groupID] = control;

			MainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(200), MinWidth = 200, MaxWidth = 200 });
		}

		public void CreateVolume(int processID, int groupID) {
			VolumeControl control = new VolumeControl(processID, groupID);

			Grid.SetColumn(control, Groups[groupID].ApplicationGrid.ColumnDefinitions.Count);
			Grid.SetRow(control, 0);
			Groups[groupID].ApplicationGrid.Children.Add(control);

			Applications[processID] = control;

			Groups[groupID].ApplicationGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(200), MinWidth = 200, MaxWidth = 200 });



			Groups[groupID].MWidth += 200;
			Groups[groupID].ApplicationGrid.Width = Groups[groupID].MWidth;
			MainGrid.ColumnDefinitions[groupID].Width = new GridLength(Groups[groupID].MWidth);
		}

		private delegate void UIDelegateMethod();

		private void UpdateApplication() {

		}

		private void UpdateData(RCDatabase database) {
			RCDatabase updatedData = new RCDatabase("UpdateData");
			foreach (var obj in database.Objects) {
				string type = obj.GetName();
				if (type.Equals("Group")) {
					int groupID = obj.FindField("gid").GetValue();
					string name = obj.FindString("name").GetString();
					float volume = obj.FindField("volume").GetValue();

					MainGrid.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
						new UIDelegateMethod(() => {
							if (!Groups.ContainsKey(groupID)) 
								CreateGroup(groupID, name, volume);
						})
					);

					if (Groups.ContainsKey(groupID)) {
						updatedData.AddObject(Groups[groupID].UpdateControl());

						Groups[groupID].RefreshControl();
					}
				} else if (type.Equals("Application")) {
					int groupID = obj.FindField("gid").GetValue();
					int processID = obj.FindField("pid").GetValue();

					MainGrid.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
						new UIDelegateMethod(() => {
							if (!Applications.ContainsKey(processID)) 
								CreateVolume(processID, groupID);
						})
					  );

					if (Applications.ContainsKey(processID)) {
						updatedData.AddObject(Applications[processID].UpdateControl());

						Applications[processID].Application.Volume = obj.FindField("volume").GetValue();
						Applications[processID].Application.Muted = obj.FindField("muted").GetValue();
						Applications[processID].Application.Name = obj.FindString("name").GetString();
						Applications[processID].Application.GroupID = groupID;

						Applications[processID].RefreshControl();
					}
				}
			}
			Networking.Send(updatedData);
		}
	}
}
