using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using static SoftwareVersionManager.Function;

namespace SoftwareVersionManager
{
    /// <summary>
    /// 软件版本管理器控制台 包括版本和验证
    /// </summary>
    public partial class Console : System.Web.UI.Page
    {
        // 部分代码借用来自 https://github.com/LorisYounger/WordWebCMS
        protected void Page_Load(object sender, EventArgs e)
        {
            string qus; string SessionID;
            if (MasterKey.Text == "" || Session[MasterKey.Text + "qus"] == null)
            {
                qus = RndQuestion(out int anser);
                SessionID = Rnd.Next().ToString("x");
                MasterKey.Text = SessionID;
                Session[SessionID + "qus"] = qus;
                Session[SessionID + "ans"] = anser;
            }
            else
            {
                SessionID = MasterKey.Text;
                qus = (string)Session[SessionID + "qus"];
                // = (int)Session[SessionID + "ans"];
            }

            CalloginKey.Text = qus;
            switch (Request.QueryString["Action"])
            {
                case "Register":
                    divregister.Visible = true;
                    if (AlowRegister == false)
                    {//如果不允许注册
                        errorboxregister.Visible = true;
                        errorboxregister.InnerHtml = "当前网站未开放注册,如需注册请联系网站管理员<br/>如已有账户,请<a href=\"?Action=Login\">前往登陆</a>";
                        buttonregister.Enabled = false;
                        CalregistKey.Enabled = false;
                        passwordreg.Enabled = false;
                    }
                    else if (EnabledEmail)
                    {
                        emailcheck.Visible = true;//只有开启发送邮件功能,会发送验证码

                    }
                    LHeader.Text = $"<title>{WebTitle} - 注册</title>";
                    break;
                default:
                    if (Session["User"] == null)
                    {
                        divlogin.Visible = true;
                        LHeader.Text = $"<title>{WebTitle} - 登录</title>";
                    }
                    else
                    {//已经登陆过了,直接跳转到用户信息页面

                    }
                    break;
            }
        }
        protected void buttonlogin_Click(object sender, EventArgs e)
        {
            if (MasterKey.Text == "" || Session[MasterKey.Text + "ans"] == null)
            //连接丢失,请重试
            {
                //Response.Redirect(Setting.WebsiteURL + "/login.aspx?" + SendERRORMSG("验证码缓存已丢失,请重试"));
                //Response.End();
                errorboxlogin.Visible = true;
                errorboxlogin.InnerText = "验证码缓存已丢失,请重试";
            }
            else if (int.TryParse(checkloginkey.Text, out int ans))
            {
                if ((int)Session[MasterKey.Text + "ans"] == ans)
                {
                    //全部正确,开始判断能否登陆
                    if (usernamelogin.Text == "")
                    {
                        errorboxlogin.Visible = true;
                        errorboxlogin.InnerText = "请输入账号";
                        return;
                    }
                    if (passwordlogin.Text == "")
                    {
                        errorboxlogin.Visible = true;
                        errorboxlogin.InnerText = "请输入密码";
                        return;
                    }

                    string ip = HttpContext.Current.Request.UserHostAddress;

                    if (Application["BAN" + ip] != null && (int)Application["BAN" + ip] > 11)
                    {
                        errorboxlogin.Visible = true;
                        errorboxlogin.InnerText = "由于错误次数过多,今日已无法重新尝试登陆";
                        return;//TODO:永久性的黑名单,使用数据库
                        //TODO:储存错误尝试到数据库,给后台看
                    }

                    Users usr = Users.GetUser(usernamelogin.Text);
                    if (usr == null)
                    {
                        errorboxlogin.Visible = true;
                        errorboxlogin.InnerHtml = "未找到账号信息,请检查输入或<a href=\"?Action=Register\">注册新账号</a>";
                        //由于涉及到了判断,清空验证码要求重新输入
                        string qus = RndQuestion(out int anser);
                        string SessionID = Rnd.Next().ToString("x");
                        MasterKey.Text = SessionID;
                        Session[SessionID + "qus"] = qus;
                        Session[SessionID + "ans"] = anser;
                        CalloginKey.Text = qus;

                        if (Application["BAN" + ip] == null)
                        {
                            Application["BAN" + ip] = 1;
                        }
                        else
                        {
                            Application["BAN" + ip] = (int)Application["BAN" + ip] + 1;
                        }
                        return;
                    }

                    if (!usr.PasswordCheck(passwordlogin.Text))
                    {
                        errorboxlogin.Visible = true;
                        errorboxlogin.InnerText = "密码输入错误,请检查输入";
                        //由于涉及到了判断,清空验证码要求重新输入
                        string qus = RndQuestion(out int anser);
                        string SessionID = Rnd.Next().ToString("x");
                        MasterKey.Text = SessionID;
                        Session[SessionID + "qus"] = qus;
                        Session[SessionID + "ans"] = anser;
                        CalloginKey.Text = qus;

                        if (Application["BAN" + ip] == null)//验证密码错误的惩罚多来点
                        {
                            Application["BAN" + ip] = 2;
                        }
                        else
                        {
                            Application["BAN" + ip] = (int)Application["BAN" + ip] + 2;
                        }
                        return;
                    }
                    //登陆成功: 把数据存Session

                    Session["User"] = usr;

                    Response.Write($"<script language='javascript'>alert('登陆成功!\\n欢迎回来,{usr.UserName}');window.location.href='{(Request.UrlReferrer == null || Request.UrlReferrer.ToString().ToLower().Contains("login.aspx") ? "\\index.aspx" : Request.UrlReferrer.ToString())}'</script>");
                }
                else
                {
                    //Response.Redirect(Setting.WebsiteURL + "/login.aspx?" + SendERRORMSG("验证码错误,请重新计算"));
                    //Response.End();
                    errorboxlogin.Visible = true;
                    errorboxlogin.InnerText = "验证码错误,请重新计算";
                }
            }
            else
            {
                errorboxlogin.Visible = true;
                errorboxlogin.InnerText = "验证码为纯数字,请检查输入";
            }
        }
    }
}