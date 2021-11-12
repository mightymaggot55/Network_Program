using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Test_Server_C_Sharp
{
    //Creates socket connections when a node connects
    class Listener
    {
        private int port;
        private ManualResetEvent wait_start_up;
        private object addClient;
        Func<Socket, bool> addFunc;
        public Listener(int _port, ManualResetEvent _wait_start_up, object _addClient)
        {
            this.port = _port;
            this.wait_start_up = _wait_start_up;
            this.addClient = _addClient;
        }

        public void Start()
        {
            byte[] bytes = new byte[1024];
            bool foundIP = false;
            bool first = true;

            //Get DNS Host name
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            //Get IP from IP host info
            IPAddress iPAddress = ipHostInfo.AddressList[0];
            IPEndPoint local_end_point;

            Socket listener;
            Random rand = new Random();
            int offset = 0, count = 0;
            do
            {
                if (first)
                {
                    offset = 0;
                    first = false;
                }
                else
                {
                    offset = rand.Next(1, 100);
                    count++;
                }
                local_end_point = new IPEndPoint(iPAddress, port + offset);
                listener = new Socket(iPAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                try
                {
                    listener.Bind(local_end_point);
                    listener.Listen(100);
                    foundIP = true;
                    port = port + offset;
                }
                catch (Exception _error)
                {

                }
            }
            while (!foundIP);
            
            wait_start_up.Set();
            

            while (true)
            {
                Console.WriteLine("Waiting for a Connection...");
                Socket handler = listener.Accept();
                addFunc(handler);
            }        
        }

        public int getPort()
        {
            return port;
        }
    }
}
