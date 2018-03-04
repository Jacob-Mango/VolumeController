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
using VolumeController.Service.Networking;
using VolumeController.AudioCore;
using System.Net;

using WebSocketSharp;
using WebSocketSharp.Server;
using Newtonsoft.Json;

namespace VolumeControllerService {

    partial class MainService : ServiceBase {

        private RootObject PreviousInfo = new RootObject();
        private RootObject ControllerInfo = new RootObject();

        private Thread ThreadWorker;
        private AutoResetEvent StopRequest;

        private Networking Networking;

        private static MainService Instance;

        public MainService() {
            StopRequest = new AutoResetEvent(false);
            Networking = new Networking();

            ControllerInfo.Groups.Add(new Group(0));

            Instance = this;

            InitializeComponent();
        }

        internal void TestStartupAndStop(string[] args) {
            this.OnStart(args);
            Console.ReadLine();
            this.OnStop();
        }

        protected override void OnStart(string[] args) {
            Networking.Start();
            ThreadWorker = new Thread(UpdateData);
            ThreadWorker.Start();
        }

        protected override void OnStop() {
            Networking.Stop();
            StopRequest.Set();
            ThreadWorker.Join();
        }

        public static RootObject GetInformation() {
            return Instance.ControllerInfo;
        }

        public static void SetApplication(Application app) {
            int index = 0;
            Instance.GetApplication(Instance.ControllerInfo, app.ProcessID, out index);

            if (index >= 0) {
                Instance.ControllerInfo.Applications[index].Volume = app.Volume;
                Instance.ControllerInfo.Applications[index].Muted = app.Muted;
                Instance.ControllerInfo.Applications[index].GroupID = app.GroupID;
            }
        }

        public static void SetGroup(Group group) {
            int index = 0;
            Instance.GetGroup(Instance.ControllerInfo, group.GroupID, out index);

            if (index >= 0) {
                Instance.ControllerInfo.Groups[index].Name = group.Name;
                Instance.ControllerInfo.Groups[index].Muted = group.Muted;
                Instance.ControllerInfo.Groups[index].Volume = group.Volume;
            }
        }

        private void SendData() {
            RootObject temp = new RootObject();

            for (int i = 0; i < ControllerInfo.Applications.Count; i++) {
                try {
                    int ind = 0;
                    Application app = GetApplication(PreviousInfo, ControllerInfo.Applications[i].ProcessID, out ind);
                    if (ind >= 0 && ControllerInfo.Applications[i].Equals(app)) {
                        temp.Applications.Add(app);
                    }
                } catch (Exception) {
                    continue;
                }
            }

            for (int i = 0; i < ControllerInfo.Groups.Count; i++) {
                try {
                    int ind = 0;
                    Group group = GetGroup(PreviousInfo, ControllerInfo.Groups[i].GroupID, out ind);
                    if (ind >= 0 && ControllerInfo.Groups[i].Equals(group)) {
                        temp.Groups.Add(group);
                    }
                } catch (Exception) {
                    continue;
                }
            }

            String data = JsonConvert.SerializeObject(temp, Formatting.Indented);

            Networking.Send(data);
        }

        private Application GetApplication(RootObject obj, int ProcessID, out int i) {
            for (i = 0; i < obj.Applications.Count; i++) {
                if (obj.Applications[i].ProcessID == ProcessID) return obj.Applications[i];
            }
            i = -1;
            return null;
        }

        private Group GetGroup(RootObject obj, int GroupID, out int i) {
            for (i = 0; i < obj.Groups.Count; i++) {
                if (obj.Groups[i].GroupID == GroupID) return obj.Groups[i];
            }
            i = -1;
            return null;
        }

        private void UpdateData() {
            while (true) {
                List<ProcessInformation> pinfos = AudioManager.GetAudioApplications();
                foreach (ProcessInformation p in pinfos) {
                    int i = 0;
                    Application app = GetApplication(ControllerInfo, p.processID, out i);
                    if (app == null) {
                        app = new Application();
                        app.ProcessID = p.processID;
                        app.GroupID = 0;
                        app.Volume = 1;
                        ControllerInfo.Applications.Add(app);
                        app = GetApplication(ControllerInfo, p.processID, out i);
                    }
                    if (app.Valid) {
                        Group group = GetGroup(ControllerInfo, app.GroupID, out i);
                        AudioManager.SetApplicationVolume(app.ProcessID, (float)(app.Volume * group.Volume));

                        try {
                            Process process = Process.GetProcessById(p.processID);
                            app.Name = process.ProcessName;
                        } catch (ArgumentException) {
                            app.Name = p.name;
                        }
                    }
                }

                SendData();

                if (StopRequest.WaitOne(500)) return;
                PreviousInfo = new RootObject(ControllerInfo);
            }
        }
    }
}
