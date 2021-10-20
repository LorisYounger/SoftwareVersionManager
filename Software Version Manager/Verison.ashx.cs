using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoftwareVersion.Manager
{
    /// <summary>
    /// 所有软件验证版本信息
    /// </summary>
    public class Verison : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            string soft = context.Request.QueryString["soft"];
            if (soft == null)
            {
                context.Response.Write("Software Name in need");
                return;
            }

            //查询软件更新版本
            var vlist = VerisonManager.GetVerisonManager(soft);
            if (vlist.Count() == 0)
            {
                context.Response.Write("No Software Found");
                return;
            }
            
            string type = context.Request.QueryString["type"];
            if (type == null)
                type = "plain";
            switch (type.ToLower())
            {
                case "lps":
                case "raw":
                    foreach (var v in vlist)
                    {
                        context.Response.Write(v.DataBuff.ToString()+'\n');
                    }
                    break;
                default:
                case "view":
                    //TODO一个更好看的界面
                    break;
                case "plain":
                    foreach (var v in vlist)
                    {
                        context.Response.Write($"{v.Verison} {v.Illustration.Split('\n')[0]} \n");
                    }
                    break;
            }
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