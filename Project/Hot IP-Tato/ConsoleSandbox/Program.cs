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
    [Serializable]
    public class IP_Tato
    {
        // Flavors of IPTato

        // TODO: Add in flag parameter constructor.
        // TODO: Add in a way to have a count of passes persist.
        // TODO: Actually use the targetClient Variable.

        // This is a nickname for the potato
        // This will also be used when costumes are added.
        public string Name { get; }
        // The state of the potato - alive, dead, exploded, etc.
        public string State { get; }
        // How many passes are left before the potato explodes.
        public int totalPasses { get; }
        public int passes { get; set; }
        // The target client which the potato will be sent to.
        public string targetClient { get; set; }
        // The previous client which held the tater.
        public string lastClient { get; set; }
        // This is a history of each of the players who have held the potato.
        public List<string> holderHistory = new List<string>();
        
        // The server which will moderate the game.
        // * Potatoes will be sent to the server to be passed around.
        public string server { get; }
        // All of the active players
        // * This will be updated by the server when it is passed into play.
        public List<string> players = new List<string>();




        public IP_Tato()
        {
            this.Name = "Potato";
            this.server = "127.0.0.1";
            this.targetClient = "127.0.0.1";
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
        public IP_Tato(string name, int totalPasses = 5)
        {
            this.Name = name;
            this.totalPasses = totalPasses;
            this.passes = 0;
            var flagList = new List<KeyValuePair<string, bool>>()
            {

            };
        }

        // This method checks a bunch of flags which can be applied to the object.
        public void CheckFlags()
        {
            // Return flags which are true
        }
        // 
        public void AddCurrentHostToHolderHistory()
        {
            this.holderHistory.Add(this.targetClient);
            Console.WriteLine("Added {0} to the end of the holderHistory", targetClient);
            Console.WriteLine("Contents of holderHistory:");
            holderHistory.ForEach(Console.WriteLine);
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
        public void TestMethods()
        {
            Console.WriteLine("-- Testing Methods within IP_Tato {0} --", this.Name);
            AddCurrentHostToHolderHistory();
        }
        public override string ToString()
        {
            string toString = null;
            toString = base.ToString();
            foreach (var x in this.GetType().GetProperties())
            {
                if (x.GetValue(this) != null)
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
                return (new BinaryFormatter()).Deserialize(memoryStream);
            }
        }
    }
    class Program
    {
        // TODO: create a class to help manage clients and servers.
        //          It will take a method to process the data (passing the Message object of course).
        //          It would also have to have a modular target.



        // The maximum value for the listeners will be 254.
        //  This is because it would take some work to develop subnetting.
        //  Unless I used the trick to get the network bound-netadapter.
        //  But that would be for the final product.
        static public int listenerHostNumber = 1;
        static public int listenerMaxNumber = 25;

        static public int taterNumber = 1;
        static public int taterMaxNumber = 5;

        // Misc constants to make life easier.
        static public int buffersize = 1024;
        static public int port = 13000;


        static public int totalretries = 0;
        static public int totalFails = 0;
        static public int failedConnections = 0;
        // Max attempted clients 12000
        

        public static void Main(string[] args)
        {
            TestTater();
            TestClientandListenerThreads();
            Console.ReadKey();
        }
        public static void TestTater()
        {
            // Test IP_Tato Object
            Console.WriteLine("Test the IP_Tato");
            IP_Tato tater = new IP_Tato("Tater", 5);
            Console.WriteLine("Hot IP_Tato Object:");
            Console.WriteLine(tater.ToString());

            // Test the methods within the tater
            tater.TestMethods();

            // Serialization Test
            Console.WriteLine("Testing Serialization and Deserialization.");
            Message serializedTater = Utilities.Serialize(tater);
            Console.WriteLine("Serialized Tater: {0}", serializedTater);
            Console.WriteLine("Size of the serialized object: {0} bytes", serializedTater.Data.Length);

            IP_Tato newTater = new IP_Tato();
            newTater = (IP_Tato)Utilities.Deserialize(serializedTater);
            Console.WriteLine("Deserialized Tater: {0}", newTater.ToString());

        }
        public static void TestClientandListenerThreads()
        {
            Console.WriteLine("\n\nTesting the Program:");
            // Test the Client and Listener Threads

            // Create Threadstarts to the methods.
            // ThreadStart listenerref = new ThreadStart(CallListenerTest);
            Console.WriteLine("In Main: Creating the Listener threads");
            
            Thread[] listenerThreadList = new Thread[listenerMaxNumber];
            for (int x = 0; x < listenerThreadList.Length; x++)
            {
                Console.WriteLine("Creating Listener {0}", x+1);
                listenerThreadList[x] = new Thread(() => CallListenerTest(listenerHostNumber++));
                listenerThreadList[x].IsBackground = true;
                listenerThreadList[x].Start();
            }
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();

            Thread[] serverThreadList = new Thread[taterMaxNumber];
            for (int x = 0; x < serverThreadList.Length; x++)
            {
                Console.WriteLine("Creating IP_Tato server {0}", x + 1);
                IP_Tato tater = new IP_Tato("Tater #" + x, 5);
                serverThreadList[x] = new Thread(() => ServerTest(tater));
                serverThreadList[x].Start();
            }
            

            // Create the listener thread.
            // Thread listenerThread = new Thread(listenerref);
            // This makes it so that the program never is stuck open waiting on the listener to shut down.
            

            // Keep going on the main thread.

            Console.WriteLine("\n\nMain Thread has concluded.\n\n");

            Console.ReadKey();
        }

        public static void CallClientTest()
        {
            while (taterNumber < taterMaxNumber)
            {
                IP_Tato tater = new IP_Tato("Tater #" + taterNumber++, 15);
                ServerTest(tater);
            }
            
        }
        private static void ServerTest(IP_Tato tater)
        {
            // TODO Add in a test over a network connection (to verify how much it can handle)
            // TODO Figure out how to negate Denial of Service Attacks.

            // Add in tater stuff

            

            // These values will actually need to be changed when tater-routing is integrated.

            // This while handles just one tater until it's lifetime expires
            // So then really this function is more like a StartTato() method.
            // Especially once potato routing is a thing.

            // When this all is converted to be OOP, then hostlist may become a part of the server class
            List<string> hostList = new List<string>();
            for (int x = 1; x < listenerMaxNumber+1; x++)
            {
                string tempip = "127.0.0." + x;
                Console.WriteLine("Adding {0} to tater.players", tempip);
                hostList.Add(tempip);
            }
            
            

            while (tater.passes < tater.totalPasses)
            {
                // Choose a targetClient

                tater.targetClient = RouteTater(hostList, tater.lastClient);

                Console.WriteLine("The new targetClient is: {0}", tater.targetClient);



                // Set the values which will be used for each connection
                IPAddress serveraddress = IPAddress.Parse(tater.targetClient);
                int port = 13000;
                IPEndPoint server = new IPEndPoint(serveraddress, port);
                // The client is disposed when the connection is closed, so it must be recreated in each loop.
                TcpClient client = new TcpClient();

                // Initialize the retry counter (which may be overkill)
                int retries;
                for (retries = 0; retries < 5; retries++)
                {
                    // Handle unseen errors
                    try
                    {
                        // Connect to the server
                        client.Connect(server);

                        // Serialize the object which will be sent
                        Message outboundMessage = Utilities.Serialize(tater);

                        // Initialize the stream
                        NetworkStream stream = client.GetStream();

                        // Send the object through the stream
                        Console.WriteLine("Server Sent {0} bytes.", outboundMessage.Data.Length);
                        stream.Write(outboundMessage.Data);
                        Console.WriteLine("Server Sent: {0} \n Retry #: {1}", tater.ToString(), retries);

                        // -- Part 2: Receive the response. --

                        // Create a Message object to receive the response
                        // Create Buffer
                        // The size of the buffer will need to be adapted to the
                        // Size of the IP_Tato object. --- Until I make a dynamic buffer
                        Message inboundMessage = new Message();
                        byte[] buffer = new byte[buffersize];

                        // This reads the server's response from the network
                        // It assigns the response byte [] to the buffer.
                        int bytes = stream.Read(buffer, 0, buffer.Length);
                        inboundMessage.Data = buffer;
                        // Deserialize the data which was received and create a tater
                        object responseData = Utilities.Deserialize(inboundMessage) as object;
                        tater = responseData as IP_Tato;
                        // Verify the current state is correct
                        Console.WriteLine("Server received: {0}", tater.ToString());

                        // Close out the connection
                        stream.Close();
                        Console.WriteLine("---Server Transaction Closed---");

                        // Break out of the retry loop.
                        break;
                    }
                    catch (ArgumentNullException e)
                    {
                        Console.WriteLine("Server ArgumentNullException: {0}", e);
                        continue;
                    }
                    catch (SocketException e)
                    {
                        Console.WriteLine("Server SocketException: {0}", e);
                        continue;
                    }

                }
                client.Close();
                
            }
            Console.WriteLine("The IP_Tato {0} has exploded.", tater.Name);
        }
        private static void ClientPassTater(IP_Tato tater)
        {
            // TODO Add in a test over a network connection (to verify how much it can handle)
            // TODO Figure out how to negate Denial of Service Attacks.
            
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

                    string message = "Herro, this is dog #" + " Retry #" + retries;

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
                    byte[] buffer = new byte[buffersize];


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
        public static void CallListenerTest(int ipHostNumber)
        {
            Console.WriteLine("IP host number passed to listener: {0}", ipHostNumber);
            TcpListener listener = null;
            try
            {
                string ip = "127.0.0." + ipHostNumber;
                Console.WriteLine("IP is {0}", ip);
                IPAddress localip = IPAddress.Parse(("127.0.0."+ ipHostNumber));
                Console.WriteLine("Starting a tcplistener at {0} using port {1}", localip, port);
                listener = new TcpListener(localip, port);
                listener.Start();
                Console.WriteLine("Listener has started.");

                // Create Buffer
                byte[] buffer = new byte[buffersize];

                while (true)
                {
                    // TODO Add in differentiation of messages - potatoes and networking business.

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
                            incomingMessage.Data = buffer;
                            // Deserialize the inbound data into an object which can be processed 
                            //   By the function or workerthread.
                            object request = Utilities.Deserialize(incomingMessage) as object;
                            // Verify that the server received the correct data
                            Console.WriteLine("Client Received: " + request.ToString());

                            Console.WriteLine("Processing Request...");
                            
                            // TODO: Create a worker thread to work with the potato.
                            //      This will be especially necessary when UI gets involved.
                            // For now it is just going to call a function
                            object objectResponse = ProcessPotato(request);
                            Thread.Sleep(100);



                            // Instantiate a Message to hold the response message
                            Message responseMessage = new Message();
                            responseMessage = Utilities.Serialize(objectResponse);

                            // Send back a response.
                            stream.Write(responseMessage.Data, 0, responseMessage.Data.Length);
                            // Verify that the data sent against the client receipt.
                            Console.WriteLine("Client Sent {0}", objectResponse.ToString());
                        }
                        catch (Exception ErrorProcessRequest)
                        {
                            Console.WriteLine("The request failed to be processed. Error details: " + ErrorProcessRequest);
                        }



                        /*
                        if (data.Contains("HERRO, THIS IS DOG #"+totalClients))
                        {
                            showClientTelemetry();
                        }
                        */
                        // byte[] msg = System.Text.Encoding.ASCII.GetBytes("Server Received: " + data.ToString());

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
            // Increment current passes
            tater.passes++;
            // Add the current host to the holderHistory
            // This is done on the client side for pessimism's sake
            // tater.AddCurrentHostToHolderHistory();
            // Instead set the previous client to self
            tater.lastClient = tater.targetClient;

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
        // lastClient may not be needed, but this class will be expanded to include other routing protocols
        public static string RouteTater(List<string> hostList, string lastClient)
        {
            // This method could include a using statement like what is used in Serialize

            // rolls is used to determine the effectiveness of the routing protocol
            // Not the best, because it is entirely dependant on the size of the host list
            int rolls = 0;
            Random random = new Random();
            string newTargetClient;
            do
            {
                newTargetClient = hostList[random.Next(hostList.ToArray().Length)];
                rolls++;
            }
            while (newTargetClient == lastClient);
            Console.WriteLine("New targetClient {0} chosen after {1} roll(s)", newTargetClient, rolls);
            return newTargetClient;
        }

        static void StartProcess()
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

// Process for picking a potato.
// Create a list of players -- This will be hardcoded at first.

// --Client actions--
// Listen for potato.
// Process Potato.
// Return potato.
// * If it were an automatic process this could all be done inside the listener

// --Server Actions--
// Creating potatoes
// Distributing Potatoes
// Sending Potatoes
// Receiving potatoes
// 

// Minimum things to do (for an automatic process).
// Clients listen
// Server sends the first potato to client
// Client received potato and processes it before passing it back as a response.
// Whenever the server receives a non-exploded potato it picks a new client and sends it to them