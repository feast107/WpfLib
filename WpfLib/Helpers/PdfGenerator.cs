using System.IO;
using System.IO.Packaging;
using System.Windows.Media;
using System.Windows.Xps;
using PdfSharp.Xps;

using WXD = System.Windows.Xps.Packaging.XpsDocument;
using PXD = PdfSharp.Xps.XpsModel.XpsDocument;

namespace WpfLib.Helpers
{
    public static class PdfGenerator
    {
        public static void FromVisual(Visual visual,string path)
        {
            using var ms = new MemoryStream();
            using (var package = Package.Open(ms, FileMode.Create))
            {
                using var doc = new WXD(package);
                var writer = WXD.CreateXpsDocumentWriter(doc);
                var vToXpsD = (VisualsToXpsDocument)writer.CreateVisualsCollator();
                vToXpsD?.Write(visual);
                vToXpsD?.EndBatchWrite();
            }
            using var document = PXD.Open(ms);
            if (string.IsNullOrWhiteSpace(path))
            {
                path = Path.GetFullPath("Anonymous.pdf");
            }
            XpsConverter.Convert(document, path, 0);
        }
    }
}
