using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LinePutScript;
using LinePutScript.SQLHelper;
using static SoftwareVersion.Manager.Function;

namespace SoftwareVersion.Manager
{
    //版本码表格式:
    //   -verisonmanager
    //     vid(int)|software(string64)|ver(int32)|verison(string32)|PublishDate|importances(sbyte)|times(int)|illustration(text)|remarks(text)|
    //     用于键控 |      软件名称    | 软件版本  | 软件版本(文本)   |  发布时间 |       重要性      |被使用次数|   更新内容   |备注(给管理员)|
    //      递增    |-------------------------------------------    |   Now    |     Default      |     0    |     空       |    空       |
    public class VerisonManager
    {
        #region "辅助构建函数"
        /// <summary>
        /// 通过vid获得版本信息
        /// </summary>
        public static VerisonManager GetVerisonManager(int vid)
        {
            LpsDocument data = RAW.ExecuteQuery("SELECT * FROM verisonmanager WHERE Vid=@vid", new MySQLHelper.Parameter("vid", vid));
            if (data.Assemblage.Count == 0)
                return null;
            return new VerisonManager(data.First().InfoToInt, data.First());
        }
        /// <summary>
        /// 通过软件名称获得版本信息
        /// </summary>
        /// <param name="software">软件名称</param>
        public static List<VerisonManager> GetVerisonManager(string software, string orderby = "ver")
        {
            List<VerisonManager> list = new List<VerisonManager>();
            LpsDocument data = RAW.ExecuteQuery("SELECT * FROM verisonmanager WHERE software=@software ORDER BY " + orderby + " DESC", new MySQLHelper.Parameter("software", software));
            foreach (Line line in data.Assemblage)
            {
                list.Add(new VerisonManager(line.InfoToInt, line));
            }
            return list;
        }
        /// <summary>
        /// 创建新激活码
        /// </summary>
        public static VerisonManager CreatVerisonManager(string software, int ver, string verison, DateTime publishdate = default, Importance importance = Importance.Default, int times = 0, string illustration = "", string remarks = "")
        {

            if (publishdate == default)
                publishdate = DateTime.Now;//9999年12月31日

            RAW.ExecuteNonQuery($"INSERT INTO verisonmanager VALUES (NULL,@sw,@v,@ver,@pd,@ip,@tm,@ds,@rm)", new MySQLHelper.Parameter("sw", software), new MySQLHelper.Parameter("v", ver),
                new MySQLHelper.Parameter("ver", verison), new MySQLHelper.Parameter("pd", publishdate), new MySQLHelper.Parameter("ip", (byte)importance), new MySQLHelper.Parameter("tm", times),
                new MySQLHelper.Parameter("ds", illustration), new MySQLHelper.Parameter("rm", remarks));
            return GetVerisonManager(RAW.ExecuteQuery("select LAST_INSERT_ID()").First().InfoToInt);
        }
        #endregion

        #region 数据库Data

        /// <summary>
        /// 从数据库获得元数据
        /// </summary>
        public Line Data
        {
            get => RAW.ExecuteQuery("SELECT * FROM verisonmanager WHERE Vid=@vid", new MySQLHelper.Parameter("vid", vID)).First();
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
        /// vid
        /// </summary>
        public readonly int vID;


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
                RAW.ExecuteNonQuery($"UPDATE verisonmanager SET software=@value WHERE Vid=@vid", new MySQLHelper.Parameter("value", value), new MySQLHelper.Parameter("vid", vID));
            }
        }
        /// <summary>
        /// 可激活版本
        /// </summary>
        public int Ver
        {
            get
            {
                return DataBuff.Find("ver").InfoToInt;
            }
            set
            {
                databf = null;
                RAW.ExecuteNonQuery($"UPDATE verisonmanager SET ver=@value WHERE Vid=@vid", new MySQLHelper.Parameter("value", value), new MySQLHelper.Parameter("vid", vID));
            }
        }
        /// <summary>
        /// 软件版本(文本)
        /// </summary>
        public string Verison
        {
            get
            {
                return DataBuff.Find("verison").Info;
            }
            set
            {
                databf = null;
                RAW.ExecuteNonQuery($"UPDATE verisonmanager SET verison=@value WHERE Vid=@vid", new MySQLHelper.Parameter("value", value), new MySQLHelper.Parameter("vid", vID));
            }
        }
        /// <summary>
        /// 发布时间
        /// </summary>
        public DateTime PublishDate
        {
            get
            {
                return Convert.ToDateTime(DataBuff.Find("publishdate").Info);
            }
            set
            {
                databf = null;
                RAW.ExecuteNonQuery($"UPDATE verisonmanager SET publishdate=@value WHERE Vid=@vid", new MySQLHelper.Parameter("value", value), new MySQLHelper.Parameter("vid", vID));
            }
        }

        /// <summary>
        /// 重要性
        /// </summary>
        public Importance Importances
        {
            get
            {
                return (Importance)Convert.ToSByte(DataBuff.Find("importances").info);
                //Importance state = (Importance)Convert.ToSByte(DataBuff.Find("state").info);
                //if (state == Importance.Default)//如果是默认 则走设置内容
                //    return Importance.PostDefault;
                //return state;
            }
            set
            {
                databf = null;
                RAW.ExecuteNonQuery($"UPDATE post SET importances=@value WHERE Vid=@vid", new MySQLHelper.Parameter("value", (sbyte)value), new MySQLHelper.Parameter("vid", vID));
            }
        }
        public enum Importance : sbyte
        {
            /// <summary>
            /// 删除 不会被显示
            /// </summary>
            Delete = 0,
            /// <summary>
            /// 默认类型
            /// </summary>
            Default = 1,        
            /// <summary>
            /// 修复错误
            /// </summary>
            BUGFix = 3,
            /// <summary>
            /// 功能更新
            /// </summary>
            Function = 5,
            /// <summary>
            /// 修复严重错误
            /// </summary>
            ERRORFix = 7,
            /// <summary>
            /// 重要更新
            /// </summary>
            Important = 8,
            /// <summary>
            /// 强制更新
            /// </summary>
            MUST = 9,

            //9以上的由不同功能 魔改 不做统一
        }
        /// <summary>
        /// 被使用次数
        /// </summary>
        public int Times
        {
            get
            {
                return DataBuff.Find("times").InfoToInt;
            }
            set
            {
                databf = null;
                RAW.ExecuteNonQuery($"UPDATE verisonmanager SET times=@value WHERE Vid=@vid", new MySQLHelper.Parameter("value", value), new MySQLHelper.Parameter("vid", vID));
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
                RAW.ExecuteNonQuery($"UPDATE verisonmanager SET illustration=@value WHERE Vid=@vid", new MySQLHelper.Parameter("value", value), new MySQLHelper.Parameter("vid", vID));
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
                RAW.ExecuteNonQuery($"UPDATE verisonmanager SET remarks=@value WHERE Vid=@vid", new MySQLHelper.Parameter("value", value), new MySQLHelper.Parameter("vid", vID));
            }
        }
        #region "构造函数"
        public VerisonManager(int vid, Line raw = null)
        {
            vID = vid;
            databf = raw;
        }
        #endregion

    }
}