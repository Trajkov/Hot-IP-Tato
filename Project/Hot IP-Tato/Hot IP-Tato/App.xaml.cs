using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Hot_IP_Tato.CS_Scripts.Utilities;

namespace Hot_IP_Tato
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        TextBoxOutputter outputter;
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            
            //Create the startup window
            MainWindow main_window = new MainWindow();
            
            if (e.Args.Length > 0)
            {
                if (e.Args[0].Equals("debug"))
                {
                    ConsoleWindow window_console = new ConsoleWindow();
                 
                    outputter = new TextBoxOutputter(window_console.test_console);
                    Console.SetOut(outputter);
                    window_console.Show();
                    Console.WriteLine("Console Started");
                    Console.WriteLine("Console is working????");
                     
                }
            }
            
            //Do stuff
            // Navigate to the main menu.
            MainMenu main_menu = new MainMenu();
            main_window.Navigate(main_menu);
            
            // Show the window
            main_window.Show();
            
        }
    }
}
