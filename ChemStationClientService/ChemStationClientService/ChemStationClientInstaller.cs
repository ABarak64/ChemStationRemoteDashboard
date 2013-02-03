using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration.Install;
using System.ComponentModel;
using System.ServiceProcess;

namespace ChemStationClientService
{
    [RunInstaller(true)]
    public class ChemStationClientInstaller : Installer
    {
        public ChemStationClientInstaller()
        {
            var processInstaller = new ServiceProcessInstaller();
            var serviceInstaller = new ServiceInstaller();

            processInstaller.Account = ServiceAccount.LocalSystem;

            serviceInstaller.DisplayName = "ChemStationClientService";
            serviceInstaller.StartType = ServiceStartMode.Manual;

            serviceInstaller.ServiceName = "ChemStationClientService";

            this.Installers.Add(processInstaller);
            this.Installers.Add(serviceInstaller);
        }
    }
}
