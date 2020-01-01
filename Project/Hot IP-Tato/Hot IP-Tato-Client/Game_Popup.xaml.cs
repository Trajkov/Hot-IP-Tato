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
    /// Interaction logic for Popup.xaml
    /// </summary>
    public partial class Game_Popup : Window
    {
        public Game_Popup()
        {
            InitializeComponent();
        }
        public void Start()
        {
            this.Show();
        }
    }
    /* * Pseudocode logic for Popup
     * Is launched by a process which listens for an incoming potato. TCP LIstener
     * Has a button which sends the potato to a destination (whether the server or another host).
     * Shows the hostname of the last non-server sender.
     * Displays a chosen picture of a potato (PotatoSkin).
     */
}
