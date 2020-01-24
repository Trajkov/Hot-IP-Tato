using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using ConsoleSandbox;

namespace ConsoleSandbox
{

    class ObjectSandbox
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
