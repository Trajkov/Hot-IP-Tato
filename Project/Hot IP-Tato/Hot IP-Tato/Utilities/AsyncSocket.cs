using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace Hot_IP_Tato.CS_Scripts.Utilities
{
    class AsyncSocket
    {
        public static void Test()
        {


        }
        public static void ServerSocket()
        {
            //Create a socket as an ipv4 datastream using tcp
            Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //Get the address of the target host (using dns)
            IPHostEntry ipHostInfo = Dns.GetHostEntry("host.contoso.com");
            // Select the first entry (which is probably correct
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPEndPoint ipe = new IPEndPoint(ipAddress, 11000);

        }
    }
}
