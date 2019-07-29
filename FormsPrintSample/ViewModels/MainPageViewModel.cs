using System;
using System.ComponentModel;
using FormsPrintSample.Services;

namespace FormsPrintSample.ViewModels
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        public string Test { get; set; }
        public IPrintService _printService { get; set; }
        public MainPageViewModel(IPrintService printService)
        {
            _printService = printService;
            Test = "Hello";
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
