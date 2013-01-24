using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChemStationDataProviders;

namespace ChemStationDataConsumers
{
    /// <summary>
    /// Interface for interacting with any class that takes ChemStation Data and does something with it.
    /// </summary>
    public interface IChemStationDataConsumer
    {
        void ConsumeChemStationStatus(ChemStationStatus status);
    }
}
