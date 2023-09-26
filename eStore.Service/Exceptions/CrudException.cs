using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace eStore.Service.Exceptions
{
    public class CrudException : Exception
    {
        public HttpStatusCode StatusCode { get; set; }
        public string Error { get; set; }

        public CrudException(HttpStatusCode statusCode, string msg, string error) : base(msg)
        {
            StatusCode = statusCode;
            Error = error;
        }
    }
}
