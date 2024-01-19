using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



// phonticから作るか、、、
// aq用の"/+"を含み、すべてかたかなにするfunc


// voiceのspeed,tone,volumeなどはcharaに緋もづく
// voiceeffectはchara classでれいむ・まりさ毎に持つ必要がある

// SlibVoicesをどこで初期化するか？ globalにもたせるか？



namespace saltstone
{
  public static class VoiceGlobal
  {
    public static Voicessoft pVoices;
    public static Voicessoft voices {
      get {
        if (pVoices == null)
        {
          pVoices = new Voicessoft();
          pVoices.init();
        }
        return pVoices;
      }
    }
    public static bool Contains(string voiceid)
    {
      if (pVoices == null)
      {
        pVoices = new Voicessoft();
      }
      bool ret = pVoices._voices.ContainsKey(voiceid);
      return ret;

    }


  }



  // aqestalk(x86 saltstonevoice_aq)などのdll/apiを使用し
  // text -> phonetictext -> wavを作成する
  // charaのライブラリだが、保守性を考え、ライブラリを分ける
  public class Voicessoft : IDisposable
  {
    // Dictionary<string,vocie> _voices
    public Dictionary<string, Voics> _voices;

    public void Dispose()
    {
      _voices.Clear();
    }

    

    public void init()
    {
      _voices = new Dictionary<string, Voics>();

      DB.Sqlite db = Globals.getSettingDB();
      DB.Query q = new DB.Query();
      q.table = "voice";
      q.select = "voiceid,voicesoft,dllfile,exefname,classname,dispname,memo";
      q.orderby = "orderno";
      DB.DBRecord rec;
      bool ret = db.getrecord_noread(q, out rec);
      if (ret == false)
      {
        return;
      }
      Voics sv;
      string voiceid;
      string voicesoft;
      while (rec.Read() == true)
      {
        // voicesoftがaquestalkであればslibvoice_aqを作成するべき
        voicesoft = rec.getstring(1);
        // TODO classnameからinstanceを作成する
        // Type type = Type.GetType(sv.classname);
        // Voice sv = Activator.CreateInstance(type);
        if (voicesoft == "aquestalk")
        {
          sv = new SlibVoice_AQ();
          
        } else
        {
          sv = new Voics();
        }
        voiceid = rec.getstring(0);
        _voices[voiceid] = sv;
        sv.voiceid = voiceid;
        sv.voicesoft = rec.getstring(1);
        
        sv.dllfile = rec.getstring(2);
        sv.exefilename = rec.getstring(3);
        sv.classname = rec.getstring(4);
        sv.dispname = rec.getstring(5);
        sv.memo = rec.getstring(6);
        // aq x86のexeが実行されていない
        sv.init();
      }
      // ここで実行する？




    }

    public Voics getVoice(string voiceid)
    {
      if (_voices.ContainsKey(voiceid) == false)
      {
        return null;
      }
      return _voices[voiceid];
    }
  }
  public class Voics :IDisposable
  {
    // table voiceを保管するのに利用
    // 後々はSlibVoiceのbase or interfaceにする
    
    /// <summary>
    /// 初期化が行われたかどうか
    /// </summary>
    public bool inited;
    public string voicesoft;
    public string voiceid;
    public string dllfile;
    public string exefilename;
    public string classname;
    public string dispname;
    public string memo;
    // intext -> phonetic textへのconvert class
    public Phonetic phoneticcnv;

    public virtual void init()
    {
    }



    public Dictionary<string, string> _parameter;
    public void Dispose()
    {
      _parameter?.Clear();
      _parameter = null;
    }

    public Dictionary<string, string> parameter
      {
      get {
        if (_parameter == null)
        {
          _parameter = new Dictionary<string, string>();
        }
        return _parameter;
      }
    }
    
    public void setParam(string arg,string value) {
      parameter[arg] = value;
    }

    public string getParam(string arg)
    {
      return parameter[arg];
    }

    public virtual bool makeWave(VoiceText arg)
    {
      return true;
    }


  }

  
}
