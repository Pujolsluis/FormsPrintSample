using System;
using System.IO;

namespace FormsPrintSample.Services
{
    public interface IPrintService
    {
        bool PrintImage(Stream img);
        bool PrintPdfFile(Stream file);
    }
}
