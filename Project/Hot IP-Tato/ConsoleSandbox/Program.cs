using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace ConsoleSandbox
{
    [SerializableAttribute]
    public class IP_Tato
    {
        // Flavors of IPTato

        // TODO: Add in flag parameter constructor.
        // TODO: Add in a way to have a count of passes persist.

        // This is a nickname for the potato
        // This will also be used when costumes are added.
        public string Name { get; }
        // The state of the potato - alive, dead, exploded, etc.
        public string State { get; }
        // How many passes are left before the potato explodes.
        public int totalPasses { get; }
        public int passes { get; set; }
        // This is a history of each of the players who have held the potato.
        public string[] holderHistory { get;}
        // The server which will moderate the game.
        // * Potatoes will be sent to the server to be passed around.
        public string server { get; }
        // All of the active players
        // * This will be updated by the server when it is passed into play.
        public string[] players { get; }
        



        public IP_Tato()
        {
            this.Name = "Potato";
            this.server = "127.0.0.1";
            this.totalPasses = 5;
            this.passes = 0;
        }
        public IP_Tato(string name)
        {
            this.Name = name;
        }
        // public IP_Tato(Byte[] serializedTater)
        // {
            // This constructor will eventually utilize the deserialize method
            //  and build a tater from the byte array
            // I don't know how to have it create itself without creating one,
            //  It might just create a tater and then assign it all of the things which
            //   The byte array contains. (but it would be nice if I could just deserialize here.
        // }
        
        // This constructor is for the server.
        public IP_Tato(string name, string origin, int totalPasses = 5)
        {
            this.Name = name;
            this.server = server;
            this.totalPasses = totalPasses;
            this.passes = 0;
            var flagList = new List<KeyValuePair<string, bool>>()
            {

            };
        }

        // This method checks a bunch of flags which can be applied to the object.
        public void CheckFlags()
        {
            
        }
        
        // This will do the things which the potato should do when it expires.
        public void Explode()
        {
            Console.WriteLine("\n.\n.\n.\nKABOOOOOOOOOOOOOOOOOM!!!!!!\nIP_Tato {0} has exploded.", this.Name);
        }
        // Passes the potato to the origin 
        public void PassToServer()
        {

        }
        // Passes the potato to another member of the group
        public void PassOnInGroup()
        {

        }
        public override string ToString()
        {
            string toString = null;
            toString = base.ToString();
            foreach (var x in this.GetType().GetProperties())
            {
                if(x.GetValue(this) != null)
                {
                    // Add to ToString
                    toString += "\n " + x.Name + " " 
                        + x.GetValue(this);
                }
            }
            return toString;
        }
        // **Manipulators for Objects**
        // Process for the process:
        // * 
    }
    public class Message
    {
        public byte[] Data { get; set; }
    }
    public static class Utilities
    {
        public static Message Serialize(object anySerializableObject)
        {
            using (var memoryStream = new MemoryStream())
            {
                (new BinaryFormatter()).Serialize(memoryStream, anySerializableObject);
                return new Message { Data = memoryStream.ToArray() };
            }
            
        }
        public static object Deserialize(Message message)
        {
            using (var memoryStream = new MemoryStream(message.Data))
            {
                Console.WriteLine("Message in Deserialize, {0}", message.Data.Length);
                Console.WriteLine("Memorystream in Deserialize, {0}", memoryStream.Length);
                return (new BinaryFormatter()).Deserialize(memoryStream);
            }
        }
    }
    class Program
    {
        static public int clientnumber = 1;
        static public int totalretries = 0;
        static public int totalFails = 0;
        static public int failedConnections = 0;
        // Max attempted clients 12000
        static public int totalClients = 1;

        public static void Main(string[] args)
        {
            ClientLogic.TestDualClientLogic();
        }
        public static void TestClientandListenerThreads()
        {
            // Test IP_Tato Object
            IP_Tato tater = new IP_Tato("Tater", "127.0.0.1", 5);
            Console.WriteLine("Hot IP_Tato Object:");
            Console.WriteLine(tater.ToString());

            // Serialization Test
            Message serializedTater = Utilities.Serialize(tater);
            Console.WriteLine("Serialized Tater: {0}", serializedTater);
            IP_Tato newTater = new IP_Tato();
            newTater = (IP_Tato)Utilities.Deserialize(serializedTater);
            Console.WriteLine("Deserialized Tater: {0}", newTater.ToString());

            // Test the Client and Listener Threads
            // TestClientandListenerThreads();

            // Serialization adventures
            Console.ReadKey();
            // Create Threadstarts to the methods.
            ThreadStart listenerref = new ThreadStart(CallListenerTest);
            Console.WriteLine("In Main: Creating the Listener thread");
            ThreadStart clientref = new ThreadStart(CallClientTest);
            Console.WriteLine("In Main: Creating the Client thread");

            // Create the listener thread.
            Thread listenerThread = new Thread(listenerref);
            // This makes it so that the program never is stuck open waiting on the listener to shut down.
            listenerThread.IsBackground = true;
            listenerThread.Start();
            Console.WriteLine("Main Thread continues after listener thread is started.");

            // Wait for a couple seconds
            Thread.Sleep(2000);
            StressTestListener(clientref);

            // Keep going on the main thread.
            
            // Console.WriteLine("\n\nMain Thread continues after client thread is started.\n\n");

            // Console.ReadKey();
        }
        public static void CallToChildThread()
        {
            Console.WriteLine("Child thread starts");

            // the thread is paused for 5000 milliseconds
            int sleepfor = 5000;

            Console.WriteLine("Child Thread Paused for {0} seconds", sleepfor / 1000);
            Thread.Sleep(sleepfor);
            Console.WriteLine("Child thread resumes");
        }
        
        public static void CallClientTest()
        {
            IP_Tato tater = new IP_Tato("Tater", "127.0.0.1", 5);
            ClientTest(tater);
        }
        private static void ClientTest(IP_Tato tater)
        {
            // TODO Add in a test over a network connection (to verify how much it can handle)
            // TODO Figure out how to negate Denial of Service Attacks.
            int clientnum = clientnumber++;
            IPAddress serveraddress = IPAddress.Parse("127.0.0.1");
            int port = 13000;
            IPEndPoint server = new IPEndPoint(serveraddress, port);
            TcpClient client = new TcpClient();

            int retries;
            for (retries = 0; retries < 5; retries++)
            {
                try
                {

                    client.Connect(server);

                    string message = "Herro, this is dog #" + clientnum + " Retry #" + retries;

                    // Serialize the message
                    

                    // Serialize the object
                    Message outboundMessage = Utilities.Serialize(tater);
                    IP_Tato testtater = (IP_Tato)Utilities.Deserialize(outboundMessage);
                    Console.WriteLine("IP_Tato before client sends it. {0}", testtater.ToString());
                    
                    NetworkStream stream = client.GetStream();

                    stream.Write(outboundMessage.Data);

                    Console.WriteLine("Client Sent: {0} Retry #: {1}", tater.ToString(), retries);

                    // Receive the response.

                    // Create Buffer
                    // The size of the buffer will need to be adapted to the
                    // Size of the IP_Tato object. --- Until I make a dynamic buffer
                    Message inboundMessage = new Message();
                    byte[] buffer = new byte[512];
                    

                    int bytes = stream.Read(buffer, 0, buffer.Length);
                    inboundMessage.Data = buffer;
                    object responseData = Utilities.Deserialize(inboundMessage) as object;
                    IP_Tato newtater = responseData as IP_Tato;
                    Console.WriteLine("Client received: {0}", newtater.ToString());

                    stream.Close();
                    totalretries = totalretries + retries;
                    break;
                }
                catch (ArgumentNullException e)
                {
                    Console.WriteLine("Client ArgumentNullException: {0}", e);
                    continue;
                }
                catch (SocketException e)
                {
                    Console.WriteLine("Client SocketException: {0}", e);
                    continue;
                }

            }
            client.Close();
            if (retries > 4)
            {
                totalFails++;
            }
            // Moved telemetry display to server
        }
        public static void showClientTelemetry()
        {
            Console.WriteLine("\n\nTest concluded with the following telemetry:");
            Console.WriteLine("Total number of clients: {0}", totalClients);
            Console.WriteLine("Total number of retries: {0}", totalretries);
            Console.WriteLine("Total failed client requests (too many retries): {0}", totalFails);
            /*
            Console.WriteLine("Retries per each amount:");
            int i;
            for(i = 0; i<retryByNumber.Length; i++)
            {
                Console.WriteLine("\t{0} retries: {1}", i, retryByNumber[i]);
            }
            */
        }

        // This listener will basically function as the client for most intents and purposes.
        public static void CallListenerTest()
        {
            TcpListener listener = null;
            try
            {
                IPAddress localip = IPAddress.Parse("127.0.0.1");
                Console.WriteLine("Starting a tcplistener at {0} with the port 13000", localip);
                listener = new TcpListener(localip, 13000);
                listener.Start();
                Console.WriteLine("Listener has started.");

                // Create Buffer
                byte[] buffer = new byte[512];

                while (true)
                {
                    Console.Write("Server wating for a connection... ");

                    TcpClient client = listener.AcceptTcpClient();
                    Console.WriteLine("Connected!");
                    
                    string response;

                    NetworkStream stream = client.GetStream();
                    Console.WriteLine("Server Stream Created");

                    while ((stream.Read(buffer, 0, buffer.Length)) != 0)
                    {
                        try
                        {
                            Console.WriteLine("Buffer inside of while loop {0}", buffer.Length);
                        // Translate data to string
                        // data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                        // TODO Add in differentiation of messages - potatoes and networking business.
                        Message incomingMessage = new Message();
                        incomingMessage.Data = buffer;
                        object request = Utilities.Deserialize(incomingMessage) as object;
                        Console.WriteLine("Server Received: " + request.ToString());

                        object objectResponse;

                        
                            Console.WriteLine("Processing Request...");
                            // Create a worker thread to work with the potato.
                            // For now it is just going to call a funciton
                            objectResponse = ProcessPotato(request);


                            response = "The request was processed successfully";

                            // Process the request sent by the client.
                            //ProcessRequest(request);
                        

                        Message outgoingMessage = new Message();
                        outgoingMessage = Utilities.Serialize(objectResponse);

                        // Send back a response.
                        stream.Write(outgoingMessage.Data, 0, outgoingMessage.Data.Length);
                        Console.WriteLine("Server Sent: " + response);
                        }
                        catch (Exception ErrorProcessRequest)
                        {
                            response = "The request failed to be processed. Error details: " + ErrorProcessRequest;
                        }



                        /*
                        if (data.Contains("HERRO, THIS IS DOG #"+totalClients))
                        {
                            showClientTelemetry();
                        }
                        */
                        // byte[] msg = System.Text.Encoding.ASCII.GetBytes("Server Received: " + data.ToString());

                    }
                    stream.Close();
                    client.Close();
                }
            } catch(SocketException e)
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
            // Create IP_Tato
            IP_Tato tater = obj as IP_Tato;
            // Increment current passes
            tater.passes++;

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
            if (tater.passes >= tater.totalPasses)
            {
                tater.Explode();
            }
            return tater as object;
        }

        private static void StressTestListener(ThreadStart clientref)
        {
            int x;
            for (x = 0; x < totalClients; x++)
            {
                Console.WriteLine("Creating client {0}", x + 1);
                // Start client connection thread.
                Thread clientThread = new Thread(clientref);
                clientThread.Start();
            }
        }

        static void StartProcess ()
        {
            try
            {
                using (Process myProcess = new Process())
                {
                    myProcess.StartInfo.UseShellExecute = false;
                    // You can start any process, HelloWorld is a do-nothing example.
                    myProcess.StartInfo = new ProcessStartInfo("chrome.exe");
                    myProcess.StartInfo.FileName = @"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe";
                    myProcess.StartInfo.CreateNoWindow = true;
                    myProcess.Start();
                    // This code assumes the process you are starting will terminate itself. 
                    // Given that is is started without a window so you cannot terminate it 
                    // on the desktop, it must terminate itself or you can do it programmatically
                    // from this application using the Kill method.
                    //C:\Users\Micah Clegg\Documents\GitHub\Hot-IP-Tato\Project\Hot IP-Tato\Hot IP-Tato\bin\Debug
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
