using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Text;

namespace Models.Exceptions
{
    public class ApiException : Exception
    {

        public int StatusCode { get; set; }
        public ApiException() : base() { }

        public ApiException(HttpStatusCode StatusCode, string message=null)
              : base(message ?? new ApiException().GetDefaultMessageForStatusCode((int)StatusCode))
        {
            this.StatusCode = (int)StatusCode;
        }


        private string GetDefaultMessageForStatusCode(int statusCode)
        {
            return statusCode switch
            {
                400 => "A bad request, you have made",
                401 => "Authorized, you are not",
                404 => "Resource found, it was not",
                500 => "Errors are the path to the dark sid. Errors lead to anger. Anger leads to hate. Hate leads to career change",
                _ => null
            };
        }
    }
}
