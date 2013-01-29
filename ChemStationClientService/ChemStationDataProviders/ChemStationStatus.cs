using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChemStationDataProviders
{
    /// <summary>
    /// The object model of the ChemStation current status.
    /// </summary>
    public class ChemStationStatus
    {
        public string Status { get; set;  }
        public DateTime Time { get; set; }
        public string MethodName { get; set;  }
        public bool MethodRunning { get; set; }
        public string SequenceName { get; set; }
        public bool SequenceRunning { get; set; }

        public ChemStationStatus() { }

        public ChemStationStatus(string status, DateTime time, string methName, bool methOn, string seqName, bool seqOn)
        {
            this.Status = status;
            this.Time = time;
            this.MethodName = methName;
            this.MethodRunning = methOn;
            this.SequenceName = seqName;
            this.SequenceRunning = seqOn;
        }
    }
}
