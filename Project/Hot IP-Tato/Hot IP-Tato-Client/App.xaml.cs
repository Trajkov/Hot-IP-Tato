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


namespace Hot_IP_Tato_Client
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            // Start the network listener.
            StartListener();
            // When the listener processes the correct request
            // Create and show the popup.
        }
        private void StartListener(string address = "127.0.0.1", int port = 13000)
        {
            TcpListener listener = null;
            try
            {
                IPAddress localAddr = IPAddress.Parse(address);
                // Console.WriteLine("Preparing to start a TCP listener at: " + address + " using port # " + port);

                // TcpListener server = new TcpListener(port);
                listener = new TcpListener(localAddr, port);
                // Console.WriteLine("listener Created Successfully");

                // Start listening for client requests.
                listener.Start();
                // Console.WriteLine("listener Started Successfully");
                Timer checkTimer = new Timer();
                checkTimer.Interval = 2000;

                checkTimer.Elapsed += (source, e) => isPending(source, e, listener, checkTimer);
                // Add in a stop to the timer interval.
                checkTimer.Enabled = true;
            }
            catch (SocketException e)
            {
                // Console.WriteLine("SocketException: " + e);
            }



            // Console.WriteLine("\nHit enter to continue...");
            // Console.Read();
        }
        public void isPending(Object source, System.Timers.ElapsedEventArgs e, TcpListener server, Timer timer)
        {
            //Check to see if any clients are waiting to connect to the server
            if (!server.Pending())
            {
                // Console.Write(".");
            }
            else
            {
                try
                {
                    // Buffer for reading data
                    Byte[] bytes = new Byte[256];
                    String response = "Server received data.";
                    //Enter the listening loop.
                    while (true)
                    {
                        // Console.Write("Listening for a connection");

                        // Perform a blocking call to accept requests.
                        TcpClient client = server.AcceptTcpClient();
                        // Console.WriteLine("Connected!");

                        // Get a stream object for reading and writing
                        NetworkStream stream = client.GetStream();

                        // Loop to receive all the data sent by the client.
                        while ((stream.Read(bytes, 0, bytes.Length)) != 0)
                        {
                            // Translate data bytes to a ASCII string.
                            object request = Deserialize(bytes) as object;
                            // Console.WriteLine("Server Received: " + request.ToString());

                            try
                            {
                                // Console.WriteLine("Processing Request...");
                                response = "The request was processed successfully";
                                Game_Popup gpop = new Game_Popup();
                                gpop.Show();
                                // Process the request sent by the client.
                                
                            }
                            catch (Exception ErrorProcessRequest)
                            {
                                response = "The request failed to be processed. Error details: " + ErrorProcessRequest;
                            }

                            byte[] msg = System.Text.Encoding.ASCII.GetBytes(response);

                            // Send back a response.
                            stream.Write(msg, 0, msg.Length);
                            // Console.WriteLine("Server Sent: " + response);
                        }

                        // Console.WriteLine("Server has exited while loop.");
                        // Shutdown and end connection

                        // When closing server for good, the timer should be stopped and/or
                        // * disposed if the server is completely stopped.
                        stream.Close();
                        client.Close();
                        // Console.WriteLine("Server Connection Closed.");
                    }
                }
                finally
                {
                    // Stop listening for new clients.
                    server.Stop();
                }
            }

        }
        public static Message Serialize(object anySerializableObject)
        {
            using (var memoryStream = new MemoryStream())
            {
                (new BinaryFormatter()).Serialize(memoryStream, anySerializableObject);
                return new Message { Data = memoryStream.ToArray() };
            }
        }
        public static object Deserialize(byte[] bytestream)
        {
            using (var memoryStream = new MemoryStream(bytestream))
                return (new BinaryFormatter()).Deserialize(memoryStream);
        }
        [SerializableAttribute]
        public class Message
        {
            public byte[] Data;
            public Message() { }
            public Message(byte[] Data) { this.Data = Data; }
        }
    }
}
