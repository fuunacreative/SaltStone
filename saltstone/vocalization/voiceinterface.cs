using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections.Generic;

// dvd 機械２
// f1 女性１
// f2 女性２
// imd1 中性
// jgr 機械１
// m1 男性２
// m2 男性１
// r1 ロボット


namespace vocalization
{
  public interface voiceinterface
  {
    // public const string dllfile = "";

    string dispname 
    {
      // 表示名
      get;
    }
    
    string id {
      get;
    }


    // 字幕(subtitle)/発音記号よりwav dataをmemory上に作成する
    bool createwav(string subtitle,MemoryStream data);

    // 字幕より合成した音声をwatとしてoutfnameに保管する
    bool createwav(string subtitle, string outfname);


    // wavを作成するにあたっての各種パラメータ
    // aquestalkならtone,volume,speedの３つ
    bool setparameter(Dictionary<string, string> parmas);

  }
}
