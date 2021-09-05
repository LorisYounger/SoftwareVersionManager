using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LinePutScript;

namespace SoftwareVersion.Client
{
    public class Verison
    {
        public string SVMURL;
        public string SoftWare;
        public List<VersionManager> Versions = new List<VersionManager>();
        public Exception Exception = null;
        public Verison(string svmurl, string software)
        {
            SVMURL = svmurl;
            SoftWare = software;
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
        public string CheckUpdate(int ver, out VersionManager.Importance importance)
        {
            importance = VersionManager.Importance.Delete;
            if (Exception != null || Versions.Count == 0)
                return "ERROR";
            if (Versions[0].Ver == ver)
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
}
