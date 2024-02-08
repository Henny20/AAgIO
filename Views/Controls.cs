using System;
using System.Diagnostics;
using System.IO;
using Avalonia;
using Avalonia.Interactivity;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using AAgIO.Dialogs;
using AAgIO.Classes;


namespace AAgIO.Views
{

    public partial class MainWindow : Window
    {
        
        private void SettingsCommunicationGPS()
        {
            isGPSCommOpen = true;

            CommSetGPSView form = new CommSetGPSView(this);
          
            form.ShowDialog(this);
            
            isGPSCommOpen = false;
        }

        private async void SettingsNTRIP()
        {
            if (isRadio_RequiredOn)
            {
                var tmb1 = new TimedMessageBox(2000, "Radio NTRIP ON", "Turn it off before using NTRIP");
                tmb1.ShowDialog(this);
                return;
            }

            if (isSerialPass_RequiredOn)
            {
                var tmb2 = new TimedMessageBox(2000, "Serial NTRIP ON", "Turn it off before using NTRIP");
                tmb2.ShowDialog(this);
                return;
            }

            var form = new NtripView();
            {
                if (await form.ShowDialog<bool>(this) == true)
                {
                    if (isNTRIP_Connected)
                    {
                        SettingsShutDownNTRIP();
                    }
                }
            }
        }

        private void SettingsRadio()
        {
            if (isSerialPass_RequiredOn)
            {
                var tmb = new TimedMessageBox(2000, "Serial Pass NTRIP ON", "Turn it off before using Radio NTRIP");
                tmb.ShowDialog(this);
                return;
            }

            if (isNTRIP_RequiredOn)
            {
                var tmb2 = new TimedMessageBox(2000, "Air NTRIP ON", "Turn it off before using Radio NTRIP");
                tmb2.ShowDialog(this);
              
                return;
            }

            if (isRadio_RequiredOn && isNTRIP_Connected)
            {
                ShutDownNTRIP();
                lblWatch.Text = "Stopped";
                btnStartStopNtrip.Content = "OffLine";
                isRadio_RequiredOn = false;
            }

            var form = new RadioView(this);
            form.ShowDialog(this);
           
        }

        private async void SettingsUDP()
        {
           var form = new UDPView(this);
          //  {
                if (await form.ShowDialog<bool>(this) == false )
                {
                    //Clicked Save
                   // Application.Restart();
                    if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
				    {
	
					   desktop.Shutdown();
					
				    }
                    Environment.Exit(0);
                }
          //  }
        }

        private void StartAOG()
        {
            Process[] processName = Process.GetProcessesByName("AAgOpenGPS");
            if (processName.Length == 0)
            {
                //Start application here
                //DirectoryInfo di = new DirectoryInfo(Application.StartupPath);
                //string strPath = di.ToString();
                string strPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
                Console.WriteLine("AAgOpenGPS pad is:   " + strPath);
                strPath += "\\AAgOpenGPS.exe";

                try
                {
                    ProcessStartInfo processInfo = new ProcessStartInfo();
                    processInfo.FileName = strPath;
                    processInfo.WorkingDirectory = Path.GetDirectoryName(strPath);
                    Process proc = Process.Start(processInfo);
                }
                catch
                {
                    var tmb = new TimedMessageBox(2000, "No File Found", "Can't Find AgOpenGPS");
                    tmb.ShowDialog(this);
                }
            }
            else
            {
                //Set foreground window
               //ShowWindow(processName[0].MainWindowHandle, 9); TODO
               if (OperatingSystem.IsWindows())
                Win32Helper.SetForegroundWindow(processName[0].MainWindowHandle);
            }
        }

        private void btnStartStopNtrip_Click(object sender, RoutedEventArgs e)
        {
            if (Properties.Settings.Default.setNTRIP_isOn || Properties.Settings.Default.setRadio_isOn)
            {
                if (isNTRIP_RequiredOn || isRadio_RequiredOn)
                {
                    ShutDownNTRIP();
                    lblWatch.Text = "Stopped";
                    btnStartStopNtrip.Content = "OffLine";
                    isNTRIP_RequiredOn = false;
                    isRadio_RequiredOn = false;
                    lblNTRIP_IP.Text = "--";
                    lblMount.Text = "--";
                }
                else
                {
                    isNTRIP_RequiredOn = Properties.Settings.Default.setNTRIP_isOn;
                    isRadio_RequiredOn = Properties.Settings.Default.setRadio_isOn;
                    lblWatch.Text = "Waiting";
                    lblNTRIP_IP.Text = "--";
                    lblMount.Text= "--";
                }
            }
            else
            {
               var box = new TimedMessageBox(2000, "Turn on NTRIP", "NTRIP Client Not Set Up");
               box.ShowDialog(this);
            }
        }

        private void loadToolStrip_Click(object sender, RoutedEventArgs e)
        {

        }

        private void saveToolStrip_Click(object sender, RoutedEventArgs e)
        {

        }
/******* hennie
        public void KeypadToNUD(NumericUpDown sender, Form owner)
        {
            sender.BackColor = System.Drawing.Color.Red;
            using (var form = new FormNumeric((double)sender.Minimum, (double)sender.Maximum, (double)sender.Value))
            {
                if (form.ShowDialog(owner) == DialogResult.OK)
                {
                    sender.Value = (decimal)form.ReturnValue;
                }
            }
            sender.BackColor = System.Drawing.Color.AliceBlue;
        }

        public void KeyboardToText(TextBox sender, Form owner)
        {
            TextBox tbox = (TextBox)sender;
            tbox.BackColor = System.Drawing.Color.Red;
            using (var form = new FormKeyboard((string)tbox.Text))
            {
                if (form.ShowDialog(owner) == DialogResult.OK)
                {
                    tbox.Text = (string)form.ReturnString;
                }
            }
            tbox.BackColor = System.Drawing.Color.AliceBlue;
        }

        private ToolStripDropDownButton toolStripDropDownButton1;
        private ToolStripMenuItem toolStripMenuItem1;
        private ToolStripMenuItem toolStripMenuItem2;
        private ToolStripMenuItem toolStripMenuItem4;
        private ToolStripMenuItem deviceManagerToolStripMenuItem;
        *************/
    }
}
