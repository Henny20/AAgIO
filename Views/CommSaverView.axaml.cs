using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using Avalonia.Input;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using AAgIO.Classes;

namespace AAgIO.Views
{
	public partial class CommSaverView : Window
	{ 
	    private readonly MainWindow mf = null;
	    
	    private readonly List<string> fileList = new List<string>();
	
		public CommSaverView()
		{
		    InitializeComponent();
		    Loaded += FormCommSaver_Load;
		    cboxEnv.SelectionChanged += cboxVeh_SelectedIndexChanged;
		}
			
        public CommSaverView(Window callingForm):this()
        {
            //get copy of the calling main form
            mf = callingForm as MainWindow;
        }   
            
		//private void InitializeComponent()
        //{
        //     AvaloniaXamlLoader.Load(this);
        //}
        
        private void FormCommSaver_Load(object sender, System.EventArgs e)
        {
            
            lblLast.Text = "Current " + mf.commFileName;
            DirectoryInfo dinfo = new DirectoryInfo(mf.commDirectory);
            //DirectoryInfo dinfo = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/AgOpenGPS/" + "AgIO/");
            FileInfo[] Files = dinfo.GetFiles("*.xml");

            if (Files.Length == 0)
            {
                cboxEnv.IsEnabled = false;
            }

            foreach (FileInfo file in Files)
            {
                fileList.Add(Path.GetFileNameWithoutExtension(file.Name));
            }
           
        }

        private void cboxVeh_SelectedIndexChanged(object sender, RoutedEventArgs e)
        {
           /******
            DialogResult result3 = MessageBox.Show(
                "Overwrite: " + cboxEnv.SelectedItem.ToString() + ".xml",
                gStr.gsSaveAndReturn,
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button2);

            if (result3 == DialogResult.Yes)
            {
                SettingsIO.ExportSettings(mf.commDirectory + cboxEnv.SelectedItem.ToString() + ".xml");
                Close();
            }
            ************/
        }

        private void tboxName_OnTextInput(object sender, TextInputEventArgs e)
        {
        
            TextBox textboxSender = (TextBox)sender;
            int cursorPosition = textboxSender.SelectionStart;
            textboxSender.Text = Regex.Replace(textboxSender.Text, glm.fileRegex, "");

            textboxSender.SelectionStart = cursorPosition;
          
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
       
            if (tboxName.Text.Trim().Length > 0)
            {
                SettingsIO.ExportSettings(mf.commDirectory + tboxName.Text.Trim() + ".xml");
                Close();
            }
            else
            {
                //DialogResult result3 = MessageBox.Show("Enter a File Name To Save...",
                //gStr.gsSaveAndReturn, MessageBoxButtons.OK);
            }
          
        }

        private void tboxName_Click(object sender, RoutedEventArgs e)
        {
        /****
            if (mf.isKeyboardOn)
            {
                mf.KeyboardToText((TextBox)sender, this);
                btnSave.Focus();
            }
            ***********/
        }

        private void btnSerialCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
	}
}	
