using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NDde.Client;
using System.Diagnostics;
using System.Reflection;

namespace ChemStationDataProviders
{
    public class ChemStationDDEDataProvider : IChemStationDataProvider 
    {
        private const string _baseAppName = "hpcore";

        private static readonly Dictionary<string, PropertyInfo> _chemStationVariableNameToPropertyMap = new Dictionary<string, PropertyInfo>()
        {
            {"_SEQFILE$", typeof(ChemStationStatus).GetProperty("SequenceName")},
            {"_METHFILE$", typeof(ChemStationStatus).GetProperty("MethodName")},
            {"_METHODON", typeof(ChemStationStatus).GetProperty("MethodRunning")},
            {"_SEQUENCEON", typeof(ChemStationStatus).GetProperty("SequenceRunning")},
            {"ACQSTATUS$", typeof(ChemStationStatus).GetProperty("Status")}
        };


        public ChemStationStatus GetCurrentStatus()
        {
            var status = new ChemStationStatus();
            using (DdeClient client = new DdeClient(GetChemStationAppName(), "CPNOWAIT"))
            {
                client.Connect();
                foreach (var variable in _chemStationVariableNameToPropertyMap)
                {
                    var rawVariable = client.Request(variable.Key, 60000);

                    if (variable.Value.PropertyType == typeof(bool))
                    {
                        var val = rawVariable[0] == '1' ? true : false;
                        _chemStationVariableNameToPropertyMap[variable.Key].SetValue(status, val, null);
                    }
                    else
                    {
                        var val = rawVariable.Remove(rawVariable.IndexOf(rawVariable.Where(c => char.IsControl(c)).First()));
                        _chemStationVariableNameToPropertyMap[variable.Key].SetValue(status, val, null);
                    }
                }
            }
            return status;
        }

        private string GetChemStationAppName()
        {
            var potentialChemStations = Process.GetProcesses().Where(x => x.ProcessName.ToUpper().Contains("CHEMMAIN"));
            foreach (var chemStation in potentialChemStations)
            {
                using (DdeClient client = new DdeClient(_baseAppName + chemStation.Id.ToString(), "CPNOWAIT"))
                {
                    client.Connect();
                    var offline = client.Request("_OFFLINE", 60000)[0] == '1' ? true : false;
                    if (offline)   return (_baseAppName + chemStation.Id.ToString()); // TODO change this logic
                }
            }
            throw new ApplicationException("Unable to find an online ChemStation process.");
        }
    }
}
