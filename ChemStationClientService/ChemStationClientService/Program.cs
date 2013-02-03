using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChemStationClients;
using System.Configuration;
using System.ServiceProcess;

namespace ChemStationClientService
{
    class Program : ServiceBase
    {
        private ChemStationClient _client;

        public Program()
        {
            this.ServiceName = "ChemStationClientService";
            this.EventLog.Source = this.ServiceName;
            this.EventLog.Log = "Application";
        }

        static void Main(string[] args)
        {
            ServiceBase.Run(new Program());
        }

        protected override void OnStart(string[] args)
        {
            base.OnStart(args);
            _client = new ChemStationClient();
        }

        protected override void OnStop()
        {
            base.OnStop();
            _client = null;
        }
    }
}
