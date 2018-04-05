using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Windows.ApplicationModel;
using Windows.ApplicationModel.AppService;
using Windows.Foundation.Collections;

namespace FullTrust
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private double d1, d2;
        private AppServiceConnection connection = null;

        public MainWindow()
        {
            InitializeComponent();
            InitializeAppServiceConnection();
        }

        /// <summary>
        /// Open connection to UWP app service
        /// </summary>
        private async void InitializeAppServiceConnection()
        {
            connection = new AppServiceConnection();
            connection.AppServiceName = "SampleInteropService";
            connection.PackageFamilyName = Package.Current.Id.FamilyName;
            connection.RequestReceived += Connection_RequestReceived;
            connection.ServiceClosed += Connection_ServiceClosed;

            AppServiceConnectionStatus status = await connection.OpenAsync();
            if (status != AppServiceConnectionStatus.Success)
            {
                // something went wrong ...
                MessageBox.Show(status.ToString());
                this.IsEnabled = false;
            }        
        }

        /// <summary>
        /// Handles the event when the app service connection is closed
        /// </summary>
        private void Connection_ServiceClosed(AppServiceConnection sender, AppServiceClosedEventArgs args)
        {
            // connection to the UWP lost, so we shut down the desktop process
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
            {
                Application.Current.Shutdown();
            })); 
        }

        /// <summary>
        /// Handles the event when the desktop process receives a request from the UWP app
        /// </summary>
        private async void Connection_RequestReceived(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
        {
            string key = args.Request.Message["KEY"] as string;
            string hiveName = key.Substring(0, key.IndexOf('\\'));
            string keyName = key.Substring(key.IndexOf('\\') + 1);
            RegistryHive hive = RegistryHive.ClassesRoot;

            switch (hiveName)
            {
                case "HKLM":
                    hive = RegistryHive.LocalMachine;
                    break;
                case "HKCU":
                    hive = RegistryHive.CurrentUser;
                    break;
                case "HKCR":
                    hive = RegistryHive.ClassesRoot;
                    break;
                case "HKU":
                    hive = RegistryHive.Users;
                    break;
                case "HKCC":
                    hive = RegistryHive.CurrentConfig;
                    break;
            }

            using (RegistryKey regKey = RegistryKey.OpenRemoteBaseKey(hive, "").OpenSubKey(keyName))
            {
                ValueSet response = new ValueSet();
                foreach (string valueName in regKey.GetValueNames())
                {
                    response.Add(valueName, regKey.GetValue(valueName).ToString());
                }
                await args.Request.SendResponseAsync(response);
            }
        }

        /// <summary>
        /// Sends a request to the UWP app
        /// </summary>
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            ValueSet request = new ValueSet();
            request.Add("D1", d1);
            request.Add("D2", d2);
            AppServiceResponse response = await connection.SendMessageAsync(request);
            double result = (double)response.Message["RESULT"];
            tbResult.Text = result.ToString();
        }

        /// <summary>
        /// Determines whether the "equals" button should be enabled
        /// based on input in the text boxes
        /// </summary>
        private void tb_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (double.TryParse(tb1.Text, out d1) && double.TryParse(tb2.Text, out d2))
            {
                btnCalc.IsEnabled = true;
            }
            else
            {
                btnCalc.IsEnabled = false;
            }
        }
    }
}
