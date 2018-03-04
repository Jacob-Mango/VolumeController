using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VolumeControllerService;
using WebSocketSharp;
using WebSocketSharp.Server;

using VolumeController.AudioCore;

namespace VolumeController.Service.Networking {
    public class App : WebSocketBehavior {

        protected override void OnMessage(MessageEventArgs e) {
            if (e.IsText) {
                try {
                    RootObject data = JsonConvert.DeserializeObject<RootObject>(e.Data);

                    for (int i = 0; i < data.Applications.Count; i++) {
                        MainService.SetApplication(data.Applications[i]);
                    }

                    for (int i = 0; i < data.Groups.Count; i++) {
                        MainService.SetGroup(data.Groups[i]);
                    }
                } catch (Exception) {

                }
            }
        }

        protected override void OnOpen() {
            String data = JsonConvert.SerializeObject(new RootObject(MainService.GetInformation()), Formatting.Indented);

            Send(data);
        }
    }
}
