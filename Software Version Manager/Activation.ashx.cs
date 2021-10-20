using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoftwareVersion.Manager
{
    /// <summary>
    /// 软件激活和校验
    /// </summary>
    public class Activation : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            string soft = context.Request.QueryString["soft"];
            if (string.IsNullOrEmpty(soft))
            {
                context.Response.Write("ERROR_1#1:|Software name in need");
                return;
            }
            string mode = context.Request.QueryString["mode"];
            if (string.IsNullOrEmpty(mode))
            {
                context.Response.Write("ERROR_1#1:|Mode function in need");
                return;
            }
            string comp = context.Request.QueryString["comp"];
            if (string.IsNullOrEmpty(comp))
            {
                context.Response.Write("ERROR_1#1:|Computer code in need");
                return;
            }
            if (!int.TryParse(comp, System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out _))
            {
                context.Response.Write("ERROR_1#1:|Computer code is Int");
                return;
            }
            string veri = context.Request.QueryString["ver"];
            if (string.IsNullOrEmpty(veri))
            {
                context.Response.Write("ERROR_1#1:|Version in need");
                return;
            }
            if (!int.TryParse(veri, out int ver))
            {
                context.Response.Write("ERROR_1#1:|Computer code is Int64");
                return;
            }
            string code = context.Request.QueryString["code"];
            if (string.IsNullOrEmpty(comp))
            {
                context.Response.Write("ERROR_1#1:|activation Code in need");
                return;
            }
            //开始找代码
            ActivationCode actcode = ActivationCode.GetActivationCode(code);
            if (actcode == null)
            {
                context.Response.Write("ERROR_2#2:|Code No Find or Error Code");
                return;
            }
            switch (mode)
            {
                case "active":
                    //检查激活和软件是否一致
                    if (actcode.Software.Split(',').Contains(soft))
                    {
                        context.Response.Write("FALSE_3#131:|Not This SoftWare\n" + actcode.DataBuff.ToString());
                        return;
                    }
                    //检查版本
                    if (actcode.Verison != -1)
                    {
                        if (actcode.Verison < ver)
                        {
                            context.Response.Write("FALSE_1#129:|Version is Pass\n" + actcode.DataBuff.ToString());
                            return;
                        }
                    }
                    //检查有没有过期
                    if (actcode.Expiration.Ticks < DateTime.Now.Ticks)
                    {
                        context.Response.Write("FALSE_2#130:|Code is Expiration\n" + actcode.DataBuff.ToString());
                        return;
                    }
                    //如果已激活
                    if (actcode.ActivatedHave(comp))
                    {
                        context.Response.Write("TRUE_0#64:|Active by Have\n" + actcode.DataBuff.ToString());
                        return;
                    }
                    //如果激活次数已用完
                    if (actcode.Times != -1)
                    {
                        if (actcode.ActivatedNumber >= actcode.Times)
                        {
                            context.Response.Write("FALSE_0#128:|Computer is Full\n" + actcode.DataBuff.ToString());
                            return;
                        }
                        actcode.ActivatedADD(comp);
                    }
                    context.Response.Write("TRUE_1#65:|Active by New\n" + actcode.DataBuff.ToString());
                    return;
                case "info":
                    context.Response.Write(actcode.DataBuff.ToString());
                    return;
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