using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using ConsoleSandbox;

// TODO: finish putting together the logic for the Headless Model.
// TODO: Perhaps break some part of this class into an object (to make code more elegant and better self-referencing).


// The broadcast will need to send the following data: 
//   Where to RSVP (ip address and the port which IP_Tato is listening on). MANDATORY
//   Possibly some other stuff about the server, probably name and stuff. (possibly so users can pick which game they want to join.)

namespace ConsoleSandbox
{
    
    class UDPHello
    {
        private const int listenPort = 11000;
        // public int IPHostnumber = 1;
        private const int maxClients = 2;
        static List<string> hostList = new List<string>();
        public static void Test()
        {
            // Start up the listener threads
            HelloPacket client = new HelloPacket("server", "127.0.0.2", listenPort);
            Thread listener1 = new Thread(() => StartListener(client));
            listener1.Start();

            // Start up the broadcast thread.
            HelloPacket hello = new HelloPacket("client", "127.0.0.3", listenPort);
            Thread broadcast1 = new Thread(() => StartBroadcast(hello));
            broadcast1.Start();

            HelloPacket hello2 = new HelloPacket("client", "127.0.0.4", listenPort);
            Thread broadcast2 = new Thread(() => StartBroadcast(hello2));
            broadcast2.Start();
            //
        }

        // This is the UDP listener which listens for the hello broadcast.
        // It will then start a tcp client to reply to the server.
        // TCP is required for the response to ensure connection.
        private static void StartListener(HelloPacket serverInfo)
        {
            Console.WriteLine($"Creating server listener {serverInfo.ToString()}");
            //IPEndPoint listenEP = new IPEndPoint(IPAddress.Parse(clientInfo.address), listenPort);
            //UdpClient listener = new UdpClient(listenEP);
            //IPEndPoint groupEP = new IPEndPoint(IPAddress.Any, listenPort);
            UdpClient server = new UdpClient(listenPort);
            Message responseData = Utilities.Serialize(serverInfo);

            try
            {
                while (true)
                {
                    
                    Console.WriteLine("Waiting for broadcast");
                    // The groupEP gets the IP address needed for the RSVP
                    // So I could just send the port as a string, but what if some other program is using this port?
                    // Then with that an object may be best.
                    
                    IPEndPoint clientEP = new IPEndPoint(IPAddress.Any, 0);
                    Message message = new Message();
                    message.data = server.Receive(ref clientEP);

                    HelloPacket receivedData = (HelloPacket)Utilities.Deserialize(message);
                    
                    Console.WriteLine($"Received broadcast from {clientEP} :");
                    Console.WriteLine($" RSVP address {receivedData.ToString()}");

                    // Generate the response data to make sure that the client
                    // Gets the correct server address and port.
                    

                    Console.WriteLine("Server sending response");
                    server.Send(responseData.data, responseData.data.Length, clientEP);

                    // Start TCP client (separate function I guess).
                    // StartTCPClient(clientInfo, serverData);
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                server.Close();
            }
        }

        // This is the UDP broadcast which will discover IP_Tato Servers on the network.
        // Stuff will have to be done for subnetting
        public static void StartBroadcast(HelloPacket thisHost)
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
            Client.Send(request.data, request.data.Length, new IPEndPoint(IPAddress.Broadcast, listenPort));

            Console.WriteLine("Message sent to the broadcast address");
            Message responseMessage = new Message(256);
            // s.Receive(responseMessage.data);

            responseMessage.data = Client.Receive(ref serverEP);
            HelloPacket responseData = (HelloPacket)Utilities.Deserialize(responseMessage);
            Console.WriteLine($"Received respone from {responseData.ToString()}");

            Client.Close();
        }


        // End of current valid code.
        // ____________________________________________________


        // This may not be needed with the shift to a client subscription model
        // This listener will listen for the response to the UDP broadcast 
        //  and finalize the setup of the client
        public static void StartTCPListener(HelloPacket server)
        {
            // This will be very similar to the client listener from the main program.
            Console.WriteLine("Starting TCP Listener for responses to Broadcast");
            TcpListener listener = null;
            try
            {

                Console.WriteLine("IP is {0}", server.address);
                IPAddress localip = IPAddress.Parse((server.address));
                Console.WriteLine("Starting a tcplistener at {0} using port {1}", localip, server.port);
                listener = new TcpListener(localip, server.port);
                listener.Start();
                Console.WriteLine("Listener has started.");

                // Create Buffer
                byte[] buffer = new byte[256];

                while (true)
                {
                    // Add an extra space to help distinguish between each server transaction.
                    Console.WriteLine("Response listener wating for a connection... ");

                    // Accept a pending connection
                    TcpClient client = listener.AcceptTcpClient();
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
                            //   By the function or workerthread.
                            HelloPacket request = Utilities.Deserialize(incomingMessage) as HelloPacket;
                            // Verify that the server received the correct data
                            Console.WriteLine("Client Received: " + request.ToString());

                            Console.WriteLine("Processing Request...");
                            // Add the client to the list of hosts
                            hostList.Add(request.address);



                            // Instantiate a Message to hold the response message
                            Message responseMessage = new Message();
                            string response = $"Client {request.ToString()} has been added to hostlist";
                            responseMessage.data = System.Text.ASCIIEncoding.ASCII.GetBytes(response);

                            // Send back a response.
                            stream.Write(responseMessage.data, 0, responseMessage.data.Length);

                            Console.WriteLine("Response Server Sent {0}", response.ToString());
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

        public static void StartTCPClient(HelloPacket clientInfo, HelloPacket server)
        {

            IPAddress serveraddress = IPAddress.Parse(server.address);
            int port = server.port;
            IPEndPoint serverEP = new IPEndPoint(serveraddress, port);
            TcpClient client = new TcpClient();
            try
            {

                client.Connect(serverEP);

                // Serialize the message


                // Serialize the object
                Message outboundMessage = Utilities.Serialize(clientInfo);


                NetworkStream stream = client.GetStream();

                stream.Write(outboundMessage.data);

                Console.WriteLine("Client Sent: {0}", clientInfo.ToString());

                // Receive the response.

                // Create Buffer
                // The size of the buffer will need to be adapted to the
                // Size of the IP_Tato object. --- Until I make a dynamic buffer
                Message inboundMessage = new Message();
                byte[] buffer = new byte[256];


                int bytes = stream.Read(buffer, 0, buffer.Length);
                inboundMessage.data = buffer;
                Console.WriteLine("Client received: {0}", System.Text.ASCIIEncoding.ASCII.GetString(inboundMessage.data));

                stream.Close();
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine("Client ArgumentNullException: {0}", e);
            }
            catch (SocketException e)
            {
                Console.WriteLine("Client SocketException: {0}", e);
            }
            client.Close();
        }

        public static void TestDualClientLogic()
        {
            int ipHostNumber = 1;
            Thread[] threadList = new Thread [maxClients];
            // For loop to spool up clients(threads).
            for (int x = 1; x < threadList.Length; x++)
            {
                Console.WriteLine("Creating Client {0}", x);
                threadList[x] = new Thread(() => CallDualClientTest(ipHostNumber++));
                threadList[x].Start();
            }
            Thread.Sleep(5000);
            // Create Threadstarts to the methods.

            Console.WriteLine("In Main: Creating the Client thread");
            Thread clientref = new Thread(() => SendUDPHello());
            clientref.Start();
            
            

            // Keep going on the main thread.

            // Console.WriteLine("\n\nMain Thread continues after client thread is started.\n\n");

            // Console.ReadKey();
        }
        public static void CallDualClientTest(int hostNumber)
        {
            // Create the ipaddress for this client.
            IPAddress listenerIP = IPAddress.Parse("127.0.0." + hostNumber);
            Console.WriteLine("Creating an endpoint at {0}", listenerIP);
            // Create an endpoint for the client to listen for udp broadcasts
            IPEndPoint udpListenerIP = new IPEndPoint(listenerIP, 12999);
            Console.WriteLine("Creating a Thread using {0}", udpListenerIP);
            // Start a thread for the udplistener
            Thread udpListenerThread = new Thread(() => StartHelloListener(udpListenerIP));
            udpListenerThread.Start();

            // Spool up the tcplistener.

            // 

            // This listener waits for a client to connect and pass a tater
            // It then sends the tater to a random other IPaddress which is active.

            // Transmission will be done through port 13000.
            // Hello (UDP) will be done through port 12999.
        }

        public static IPAddress[] SendUDPHello()
        {
            // Create a list to contain all of the hello responses.
            IPAddress[] HostList = new IPAddress [254];
            // Broadcast the request
            Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            IPAddress broadcast = IPAddress.Parse("127.0.0.255");

            byte[] sendbuf = Encoding.ASCII.GetBytes("Hello from broadcast");
            IPEndPoint ep = new IPEndPoint(broadcast, 12999);

            s.SendTo(sendbuf, ep);

            Console.WriteLine("Message has been sent to the broadcast address");
            
            return HostList;
        }

        public static void StartHelloListener(IPEndPoint localAddress)
        {
            UdpClient listener = new UdpClient(localAddress);
            IPEndPoint groupEP = new IPEndPoint(IPAddress.Parse("127.0.0."), 12999);

            try
            {
                while (true)
                {
                    Console.WriteLine("Waiting for broadcast");
                    byte[] bytes = listener.Receive(ref groupEP);

                    Console.WriteLine($"Received broadcast from {groupEP} :");
                    Console.WriteLine($" {Encoding.ASCII.GetString(bytes, 0, bytes.Length)}");
                    
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                listener.Close();
            }
        }
    }
    }

// So here goes.
// Server creates the tater
// Tater gets sent to client.
// Client gets tater and does stuff with it.
// Client sends tater back to server.
// Server receives tater and passes it back.

// So here is the plan.
// Two "clients" work together to make this whole tater thing work.

// Client one is gonna make the tater and pass it to client two
// Client two then sends it back to client one.
// Repeat until the tater explodes.

// UDP Tutorial: https://docs.microsoft.com/en-us/dotnet/framework/network-programming/using-udp-services?redirectedfrom=MSDN
// UDP Turorial #2: https://stackoverflow.com/questions/22852781/how-to-do-network-discovery-using-udp-broadcast

// So here's how the whole udp/tcp business is gonna go.
// The purpose of udp is to compile a list of hosts for the tcp to connect to.
// So when the host sends out the udp broadcast it compiles a list of replies.
// It then takes said replies and does stuff with them
// It chooses one client from the replies and sends them a tater.
