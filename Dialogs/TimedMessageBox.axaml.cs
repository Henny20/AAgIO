using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;

namespace AAgIO.Dialogs {

  public partial class TimedMessageBox: Window {

    private DispatcherTimer _timer = new DispatcherTimer();
    
   public TimedMessageBox() 
    {
      InitializeComponent();
   }
      /********  
    private void InitializeComponent()
    {
       AvaloniaXamlLoader.Load(this);
    }  
    **************/
    public TimedMessageBox(int timeout, string s1, string s2) {
    
    InitializeComponent();

      _timer.Interval = TimeSpan.FromMilliseconds((double) timeout);
      _timer.IsEnabled = true;
      _timer.Tick += autoCloseTimer_Tick;
      _timer.Start();
      
       var textblock = new TextBlock { Text = s1 };
       stapa.Children.Add(textblock); 
       mes2.Text= s2;
        Console.WriteLine(s1);
    }

    void autoCloseTimer_Tick(object ? sender, EventArgs e) {

      Dispatcher.UIThread.InvokeAsync(new Action(() =>{
        this.Close();
      }), DispatcherPriority.Render);
    }
  }
}
