using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

// mmfで使用するデータ連携用の本体
// 課題 c++はマルチバイト文字 おそらくsjis 、c#はutf8
// 文字コードを判定する必要がある -> readjenc
// 一番よいのはstructにして、一括してシリアライズできるとよい
// しかし、、、c++ c#の相互互換のseriaalizeはかなり難しそう 文字コード変換の問題もある
// c++側でのロジック作成が必要なのであとまわし
// 起動時のログ書き込みもね

namespace saltstone
{
  // 全体サイズ
  // 固定長string 10バイト
  // ヘッダサイズ(int, str , bitmap)
  // int arrayの(
  public class LinkData
  {
    public int totalsize; // ４バイト
    public string datatype; // 6バイト ascii
    public int headersize;
    public Header header;
    public int[] intary;
    // public strary null termへのstring dataへのoffset + size
    public List<Stritem_Heaer> strheader;
    public string[] strary;

    public byte[] bmpdata;
    public IntPtr bmpraw;
    public Bitmap bmp;

    // readしながら作る
    // 問題 stringのserializeが複雑。解析が必要

    [SupportedOSPlatform("windows")]
    public bool read(BinaryReader r)
    {
      // readerはmmfのreader
      totalsize = r.ReadInt32();
      // TODO total sizeのチェックはどうするか？
      byte[] bytebuff = r.ReadBytes(6); // type ascii
      datatype = Encoding.ASCII.GetString(bytebuff).TrimEnd((Char)0);
      headersize = r.ReadInt32();
      bytebuff = r.ReadBytes(headersize);
      header = new Header();
      header.read(bytebuff);
      bytebuff = r.ReadBytes(header.blocksize_int);
      intary = Array.ConvertAll(bytebuff, Convert.ToInt32);
      // string arrayは問題だな
      // 固定長のoffset + sizeにするべきだな
      strheader = new List<Stritem_Heaer>();
      Stritem_Heaer sh;
      for (int i = 0; i < header.arraysize_str; i++)
      {
        sh = new Stritem_Heaer();
        sh.offset = r.ReadInt32();
        sh.size = r.ReadInt32();
        strheader.Add(sh);
      }
      string buff;
      foreach (Stritem_Heaer s in strheader)
      {
        bytebuff = r.ReadBytes(s.size + 1); // null termのため+1
        // TODO c++ではsjisと思われる utf8にしてmmfに乗せる
        buff = Encoding.UTF8.GetString(bytebuff).TrimEnd((Char)0);
        strary.Append<string>(buff);
      }

      // bitmap rgba32 -> bitmapへ変換
      // bmpraw = r.BaseStream.Position
      bmpdata = r.ReadBytes(header.blocksize_bmp);
      bmp = new Bitmap(header.bmp_width, header.bmp_height,System.Drawing.Imaging.PixelFormat.Format32bppArgb);
      BitmapData bmpData = bmp.LockBits(
                    new Rectangle(0, 0, bmp.Width, bmp.Height),
                    ImageLockMode.WriteOnly, bmp.PixelFormat);
      Marshal.Copy(bmpdata, 0, bmpData.Scan0, bmpdata.Length);
      bmp.UnlockBits(bmpData);

      return true;
    }

  }

  public class Header
  {
    public int offset_int; // データblockの先頭からのoffset
    public int blocksize_int; // 全体のバイト数
    public int arraysize_int; // offset(int)のary数
    public int offset_str; // データblockの先頭からのoffset
    public int blocksize_str; // 全体のバイト数
    public int arraysize_str; // offset(int)のary数
    public int offset_bmp;
    public int bmp_width;
    public int bmp_height;
    public int blocksize_bmp;

    public bool read(byte[] header)
    {
      MemoryStream ms = null;
      BinaryReader bs = null;
      try
      {
        ms = new MemoryStream(header);
        bs = new BinaryReader(ms);
        offset_int = bs.ReadInt32();
        blocksize_int = bs.ReadInt32();
        arraysize_int = bs.ReadInt32();
        offset_str = bs.ReadInt32();
        blocksize_str = bs.ReadInt32();
        arraysize_str = bs.ReadInt32();
        offset_bmp = bs.ReadInt32();
        bmp_width = bs.ReadInt32();
        bmp_height = bs.ReadInt32();
        blocksize_bmp = bs.ReadInt32();
      }
      finally
      {
        ms?.Dispose();
        bs?.Dispose();
      }

      return true;
    }

  }


  public class Stritem_Heaer
  {
    public int offset;
    public int size;
  }

}
