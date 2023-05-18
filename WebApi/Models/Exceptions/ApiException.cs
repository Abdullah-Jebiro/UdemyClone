using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Text;

namespace Models.Exceptions
{
    public class ApiException : Exception 
    {
        public ApiException() : base() { }

        public ApiException(string message) : base(message) { }
        public ApiException(HttpStatusCode StatusCode , string message) : base(message) {
            this.StatusCode = (int)StatusCode;
        }

      
        public int StatusCode { get; set; }
    }
}
