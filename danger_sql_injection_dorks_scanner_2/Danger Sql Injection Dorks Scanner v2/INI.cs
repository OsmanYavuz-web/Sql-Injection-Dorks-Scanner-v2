using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Danger_Sql_Injection_Dorks_Scanner_v2
{
    class INI
    {
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
 
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

        public INI(string path)
        {
            Path = path;
        }
 
        private string Path = String.Empty;
        public string Default { get; set; }
 
        public string Read(string section, string key)
        {
            Default = Default ?? String.Empty;
            StringBuilder StrBuild = new StringBuilder(256);
            GetPrivateProfileString(section, key, Default, StrBuild, 255, Path);
            return StrBuild.ToString();
        }
 
        public long Write(string section, string key, string value)
        {
            return WritePrivateProfileString(section, key, value, Path);
        }
    }
}
