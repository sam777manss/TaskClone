using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CommonModel.Enum.Enum;

namespace CommonModel
{
    public class ApiResponseModel
    {
        public ApiResponseModel()
        {
            Code = (int)ResponseStatusCode.OK;
        }
        public string Message { get; set; } = string.Empty;

        public int Code { get; set; }
        public bool IsSuccess { get; set; }

        public dynamic Data { get; set; }
    }
}
