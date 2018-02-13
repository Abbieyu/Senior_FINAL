using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SeniorTester.ConTester;
namespace SeniorTester
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Game started at : " + DateTime.Now);
            var prox = new NsashServicesClient();
            prox.ChooseGame(2);
            int index = prox.JoinGame("Split&Steal", "a");
            
                Console.WriteLine("index = "+index);
            int status = prox.CheckGameStatus("Split&Steal", "a");
            while(status==-1)
            {
                status = prox.CheckGameStatus("Split&Steal", "a");
            }
            Console.WriteLine("GID is : " + status);

        }
    }
}
