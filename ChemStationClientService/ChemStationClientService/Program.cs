using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChemStationClients;

namespace ChemStationClientService
{
    class Program
    {
        static void Main(string[] args)
        {
            var test = new ChemStationClient();
            Console.WriteLine(test.DebugReport());
            Console.Read();
        }
    }
}
