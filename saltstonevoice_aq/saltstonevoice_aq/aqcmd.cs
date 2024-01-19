using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using saltstone; 

namespace saltstonevoice_aq
{
  public class AQcmd
  {
    public string jobid;
    public string voiceid;
    public int speed;
    public string text;
    public string outfname;

    public AQcmd()
    {

    }

    public AQcmd(AqcmdMemstrucure arg)
    {
      jobid = arg.jobid;
      voiceid = arg.voiceid;
      speed = arg.speed;
      text = arg.phonetictext;
      outfname = arg.outwavfile;
    }

    public bool parse(string argfname)
    {
      bool fret = false;

      System.Text.Encoding enc = new System.Text.UTF8Encoding();
      string buffall = System.IO.File.ReadAllText(argfname, enc);
      if(buffall.Length == 0)
      {
        return fret;

      }

      string[] lineary = buffall.Replace("\r\n", "\n").Split(new[] { '\n', '\r' });
      buffall = null;
      int i;
      string line;
      string[] ary;
      foreach (string l in lineary)
      {
        if (l.Length == 0)
        {
          continue;
        }
        if (l.Substring(0, 2) == "//")
        {
          continue;
        }
        line = l;
        i = line.IndexOf("//");
        if (i > 0)
        {
          line = line.Substring(0, i - 1);
        }
        ary = line.Split('=');

        // classを作る？ 何の？ aqcmd.txtの解析->処理に必要な情報の保管
        // これとは別にaq dll用のclassが必要
        // queue化する？
        if (ary[0] == "jobid")
        {
          jobid = ary[1];
          continue;
        }
        if (ary[0] == "voiceid")
        {
          voiceid = ary[1];
          continue;
        }
        if (ary[0] == "speed")
        {
          bool ret = int.TryParse(ary[1], out i);
          speed = 100;
          if (ret == true)
          {
            speed = i;
          }
          continue;
        }
        if (ary[0] == "text")
        {
          // ""で区切られていると過程する
          // 前後の"をtrimする
          text = ary[1].Trim('"');
          // aq1ではsjisにしか対応していない
          continue;
        }
        if (ary[0] == "wavfile")
        {
          outfname = ary[1];
          continue;
        }

      }

      fret = true;
      return fret;
    }

    public bool createwav()
    {
      bool fret = false;

      voiceinterface_aq voice = AQvoices.getVoice(voiceid);
      voice.speed = speed;
      bool ret = voice.createwav(text,outfname);
      if(ret == false)
      {
        // TODO logをどうするか？ 32bit版の簡易logを作成する方向で
        return fret;
      }


      fret = true;
      return fret;

    }
    //public static void exectxtcmd(string msg)
    //{
    //  // msg形式 aqtxtcmd,10(jobid),incmdtxt = txtファイルによるwav出力指示

    //  string[] ary = msg.Split(',');
    //  if(ary.Length != 3)
    //  {
    //    saltstone.Logs.write("不正な命令です[" + msg + "]");
    //    return;
    //  }
    //  int jobid;
    //  bool ret = int.TryParse(ary[1], out jobid);
    //  if(ret == false)
    //  {
    //    msg = "aquestalk_x86" + "不正なjobidです[" + ary[1] + "]";
    //    Logs.write(msg);
    //    return;
    //  }
    //  string incmd = ary[2];
    //  AQcmd cmd = new AQcmd();
    //  cmd.parse(incmd);
    //  // voiceidとvoice objectをどうするか？

    //  // ret = cmd.createwav(voice);

    //}

  }

}
