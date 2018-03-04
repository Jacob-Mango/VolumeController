using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VolumeController.AudioCore;
using WebSocketSharp.Server;

namespace VolumeController.Service.Networking {
    public class Networking {

        private WebSocketServer webSocketServer;

        public Networking() {
            webSocketServer = new WebSocketServer(3820);
            webSocketServer.AddWebSocketService<App>("/");
        }

        public void Start() {
            webSocketServer.Start();
        }

        public void Send(String data) {
            webSocketServer.WebSocketServices.Broadcast(data);
        }

        public void Stop() {
            webSocketServer.Stop();
        }
    }
}
