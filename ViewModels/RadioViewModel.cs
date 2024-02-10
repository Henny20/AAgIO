using AAgIO.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace AAgIO.ViewModels
{
    public class RadioViewModel : ViewModelBase
    {
        public ObservableCollection<RadioChannel> Kanaal { get; }

        public  RadioViewModel()
        {
            var channels = new List<RadioChannel> 
            {
                new RadioChannel(1, "Hier", "112234", "Hier")
               
            };
            Kanaal = new ObservableCollection<RadioChannel>(channels);
        }
    }
}
