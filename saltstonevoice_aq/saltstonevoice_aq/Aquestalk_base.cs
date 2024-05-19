using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace saltstonevoice_aq
{

  public abstract class Aquestalk_base
  {
    protected const string dlldir = "AquesTalk";
    protected const string dllfile = "AquesTalk.dll";

    protected int _speed;
    // public int _speed;
    public int speed
    { 
      get
      {
        if(_speed == 0)
        {
          speed = 100;
        }
        return _speed;
      }
      set
      {
        _speed = value;
      }
    }

    protected delegate IntPtr del_AQSynthe(string koe, int iSpeed, out int size);
    protected delegate void del_AQFree(IntPtr wav);
    protected del_AQSynthe func_AQSynthe;
    protected del_AQFree func_AQFree;

    //protected static  void AquesTalk_FreeWave(IntPtr wav)
    //{

    //}
   




    public  bool createwav(string subtitle, MemoryStream data)
    {
      bool fret = false;
      int size;
      // utf8 ok sjis , eucも可
      // subtitle.ToCharArray()が必要かも



      // 32bitのdllなので、64bitとの変換部分が必須となる 
      // ここは共通なんだよねー、、、
      // とすると構造がおかしい
      // 派生先でoverride or newしても、baseのものが呼び出されてしまう
      if(func_AQSynthe == null)
      {
        return fret;
      }
      if (func_AQFree == null)
      {
        return fret;
      }
      if(subtitle.Length == 0)
      {
        return fret;
      }
      //Encoding enc =  Encoding.GetEncoding("shift_jis");
      //Encoding utfenc = Encoding.UTF8;
      //byte[] ubyte = utfenc.GetBytes(subtitle);
      //byte[] sbytea = Encoding.Convert(utfenc, enc, ubyte);
      //string sjisstr = enc.GetString(sbytea);
      // byte[] bufftest = System.Text.Encoding.ASCII.GetBytes(subtitle);
      // byte[] src = enc.GetBytes(subtitle);
      // string srcstr = enc.GetString(src);
      // string buff = "あー";
      // string aa = enc.GetString(subtitle);
      //string xa = "今日";
      // たぶん、文字コード変換が問題だと思うんだが、、、
      // sjisに変換して渡しても105(no hatuon mark)エラーとなる
      // 今日だと sjisで 8D A1 93 FA
      // あー。なるへそ。漢字がだめなんだ
      // utf,sjisはmarsalが自動でおこなっているみたいだね

      IntPtr ret =  func_AQSynthe(subtitle, speed, out size);
      if(ret == IntPtr.Zero)
      {
        // error
        string errmsg = "Aquestalk.DLLでエラーが発生しました[" + size.ToString() + "]";
        saltstone.Logs.write(errmsg);
      }
      // IntPtr ret = AquesTalk_Synthe(subtitle, speed, out size);
      getStream(ret, size, data);
      func_AQFree(ret);

      // aquestalkはこれでいいが、
      // wavデータ作成をサポートしていないvoiceもある
      return true;


    }

    public bool createwav(string subtitle, string wavfname)
    {
      saltstone.Utils.Files.delete(wavfname);

      MemoryStream ms = null;
      FileStream fs = null;

      try
      {
        ms = new MemoryStream();
        bool ret = createwav(subtitle, ms);
        fs = new FileStream(wavfname, FileMode.CreateNew);
        ms.WriteTo(fs);

      } catch (Exception e)
      {
        // string buff = e.Message;
        // TODO log出力をどうするか？
        saltstone.Logs.write(e);
      } finally
      {
        fs?.Close();
        ms?.Close();
        fs?.Dispose();
        ms?.Dispose();

      }

      return true;

    }


    // dll intptrをmanaged memory streamに変換する
    protected MemoryStream getStream(IntPtr pt, int size,MemoryStream data)
    {
      // safe codeにするため、いったんbyte[]にデータをコピー
      // それからmemorystreamへコピーする
      byte[] buff = new byte[size];
      System.Runtime.InteropServices.Marshal.Copy(pt, buff, 0, size);
      data.Write(buff, 0, size);

      /*
      unsafe
      {
        byte* pnt = (byte*)pt.ToPointer();
        byte* buff = pnt;
        // UnmanagedMemoryStream writeStream
        // intptrからsizeを読み込みmemstremへコピー
        data.Write(buff, 0, size);
      }
      */
      /*
      for (int i = 0; i < size; i++)
      {
        data.WriteByte(pt);
      }
      */
      return data;
    
    }




  }
}
