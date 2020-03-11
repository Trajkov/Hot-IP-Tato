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
using System.Threading;
using Common;

namespace Hot_IP_Tato_Client
{
    /// <summary>
    /// Interaction logic for Game_Config.xaml
    /// </summary>
    public partial class Game_Config : Page
    {
        
        private Host host;
        private Thread[] ClientThreadArray = new Thread[25];
        private List<Thread> ClientThreads = new List<Thread>();
        private int ClientNum = 1;
        
        public Game_Config()
        {
            // Start listening for clients on the LAN
            host = new Host(localIP: true);
            host.StartDiscoverClient();
            // host.ClientJoined += UpdateClientList();
            // Bind the Hostlist to the ListBox
            
            // Binding bind_ListBox_ConnectedClients = new Binding();
            // bind_ListBox_ConnectedClients.Source = host.ClientList;
            InitializeComponent();

            host.ClientJoined += Host_ClientJoined;
            List_ConnectedClients.ItemsSource = host.HostList;
        }

        private void Host_ClientJoined(object sender, Common.HelloPacket e)
        {
            // Add the new client to the listview.
            
        }

        private void UpdateHostList()
        {
            // It was not updating because the source was already set to the hostlist.
            // This will refresh the binding on the ListView
            List_ConnectedClients.ItemsSource = null;
            List_ConnectedClients.ItemsSource = host.HostList;
        }
        public void btn_Start_Click(object sender, RoutedEventArgs e)
        {
            //Collect data from config sliders and stuff

            // Make sure that there is at least one host.

            //Send data to Game_Host
            NavigationService.Navigate(new Game_Host(host));
        }

        private void btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            host.Dispose();
            NavigationService.GoBack();
        }

        private void btn_SpawnClient_Click(object sender, RoutedEventArgs e)
        {
            Thread clientThread = new Thread(() => {
                Client client = new Client($"Client # {ClientNum++}", true);
                client.Start();
            });
            clientThread.Start();
            // When used this way this list will allow us to clean up all of the clients.
            ClientThreads.Add(clientThread);
        }

        private void btn_RefreshHostList_Click(object sender, RoutedEventArgs e)
        {
            UpdateHostList();
        }
    }
}
