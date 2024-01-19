using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;

// dllimportの動的読み込みは行わない（非推奨のため)
// 各自クラスを用意し、静的にdllimportを行う
namespace saltstonevoice_aq
{
  public class Aquestalk_f1 : Aquestalk_base, voiceinterface_aq
  { 
    // private const string dlldir = "AquesTalk";
    // private const string dllfile = "AquesTalk.dll";
    private const string _id = "AQF1";
      
    // [DllImport(dlldir + "\\f1\\" + dllfile, CallingConvention = CallingConvention.Winapi)]
    [DllImport(dlldir + @"\f1\" + dllfile)]
    [SuppressUnmanagedCodeSecurity] // managed -> unmanagedのsecurity check skip
    public static extern IntPtr AquesTalk_Synthe(string koe, int iSpeed, out int size);

    [DllImport(dlldir + @"\f1\" + dllfile)]
    [SuppressUnmanagedCodeSecurity]
    private static extern void AquesTalk_FreeWave(IntPtr wav);
    public string id {
      get {
        return _id;
      }
    }


    public Aquestalk_f1()
    {
      func_AQSynthe = AquesTalk_Synthe;
      func_AQFree = AquesTalk_FreeWave;
    }


    //public override bool createwav(string subtitle, MemoryStream data)
    //{
    //  // subtitleの発音文字をwavにし、dataに保存
    //  int size;
    //  // utf8 ok sjis , eucも可
    //  // subtitle.ToCharArray()が必要かも

    //  // 32bitのdllなので、64bitとの変換部分が必須となる 
    //  // ここは共通なんだよねー、、、
    //  // とすると構造がおかしい
    //  IntPtr ret = AquesTalk_Synthe(subtitle, speed, out size);
    //  getStream(ret, size, data);
    //  AquesTalk_FreeWave(ret);

    //  // aquestalkはこれでいいが、
    //  // wavデータ作成をサポートしていないvoiceもある
    //  return true;
    //}



  }
}
