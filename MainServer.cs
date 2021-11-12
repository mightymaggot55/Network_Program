using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;

namespace Test_Server_C_Sharp
{
    class MainServer
    {
        private ConnManager connManager;

        public MainServer(ConnManager connManager)
        {
            this.connManager = connManager;
        } 
        
        public List<String> Process_String(string v)
        {
            List<string> returnMessages = new List<string>();

            string[] strings = v.Split(' ');
            strings[0] = strings[0].ToUpper();

            switch (strings[0])
            {
                case "TEST":
                    Console.WriteLine("TEST");
                    returnMessages.Add("Test received");
                    break;
                case "FILELIST":
                    Console.WriteLine("File List Request");
                    string[] files = Directory.GetFiles("");
                    for (int i = 0; i < files.Length; i++)
                    {
                        Console.WriteLine(files[i].ToString());
                        returnMessages.Add(files[i].ToString());
                    }
                    //returnMessages.Add(files.ToString());
                    break;
                
                default:
                    returnMessages.Add("Invalid Command");
                    break;
            }
            return returnMessages;
        }
    }
}
