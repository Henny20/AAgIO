using Avalonia.Controls;
using Avalonia.Threading;
//using Avalonia.Markup.Xaml;

namespace AAgIO.Views
{
	public partial class GPSDataView : Window
	{
	    private DispatcherTimer _timer = new DispatcherTimer();
	    
		public GPSDataView()
		{
		    InitializeComponent();
		    Loaded += FormGPSData_Load;
		    Closing += FormGPSData_FormClosing;
		    
		    _timer.Interval = System.TimeSpan.FromMilliseconds(250);
            _timer.IsEnabled = true;
            _timer.Tick += timer1_Tick;
            _timer.Start();
		}
		
	   // private void InitializeComponent()
       // {
       //     AvaloniaXamlLoader.Load(this);
       // }
       
        private readonly MainWindow mf = null;

        public GPSDataView(Window callingForm):this()
        {
            mf = callingForm as MainWindow;
           
        }

        private void timer1_Tick(object sender, System.EventArgs e)
        {
            lblLatitude.Text = mf.latitude.ToString("N7");
            lblLongitude.Text = mf.longitude.ToString("N7");

            ////other sat and GPS info
            lblFixQuality.Text = mf.FixQuality;
            lblSatsTracked.Text = mf.satellitesData.ToString();
            lblHDOP.Text = mf.hdopData.ToString();
            lblSpeed.Text = mf.speedData.ToString("N1");

            lblRoll.Text = mf.rollData.ToString("N2");
            lblIMURoll.Text = mf.imuRollData.ToString();
            lblIMUPitch.Text = mf.imuPitchData.ToString();
            lblIMUYawRate.Text = mf.imuYawRateData.ToString();
            lblIMUHeading.Text = mf.imuHeadingData.ToString();

            lblAge.Text = mf.ageData.ToString("N1");

            lblGPSHeading.Text = mf.headingTrueData.ToString("N2");
            lblDualHeading.Text = mf.headingTrueDualData.ToString("N2");

            lblAltitude.Text = mf.altitudeData.ToString("N1");

            tboxVTG.Text = mf.vtgSentence;
            tboxGGA.Text = mf.ggaSentence;
            tboxPAOGI.Text = mf.paogiSentence;
            tboxAVR.Text = mf.avrSentence;
            tboxHDT.Text = mf.hdtSentence;
            tboxHPD.Text = mf.hpdSentence;
            tboxPANDA.Text = mf.pandaSentence;
            tboxKSXT.Text = mf.ksxtSentence;
        }

        private void FormGPSData_Load(object sender, System.EventArgs e)
        {
            tboxGGA.Text = "";
            tboxVTG.Text = "";
            tboxHDT.Text = "";
            tboxAVR.Text = "";
            tboxPAOGI.Text = "";
            tboxHPD.Text = "";
            tboxPANDA.Text = "";
            tboxKSXT.Text = "";
        }

        private void FormGPSData_FormClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            mf.isGPSSentencesOn = false;
        }
	}
}	
