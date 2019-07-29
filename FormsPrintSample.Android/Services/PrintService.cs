using System;
using System.IO;
using FormsPrintSample.Services;
using Android.Content;
using Android.Graphics;
using Android.Print;
using Android.Support.V4.Print;
using Plugin.CurrentActivity;
using Xamarin.Forms;
using System.Diagnostics;

namespace FormsPrintSample.Droid.Services
{
    public class PrintService : IPrintService
    {
        public PrintService()
        {
        }

        public bool PrintImage(Stream img)
        {
            PrintHelper photoPrinter = new PrintHelper(CrossCurrentActivity.Current.Activity);
            photoPrinter.ScaleMode = PrintHelper.ScaleModeFit;
            Bitmap bitmap = BitmapFactory.DecodeStream(img);
            photoPrinter.PrintBitmap("PrintSampleImg.jpg", bitmap);

            return true;
        }

        public bool PrintPdfFile(Stream file)
        {
            try
            {
                if (file.CanSeek)
                    //Reset the position of PDF document stream to be printed
                    file.Position = 0;
                //Create a new file in the Personal folder with the given name
                string createdFilePath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "PrintSampleFile");
                //Save the stream to the created file
                using (var dest = System.IO.File.OpenWrite(createdFilePath))
                    file.CopyTo(dest);
                string filePath = createdFilePath;
                PrintManager printManager = (PrintManager)CrossCurrentActivity.Current.Activity.GetSystemService(Context.PrintService);
                PrintDocumentAdapter pda = new CustomPrintDocumentAdapter(filePath);
                //Print with null PrintAttributes
                printManager.Print("PrintSampleFile Job", pda, null);
                file.Dispose();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            return true;
        }
    }
}
