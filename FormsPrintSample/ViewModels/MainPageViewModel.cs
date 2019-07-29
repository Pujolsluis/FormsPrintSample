using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using FormsPrintSample.Services;
using Plugin.Media.Abstractions;
using Plugin.Permissions.Abstractions;
using Prism.Commands;
using Prism.Services;
using Xamarin.Forms;

namespace FormsPrintSample.ViewModels
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        IMedia _mediaService;
        IPermissions _permissionService;
        IPageDialogService _pageDialogService;
        IPrintService _printService;
        public DelegateCommand OnPrintImageCommand { get; set; }
        public DelegateCommand OnPrintPDFCommand { get; set; }
        string TakePhoto = "Take Photo";
        string PickPhoto = "Pick Photo";

        public event PropertyChangedEventHandler PropertyChanged;

        public MainPageViewModel(IMedia media, IPermissions permissions, IPageDialogService pageDialogService, IPrintService printService)
        {

            _mediaService = media;
            _permissionService = permissions;
            _pageDialogService = pageDialogService;
            _printService = printService;

            OnPrintImageCommand = new DelegateCommand(PrintImage);
            OnPrintPDFCommand = new DelegateCommand(PrintPDF);
        }

        async void PrintPDF()
        {
            // Get Stream of Embedded PDF file
            Stream stream = typeof(App).GetTypeInfo().Assembly.GetManifestResourceStream("FormsPrintSample.Resources.SamplePDF.pdf");
            _printService.PrintPdfFile(stream);
        }

        private async void PrintImage()
        {
            var action = await _pageDialogService.DisplayActionSheetAsync("Pick Photo", "Cancel",
                                                                          null, TakePhoto, PickPhoto);

            if (action == null) return;

            try
            {

                if (action.Equals(TakePhoto))
                {
                    if (!_mediaService.IsCameraAvailable || !_mediaService.IsTakePhotoSupported)
                    {
                        await _pageDialogService.DisplayAlertAsync("No Camera", "There is no camera available", "Ok");
                        return;
                    }

                    var file = await _mediaService.TakePhotoAsync(new StoreCameraMediaOptions
                    {
                        Directory = "TestPrintApp",
                        SaveToAlbum = true,
                        CompressionQuality = 75,
                        CustomPhotoSize = 50,
                        PhotoSize = PhotoSize.MaxWidthHeight,
                        MaxWidthHeight = 2000,
                        DefaultCamera = CameraDevice.Front
                    });


                    if (file != null)
                    {
                        // Get stream of selected image and send to service
                        _printService.PrintImage(file.GetStream());
                    }
                }
                else if (action.Equals(PickPhoto))
                {
                    if (!_mediaService.IsPickPhotoSupported)
                    {
                        await _pageDialogService.DisplayAlertAsync("No Photos", "No Photo permission", "Ok");
                        return;
                    }
                    var file = await _mediaService.PickPhotoAsync(new PickMediaOptions
                    {
                        PhotoSize = PhotoSize.Medium,

                    });

                    if (file != null)
                    {
                        // Get stream of selected image and send to service
                        _printService.PrintImage(file.GetStream());
                    }
                }
            }
            catch (MediaPermissionException ex)
            {
                Debug.WriteLine(ex.ToString());
                if (Device.RuntimePlatform == Device.iOS)
                {
                    var result = await _pageDialogService.DisplayAlertAsync("Permission Rejected", "No permission", "Enable", "Cancel");
                    if (result)
                    {
                        _permissionService.OpenAppSettings();
                    }
                }
                else
                {
                    await _pageDialogService.DisplayAlertAsync("Permission Rejected", "No permission", "Cancel");
                }
            }
        }

    }
}
