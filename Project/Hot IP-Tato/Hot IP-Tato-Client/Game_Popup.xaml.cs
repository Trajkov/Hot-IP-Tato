using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Threading;
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
        public bool Passing;
        public event EventHandler<PopupClosedEventArgs> PopupClosed;

        public Game_Popup(IP_Tato tater)
        {
            
            InitializeComponent();
            this.DataContext = this;
            string Popup_Title = $"IP_Tato - {tater.TargetClient.hostname}";
            this.Title = Popup_Title;
            
            string whoSentText = $"{tater.LastClient.hostname} has sent you a Hot IP_Tato";
            Binding bind_WhoSentTater = new Binding();
            bind_WhoSentTater.Source = whoSentText;
            txtWhoSentTater.SetBinding(TextBlock.TextProperty, bind_WhoSentTater);

            if (tater.Exploded)
            {
                Binding bind_imgPotato = new Binding();
                bind_imgPotato.Source = "img/exploded-potato-edit.jpg";
                imgPotato.SetBinding(Image.SourceProperty, bind_imgPotato);

                string bind_btnPassPotato = "Darn";
                Binding btnPassPotatoBinding = new Binding();
                btnPassPotatoBinding.Source = bind_btnPassPotato;
                btnPassPotato.SetBinding(Button.ContentProperty, btnPassPotatoBinding);
            }

            // Push the popup to the front.
            this.Topmost = true;
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
        private void btnPassPotato_Click(object sender, RoutedEventArgs e)
        {
            // Setting the dialog result to true could help with 
            // Tracking player disconnections.
            Passing = true;
            this.Close();
        }
        private void btnUpdateSource_Click(object sender, RoutedEventArgs e)
        {
            // BindingExpression binding = txtWindowTitle.GetBindingExpression(TextBox.TextProperty);
            // binding.UpdateSource();
        }


        protected virtual void OnPopupClosed(PopupClosedEventArgs e)
        {
            PopupClosed?.Invoke(this, e);
        }

        private void Game_Popup_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (Passing != true)
            {
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
