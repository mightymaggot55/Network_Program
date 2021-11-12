using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Test_Server_C_Sharp
{
    class Connection
    {
        public Connection_State _state;
        private Reader _reader;

        public Guid _g;
        private Func<Connection, int> processing_function;

        public Connection(Socket sock, Func<Connection, int> processing_function)
        {

        }

        public void Start()
        {

        }

        public void Write(string message)
        {

        }

        public void Process_Write()
        {

        }

        public bool Dead()
        {
            return true;
        }

        public void Set_Kill()
        {

        }

        

       
    }
}
