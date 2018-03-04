using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VolumeController.AudioCore {
	public class VolumeGroup {

		public int GroupID;
		public string Name;
		public float Volume;

		public VolumeGroup(int groupID, string name, float volume = 1.0f) {
			GroupID = groupID;
			Name = name;
			Volume = volume;
		}

	}
}