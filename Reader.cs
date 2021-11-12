using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test_Server_C_Sharp
{
    //Processes data from the network into the read queue
    class Reader
    {
        ConnectionState state;
        const int buffer_size = 1024;
        int offset;
        bool first_fill;
        bool over_spill;
        byte[] bytes = new byte[buffer_size];

        public Reader(ConnectionState state)
        {
            this.state = state;
        }

        public void Start()
        {
            string message = "";
            int bytesRead = 0;
            offset = 0;
            first_fill = true;
            over_spill = false;
            int length = 0;
            bool continuation = false;

            while(true)
            {
                try
                {
                    //we have never read into the buffer before, read to fill it
                    if (first_fill)
                    {
                        bytesRead = state.sock.Receive(bytes);
                        offset = 0;
                        length = 0;
                        continuation = false;
                        first_fill = false;
                    }
                    do
                    {
                        if (over_spill)
                        {
                            Array.Clear(bytes, 0, buffer_size);
                            bytesRead = state.sock.Receive(bytes);
                            message = message + Encoding.ASCII.GetString(bytes, 2, length);
                            offset = length;
                            over_spill = false;
                            length = 0;
                        }
                        else
                        {
                            while (offset < buffer_size && bytes[offset] == 0)
                            {
                                offset += 1;
                            }
                            if (offset == buffer_size)
                            {
                                Array.Clear(bytes, 0, buffer_size);
                                bytesRead = state.sock.Receive(bytes);
                                offset = 0;
                                length = 0;
                            }
                            length = (int)bytes[offset];
                            continuation = bytes[offset + 1] > 0 ? true : false;

                            if (offset + length + 2 < buffer_size)
                            {
                                message = message + Encoding.ASCII.GetString(bytes, offset + 2, length);
                                offset = offset + 2 + length;
                                length = 0;
                            }
                            else
                            {
                                int chunk = buffer_size - offset - 2;
                                message = message + Encoding.ASCII.GetString(bytes, offset + 2, chunk);
                                length -= chunk;
                                over_spill = true;
                            }
                        }
                    }
                    while (over_spill || continuation);

                        state.Enqueue_Read(message);
                        message = "";
                        bytesRead = 0;
                        continuation = false;
                        over_spill = false;
                        length = 0;
                    
                }
                catch (Exception _error)
                {
                    state.kill = true;
                    break;
                }
                
                
            }
        }
    }
}
