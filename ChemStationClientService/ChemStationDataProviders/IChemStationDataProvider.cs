using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChemStationDataProviders
{
    public interface IChemStationDataProvider
    {
        ChemStationStatus GetCurrentStatus();
    }
}
