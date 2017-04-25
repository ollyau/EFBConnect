using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ClientDataTest
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();

            if (BeatlesBlog.SimConnect.SimConnect.IsLocalRunning())
            {
                dataHost.ConnectLocal();
                dataClient.ConnectLocal();
            }
            else
            {
                dataHost.Visibility = Visibility.Collapsed;
                dataClient.Visibility = Visibility.Collapsed;
                msgNoLocal.Visibility = Visibility.Visible;
            }
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            dataHost.Disconnect();
            dataClient.Disconnect();

            base.OnClosing(e);
        }
    }
}
