using OfficeOpenXml;
using OfficeOpenXml.Drawing;
using ReadFromExcelSheet.BLL.Interface;

namespace ReadFromExcelSheet.Utiltes
{
    public static class Utilites
    {

        public static T MapRowToDto<T>(ExcelWorksheet worksheet, int row, List<byte[]> images, IFileService fileService) where T :class, new()
        {
            var obj = new T();
            var props = typeof(T).GetProperties();

            for (int col = 1; col <= props.Length; col++)
            {
                var prop = props[col - 1];
                var cellValue = worksheet.Cells[row, col].Text;

                if (prop.PropertyType == typeof(string))
                {
                    prop.SetValue(obj, cellValue);
                }
                else if (prop.PropertyType == typeof(int))
                {
                    prop.SetValue(obj, int.TryParse(cellValue, out var intValue) ? intValue : 0);
                }
                else if (prop.PropertyType == typeof(decimal))
                {
                    prop.SetValue(obj, decimal.TryParse(cellValue, out var decimalValue) ? decimalValue : 0);
                }
                else if (prop.PropertyType == typeof(float))
                {
                    prop.SetValue(obj, float.TryParse(cellValue, out var floatValue) ? floatValue : 0);
                }
                else if (prop.PropertyType == typeof(byte[]) && prop.Name.ToLower() == "profilepicture")
                {
                    // Assuming that the image is stored in the 'images' list corresponding to the row number
                    int imageIndex = row - 2; // Image index is based on row number (adjust if needed)

                    if (images != null && imageIndex >= 0 && imageIndex < images.Count)
                    {
                        var imageBytes = images[imageIndex]; // Use the byte[] image
                        prop.SetValue(obj, imageBytes); // Assign the byte[] to the property
                    }
                    else
                    {
                        prop.SetValue(obj, Array.Empty<byte>());
                    }
                }
                else if (prop.PropertyType == typeof(byte[]) && prop.Name.ToLower() == "logo")
                {
                    // Assuming that the image is stored in the 'images' list corresponding to the row number
                    int imageIndex = row - 2; // Image index is based on row number (adjust if needed)

                    if (images != null && imageIndex >= 0 && imageIndex < images.Count)
                    {
                        var imageBytes = images[imageIndex]; // Use the byte[] image
                        prop.SetValue(obj, imageBytes); // Assign the byte[] to the property
                    }
                    else
                    {
                        prop.SetValue(obj, Array.Empty<byte>());
                    }
                }
            }

            return obj;
        }


    }
}
