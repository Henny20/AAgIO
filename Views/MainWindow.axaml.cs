using System;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Linq;
﻿using AAgIO.Properties;
using AAgIO.Dialogs;
using AAgIO.Enums;
using Avalonia;
using Avalonia.Threading;
using Avalonia.Interactivity;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Media.Immutable;
using MsBox.Avalonia;
using MsBox.Avalonia.Dto;
using MsBox.Avalonia.Enums;
using HidSharp;


namespace AAgIO.Views
{
	public partial class MainWindow : Window
	{
	    private static MainWindow Instance = null;
	   //  public static MainWindow Instance { get; private set; }
	    
	    private DispatcherTimer oneSecondLoopTimer = new DispatcherTimer();
	    private DispatcherTimer ntripMeterTimer = new DispatcherTimer();
	    
	    /***** 
	    [System.Runtime.InteropServices.DllImport("User32.dll")]
        private static extern bool SetForegroundWindow(IntPtr handle);

        [System.Runtime.InteropServices.DllImport("User32.dll")]
        private static extern bool ShowWindow(IntPtr hWind, int nCmdShow);

        [System.Runtime.InteropServices.DllImport("User32.dll")]
        public static extern bool IsIconic(IntPtr handle);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, int dwExtraInfo);
        *******/
        
        //key event to restore window
        private const int ALT = 0xA4;
        private const int EXTENDEDKEY = 0x1;
        private const int KEYUP = 0x2;

        //Stringbuilder
        public StringBuilder logNMEASentence = new StringBuilder();
        private StringBuilder sbRTCM = new StringBuilder();

        public bool isKeyboardOn = true;

        public bool isSendToSerial = true, isSendToUDP = false;

        public bool isGPSSentencesOn = false, isSendNMEAToUDP;

        //timer variables
        public double secondsSinceStart, twoSecondTimer, tenSecondTimer, threeMinuteTimer;

        public string lastSentence;

        public bool isPluginUsed;

        //usually 256 - send ntrip to serial in chunks
        public int packetSizeNTRIP;

        public bool lastHelloGPS, lastHelloAutoSteer, lastHelloMachine, lastHelloIMU;
        public bool isConnectedIMU, isConnectedSteer, isConnectedMachine;

        //is the fly out displayed
        public bool isViewAdvanced = false;
        public bool isLogNMEA;

        //used to hide the window and not update text fields and most counters
        public bool isAppInFocus = true, isLostFocus;
        public int focusSkipCounter = 310;

        //The base directory where Drive will be stored and fields and vehicles branch from
        public string baseDirectory;

        //current directory of Comm storage
        public string commDirectory, commFileName = "";
        private int smallView = 480;
        
		public MainWindow()
		{
		    InitializeComponent();
		    Instance = this;
		    Loaded += FormLoop_Load;
		    Closing += FormLoop_FormClosing;
		    oneSecondLoopTimer.Interval = TimeSpan.FromMilliseconds(4000);
            oneSecondLoopTimer.IsEnabled = true;
            oneSecondLoopTimer.Tick += oneSecondLoopTimer_Tick;
            oneSecondLoopTimer.Start();
            
            ntripMeterTimer.Interval = TimeSpan.FromMilliseconds(50);
            ntripMeterTimer.IsEnabled = true;
            ntripMeterTimer.Tick +=  ntripMeterTimer_Tick;
            ntripMeterTimer.Start();
		}
			
		public static MainWindow getInstance() {
            return Instance;
        }
		
		 //First run
        private void FormLoop_Load(object sender, System.EventArgs e)
        {
            // Deals with slash vs backslash
            if (Settings.Default.setF_workingDirectory == "Default")
	            baseDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "AgOpenGPS");
	        else baseDirectory =  Path.Combine(Settings.Default.setF_workingDirectory, "AgOpenGPS");

	        //get the fields directory, if not exist, create
	        commDirectory = Path.Combine(baseDirectory, "AgIO" + Path.DirectorySeparatorChar);
	        Console.WriteLine("dir is" + commDirectory);
     
            string dir = Path.GetDirectoryName(commDirectory);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir)) { Directory.CreateDirectory(dir); }

            if (Settings.Default.setUDP_isOn)
            {
                LoadUDPNetwork();
            }
            else
            {
                label2.IsVisible = false;
                label3.IsVisible = false;
                label4.IsVisible = false;
                label9.IsVisible = false;

                lblSteerAngle.IsVisible = false;
                lblWASCounts.IsVisible = false;
                lblSwitchStatus.IsVisible = false;
                lblWorkSwitchStatus.IsVisible = false;

                label10.IsVisible = false;
                label12.IsVisible = false;
                lbl1To8.IsVisible = false;
                lbl9To16.IsVisible = false;

                btnRelayTest.IsVisible = false;

                btnUDP.Background = new ImmutableSolidColorBrush(Colors.Gainsboro);
                lblIP.Content = "Off";
            }

            //smallView view
            //this.Width = smallView; Tja

            LoadLoopback();

            isSendNMEAToUDP = Properties.Settings.Default.setUDP_isSendNMEAToUDP;
            isPluginUsed = Properties.Settings.Default.setUDP_isUsePluginApp;

            packetSizeNTRIP = Properties.Settings.Default.setNTRIP_packetSize;

            isSendToSerial = Settings.Default.setNTRIP_sendToSerial;
            isSendToUDP = Settings.Default.setNTRIP_sendToUDP;

            //lblMount.Text = Properties.Settings.Default.setNTRIP_mount;

            lblGPS1Comm.Text = "---";
            lblIMUComm.Text = "---";
            lblMod1Comm.Text = "---";
            lblMod2Comm.Text = "---";
            
            //set baud and port from last time run
            baudRateGPS = Settings.Default.setPort_baudRateGPS;
            portNameGPS = Settings.Default.setPort_portNameGPS;
            wasGPSConnectedLastRun = Settings.Default.setPort_wasGPSConnected;
            if (wasGPSConnectedLastRun)
            {
                OpenGPSPort();
                if (spGPS.IsOpen) lblGPS1Comm.Text = portNameGPS;
            }

            // set baud and port for rtcm from last time run
            baudRateRtcm = Settings.Default.setPort_baudRateRtcm;
            portNameRtcm = Settings.Default.setPort_portNameRtcm;
            wasRtcmConnectedLastRun = Settings.Default.setPort_wasRtcmConnected;

            if (wasRtcmConnectedLastRun)
            {
                OpenRtcmPort();
            }

            //Open IMU
            portNameIMU = Settings.Default.setPort_portNameIMU;
            wasIMUConnectedLastRun = Settings.Default.setPort_wasIMUConnected;
            if (wasIMUConnectedLastRun)
            {
                OpenIMUPort();
                if (spIMU.IsOpen) lblIMUComm.Text = portNameIMU;
            }


            //same for SteerModule port
            portNameSteerModule = Settings.Default.setPort_portNameSteer;
            wasSteerModuleConnectedLastRun = Settings.Default.setPort_wasSteerModuleConnected;
            if (wasSteerModuleConnectedLastRun)
            {
                OpenSteerModulePort();
                if (spSteerModule.IsOpen) lblMod1Comm.Text = portNameSteerModule;
            }

            //same for MachineModule port
            portNameMachineModule = Settings.Default.setPort_portNameMachine;
            wasMachineModuleConnectedLastRun = Settings.Default.setPort_wasMachineModuleConnected;
            if (wasMachineModuleConnectedLastRun)
            {
                OpenMachineModulePort();
                if (spMachineModule.IsOpen) lblMod2Comm.Text = portNameMachineModule;
            }

            ConfigureNTRIP();

            string[] ports = System.IO.Ports.SerialPort.GetPortNames();

            if (ports.Length == 0)
            {
                lblSerialPorts.Text = "None";
            }
            else
            {
                for (int i = 0; i < ports.Length; i++)
                {
                    lblSerialPorts.Text = ports[i] + "\r\n";
                }
            }

            isConnectedIMU = (bool)cboxIsIMUModule.IsChecked == Properties.Settings.Default.setMod_isIMUConnected;
            isConnectedSteer = cboxIsSteerModule.IsChecked ?? false == Properties.Settings.Default.setMod_isSteerConnected;
            isConnectedMachine = cboxIsMachineModule.IsChecked ?? false == Properties.Settings.Default.setMod_isMachineConnected;
            
            SetModulesOnOff();

            oneSecondLoopTimer.IsEnabled = true;
            /**** TODO
            pictureBox1.IsVisible = true;
            pictureBox1.BringToFront();
            pictureBox1.Width = 430;
            pictureBox1.Height = 500;
            pictureBox1.Left = 0;
            pictureBox1.Top = 0;
            //pictureBox1.Dock = DockStyle.Fill;
            ******/

            //On or off the module rows
            SetModulesOnOff();
        }

        public void SetModulesOnOff()
        {
            if (isConnectedIMU)
            {
                btnIMU.IsVisible = true; 
                lblIMUComm.IsVisible = true;
                lblFromMU.IsVisible = true;
               // cboxIsIMUModule.Content = Properties.Resources.Cancel64;
            }
            else
            {
                btnIMU.IsVisible = false;
                lblIMUComm.IsVisible = false;
                lblFromMU.IsVisible = false;
                //cboxIsIMUModule.BackgroundImage = Properties.Resources.AddNew; //TODO
            }

            if (isConnectedMachine)
            {
                btnMachine.IsVisible = true;
                lblFromMachine.IsVisible = true;
                lblToMachine.IsVisible = true;
                lblMod2Comm.IsVisible = true;
                //cboxIsMachineModule.Content = Properties.Resources.Cancel64; //TODO
            }
            else
            {
                btnMachine.IsVisible = false;
                lblFromMachine.IsVisible = false;
                lblToMachine.IsVisible = false;
                lblMod2Comm.IsVisible = false;
               // cboxIsMachineModule.BackgroundImage = Properties.Resources.AddNew; TODO
            }

            if (isConnectedSteer)
            {
                btnSteer.IsVisible = true;
                lblFromSteer.IsVisible = true;
                lblToSteer.IsVisible = true; 
                lblMod1Comm.IsVisible = true;
                //cboxIsSteerModule.BackgroundImage = Properties.Resources.Cancel64; TODO
            }
            else
            {
                btnSteer.IsVisible = false;
                lblFromSteer.IsVisible = false;
                lblToSteer.IsVisible = false;
                lblMod1Comm.IsVisible = false;
                //cboxIsSteerModule.Content = Properties.Resources.AddNew;
            }

            Properties.Settings.Default.setMod_isIMUConnected = isConnectedIMU;
            Properties.Settings.Default.setMod_isSteerConnected = isConnectedSteer;
            Properties.Settings.Default.setMod_isMachineConnected = isConnectedMachine;

            Properties.Settings.Default.Save();
        }

        private void FormLoop_FormClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Settings.Default.setPort_wasGPSConnected = wasGPSConnectedLastRun;
            Settings.Default.setPort_wasIMUConnected = wasIMUConnectedLastRun;
            Settings.Default.setPort_wasSteerModuleConnected = wasSteerModuleConnectedLastRun;
            Settings.Default.setPort_wasMachineModuleConnected = wasMachineModuleConnectedLastRun;
            Settings.Default.setPort_wasRtcmConnected = wasRtcmConnectedLastRun;

            Settings.Default.Save();

            if (loopBackSocket != null)
            {
                try
                {
                    loopBackSocket.Shutdown(SocketShutdown.Both);
                }
                finally { loopBackSocket.Close(); }
            }

            if (UDPSocket != null)
            {
                try
                {
                    UDPSocket.Shutdown(SocketShutdown.Both);
                }
                finally { UDPSocket.Close(); }
            }
        }

        private void oneSecondLoopTimer_Tick(object sender, System.EventArgs e)
        {
            if (oneSecondLoopTimer.Interval > TimeSpan.FromMilliseconds(1200))
            {
                //Controls.Remove(pictureBox1); TODO
                //pictureBox1.Dispose();
                oneSecondLoopTimer.Interval = TimeSpan.FromMilliseconds(1000);
                this.Width = smallView;
                this.Height = 550;
                return;
            }

            secondsSinceStart = (DateTime.Now - Process.GetCurrentProcess().StartTime).TotalSeconds;

            //Hello Alarm logic
            DoHelloAlarmLogic();

            DoTraffic();

            if (focusSkipCounter != 0)
            {
                lblCurentLon.Text = longitude.ToString("N7");
                lblCurrentLat.Text = latitude.ToString("N7");
            }

            //do all the NTRIP routines
            DoNTRIPSecondRoutine();

            //send a hello to modules
            SendUDPMessage(helloFromAgIO, epModule);
            helloFromAgIO[7] = 0;

            #region Sleep

            //is this the active window
           // isAppInFocus = FormLoop.ActiveForm != null;
           var desktop = Application.Current.ApplicationLifetime 
                as IClassicDesktopStyleApplicationLifetime;
           isAppInFocus =  desktop.MainWindow.IsActive != null; //TODO
         
            //start counting down to minimize
            if (!isAppInFocus && !isLostFocus)
            {
                focusSkipCounter = 310;
                isLostFocus = true;
            }

            // Is active window again
            if (isAppInFocus && isLostFocus)
            {
                isLostFocus = false;
                focusSkipCounter = int.MaxValue;
            }

            if (isLostFocus && focusSkipCounter !=0)
            {
                if (focusSkipCounter == 1)
                {
                    WindowState = WindowState.Minimized;
                }

                focusSkipCounter-- ;
            }

            #endregion

            //every couple or so seconds
            if ((secondsSinceStart - twoSecondTimer) > 2)
            {
                TwoSecondLoop();
                twoSecondTimer = secondsSinceStart;
            }

            //every 10 seconds
            if ((secondsSinceStart - tenSecondTimer) > 9.5)
            {
                TenSecondLoop();
                tenSecondTimer = secondsSinceStart;
            }

            //3 minute egg timer
            if ((secondsSinceStart - threeMinuteTimer) > 180)
            {
                ThreeMinuteLoop();
                threeMinuteTimer = secondsSinceStart;
            }

            // 1 Second Loop Part2 
            if (isViewAdvanced)
            {
                if (isNTRIP_RequiredOn)
                {
                    sbRTCM.Append(".");
                    lblMessages.Content = sbRTCM.ToString();
                }
                btnResetTimer.Content = ((int)(180 - (secondsSinceStart - threeMinuteTimer))).ToString();
            }
        }

        private void TwoSecondLoop()
        {
            if (isLogNMEA)
            {
                using (StreamWriter writer = new StreamWriter("zAgIO_log.txt", true))
                {
                    writer.Write(logNMEASentence.ToString());
                }
                logNMEASentence.Clear();
            }

            if (focusSkipCounter < 310) lblSkipCounter.Text = focusSkipCounter.ToString();
            else lblSkipCounter.Text = "On";

        }

        private void TenSecondLoop()
        {
            if (focusSkipCounter != 0 && WindowState == WindowState.Minimized)
            {
                focusSkipCounter = 0;
                isLostFocus = true;
            }

            if (focusSkipCounter != 0)
            {
                if (isViewAdvanced && isNTRIP_RequiredOn)
                {
                    try
                    {
                        //add the uniques messages to all the new ones
                        foreach (var item in aList)
                        {
                                rList.Add(item);
                        }

                        //sort and group using Linq
                        sbRTCM.Clear();

                        var g = rList.GroupBy(i => i)
                            .OrderBy(grp => grp.Key);
                        int count = 0;
                        aList.Clear();

                        //Create the text box of unique message numbers
                        foreach (var grp in g)
                        {
                            aList.Add(grp.Key);
                            sbRTCM.AppendLine(grp.Key + " - " + (grp.Count()-1));
                            count++;
                        }

                        rList?.Clear();

                        //too many messages or trash
                        if (count > 25)
                        {
                            aList?.Clear();
                            sbRTCM.Clear();
                            sbRTCM.Append("Reset..");
                        }

                        lblMessagesFound.Text = count.ToString();
                    }

                    catch
                    {
                        sbRTCM.Clear();
                        sbRTCM.Append("Error");
                    }
                }

                #region Serial update

                if (wasIMUConnectedLastRun)
                {
                    if (!spIMU.IsOpen)
                    {
                        byte[] imuClose = new byte[] { 0x80, 0x81, 0x7C, 0xD4, 2, 1, 0, 83 };

                        //tell AOG IMU is disconnected
                        SendToLoopBackMessageAOG(imuClose);
                        wasIMUConnectedLastRun = false;
                        lblIMUComm.Text = "---";
                    }
                }

                if (wasGPSConnectedLastRun)
                {
                    if (!spGPS.IsOpen)
                    {
                        wasGPSConnectedLastRun = false;
                        lblGPS1Comm.Text = "---";
                    }
                }

                if (wasSteerModuleConnectedLastRun)
                {
                    if (!spSteerModule.IsOpen)
                    {
                        wasSteerModuleConnectedLastRun = false;
                        lblMod1Comm.Text = "---";
                    }
                }

                if (wasMachineModuleConnectedLastRun)
                {
                    if (!spMachineModule.IsOpen)
                    {
                        wasMachineModuleConnectedLastRun = false;
                        lblMod2Comm.Text = "---";
                    }
                }

                #endregion
            }
        }

        private void btnSlide_Click(object sender, RoutedEventArgs e)
        {
            if (this.Width < 600)
            {
                //this.Width = 700;
                this.Width = 820;
                isViewAdvanced = true;
                btnSlide_Image.Source = new Bitmap("Assets/ArrowGrnLeft.png"); //Properties.Resources.ArrowGrnLeft; 
                sbRTCM.Clear();
                lblMessages.Content = "Reading...";
                threeMinuteTimer = secondsSinceStart;
                lblMessagesFound.Text = "-";
                aList.Clear();  
                rList.Clear();

            }
            else
            {
                this.Width = smallView;
                isViewAdvanced = false;
               // btnSlide.Content = Properties.Resources.ArrowGrnRight; 
                btnSlide_Image.Source = new Bitmap("Assets/ArrowGrnRight.png");
                aList.Clear();
                rList.Clear();
                lblMessages.Content = "Reading...";
                lblMessagesFound.Text = "-";
                aList.Clear();
                rList.Clear();
            }
        }

        private void ThreeMinuteLoop()
        {
            if (isViewAdvanced)
            {
                btnSlide.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            }
        }

        private void DoHelloAlarmLogic()
        {
            bool currentHello;

            if (isConnectedMachine)
            {
                currentHello = traffic.helloFromMachine < 3;

                if (currentHello != lastHelloMachine)
                {
                    if (currentHello) btnMachine.Background = new ImmutableSolidColorBrush(Colors.LimeGreen);
                    else btnMachine.Background = new ImmutableSolidColorBrush(Colors.Red);
                    lastHelloMachine = currentHello;
                    ShowAgIO();
                }
            }

            if (isConnectedSteer)
            {
                currentHello = traffic.helloFromAutoSteer < 3;

                if (currentHello != lastHelloAutoSteer)
                {
                    if (currentHello) btnSteer.Background = new ImmutableSolidColorBrush(Colors.LimeGreen);
                    else btnSteer.Background = new ImmutableSolidColorBrush(Colors.Red);
                    lastHelloAutoSteer = currentHello;
                    ShowAgIO();
                }
            }

            if (isConnectedIMU)
            {
                currentHello = traffic.helloFromIMU < 3;

                if (currentHello != lastHelloIMU)
                {
                    if (currentHello) btnIMU.Background = new ImmutableSolidColorBrush(Colors.LimeGreen);
                    else btnIMU.Background = new ImmutableSolidColorBrush(Colors.Red);
                    lastHelloIMU = currentHello;
                    ShowAgIO();
                }
            }

            currentHello = traffic.cntrGPSOut != 0;

            if (currentHello != lastHelloGPS)
            {
                if (currentHello) btnGPS.Background =  new SolidColorBrush(Colors.LimeGreen);
                else btnGPS.Background = new ImmutableSolidColorBrush(Colors.Red);
                lastHelloGPS = currentHello;
                ShowAgIO();
            }
        }

        private void FormLoop_Resize(object sender, RoutedEventArgs e)
        {
            if(this.WindowState == WindowState.Minimized)
            {
                if (isViewAdvanced) btnSlide.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                isLostFocus = true;
                focusSkipCounter = 0;
            }
        }

        private void ShowAgIO()
        {
           /** vragen om ellende
            using var process = Process.Start(
              new ProcessStartInfo
              {
                 FileName = "AAgIO",
                 //ArgumentList = { "hello world" }
              });
            process.WaitForExit();
            ********/
        
        
            Process[] processName = Process.GetProcessesByName("AgIO");
            
            if (processName.Length != 0)
            {
               /********** not Linux
                // Guard: check if window already has focus.
                if (processName[0].MainWindowHandle == GetForegroundWindow()) return;

                // Show window maximized.
                ShowWindow(processName[0].MainWindowHandle, 9);

                // Simulate an "ALT" key press.
                keybd_event((byte)ALT, 0x45, EXTENDEDKEY | 0, 0);

                // Simulate an "ALT" key release.
                keybd_event((byte)ALT, 0x45, EXTENDEDKEY | KEYUP, 0);

                // Show window in forground.
                SetForegroundWindow(processName[0].MainWindowHandle);
               ************/
                 Console.WriteLine("Jammer, maar helaas");
            }  
         
            //{
            //    //Set foreground window
            //    if (IsIconic(processName[0].MainWindowHandle))
            //    {
            //        ShowWindow(processName[0].MainWindowHandle, 9);
            //    }
            //    SetForegroundWindow(processName[0].MainWindowHandle);
            //}
        }

        private void DoTraffic()
        {
            traffic.helloFromMachine++;
            traffic.helloFromAutoSteer++;
            traffic.helloFromIMU++;

            if (focusSkipCounter != 0)
            {

                lblFromGPS.Text = traffic.cntrGPSOut == 0 ? "--" : (traffic.cntrGPSOut).ToString();

                if (isConnectedSteer)
                {
                    lblToSteer.Text = traffic.cntrSteerIn == 0 ? "--" : (traffic.cntrSteerIn).ToString();
                    lblFromSteer.Text = traffic.cntrSteerOut == 0 ? "--" : (traffic.cntrSteerOut).ToString();
                }

                if (isConnectedMachine)
                {
                    lblToMachine.Text = traffic.cntrMachineIn == 0 ? "--" : (traffic.cntrMachineIn).ToString();
                    lblFromMachine.Text = traffic.cntrMachineOut == 0 ? "--" : (traffic.cntrMachineOut).ToString();
                }

                if (isConnectedIMU)
                lblFromMU.Text = traffic.cntrIMUOut == 0 ? "--" : (traffic.cntrIMUOut).ToString();

                //reset all counters
                traffic.cntrPGNToAOG = traffic.cntrPGNFromAOG = traffic.cntrGPSOut =
                    traffic.cntrIMUOut = traffic.cntrSteerIn = traffic.cntrSteerOut =
                    traffic.cntrMachineOut = traffic.cntrMachineIn = 0;

                lblCurentLon.Text = longitude.ToString("N7");
                lblCurrentLat.Text = latitude.ToString("N7");
            }
        }

        // Buttons, Checkboxes and Clicks  ***************************************************

        private void RescanPorts()
        {
            string[] ports = System.IO.Ports.SerialPort.GetPortNames();

            if (ports.Length == 0)
            {
                lblSerialPorts.Text = "None";
            }
            else
            {
                for (int i = 0; i < ports.Length; i++)
                {
                    lblSerialPorts.Text = ports[i] + " ";
                }
            }
        }

        private void deviceManagerToolStripMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (OperatingSystem.IsWindows())
              Process.Start("devmgmt.msc");
            else
            {
		       //Console.WriteLine(" Koop een andere fiets");  
		       var list = DeviceList.Local;
			   //var device = list.GetHidDevices().FirstOrDefault();
			   var allDevices = list.GetAllDevices();
               foreach (HidDevice dev in allDevices)
			   {
				  Console.WriteLine("  " + dev.ToString());
			   }
				
			}
        }

        private void cboxIsSteerModule_Click(object sender, RoutedEventArgs e)
        {
            isConnectedSteer = cboxIsSteerModule.IsChecked ?? false;
            SetModulesOnOff();  
        }

        private void cboxIsMachineModule_Click(object sender, RoutedEventArgs e)
        {
            isConnectedMachine = cboxIsMachineModule.IsChecked ?? false;
            SetModulesOnOff();
        }

        private void lblMessages_Click(object sender, RoutedEventArgs e)
        {
            aList?.Clear();
            sbRTCM.Clear();
            sbRTCM.Append("Reset..");
        }

        private void btnResetTimer_Click(object sender, RoutedEventArgs e)
        {
            threeMinuteTimer = secondsSinceStart;
        }

        private async void serialPassThroughToolStripMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (isRadio_RequiredOn)
            {
                var tmb = new TimedMessageBox(2000, "Radio NTRIP ON", "Turn it off before using Serial Pass Thru");
                tmb.ShowDialog(this);
                return;
            }

            if (isNTRIP_RequiredOn)
            {
                var tbox = new TimedMessageBox(2000, "Air NTRIP ON", "Turn it off before using Serial Pass Thru");
                tbox.ShowDialog(this);
                return;
            }
                   

            var form = new SerialPassView(this);
            if (await form.ShowDialog<bool>(this) == true)
            {
                 if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                 {
                     //desktop.ShutdownRequested += This_ShutdownRequested;
                     desktop.MainWindow = new MainWindow();
                 }  
                    ////Clicked Save
                    //Application.Restart();
                 Environment.Exit(0);
            }
          
        }

        private void btnRelayTest_Click(object sender, RoutedEventArgs e)
        {
                helloFromAgIO[7] = 1;
        }

        private void toolStripMenuItem4_Click(object sender, RoutedEventArgs e)
        {
            /****
            Form f = Application.OpenForms["FormGPSData"];

            if (f != null)
            {
                f.Focus();
                f.Close();
                isGPSSentencesOn = false;
                return;
            }
             *****/
                var windows = ((IClassicDesktopStyleApplicationLifetime)Application.Current.ApplicationLifetime).Windows;
          
	       foreach (Window w in windows)
	       {
	           if (w.ToString() == "AAgIO.Views.GPSDataView")
			   {
					if (w.IsActive)
					{
						w.Focus();
						w.Close();
					    isGPSSentencesOn = false;
	                	return;
					}
				}		
			}	
            isGPSSentencesOn = true;

            var view = new GPSDataView(this);
            view.ShowDialog(this);
        }

        private void lblIP_Click(object sender, RoutedEventArgs e)
        {
            lblIP.Content = "";

            foreach (IPAddress IPA in Dns.GetHostAddresses(Dns.GetHostName()))
            {
                if (IPA.AddressFamily == AddressFamily.InterNetwork)
                {
                    string data = IPA.ToString();
                    lblIP.Content += IPA.ToString() + "\r\n";
                }
            }
        }

        private void toolStripMenuItem1_Click(object sender, RoutedEventArgs e)
        {
            //Save curent Settngs
            var form = new CommSaverView(this);
            form.ShowDialog(this);
            
        }

        private async void toolStripMenuItem2_Click(object sender, RoutedEventArgs e)
        {
            //Load new settings
            //using (var form = new FormCommPicker(this))
            var view = new CommPickerView(this);
            var result = await view.ShowDialog<DialogResult>(this);
         
            
            if (result == DialogResult.OK)
            {
                //Application.Restart();
                if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
			    {
					   desktop.Shutdown();
					   desktop.MainWindow = new MainWindow();
			    }
                Environment.Exit(0);
            }
            
          
        }

        private void lblNTRIPBytes_Click(object sender, RoutedEventArgs e)
        {
            tripBytes = 0;
        }

        private void cboxIsIMUModule_Click(object sender, RoutedEventArgs e)
        {
            isConnectedIMU = cboxIsIMUModule.IsChecked ?? false;
            SetModulesOnOff();
        }

        private void btnBringUpCommSettings_Click(object sender, RoutedEventArgs e)
        {
            SettingsCommunicationGPS();
            RescanPorts();
        }

        private void btnUDP_Click(object sender, RoutedEventArgs e)
        {
            SettingsUDP();
        }

        private void btnRunAOG_Click(object sender, RoutedEventArgs e)
        {
            StartAOG();
        }

        private void btnNTRIP_Click(object sender, RoutedEventArgs e)
        {
            SettingsNTRIP();
        }

        private async void btnExit_Click(object sender, RoutedEventArgs e)
        {
            //test
            //var result = await  AAgIO.Dialogs.MessageBox.Show(this, "Bevestig", "Afsluiten?", AAgIO.Dialogs.MessageBox.MessageBoxButtons.YesNoCancel);
           //if (result == AAgIO.Dialogs.MessageBox.MessageBoxResult.Yes)
            Close();
        }

        private void pictureBox2_Click(object sender, RoutedEventArgs e)
        {
           /*******
            Form f = Application.OpenForms["FormGPSData"];

            if (f != null)
            {
                f.Focus();
                f.Close();
                isGPSSentencesOn = false;
                return;
            }
            **********/
           var windows = ((IClassicDesktopStyleApplicationLifetime)Application.Current.ApplicationLifetime).Windows;
          
	       foreach (Window w in windows)
	       {
	           if (w.ToString() == "AAgIO.Views.GPSDataView")
			   {
					if (w.IsActive)
					{
						w.Focus();
						w.Close();
					    isGPSSentencesOn = false;
	                	return;
					}
				}		
			}	

            isGPSSentencesOn = true;

            var view = new GPSDataView(this);
            view.Show(this);
        }

        private void btnRadio_Click_1(object sender, RoutedEventArgs e)
        {
            SettingsRadio();
        }
      
         public async Task<ButtonResult> callMessageBox(string message)
         {
              var box = MessageBoxManager.GetMessageBoxStandard(
		       new MessageBoxStandardParams
		       {
		           ButtonDefinitions = ButtonEnum.YesNo,
		           ContentMessage = message,
		           Icon = MsBox.Avalonia.Enums.Icon.Question
		       });   
		      return await box.ShowWindowDialogAsync(this);  
		       
        }   

        private async void btnWindowsShutDown_Click(object sender, RoutedEventArgs e)
        {
            // var result3 = callMessageBox("Shutdown Windows For Realz ?" +
            //     "For Sure For Sure ?");
            /**** 
            DialogResult result3 = MessageBox.Show("Shutdown Windows For Realz ?",
                "For Sure For Sure ?",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button2);
            ******/
            var box = MessageBoxManager.GetMessageBoxStandard(
		       new MessageBoxStandardParams
		       {
		           ButtonDefinitions = ButtonEnum.YesNo,
		           ContentTitle = "",
		           ContentMessage = "Shutdown Windows For Realz ?  " +
                "For Sure For Sure ?",
		           Icon = MsBox.Avalonia.Enums.Icon.Question
		       });   
		    var result = await box.ShowWindowDialogAsync(this);
            Console.WriteLine("resultaat " + result);
            if (result == ButtonResult.Yes)
            {
              if (OperatingSystem.IsWindows())
                Process.Start("shutdown", "/s /t 0");
            }
        }

        private void toolStripGPSData_Click(object sender, RoutedEventArgs e)
        {
            /******
            Form f = Application.OpenForms["FormGPSData"];

            if (f != null)
            {
                f.Focus();
                f.Close();
                isGPSSentencesOn = false;
                return;
            }
            **********/
           var windows = ((IClassicDesktopStyleApplicationLifetime)Application.Current.ApplicationLifetime).Windows;
          
	       foreach (Window w in windows)
	       {
	           if (w.ToString() == "AAgIO.Views.GPSDataView")
			   {
					if (w.IsActive)
					{
						w.Focus();
						w.Close();
					    isGPSSentencesOn = false;
	                	return;
					}
				}		
			}	

            isGPSSentencesOn = true;

            var view = new GPSDataView(this);
            view.Show(this);
        }

        private void cboxLogNMEA_CheckedChanged(object sender, RoutedEventArgs e)
        {
            isLogNMEA = cboxLogNMEA.IsChecked ?? false;
        }
     
    }
	
}	
