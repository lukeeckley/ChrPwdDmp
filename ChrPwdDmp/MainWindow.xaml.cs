using Microsoft.Win32;
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

namespace ChrPwdDmp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// 
    /// Changed the output to the console for testing purposes:
    /// Right click on the project, "Properties", "Application" tab, change "Output Type" to "Console Application", and then it will also have a console.
    /// Change this back to "Windows Application" when it is released
    /// 
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            FindLoginData();
            ShowBrowsers();
        }

        private void FindLoginData()
        {
            string userProfile = Environment.GetEnvironmentVariable("userprofile");
            Console.WriteLine("%USERPROFILE% = " + userProfile);

        }

        // This doesn't show if the browser was installed by someone else :/
        private void ShowBrowsers()
        {
            RegistryKey browserKeys;
            //on 64bit the browsers are in a different location
            browserKeys = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\Clients\StartMenuInternet");
            if (browserKeys == null)
                browserKeys = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Clients\StartMenuInternet");

            string[] lines = browserKeys.GetSubKeyNames();

            foreach (string line in lines)
                Console.WriteLine(line);
        }


    }
}
