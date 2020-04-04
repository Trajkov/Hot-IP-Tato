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
using System.Windows.Shapes;

namespace Hot_IP_Tato_Client
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class WinWindow : Window
    {
        public WinWindow()
        {
            InitializeComponent();

            this.Topmost = true;
        }

        private void BtnPassPotato_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
