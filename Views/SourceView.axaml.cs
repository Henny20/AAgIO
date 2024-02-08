using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using System.Collections.Generic;

namespace AAgIO.Views
{
	public partial class SourceView : Window
	{
	    private readonly NtripView nt;
        private readonly List<string> dataList = new List<string>(); 
        private readonly double lat, lon;
        private readonly string site;

        private int order = 0;
        
		public SourceView()
		{
		    InitializeComponent();
		    Loaded += FormSource_Load;
		}
		
	    private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
        
        public SourceView(Window callingForm, List<string> _dataList, double _lat, double _lon, string syte)
           :this()
        {
           
            dataList = _dataList;
            lat = _lat;
            lon = _lon;
            site = syte;
            nt = callingForm as NtripView;
        }

        private void FormSource_Load(object sender, System.EventArgs e)
        {
        }
        
          private void btnSite_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(site);
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnUseMount_Click(object sender, RoutedEventArgs e)
        {
           
        }
        
         private void btnSort_Click(object sender, RoutedEventArgs e)
        {
        }
        
        private void lvLines_SelectedIndexChanged(object sender, RoutedEventArgs e)
        {
        }
        
         public double GetDistance(double longitude, double latitude, double otherLongitude, double otherLatitude)
        {
            double d1 = latitude * (Math.PI / 180.0);
            double num1 = longitude * (Math.PI / 180.0);
            double d2 = otherLatitude * (Math.PI / 180.0);
            double num2 = otherLongitude * (Math.PI / 180.0) - num1;
            double d3 = Math.Pow(Math.Sin((d2 - d1) / 2.0), 2.0) + Math.Cos(d1) 
                * Math.Cos(d2) * Math.Pow(Math.Sin(num2 / 2.0), 2.0);

            return 6376500.0 * (2.0 * Math.Atan2(Math.Sqrt(d3), Math.Sqrt(1.0 - d3)));
        }

	}
}	
