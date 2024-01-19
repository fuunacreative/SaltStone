using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;


namespace saltstonevoice_aq
{
  public interface voiceinterface_aq
  {
    // public const string dllfile = "";

    // settings.db table voice
    string id {
      get;
    }


    // 字幕(subtitle)/発音記号よりwav dataをmemory上に作成する
    bool createwav(string subtitle,MemoryStream data);

    // 字幕より合成した音声をwatとしてoutfnameに保管する
    bool createwav(string subtitle, string outfname);

    // aquestalkではspeedのみパラメータとして指定可能
    int speed
    {
      get; set;
    }


  }

 
}
