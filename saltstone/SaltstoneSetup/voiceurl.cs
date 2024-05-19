using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace saltstone
{
  public class voiceDefines
  {
    public const string VoiceSoftdir = "Voice";

    public string id;
    public string name;
    public string talkEngine;
    public string url;
    public string downloadurl;
    public string xpath;
    public string destdir;
    public string setup;
    public bool installflag;
    public string version;
    public string exefile; // shortcut用
    public bool existfile; // exefileの存在チェック

    // 画面表示用
    public int rowindex;
  }

}
