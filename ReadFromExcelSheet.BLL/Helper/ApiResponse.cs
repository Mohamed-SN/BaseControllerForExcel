using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ReadFromExcelSheet.BLL.Helper
{
    public class ApiResponse<T>
    {
        public HttpStatusCode Status { get; set; }
        public string Message { get; set; } = "";
        public T? Data { get; set; }
        public string[] Errors { get; set; }

        public ApiResponse(HttpStatusCode Status, string Message, T Data, string[] Errors = null)
        {
            this.Status = Status;
            this.Message = Message;
            this.Data = Data;
            this.Errors = Errors;
        }

        // localized message key
        public ApiResponse(HttpStatusCode status, string messageKey, IStringLocalizer localizer, T data, string[] errors = null)
        {
            Status = status;
            Message = localizer[messageKey] ?? messageKey;
            Data = data;
            Errors = errors ?? new string[0];
        }

    }
}
