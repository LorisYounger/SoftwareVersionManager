using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using static SoftwareVersion.Manager.Function;

namespace SoftwareVersion.Manager
{
    /// <summary>
    /// Console 的摘要说明
    /// </summary>
    public class API : IHttpHandler
    {
        public bool IsReusable => false;

        public void ProcessRequest(HttpContext context)
        {
            if (context.Request.QueryString["usr"] == null)
                return;
            context.Response.ContentType = "text/plain";

            //IP封禁
            string ip = HttpContext.Current.Request.UserHostAddress;
            if (context.Application["BAN" + ip] != null && (int)context.Application["BAN" + ip] > 11)
            {
                context.Response.Write("由于错误次数过多,今日已无法重新尝试登陆");
                return;//TODO:永久性的黑名单,使用数据库
                       //TODO:储存错误尝试到数据库,给后台看
            }

            Users usr = Users.GetUser(context.Request.QueryString["usr"]);
            if (usr == null)
            {
                context.Response.Write("未找到账号信息");

                if (context.Application["BAN" + ip] == null)
                {
                    context.Application["BAN" + ip] = 2;
                }
                else
                {
                    context.Application["BAN" + ip] = (int)context.Application["BAN" + ip] + 2;
                }
                return;
            }

            if (!usr.PasswordCheck(context.Request.QueryString["pas"]))
            {
                context.Response.Write("密码输入错误,请检查输入");
                if (context.Application["BAN" + ip] == null)//验证密码错误的惩罚多来点
                {
                    context.Application["BAN" + ip] = 4;
                }
                else
                {
                    context.Application["BAN" + ip] = (int)context.Application["BAN" + ip] + 4;
                }
                return;
            }
            switch (context.Request.QueryString["action"]?.ToLower())
            {
                case "create":
                    CreateKey(context, usr);
                    break;
                default:
                    context.Response.Write("暂无该功能");
                    break;
            }
        }

        public void CreateKey(HttpContext context, Users usr)
        {
            if (usr.Authority != AuthLevel.Admin)
            {
                context.Response.Write("无权限");
                return;
            }
            string software = context.Request.QueryString["soft"];
            if (string.IsNullOrEmpty(software))
            {
                context.Response.Write("ERROR: No SoftwareName");
                return;
            }
            if (!short.TryParse(context.Request.QueryString["times"] ?? "5", out short times))
            {
                context.Response.Write("ERROR: Times Must be short");
                return;
            }
            if (!int.TryParse(context.Request.QueryString["uid"] ?? "-1", out int uid))
            {
                context.Response.Write("ERROR: Uid Must be Int");
                return;
            }
            if (!int.TryParse(context.Request.QueryString["ver"] ?? "-1", out int ver))
            {
                context.Response.Write("ERROR: Version Must be Int");
                return;
            }
            if (!DateTime.TryParse(context.Request.QueryString["exp"] ?? "2099/01/01", out DateTime exp))
            {
                context.Response.Write("ERROR: Expiration Date Must be DateTime");
                return;
            }
            context.Response.Write(ActivationCode.CreatActivationCode(software, ver, uid, exp, times, context.Request.QueryString["ill"] ?? "", usr.UserName + ':' + context.Request.QueryString["mark"] ?? "").CodeHEX);
        }
    }
}