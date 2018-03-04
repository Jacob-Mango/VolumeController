using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

#if (SERVICE)
using VolumeController.Service;
#endif

namespace VolumeController.AudioCore {
	public class VolumeApplication {

		public int ProcessID;
		public String Name;
		public int GroupID;

#if (SERVICE)
		public bool Valid {
			get {
				try {
					return Volume == 0 ? true : true;
				} catch (InvalidComObjectException) {
					return false;
				}
			}
		}
#else
		public bool Valid;
#endif
		
		public float Volume;

#if (SERVICE)
		public bool Muted {
			get {
				return AudioManager.GetApplicationMute(ProcessID);
			}
			set {
				AudioManager.SetApplicationMute(ProcessID, value);
			}
		}
#else
		public bool Muted;
#endif

#if (SERVICE)
		public string IconPath {
			get {
				return AudioManager.GetApplicationIconPath(ProcessID);
			}
		}
#else
		public string IconPath;
#endif

		public VolumeApplication(int processID, int groupID) {
			ProcessID = processID;
			GroupID = groupID;
		}
	}
}
