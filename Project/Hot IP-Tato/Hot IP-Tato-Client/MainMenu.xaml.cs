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

namespace Hot_IP_Tato_Client
{
    /// <summary>
    /// Interaction logic for Page2.xaml
    /// </summary>
    public partial class MainMenu : Page
    {
        public MainMenu()
        {
            InitializeComponent();
        }
        private void btn_Join_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("Join.xaml", UriKind.Relative));
        }
        private void btn_Options_Click(object sender, RoutedEventArgs e)
        {

        }
        private void btn_Quit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
