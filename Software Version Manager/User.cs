using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LinePutScript;
using LinePutScript.SQLHelper;
using static Software_Version_Manager.Function;

namespace Software_Version_Manager
{
    //用户表使用的是wwcms同款用户表,可以与wwcms兼容
    //用户表格式:
    //   -users
    //     Uid(int)|name(string30)|email(string40)|password(string32)|isroot(bool)|money(int)|exp(int)|lv(int)|headport(tinytext)|
    //      用户id  |    用户名    |   电子邮件    |      密码md5s     |是超级管理员 |   金钱   | 经验值 | 等级  |      头像url     |
    //     主键递增|  --------------------------------------------   |  false     |   0      |  0     | 1     |        空        |
    //建议手动在sql新建一个账户作为超级管理员=>admin
    public class Users
    {
        #region "辅助构建函数"
        /// <summary>
        /// 通过用户名或者用户邮箱获得用户信息
        /// </summary>
        /// <param name="UserName">用户名称或者用户邮箱</param>
        /// <returns></returns>
        public static Users GetUser(string UserName)
        {
            LpsDocument data = RAWUser.ExecuteQuery("SELECT * FROM users WHERE name=@usrname OR email=@usrname", new MySQLHelper.Parameter("usrname", UserName));
            if (data.Assemblage.Count == 0)
                return null;
            return new Users(data.First());
        }
        /// <summary>
        /// 通过用户id获得用户信息
        /// </summary>
        /// <param name="UserName">用户id</param>
        public static Users GetUser(int UserID)
        {
            LpsDocument data = RAWUser.ExecuteQuery("SELECT * FROM users WHERE Uid=@uid", new MySQLHelper.Parameter("uid", UserID));
            if (data.Assemblage.Count == 0)
                return null;
            return new Users(UserID, data.First());
        }
        /// <summary>
        /// 创建用户 此方法并没有检查是否存在该用户
        /// </summary>
        public static Users CreatUser(string UserName, string Email, string Password)
        {//注意 此方法并没有检查是否存在该用户
            RAWUser.ExecuteNonQuery($"INSERT INTO users VALUES (NULL,@usrname,@email,@pw)", new MySQLHelper.Parameter("usrname", UserName),
                new MySQLHelper.Parameter("email", Email), new MySQLHelper.Parameter("pw", Function.MD5salt(Password)));
            return GetUser(RAW.ExecuteQuery("select LAST_INSERT_ID()").First().InfoToInt);
        }
        #endregion

        #region 数据库Data

        /// <summary>
        /// 从数据库获得元数据
        /// </summary>
        public Line Data
        {
            get => RAWUser.ExecuteQuery("SELECT * FROM users WHERE Uid=@uid", new MySQLHelper.Parameter("uid", uID)).First();
            //之所以没有个 data进行缓存是 user数据蛮重要的 不能缓存 也不需要 或许以后的文章数据可以加上缓存

            //没有set是因为这是操作整个行 ToDO
        }
        /// <summary>
        /// 从缓存中获取的元数据
        /// </summary>
        public Line DataBuff //用于读取用 如果要写啥 禁止使用这个 需手动获取最新数据
        {
            get
            {
                if (databf == null)
                {
                    databf = Data;
                }
                return databf;
            }
            //没有set是因为这是操作整个行 太费了
        }
        private Line databf;//用于读取用 如果要写啥 禁止使用这个 需手动获取最新数据
        #endregion

        /// <summary>
        /// 用户id
        /// </summary>
        public readonly int uID;
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName
        {
            get
            {
                if (username == null)
                    username = DataBuff.Find("name").Info;
                return username;
            }
            //set//用户名暂时不可以更改 顺便偷个懒
            //{
            //    RAWUser.ExecuteNonQuery($"UPDATE users SET money={Convert.ToInt32(value * 10)} WHERE Uid={Uid}", CommandType.Text);
            //}
        }
        private string username;
        /// <summary>
        /// 密码确认 确认这个密码是否符合
        /// </summary>
        /// <param name="pw">要确认的密码</param>
        /// <returns></returns>
        public bool PasswordCheck(string pw) => Function.MD5salt(pw) == Data.Find("password").info;
        /// <summary>
        /// 设置新密码
        /// </summary>
        /// <param name="pw">新密码</param>
        public void PasswordSet(string pw)
        {
            RAWUser.ExecuteNonQuery($"UPDATE users SET password=@pw WHERE Uid=@uid", new MySQLHelper.Parameter("pw", Function.MD5salt(pw)), new MySQLHelper.Parameter("uid", uID));
        }

        /// <summary>
        /// 电子邮件
        /// </summary>
        public string Email
        {
            get
            {
                return DataBuff.Find("email").Info;
            }
            set
            {
                databf = null;
                RAWUser.ExecuteNonQuery($"UPDATE users SET email=@email WHERE Uid=@uid", new MySQLHelper.Parameter("email", value), new MySQLHelper.Parameter("uid", uID));
            }
        }
        /// <summary>
        /// 金钱 最小单位0.1
        /// </summary>
        public double Money
        {//钱是重要的 所以无论增改都不使用缓存
            get => Data.Find("money").InfoToInt * 0.1;
            set => RAWUser.ExecuteNonQuery($"UPDATE users SET money=@money WHERE Uid=@uid", new MySQLHelper.Parameter("money", Convert.ToInt32(value * 10)), new MySQLHelper.Parameter("uid", uID));
        }

        /// <summary>
        /// 增加经验值
        /// </summary>
        /// <param name="exp">要增加的经验值</param>
        public void ExpAdd(int exp)
        {
            Line data = Data;
            exp += data.Find("exp").InfoToInt;
            int lv = Data.Find("lv").InfoToInt;
            if (exp >= lv * 10)
            {
                exp -= lv * 10;
                lv++;
            }
            RAWUser.ExecuteNonQuery($"UPDATE users SET lv=@lv,exp=@exp WHERE Uid=@uid", new MySQLHelper.Parameter("exp", exp), new MySQLHelper.Parameter("lv", lv), new MySQLHelper.Parameter("uid", uID));
            databf = null;//清空缓存
        }
        /// <summary>
        /// 当前经验值
        /// </summary>
        public int Exp
        {
            get => DataBuff.Find("exp").InfoToInt;
        }
        /// <summary>
        /// 当前用户等级
        /// </summary>
        public int Lv
        {
            get => DataBuff.Find("lv").InfoToInt;
            //set { RAWUser.ExecuteNonQuery($"UPDATE users SET lv=@money WHERE Uid=@uid", new MySQLHelper.Parameter("money", value), new MySQLHelper.Parameter("uid", uID)); }
        }

        /// <summary>
        /// 用户权限
        /// </summary>
        public AuthLevel Authority
        {//用户权限并非储存在用户里的 而是储存在网站设置里的(保证所有网站权限不互通) 除了超级管理员除外
            get
            {
                if (Convert.ToBoolean(Data.Find("isroot").info))
                    return AuthLevel.Admin;//如果是root直接无脑admin
                return GetUserAuthority(uID);
            }
            set => SetUserAuthority(uID, value);
        }

        /// <summary>
        /// 用户头像链接
        /// </summary>
        public string AvatarURL
        {
            get
            {
                string str = DataBuff.Find("headport").Info;
                if (str == "")
                    return "/image/user.png";
                return str;
            }
            set
            {
                databf = null;
                RAWUser.ExecuteNonQuery($"UPDATE users SET headport=@headp WHERE Uid=@uid", new MySQLHelper.Parameter("headp", value), new MySQLHelper.Parameter("uid", uID));
            }
        }


        #region "构造函数"
        public Users(int uid, Line raw = null)
        {
            uID = uid;
            databf = raw;
        }
        public Users(Line raw)
        {
            uID = raw.InfoToInt;
            databf = raw;
        }
        #endregion

    }
}