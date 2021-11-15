using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.IO;

namespace Test_Server_C_Sharp
{
    //Central point for managing all connections 
    //and provides the callback functions for reading
    class ConnManager
    {
        private List<Connection> client_connections;
        private List<Connection> server_connections;
        private Dictionary<string, Connection> servers;
        private Dictionary<string, Connection> clients;
        private List<KeyValuePair<IPAddress, int>> server_locations;

        private ManualResetEvent client_wait;
        private int base_port;
        private int server_port;
        private int client_port;
        private Listener client_listener = null;
        private Listener server_listener = null;
        Guid myID;
        private MainServer main_server;
        private Func<string, List<string>> state_function;
        private bool root;
        private System.Timers.Timer a_timer;
        private System.Timers.Timer server_timer;
        private System.Timers.Timer file_timer;

        private void Load_Known_servers()
        {
            //This function gets a text file for known servers and reads it into the program
            string line;
            string[] stringy;
            // Read the file and display it line by line.  
            System.IO.StreamReader file = new System.IO.StreamReader("Known_Servers.txt");
            while ((line = file.ReadLine()) != null)
            {
                stringy = line.Split(" ");
                server_locations.Add(new KeyValuePair<IPAddress, int>(IPAddress.Parse(stringy[0]), int.Parse(stringy[1])));
            }
            file.Close();
        }

        private void Heart_Beat()
        {
            //This uses timers to monitor the program
            
        }

        private void OnTimeEvent(Object source, System.Timers.ElapsedEventArgs _event)
        {
            //This function is used to track events using a timer
            //writes to each server connection a time
        }

        private void Run_Server_Synchronisation()
        {
            //creates a timer which is used to track servers sychronisation

        }

        private void Run_File_Synchronisation()
        {
            //Used to track file synchronisation - uses timer to track events
        }

        private void On_Timed_Synch(Object source, System.Timers.ElapsedEventArgs _event)
        {
            //Used to record Server synchronisation Events
        }

        private void On_Time_File(Object source, System.Timers.ElapsedEventArgs _event)
        {
            //Write to console File Synchronisation Event and the time it occurred
        }

        public bool Add_Client(Socket sock)
        {
            Connection conn;
            conn = new Connection(sock, processFS);

            Thread conn_thread = new Thread(new ThreadStart(conn.Start));
            conn_thread.Start();
            client_connections.Add(conn);
            return true;
        }

        public bool Add_Server(Socket sock)
        {
            Connection conn;
            conn = new Connection(sock, processFS);

            Thread conn_thread = new Thread(new ThreadStart(conn.Start));
            conn_thread.Start();
            server_connections.Add(conn);
            return true;
        }

        public ConnManager(int port, bool isServer)
        {
            //create list of server locations {IP, port}
            server_locations = new List<KeyValuePair<IPAddress, int>>();
            Load_Known_servers();
            //Heart_Beat();
            Run_Server_Synchronisation();
            Run_File_Synchronisation();
            root = false;

            //Guid is a universal identifier - used for encryption
            myID = Guid.NewGuid();

            //Dictonary<,>: store set of key-value pairs
            //creates a new dictonary to store server information
            servers = new Dictionary<string, Connection>();
            //creates a new dictonary to store client information
            clients = new Dictionary<string, Connection>();

            //List<>: simply store set of items
            //creates new list for client connections
            client_connections = new List<Connection>();
            //creates new list for server conenctions
            server_connections = new List<Connection>();

            //handles main processing functions for the server
            main_server = new MainServer(this);

            //This can be improved - set up better system for client port
            this.base_port = port;
            this.server_port = port;
            this.client_port = port;

            client_wait = new ManualResetEvent(false);
            client_wait.Reset();

            if (isServer == true)
            {                
                Start_Server_Listener();
            }
            else if(isServer == false)
            {                
                Start_Client_Listener();
            }
        }

        public void Start_Client_Listener()
        {
           try
           {
                //client_wait.WaitOne(); //blocks thread until WaitHandle receives signal
                                       //Fire up a listener to create new threads
                ManualResetEvent wait_start_up = new ManualResetEvent(false);
                //create new client listener
                client_listener = new Listener(client_port, wait_start_up, clients);
                //resets flag from wait_start_up
                wait_start_up.Reset();

                //open listening thread
                Thread listen_thread = new Thread(new ThreadStart(client_listener.Start));
                //start listener thread
                listen_thread.Start();
                //waits for flag
                wait_start_up.WaitOne();
                //returns the clients port
                client_port = client_listener.getPort();

                Console.WriteLine("Client Port: " + client_port);
                if (!root)
                {
                    servers["ROOT"]._state.Enqueue_Write("CLIENTPORT<FS>" + Dns.GetHostEntry(Dns.GetHostName()).AddressList[0].ToString() + "<FS>" + client_port);
                }
            }
            catch (Exception _error)
            {

            }
        }

        public void Start_Server_Listener()
        {
            bool set_root = true;
            root = true;
            foreach(KeyValuePair<IPAddress, int> server_list in server_locations)
            {
                try
                {
                    //create new End Point
                    IPEndPoint localEndPoint = new IPEndPoint(server_list.Key, server_list.Value);
                    
                    //create new socket to send
                    Socket sender = new Socket(server_list.Key.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                    
                    //connect Socket sender to new local end point
                    sender.Connect(localEndPoint);
                    
                    //create new connection - uses sender socket 
                    Connection connection = new Connection(sender, processFS);
                    
                    //create new thread
                    Thread conn_thread = new Thread(new ThreadStart(connection.Start));
                    
                    //start new connection thread
                    conn_thread.Start();
                    
                    //add to server connection lists the connection 
                    server_connections.Add(connection);
                    root = false;

                    if (set_root)
                    {
                        servers.Add("ROOT", connection);
                    }
                    else
                    {
                        servers.Add(Guid.NewGuid().ToString(), connection);
                    }
                }
                catch (Exception _event)
                {
                    Console.WriteLine(_event);
                }            
            }
        //Fire up listener to create new threads
        ManualResetEvent wait_start_up = new ManualResetEvent(false);
        
        server_listener = new Listener(server_port, wait_start_up, servers);
        wait_start_up.Reset();
        Thread listen_thread = new Thread(new ThreadStart(server_listener.Start));
            listen_thread.Start();

            wait_start_up.WaitOne();
            server_port = server_listener.getPort();

        if(!root)
            {
                Console.WriteLine(((IPEndPoint)servers["ROOT"]._state.sock.RemoteEndPoint).Address.ToString() + ":" + ((IPEndPoint)servers["ROOT"]._state.sock.RemoteEndPoint).Port.ToString());
            }
            Console.WriteLine("Server Port: " + server_port);
            wait_start_up.Set();
        }

        public int processFS(Connection conn)
        {
            if(conn._state.Has_Read())
            {
                string message = conn._state.Dequeue_Read();
                string[] message_array = message.Split(""); //Check here - decide what we need to split the string up

                switch(message_array[0])
                {
                    case "REGISTER":
                        Console.WriteLine("processFS:Register");
                        servers.Add(message_array[1], conn);
                        
                        foreach(KeyValuePair<string, Connection> s in servers)
                        {
                            if(s.Key != message_array[1])
                            {
                                Console.WriteLine("Writing new connection to: " + s.Key.ToString());
                                //check this - see what it does
                                s.Value._state.Enqueue_Write("NEWFS<FS>" + message_array[1] + "<FS>" + message_array[2] + "<FS>" + message_array[3]);
                            }
                        }
                        break;
                    case "NEWFS":
                            bool aleady_has = false;
                            
                        foreach(KeyValuePair<string, Connection> s in servers)
                        {
                            Console.WriteLine("CHECKING FOR SERVER...");
                            if(s.Key == message_array[1])
                            {
                                aleady_has = true;
                            }
                        }
                        if(!aleady_has)
                        {
                            int port = int.Parse(message_array[3]);
                            IPAddress address = IPAddress.Parse(message_array[2]);
                            IPEndPoint local_end_point = new IPEndPoint(address, port);

                            Socket sender = new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                            sender.Connect(local_end_point);

                            Connection connection = new Connection(sender, this.processFS);
                            servers.Add(message_array[1], connection);

                            Thread client_thread = new Thread(new ThreadStart(connection.Start));
                            client_thread.Start();
                        }
                        foreach(KeyValuePair<string, Connection> s in servers)
                        {
                            Console.WriteLine(s.Key.ToString() + ":" + s.Value.ToString());
                        }
                        break;
                    default:
                        Console.ForegroundColor = conn._state.Console_Color;
                        Console.WriteLine(((IPEndPoint)conn._state.sock.RemoteEndPoint).Address.ToString() + ":" + ((IPEndPoint)conn._state.sock.RemoteEndPoint).Port.ToString() + " " + message);
                        Console.ResetColor();
                        break;
                }
            }
            else
            {
                Thread.Sleep(1);
            }
            return 1;
        }

        public int Process_Server(Connection conn)
        {
            if(conn._state.Has_Read())
            {
                Console.WriteLine("Process Server");
                string message = conn._state.Dequeue_Read();

                if(message.StartsWith("READ"))
                {
                    message = message.Substring(4);

                    if(File.Exists(message))
                    {
                        TextReader text_reader = new StreamReader(message);
                        string line = "";
                        string file_contents = "";

                        while((line = text_reader.ReadLine()) != null)
                        {
                            file_contents = file_contents + line;
                        }
                        text_reader.Close();
                        message = file_contents;
                    }                    
                }

                conn._state.Enqueue_Write(message);

                List<string> string_list= main_server.Process_String(message);
                if(string_list != null)
                {
                    foreach(string s in string_list)
                    {
                        conn._state.Enqueue_Write(s);
                    }
                }
            }
            else{
                Thread.Sleep(1);
            }
            return 1;
        }

        //Connect to specifc address
        public Connection Connect(string _hostname, int _port, Func<Connection, int> _processing_function)
        {
            #region connect to the specified address
            System.Net.IPHostEntry ipHostInfo = Dns.GetHostEntry(_hostname);
            IPAddress _IPAddress = ipHostInfo.AddressList[0];
            IPEndPoint local_end_point = new IPEndPoint(_IPAddress, _port);
            Socket sender = new Socket(_IPAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            sender.Connect(local_end_point);
            #endregion

            Connection connection = new Connection(sender, _processing_function);
            client_connections.Add(connection);

            Thread client_thread = new Thread(new ThreadStart(connection.Start));
            client_thread.Start();

            return connection;
        }

        public void Write_Servers(string message)
        {
            foreach(Connection server in server_connections)
            {
                Console.WriteLine("Writing to FileServer");
                server._state.Enqueue_Write(message);
            }
        }

        


    }
}
