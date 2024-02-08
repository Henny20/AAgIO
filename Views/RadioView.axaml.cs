using System;
using System.IO.Ports;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using AAgIO.Classes;
using AAgIO.Models;
using AAgIO.Dialogs;

namespace AAgIO.Views
{
	public partial class RadioView : Window
	{
	    private readonly MainWindow mf;
        private List<CRadioChannel> _channels;
        private double _currentLat = 0;
        private double _currentLon = 0;
        
        public ObservableCollection<CRadioChannel> Channel { get; set;}

        //private readonly ListViewColumnSorterExt _listViewColumnSorter;
        
		public RadioView()
		{
		    InitializeComponent();
		    Loaded += FormRadio_Load;
		}
		
		//private void InitializeComponent()
       //// {
       //     AvaloniaXamlLoader.Load(this);
       // }
        
        public RadioView(Window callingForm)
              :this()
        {
            mf = callingForm as MainWindow;

            // Set the icon, it is not shown on top. But it is in the taskbar
            this.Icon = mf.Icon;
            //_listViewColumnSorter = new ListViewColumnSorterExt(lvChannels); Niet bruikbaar

            _currentLat = mf.latitude;
            _currentLon = mf.longitude;

            // Load radio channels
            _channels = Properties.Settings.Default.setRadio_Channels;

            if (_channels == null)
            {
                // No channels found, create a new list
                _channels = new List<CRadioChannel>();
            }

            foreach (var channel in _channels)
            {
                AddChannelToListView(channel);
            }
        }

        private void FormRadio_Load(object sender, System.EventArgs e)
        {
            cboxRadioPort.Items.Clear();

            foreach (string s in System.IO.Ports.SerialPort.GetPortNames())
            {
                cboxRadioPort.Items.Add(s);
            }

            lblCurrentPort.Text = Properties.Settings.Default.setPort_portNameRadio;
            lblCurrentBaud.Text = Properties.Settings.Default.setPort_baudRateRadio;
            cboxRadioPort.PlaceholderText = Properties.Settings.Default.setPort_portNameRadio;
            cboxBaud.PlaceholderText = Properties.Settings.Default.setPort_baudRateRadio;
            cboxIsRadioOn.IsChecked = Properties.Settings.Default.setRadio_isOn;
            /***      Geen idee
            for(var i = 0; i < lvChannels.Items.Count; i++)
            {
                ListViewItem lvItem = lvChannels.Items[i];

                if (((CRadioChannel)lvItem.Tag).Frequency == Properties.Settings.Default.setPort_radioChannel)
                {
                    lvItem.Selected = true;
                    lvItem.Focused = true;
                    lvItem.EnsureVisible();
                    break;
                }
            }

            lvChannels.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            lvChannels.AutoResizeColumn(1, ColumnHeaderAutoResizeStyle.ColumnContent);

            SetButtonState();
            ***********/
            var channels = new List<CRadioChannel>();
            channels.Add(new CRadioChannel()
            {
                    Id = 1,
                    Name = "AAp",
                    Frequency = "1234",
                    Location = ""
            });
            Channel = new ObservableCollection<CRadioChannel>(channels);
            lvChannels.ItemsSource = Channel;
        }

        private void btnRadioOK_Click(object sender, RoutedEventArgs e)
        {
            if(cboxIsRadioOn.IsChecked == false)//&& lvChannels.SelectedItems.Count == 0)
            {
                var tmb = new TimedMessageBox(2000, "No channel", "Radio is set to on. But no channel is selected");
                tmb.ShowDialog(this);
                // Cancel close
                //DialogResult = DialogResult.None;
                return;
            }
            /** Geen idee
            if (lvChannels.SelectedItems.Count > 0)
            {
                var selectedChannel = (CRadioChannel)lvChannels.SelectedItems[0].Tag;
                Properties.Settings.Default.setPort_radioChannel = selectedChannel.Frequency;
            }
            *******/

            Properties.Settings.Default.setPort_portNameRadio = cboxRadioPort.PlaceholderText;
            Properties.Settings.Default.setPort_baudRateRadio = cboxBaud.PlaceholderText;
            Properties.Settings.Default.setRadio_isOn = (bool)cboxIsRadioOn.IsChecked;

            if (Properties.Settings.Default.setRadio_isOn && Properties.Settings.Default.setNTRIP_isOn)
            {
                var box = new TimedMessageBox(2000, "NTRIP also IsEnabled", "NTRIP is also IsEnabled, diabling it");
                box.ShowDialog(this);
                Properties.Settings.Default.setNTRIP_isOn = false;
            }

            // Save radio channels
            Properties.Settings.Default.setRadio_Channels = _channels;
            Properties.Settings.Default.Save();

            Close();
            mf.ConfigureNTRIP();
        }

        private void btnRescan_Click(object sender, RoutedEventArgs e)
        {
            cboxRadioPort.Items.Clear();

            foreach (string s in System.IO.Ports.SerialPort.GetPortNames())
            {
                cboxRadioPort.Items.Add(s);
            }
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
            mf.spRadio.NewLine = "\r\n";

            btnOpenSerial.IsEnabled = false;
            btnSetChannel.IsEnabled = true;
            btnCloseSerial.IsEnabled = true;

            try
            {
                mf.spRadio.Open();

                lblCurrentPort.Text = Properties.Settings.Default.setPort_portNameRadio;
                lblCurrentBaud.Text = Properties.Settings.Default.setPort_baudRateRadio;

                cboxRadioPort.IsEnabled = false;
                cboxBaud.IsEnabled = false;
            }
            catch(Exception ex)
            {
               var tmb= new TimedMessageBox(3000, "Error opening port", ex.Message);
               tmb.ShowDialog(this);
            }

            SetButtonState();
        }

        private void btnCloseSerial_Click(object sender, RoutedEventArgs e)
        {
            if(mf.spRadio != null && mf.spRadio.IsOpen)
            {
                mf.spRadio.Close();
                mf.spRadio.Dispose();
                mf.spRadio = null;

                btnOpenSerial.IsEnabled = true;
                btnSetChannel.IsEnabled = false;
                btnCloseSerial.IsEnabled = false;

                cboxRadioPort.IsEnabled = true;
                cboxBaud.IsEnabled = true;
            }

            SetButtonState();
        }

        private void btnSetChannel_Click(object sender, RoutedEventArgs e)
        {
           if(mf.spRadio != null && mf.spRadio.IsOpen) // && lvChannels.SelectedItems.Count == 1)
           {
               var selectedChannel = (CRadioChannel)lvChannels.SelectedItem; //(CRadioChannel)lvChannels.SelectedItems[0].Tag;

               //SL&F=nnn.nnnnn
               mf.spRadio.WriteLine($"SL&F={selectedChannel.Frequency}");
           }    
        }

        private void btnSendCommand_Click(object sender, RoutedEventArgs e)
        {
            if (mf.spRadio != null && mf.spRadio.IsOpen)
            {
                mf.spRadio.WriteLine(tbCommand.Text);
                System.Threading.Thread.Sleep(0);

                int bytesToRead = mf.spRadio.BytesToRead;
                
                if (bytesToRead == 0)
                    return;
                
                // TODO: What if the radio is receiving data? It will be in the read
                byte[] buffer = new byte[bytesToRead];
                mf.spRadio.Read(buffer, 0, bytesToRead);                
                tbResponse.Text = System.Text.Encoding.UTF8.GetString(buffer, 0, buffer.Length);
            }
        }

        private async void btnAddChannel_Click(object sender, RoutedEventArgs e)
        {
            var form = new RadioChannelView(mf);
            {
                // Get max id
                var maxChannelId = 0;

                if (_channels.Count > 0)
                {
                    maxChannelId = _channels.Max(c => c.Id);
                }

                form.Channel.Id = maxChannelId + 1;

                if (await form.ShowDialog<bool>(this) == true)
                {
                    _channels.Add(form.Channel);
                    AddChannelToListView(form.Channel);

                    // Resize
                    //lvChannels.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize); Geen idee
                   // lvChannels.AutoResizeColumn(1, ColumnHeaderAutoResizeStyle.ColumnContent);
                }
            }
        }

        private void btnEditChannel_Click(object sender, RoutedEventArgs e)
        {
        /****
            if (lvChannels.SelectedItems.Count == 1)
            {
                var form = new RadioChannelView(mf);
                
                var selectedChannel = (CRadioChannel)lvChannels.SelectedItems[0].Tag;

                form.Channel.Id = selectedChannel.Id;
                form.Channel.Name = selectedChannel.Name;
                form.Channel.Frequency = selectedChannel.Frequency;
                form.Channel.Location = selectedChannel.Location;

                if (form.ShowDialog<bool>(this).Result == true)
                {
                    // Set in channel
                    selectedChannel.Id = form.Channel.Id;
                    selectedChannel.Name = form.Channel.Name;
                    selectedChannel.Frequency = form.Channel.Frequency;
                    selectedChannel.Location = form.Channel.Location;

                    // Set in listview
                    // TODO: Use keys
                    lvChannels.SelectedItems[0].SubItems[0].Text = selectedChannel.Id.ToString(); geen idee
                    lvChannels.SelectedItems[0].SubItems[1].Text = selectedChannel.Name;
                    lvChannels.SelectedItems[0].SubItems[2].Text = selectedChannel.Frequency;

                    // Resize
                    lvChannels.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
                    lvChannels.AutoResizeColumn(1, ColumnHeaderAutoResizeStyle.ColumnContent);
                }
                
            }*************/
        }

        private void btnDeleteChannel_Click(object sender, RoutedEventArgs e)
        {
        /**** Geen idee
            if (lvChannels.SelectedItems.Count > 0)
            {
                var selectedItem = lvChannels.SelectedItems[0];
                lvChannels.Items.Remove(selectedItem);
                _channels.Clear();

                foreach(Item lvItem in lvChannels.Items)
                {
                    _channels.Add((CRadioChannel)lvItem.Tag);
                }

                // Resize
                lvChannels.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
                lvChannels.AutoResizeColumn(1, ColumnHeaderAutoResizeStyle.ColumnContent);
            }
            ***********/
        }

        private void AddChannelToListView(CRadioChannel channel)
        {
       
            var distance = "-";

            // calculate distance
            if(!string.IsNullOrEmpty(channel.Location) && _currentLat > 0 && _currentLon > 0)
            {
                var locationArray = channel.Location.Split(' ');

                if (locationArray.Length >= 2)
                {
                    if (double.TryParse(locationArray[0], NumberStyles.Float, CultureInfo.InvariantCulture, out double lat) &&
                        double.TryParse(locationArray[1], NumberStyles.Float, CultureInfo.InvariantCulture, out double lon))
                    {
                        distance = glm.DistanceLonLat(lon, lat, _currentLon, _currentLat).ToString("N2");
                    }
                }
            }

            Channel.Add(new CRadioChannel()
            {
                    Id = channel.Id,
                    Name = channel.Name,
                    Frequency = channel.Frequency,
                    Location = distance
            });
            //{
           //     Tag = channel
            //});
           
        }

        private void SetButtonState()
        {
            // Set connect disconnect and send buttons
            var portOpen = mf.spRadio != null && mf.spRadio.IsOpen;

            btnOpenSerial.IsEnabled = !portOpen;
            btnCloseSerial.IsEnabled = portOpen;

            btnSetChannel.IsEnabled = portOpen;
            btnSendCommand.IsEnabled = portOpen;
        }

        private void tbox_Click(object sender, RoutedEventArgs e)
        {
          //  if (mf.isKeyboardOn)
         //   {
         //       mf.KeyboardToText((TextBox)sender, this);
          //  }
        }
	}
}	
