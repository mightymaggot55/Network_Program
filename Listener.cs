using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Test_Server_C_Sharp
{
    //Creates socket connections when a node connects
    class Listener
    {
        private int client_port;
        private ManualResetEvent wait_start_up;
        private object addClient;

        public Listener(int client_port, ManualResetEvent wait_start_up, object addClient)
        {
            this.client_port = client_port;
            this.wait_start_up = wait_start_up;
            this.addClient = addClient;
        }

        public void Start()
        {
            
        }

        public int getPort()
        {
            return client_port;
        }
    }
}
