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
            int server_port = 11000;
            this.conn_manager = new ConnManager(server_port, true);
            Console.WriteLine("Server: ONLINE");
        }

        public void boot_client()
        {
            int client_port = 12000;
            this.conn_manager = new ConnManager(client_port, false);
            Console.WriteLine("Client: ONLINE");
        }

        static void Main(string[] args)
        {
            try
            {
                bool flag = false;
                while(flag == false)
                {
                    Console.WriteLine("Start Server or CLient?");
                    string Q;
                    Q = Console.ReadLine();
                    Q.ToUpper();
                    if (Q == "CLIENT" || Q == "client")
                    {
                        flag = true;
                        Console.WriteLine("Starting up Client");
                        UserInterface program = new UserInterface();
                        program.boot_client();
                    }
                    else if (Q == "SERVER" || Q == "server")
                    {
                        flag = true;
                        Console.WriteLine("Starting up Server");
                        UserInterface program = new UserInterface();
                        program.boot_server();
                    }
                    else
                    {
                        Console.WriteLine(Q);
                        Console.WriteLine("Invalid Input. Try Again...");
                    }
                }
            }
            catch (Exception _error)
            {

            }
        }
    }
}

