using System;
using Avalonia.Controls;
using Avalonia;
using Avalonia.Interactivity;
using Avalonia.Controls.Utils;
//using Avalonia.Markup.Xaml;
using MsBox.Avalonia;
using MsBox.Avalonia.Dto;
using MsBox.Avalonia.Enums;

namespace AAgIO.Views
{
	public partial class CommSetGPSView : Window
	{
	
	    private readonly MainWindow mf = null;

		public CommSetGPSView()
		{
		    InitializeComponent();
		    Loaded += FormCommSet_Load;
		    
		    cboxMachineModulePort.SelectionChanged += cboxMachineModulePort_SelectedIndexChanged;
		    cboxModule3Port.SelectionChanged += cboxModule3Port_SelectedIndexChanged;
		    cboxSteerModulePort.SelectionChanged += cboxSteerModule_SelectedIndexChanged;
		    cboxBaud2.SelectionChanged += cboxBaud2_SelectedIndexChanged;
		    cboxPort2.SelectionChanged += cboxPort2_SelectedIndexChanged;
		    cboxRtcmPort.SelectionChanged += cboxRtcmPort_SelectedIndexChanged;
		    cboxRtcmBaud.SelectionChanged += cboxRtcmBaud_SelectedIndexChanged;
		    cboxPort.SelectionChanged += cboxPort_SelectedIndexChanged_1;
		    cboxBaud.SelectionChanged += cboxBaud_SelectedIndexChanged_1;
		    
		    cboxBaud.ItemsSource = new int[]
                {4800, 9600, 19200, 38400, 57600, 115200 };
                
            cboxRtcmBaud.ItemsSource = new string[]
                {"4800", "9600", "19200", "38400", "57600", "115200", "128000", "256000" };
                  
        }
		/***
		 private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
        ***/
        public CommSetGPSView(Window callingForm)
            :this()
        {
             mf = callingForm as MainWindow;
        }
        
        private void FormCommSet_Load(object sender, System.EventArgs e)
        {
            //check if GPS port is open or closed and set buttons accordingly
            if (mf.spGPS.IsOpen)
            {
                cboxBaud.IsEnabled = false;
                cboxPort.IsEnabled = false;
                btnCloseSerial.IsEnabled = true;
                btnOpenSerial.IsEnabled = false;
            }
            else
            {
                cboxBaud.IsEnabled = true;
                cboxPort.IsEnabled = true;
                btnCloseSerial.IsEnabled = false;
                btnOpenSerial.IsEnabled = true;
            }

            if (mf.spGPS2.IsOpen)
            {
                cboxBaud2.IsEnabled = false;
                cboxPort2.IsEnabled = false;
                btnCloseSerial2.IsEnabled = true;
                btnOpenSerial2.IsEnabled = false;
            }
            else
            {
                cboxBaud2.IsEnabled = true;
                cboxPort2.IsEnabled = true;
                btnCloseSerial2.IsEnabled = false;
                btnOpenSerial2.IsEnabled = true;
            }

            if (mf.spRtcm.IsOpen)
            {
                cboxRtcmBaud.IsEnabled = false;
                cboxRtcmPort.IsEnabled = false;
                btnCloseRTCM.IsEnabled = true;
                btnOpenRTCM.IsEnabled = false;
                labelRtcmBaud.Text = mf.spGPS.BaudRate.ToString();
                labelRtcmPort.Text = mf.spGPS.PortName;

            }
            else
            {
                cboxRtcmBaud.IsEnabled = true;
                cboxRtcmPort.IsEnabled = true;
                btnCloseRTCM.IsEnabled = false;
                btnOpenRTCM.IsEnabled = true;
                labelRtcmBaud.Text = "-";
                labelRtcmPort.Text = "-";

            }

            //load the port box with valid port names
            cboxPort.Items.Clear();
            cboxPort2.Items.Clear();
            cboxRtcmPort.Items.Clear();

            foreach (string s in System.IO.Ports.SerialPort.GetPortNames())
            {
                cboxPort.Items.Add(s);
                cboxPort2.Items.Add(s);
                cboxRtcmPort.Items.Add(s);
            }
            cboxPort.Items.Add("/dev/tnt0"); //hennie

            lblCurrentBaud.Text = mf.spGPS.BaudRate.ToString();
            lblCurrentPort.Text = mf.spGPS.PortName;

            lblCurrentBaud2.Text = mf.spGPS2.BaudRate.ToString();
            lblCurrentPort2.Text = mf.spGPS2.PortName;

            labelRtcmBaud.Text = mf.spRtcm.BaudRate.ToString();
            labelRtcmPort.Text = mf.spRtcm.PortName.ToString();

            if (mf.spIMU.IsOpen)
            {
                cboxIMU.IsEnabled = false;
                btnCloseIMU.IsEnabled = true;
                btnOpenIMU.IsEnabled = false;
            }
            else
            {
                cboxIMU.IsEnabled = true;
                btnCloseIMU.IsEnabled = false;
                btnOpenIMU.IsEnabled = true;
            }

            //load the port box with valid port names
            cboxIMU.Items.Clear();
            foreach (string s in System.IO.Ports.SerialPort.GetPortNames())
            {
                cboxIMU.Items.Add(s);
            }

            lblCurrentIMU.Text = mf.spIMU.PortName;

            if (mf.spSteerModule.IsOpen)
            {
                cboxSteerModulePort.IsEnabled = false;
                btnCloseSerialSteerModule.IsEnabled = true;
                btnOpenSerialSteerModule.IsEnabled = false;
            }
            else
            {
                cboxSteerModulePort.IsEnabled = true;
                btnCloseSerialSteerModule.IsEnabled = false;
                btnOpenSerialSteerModule.IsEnabled = true;
            }

            //check if AutoSteer port is open or closed and set buttons accordingly
            if (mf.spMachineModule.IsOpen)
            {
                cboxMachineModulePort.IsEnabled = false;
                btnCloseSerialMachineModule.IsEnabled = true;
                btnOpenSerialMachineModule.IsEnabled = false;
            }
            else
            {
                cboxMachineModulePort.IsEnabled = true;
                btnCloseSerialMachineModule.IsEnabled = false;
                btnOpenSerialMachineModule.IsEnabled = true;
            }

            //if (mf.spModule3.IsOpen)
            //{
            //    cboxModule3Port.IsEnabled = false;
            //    btnCloseSerialModule3.IsEnabled = true;
            //    btnOpenSerialModule3.IsEnabled = false;
            //}
            //else
            //{
            //    cboxModule3Port.IsEnabled = true;
            //    btnCloseSerialModule3.IsEnabled = false;
            //    btnOpenSerialModule3.IsEnabled = true;
            //}

            //load the port box with valid port names
            cboxSteerModulePort.Items.Clear();
            cboxMachineModulePort.Items.Clear();
            //cboxModule3Port.Items.Clear();
            foreach (string s in System.IO.Ports.SerialPort.GetPortNames())
            {
                cboxSteerModulePort.Items.Add(s);
                cboxMachineModulePort.Items.Add(s);
                //cboxModule3Port.Items.Add(s);
            }

            lblCurrentSteerModulePort.Text = mf.spSteerModule.PortName;
            lblCurrentMachineModulePort.Text = mf.spMachineModule.PortName;
            //lblCurrentModule3Port.Text = mf.spModule3.PortName;
        }

        #region PortSettings //----------------------------------------------------------------

        // GPS Serial Port
        private void cboxBaud_SelectedIndexChanged_1(object sender, RoutedEventArgs e)
        {
            mf.spGPS.BaudRate = Convert.ToInt32(cboxBaud.SelectedItem);//Convert.ToInt32(cboxBaud.SelectionBoxItem);
            MainWindow.baudRateGPS = Convert.ToInt32(cboxBaud.SelectedItem);//Convert.ToInt32(cboxBaud.SelectionBoxItem);
        }
        private void cboxBaud2_SelectedIndexChanged(object sender, RoutedEventArgs e)
        {
            mf.spGPS2.BaudRate = Convert.ToInt32(cboxBaud2.SelectionBoxItem);
            MainWindow.baudRateGPS2 = Convert.ToInt32(cboxBaud2.SelectionBoxItem);
        }

        private void cboxPort_SelectedIndexChanged_1(object sender, RoutedEventArgs e)
        {
            if(cboxPort.SelectedItem is not null) {
               mf.spGPS.PortName = cboxPort.SelectedItem.ToString();
               Console.WriteLine("Selectie is:  " + cboxPort.SelectedItem.ToString());
               MainWindow.portNameGPS = cboxPort.SelectedItem.ToString();
            }   
        }

        private void cboxPort2_SelectedIndexChanged(object sender, RoutedEventArgs e)
        {
            if(cboxPort2.SelectedItem is not null) {
               mf.spGPS2.PortName = cboxPort2.SelectedItem.ToString();
               MainWindow.portNameGPS2 = cboxPort2.SelectedItem.ToString();
            }   
        }

        private async void btnOpenSerial_Click(object sender, RoutedEventArgs e)
        {
            mf.OpenGPSPort();
            if (mf.spGPS.IsOpen)
            {
                cboxBaud.IsEnabled = false;
                cboxPort.IsEnabled = false;
                btnCloseSerial.IsEnabled = true;
                btnOpenSerial.IsEnabled = false;
                lblCurrentBaud.Text = mf.spGPS.BaudRate.ToString();
                lblCurrentPort.Text = mf.spGPS.PortName;
            }
            else
            {
                cboxBaud.IsEnabled = true;
                cboxPort.IsEnabled = true;
                btnCloseSerial.IsEnabled = false;
                btnOpenSerial.IsEnabled = true;
                //MessageBox.Show("Unable to connect to Port");
                var messageBoxStandardWindow =
                    MessageBoxManager.GetMessageBoxStandard("Unable to connect to Port","");
                await messageBoxStandardWindow.ShowWindowDialogAsync(this);
            }
        }

        private void btnCloseSerial_Click(object sender, RoutedEventArgs e)
        {
            mf.CloseGPSPort();
            if (mf.spGPS.IsOpen)
            {
                cboxBaud.IsEnabled = false;
                cboxPort.IsEnabled = false;
                btnCloseSerial.IsEnabled = true;
                btnOpenSerial.IsEnabled = false;
            }
            else
            {
                cboxBaud.IsEnabled = true;
                cboxPort.IsEnabled = true;
                btnCloseSerial.IsEnabled = false;
                btnOpenSerial.IsEnabled = true;
            }
        }

        private async void btnOpenRTCM_Click(object sender, RoutedEventArgs e)
        {
            mf.OpenRtcmPort();
            if (mf.spRtcm.IsOpen)
            {
                cboxRtcmBaud.IsEnabled = false;
                cboxRtcmPort.IsEnabled = false;
                btnCloseRTCM.IsEnabled = true;
                btnOpenRTCM.IsEnabled = false;
                labelRtcmBaud.Text = mf.spRtcm.BaudRate.ToString();
                labelRtcmPort.Text = mf.spRtcm.PortName;
            }
            else
            {
                cboxRtcmBaud.IsEnabled = true;
                cboxRtcmPort.IsEnabled = true;
                btnCloseRTCM.IsEnabled = false;
                btnOpenRTCM.IsEnabled = true;
                //MessageBox.Show("Unable to connect to Port");
                var messageBoxStandardWindow =
                    MessageBoxManager.GetMessageBoxStandard("Unable to connect to Port", "");
                await messageBoxStandardWindow.ShowWindowDialogAsync(this);
            }
        }

        private void btnCloseRTCM_Click(object sender, RoutedEventArgs e)
        {
            mf.CloseRtcmPort();

            if (mf.spRtcm.IsOpen)
            {
                cboxRtcmBaud.IsEnabled = false;
                cboxRtcmPort.IsEnabled = false;
                btnCloseRTCM.IsEnabled = true;
                btnOpenRTCM.IsEnabled = false;
            }
            else
            {
                cboxRtcmBaud.IsEnabled = true;
                cboxRtcmPort.IsEnabled = true;
                btnCloseRTCM.IsEnabled = false;
                btnOpenRTCM.IsEnabled = true;
            }
        }

        private void btnOpenSerial2_Click(object sender, RoutedEventArgs e)
        {
            mf.OpenGPS2Port();
            if (mf.spGPS2.IsOpen)
            {
                cboxBaud2.IsEnabled = false;
                cboxPort2.IsEnabled = false;
                btnCloseSerial2.IsEnabled = true;
                btnOpenSerial2.IsEnabled = false;
                lblCurrentBaud2.Text = mf.spGPS.BaudRate.ToString();
                lblCurrentPort2.Text = mf.spGPS.PortName;
            }
            else
            {
                cboxBaud2.IsEnabled = true;
                cboxPort2.IsEnabled = true;
                btnCloseSerial2.IsEnabled = false;
                btnOpenSerial2.IsEnabled = true;
                //MessageBox.Show("Unable to connect to Port"); TODO
            }
        }

        private void btnCloseSerial2_Click(object sender, RoutedEventArgs e)
        {
            mf.CloseGPS2Port();
            if (mf.spGPS2.IsOpen)
            {
                cboxBaud2.IsEnabled = false;
                cboxPort2.IsEnabled = false;
                btnCloseSerial2.IsEnabled = true;
                btnOpenSerial2.IsEnabled = false;
            }
            else
            {
                cboxBaud2.IsEnabled = true;
                cboxPort2.IsEnabled = true;
                btnCloseSerial2.IsEnabled = false;
                btnOpenSerial2.IsEnabled = true;
            }
        }

        #endregion PortSettings //----------------------------------------------------------------

        private void timer1_Tick(object sender, RoutedEventArgs e)
        {
            //GPS phrase
            textBoxRcv.Text = mf.recvGPSSentence;
            textBoxRcv2.Text = mf.recvGPS2Sentence;
            lblSteer.Text = mf.spSteerModule.PortName;
            lblGPS.Text = mf.spGPS.PortName;
            lblIMU.Text = mf.spIMU.PortName;
            lblMachine.Text = mf.spMachineModule.PortName;

            lblFromGPS.Text = mf.traffic.cntrGPSIn == 0 ? "--" : (mf.traffic.cntrGPSIn).ToString();

            lblFromSteerModule.Text = mf.traffic.cntrSteerIn == 0 ? "--" : (mf.traffic.cntrSteerIn).ToString();

            lblFromMachineModule.Text = mf.traffic.cntrMachineIn == 0 ? "--" : (mf.traffic.cntrMachineIn).ToString();
        }

        private void btnSerialOK_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnRescan_Click(object sender, RoutedEventArgs e)
        {
            cboxPort.Items.Clear();
            cboxRtcmPort.Items.Clear();
            cboxPort2.Items.Clear();
            cboxIMU.Items.Clear();
            cboxSteerModulePort.Items.Clear();
            cboxMachineModulePort.Items.Clear();
            cboxModule3Port.Items.Clear();

            foreach (string s in System.IO.Ports.SerialPort.GetPortNames())
            {
                cboxPort.Items.Add(s);
                cboxPort2.Items.Add(s);
                cboxIMU.Items.Add(s);
                cboxRtcmPort.Items.Add(s);
                cboxSteerModulePort.Items.Add(s);
                cboxMachineModulePort.Items.Add(s);
                cboxModule3Port.Items.Add(s);
            }
        }

        private void btnClrGPS_Click(object sender, RoutedEventArgs e)
        {
            mf.CloseGPSPort();
            MainWindow.portNameGPS = "GPS 1";
            Properties.Settings.Default.setPort_portNameGPS = MainWindow.portNameGPS;
            Properties.Settings.Default.Save();
        }

        private void btnOpenIMU_Click(object sender, RoutedEventArgs e)
        {
            mf.OpenIMUPort();
            if (mf.spIMU.IsOpen)
            {
                cboxIMU.IsEnabled = false;
                btnCloseIMU.IsEnabled = true;
                btnOpenIMU.IsEnabled = false;
                lblCurrentIMU.Text = mf.spIMU.PortName;
            }
            else
            {
                cboxIMU.IsEnabled = true;
                btnCloseIMU.IsEnabled = false;
                btnOpenIMU.IsEnabled = true;
            }
        }

        private void btnCloseIMU_Click(object sender, RoutedEventArgs e)
        {
            mf.CloseIMUPort();
            if (mf.spIMU.IsOpen)
            {
                cboxIMU.IsEnabled = false;
                btnCloseIMU.IsEnabled = true;
                btnOpenIMU.IsEnabled = false;
            }
            else
            {
                cboxIMU.IsEnabled = true;
                btnCloseIMU.IsEnabled = false;
                btnOpenIMU.IsEnabled = true;
            }
        }

        private void cboxIMU_SelectedIndexChanged(object sender, RoutedEventArgs e)
        {
            mf.spIMU.PortName = cboxIMU.SelectedItem.ToString();
            MainWindow.portNameIMU = cboxIMU.SelectedItem.ToString();
            lblCurrentIMU.Text = cboxIMU.SelectedItem.ToString();
        }

        private void btnOpenSerialSteerModule_Click(object sender, RoutedEventArgs e)
        {
            mf.OpenSteerModulePort();
            if (mf.spSteerModule.IsOpen)
            {
                cboxSteerModulePort.IsEnabled = false;
                btnCloseSerialSteerModule.IsEnabled = true;
                btnOpenSerialSteerModule.IsEnabled = false;
                lblCurrentSteerModulePort.Text = mf.spSteerModule.PortName;
            }
            else
            {
                cboxSteerModulePort.IsEnabled = true;
                btnCloseSerialSteerModule.IsEnabled = false;
                btnOpenSerialSteerModule.IsEnabled = true;
            }
        }

        private void btnCloseSerialSteerModule_Click(object sender, RoutedEventArgs e)
        {
            mf.CloseSteerModulePort();
            if (mf.spSteerModule.IsOpen)
            {
                cboxSteerModulePort.IsEnabled = false;
                btnCloseSerialSteerModule.IsEnabled = true;
                btnOpenSerialSteerModule.IsEnabled = false;
            }
            else
            {
                cboxSteerModulePort.IsEnabled = true;
                btnCloseSerialSteerModule.IsEnabled = false;
                btnOpenSerialSteerModule.IsEnabled = true;
            }
        }

        private void cboxSteerModule_SelectedIndexChanged(object sender, RoutedEventArgs e)
        {
            if(cboxSteerModulePort.SelectedItem is not null) {
               mf.spSteerModule.PortName = cboxSteerModulePort.SelectedItem.ToString();
               MainWindow.portNameSteerModule = cboxSteerModulePort.SelectedItem.ToString();
               lblCurrentSteerModulePort.Text = cboxSteerModulePort.SelectedItem.ToString();
            }   
        }

        private void cboxMachineModulePort_SelectedIndexChanged(object sender, RoutedEventArgs e)
        {
            if(cboxMachineModulePort.SelectedItem is not null) {
               mf.spMachineModule.PortName = cboxMachineModulePort.SelectedItem.ToString();
               MainWindow.portNameMachineModule = cboxMachineModulePort.SelectedItem.ToString();
               lblCurrentMachineModulePort.Text = cboxMachineModulePort.SelectedItem.ToString();
            }   
        }

        private void btnOpenSerialMachineModule_Click(object sender, RoutedEventArgs e)
        {
            mf.OpenMachineModulePort();
            if (mf.spMachineModule.IsOpen)
            {
                cboxMachineModulePort.IsEnabled = false;
                btnCloseSerialMachineModule.IsEnabled = true;
                btnOpenSerialMachineModule.IsEnabled = false;
                lblCurrentMachineModulePort.Text = mf.spMachineModule.PortName;
            }
            else
            {
                cboxMachineModulePort.IsEnabled = true;
                btnCloseSerialMachineModule.IsEnabled = false;
                btnOpenSerialMachineModule.IsEnabled = true;
            }
        }

        private void btnCloseSerialMachineModule_Click(object sender, RoutedEventArgs e)
        {
            mf.CloseMachineModulePort();
            if (mf.spMachineModule.IsOpen)
            {
                cboxMachineModulePort.IsEnabled = false;
                btnCloseSerialMachineModule.IsEnabled = true;
                btnOpenSerialMachineModule.IsEnabled = false;
            }
            else
            {
                cboxMachineModulePort.IsEnabled = true;
                btnCloseSerialMachineModule.IsEnabled = false;
                btnOpenSerialMachineModule.IsEnabled = true;
            }
        }

        private void cboxModule3Port_SelectedIndexChanged(object sender, RoutedEventArgs e)
        {
            //mf.spModule3.PortName = cboxModule3Port.Text;
            //FormLoop.portNameModule3 = cboxModule3Port.Text;
            //lblCurrentModule3Port.Text = cboxModule3Port.Text;
        }

        private void btnOpenSerialModule3_Click(object sender, RoutedEventArgs e)
        {
            //mf.OpenModule3Port();
            //if (mf.spModule3.IsOpen)
            //{
            //    cboxModule3Port.IsEnabled = false;
            //    btnCloseSerialModule3.IsEnabled = true;
            //    btnOpenSerialModule3.IsEnabled = false;
            //    lblCurrentModule3Port.Text = mf.spModule3.PortName;
            //}
            //else
            //{
            //    cboxModule3Port.IsEnabled = true;
            //    btnCloseSerialModule3.IsEnabled = false;
            //    btnOpenSerialModule3.IsEnabled = true;
            //}
        }

        private void btnCloseSerialModule3_Click(object sender, RoutedEventArgs e)
        {
            //mf.CloseModule3Port();
            //if (mf.spModule3.IsOpen)
            //{
            //    cboxModule3Port.IsEnabled = false;
            //    btnCloseSerialModule3.IsEnabled = true;
            //    btnOpenSerialModule3.IsEnabled = false;
            //}
            //else
            //{
            //    cboxModule3Port.IsEnabled = true;
            //    btnCloseSerialModule3.IsEnabled = false;
            //    btnOpenSerialModule3.IsEnabled = true;
            //}
        }

        private void cboxRtcmPort_SelectedIndexChanged(object sender, SelectionChangedEventArgs e)
        {
           if(cboxRtcmPort.SelectedItem is not null) {
              mf.spRtcm.PortName = cboxRtcmPort.SelectedItem.ToString();
              MainWindow.portNameRtcm = cboxRtcmPort.SelectedItem.ToString();
           } 
        }

        private void cboxRtcmBaud_SelectedIndexChanged(object sender, SelectionChangedEventArgs e)
        {
            Console.WriteLine("gedoe is:   " + cboxRtcmBaud.SelectedValue.ToString());
            mf.spRtcm.BaudRate = Convert.ToInt32(cboxRtcmBaud.SelectedValue.ToString());      //(cboxRtcmBaud.SelectionBoxItem);
            MainWindow.baudRateRtcm = Convert.ToInt32(cboxRtcmBaud.SelectedValue.ToString()); //(cboxRtcmBaud.SelectionBoxItem);
        }
        
        
	}
}	
