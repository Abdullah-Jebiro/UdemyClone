using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.ResponseModels
{
    public class BaseResponse<T>
    {

        public BaseResponse()
        {
          
        }

        public BaseResponse(T data, string message = null)
        {
            Message = message;
            Data = data;
        }
   
        public string Message { get; set; }
        public T Data { get; set; }
    }
}
