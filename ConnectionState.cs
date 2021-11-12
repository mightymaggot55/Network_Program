using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Test_Server_C_Sharp
{
    public class ConnectionState
    {
        AutoResetEvent is_connectionInProgress;
        ManualResetEvent event_ReadQueue;
        ManualResetEvent event_WriteQueue;
        Queue<string> readQueue;
        Queue<string> writeQueue;
        public Socket sock;
        public bool kill = false;
        public ConsoleColor Console_Color;

        public ConnectionState()
        {
            event_ReadQueue = new ManualResetEvent(true);
            event_WriteQueue = new ManualResetEvent(true);
            readQueue = new Queue<string>();
            writeQueue = new Queue<string>();
            sock = null;

            Random r = new Random();
            Console_Color = (ConsoleColor)r.Next(0, 16);
        }

        public bool Has_Read()
        {
            if(readQueue.Count > 0)
            {
                return true;
            }
            return false;
        }

        public bool Has_Write()
        {
            if(writeQueue.Count > 0)
            {
                return true;
            }
            return false;
        }

        public int Enqueue_Read(string temp)
        {
            event_ReadQueue.WaitOne();
            event_ReadQueue.Reset();
            readQueue.Enqueue(temp);
            event_ReadQueue.Set();
            return 0;
        }

        public int Enqueue_Write(string temp)
        {
            event_ReadQueue.WaitOne();
            event_ReadQueue.Reset();
            writeQueue.Enqueue(temp);
            event_WriteQueue.Set();
            return 0;
        }

        public string Dequeue_Read()
        {
            string temp;
            event_ReadQueue.WaitOne();
            event_ReadQueue.Reset();
            temp = readQueue.Dequeue();
            event_ReadQueue.Set();
            return temp;
        }

        public string Dequeue_Write()
        {
            string temp;
            event_WriteQueue.WaitOne();
            event_WriteQueue.Reset();
            temp = writeQueue.Dequeue();
            event_WriteQueue.Set();
            return temp;
        }
    }
}
