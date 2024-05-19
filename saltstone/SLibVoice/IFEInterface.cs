using System;
using System.Runtime.InteropServices;

namespace saltstone
{
    // Represents the IFELanguage2 interface.
    [Guid("21164102-C24A-11d1-851A-00C04FCC6B14")]//IFEInterface2
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [ComVisible(true)]


    internal interface IFEInterface
    {


        // Opens the interface to users.
        int Open();

        // Closes the interface to users.
        int Close();

        // Declares the return result of the IME.
        [PreserveSig]
        int GetJMorphResult(
            Int32 dwRequest,
            Int32 dwCMode,
            Int32 cwchInput,
            [MarshalAs(UnmanagedType.LPWStr)]
            string pwchInput,
            IntPtr pfCInfo,
            out IntPtr ppResult);

        // Omits other methods.
        int GetConversionModeCaps(ref uint pdwCaps);

        /*
        [return: MarshalAs(UnmanagedType.BStr)]
        string GetPhonetic([MarshalAs(UnmanagedType.BStr)] string str, int start, int length);
        */
        int GetPhonetic([MarshalAs(UnmanagedType.BStr)] string @string, int start, int length, [MarshalAs(UnmanagedType.BStr)] out string result);


        int GetConversion([MarshalAs(UnmanagedType.BStr)] string @string, int start, int length, [MarshalAs(UnmanagedType.BStr)] out string result);


    }
}
