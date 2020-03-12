using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Danger_Sql_Injection_Dorks_Scanner_v2
{
    public static class Proxy
    {
        #region API

        public static string applicationName;

        [DllImport("wininet.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern IntPtr InternetOpen(
            string lpszAgent, int dwAccessType, string lpszProxyName,
            string lpszProxyBypass, int dwFlags);

        [DllImport("wininet.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool InternetCloseHandle(IntPtr hInternet);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        private struct INTERNET_PER_CONN_OPTION_LIST
        {
            public int Size;
            public System.IntPtr Connection;
            public int OptionCount;
            public int OptionError;
            public IntPtr pOptions;
        }
        private enum INTERNET_OPTION
        {
            INTERNET_OPTION_PER_CONNECTION_OPTION = 75,
            INTERNET_OPTION_SETTINGS_CHANGED = 39,
            INTERNET_OPTION_REFRESH = 37
        }

        private enum INTERNET_PER_CONN_OptionEnum
        {
            INTERNET_PER_CONN_FLAGS = 1,
            INTERNET_PER_CONN_PROXY_SERVER = 2,
            INTERNET_PER_CONN_PROXY_BYPASS = 3,
            INTERNET_PER_CONN_AUTOCONFIG_URL = 4,
            INTERNET_PER_CONN_AUTODISCOVERY_FLAGS = 5,
            INTERNET_PER_CONN_AUTOCONFIG_SECONDARY_URL = 6,
            INTERNET_PER_CONN_AUTOCONFIG_RELOAD_DELAY_MINS = 7,
            INTERNET_PER_CONN_AUTOCONFIG_LAST_DETECT_TIME = 8,
            INTERNET_PER_CONN_AUTOCONFIG_LAST_DETECT_URL = 9,
            INTERNET_PER_CONN_FLAGS_UI = 10
        }
        private const int INTERNET_OPEN_TYPE_DIRECT = 1;  // direct to net
        private const int INTERNET_OPEN_TYPE_PRECONFIG = 0; // read registry
        private enum INTERNET_OPTION_PER_CONN_FLAGS
        {
            PROXY_TYPE_DIRECT = 0x00000001,   // direct to net
            PROXY_TYPE_PROXY = 0x00000002,   // via named proxy
            PROXY_TYPE_AUTO_PROXY_URL = 0x00000004,   // autoproxy URL
            PROXY_TYPE_AUTO_DETECT = 0x00000008   // use autoproxy detection
        }
        [StructLayout(LayoutKind.Explicit)]
        private struct INTERNET_PER_CONN_OPTION_OptionUnion
        {
            [FieldOffset(0)]
            public int dwValue;
            [FieldOffset(0)]
            public IntPtr pszValue;
            [FieldOffset(0)]
            public FILETIME ftValue;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct INTERNET_PER_CONN_OPTION
        {
            public int dwOption;
            public INTERNET_PER_CONN_OPTION_OptionUnion Value;
        }

        [DllImport("wininet.dll", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern bool InternetSetOption(
            IntPtr hInternet,
            INTERNET_OPTION dwOption,
            IntPtr lpBuffer,
            int lpdwBufferLength);

        [DllImport("wininet.dll", CharSet = CharSet.Ansi, SetLastError = true,
            EntryPoint = "InternetQueryOption")]
        private extern static bool InternetQueryOptionList(
            IntPtr Handle,
            INTERNET_OPTION OptionFlag,
            ref INTERNET_PER_CONN_OPTION_LIST OptionList,
            ref int size);

        #endregion

        /// <summary>
        /// Proxy Ayarlama İşlemi
        /// </summary>
        /// <param name="proxy">Parametre olarak bir proxy değeri verin Ör : 192.168.1.1:8080</param>
        /// <returns>Proxy Ayar Sonucu</returns>
        public static bool ProxyAyarla(string proxy)
        {

            IntPtr hInternet = InternetOpen(applicationName, INTERNET_OPEN_TYPE_DIRECT, null, null, 0);
            INTERNET_PER_CONN_OPTION[] Options = new INTERNET_PER_CONN_OPTION[2];
            Options[0] = new INTERNET_PER_CONN_OPTION();
            Options[0].dwOption = (int)INTERNET_PER_CONN_OptionEnum.INTERNET_PER_CONN_FLAGS;
            Options[0].Value.dwValue = (int)INTERNET_OPTION_PER_CONN_FLAGS.PROXY_TYPE_PROXY;
            Options[1] = new INTERNET_PER_CONN_OPTION();
            Options[1].dwOption =
                (int)INTERNET_PER_CONN_OptionEnum.INTERNET_PER_CONN_PROXY_SERVER;
            Options[1].Value.pszValue = Marshal.StringToHGlobalAnsi(proxy);

            System.IntPtr buffer = Marshal.AllocCoTaskMem(Marshal.SizeOf(Options[0])
                + Marshal.SizeOf(Options[1]));

            IntPtr current = buffer;
            for (int i = 0; i < Options.Length; i++)
            {
                Marshal.StructureToPtr(Options[i], current, false);
                int bir = Marshal.SizeOf(Options[i]);
                current += (int)bir;
            }
            INTERNET_PER_CONN_OPTION_LIST option_list = new INTERNET_PER_CONN_OPTION_LIST();
            option_list.pOptions = buffer;
            option_list.Size = Marshal.SizeOf(option_list);
            option_list.Connection = IntPtr.Zero;
            option_list.OptionCount = Options.Length;
            option_list.OptionError = 0;
            int size = Marshal.SizeOf(option_list);
            IntPtr intptrStruct = Marshal.AllocCoTaskMem(size);
            Marshal.StructureToPtr(option_list, intptrStruct, true);
            bool bReturn = InternetSetOption(hInternet,
                INTERNET_OPTION.INTERNET_OPTION_PER_CONNECTION_OPTION, intptrStruct, size);
            Marshal.FreeCoTaskMem(buffer);
            Marshal.FreeCoTaskMem(intptrStruct);
            InternetCloseHandle(hInternet);
            if (!bReturn)
            {
                throw new ApplicationException("Proxy Ayarlarken Hata Oluştu!");
            }
            return bReturn;
        }

        private static INTERNET_PER_CONN_OPTION_LIST SistemProxy()
        {
            INTERNET_PER_CONN_OPTION[] Options = new INTERNET_PER_CONN_OPTION[3];
            Options[0] = new INTERNET_PER_CONN_OPTION();
            Options[0].dwOption = (int)INTERNET_PER_CONN_OptionEnum.INTERNET_PER_CONN_FLAGS;
            Options[1] = new INTERNET_PER_CONN_OPTION();
            Options[1].dwOption = (int)INTERNET_PER_CONN_OptionEnum.INTERNET_PER_CONN_PROXY_SERVER;
            Options[2] = new INTERNET_PER_CONN_OPTION();
            Options[2].dwOption = (int)INTERNET_PER_CONN_OptionEnum.INTERNET_PER_CONN_PROXY_BYPASS;
            System.IntPtr buffer = Marshal.AllocCoTaskMem(Marshal.SizeOf(Options[0])
                + Marshal.SizeOf(Options[1]) + Marshal.SizeOf(Options[2]));

            System.IntPtr current = (System.IntPtr)buffer;
            for (int i = 0; i < Options.Length; i++)
            {
                try
                {
                    Marshal.StructureToPtr(Options[i], current, false);
                    current = (System.IntPtr)((int)current + Marshal.SizeOf(Options[i]));
                }
                catch { }
            }
            INTERNET_PER_CONN_OPTION_LIST Request = new INTERNET_PER_CONN_OPTION_LIST();
            Request.pOptions = buffer;
            Request.Size = Marshal.SizeOf(Request);
            Request.Connection = IntPtr.Zero;
            Request.OptionCount = Options.Length;
            Request.OptionError = 0;
            int size = Marshal.SizeOf(Request);
            bool result = InternetQueryOptionList(IntPtr.Zero,
                INTERNET_OPTION.INTERNET_OPTION_PER_CONNECTION_OPTION,
                ref Request, ref size);
            if (!result)
            {
                try
                {
                    throw new ApplicationException("Proxy Ayarlarken Hata Oluştu!");
                }
                catch { }
            }
            return Request;
        }

        public static bool VarsayılanProxy()
        {
            IntPtr hInternet = InternetOpen(applicationName, INTERNET_OPEN_TYPE_DIRECT, null, null, 0);
            INTERNET_PER_CONN_OPTION_LIST request = SistemProxy();
            int size = Marshal.SizeOf(request);
            IntPtr intptrStruct = Marshal.AllocCoTaskMem(size);
            Marshal.StructureToPtr(request, intptrStruct, true);
            bool bReturn = InternetSetOption(hInternet,
                INTERNET_OPTION.INTERNET_OPTION_PER_CONNECTION_OPTION,
                intptrStruct, size);
            Marshal.FreeCoTaskMem(request.pOptions);
            Marshal.FreeCoTaskMem(intptrStruct);
            if (!bReturn)
            {
                try
                {
                    throw new ApplicationException("Proxy Ayarlarken Hata Oluştu!");
                }
                catch { }
            }
            InternetSetOption(hInternet, INTERNET_OPTION.INTERNET_OPTION_SETTINGS_CHANGED,
                IntPtr.Zero, 0);
            InternetSetOption(hInternet, INTERNET_OPTION.INTERNET_OPTION_REFRESH,
                IntPtr.Zero, 0);
            InternetCloseHandle(hInternet);
            return bReturn;
        }
    }
}
