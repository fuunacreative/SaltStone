using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace saltstone
{
  class SenarioMDFile
  {
    const string FileExtension = ".md";

    private Aviutl au;
    private string senariofile;
    public string mdfile;

    private enum parsetype
    {
      global, globalvoice, globalvoiceEffect, globalfaceexp, globalcredit, memo, chara, charapicture, scene
    }


    public SenarioMDFile(Aviutl auobj = null)
    {
      au = auobj;
    }

    // sceneは#、subsceneは##
    // セリフは
    // r）字幕[[発音,speed,tone,vol]]@@立ち絵^^excmd
    // r）やっぱり冬だね[[やっぱりふゆだね,100,110,100]]@@set:04^^img=c:\a.png
    // 解析しやすいかどうか？
    // 可読性があるかどうか


    public bool Save(string savefile = "")
    {
      if (savefile == "")
      {
        // projectdir + projectfile + .md
        senariofile = au.projectdir + "\\" +  Utils.Files.getbasename(au.aupfile) + ".md";
      } else
      {
        senariofile = savefile;
      }
      // TODO txtのシナリオファイルとmdのシナリオファイルの２つある
      mdfile = senariofile;

      // TODO pathの存在チェック、シナリオファイルが書き込めるかどうかチェック

      // 画面のscene , lstsubtitleのデータをDatasに保存

      // Datasにシーン、セリフが入ってる
      // TODO Datasのシーンはツリー構造には対応してない
      string buff = ""; // writeするbuffer
      // stream writ:
      // bomなしutf8にする
      StreamWriter fs = new StreamWriter(mdfile);
      string scname;
      Scenes s;
      foreach(KeyValuePair<string,Scenes> kvp in Datas.scenes)
      {
        scname = kvp.Key;
        s = kvp.Value;
        buff = "# " + scname;
        fs.WriteLine(buff);
        // TODO sub nodeがある場合の処理
        foreach (Quote q in s.messages)
        {
          buff = q.charaid + "）";
          buff += q.message + "[[";
          buff += q.pronmessage + "," + q.speed + "," + q.tone + "," + q.volume + "]]";
          buff += "@@" + q.charafacestr;
          buff += "^^" + q.othercommand;
          fs.WriteLine(buff);
        }
        // 可読性をあげるため、シーンが終わったら改行を出力
        fs.WriteLine(Environment.NewLine);

      }
      fs.Close();
      fs.Dispose();
      fs = null;
      
      
      return true;
    }

    public bool Load(string loadfile = "")
    {
      string filename = loadfile;
      if (Utils.Files.exist(filename, Utils.Files.filesearchmode.fileonly) == false)
      {
        return false;
      }

      string alltext = System.IO.File.ReadAllText(filename);
      // Logs.writelog(alltext);
      List<string> lines = new List<string>(alltext.Replace("\r\n", "\n").Split(new[] { '\n', '\r' }));

      Datas.Clear();

      // シーンファイルの解析
      parsetype mode = parsetype.global;
      string scenename = "";
      string buff;
      string voice;
      string[] ary;
      string[] voicedef;
      int i;
      string line;
      Scenes s = null;
      Quote q;
      foreach (string l in lines)
      {
        line = l;
        if (l.Length == 0)
        {
          continue;
        }
        if (l.Substring(0, 1) == "#")
        {
          // sceneの定義
          scenename = l.Substring(2);
          mode = parsetype.scene;
          s = new Scenes();
          s.scenename = scenename;
          //Datas.scenes.Add(s);
          Datas.addscene(scenename, s);
          continue;
        }
        if (mode == parsetype.scene)
        {
          // セリフの切り分け
          // r）字幕[[発音,speed,tone,vol]]@@立ち絵^^excmd
          ary = line.Split(new string[] { "^^" }, StringSplitOptions.RemoveEmptyEntries);
          if (ary.Length == 0)
          {
            continue;
          }
          q = new Quote();
          s.messages.Add(q);
          if (ary.Length == 2)
          {
            q.othercommand = ary[1];
          }
          buff = ary[0];
          ary = buff.Split(new string[] { "@@" }, StringSplitOptions.RemoveEmptyEntries);
          if (ary.Length == 0)
          {
            continue;
          }
          if (ary.Length == 2)
          {
            q.charafacestr = ary[1];
          }
          buff = ary[0];
          ary = buff.Split(new string[] { "[[" }, StringSplitOptions.RemoveEmptyEntries);
          if (ary.Length == 0)
          {
            continue;
          }
          if (ary.Length == 2)
          {
            voice = ary[1];
            // 最後の]]を取り除く
            i = voice.IndexOf("]");
            voice = voice.Substring(0, i);
            // TODO 発音にコンマが含まれていると誤動作する
            voicedef = voice.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            do
            {
              if (voicedef.Length == 0)
              {
                break;
              }
              q.pronmessage = voicedef[0];
              if (voicedef.Length <= 1)
              {
                break;
              }
              q.speed = Utils.toint(voicedef[1]);
              if (voicedef.Length <= 2)
              {
                break;
              }
              q.tone = Utils.toint(voicedef[2]);
              if (voicedef.Length <= 3)
              {
                break;
              }
              q.volume = Utils.toint(voicedef[3]);


            } while (false);
          }
          buff = ary[0];
          ary = buff.Split(new string[] { "）" }, StringSplitOptions.RemoveEmptyEntries);
          if (ary.Length == 0)
          {
            continue;
          }
          q.charaid = ary[0];
          if (ary.Length <= 1)
          {
            continue;
          }
          // 改行コードが含まれていると誤動作する
          q.message = ary[1];



        }

      }
      return true;
    }
  }
}
