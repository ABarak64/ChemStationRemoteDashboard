using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChemStationDataProviders;

namespace ChemStationDataConsumers
{
    /// <summary>
    /// An IChemStationDataConsumer that sends the most recent ChemStationStatus to a remote server via HTTP.
    /// </summary>
    public class ChemStationDataHTTPSender : IChemStationDataConsumer
    {
        //TODO: Finish this up once the server is going.
        public void ConsumeChemStationStatus(ChemStationStatus status)
        {
            var test = 1;
        }
    }
}
