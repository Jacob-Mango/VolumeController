using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.InteropServices;

namespace VolumeController.Service {
    public class Group {
        public int GroupID { get; set; }
        public string Name { get; set; }
        public double Volume { get; set; }
        public bool Muted { get; set; }

        public Group(int groupID) {
            Name = "Group " + groupID;
            Volume = 1;
            Muted = false;
        }

        public override bool Equals(Object obj) {
            if (obj is Group) {
                Group group = (Group)obj;
                if (group.GroupID == GroupID) return false;
                if (group.Volume == Volume) return false;
                if (group.Name == Name) return false;
                return true;
            }
            return false;
        }
    }

    public class Application {
        public int ProcessID { get; set; }
        public string Name { get; set; }
        public int GroupID { get; set; }

        public double Volume {
            get {
                return (double)AudioManager.GetApplicationVolume(ProcessID);
            }
            set {
                AudioManager.SetApplicationVolume(ProcessID, (float)value);
            }
        }

        public bool Valid {
            get {
                try {
                    return Volume == 0 ? true : true;
                } catch (InvalidComObjectException) {
                    return false;
                }
            }
        }

        public bool Muted {
            get {
                return AudioManager.GetApplicationMute(ProcessID);
            }
            set {
                AudioManager.SetApplicationMute(ProcessID, value);
            }
        }

        public override bool Equals(Object obj) {
            if (obj is Application) {
                Application app = (Application)obj;
                return GroupID == app.GroupID && ProcessID == app.ProcessID && Valid == app.Valid && Volume == app.Volume;
            }
            return false;
        }
    }

    public class IconImage {
        public string Image { get; set; }
        public int ProcessID { get; set; }

        public IconImage(int processID) {
            ProcessID = processID;

            string path = AudioManager.GetApplicationIconPath(ProcessID);
            byte[] imageArray = System.IO.File.ReadAllBytes(@path);
            Image = Convert.ToBase64String(imageArray);
        }
    }

    public class RootObject {
        public List<Group> Groups { get; set; }
        public List<Application> Applications { get; set; }
        public List<IconImage> Icons { get; set; }

        public RootObject() {
            Groups = new List<Group>();
            Applications = new List<Application>();
            Icons = new List<IconImage>();
        }
        
        public RootObject(RootObject obj) {
            Groups = new List<Group>();
            Applications = new List<Application>();
            Icons = new List<IconImage>();

            for (int i = 0; i < obj.Groups.Count; i++) {
                Groups.Add(obj.Groups[i]);
            }
            for (int i = 0; i < obj.Applications.Count; i++) {
                Applications.Add(obj.Applications[i]);
            }
            for (int i = 0; i < obj.Icons.Count; i++) {
                Icons.Add(obj.Icons[i]);
            }
        }
    }
}
