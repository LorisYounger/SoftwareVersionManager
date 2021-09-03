using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoftwareVersionManager
{
    /// <summary>
    /// 所有软件验证版本信息
    /// </summary>
    public class Verison : IHttpHandler
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