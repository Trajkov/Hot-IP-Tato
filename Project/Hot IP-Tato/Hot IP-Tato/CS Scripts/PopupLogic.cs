using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;
using System.Net;
using System.Net.Sockets;
using System.Timers;

namespace Hot_IP_Tato.CS_Scripts
{
    class PopupLogic : BaseTCPListener
    // Ideally this listener would extend the serversocket
    {
        protected Game_Popup popup_window;
        public PopupLogic()
        {

        }
        new public void Test()
        {
            // Use a UDP connection to get the internet facing ipv4 address.
            // https://stackoverflow.com/a/27376368
            string localIP;
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
            {
                socket.Connect("8.8.8.8", 65530);
                IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                localIP = endPoint.Address.ToString();
            }
            // Get and show ipaddress
            Console.WriteLine("This machine's ip address is: {0}", localIP);

            // Start a listener on the local ipaddress on a shared port.
            // The shared port is 17470 (ITATO in 1337.)
            StartListener("127.0.0.1", 17470);
        }
        public override void ProcessRequest(object request)
        {
            IPTato iptato = request as IPTato;
            // Populate popupView with IPTato data.
            // this.Dispatcher.CheckAccess();
        }
    }
}
