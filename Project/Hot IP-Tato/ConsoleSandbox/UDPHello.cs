using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using ConsoleSandbox;

// TODO: finish putting together the logic for the Headless Model.
// TODO: Perhaps break some part of this class into an object (to make code more elegant and better self-referencing).

namespace ConsoleSandbox
{
    class ClientLogic
    {
        // public int IPHostnumber = 1;
        private const int maxClients = 2;
        public static void TestDualClientLogic()
        {
            int ipHostNumber = 1;
            Thread[] threadList = new Thread [maxClients];
            // For loop to spool up clients(threads).
            for (int x = 1; x < threadList.Length; x++)
            {
                Console.WriteLine("Creating Client {0}", x);
                threadList[x] = new Thread(() => CallDualClientTest(ipHostNumber));
                threadList[x].Start();
                ipHostNumber++;
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
            IPEndPoint groupEP = new IPEndPoint(IPAddress.Parse("127.0.0.0"), 12999);

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

    // So here's how the whole udp/tcp business is gonna go.
    // The purpose of udp is to compile a list of hosts for the tcp to connect to.
    // So when the host sends out the udp broadcast it compiles a list of replies.
    // It then takes said replies and does stuff with them
    // It chooses one client from the replies and sends them a tater.
