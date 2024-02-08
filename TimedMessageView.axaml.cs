using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace AAgIO.Views
{
	public partial class TimedMessageView : Window
	{
		public TimedMessageView()
		{
		    InitializeComponent();
		}
		
		 private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
	}
}	
