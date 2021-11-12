using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test_Server_C_Sharp
{
    //Initiation point for the program as a whole, sets up
    //the type of program running
    class UserInterface
    {
        private ConnManager conn_manager;
        public UserInterface()
        {
        }

        public void boot_server()
        {
            int port = 11000;
            this.conn_manager = new ConnManager(port, false);
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Starting Directory Server...");
            UserInterface program = new UserInterface();
            program.boot_server();
        }
    }
}
