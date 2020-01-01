using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
using Hot_IP_Tato.CS_Scripts;
using System.Net;
using System.Diagnostics;


namespace Hot_IP_Tato
{

    /// <summary>
    /// Interaction logic for TestPage.xaml
    /// </summary>
    /// 

    public partial class TestPage : Page
    {
        public TestPage()
        {
            InitializeComponent();

        }

        void TimerTick(object state)
        {
            var who = state as string;
            Console.WriteLine(who);
        }

        public void Server_Test_Click(object sender, RoutedEventArgs e)
        {
            using (Process serverProcess = new Process())
            {
                serverProcess.StartInfo.UseShellExecute = false;
                serverProcess.StartInfo = new ProcessStartInfo("Hot_IP_Tato_Client.exe");

                serverProcess.StartInfo.FileName = @"C:\Users\Micah Clegg\Documents\GitHub\Hot-IP-Tato\Project\Hot IP-Tato\Hot IP-Tato-Client\bin\Debug\Hot IP-Tato-Client.exe";


                serverProcess.Start();

                //C:\Users\Micah Clegg\Documents\GitHub\Hot-IP-Tato\Project\Hot IP-Tato\Hot IP-Tato\bin\Debug
                //C:\Users\Micah Clegg\Documents\GitHub\Hot-IP-Tato\Project\Hot IP-Tato\Hot IP-Tato-Client\bin\Debug
            }



        }
        public void Client_Test_Click(object sender, RoutedEventArgs e)
        {
            IPAddress localAddr = IPAddress.Parse("127.0.0.1");
            string test = "test";
            Console.WriteLine("a {0}", test);
            Console.WriteLine("ip ", localAddr);
            ClientSocket.Test();
        }
        public void Popup_Test_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Popup Listener Test");
            // Game_Popup gpop = new Game_Popup();
            PopupLogic popup1 = new PopupLogic();
            this.Dispatcher.Invoke(() => popup1.Test());
        }
        public void New_Window_Test_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("New Window Test");
            Game_Popup gpop = new Game_Popup();
            gpop.Show();
        }
    }

}
