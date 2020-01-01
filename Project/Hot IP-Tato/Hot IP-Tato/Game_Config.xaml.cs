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

namespace Hot_IP_Tato
{
    /// <summary>
    /// Interaction logic for Game_Config.xaml
    /// </summary>
    public partial class Game_Config : Page
    {
        public Game_Config()
        {
            InitializeComponent();
        }
        public void Click_Start(object sender, RoutedEventArgs e)
        {
            //Collect data from config sliders and stuff

            //Send data to Game_Host
            Game_Host game_Host = new Game_Host();
            this.NavigationService.Navigate(game_Host);
        }
    }
}
