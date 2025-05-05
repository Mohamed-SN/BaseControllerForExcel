using DocumentFormat.OpenXml.Packaging;
using System.IO;
using System.Collections.Generic;
using DocumentFormat.OpenXml.Experimental;

namespace ReadFromExcelSheet.Utilites
{
    public static class ExcelImageExtractor
    {
        public static List<byte[]> ExtractImagesByOrder(string filePath)
        {
            var images = new List<byte[]>();

            using (SpreadsheetDocument document = SpreadsheetDocument.Open(filePath, false))
            {
                var package = document.GetPackage();

                foreach (var part in package.GetParts())
                {
                    if (part.ContentType.StartsWith("image/"))
                    {
                        using (var stream = part.GetStream(FileMode.Open, FileAccess.Read))
                        using (var ms = new MemoryStream())
                        {
                            stream.CopyTo(ms);
                            images.Add(ms.ToArray());
                        }
                    }
                }
            }

            return images;
        }
    }
}
