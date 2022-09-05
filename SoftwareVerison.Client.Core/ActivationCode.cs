using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LinePutScript;

namespace SoftwareVersion.Client
{
    //激活码表格式:
    //   -activecode
    //     code(int64)|software(string64)|verison(int32)|Uid(int)|Expiration(datetime)|times(short)|illustration(text)|remarks(text)|Activated(text)|
    //      激活码     | 可以激活的软件名  | 可激活版本   | 用户id  |      失效日期      | 可以激活次数|       描述       |备注(给管理员)| 已激活电脑     |
    //      随机数     |------------------| -1为不限版本 |-1未绑定 |      9999-12-30   |       5     |         空       |    空       |        空     |
    /// <summary>
    /// 激活码表,无需自行创建
    /// </summary>
    public class ActivationCode
    {

        #region 数据库Data

        /// <summary>
        /// 从数据库获得元数据
        /// </summary>
        private Line Data;
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
                return Data.Find("software").Info;
            }
        }
        /// <summary>
        /// 可激活版本
        /// </summary>
        public int Verison
        {
            get
            {
                return Data.Find("verison").InfoToInt;
            }
        }
        /// <summary>
        /// 可激活版本
        /// </summary>
        public int Uid
        {
            get
            {
                return Data.Find("uid").InfoToInt;
            }
        }
        /// <summary>
        /// 失效日期
        /// </summary>
        public DateTime Expiration
        {
            get
            {
                return Convert.ToDateTime(Data.Find("expiration").Info);
            }
        }
        /// <summary>
        /// 可以激活次数
        /// </summary>
        public short Times
        {
            get
            {
                return (short)Data.Find("times").InfoToInt;
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
        /// <summary>
        /// 已激活电脑原数据
        /// </summary>
        public string Activated
        {
            get
            {
                return Data.Find("activated").Info;
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
        /// 判断是否已经激活过这台电脑
        /// </summary>
        /// <param name="computer">电脑唯一id,一般是一串HEX由电脑硬件计算而来</param>
        public bool ActivatedHave(string computer) => Activated.Contains(computer);


        #region 构造函数
        /// <summary>
        /// 激活码表,无需自行创建
        /// </summary>
        /// <param name="bf"></param>
        public ActivationCode(Line bf)
        {
            Code = bf.InfoToInt64;
            CodeHEX = Code.ToString("X");
            Data = bf;
        }
        #endregion
    }
}