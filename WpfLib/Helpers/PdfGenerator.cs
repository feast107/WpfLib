using System.IO;
using System.IO.Packaging;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Xps;
using System.Windows.Xps.Packaging;
using PdfSharp.Xps;
namespace WpfLib.Helpers
{
    public static class PdfGenerator
    {
        public static void FromVisual(Visual visual,string path)
        {
            MemoryStreamToPdf(path, Visual2MemoryStream(visual));
        }
        private static MemoryStream Visual2MemoryStream(Visual visual)
        {
            MemoryStream lMemoryStream = new ();
            using Package package = Package.Open(lMemoryStream, FileMode.Create);
            using XpsDocument doc = new(package);
            XpsDocumentWriter writer = XpsDocument.CreateXpsDocumentWriter(doc);
            VisualsToXpsDocument vToXpsD = (VisualsToXpsDocument)writer.CreateVisualsCollator();
            vToXpsD?.Write(visual);
            vToXpsD?.EndBatchWrite();
            return lMemoryStream;
        }
        private static void MemoryStreamToPdf(string path, MemoryStream lMemoryStream)
        {
            var pdfXpsDoc = PdfSharp.Xps.XpsModel.XpsDocument.Open(lMemoryStream);
            if (string.IsNullOrWhiteSpace(path))
            {
                path = Path.GetFullPath("Anonymous.pdf");
            }
            XpsConverter.Convert(pdfXpsDoc, path, 0);
        }
    }
}
