using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Common;

namespace Hot_IP_Tato_Client
{
    /// <summary>
    /// Interaction logic for Page1.xaml
    /// </summary>
    public partial class Join : Page
    {
        HelloPacket clientInfo;
        public Join()
        {
            clientInfo = new HelloPacket("client", Utilities.getExternalIPE(), 13000);
            InitializeComponent();
        }

        private void btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            // Clean up everything.
            NavigationService.Navigate(new Uri("MainMenu.xaml", UriKind.Relative));
        }

        private void btn_Refresh_Click(object sender, RoutedEventArgs e)
        {
            // Start the network discovery
            Console.WriteLine("Starting the UDP Broadcast at {0}", clientInfo.ToString());
            Thread UDPBroadcast = new Thread(() => StartBroadcast(clientInfo));
            UDPBroadcast.Start();
        }

        private void btn_Join_Click(object sender, RoutedEventArgs e)
        {
            // Start the network listener.
            Thread ListenerThread = new Thread(() => Client.Listen(clientInfo));
            ListenerThread.Start();
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
    }
}
