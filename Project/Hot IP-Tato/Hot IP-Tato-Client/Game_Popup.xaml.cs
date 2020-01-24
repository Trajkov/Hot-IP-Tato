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
using Common;

namespace Hot_IP_Tato_Client
{
    /// <summary>
    /// Interaction logic for Popup.xaml
    /// </summary>
    public partial class Game_Popup : Window
    {
        public Game_Popup(IP_Tato tater)
        {
            InitializeComponent();
            this.DataContext = this;
            string whoSentText = $"{tater.TargetClient.hostname} has sent you a Hot IP_Tato";
            Binding binding = new Binding();
            binding.Source = whoSentText;
            txtWhoSentTater.SetBinding(TextBlock.TextProperty, binding);
        }
        public bool subscribe()
        {
            btnPassPotato.Click += new RoutedEventHandler(btnPassPotato_Click);
            return true;
        }
        private void btnPassPotato_Click(object sender, RoutedEventArgs e)
        {
            
        }
        private void btnUpdateSource_Click(object sender, RoutedEventArgs e)
        {
            // BindingExpression binding = txtWindowTitle.GetBindingExpression(TextBox.TextProperty);
            // binding.UpdateSource();
        }
    }
    /* * Pseudocode logic for Popup
     * Is launched by a process which listens for an incoming potato. TCP LIstener
     * Has a button which sends the potato to a destination (whether the server or another host).
     * Shows the hostname of the last non-server sender.
     * Displays a chosen picture of a potato (PotatoSkin).
     */
}
