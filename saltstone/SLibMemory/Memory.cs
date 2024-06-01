namespace SLibMemory
{
  public static class Memory
  {
    unsafe public static void* Alloc(int arg)
    {
      nuint len = (nuint)(sizeof(byte) * arg);
      void* ret = System.Runtime.InteropServices.NativeMemory.Alloc(len);
      return ret;
    }

    unsafe public static void Free(void* ptr)
    {
      System.Runtime.InteropServices.NativeMemory.Free(ptr);
    }

    unsafe public static void Copy(void* src, IntPtr dst ,int len)
    {
      nuint i = (nuint)len;
      void* dstarg = dst.ToPointer();
      System.Runtime.InteropServices.NativeMemory.Copy(src, dstarg, i);
    
    }
  }
}
