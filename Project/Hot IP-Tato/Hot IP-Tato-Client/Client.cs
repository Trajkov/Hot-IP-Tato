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
    class Client : IDisposable
    {
        private const int BufferSize = 1024;
        private const int Port = 13000;

        public event EventHandler<HelloPacket> RaiseClientDisconnectedEvent;
        public event EventHandler<HelloPacket> RaiseClientWinEvent;

        public HelloPacket ClientInfo;
        public HelloPacket ServerInfo;
        private TcpListener TCPListener = null;

        private Thread ServerDiscoverThread;
        private Thread TCPGameThread;

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
            RaiseClientDisconnectedEvent += HandleClientDisconnectedEvent;
            RaiseClientWinEvent += HandleClientWinEvent;
        }
        private void DiscoverServer()
        {
            // Thread BroadcastResponseListener = new Thread(() => StartTCPListener(thisHost));
            // BroadcastResponseListener.Start();
            // Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            // Console.WriteLine();

            UdpClient Client = new UdpClient();
            Message request = new Message();
            request = Utilities.Serialize(ClientInfo);

            IPEndPoint broadcastEP = new IPEndPoint(IPAddress.Any, 0);

            Client.EnableBroadcast = true;
            Client.Send(request.data, request.data.Length, new IPEndPoint(IPAddress.Broadcast, 13000));

            Console.WriteLine("Message sent to the broadcast address");
            Message responseMessage = new Message(256);
            // s.Receive(responseMessage.data);

            responseMessage.data = Client.Receive(ref broadcastEP);
            HelloPacket responseData = (HelloPacket)Utilities.Deserialize(responseMessage);
            Console.WriteLine($"Received respone from {responseData.ToString()}");

            Client.Close();
        }
        private void StartDiscoveringServers()
        {
            ServerDiscoverThread = new Thread(DiscoverServer);
            ServerDiscoverThread.Start();
        }
        private void StopDiscoveringServers()
        {
            ServerDiscoverThread.Abort();
            ServerDiscoverThread = null;
        }

        private void UDPCommandListener()
        {
            UdpClient CommandListener = new UdpClient(ClientInfo.EndPoint());
            Message responseData = Utilities.Serialize("ERR:Response Not Processed");
            bool active = true;
            try
            {

                while (active)
                {
                    Console.WriteLine("UDP Waiting for broadcast");

                    IPEndPoint serverEP = ServerInfo.EndPoint();
                    Message message = new Message();
                    message.data = CommandListener.Receive(ref serverEP);
                    Console.WriteLine(ServerInfo);

                    object receivedData = Utilities.Deserialize(message);
                    if (receivedData is string)
                    {
                        switch ((string)receivedData)
                        {
                            case "CMD:Win":
                                this.OnRaiseClientWinEvent();
                                break;
                            case "CMD:Disconnect":
                                Console.WriteLine("Received Command: Disconnect");
                                active = false;
                                this.OnRaiseClientDisconnectedEvent(this.ClientInfo);
                                receivedData = "CMD:Disconnect";
                                responseData = Utilities.Serialize("RESPONSE:Disconnect");
                                break;
                        }
                    }

                    Console.WriteLine($"Received broadcast from {serverEP} :");
                    Console.WriteLine($" RSVP address {receivedData.ToString()}");


                    // Send the server verification that the response was received.
                    Console.WriteLine("UDP Client sending response");
                    CommandListener.Send(responseData.data, responseData.data.Length, serverEP);

                }
            }
            catch (SocketException e)
            {
                Console.WriteLine(e);
            }
            catch (ThreadAbortException e)
            {
                Console.WriteLine("Thread Aborted: {0}", e);
                Console.WriteLine("Disposing of UDP server");
            }
            finally
            {
                CommandListener.Close();
            }
        }



        public void Start()
        {
            Console.WriteLine("Starting the UDP Broadcast at {0}", ClientInfo.ToString());
            Thread UDPBroadcast = new Thread(() => JoinServer());
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
        private void JoinServer()
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

            object responseData = Utilities.Deserialize(responseMessage);
            if (responseData is HelloPacket)
            {
                ServerInfo = responseData as HelloPacket;
                Thread UDPCommandListenerThread = new Thread(UDPCommandListener);
                UDPCommandListenerThread.Start();
            }
            Console.WriteLine($"Received respone from {responseData.ToString()}");

            Client.Close();
        }
        private void SendUDPToServer(string command)
        {
            UdpClient client = new UdpClient(ClientInfo.EndPoint());
            Message request = new Message();
            request = Utilities.Serialize(command);

            IPEndPoint clientEP = new IPEndPoint(IPAddress.Any, 0);

            client.Send(request.data, request.data.Length, ServerInfo.EndPoint());

            Console.WriteLine($"Command {command} sent to the server: {ServerInfo.ToString()}");
            Message responseMessage = new Message(256);
            // s.Receive(responseMessage.data);

            responseMessage.data = client.Receive(ref clientEP);
            object responseData = Utilities.Deserialize(responseMessage);
            Console.WriteLine($"Received respone from {responseData.ToString()}");

            client.Close();
        }
        private void StartListener()
        {
            try
            {
                Console.WriteLine("IP is {0}", ClientInfo.address);
                IPAddress localip = IPAddress.Parse((ClientInfo.address));
                Console.WriteLine("Starting a tcplistener at {0} using port {1}", localip, ClientInfo.port);
                TCPListener = new TcpListener(localip, ClientInfo.port);
                TCPListener.Start();
                Console.WriteLine("Listener has started.");

                // Create Buffer
                byte[] buffer = new byte[1024];

                while (true)
                {
                    // Add an extra space to help distinguish between each server transaction.
                    Console.WriteLine();
                    Console.WriteLine("Client wating for a connection... ");

                    // Accept a pending connection
                    TcpClient client = TCPListener.AcceptTcpClient();
                    Console.WriteLine("Connected!");

                    // Instantiate the stream
                    NetworkStream stream = client.GetStream();

                    // While there is data to be read
                    while ((stream.Read(buffer, 0, buffer.Length)) != 0)
                    {
                        try
                        {
                            // Instantiate a Message object to hold the incoming object
                            Message incomingMessage = new Message();
                            // Assign the data which has been read to incomingMessage
                            incomingMessage.data = buffer;
                            // Deserialize the inbound data into an object which can be processed 

                            object receivedObject = Utilities.Deserialize(incomingMessage);
                            // Verify that the server received the correct data
                            Console.WriteLine("Client Received: " + receivedObject.ToString());

                            Console.WriteLine("Processing Request...");

                            // Instantiate a Message to hold the response message
                            Message responseMessage = new Message();
                            object objectResponse = ProcessRequest(receivedObject);

                            responseMessage = Utilities.Serialize(objectResponse);

                            // Send back a response.
                            stream.Write(responseMessage.data, 0, responseMessage.data.Length);
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
                TCPListener.Stop();
            }
        }
        private object ProcessRequest(object receivedObject)
        {
            object objectResponse = "ERR:ProcessFailure";
            if (receivedObject is IP_Tato)
            {
                // Process the IP_Tato 
                IP_Tato tater = receivedObject as IP_Tato;
                tater.Passes--;


                // Check if tater has exploded
                if (tater.Passes == 0)
                {
                    tater.Explode();
                    objectResponse = tater;
                    // OnRaiseClientDisconnectedEvent(ClientInfo);
                }


                // Set up a EventWaitHandle to block code execution until the popup is closing.
                EventWaitHandle ewh = new EventWaitHandle(false, EventResetMode.ManualReset);
                bool taterIsPassed = false;

                // Update the GUI with the results of the tater
                Application app = Application.Current;
                app.Dispatcher.Invoke((Action)delegate
                {
                    // Create a new popup
                    Game_Popup game_Popup = new Game_Popup(tater);

                    // Show the popup
                    game_Popup.Show();



                    // Set up an event handler to capture the results of the popup
                    game_Popup.Closing += (object send, System.ComponentModel.CancelEventArgs eargs) =>
                    {
                        taterIsPassed = game_Popup.Passing;
                        ewh.Set();
                    };
                });
                if (tater.Exploded)
                {
                    ewh.Set();
                    for (int x = 0; x < 10; x++)
                    {
                        app.Dispatcher.Invoke((Action)delegate
                        {
                            // Create a new popup
                            Game_Popup game_Popup = new Game_Popup(tater);

                            // Show the popup
                            game_Popup.Show();
                        });
                    }
                    // OnRaiseClientDisconnectedEvent(ClientInfo);
                }
                // Block until the popup closing event fires.
                ewh.WaitOne();
                if (taterIsPassed || tater.Exploded)
                {
                    // Set the previous client to the current client.
                    tater.LastClient = tater.TargetClient;
                    objectResponse = tater;
                }
                else
                {
                    OnRaiseClientDisconnectedEvent(ClientInfo);
                    objectResponse = "RESPONSE:Disconnect";
                }

            }

            return objectResponse;
        }

        #region Events Support
        protected virtual void OnRaiseClientDisconnectedEvent(HelloPacket client)
        {
            Console.WriteLine("OnClientDisconnected Called");
            EventHandler<HelloPacket> handler = RaiseClientDisconnectedEvent;
            RaiseClientDisconnectedEvent?.Invoke(this, client);
        }
        private void HandleClientDisconnectedEvent(object sender, HelloPacket client)
        {
            Console.WriteLine($"This Client: {client.ToString()} has disconnected.");
            
            // Send a message to the server informing them of the disconnection
            // **** This will use the UDP broadcast
            // **** the UDP server is still running on the server so why not send the info to it.
        }

        protected virtual void OnRaiseClientWinEvent()
        {
            Console.WriteLine("RaiseClientWinEvent Fired");
            EventHandler<HelloPacket> handler = RaiseClientWinEvent;
            RaiseClientWinEvent?.Invoke(this, ClientInfo);
        }

        private void HandleClientWinEvent(object sender, HelloPacket client)
        {
            Application.Current.Dispatcher.Invoke((Action)delegate
            {
                // Create a new popup
                WinWindow win = new WinWindow();

                // Show the popup
                win.Show();


            });
        }

        #endregion
        
        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).

                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~Client() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion

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
        //private void StartGameListener(HelloPacket ClientInfo)
        //{
        //    TcpListener listener = null;
        //    try
        //    {
        //        Console.WriteLine("IP is {0}", ClientInfo.address);
        //        IPAddress localip = IPAddress.Parse((ClientInfo.address));
        //        Console.WriteLine("Starting a tcplistener at {0} using port {1}", localip, ClientInfo.port);
        //        listener = new TcpListener(localip, ClientInfo.port);
        //        listener.Start();
        //        Console.WriteLine("Listener has started.");

        //        // Create Buffer
        //        byte[] buffer = new byte[BufferSize];

        //        while (true)
        //        {
        //            // Add an extra space to help distinguish between each server transaction.
        //            Console.WriteLine();
        //            Console.WriteLine("Client wating for a connection... ");

        //            // Accept a pending connection
        //            TcpClient client = listener.AcceptTcpClient();
        //            Console.WriteLine("Connected!");

        //            // Instantiate the stream
        //            NetworkStream stream = client.GetStream();

        //            // While there is data to be read
        //            // TODO: Implement the ability to read more data with a smaller buffer.
        //            while ((stream.Read(buffer, 0, buffer.Length)) != 0)
        //            {
        //                try
        //                {
        //                    // Prep the objects needed for the transaction
        //                    // Instantiate a Message object to hold the incoming object
        //                    Message incomingMessage = new Message();
        //                    // Instantiate a Message object to hold the response
        //                    Message responseMessage = new Message();
        //                    object objectResponse;


        //                    // Assign the data which has been read to incomingMessage
        //                    incomingMessage.data = buffer;
        //                    // Deserialize the inbound data into an object which can be processed 
        //                    //   By the function or workerthread.
        //                    object receivedObject = Utilities.Deserialize(incomingMessage) as object;

        //                    // Verify what type of object was received.
        //                    Console.WriteLine("Client Received: " + receivedObject.ToString());
        //                    Console.WriteLine("Processing Request...");
        //                    if (receivedObject is IP_Tato)
        //                    {
        //                        objectResponse = (IP_Tato)ProcessRequest(receivedObject);
        //                    }
        //                    else
        //                    {
        //                        responseMessage.successfulTransmission = false;
        //                        objectResponse = "Received a non-IP_Tato object";
        //                    }
        //                    // Verify that the server received the correct data


        //                    // Instantiate a Message to hold the response message

        //                    responseMessage = Utilities.Serialize(objectResponse);

        //                    // Send back a response.
        //                    // This should also include provisions for a voluntary host disconnect
        //                    stream.Write(responseMessage.data, 0, responseMessage.data.Length);
        //                    // Verify that the data sent against the client receipt.
        //                    Console.WriteLine("Client Sent {0}", objectResponse.ToString());
        //                }
        //                catch (Exception ErrorProcessRequest)
        //                {
        //                    Console.WriteLine("The request failed to be processed. Error details: " + ErrorProcessRequest);
        //                }
        //            }
        //            Console.WriteLine("---Listener Transaction Closed---");
        //            stream.Close();
        //            client.Close();
        //        }
        //    }
        //    catch (SocketException e)
        //    {
        //        Console.WriteLine("Server SocketException: {0}", e);
        //    }
        //    finally
        //    {
        //        listener.Stop();
        //    }
        //}
    }

}
