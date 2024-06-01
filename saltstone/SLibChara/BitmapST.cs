using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Drawing;


// saltstone c++ dll interface
namespace saltstone
{
  /// <summary>
  ///  salt stone 用のbitmap class
  /// </summary>
  public class SLibBitmap : BitmapInfo,IDisposable
  {

    [DllImport("imaging.dll", EntryPoint = "clipping")]
    public static extern bool clipping(BitmapInfo src, BitmapInfo dst);



    // これ以降はc++に渡したくない 
    // getter setter or pointer? or another class inherit?
    // https://stackoverflow.com/questions/68260141/exclude-extra-private-field-in-struct-with-layoutkind-explicit-from-being-part-o
    string filename; // filenameが指定されている場合
    Bitmap imgbitmap; // bitmapが作成された場合

    public SLibBitmap(string filename)
    {
      this.filename = filename;
      setBitmap(filename);
    }

    public new void Dispose()
    {
      imgbitmap = null;
      base.Dispose();
    }


    public bool setBitmap(string filename)
    {
      imgbitmap = new Bitmap(filename);
      if (imgbitmap == null)
      {
        return false;
      }
      bool ret = this.setBitmap(imgbitmap);
      return ret;
    }


    unsafe public bool getClipping(out Bitmap outbmp)
    {
      // これは chara partsのここのファイル（レイヤー）から呼びされるもの
      // ファイル名の保管もcharapartsで行う

      // outbmpはdisposeしてやらないとgdi+は開放されずごみが残る
      // 呼び出し元で制御する

      outbmp = null;
      // src=this dst=outbmp
      if (data == null)
      {
        return false;
      }
      // cliping用のバッファを用意する
      BitmapInfo dst = new BitmapInfo(this);
      // c++でclipping処理
      bool ret = clipping((BitmapInfo)this, dst);

      // mangeed用のbitmapを用意
      outbmp = new Bitmap(dst.width, dst.height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

      // c++からのrgba32をbitmapにコピー
      Rectangle rect = new Rectangle(0, 0, dst.width, dst.height);
      System.Drawing.Imaging.BitmapData bmpData =
         outbmp.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, outbmp.PixelFormat);

      datalength = bmpData.Stride * dst.height;
      Copy(bmpData.Scan0);
      // Utils.memory.copy(dst.data, bmpData.Scan0, );
      outbmp.UnlockBits(bmpData);

      dst.Dispose();
      

      // この後、cacheに保存

      return true;
    }




  }

}
