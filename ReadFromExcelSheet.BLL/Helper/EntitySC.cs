using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadFromExcelSheet.BLL.Helper
{
    public class EntitySC
    {
        public const int MaxPages = 10000;
        private int pageIndex;

        public int PageIndex
        {
            get { return pageIndex; }
            set { pageIndex = value < 1 ? 1 : value; }
        }

        private int pageSize;

        public int PageSize
        {
            get { return pageSize; }
            set { pageSize = value > 10000 ? 10000 : value; }
        }


        public bool SortIsAsc { get; set; }
        public string SortCol { get; set; }
        public bool WithDetails { get; set; }
        public bool? IsDelete { get; set; }
        public int? Id { get; set; }

        public short? CompanyId { get; set; }
        public EntitySC()
        {
            pageIndex = 1;
            pageSize = 10;
            SortIsAsc = true;
            SortCol = "id";
            WithDetails = false;
            IsDelete = false;
        }

    }
    public class SuperAdminFilter
    {
        public bool? IsDelete { get; set; }
        public short? CompanyId { get; set; }
        public int? EmployeeId { get; set; }
    }
}
