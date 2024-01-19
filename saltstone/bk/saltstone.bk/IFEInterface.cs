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
        void Open();

        // Closes the interface to users.
        void Close();

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
    }
}
