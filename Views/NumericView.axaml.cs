using System;
using System.Globalization ;
using System.Threading;
using Avalonia.Controls;
using Avalonia;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;


namespace AAgIO.Views
{
	public partial class NumericView : Window
	{
		public NumericView()
		{
		    InitializeComponent();
		    AttachedToVisualTree += FormNumeric_Load;
		    this.KeyDown += RegisterKeypad1_ButtonPressed;
		}
		
		 private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
        
        private readonly double max;
        private readonly double min;
        private bool isFirstKey;

        public double ReturnValue { get; set; }

        public NumericView(double _min, double _max, double currentValue)
            :this()
        {
            max = _max;
            min = _min;
            InitializeComponent();

            this.Title = "Enter Value";
            //fill in the display
            tboxNumber.Text = currentValue.ToString();

            isFirstKey = true;
        }

        private void FormNumeric_Load(object sender, VisualTreeAttachmentEventArgs e)
        {
            lblMax.Text = max.ToString();
            lblMin.Text = min.ToString();
            tboxNumber.SelectionStart = tboxNumber.Text.Length;
            //tboxNumber.SelectionLength = 0; SelectionEnd?
            //keypad1.Focus(); Geen idee
        }

        private void RegisterKeypad1_ButtonPressed(object sender, KeyEventArgs e)
        {

            if (isFirstKey)
            {
                tboxNumber.Text = "";
                isFirstKey = false;
            }

            //clear the error as user entered new values
            if (tboxNumber.Text == "Error")
            {
                tboxNumber.Text = "";
                lblMin.Foreground = new SolidColorBrush(Colors.Blue); //SystemColors.ControlText; Windows shit
                lblMax.Foreground =  new SolidColorBrush(Colors.Red); //SystemColors.ControlText;
            }

            //if its a number just add it
            //if (Char.IsNumber(e.Key))
            if ((e.Key >= Key.A && e.Key <= Key.Z) || (e.Key >= Key.D0 && e.Key <= Key.D9) || (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9)) //StackOverflow
            {
                tboxNumber.Text += e.Key;
            }

            //Backspace key, remove 1 char
            else if (e.Key == Key.B)
            {
                if (tboxNumber.Text.Length > 0)
                {
                    tboxNumber.Text = tboxNumber.Text.Remove(tboxNumber.Text.Length - 1);
                }
            }

            //decimal point
            //else if (e.Key == '.')
            else if (e.Key == Key.Decimal)
            {
                //does it already have a decimal?
                if (!tboxNumber.Text.Contains(Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator))
                {
                    tboxNumber.Text += Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator;

                    //if decimal is first char, prefix with a zero
                    if (tboxNumber.Text.IndexOf(Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator) == 0)
                    {
                        tboxNumber.Text = "0" + tboxNumber.Text;
                    }

                    //neg sign then added a decimal, insert a 0 
                    if (tboxNumber.Text.IndexOf("-") == 0 && tboxNumber.Text.IndexOf(Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator) == 1)
                    {
                        tboxNumber.Text = "-0" + Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator;
                    }
                }
            }

            //negative sign
            //else if (e.KeyChar == '-')
            else if (e.Key == Key.Subtract)
            {
                //If already has a negative don't add again
                if (!tboxNumber.Text.Contains("-"))
                {
                    //prefix the negative sign
                    tboxNumber.Text = "-" + tboxNumber.Text;
                }
                else
                {
                    //if already has one, take it away = +/- does that
                    if (tboxNumber.Text.StartsWith("-"))
                    {
                        tboxNumber.Text = tboxNumber.Text.Substring(1);
                    }
                }
            }

            //Exit or cancel
            else if (e.Key == Key.X)
            {
               // this.DialogResult = DialogResult.Cancel; TODO
                Close();
            }

            //clear whole display
            else if (e.Key == Key.C)
            {
                tboxNumber.Text = "";
            }

            //ok button
            else if (e.Key == Key.K)
            {
                //not ok if empty - just return
                if (tboxNumber.Text == "") return;

                //culture invariant parse to double
                double tryNumber = double.Parse(tboxNumber.Text, CultureInfo.CurrentCulture);

                //test if above or below min/max
                if (tryNumber < min)
                {
                    tboxNumber.Text = "Error";
                    lblMin.Foreground = new SolidColorBrush(Colors.Blue);
                }
                else if (tryNumber > max)
                {
                    tboxNumber.Text = "Error";
                    lblMax.Foreground = new SolidColorBrush(Colors.Red);
                }
                else
                {
                    //all good, return the value
                    this.ReturnValue = tryNumber;
                    //this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }

            //Show the cursor
            tboxNumber.SelectionStart = tboxNumber.Text.Length;
           // tboxNumber.SelectionLength = 0; TODO
            tboxNumber.Focus();
        }

        private void BtnDistanceUp_MouseDown(object sender, EventArgs e)
        {
            if (tboxNumber.Text == "" || tboxNumber.Text == "-" || tboxNumber.Text == "Error") tboxNumber.Text = "0";
            double tryNumber = double.Parse(tboxNumber.Text, CultureInfo.CurrentCulture);

            tryNumber++;
            if (tryNumber > max) tryNumber = max;
            tboxNumber.Text = tryNumber.ToString();
            isFirstKey = false;
        }

        private void BtnDistanceDn_MouseDown(object sender, EventArgs e)
        {
            if (tboxNumber.Text == "" || tboxNumber.Text == "-" || tboxNumber.Text == "Error") tboxNumber.Text = "0";
            double tryNumber = double.Parse(tboxNumber.Text, CultureInfo.CurrentCulture);

            tryNumber--;
            if (tryNumber < min) tryNumber = min;

            tboxNumber.Text = tryNumber.ToString();
            isFirstKey = false;
        }
	}
}	
