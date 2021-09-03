using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoftwareVersionManager
{
    /// <summary>
    /// 软件激活和校验
    /// </summary>
    public class Activation : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            context.Response.Write("Hello World");
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}