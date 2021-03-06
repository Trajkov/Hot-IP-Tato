﻿using System;
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
            
            // host.ClientJoined += UpdateClientList();
            // Bind the Hostlist to the ListBox
            
            // Binding bind_ListBox_ConnectedClients = new Binding();
            // bind_ListBox_ConnectedClients.Source = host.ClientList;
            InitializeComponent();

            host = new Host(localIP: true);
            host.RaiseClientJoinedEvent += Host_ClientJoined;
            // Start listening for clients on the LAN
            host.StartDiscoverClient();

            List_ConnectedClients.ItemsSource = host.HostList;
        }

        private void Host_ClientJoined(object sender, Common.HelloPacket e)
        {
            // Do anything special needed to set up the client
            //  Nothing so far

            UpdateHostList();
        }

        private void UpdateHostList()
        {
            Application app = Application.Current;
            app.Dispatcher.Invoke((Action)delegate
            {
                List_ConnectedClients.Items.Refresh();

                // If there is more than the minimum number of clients enable the game to start
                if (host.HostList.ToArray().Length > 1)
                {
                    btn_Start.IsEnabled = true;
                }
                else if (host.HostList.ToArray().Length < 2)
                {
                    btn_Start.IsEnabled = false;
                }
            });
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
