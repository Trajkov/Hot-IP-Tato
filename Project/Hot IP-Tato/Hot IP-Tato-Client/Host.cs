using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel;
using Common;

namespace Hot_IP_Tato_Client
{
    public class Host : IDisposable
    {
        // Delegates
        // public delegate void ClientJoinedEventHandler(object sender, HelloPacket clientJoined);

        // Events
        public event EventHandler<HelloPacket> RaiseClientJoinedEvent;
        public event EventHandler<HelloPacket> RaiseClientDisconnectedEvent;

        // private int gameID
        // Consts
        private const int BufferSize = 1024;
        private const int Port = 13000;

        //Generated at Constructions
        private HelloPacket HostInformation;

        private Thread UDPListener;
        UdpClient server = new UdpClient(Port);

        public List<HelloPacket> HostList = new List<HelloPacket>();
        public List<IPEndPoint> ClientList = new List<IPEndPoint>();

        private List<Thread> IP_TatoThreads = new List<Thread>();

        // State Variables
        private bool ClosedGame = false;
        private bool GamePaused = false;

        private int IP_TatoID = 1;

        /// <summary>
        /// This creates a Host which will host the game.
        /// The UDP Listener is the backbone of a host and is requisite for the entire process.
        /// </summary>
        public Host(string hostname = "Host", bool localIP = false)
        {
            // Set up the IP Address to send out with the UDP Packet
            if (!localIP)
            {
                HostInformation = new HelloPacket(hostname, Networking.getExternalIPE(), Port);
            } else if (localIP)
            {
                HostInformation = new HelloPacket(hostname, Networking.getLocalIPE(), Port);
            }
            // TODO: Figure out whether the Host should start listening when created or if it should be started after creation by a public method.
            RaiseClientJoinedEvent += HandleClientJoinedEvent;
            RaiseClientDisconnectedEvent += HandleClientDisconnectedEvent;
        }

        /// <summary>
        /// This will be the way that clients are added to the current group.
        /// Games can be hosted as open or closed. (closed will keep new players from joining)
        /// Closed will be the default.
        /// 
        /// Returns a reference to the Listener
        /// </summary>
        private void ClientDiscover()
        {
            // UDP listener logic 
            Console.WriteLine($"Creating UDP server listener {HostInformation.ToString()}");
            
            Message responseData = Utilities.Serialize(HostInformation);

            try
            {
                // Stop listening when the game starts (in closed mode).
                while (!ClosedGame)
                {
                    Console.WriteLine("UDP Waiting for broadcast");

                    IPEndPoint clientEP = new IPEndPoint(IPAddress.Any, 0);
                    Message message = new Message();
                    message.data = server.Receive(ref clientEP);
                    if (!ClosedGame)
                    {
                        object receivedData = Utilities.Deserialize(message);
                        if (receivedData is HelloPacket)
                        {
                            // Add new client to Lists
                            HelloPacket newClient = receivedData as HelloPacket;
                            this.OnRaiseClientJoinedEvent(newClient);
                            
                        } if (receivedData is string)
                        {
                            switch(receivedData as string)
                            {
                                case "RESPONSE:Disconnect":
                                    OnRaiseClientDisconnectedEvent(HostList.Find(HelloPacket => HelloPacket.EndPoint() == clientEP));
                                    break;
                            }
                        }

                        Console.WriteLine($"Received broadcast from {clientEP} :");
                        Console.WriteLine($" RSVP address {receivedData.ToString()}");



                        // Add the client EndPoint to the ClientList
                        Console.WriteLine($"{clientEP} added to the clientlist");
                        ClientList.Add(clientEP);


                        // Send the server data to the client
                        Console.WriteLine("UDP Server sending response");
                        server.Send(responseData.data, responseData.data.Length, clientEP);
                    }
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
                server.Close();
            }
        }
        public void StartDiscoverClient()
        {
            // Call the UDPListener to listen for new Clients
            UDPListener = new Thread(ClientDiscover);
            UDPListener.IsBackground = true;
            UDPListener.Start();
        }
        public void StopDiscoverClient()
        {
            Console.WriteLine("StopDiscoverClient Called");
            server.Dispose();
            UDPListener.Abort();
            ClosedGame = true;
        }
        private void SendCommandToClient(HelloPacket targetClient, string command)
        {
            UdpClient client = new UdpClient();
            Message request = new Message();
            request = Utilities.Serialize(command);

            IPEndPoint serverEP = new IPEndPoint(IPAddress.Any, 0);
            
            client.Send(request.data, request.data.Length, targetClient.EndPoint());

            Console.WriteLine($"Command {command} sent to the client: {targetClient.ToString()}");
            Message responseMessage = new Message(256);
            // s.Receive(responseMessage.data);

            responseMessage.data = client.Receive(ref serverEP);
            object responseData = Utilities.Deserialize(responseMessage);
            Console.WriteLine($"Received respone from {responseData.ToString()}");

            client.Close();
        }
        /// <summary>
        /// This will start a new game.
        /// Will return when the game is over, 
        ///     or it will return an object which can manage the game.
        /// 
        /// </summary>
        public void StartGame(int NumberOfPotatoes)
        {
            ClosedGame = true;
            // Prep the potatoes
            // Every potato should get a random limit of passes between 5 and 10
            Thread[] IP_TatoThreads = new Thread[NumberOfPotatoes];
            for (int x = 0; x < IP_TatoThreads.Length; x++)
            {
                Console.WriteLine("Creating IP_Tato handler {0}", x + 1);
                Add_IP_Tato();
            }
        }
        public void Add_IP_Tato()
        {
            int numberOfClients = HostList.ToArray().Length;
            IP_Tato tater = new IP_Tato("Tater #" + IP_TatoID++ , Utilities.RandomInteger(numberOfClients, numberOfClients * 2));
            Thread newTaterThread = new Thread(() => IP_TatoHandler(tater));
            newTaterThread.IsBackground = true;
            newTaterThread.Start();
            IP_TatoThreads.Add(newTaterThread);
        }
        /// <summary>
        /// Cleans up the object and ensures that the clients are properly handled.
        /// </summary>
        public void StopGame()
        {
            // Signal everything that the game is stopping.
        }
        /// <summary>
        /// This is meant to Manage the IP_Tato either as an object that can be returned
        ///   Or just a function to take care of the logic of the potato.
        /// </summary>
        /// <param name="tater"></param>
        private void IP_TatoHandler(IP_Tato tater)
        {
            //TODO: Finish translating what is in PassDataToClient into this function.
            tater.LastClient = HostInformation;
            while (tater.Passes > 0)
            {
                // Select the target client
                tater.TargetClient = RouteIP_Tato(tater);

                Console.WriteLine("The new targetClient is: {0}", tater.TargetClient);
                // Set up the target address
                IPEndPoint targetClient = tater.TargetClient.EndPoint();
                // Pass data to be sent to client
                // This function returns the response data from the client.
                object responseObject = PassDataToClient(tater, targetClient);

                if (responseObject is IP_Tato)
                {
                    // Do stuff with the tater (probably reroute)
                    tater = responseObject as IP_Tato;
                } else if (responseObject is string)
                {
                    switch((string)responseObject)
                    {
                        case "CMD:Disconnect":
                            this.KickPlayer(tater.TargetClient);
                            break;
                        case "RESPONSE:Disconnect":
                            OnRaiseClientDisconnectedEvent(tater.TargetClient);
                            continue;
                    }
                    // Right now the only string that will be returned is that the client refused the tater.
                    
                }

                // Verify the current state is correct
                Console.WriteLine("Server received: {0}", tater.ToString());
            }
        }
        private HelloPacket RouteIP_Tato(IP_Tato tater)
        {
            // TODO add different routing techniques
            int rolls = 0;
            HelloPacket[] hostArray = HostList.ToArray();
            do {
                int index = Utilities.RandomInteger(0, hostArray.Length);
                tater.TargetClient = hostArray[index];
                rolls++;
            }
            while (tater.TargetClient.address == tater.LastClient.address);

            Console.WriteLine("Previous Client: {0}", tater.LastClient.address);
            Console.WriteLine("New targetClient {0} chosen after {1} roll(s)", tater.TargetClient, rolls);
            return tater.TargetClient;
        }
        private object PassDataToClient(object ObjectToBeSent, IPEndPoint target)
        {
            TcpClient client = new TcpClient();

            // Initialize the retry counter (which may be overkill)
            // Handle unseen errors
            try
            {
                // Connect to the target client
                client.Connect(target);

                // Serialize the object which will be sent
                Message outboundMessage = Utilities.Serialize(ObjectToBeSent);

                // Initialize the stream
                NetworkStream stream = client.GetStream();

                // Send the object through the stream
                Console.WriteLine("Server sending {0} bytes.", outboundMessage.data.Length);
                stream.Write(outboundMessage.data, 0, outboundMessage.data.Length);

                // -- Part 2: Receive the response. --

                // Create a Message object to receive the response
                Message inboundMessage = new Message(BufferSize);

                // This reads the server's response from the network
                // It assigns the response byte [] to the buffer.
                int bytes = stream.Read(inboundMessage.data, 0, inboundMessage.data.Length);

                if (inboundMessage.data.Equals(0))
                {
                    Console.WriteLine("Received an empty message");
                }

                // Deserialize the data which was received and create a tater
                object responseData = Utilities.Deserialize(inboundMessage) as object;

                // Close out the connection
                stream.Close();
                Console.WriteLine("---Server Transaction Closed---");
                return responseData;
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine("Server ArgumentNullException: {0}", e);
                throw;
            }
            catch (SocketException e)
            {
                Console.WriteLine("Server SocketException: {0}", e);
                throw;
            }
            finally
            {
                // Ensure that the client connection is properly closed.
                client.Close();
                
            }
        }

        public void KickPlayer(HelloPacket player)
        {
            Console.WriteLine("Sending disconnection command to client");
            SendCommandToClient(player, "CMD:Disconnect");
            this.OnRaiseClientDisconnectedEvent(player);
            // The tcplistener on their end will shut down and send the player back to the join lobby.
            // That will trigger the Client Disconnected method on that end which will trigger the event
            // On the server side, so really this should just do that.
            // If it doesn't receive a response it will remove it from the server.
        }

        protected virtual void OnRaiseClientJoinedEvent(HelloPacket client)
        {
            Console.WriteLine("OnClientJoined Called");
            EventHandler<HelloPacket> handler = RaiseClientJoinedEvent;
            RaiseClientJoinedEvent?.Invoke(this, client);
        }
        private void HandleClientJoinedEvent(object sender, HelloPacket client)
        {
            Console.WriteLine($"Adding {client.ToString()} to HostList.");
            HostList.Add(client);
        }
        protected virtual void OnRaiseClientDisconnectedEvent(HelloPacket client)
        {            
            Console.WriteLine("OnClientDisconnected Called");
            EventHandler<HelloPacket> handler = RaiseClientDisconnectedEvent;
            RaiseClientDisconnectedEvent?.Invoke(this, client);
        }
        private void HandleClientDisconnectedEvent(object sender, HelloPacket client)
        {
            Console.WriteLine($"Client {client.ToString()} has disconnected.");
            Console.WriteLine($"Removing {client.ToString()} from HostList.");
            HostList.Remove(client);
            if (HostList.ToArray().Length < 2)
            {
                GamePaused = true;
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                    StopDiscoverClient();
                    StopGame();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~Host() {
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
    }
}
