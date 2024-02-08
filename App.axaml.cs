using System;
using System.Globalization;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using AAgIO.Views;

namespace AAgIO;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        gStr.Culture = new CultureInfo("nl-NL");
        
        var fonts = Avalonia.Media.FontManager.Current.SystemFonts;
        foreach (var item in fonts)
        {
           Console.WriteLine(item);
         }
        
              
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow();
        }

        base.OnFrameworkInitializationCompleted();
    }
}
