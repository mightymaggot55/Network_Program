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
        public ConnectionState _state;
        private Reader _reader;

        public Guid _g;
        private Func<Connection, int> processing_function;

        public Connection(Socket sock, Func<Connection, int> processing_function)
        {
            _state = new ConnectionState();
            _state.sock = sock;
            _g = Guid.NewGuid();
            _reader = new Reader(_state);
            this.processing_function = processing_function;
        }

        public void Start()
        {
            Thread reader_thread = new Thread(new ThreadStart(_reader.Start));
            reader_thread.Start();

            //kill loop
            while(!_state.kill)
            {
                Process_Write();
                processing_function(this);
            }

            reader_thread.Join();
            _state.sock.Shutdown(SocketShutdown.Both);
            _state.sock.Close();
        }

        public void Write(string message)
        {
            _state.Enqueue_Write(message);
        }

        public void Process_Write()
        {
            while(_state.Has_Write())
            {
                string message = _state.Dequeue_Write();
                message = message.Replace("\0", string.Empty);
                int byte_count = 0;
                int sets = 0;
                byte[] send_buffer = new byte[256];
                byte_count = message.Length;
                byte[] temp = Encoding.ASCII.GetBytes(message);

                try
                {
                    while(byte_count > 254)
                    {
                        //Set message buffer components
                        send_buffer[0] = 254;
                        send_buffer[1] = 1;

                        //Copy Across the first 254 bits of the message
                        Buffer.BlockCopy(temp, sets * 254, send_buffer, 2, 254);

                        //incrememnt the number of message sets and decrease the byte counter for this message
                        sets++;
                        byte_count -= 254;

                        //add this to the send buffer in order
                        _state.sock.Send(send_buffer);

                        //clear the array - we dont know how many bytes we will have to deal with in future attempts
                        Array.Clear(send_buffer, 0, 256);

                    }
                    //last chunk of the message - set the byte count to a real number and deal and contunuation to false
                    send_buffer[0] = (byte)byte_count;
                    send_buffer[1] = 0;

                    //copy at offset sets * 254 into sendBuffer offset at 2
                    Buffer.BlockCopy(temp, sets * 254, send_buffer, 2, byte_count);
                    _state.sock.Send(send_buffer);
                }
                catch(Exception _error)
                {
                    _state.kill = true;
                    break;
                }
            }
        }

        public bool Dead()
        {
            return true;
        }

        public void Set_Kill()
        {
            _state.kill = true;
        }

        

       
    }
}
