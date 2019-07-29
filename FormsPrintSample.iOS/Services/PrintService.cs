using System;
using System.IO;
using CoreGraphics;
using FormsPrintSample.Services;
using Foundation;
using UIKit;


namespace FormsPrintSample.iOS.Services
{
    public class PrintService : IPrintService
    {
        public bool PrintImage(Stream img)
        {
            var data = NSData.FromStream(img);
            var uiimage = UIImage.LoadFromData(data);

            var printer = UIPrintInteractionController.SharedPrintController;

            if (printer == null)
            {
                Console.WriteLine("Unable to print at this time.");
            }
            else
            {

                var printInfo = UIPrintInfo.PrintInfo;

                printInfo.OutputType = UIPrintInfoOutputType.General;

                printInfo.JobName = "Print Job Name";

                printer.PrintInfo = printInfo;

                printer.PrintingItem = uiimage;

                printer.ShowsPageRange = true;



                var handler = new UIPrintInteractionCompletionHandler((printInteractionController, completed, error) =>
                {
                    if (completed)
                    {
                        Console.WriteLine("Print Completed.");
                    }
                    else if (!completed && error != null)
                    {
                        Console.WriteLine("Error Printing.");
                    }

                });

                CGRect frame = new CGRect();
                frame.Size = uiimage.Size;

                printer.Present(true, handler);
            }

            return true;
        }

        public bool PrintPdfFile(Stream file)
        {
            var printInfo = UIPrintInfo.PrintInfo;
            printInfo.OutputType = UIPrintInfoOutputType.General;
            printInfo.JobName = "Print PDF";

            //Get the path of the MyDocuments folder
            var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            //Get the path of the Library folder within the MyDocuments folder
            var library = Path.Combine(documents, "..", "Library");
            //Create a new file with the input file name in the Library folder
            var filepath = Path.Combine(library, "PrintSampleFile");

            //Write the contents of the input file to the newly created file
            using (MemoryStream tempStream = new MemoryStream())
            {
                file.Position = 0;
                file.CopyTo(tempStream);
                File.WriteAllBytes(filepath, tempStream.ToArray());
            }

            var printer = UIPrintInteractionController.SharedPrintController;
            printInfo.OutputType = UIPrintInfoOutputType.General;

            printer.PrintingItem = NSUrl.FromFilename(filepath);
            printer.PrintInfo = printInfo;


            printer.ShowsPageRange = true;

            printer.Present(true, (handler, completed, err) => {
                if (!completed && err != null)
                {
                    Console.WriteLine("error");
                }
            });
            file.Dispose();
            return true;
        }
    }
}
