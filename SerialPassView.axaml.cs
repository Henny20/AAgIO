using System;
using System.IO.Ports;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using AAgIO.Classes;
using AAgIO.Dialogs;

namespace AAgIO.Views
{
	public partial class SerialPassView : Window
	{
		public SerialPassView()
		{
		    InitializeComponent();
		    Loaded += FormSerialPass_Load;
		}
		
		//private void InitializeComponent()
        //{
        //    AvaloniaXamlLoader.Load(this);
        //}
        
        private readonly MainWindow mf = null;

        public SerialPassView(Window callingForm):this()
        {
            //get copy of the calling main form
            mf = callingForm as MainWindow;
         
        }

        private void FormSerialPass_Load(object sender, System.EventArgs e)
        {
            cboxSerialPassOn.IsChecked = Properties.Settings.Default.setPass_isOn;
            cboxToSerial.IsChecked = Properties.Settings.Default.setNTRIP_sendToSerial;
            cboxToUDP.IsChecked = Properties.Settings.Default.setNTRIP_sendToUDP;
            nudSendToUDPPort.Value = Properties.Settings.Default.setNTRIP_sendToUDPPort;

            cboxRadioPort.Items.Clear();

            foreach (string s in System.IO.Ports.SerialPort.GetPortNames())
            {
                cboxRadioPort.Items.Add(s);
            }

            lblCurrentPort.Text = Properties.Settings.Default.setPort_portNameRadio;
            cboxRadioPort.PlaceholderText = Properties.Settings.Default.setPort_portNameRadio;
            lblCurrentBaud.Text = Properties.Settings.Default.setPort_baudRateRadio;
            cboxBaud.PlaceholderText = Properties.Settings.Default.setPort_baudRateRadio;

            if (mf.spRadio != null && mf.spRadio.IsOpen)
            {
                btnOpenSerial.IsEnabled = false;
                btnCloseSerial.IsEnabled = true;
                cboxRadioPort.IsEnabled = false;
                cboxBaud.IsEnabled = false;
            }
            else
            {
                btnOpenSerial.IsEnabled = true;
                btnCloseSerial.IsEnabled = false;
                cboxRadioPort.IsEnabled = true;
                cboxBaud.IsEnabled = true;
            }

        }

        private void btnSerialOK_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.setPass_isOn = (bool)cboxSerialPassOn.IsChecked;

            if (cboxSerialPassOn.IsChecked ?? false)
            {
                Properties.Settings.Default.setNTRIP_isOn = mf.isNTRIP_RequiredOn = false;
                Properties.Settings.Default.setRadio_isOn = mf.isRadio_RequiredOn = false;
            }

            Properties.Settings.Default.setNTRIP_sendToUDPPort = (int)nudSendToUDPPort.Value;

            Properties.Settings.Default.setNTRIP_sendToSerial = (bool)cboxToSerial.IsChecked;
            Properties.Settings.Default.setNTRIP_sendToUDP = (bool)cboxToUDP.IsChecked;

            mf.isSendToSerial = (bool)cboxToSerial.IsChecked;
            mf.isSendToUDP = (bool)cboxToUDP.IsChecked;

            Properties.Settings.Default.setPort_portNameRadio = cboxRadioPort.PlaceholderText;
            Properties.Settings.Default.setPort_baudRateRadio = cboxBaud.PlaceholderText;
            Properties.Settings.Default.Save();

           //Application.Restart();
            if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
				{
					desktop.Shutdown();
				}
            Environment.Exit(0);
            Close();
        }


        private void btnSerialCancel_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnOpenSerial_Click(object sender, RoutedEventArgs e)
        {
            if (mf.spRadio != null && mf.spRadio.IsOpen)
            {
                mf.spRadio.Close();
                mf.spRadio.Dispose();
                mf.spRadio = null;
            }

            // Setup and open serial port
            mf.spRadio = new SerialPort(cboxRadioPort.PlaceholderText, int.Parse(cboxBaud.PlaceholderText));

            btnOpenSerial.IsEnabled = false;
            btnCloseSerial.IsEnabled = true;

            try
            {
                mf.spRadio.Open();

                lblCurrentPort.Text = Properties.Settings.Default.setPort_portNameRadio;
                lblCurrentBaud.Text = Properties.Settings.Default.setPort_baudRateRadio;

                cboxRadioPort.IsEnabled = false;
                cboxBaud.IsEnabled = false;
            }
            catch (Exception ex)
            {
                var tmb = new TimedMessageBox(3000, "Error opening port", ex.Message);
                tmb.ShowDialog(this);
            }
        }

        private void btnCloseSerial_Click(object sender, RoutedEventArgs e)
        {
            if (mf.spRadio != null && mf.spRadio.IsOpen)
            {
                mf.spRadio.Close();
                mf.spRadio.Dispose();
                mf.spRadio = null;

                btnOpenSerial.IsEnabled = true;
                btnCloseSerial.IsEnabled = false;

                cboxRadioPort.IsEnabled = true;
                cboxBaud.IsEnabled = true;
            }
        }

        private void btnRescan_Click(object sender, RoutedEventArgs e)
        {
            cboxRadioPort.Items.Clear();

            foreach (string s in System.IO.Ports.SerialPort.GetPortNames())
            {
                cboxRadioPort.Items.Add(s);
            }
        }

        private void cboxToSerial_Click(object sender, RoutedEventArgs e)
        {
            if (cboxToUDP.IsChecked ?? false) cboxToUDP.IsChecked = false;
        }

        private void cboxToUDP_Click(object sender, RoutedEventArgs e)
        {
            if (cboxToSerial.IsChecked ?? false) cboxToSerial.IsChecked = false;
        }

        private void cboxSerialPassOn_Click(object sender, RoutedEventArgs e)
        {
        }
	}
}	
