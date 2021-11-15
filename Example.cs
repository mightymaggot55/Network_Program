using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.IO;

namespace Test_Server_C_Sharp
{
    class Client
    {
        public void Test_Server()
        {
            try
            {
                //Connection is established
                TcpClient client = new TcpClient("127.0.0.1", 1301);
                string messageToSend = "My Name Is Neo";

                //build buffer and prepare to send
                int byteCount = Encoding.ASCII.GetByteCount(messageToSend + 1);
                byte[] sendData = new byte[byteCount];
                sendData = Encoding.ASCII.GetBytes(messageToSend);

                //Buffer is sending
                NetworkStream stream = client.GetStream();
                stream.Write(sendData, 0, sendData.Length);
                Console.WriteLine("Sending Data to Server");
                
                //Buffer is Sent
                StreamReader sr = new StreamReader(stream);
                string response = sr.ReadLine();
                Console.WriteLine(response);

                stream.Close();
                client.Close();
                Console.ReadKey();
            }
            catch
            {

            }
        }
    }

    class server
    {
        public void Server()
        {
            TcpListener listener = new TcpListener(System.Net.IPAddress.Any, 1302);
            listener.Start();
            while(true)
            {
                Console.WriteLine("Waiting for Connection");
                TcpClient client = listener.AcceptTcpClient();
                Console.WriteLine("Client Accepted");
                NetworkStream stream = client.GetStream();
                StreamReader sr = new StreamReader(client.GetStream());
                StreamWriter sw = new StreamWriter(client.GetStream());
                try
                {
                    byte[] buffer = new byte[1024];
                    stream.Read(buffer, 0, buffer.Length);

                }
                catch(Exception _error)
                {

                }
                
            
            }
        }
    }



}
