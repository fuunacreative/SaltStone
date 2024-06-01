using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Drawing;
using Windows.Devices.Perception.Provider;

namespace saltstone
{

  [StructLayout(LayoutKind.Sequential)]
  public class BitmapInfo : IDisposable
  {
    // static const int maxwidth = 6000; // 5k 5120
    // static const int maxheight = 6000; // 5k 2880
    public const int maxwidth = 6000;
    public const int maxheight = 6000;



    // pixelformat は必ずARGB32 byte orderは B->G->R->A
    public int width;
    public int height;
    public int datalength;
    unsafe public void* data = null;
    // これ以降はc++に渡したくない 
    // getter setter or pointer? or another class inherit?
    // https://stackoverflow.com/questions/68260141/exclude-extra-private-field-in-struct-with-layoutkind-explicit-from-being-part-o
    // string filename; // filenameが指定されている場合
    // Bitmap imgbitmap; // bitmapが作成された場合

    public BitmapInfo()
    {
    
    }

    public BitmapInfo(BitmapInfo arg)
    {
      // 中身をコピーする
      width = arg.width;
      height = arg.height;
      // data = Utils.Memory.alloc(arg.datalength);
      // native memoryを使うしかないのかな？
      Alloc(arg.datalength);

      datalength = arg.datalength;
    }

  public void Dispose()
    {
      // Utils.memory.free(data);
      Free();
    }

    unsafe public void Alloc(int len)
    {
      Free();
      data = SLibMemory.Memory.Alloc(len);
    }

    unsafe public void Free()
    {
      if (data != null)
      {
        SLibMemory.Memory.Free(data);
      }
    }

    /// <summary>
    /// argからmemger dataへmember datalength分をmemory copyする
    /// </summary>
    /// <param name="arg"></param>
    unsafe public void Copy(IntPtr arg)
    {
      SLibMemory.Memory.Copy(data, arg, datalength);
    }

    /// <summary>
    /// argのbitmapから dataへコピーする
    /// 高速化のため native memoryを利用している
    /// </summary>
    /// <param name="arg"></param>
    /// <returns></returns>
    public bool setBitmap(Bitmap arg)
    {
      width = arg.Width;
      height = arg.Height;

      Rectangle rect = new Rectangle(0, 0, width, height);
      // argをcloneする必要があるか？
      // Bitmap b = orgb.Clone(rect, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

      System.Drawing.Imaging.BitmapData bmpData =
    arg.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

      IntPtr ptr = bmpData.Scan0;
      
      datalength = Math.Abs(bmpData.Stride) * height;
      // 一応、managed memoryに直接c++からアクセスさせたくないのでコピーしておく
      
      // dataが確保されている場合を考慮し、いったんクリア
      Free();
      Alloc(datalength);
      // Utils.win32api.CopyMemory(ptr, data, (uint)datalength);
      Copy(ptr);
      // Utils.win32api.CopyMemory(ptr, data, (uint)datalength);
      arg.UnlockBits(bmpData);

      return true;
    }

  }


}
