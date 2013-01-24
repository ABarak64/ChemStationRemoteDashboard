using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChemStationDataProviders;
using ChemStationDataConsumers;
using System.Timers;
using System.Configuration;

namespace ChemStationClients
{
    /// <summary>
    /// This class manages the rules for how an IChemStationDataProvider and IChemStationDataConsumer interact with each other.
    /// In this case, we want to regularly pole the IChemStationDataProvider, and regularly pass the data to the IChemStationDataConsumer.
    /// </summary>
    public class ChemStationClient
    {
        private readonly IChemStationDataProvider _provider;
        private readonly IChemStationDataConsumer _consumer;
        private readonly Timer _transmissionTimer;
        private DateTime _lastSyncTime;
        private ChemStationStatus _status;

        private ChemStationStatus Status
        {
            get 
            {
                return _status;
            }
            // When the status gets set, determine if the consumer needs the new status. If the status differs from the previous one
            //    or the maximum number of minutes between status consumption has elapsed, give it to the consumer.
            set
            {
                int maximumMinutesBetweenSync;
                int.TryParse(ConfigurationManager.AppSettings["MaximumMinutesBetweenSync"], out maximumMinutesBetweenSync);

                if (_status == null ||
                    _status.MethodName != value.MethodName ||
                    _status.SequenceName != value.SequenceName ||
                    _status.Status != value.Status ||
                    _status.MethodRunning != value.MethodRunning ||
                    _status.SequenceRunning != value.SequenceRunning ||
                    (DateTime.Now - _lastSyncTime).Minutes >= maximumMinutesBetweenSync)
                {
                    try
                    {
                        _consumer.ConsumeChemStationStatus(value);
                        _lastSyncTime = DateTime.Now;
                    }
                    // At this point, I'm not interested in diagnosing why the consumer fails. Just suppress it and continue on.
                    catch (Exception e) 
                    {
                        // Force the next status setting to attempt to resend the status to the consumer.
                        _lastSyncTime.AddMinutes(maximumMinutesBetweenSync);
                    }
                }
                _status = value;
            }
        }

        public ChemStationClient() : this(new ChemStationDDEDataProvider(), new ChemStationDataHTTPSender())
        {
        }

        /// <summary>
        /// Main constructor. Sets the provider and consumer and configures the timer to the externally-set periodic provider polling.
        /// </summary>
        /// <param name="provider">The IChemStationDataProvider that will give us the ChemStationStatus.</param>
        /// <param name="consumer">The IChemStationDataConsumer that will consume said ChemStationStatus.</param>
        public ChemStationClient(IChemStationDataProvider provider, IChemStationDataConsumer consumer)
        {
            _provider = provider;
            _consumer = consumer;

            int intervalMinutes;
            int.TryParse(ConfigurationManager.AppSettings["IntervalMinutes"], out intervalMinutes);

            _lastSyncTime = DateTime.MinValue;
            _transmissionTimer = new Timer(1000 * 60 * intervalMinutes);
            _transmissionTimer.Elapsed += (s, e) => { GetData(); };
            _transmissionTimer.Enabled = true;
        }

        /// <summary>
        /// Method to encapsulate all the calls necessary for polling the provider.
        /// </summary>
        public void GetData()
        {
            Status = _provider.GetCurrentStatus();
        }
    }
}
