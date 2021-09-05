using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Cryptography;
using System.Text;
using LinePutScript.SQLHelper;
using System.Configuration;
using LinePutScript;

namespace SoftwareVersion.Manager
{
    public static class Function
    {
        /// <summary>
        /// 从数据库获得元数据
        /// </summary>
        public static LpsDocument Data
        {
            //get => new LpsDocument();//Debug
            get
            {
            ReTry: try
                {
                    return RAW.ExecuteQuery("SELECT * FROM setting");
                }
                catch
                {
                    System.Threading.Thread.Sleep(1000);
                    goto ReTry;
                }
            }
        }
        /// <summary>
        /// 从缓存中获取的元数据
        /// </summary>
        public static LpsDocument DataBuff //用于读取用 如果要写啥 禁止使用这个 需手动获取最新数据
        {
            get
            {
                if (databf == null)
                {
                    databf = Data;
                }
                return databf;
            }
            set
            {
                databf = null;
            }
        }
        private static LpsDocument databf;//用于读取用 如果要写啥 禁止使用这个 需手动获取最新数据
        /// <summary>
        /// MD5字符串加盐
        /// </summary>
        /// <param name="txt">需要加密的文本</param>
        /// <returns>加密后字符串</returns>
        public static string MD5salt(string txt)
        {
            txt = txt.GetHashCode().ToString() + txt + txt.GetHashCode().ToString();
            using (MD5 mi = MD5.Create())
            {
                byte[] buffer = Encoding.Default.GetBytes(txt);
                //开始加密
                byte[] newBuffer = mi.ComputeHash(buffer);
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < newBuffer.Length; i++)
                {
                    sb.Append(newBuffer[i].ToString("x2"));
                }
                return sb.ToString();
            }
        }
        public static Random Rnd = new Random();
        /// <summary>
        /// 随机生成数学题
        /// </summary>
        public static string RndQuestion(out int anser)
        {
            int a, b; string str;
            if (Rnd.Next(2) == 0)
            {
                a = Rnd.Next(-20, 20); b = Rnd.Next(0, 20);
                if (Rnd.Next(2) == 0)
                {
                    anser = a + b;
                    str = a + " + " + b;
                }
                else
                {
                    anser = a - b;
                    str = a + " - " + b;
                }
            }
            else
            {
                a = Rnd.Next(-10, 10); b = Rnd.Next(-10, 10);
                anser = a * b;
                str = a + " x " + b;
            }
            return str + " = ";
        }

        /// <summary>
        /// 返回一个随机的64位long
        /// </summary>
        public static long RndLong()
        {
            //Min:1152921504606846976
            //Max:9223372036854775807
            byte[] salt = new byte[8];
            Rnd.NextBytes(salt);
            return BitConverter.ToInt64(salt, 0);
        }
        /// <summary>
        /// 用户数据 兼容WWCMS
        /// </summary>
        public static MySQLHelper RAWUser = new MySQLHelper(ConfigurationManager.ConnectionStrings["connUsrStr"].ConnectionString);        
        /// <summary>
        /// 该系统的全部数据
        /// </summary>
        public static MySQLHelper RAW = new MySQLHelper(ConfigurationManager.ConnectionStrings["connStr"].ConnectionString);



        #region 用户权限信息处理
        /// <summary>
        /// 获得用户的权限辨识
        /// </summary>
        public static AuthLevel GetUserAuthority(int uID)
        {
            Line lin = DataBuff.Assemblage.Find(x => x.info == "auth_" + uID.ToString());
            if (lin == null)
                return UserAuthorityDefault;
            return (AuthLevel)Convert.ToSByte(lin.First().info);
        }//set => RAW.ExecuteNonQuery($"UPDATE user SET auth=@auth WHERE Uid=@uid", new MySQLHelper.Parameter("auth", (byte)value), new MySQLHelper.Parameter("uid", uID));
        /// <summary>
        /// 获得用户的权限辨识
        /// </summary>
        public static void SetUserAuthority(int uID, AuthLevel auth)
        {
            Line lin = DataBuff.Assemblage.Find(x => x.info == "auth_" + uID.ToString());
            if (lin == null)
            {
                RAW.ExecuteNonQuery($"INSERT INTO setting VALUES (@sele,@prop)", new MySQLHelper.Parameter("sele", "auth_" + uID.ToString()), new MySQLHelper.Parameter("prop", ((short)auth).ToString()));
            }
            else
                RAW.ExecuteNonQuery($"UPDATE setting SET property=@prop WHERE selector=@sele", new MySQLHelper.Parameter("sele", "auth_" + uID.ToString()), new MySQLHelper.Parameter("prop", ((short)auth).ToString()));
        }
        /// <summary>
        /// 新用户默认角色
        /// </summary>
        public static AuthLevel UserAuthorityDefault
        {
            get
            {
                var line = DataBuff.Assemblage.Find(x => x.info == "defaultuserauth");
                if (line == null)
                    return AuthLevel.Default;
                return (AuthLevel)Convert.ToSByte(line.First().info);
            }
            set
            {
                Line lin = DataBuff.Assemblage.Find(x => x.info == "defaultuserauth");
                if (lin == null)
                {
                    RAW.ExecuteNonQuery($"INSERT INTO setting VALUES (@sele,@prop)", new MySQLHelper.Parameter("sele", "defaultuserauth"), new MySQLHelper.Parameter("prop", ((short)value).ToString()));
                }
                else
                    RAW.ExecuteNonQuery($"UPDATE setting SET property=@prop WHERE selector=@sele", new MySQLHelper.Parameter("sele", "defaultuserauth"), new MySQLHelper.Parameter("prop", ((short)value).ToString()));
                DataBuff = null;
            }
        }
        /// <summary>
        /// 用户权限类别
        /// </summary>
        public enum AuthLevel : byte
        {
            /// <summary>
            /// 封禁用户 - 无法登陆
            /// </summary>
            Ban = 0,
            /// <summary>
            /// 默认权限
            /// </summary>
            Default = 1,
            /// <summary>
            /// 无权限
            /// </summary>
            None = 2,
            /// <summary>
            /// 普通顾客 - 可以激活软件
            /// </summary>
            Consumers = 3,
            /// <summary>
            /// 高级顾客 - 可以激活软件 有更多功能(需要分配)
            /// </summary>
            ImportantConsumers = 4,
            /// <summary>
            /// 重要顾客 - 可以激活软件 有更多功能(需要分配)
            /// </summary>
            VeryImportantConsumers = 5,
            /// <summary>
            /// 激活工具用户 - 如果不使用用户系统,可以通过创建这种激活工具账户分配激活秘钥
            /// </summary>
            Activation = 6,
            /// <summary>
            /// 维护人员 - 可以修改和发布版本号
            /// </summary>
            Maintenance = 7,           
            /// <summary>
            /// 秘钥管理员 - 可以生成可用秘钥
            /// </summary>
            PostManager = 8,
            /// <summary>
            /// 管理员 - 最高权限 可以改任何东西
            /// </summary>
            Admin = 9,

            
        }
        #endregion

        /// <summary>
        /// 网站标题
        /// </summary>
        public static string WebTitle
        {
            get
            {
                var line = DataBuff.Assemblage.Find(x => x.info == "webtitle");
                if (line != null)
                    return line.First().Info;
                return "软件版本管理器";
            }
            set
            {
                Line lin = DataBuff.Assemblage.Find(x => x.info == "webtitle");
                if (lin == null)
                {
                    RAW.ExecuteNonQuery($"INSERT INTO setting VALUES (@sele,@prop)", new MySQLHelper.Parameter("sele", "webtitle"), new MySQLHelper.Parameter("prop", value));
                }
                else
                    RAW.ExecuteNonQuery($"UPDATE setting SET property=@prop WHERE selector=@sele", new MySQLHelper.Parameter("sele", "webtitle"), new MySQLHelper.Parameter("prop", value));
                DataBuff = null;
            }
        }
        /// <summary>
        /// 网站副标题
        /// </summary>
        public static string WebSubTitle
        {
            get
            {
                var line = DataBuff.Assemblage.Find(x => x.info == "websubtitle");
                if (line != null)
                    return line.First().Info;
                return "支持管理软件版本,软件更新,激活服务等 by LorisYounger";
            }
            set
            {
                Line lin = DataBuff.Assemblage.Find(x => x.info == "websubtitle");
                if (lin == null)
                {
                    RAW.ExecuteNonQuery($"INSERT INTO setting VALUES (@sele,@prop)", new MySQLHelper.Parameter("sele", "websubtitle"), new MySQLHelper.Parameter("prop", value));
                }
                else
                    RAW.ExecuteNonQuery($"UPDATE setting SET property=@prop WHERE selector=@sele", new MySQLHelper.Parameter("sele", "websubtitle"), new MySQLHelper.Parameter("prop", value));
                DataBuff = null;
            }
        }
        /// <summary>
        /// 网站URL
        /// </summary>
        public static string WebsiteURL
        {
            get
            {
                var line = DataBuff.Assemblage.Find(x => x.info == "websiteurl");
                if (line != null)
                    return line.First().Info;
                return HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);//HttpContext.Current.Request.Url.Host
            }
            set
            {
                Line lin = DataBuff.Assemblage.Find(x => x.info == "websideurl");
                if (lin == null)
                {
                    RAW.ExecuteNonQuery($"INSERT INTO setting VALUES (@sele,@prop)", new MySQLHelper.Parameter("sele", "websiteurl"), new MySQLHelper.Parameter("prop", value));
                }
                else
                    RAW.ExecuteNonQuery($"UPDATE setting SET property=@prop WHERE selector=@sele", new MySQLHelper.Parameter("sele", "websiteurl"), new MySQLHelper.Parameter("prop", value));
                DataBuff = null;
            }
        }
        /// <summary>
        /// 允许注册
        /// </summary>
        public static bool AlowRegister
        {
            get
            {
                var line = DataBuff.Assemblage.Find(x => x.info == "alowregister");
                if (line != null)
                    return Convert.ToBoolean(line.First().info);
                return true;//默认允许注册
            }
            set
            {
                Line lin = DataBuff.Assemblage.Find(x => x.info == "alowregister");
                if (lin == null)
                {
                    RAW.ExecuteNonQuery($"INSERT INTO setting VALUES (@sele,@prop)", new MySQLHelper.Parameter("sele", "alowregister"), new MySQLHelper.Parameter("prop", value));
                }
                else
                    RAW.ExecuteNonQuery($"UPDATE setting SET property=@prop WHERE selector=@sele", new MySQLHelper.Parameter("sele", "alowregister"), new MySQLHelper.Parameter("prop", value));
                DataBuff = null;
            }
        }
        /// <summary>
        /// 指示有无启用邮件功能 需要设置正确的SMTP才能使用
        /// </summary>
        public static bool EnabledEmail
        {
            get
            {
                var line = DataBuff.Assemblage.Find(x => x.info == "enabledemail");
                if (line != null)
                    return Convert.ToBoolean(line.First().info);
                return false;//默认未开启邮件功能
            }
            set
            {
                Line lin = DataBuff.Assemblage.Find(x => x.info == "enabledemail");
                if (lin == null)
                {
                    RAW.ExecuteNonQuery($"INSERT INTO setting VALUES ('enabledemail',@prop)", new MySQLHelper.Parameter("prop", value));
                }
                else
                    RAW.ExecuteNonQuery($"UPDATE setting SET property=@prop WHERE selector='enabledemail'", new MySQLHelper.Parameter("prop", value));
                DataBuff = null;
            }
        }

        /// <summary>
        /// 邮箱SMTP使用的邮箱名
        /// </summary>
        public static string SMTPEmail
        {
            get
            {
                var line = DataBuff.Assemblage.Find(x => x.info == "smtpemail");
                if (line != null)
                    return line.First().Info;
                return "SMTPEmail";
            }
            set
            {
                Line lin = DataBuff.Assemblage.Find(x => x.info == "smtpemail");
                if (lin == null)
                {
                    RAW.ExecuteNonQuery($"INSERT INTO setting VALUES ('smtpemail',@prop)", new MySQLHelper.Parameter("prop", value));
                }
                else
                    RAW.ExecuteNonQuery($"UPDATE setting SET property=@prop WHERE selector='smtpemail'", new MySQLHelper.Parameter("prop", value));
                DataBuff = null;
            }
        }
        /// <summary>
        /// 邮箱SMTP使用的密码
        /// </summary>
        public static string SMTPPassword
        {
            get
            {
                var line = DataBuff.Assemblage.Find(x => x.info == "smtppassword");
                if (line != null)
                    return line.First().Info;
                return "SMTPPassword";
            }
            set
            {
                Line lin = DataBuff.Assemblage.Find(x => x.info == "smtppassword");
                if (lin == null)
                {
                    RAW.ExecuteNonQuery($"INSERT INTO setting VALUES ('smtppassword',@prop)", new MySQLHelper.Parameter("prop", value));
                }
                else
                    RAW.ExecuteNonQuery($"UPDATE setting SET property=@prop WHERE selector='smtppassword'", new MySQLHelper.Parameter("prop", value));
                DataBuff = null;
            }
        }
        /// <summary>
        /// 邮箱SMTP服务器的链接
        /// </summary>
        public static string SMTPURL
        {
            get
            {
                var line = DataBuff.Assemblage.Find(x => x.info == "smtpurl");
                if (line != null)
                    return line.First().Info;
                return "SMTPURL";
            }
            set
            {
                Line lin = DataBuff.Assemblage.Find(x => x.info == "smtpurl");
                if (lin == null)
                {
                    RAW.ExecuteNonQuery($"INSERT INTO setting VALUES ('smtpurl',@prop)", new MySQLHelper.Parameter("prop", value));
                }
                else
                    RAW.ExecuteNonQuery($"UPDATE setting SET property=@prop WHERE selector='smtpurl'", new MySQLHelper.Parameter("prop", value));
                DataBuff = null;
            }
        }
    }
}
