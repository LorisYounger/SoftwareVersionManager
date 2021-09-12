using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LinePutScript;
using LinePutScript.SQLHelper;
using static SoftwareVersion.Manager.Function;

namespace SoftwareVersion.Manager
{
    //激活码表格式:
    //   -activecode
    //     code(int64)|software(string64)|verison(int32)|Uid(int)|Expiration(datetime)|times(short)|illustration(text)|remarks(text)|Activated(text)|
    //      激活码     | 可以激活的软件名  | 可激活版本   | 用户id  |      失效日期      | 可以激活次数|       描述       |备注(给管理员)| 已激活电脑     |
    //      随机数     |------------------| -1为不限版本 |-1未绑定 |      9999-12-30    |5 -1为不限次数|         空       |    空       |        空     |
    public class ActivationCode
    {
        #region "辅助构建函数"
        /// <summary>
        /// 通过激活码获得激活码信息
        /// </summary>
        /// <param name="code">16进制16位数激活码</param>
        /// <returns></returns>
        public static ActivationCode GetActivationCode(string code)
        {
            //先除去多余文本
            code = code.Replace("-","").Replace(" ","");
            //先将CODE转换成Long
            if (!long.TryParse(code, System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out long activationCode))
            {
                return null;
            }
            return GetActivationCode(activationCode);
        }
        /// <summary>
        /// 通过激活码获得激活码信息
        /// </summary>
        /// <param name="code">激活码</param>
        /// <returns></returns>
        public static ActivationCode GetActivationCode(long code)
        {
            LpsDocument data = RAW.ExecuteQuery("SELECT * FROM activecode WHERE code=@code", new MySQLHelper.Parameter("code", code));
            if (data.Assemblage.Count == 0)
                return null;
            return new ActivationCode(data.First().InfoToInt64, data.First());
        }
        /// <summary>
        /// 通过用户id获得该用户的所有激活码
        /// </summary>
        /// <param name="UserName">用户id</param>
        public static List<ActivationCode> GetActivationCode(int UserID)
        {
            List<ActivationCode> list = new List<ActivationCode>();
            LpsDocument data = RAW.ExecuteQuery("SELECT * FROM activecode WHERE Uid=@uid", new MySQLHelper.Parameter("uid", UserID));
            foreach (Line line in data.Assemblage)
            {
                list.Add(new ActivationCode(line.InfoToInt64, line));
            }
            return list;
        }
        /// <summary>
        /// 创建新激活码
        /// </summary>
        public static ActivationCode CreatActivationCode(string software, int verison = -1, int uid = -1, DateTime expiration = default, short times = 5, string illustration = "", string remarks = "")
        {
            long rndkey = RndLong();
            while (GetActivationCode(rndkey) != null)//确保没有重复的key
                rndkey = RndLong();
            if (expiration == default)
                expiration = DateTime.MaxValue;//9999年12月31日

            RAW.ExecuteNonQuery($"INSERT INTO activecode VALUES (@code,@sw,@vs,@uid,@exp,@tm,@ds,@rm,\"\")", new MySQLHelper.Parameter("code", rndkey), new MySQLHelper.Parameter("sw", software),
                new MySQLHelper.Parameter("vs", verison), new MySQLHelper.Parameter("uid", uid), new MySQLHelper.Parameter("exp", expiration), new MySQLHelper.Parameter("tm", times),
                new MySQLHelper.Parameter("ds", illustration), new MySQLHelper.Parameter("rm", remarks));
            return GetActivationCode(rndkey);
        }
        #endregion
        #region 数据库Data

        /// <summary>
        /// 从数据库获得元数据
        /// </summary>
        public Line Data
        {
            get => RAW.ExecuteQuery("SELECT * FROM activecode WHERE code=@code", new MySQLHelper.Parameter("code", Code)).First();
            //之所以没有个 data进行缓存是 post数据蛮重要的 不能缓存 也不需要 或许以后的文章数据可以加上缓存
        }
        /// <summary>
        /// 从缓存中获取的元数据
        /// </summary>
        public Line DataBuff //用于读取用 如果要写啥 禁止使用这个 需手动获取最新数据
        {
            get
            {
                if (databf == null)
                    databf = Data;
                return databf;
            }
            //没有set是因为这是操作整个行 太费了
        }
        private Line databf;//用于读取用 如果要写啥 禁止使用这个 需手动获取最新数据
        #endregion

        /// <summary>
        /// 激活码源64位
        /// </summary>
        public readonly long Code;
        /// <summary>
        /// 激活码16进制16位数
        /// </summary>
        public readonly string CodeHEX;

        /// <summary>
        /// 软件名称
        /// </summary>
        public string Software
        {
            get
            {
                return DataBuff.Find("software").Info;
            }
            set
            {
                databf = null;
                RAW.ExecuteNonQuery($"UPDATE activecode SET software=@value WHERE code=@code", new MySQLHelper.Parameter("value", value), new MySQLHelper.Parameter("code", Code));
            }
        }
        /// <summary>
        /// 可激活版本 -1为不限版本
        /// </summary>
        public int Verison
        {
            get
            {
                return DataBuff.Find("verison").InfoToInt;
            }
            set
            {
                databf = null;
                RAW.ExecuteNonQuery($"UPDATE activecode SET verison=@value WHERE code=@code", new MySQLHelper.Parameter("value", value), new MySQLHelper.Parameter("code", Code));
            }
        }
        /// <summary>
        /// 可激活版本
        /// </summary>
        public int Uid
        {
            get
            {
                return DataBuff.Find("uid").InfoToInt;
            }
            set
            {
                databf = null;
                RAW.ExecuteNonQuery($"UPDATE activecode SET uid=@value WHERE code=@code", new MySQLHelper.Parameter("value", value), new MySQLHelper.Parameter("code", Code));
            }
        }
        /// <summary>
        /// 失效日期
        /// </summary>
        public DateTime Expiration
        {
            get
            {
                return Convert.ToDateTime(DataBuff.Find("expiration").Info);
            }
            set
            {
                databf = null;
                RAW.ExecuteNonQuery($"UPDATE activecode SET expiration=@date WHERE code=@code", new MySQLHelper.Parameter("date", value), new MySQLHelper.Parameter("code", Code));
            }
        }
        /// <summary>
        /// 可以激活次数
        /// </summary>
        public short Times
        {
            get
            {
                return (short)DataBuff.Find("times").InfoToInt;
            }
            set
            {
                databf = null;
                RAW.ExecuteNonQuery($"UPDATE activecode SET times=@value WHERE code=@code", new MySQLHelper.Parameter("value", value), new MySQLHelper.Parameter("code", Code));
            }
        }
        /// <summary>
        /// 描述 给用户看的描述
        /// </summary>
        public string Illustration
        {
            get
            {
                return DataBuff.Find("illustration").Info;
            }
            set
            {
                databf = null;
                RAW.ExecuteNonQuery($"UPDATE activecode SET illustration=@value WHERE code=@code", new MySQLHelper.Parameter("value", value), new MySQLHelper.Parameter("code", Code));
            }
        }
        /// <summary>
        /// 备注 给管理员或者软件设置的备注
        /// </summary>
        public string Remarks
        {
            get
            {
                return DataBuff.Find("remarks").Info;
            }
            set
            {
                databf = null;
                RAW.ExecuteNonQuery($"UPDATE activecode SET remarks=@value WHERE code=@code", new MySQLHelper.Parameter("value", value), new MySQLHelper.Parameter("code", Code));
            }
        }
        /// <summary>
        /// 已激活电脑原数据
        /// </summary>
        public string Activated
        {
            get
            {
                return DataBuff.Find("activated").Info;
            }
            set
            {
                databf = null;
                RAW.ExecuteNonQuery($"UPDATE activecode SET activated=@value WHERE code=@code", new MySQLHelper.Parameter("value", value), new MySQLHelper.Parameter("code", Code));
            }
        }
        /// <summary>
        /// 已激活电脑个数
        /// </summary>
        public int ActivatedNumber
        {
            get
            {
                if (string.IsNullOrEmpty(Activated))
                    return 0;
                return Activated.Split(',').Length;
            }
        }
        /// <summary>
        /// 添加激活电脑
        /// </summary>
        /// <param name="computer">电脑唯一id,一般是一串HEX由电脑硬件计算而来</param>
        public void ActivatedADD(string computer)
        {
            if (string.IsNullOrEmpty(Activated))
                Activated = computer;
            else
                Activated += ',' + computer;
        }
        /// <summary>
        /// 判断是否已经激活过这台电脑
        /// </summary>
        /// <param name="computer">电脑唯一id,一般是一串HEX由电脑硬件计算而来</param>
        public bool ActivatedHave(string computer) => Activated.Contains(computer);
        /// <summary>
        /// 移除激活电脑
        /// </summary>
        /// <param name="computer">电脑唯一id,一般是一串HEX由电脑硬件计算而来</param>
        public void ActivatedRemove(string computer)
        {
            if (!string.IsNullOrEmpty(Activated))
                Activated = Activated.Replace(computer,"").Replace(",,",",").Trim(',');
        }

        #region 构造函数
        public ActivationCode(long code)
        {
            Code = code;
            CodeHEX = Code.ToString("X");
        }
        public ActivationCode(long code, Line bf)
        {
            Code = code;
            CodeHEX = Code.ToString("X").Insert(12,"-").Insert(8, "-").Insert(4, "-");            
            databf = bf;
        }
        #endregion
    }
}