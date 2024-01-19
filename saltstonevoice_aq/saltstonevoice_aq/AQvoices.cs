using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace saltstonevoice_aq
{
  public static class AQvoices
  {
    public static Dictionary<string, voiceinterface_aq> voices;

    public static void init()
    {
      voices = new Dictionary<string, voiceinterface_aq>();

      // dbよりvoiceidを読み込み,class名も読み込む
      string dbpath = "settings.db"; 
      DB.Sqlite db = new DB.Sqlite(dbpath);

      DB.Query q = new DB.Query();
      q.table = "voice";
      q.select = "voiceid,exefname,classname";
      q.where("voicesoft", "aquestalk");
      q.where("exefname", "saltstonevoice_aq.exe");
      DB.DBRecord rec;
      bool ret = db.getrecord_noread(q,out rec);
      if (ret == false)
      {
        return;
      }
      voiceinterface_aq voiceinst;
      Type objType;
      string vclass;
      string voiceid;
      while (rec.Read() == true)
      {
        // classを実態化してdic voicesに格納
        voiceid = rec.getstring(0);
        vclass = rec.getstring(2);
        objType = Type.GetType(vclass);
        if(objType == null)
        {
          continue;
        }
        voiceinst = (voiceinterface_aq)Activator.CreateInstance(objType);
        voices[voiceid] = voiceinst;
      }




      // operator []でaccess可能にする
    }

    public static voiceinterface_aq getVoice(string voiceid)
    {
      if(voices.ContainsKey(voiceid) == false)
      {
        return null;
      }
      return voices[voiceid];
    }

  }
}
