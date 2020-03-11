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
            host = newHost;
            InitializeComponent();
            host.StartGame(1);
        }
        public void Wait()
        {
            Thread.Sleep(5000);
            host = null;
            NavigationService.GoBack();
        }

        private void btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            host.StopGame();
            host = null;
            NavigationService.Navigate(new MainMenu());
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }
    }
}
