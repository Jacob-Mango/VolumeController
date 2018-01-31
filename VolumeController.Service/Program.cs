using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace VolumeController.Service {
	static class Program {
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		static void Main() {
			if (Environment.UserInteractive) {
				VolumeControllerService.MainService service1 = new VolumeControllerService.MainService();
				service1.TestStartupAndStop(new string[] { });
			} else {

				ServiceBase[] ServicesToRun;
				ServicesToRun = new ServiceBase[]
				{
					new VolumeControllerService.MainService()
				};
				ServiceBase.Run(ServicesToRun);
			}
		}
	}
}