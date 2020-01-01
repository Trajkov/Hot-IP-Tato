using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Timers;

namespace Hot_IP_Tato.CS_Scripts
{
    abstract class BaseTCPListener
    {
        // TODO expand ServerSocket to implement OOP
        // Should this only allow one active listener per object?
        public void Test()
        {
            Console.WriteLine("Testing Base TCP Listener");
            StartListener("127.0.0.1", 13000);
            //StartListener("127.0.0.2", 13001);
        }
        public void isPending(Object source, System.Timers.ElapsedEventArgs e, TcpListener server, Timer timer)
        {
            //Check to see if any clients are waiting to connect to the server
            if(!server.Pending())
            {
                Console.Write(".");
            } else
            {
                try
                {
                    // Buffer for reading data
                    Byte[] bytes = new Byte[256];
                    String response = "Server received data.";
                    //Enter the listening loop.
                    while (true)
                    {
                        Console.Write("Listening for a connection");

                        // Perform a blocking call to accept requests.
                        TcpClient client = server.AcceptTcpClient();
                        Console.WriteLine("Connected!");

                        // Get a stream object for reading and writing
                        NetworkStream stream = client.GetStream();
                        
                        // Loop to receive all the data sent by the client.
                        while ((stream.Read(bytes, 0, bytes.Length)) != 0)
                        {
                            // Translate data bytes to a ASCII string.
                            object request = ClientSocket.Deserialize(bytes) as object;
                            Console.WriteLine("Server Received: " + request.ToString());

                            try
                            {
                                Console.WriteLine("Processing Request...");
                                response = "The request was processed successfully";

                                // Process the request sent by the client.
                                ProcessRequest(request);
                            }
                            catch (Exception ErrorProcessRequest)
                            {
                                response = "The request failed to be processed. Error details: " + ErrorProcessRequest;
                            }

                            byte[] msg = System.Text.Encoding.ASCII.GetBytes(response);

                            // Send back a response.
                            stream.Write(msg, 0, msg.Length);
                            Console.WriteLine("Server Sent: " + response);
                        }

                        Console.WriteLine("Server has exited while loop.");
                        // Shutdown and end connection

                        // When closing server for good, the timer should be stopped and/or
                        // * disposed if the server is completely stopped.
                        stream.Close();
                        client.Close();
                        Console.WriteLine("Server Connection Closed.");
                    }
                }
                finally
                {
                    // Stop listening for new clients.
                    server.Stop();
                }
            }
            
        }
        public abstract void ProcessRequest(object request);
        
        public void StartListener(string address = "127.0.0.1", int port = 13000)
        {
            TcpListener listener = null;
            try
            {
                IPAddress localAddr = IPAddress.Parse(address);
                Console.WriteLine("Preparing to start a TCP listener at: " + address + " using port # " + port);

                // TcpListener server = new TcpListener(port);
                listener = new TcpListener(localAddr, port);
                Console.WriteLine("listener Created Successfully");

                // Start listening for client requests.
                listener.Start();
                Console.WriteLine("listener Started Successfully");
                Timer checkTimer = new Timer();
                checkTimer.Interval = 2000;

                checkTimer.Elapsed += (source, e) => isPending(source, e, listener, checkTimer);
                // Add in a stop to the timer interval.
                checkTimer.Enabled = true;
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: " + e);
            }
            


            Console.WriteLine("\nHit enter to continue...");
            Console.Read();
        }
    }
}

//Retrieved from: https://docs.microsoft.com/en-us/dotnet/api/system.net.sockets.tcplistener?view=netframework-4.8
