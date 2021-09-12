using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using LinePutScript;
using System.Management;
using System.Net;

namespace SoftwareVersion.Client
{
    /// <summary>
    /// 版本检查类
    /// </summary>
    public class Verison
    {
        /// <summary>
        /// 版本检查网站地址  项目 SoftwareVerison.Manager
        /// </summary>
        public string SVMURL;
        /// <summary>
        /// 软件名称
        /// </summary>
        public string SoftWare;
        /// <summary>
        /// 版本
        /// </summary>
        public int Version;
        /// <summary>
        /// 该软件的所有版本
        /// </summary>
        public List<VersionManager> Versions = new List<VersionManager>();
        /// <summary>
        /// 检查更新的时候是否有错误 如果有错误,则其他所有功能无法使用
        /// </summary>
        public Exception Exception = null;
        /// <summary>
        /// 新建一个版本检查类
        /// </summary>
        /// <param name="svmurl">版本检查网站地址  项目 SoftwareVerison.Manager</param>
        /// <param name="software">软件名称</param>
        /// <param name="version">版本</param>
        public Verison(string svmurl, string software, int version)
        {
            SVMURL = svmurl;
            SoftWare = software;
            Version = version;
            try
            {
                System.IO.Stream stream = System.Net.WebRequest.Create($"{SVMURL}/Verison.ashx?soft={SoftWare}&type=lps").GetResponse().GetResponseStream();
                StreamReader sr = new StreamReader(stream, System.Text.Encoding.UTF8);
                string ReadText = sr.ReadToEnd(); //由于这里并非读取全部文件，这里正常为空
                sr.Dispose(); //关闭流
                ReadText = System.Uri.UnescapeDataString(ReadText);
                LpsDocument lps = new LpsDocument(ReadText);
                foreach (Line line in lps)
                {
                    Versions.Add(new VersionManager(line));
                }
            }
            catch (Exception ex)
            {
                Exception = ex;
            }
        }
        /// <summary>
        /// 检查更新 如果需要更新返回更新的版本和内容 如果不需要返回""
        /// </summary>
        /// <param name="importance">重要性 更新软件重要性</param>
        /// <returns>如果需要更新返回更新的版本和内容 如果不需要返回""</returns>
        public string CheckUpdate(out VersionManager.Importance importance)
        {
            importance = VersionManager.Importance.Delete;
            if (Exception != null || Versions.Count == 0)
                return "ERROR";
            if (Versions[0].Ver == Version)
                return "";
            StringBuilder sb = new StringBuilder();
            foreach (VersionManager v in Versions)
            {
                if (v.Importances != VersionManager.Importance.Delete)
                {
                    if (((byte)v.Importances) > ((byte)importance))
                        importance = v.Importances;
                    sb.AppendLine(v.Verison + " " + v.Illustration);
                }
            }
            return sb.ToString();
        }
    }
    /// <summary>
    /// 激活检查类
    /// </summary>    
    public class Activation
    {
        /// <summary>
        /// 版本检查网站地址  项目 SoftwareVerison.Manager
        /// </summary>
        public string SVMURL;
        /// <summary>
        /// 软件名称
        /// </summary>
        public string SoftWare;
        /// <summary>
        /// 版本
        /// </summary>
        public int Version;
        /// <summary>
        /// 检查激活的时候是否有错误 如果有错误,则其他所有功能无法使用
        /// </summary>
        public Exception Exception = null;
        /// <summary>
        /// 新建一个激活检查类
        /// </summary>
        /// <param name="svmurl">激活检查网站地址  项目 SoftwareVerison.Manager</param>
        /// <param name="software">软件名称</param>
        /// <param name="version">版本</param>
        public Activation(string svmurl, string software, int version)
        {
            SVMURL = svmurl;
            SoftWare = software;
            Version = version;
        }
        /// <summary>
        /// 激活这台设备 返回是否激活成功
        /// </summary>
        /// <param name="code">激活码</param>
        /// <param name="actcode">如果激活码可用,退回激活码详细信息</param>
        /// <param name="message">退回原因</param>
        /// <returns>激活成功为True,否则为False</returns>
        public bool TryActivation(string code, out ActivationCode actcode, out ReturnMessage message)
        {
            code = code.Replace("-", "").Replace(" ", "");
            actcode = null;
            message = ReturnMessage.ERROR0;
            try
            {
                Stream stream = WebRequest.Create($"{SVMURL}/Activation.ashx?mode=active&soft={SoftWare}&code={code}&comp={GetComputerHash():X}&ver={Version}").GetResponse().GetResponseStream();
                StreamReader sr = new StreamReader(stream, Encoding.UTF8);
                string ReadText = sr.ReadToEnd(); //由于这里并非读取全部文件，这里正常为空
                sr.Dispose(); //关闭流
                ReadText = Uri.UnescapeDataString(ReadText);
                LpsDocument lps = new LpsDocument(ReadText);
                message = (ReturnMessage)lps.First().InfoToInt;
                if (lps.First().InfoToInt / 32 == 0)
                {
                    return false;
                }
                if (lps.First().InfoToInt / 32 == 2)
                {
                    actcode = new ActivationCode(lps.Assemblage[1]);
                    return true;
                }
                else if (lps.First().InfoToInt / 32 == 4)
                {
                    actcode = new ActivationCode(lps.Assemblage[1]);
                    return false;
                }
                else
                {
                    return false;
                }
                //switch (message)
                //{
                //    case ReturnMessage.ERROR0:
                //    case ReturnMessage.ERROR1:
                //    case ReturnMessage.ERROR2:
                //        break;
                //    case ReturnMessage.FALSE0:
                //    case ReturnMessage.FALSE1:
                //    case ReturnMessage.FALSE2:
                //    case ReturnMessage.FALSE3:
                //        break;
                //    case ReturnMessage.TRUE0:
                //    case ReturnMessage.TRUE1:
                //        break;
                //}
            }
            catch (Exception ex)
            {
                Exception = ex;
                return false;
            }
        }
        /// <summary>
        /// 获取计算机信息计算出来的唯一计算机码
        /// </summary>
        /// <returns></returns>
        public static int GetComputerHash()
        {
            int hash = 0;
            foreach (ManagementBaseObject obj in new ManagementObjectSearcher("SELECT * FROM Win32_Processor").Get())
            {
                hash += ((string)obj["Processorid"]).GetHashCode();
            }
            foreach (ManagementBaseObject obj in new ManagementObjectSearcher("SELECT * FROM Win32_PhysicalMemory").Get())
            {
                hash += ((string)obj["PartNumber"]).GetHashCode();
            }
            foreach (ManagementBaseObject obj in new ManagementObjectSearcher("SELECT * FROM Win32_LogicalDisk").Get())
            {
                hash += ((string)obj["VolumeSerialNumber"]).GetHashCode();
            }
            return hash;
        }
        /// <summary>
        /// 从网站上返回的消息
        /// </summary>
        public enum ReturnMessage : byte
        {
            /// <summary>
            /// 错误0: 程序内部错误
            /// </summary>
            ERROR0 = 0,
            /// <summary>
            /// 错误1: 缺失信息或信息错误
            /// </summary>
            ERROR1 = 1,
            /// <summary>
            /// 错误2: 激活码错误或激活码不存在
            /// </summary>
            ERROR2 = 2,
            /// <summary>
            /// 激活成功 这台电脑被二次激活
            /// </summary>
            TRUE0 = 64,
            /// <summary>
            /// 激活成功 这台电脑被首次激活
            /// </summary>
            TRUE1 = 65,
            /// <summary>
            /// 激活失败 激活次数被用完
            /// </summary>
            FALSE0 = 128,
            /// <summary>
            /// 激活失败 激活码无法激活这个版本的软件
            /// </summary>
            FALSE1 = 129,
            /// <summary>
            /// 激活失败 激活码已过有效期
            /// </summary>
            FALSE2 = 130,
            /// <summary>
            /// 激活失败 激活码无法激活这个软件
            /// </summary>
            FALSE3 = 131,
        }
    }
}
