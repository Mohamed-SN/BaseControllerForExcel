using OfficeOpenXml;
using OfficeOpenXml.Drawing;

namespace ReadFromExcelSheet.Utiltes
{
    public static class Utilites
    {
        public static T MapRowToDto<T>(ExcelWorksheet worksheet, int row, List<ExcelPicture> pictures = null) where T : new()
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
                else if(prop.PropertyType == typeof(decimal))
                {
                    prop.SetValue(obj, decimal.TryParse(cellValue, out var decimalValue) ?decimalValue:0);
                }

                else if (prop.PropertyType == typeof(float))
                {
                    prop.SetValue(obj, float.TryParse(cellValue, out var floatValue) ? floatValue : 0);
                }
                
                else if (prop.PropertyType == typeof(byte[]) && prop.Name == "ProfilePicture")
                {
                    var pictureBytes = pictures?
                        .FirstOrDefault(p => p.From.Row == row - 1)?.Image.ImageBytes;

                    prop.SetValue(obj, pictureBytes ?? Array.Empty<byte>());
                } 
            }

            return obj;
        }

    }
}
