using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace VarnetFileService.Entity
{
    public class HttpResponseEntity
    {
        public HttpStatusCode Status { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
    }
}