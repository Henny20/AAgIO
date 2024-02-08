using System.IO;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Interactivity;
//using Avalonia.Markup.Xaml;
using AAgIO.Enums;
using AAgIO.Dialogs;
using AAgIO.Classes;
using MsBox.Avalonia.Enums;

namespace AAgIO.Views
{
	public partial class CommPickerView : Window
	{
		public CommPickerView()
		{
		    InitializeComponent();
		    Loaded += FormCommPicker_Load;
		    cboxEnv.SelectionChanged += cboxEnv_SelectedIndexChanged;
		}
		
		private readonly MainWindow mf = null;

        public CommPickerView(Window callingForm):this()
        {
            //get copy of the calling main form
            mf = callingForm as MainWindow;
		}
		
		// private void InitializeComponent()
        //{
        //     AvaloniaXamlLoader.Load(this);
        //}
        
        private readonly List<string> fileList = new List<string>();

        
        private async void FormCommPicker_Load(object sender, System.EventArgs e)
        {
            DirectoryInfo dinfo = new DirectoryInfo(mf.commDirectory);
            FileInfo[] Files = dinfo.GetFiles("*.xml");
            if (Files.Length == 0)
            {
                //DialogResult result = DialogResult.Ignore;
                var result = await mf.callMessageBox("Non Saved  " + "Save one First");
                if (result == ButtonResult.Ok)
                   Close();
               
            }

            else
            {
                foreach (FileInfo file in Files)
                {
                    fileList.Add(Path.GetFileNameWithoutExtension(file.Name));
                }
            }
        }

        private void cboxEnv_SelectedIndexChanged(object sender, RoutedEventArgs e)
        {
            SettingsIO.ImportSettings(mf.commDirectory + cboxEnv.SelectedItem.ToString() + ".xml");

            DialogResult result = DialogResult.OK;
            Close();
        }
	}
}	
