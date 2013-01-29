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
    /// <summary>
    /// An IChemStationDataProvider that sources its ChemStation Data via DDE.
    /// </summary>
    public class ChemStationDDEDataProvider : IChemStationDataProvider 
    {
        private const string _baseDDEAppName = "hpcore";
        private const string _DDETopicName = "CPNOWAIT";
        private const string _windowsAppName = "CHEMMAIN";

        // This dictionary maps the various ChemStation variables to properties on the ChemStationStatus model object.
        private static readonly Dictionary<string, PropertyInfo> _chemStationVariableNameToPropertyMap = new Dictionary<string, PropertyInfo>()
        {
            {"_SEQFILE$", typeof(ChemStationStatus).GetProperty("SequenceName")},
            {"_METHFILE$", typeof(ChemStationStatus).GetProperty("MethodName")},
            {"_METHODON", typeof(ChemStationStatus).GetProperty("MethodRunning")},
            {"_SEQUENCEON", typeof(ChemStationStatus).GetProperty("SequenceRunning")},
            {"ACQSTATUS$", typeof(ChemStationStatus).GetProperty("Status")}
        };

        /// <summary>
        /// Gets the current status of the ChemStation app via DDE and maps it to our model object.
        /// </summary>
        /// <returns>The ORMed ChemStation Status.</returns>
        public ChemStationStatus GetCurrentStatus()
        {
            var status = new ChemStationStatus();
            status.Time = DateTime.Now;
            var appName = string.Empty;
            try
            {
                appName = GetChemStationAppName();
            }
            catch (ApplicationException e)
            {
                status.Status = e.Message;
                status.MethodName = string.Empty;
                status.SequenceName = string.Empty;
                return status;
            }

            using (DdeClient client = new DdeClient(appName, _DDETopicName))
            {
                client.Connect();
                foreach (var variable in _chemStationVariableNameToPropertyMap)
                {
                    var rawVariable = client.Request(variable.Key, 60000);

                    // Use reflection to map the raw variable just extracted from ChemStation to the model object.
                    if (variable.Value.PropertyType == typeof(bool))
                    {
                        var val = rawVariable[0] == '1' ? true : false;
                        _chemStationVariableNameToPropertyMap[variable.Key].SetValue(status, val, null);
                    }
                    else
                    {
                        // String variables tend to come out of ChemStation with a bizarre amount of control characters followed by jibberish at the end.
                        var val = rawVariable.Remove(rawVariable.IndexOf(rawVariable.Where(c => char.IsControl(c)).First()));
                        _chemStationVariableNameToPropertyMap[variable.Key].SetValue(status, val, null);
                    }
                }
            }
            return status;
        }

        /// <summary>
        /// This method may potentially be refactored into its own interface/class to deal with other versions of ChemStation. The
        ///    version currently being used requires the DDE app name to be composed of 'hpcore' + the process number of the instance of
        ///    ChemStation that is currently online (connected to an instrument).
        /// </summary>
        /// <returns>The DDE App name used to connect to ChemStation via DDE as a string.</returns>
        private string GetChemStationAppName()
        {
            var potentialChemStations = Process.GetProcesses().Where(x => x.ProcessName.ToUpper().Contains(_windowsAppName));
            foreach (var chemStation in potentialChemStations)
            {
                using (DdeClient client = new DdeClient(_baseDDEAppName + chemStation.Id.ToString(), _DDETopicName))
                {
                    client.Connect();
                    var offline = client.Request("_OFFLINE", 60000)[0] == '1' ? true : false;
                    if (!offline)   return (_baseDDEAppName + chemStation.Id.ToString());
                }
            }
            throw new ApplicationException("Unable to find an online ChemStation process.");
        }
    }
}
