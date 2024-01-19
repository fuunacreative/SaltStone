using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;

namespace vocalization
{
  public class Aquestalk_f1 : Aquestalk_base,voiceinterface
  {
    // private const string dlldir = "AquesTalk";
    // private const string dllfile = "AquesTalk.dll";
    private const string _dispname = "女性１";
    private const string _id = "AQ_F1";

    [DllImport(dlldir + "\\f1\\" + dllfile, CallingConvention = CallingConvention.Cdecl)]
    [SuppressUnmanagedCodeSecurity]
    public static extern IntPtr AquesTalk_Synthe(string koe, int iSpeed, out int size);

    [DllImport("AquesTalk\\f1\\AquesTalk.dll")]
    [SuppressUnmanagedCodeSecurity]
    private static extern void AquesTalk_FreeWave(IntPtr wav);
    public string dispname 
    {
      get {
        return _dispname;
      }
    }
    public string id {
      get {
        return _id;
      }
    }

    protected override IntPtr Synthe(string koe, int iSpeed, out int size)
    {
      IntPtr retp = AquesTalk_Synthe(koe, iSpeed, out size);
      return retp;
    }
    protected override void free(IntPtr pt)
    {
      AquesTalk_FreeWave(pt);
    }

    public override bool createwav(string subtitle, MemoryStream data)
    {
      // subtitleの発音文字をwavにし、dataに保存
      int size;
      // utf8 ok sjis , eucも可
      // subtitle.ToCharArray()が必要かも

      // 32bitのdllなので、64bitとの変換部分が必須となる
      IntPtr ret = AquesTalk_Synthe(subtitle, speed, out size);
      getStream(ret, size, data);
      AquesTalk_FreeWave(ret);

      // aquestalkはこれでいいが、
      // wavデータ作成をサポートしていないvoiceもある
      return true;
    }

    public bool createwav(string subtitle, string outfname)
    {
      MemoryStream ms = new MemoryStream();
      bool ret = createwav(subtitle, ms);

      return true;
    }

    private int speed = 100;
    private int tone;

    public bool setparameter(Dictionary<string, string> param)
    {
      // speed と　toneの関係は？
      // speedを上げればtoneはあがる
      // これのサンプリング数を引き伸ばせば高い音のまま・スピードが同じままで高い声になる


      speed = 100;
      bool ret;
      foreach (KeyValuePair<string, string> kvp in param)
      {
        if (kvp.Key == "speed")
        {
          ret = int.TryParse(kvp.Key, out speed);
        }

      }

      return true;
    }


  }
}
