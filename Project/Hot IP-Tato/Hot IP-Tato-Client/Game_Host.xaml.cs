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


namespace Hot_IP_Tato_Client
{
    /// <summary>
    /// Interaction logic for Game_Host.xaml
    /// </summary>
    public partial class Game_Host : Page
    {
        private Host host;
        public Game_Host()
        {

        }
        public Game_Host(Host newHost)
        {
            
            InitializeComponent();
            host = newHost;
            host.RaiseClientJoinedEvent += Host_ClientJoined;
            host.RaiseClientDisconnectedEvent += Host_ClientDisconnected;
            List_ConnectedClients.ItemsSource = host.HostList;

            host.StartGame(1);
        }
        private void Host_ClientJoined(object sender, Common.HelloPacket e)
        {
            // Do anything special needed to set up the client
            //  Nothing so far

            this.UpdateHostList();
        }

        private void Host_ClientDisconnected(object sender, Common.HelloPacket e)
        {
            // Do anything special needed to set up the client
            //  Nothing so far
            //  Maybe send an update to the players saying client has disconnected 
            //    with a reason why or if the tater exploded. This would be part of the client screen.

            this.UpdateHostList();
        }

        private void UpdateHostList()
        {
            Application app = Application.Current;
            app.Dispatcher.Invoke((Action)delegate
            {
                List_ConnectedClients.Items.Refresh();
            });
        }

        private void btn_EndGame_Click(object sender, RoutedEventArgs e)
        {
            host.Dispose();
            NavigationService.Navigate(new MainMenu());
        }

        private void btn_AddPotato_Click(object sender, RoutedEventArgs e)
        {
            host.Add_IP_Tato();
        }

        private void btn_KickPlayer_Click(object sender, RoutedEventArgs e)
        {
            host.KickPlayer(List_ConnectedClients.SelectedItem as Common.HelloPacket);

            UpdateHostList();
        }
    }
}
