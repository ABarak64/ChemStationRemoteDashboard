using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChemStationDataProviders;

namespace ChemStationClients
{
    public class ChemStationClient
    {
        private readonly IChemStationDataProvider _provider;

        public ChemStationClient()
        {
            _provider = new ChemStationDDEDataProvider();
            var status = _provider.GetCurrentStatus().Status;
        }

        public string DebugReport()
        {
            var sb = new StringBuilder();
            try
            {
                var status = _provider.GetCurrentStatus();
                sb.AppendLine("Status: " + status.Status);
                sb.AppendLine("Method: " + status.MethodName);
                sb.AppendLine("Method Running: " + status.MethodRunning);
                sb.AppendLine("Sequence: " + status.SequenceName);
                sb.AppendLine("Sequence Running: " + status.SequenceRunning);
            }
            catch (Exception e)
            {
                sb.AppendLine("Exception occurred: " + e.Message);
            }
            return sb.ToString();
        }
    }
}
