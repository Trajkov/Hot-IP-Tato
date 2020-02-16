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
        private TaskCompletionSource<bool> tcs = null;
        public event EventHandler <PopupClosedEventArgs> PopupClosed;
        public Game_Popup(IP_Tato tater)
        {
            InitializeComponent();
            this.DataContext = this;
            string whoSentText = $"{tater.TargetClient.hostname} has sent you a Hot IP_Tato";
            Binding binding = new Binding();
            binding.Source = whoSentText;
            txtWhoSentTater.SetBinding(TextBlock.TextProperty, binding);
        }

        public bool Start()
        {
            this.Show();
            
            return true;
        }
        public bool Subscribe()
        {
            btnPassPotato.Click += new RoutedEventHandler(btnPassPotato_Click);
            
            return true;
        }
        private async void btnPassPotato_Click(object sender, RoutedEventArgs e)
        {
            // Setting the dialog result to true could help with 
            // Tracking player disconnections.
            tcs = new TaskCompletionSource<bool>();
            await tcs.Task;
            this.Close();
        }
        private void btnUpdateSource_Click(object sender, RoutedEventArgs e)
        {
            // BindingExpression binding = txtWindowTitle.GetBindingExpression(TextBox.TextProperty);
            // binding.UpdateSource();
        }
        

        protected virtual void OnPopupClosed(PopupClosedEventArgs e)
        {
            EventHandler <PopupClosedEventArgs> handler = PopupClosed;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void Game_Popup_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MessageBox.Show("Closing Called");

            
            
                string msg = "Would you like to close Hot IP_Tato?";
                MessageBoxResult result =
                    MessageBox.Show(
                        msg,
                        "Data App",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question);
            if (result == MessageBoxResult.No)
            {
                e.Cancel = true;
            }
            else if (result == MessageBoxResult.Yes)
            {
                PopupClosedEventArgs args = new PopupClosedEventArgs();
                args.sending = true;
                OnPopupClosed(args);
            }
             
        }
    }

    public class PopupClosedEventArgs : EventArgs
    {
        public bool sending { get; set; }
    }

    /* * Pseudocode logic for Popup
     * Is launched by a process which listens for an incoming potato. TCP LIstener
     * Has a button which sends the potato to a destination (whether the server or another host).
     * Shows the hostname of the last non-server sender.
     * Displays a chosen picture of a potato (PotatoSkin).
     */
}
