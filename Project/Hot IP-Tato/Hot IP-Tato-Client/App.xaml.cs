using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Net;
using System.Net.Sockets;
using System.Timers;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using Common;


namespace Hot_IP_Tato_Client
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        [STAThread]
        public static void Main()
        {
            Console.WriteLine("Main is run");
            var app = new App();
            app.InitializeComponent();
            app.Run();
        }
        private void Application_Startup()
        {
            Console.WriteLine("Application Startup has been called.");
            // TODO: add in a config screen which will update the client information.
            // IP_Tato tater = new IP_Tato();
            // ProcessPotato(tater);

            HelloPacket clientInfo = new HelloPacket("client", Networking.getExternalIPE(), 13000);

            // Start the network discovery
            Console.WriteLine("Starting the UDP Broadcast at {0}", clientInfo.ToString());
            Thread UDPBroadcast = new Thread(() => StartBroadcast(clientInfo));
            UDPBroadcast.Start();

            // Start the network listener.
            Thread ListenerThread = new Thread(()=>StartListener(clientInfo));
            ListenerThread.Start();
            // When the listener processes the correct request
            // Create and show the popup.
            
        }
        // This listener will basically function as the client for most intents and purposes.
        private static void StartListener(HelloPacket clientInfo)
        {
            TcpListener listener = null;
            try
            {
                Console.WriteLine("IP is {0}", clientInfo.address);
                IPAddress localip = IPAddress.Parse((clientInfo.address));
                Console.WriteLine("Starting a tcplistener at {0} using port {1}", localip, clientInfo.port);
                listener = new TcpListener(localip, clientInfo.port);
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
            

            // Add the current host to the holderHistory
            // This is done on the client side for pessimism's sake
            // tater.AddCurrentHostToHolderHistory();
            // Instead set the previous client to self
            tater.LastClient = tater.TargetClient;

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
            Application app = Application.Current;
            if (app == null)
            {
                Main();
                app = Current;
            }
            app.Dispatcher.Invoke((Action)delegate {
                // Try to update GUI from this thread.
                Game_Popup game_Popup = new Game_Popup(tater);
                
                // The ShowDialog is the perfect function for this.
                // It blocks until the window is closed which is all I needed it to do.
                game_Popup.ShowDialog();
            });
            // Increment current passes
            // This is done at the end in case of an involuntary host disconnect
            tater.Passes++;

            return tater as object;
        }

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
            Client.Send(request.data, request.data.Length, new IPEndPoint(IPAddress.Broadcast, 13000));

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
