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
    /// Interaction logic for MainMenu.xaml
    /// </summary>
    public partial class MainMenu : Page
    {
        public MainMenu()
        {
            InitializeComponent();
        }

        public void Click_Start(object sender, RoutedEventArgs e)
        {
            //Navigate to the start page
            Game_Config game_Config = new Game_Config();
            this.NavigationService.Navigate(game_Config);
        }
        public void Click_Options(object sender, RoutedEventArgs e)
        {
            //Navigate to the options page
            App_Options app_Options = new App_Options();
            this.NavigationService.Navigate(app_Options);
        }
        public void Click_Tests(object sender, RoutedEventArgs e)
        {
            //Navigate to the tests page
            TestPage testPage = new TestPage();
            this.NavigationService.Navigate(testPage);
        }
        public void Click_Quit(object sender, RoutedEventArgs e)
        {
            //Quit the game
            Application.Current.Shutdown();
        }
    }
}
