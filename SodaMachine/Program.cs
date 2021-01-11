using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SodaMachine
{
    class Program
    {
        static void Main(string[] args)
        {
            Simulation simulation = new Simulation();
            simulation.Simulate();

           // Customer test = new Customer();
            //Cola cola = new Cola();
            //test.GatherCoinsFromWallet(cola);

        }
    }
}
