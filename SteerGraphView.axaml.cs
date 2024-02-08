using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace AAgIO.Views
{
	public partial class SteerGraphView : Window
	{
	    private readonly MainWindow mf = null;

        //chart data
        private string dataSteerAngle = "0";

        private string dataPWM = "-1";

	
		public SteerGraphView()
		{
		    InitializeComponent();
		    AttachedToVisualTree += FormSteerGraph_Load;
		}
		
		 private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
        
        public SteerGraphView(Window callingForm):this()
        {
            mf = callingForm as MainWindow;
           
            this.label5.Text = "gStr.gsSetPoint";
            this.label1.Text = "gStr.gsActual";

            this.Title = "gStr.gsSteerChart";
        }

        private void timer1_Tick(object sender, RoutedEventArgs e)
        {
           DrawChart();
        }

        private void DrawChart()
        {
        }
        
         private void FormSteerGraph_Load(object sender, VisualTreeAttachmentEventArgs e)
        {
           // timer1.Interval = (int)((1 / (double)mf.fixUpdateHz) * 1000);
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnAuto_Click(object sender, RoutedEventArgs e)
        {
        }
        
         private void btnPlus_Click(object sender, RoutedEventArgs e)
        {
        }
        
        private void btnMinus_Click(object sender, RoutedEventArgs e)
        {
        }
	}
}	
