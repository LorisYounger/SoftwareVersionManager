using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LinePutScript;

namespace SoftwareVersion.Client
{
    //版本码表格式:
    //   -verisonmanager
    //     vid(int)|software(string64)|ver(int32)|verison(string32)|PublishDate|importances(sbyte)|times(int)|illustration(text)|remarks(text)|
    //     用于键控 |      软件名称    | 软件版本  | 软件版本(文本)   |  发布时间 |       重要性      |被使用次数|   更新内容   |备注(给管理员)|
    //      递增    |-------------------------------------------    |   Now    |     Default      |     0    |     空       |    空       |
    /// <summary>
    /// 版本码表 无需自行创建
    /// </summary>
    public class VersionManager
    {
  

        #region 数据库Data

        /// <summary>
        /// 从数据库获得元数据
        /// </summary>
        private Line Data;
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
                return Data.Find("software").Info;
            }
        }
        /// <summary>
        /// 可激活版本
        /// </summary>
        public int Ver
        {
            get
            {
                return Data.Find("ver").InfoToInt;
            }
        }
        /// <summary>
        /// 软件版本(文本)
        /// </summary>
        public string Verison
        {
            get
            {
                return Data.Find("verison").Info;
            }
        }
        /// <summary>
        /// 发布时间
        /// </summary>
        public DateTime PublishDate
        {
            get
            {
                return Convert.ToDateTime(Data.Find("publishdate").Info);
            }
        }

        /// <summary>
        /// 重要性
        /// </summary>
        public Importance Importances
        {
            get
            {
                return (Importance)Convert.ToSByte(Data.Find("importances").info);
                //Importance state = (Importance)Convert.ToSByte(Data.Find("state").info);
                //if (state == Importance.Default)//如果是默认 则走设置内容
                //    return Importance.PostDefault;
                //return state;
            }
        }
        /// <summary>
        /// 更新重要性
        /// </summary>
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
                return Data.Find("times").InfoToInt;
            }
        }

        /// <summary>
        /// 描述 给用户看的描述
        /// </summary>
        public string Illustration
        {
            get
            {
                return Data.Find("illustration").Info;
            }
        }
        /// <summary>
        /// 备注 给管理员或者软件设置的备注
        /// </summary>
        public string Remarks
        {
            get
            {
                return Data.Find("remarks").Info;
            }
        }
        #region "构造函数"
        /// <summary>
        /// 版本码表 无需自行创建
        /// </summary>
        public VersionManager(Line raw)
        {
            vID = raw.InfoToInt;
            Data = raw;
        }
        #endregion

    }
}