using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChemStationDataProviders
{
    /// <summary>
    /// Interface for interacting with any class that is capable of provided ChemStation data.
    /// </summary>
    public interface IChemStationDataProvider
    {
        ChemStationStatus GetCurrentStatus();
    }
}
