using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using AAgIO.Dialogs;
using AAgIO.Classes;
using AAgIO.Models;
using Avalonia.Interactivity;
using Avalonia;

namespace AAgIO.Views
{
	public partial class RadioChannelView : Window
	{
		public RadioChannelView()
		{
		    InitializeComponent();
		}
		
		// private void InitializeComponent()
       // {
        //    AvaloniaXamlLoader.Load(this);
        //}
        
        private readonly MainWindow mf;

        public CRadioChannel Channel { get; set; } = new CRadioChannel();

        public RadioChannelView(MainWindow mainForm):this()
        {
            mf = mainForm;
            Loaded += FormRadioChannel_Load;
            
            // Set the icon, it is not shown on top. But it is in the taskbar
            //Icon = mf.Icon;
      
        }

        private void FormRadioChannel_Load(object sender, System.EventArgs e)
        {
            tbId.Text = Channel.Id.ToString();
            tbName.Text = Channel.Name;
            tbFrequency.Text = Channel.Frequency;

            if (!string.IsNullOrEmpty(Channel.Location))
            {
                var locationArray = Channel.Location.Split(' ');

                if (locationArray.Length >= 2)
                {
                    tbLat.Text = locationArray[0];
                    tbLon.Text = locationArray[1];
                }
            }
        }

        private void btnSerialOK_Click(object sender, RoutedEventArgs e)
        {
            if(!int.TryParse(tbId.Text, out int channelId))
            {
                var tmb = new TimedMessageBox(2000, "Invalid Id", $"Id '{tbId.Text}' is not a valid number");
                tmb.ShowDialog(this);
                //DialogResult = DialogResult.None;
                return;
            }

            if (string.IsNullOrEmpty(tbName.Text))
            {
                var tmb1 = new TimedMessageBox(2000, "Invalid Name", $"Name is not filled in");
                tmb1.ShowDialog(this);
                //DialogResult = DialogResult.None;
                return;
            }

            if (string.IsNullOrEmpty(tbFrequency.Text))
            {
                var tmb2 = new TimedMessageBox(2000, "Invalid Frequency", $"Frequency is not filled in");
                tmb2.ShowDialog(this);
                //DialogResult = DialogResult.None;
                return;
            }

            Channel.Id = channelId;
            Channel.Name = tbName.Text;
            Channel.Frequency = tbFrequency.Text;

            if (!string.IsNullOrEmpty(tbLat.Text) && !string.IsNullOrEmpty(tbLon.Text))
            {
                Channel.Location = $"{tbLat.Text} {tbLon.Text}";
            }
        }

        private void tbox_Click(object sender, RoutedEventArgs e)
        {
        /***
            if (mf.isKeyboardOn)
            {
                mf.KeyboardToText((TextBox)sender, this);
            }
        ****/    
        }
	}
}	
