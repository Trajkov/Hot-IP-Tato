using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Common;

namespace Hot_IP_Tato_Client
{
    class Client
    {
        private const int BufferSize = 1024;
        private const int Port = 13000;

        public HelloPacket ClientInfo;

        public List<HelloPacket> GameServerList = new List<HelloPacket>();
        
        public Client(string hostname = "Client", bool localIP = false)
        {
            if (!localIP)
            {
                ClientInfo = new HelloPacket(hostname, Networking.getExternalIPE(), Port);
            }
            else if (localIP)
            {
                ClientInfo = new HelloPacket(hostname, Networking.getLocalIPE(), Port);
            }
        }
        private static void StartServerDiscover(HelloPacket thisHost)
        {
            // Thread BroadcastResponseListener = new Thread(() => StartTCPListener(thisHost));
            // BroadcastResponseListener.Start();
            // Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            // Console.WriteLine();

            UdpClient Client = new UdpClient();
            Message request = new Message();
            request = Utilities.Serialize(thisHost);

            IPEndPoint serverEP = new IPEndPoint(IPAddress.Any, 0);

            Client.EnableBroadcast = true;
            Client.Send(request.data, request.data.Length, new IPEndPoint(IPAddress.Broadcast, 13000));

            Console.WriteLine("Message sent to the broadcast address");
            Message responseMessage = new Message(256);
            // s.Receive(responseMessage.data);

            responseMessage.data = Client.Receive(ref serverEP);
            HelloPacket responseData = (HelloPacket)Utilities.Deserialize(responseMessage);
            Console.WriteLine($"Received respone from {responseData.ToString()}");

            Client.Close();
        }

        private void StartGameListener(HelloPacket ClientInfo)
        {
            TcpListener listener = null;
            try
            {
                Console.WriteLine("IP is {0}", ClientInfo.address);
                IPAddress localip = IPAddress.Parse((ClientInfo.address));
                Console.WriteLine("Starting a tcplistener at {0} using port {1}", localip, ClientInfo.port);
                listener = new TcpListener(localip, ClientInfo.port);
                listener.Start();
                Console.WriteLine("Listener has started.");

                // Create Buffer
                byte[] buffer = new byte[BufferSize];

                while (true)
                {
                    // Add an extra space to help distinguish between each server transaction.
                    Console.WriteLine();
                    Console.WriteLine("Client wating for a connection... ");

                    // Accept a pending connection
                    TcpClient client = listener.AcceptTcpClient();
                    Console.WriteLine("Connected!");

                    // Instantiate the stream
                    NetworkStream stream = client.GetStream();

                    // While there is data to be read
                    // TODO: Implement the ability to read more data with a smaller buffer.
                    while ((stream.Read(buffer, 0, buffer.Length)) != 0)
                    {
                        try
                        {
                            // Prep the objects needed for the transaction
                            // Instantiate a Message object to hold the incoming object
                            Message incomingMessage = new Message();
                            // Instantiate a Message object to hold the response
                            Message responseMessage = new Message();
                            object objectResponse;

                            
                            // Assign the data which has been read to incomingMessage
                            incomingMessage.data = buffer;
                            // Deserialize the inbound data into an object which can be processed 
                            //   By the function or workerthread.
                            object receivedObject = Utilities.Deserialize(incomingMessage) as object;

                            // Verify what type of object was received.
                            Console.WriteLine("Client Received: " + receivedObject.ToString());
                            Console.WriteLine("Processing Request...");
                            if (receivedObject is IP_Tato)
                            {
                                objectResponse = (IP_Tato)ProcessPotato(receivedObject);
                            }
                            else
                            {
                                responseMessage.successfulTransmission = false;
                                objectResponse = "Received a non-IP_Tato object";
                            }
                            // Verify that the server received the correct data
                            
                            
                            // Instantiate a Message to hold the response message
                           
                            responseMessage = Utilities.Serialize(objectResponse);

                            // Send back a response.
                            // This should also include provisions for a voluntary host disconnect
                            stream.Write(responseMessage.data, 0, responseMessage.data.Length);
                            // Verify that the data sent against the client receipt.
                            Console.WriteLine("Client Sent {0}", objectResponse.ToString());
                        }
                        catch (Exception ErrorProcessRequest)
                        {
                            Console.WriteLine("The request failed to be processed. Error details: " + ErrorProcessRequest);
                        }
                    }
                    Console.WriteLine("---Listener Transaction Closed---");
                    stream.Close();
                    client.Close();
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("Server SocketException: {0}", e);
            }
            finally
            {
                listener.Stop();
            }
        }

        public void Start()
        {
            Console.WriteLine("Starting the UDP Broadcast at {0}", ClientInfo.ToString());
            Thread UDPBroadcast = new Thread(() => StartBroadcast());
            UDPBroadcast.Start();

            Client client = new Client();
            Thread ListenerThread = new Thread(() => StartListener());
            ListenerThread.IsBackground = true;
            ListenerThread.Start();
            //Console.WriteLine("IP is {0}", ClientInfo.address);
            //IPAddress localip = IPAddress.Parse((ClientInfo.address));
            //Console.WriteLine("Starting a tcplistener at {0} using port {1}", localip, ClientInfo.port);
            //TcpListener listener = new TcpListener(localip, ClientInfo.port);
            //listener.Start();
            //Console.WriteLine("Listener has started.");
            //while (true)
            //{
            //    if (listener.Pending())
            //    {
            //        Thread listenthread = new Thread(() => { 
            //        DoBeginAcceptTcpClient(listener);
            //        });
            //        listenthread.Start();
            //    }
            //}
        }
        private void StartBroadcast()
        {
            // Thread BroadcastResponseListener = new Thread(() => StartTCPListener(thisHost));
            // BroadcastResponseListener.Start();
            // Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            // Console.WriteLine();

            UdpClient Client = new UdpClient();
            Message request = new Message();
            request = Utilities.Serialize(ClientInfo);

            IPEndPoint serverEP = new IPEndPoint(IPAddress.Any, 0);

            Client.EnableBroadcast = true;
            Client.Send(request.data, request.data.Length, new IPEndPoint(IPAddress.Broadcast, 13000));

            Console.WriteLine("Message sent to the broadcast address");
            Message responseMessage = new Message(256);
            // s.Receive(responseMessage.data);

            responseMessage.data = Client.Receive(ref serverEP);
            HelloPacket responseData = (HelloPacket)Utilities.Deserialize(responseMessage);
            Console.WriteLine($"Received respone from {responseData.ToString()}");

            Client.Close();
        }
        private void StartListener()
        {
            TcpListener listener = null;
            try
            {
                Console.WriteLine("IP is {0}", ClientInfo.address);
                IPAddress localip = IPAddress.Parse((ClientInfo.address));
                Console.WriteLine("Starting a tcplistener at {0} using port {1}", localip, ClientInfo.port);
                listener = new TcpListener(localip, ClientInfo.port);
                listener.Start();
                Console.WriteLine("Listener has started.");

                // Create Buffer
                byte[] buffer = new byte[1024];

                while (true)
                {
                    // Add an extra space to help distinguish between each server transaction.
                    Console.WriteLine();
                    Console.WriteLine("Client wating for a connection... ");

                    // Accept a pending connection
                    TcpClient client = listener.AcceptTcpClient();
                    Console.WriteLine("Connected!");

                    // Instantiate the stream
                    NetworkStream stream = client.GetStream();

                    // While there is data to be read
                    // TODO: Implement the ability to read more data with a smaller buffer.
                    while ((stream.Read(buffer, 0, buffer.Length)) != 0)
                    {
                        try
                        {
                            // Instantiate a Message object to hold the incoming object
                            Message incomingMessage = new Message();
                            // Assign the data which has been read to incomingMessage
                            incomingMessage.data = buffer;
                            // Deserialize the inbound data into an object which can be processed 
                            //   By the function or workerthread.
                            IP_Tato receivedTato = Utilities.Deserialize(incomingMessage) as IP_Tato;
                            // Verify that the server received the correct data
                            Console.WriteLine("Client Received: " + receivedTato.ToString());

                            Console.WriteLine("Processing Request...");

                            // TODO: Create a worker thread to work with the potato.
                            //      This will be especially necessary when UI gets involved.
                            // For now it is just going to call a function
                            IP_Tato objectResponse = (IP_Tato)ProcessPotato(receivedTato);


                            // Instantiate a Message to hold the response message
                            Message responseMessage = new Message();
                            responseMessage = Utilities.Serialize(objectResponse);

                            // Send back a response.
                            // This should also include provisions for a voluntary host disconnect
                            stream.Write(responseMessage.data, 0, responseMessage.data.Length);
                            // Verify that the data sent against the client receipt.
                            Console.WriteLine("Client Sent {0}", objectResponse.ToString());
                        }
                        catch (Exception ErrorProcessRequest)
                        {
                            Console.WriteLine("The request failed to be processed. Error details: " + ErrorProcessRequest);
                        }
                    }
                    Console.WriteLine("---Listener Transaction Closed---");
                    stream.Close();
                    client.Close();
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("Server SocketException: {0}", e);
            }
            finally
            {
                listener.Stop();
            }
        }
        private static object ProcessPotato(object obj)
        {
            // Some of these commands should be placed in the potato object
            //  Because they then can be protected 

            // Create IP_Tato
            IP_Tato tater = obj as IP_Tato;


            

            // Pseudocode
            // Check Flags of tater
            // Do stuff according to the flags on tater
            // Flags are stored as bools and should be assigned by
            // * names rather than position.
            // Flag Precedence is a thing a potato should explode before other things happen.

            // Print last player that tater was passed from
            // "$player passed a potato to you"


            // Check if number of passes is done.
            // Greater than used to catch too many passes.
            if (tater.Passes >= tater.TotalPasses)
            {
                tater.Explode();
            }
            // This will update the GUI with the results of the tater
            // There may be a better process in the test
            Application app = Application.Current;
            app.Dispatcher.Invoke((Action)delegate {
                // Try to update GUI from this thread.
                Game_Popup game_Popup = new Game_Popup(tater);

                // The ShowDialog is the perfect function for this.
                // It blocks until the window is closed which is all I needed it to do.
                game_Popup.ShowDialog();
            });
            
            // Set the previous client to the current client.
            tater.LastClient = tater.TargetClient;
            // Increment current passes
            // This is done at the end in case of an involuntary host disconnect
            tater.Passes++;

            return tater as object;
        }

        //// Accept one client connection asynchronously.
        //private void DoBeginAcceptTcpClient(TcpListener
        //    listener)
        //{

        //    // Start to listen for connections from a client.
        //    Console.WriteLine("Waiting for a connection...");

        //    // Accept the connection. 
        //    // BeginAcceptSocket() creates the accepted socket.
        //    listener.BeginAcceptTcpClient(
        //        new AsyncCallback(DoAcceptTcpClientCallback),
        //        listener);
        //}

        //// Process the client connection.
        //private void DoAcceptTcpClientCallback(IAsyncResult ar)
        //{
        //    // Get the listener that handles the client request.
        //    TcpListener listener = (TcpListener)ar.AsyncState;
        //    byte[] buffer = new byte[1024];

        //    // End the operation and display the received data on 
        //    // the console.
        //    TcpClient client = listener.EndAcceptTcpClient(ar);

        //    NetworkStream stream = client.GetStream();

        //    // While there is data to be read
        //    // TODO: Implement the ability to read more data with a smaller buffer.
        //    while ((stream.Read(buffer, 0, buffer.Length)) != 0)
        //    {
        //        try
        //        {
        //            // Instantiate a Message object to hold the incoming object
        //            Message incomingMessage = new Message();
        //            // Assign the data which has been read to incomingMessage
        //            incomingMessage.data = buffer;
        //            // Deserialize the inbound data into an object which can be processed 
        //            //   By the function or workerthread.
        //            IP_Tato receivedTato = Utilities.Deserialize(incomingMessage) as IP_Tato;
        //            // Verify that the server received the correct data
        //            Console.WriteLine("Client Received: " + receivedTato.ToString());

        //            Console.WriteLine("Processing Request...");

        //            // TODO: Create a worker thread to work with the potato.
        //            //      This will be especially necessary when UI gets involved.
        //            // For now it is just going to call a function
        //            IP_Tato objectResponse = (IP_Tato)ProcessPotato(receivedTato);


        //            // Instantiate a Message to hold the response message
        //            Message responseMessage = new Message();
        //            responseMessage = Utilities.Serialize(objectResponse);

        //            // Send back a response.
        //            // This should also include provisions for a voluntary host disconnect
        //            stream.Write(responseMessage.data, 0, responseMessage.data.Length);
        //            // Verify that the data sent against the client receipt.
        //            Console.WriteLine("Client Sent {0}", objectResponse.ToString());
        //        }
        //        catch (Exception ErrorProcessRequest)
        //        {
        //            Console.WriteLine("The request failed to be processed. Error details: " + ErrorProcessRequest);
        //        }
        //    }

        //    // Process the connection here. (Add the client to a
        //    // server table, read data, etc.)
        //    Console.WriteLine("Client connected completed");
        //}

    }
    
}
