using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadFromExcelSheet.BLL.Helper
{
    public class Pagination<Entity>
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalRows { get; set; }
        public int TotalPages { get; set; }
        public List<Entity> List { get; set; }
        public Pagination()
        {
            List = new List<Entity>();
        }
        public Pagination(int PageIndex, int PageSize, int TotalRows, int TotalPages, List<Entity> List)
            : this()
        {
            this.PageIndex = PageIndex;
            this.PageSize = PageSize;
            this.TotalRows = TotalRows;
            this.TotalPages = TotalPages;
            this.List = List;
        }
    }
}
