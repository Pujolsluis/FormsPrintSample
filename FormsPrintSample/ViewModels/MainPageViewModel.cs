using System;
using System.ComponentModel;

namespace FormsPrintSample.ViewModels
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        public string Test { get; set; }
        public MainPageViewModel()
        {
            Test = "Hello";
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
