using System;
using System.Runtime.InteropServices;

namespace saltstone
{
    // Wrap native methods.
    internal static class NativeMethods
    {
        #region Constant Values for Windows/IME API

        public const int FELANG_REQ_REV = 0x00030000;
        public const int FELANG_CMODE_HIRAGANAOUT = 0x00000000; // Default output
        public const int FELANG_CMODE_KATAKANAOUT = 0x00000008;
        public const int S_OK = 0x00000000;

        public const int CLSCST = 0x15;

        #endregion

        #region API Structs Declaration

        [StructLayout(LayoutKind.Sequential)]

        // Defines the struct MORRSLT contains many public values.
        public struct MORRSLT
        {
            public uint dwSize;
            public IntPtr pwchOutput;
            public ushort cchOutput;
            public IntPtr rdstr;
            public ushort lenOfRdStr;
            public IntPtr pchInputPos;
            public IntPtr pchOutputIdxWDD;
            public IntPtr arrOfRdCharWDD;
            public IntPtr paMonoRubyPos;
            public IntPtr pWDD;
            public int cWDD;
            public IntPtr pPrivate;
            public IntPtr BLKBuff;
        }

        #endregion

        #region API Methods Declaration

        [DllImport("Ole32.dll")]
        public static extern int CLSIDFromString(
            [MarshalAs(UnmanagedType.LPWStr)]
            string s,
            ref Guid pClsId);

        [DllImport("Ole32.dll")]
        public static extern int CoCreateInstance(
            ref Guid pClsId,
            IntPtr pUnkOuter,
            uint dwClsContext,
            ref Guid riid,
            [Out, MarshalAs(UnmanagedType.Interface)]
            out object pUnk);
        #endregion

    }
}
