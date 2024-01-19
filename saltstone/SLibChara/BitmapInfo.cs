using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Drawing;

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
    public IntPtr data = IntPtr.Zero;
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
      data = Utils.memory.alloc(arg.datalength);
      datalength = arg.datalength;
    }

    public void Dispose()
    {
      Utils.memory.free(data);
    }

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
      Utils.memory.free(data); // dataが確保されている場合を考慮し、いったんクリア
      data = Utils.memory.alloc(datalength);
      // Utils.win32api.CopyMemory(ptr, data, (uint)datalength);
      Utils.memory.copy(ptr, data, datalength);
      // Utils.win32api.CopyMemory(ptr, data, (uint)datalength);
      arg.UnlockBits(bmpData);

      return true;
    }

  }


}
