using System;
using System.Collections.Generic;




// TODO volumeタグの識別
// TODO imageタグの識別
// TODO mdファイルとtxtファイルで解析方法が違う 特にコメントである#

namespace saltstone
{



  static class infileparser
  {


    private enum parsetype
    {
      global, globalvoice, globalvoiceEffect, globalfaceexp, globalcredit, memo, chara, charapicture, scene
    }

    public static bool parse(string infile)
    {
      // Logs.write(infile);

      if (System.IO.File.Exists(infile) == false)
      {
        return false;
      }
      // シーンファイルの読み込み
      string filename = infile;
      if (filename.Length == 0)
      {
        return false;
      }
      // やっぱりここで再読み込みしてるな　なぜ、展開時に際読み込みされないか?
      // fromのcheckboxで判定していて、チェックがついてないからスキップされてる
      string alltext = System.IO.File.ReadAllText(filename);
      // Logs.writelog(alltext);
      List<string> lines = new List<string>(alltext.Replace("\r\n", "\n").Split(new[] { '\n', '\r' }));

      Datas.Clear();

      // シーンファイルの解析
      parsetype mode;
      string scenename = "";
      string buff;
      int i;

      mode = parsetype.global;
      Scenes s = null;
      foreach (string l in lines)
      {
        string line = l;
        if (l.Length == 0)
        {
          continue;
        }
        if (l.Substring(0, 1) == "#")
        {
          continue;
        }
        // 全角＃も削除対象
        if (l.Substring(0, 1) == "＃")
        {
          continue;
        }
        // コメント削除
        if (l.Contains("#") == true)
        {
          i = l.IndexOf("#");
          line = l.Substring(0, i).Trim();
        }
        // emacsでのutf8認識のための識別をいれる
        // /*で始まる場合、スキップ
        i = l.Length;
        if (i > 1)
        {
          buff = l.Substring(0, 2);
          if (buff == "/*")
          {
            continue;
          }
        }

        // line を trimする
        line = line.Trim();

        // 先頭が全角＃の場合スキップする
        // if (line.Substring(0,1) == "＃")
        // {
        //     continue;
        // }

        // Logs.writelog(l);
        if (line.Contains("[global]"))
        {
          // globalセクション
          mode = parsetype.global;
          continue;
        }
        // Logs.write(l);
        if (line.Contains("[global.voice]"))
        {
          mode = parsetype.globalvoice;
          Logs.write("vicedefs");
          continue;
        }
        if (line.Contains("[global.face"))
        {
          mode = parsetype.globalfaceexp;
          continue;
        }
        if (line.Contains("[global.credit]"))
        {
          mode = parsetype.globalcredit;
          continue;
        }
        if (line.Contains("[global.memo]"))
        {
          mode = parsetype.memo;
          continue;
        }
        if (line.ToLower().Contains("[memo"))
        {
          mode = parsetype.memo;
          continue;
        }
        if (line.ToLower().Contains("[global.chara"))
        {
          mode = parsetype.chara;
          continue;
        }
        if (line.ToLower().Contains("[global.charapicture"))
        {
          mode = parsetype.charapicture;
          continue;
        }
        // ボイスエフェクトを追加
        if (line.ToLower().Contains("global.voiceeffect"))
        {
          mode = parsetype.globalvoiceEffect;
          continue;
        }
        // sectionがmemoの場合、何の文字が入っていても読み飛ばし
        if (mode == parsetype.memo)
        {
          continue;
        }
        if (line.Contains("[scene"))
        {
          mode = parsetype.scene;
          // 切り分けをシーンモードに設定し、シーン名を切り出す
          i = line.IndexOf("e ");
          int j = line.IndexOf("]");
          // # pg [sectionで、]が含まれていない場合、 "[section "以降の文字をセクション名とする
          if (j == -1)
          {
            scenename = line.Substring(i + 1).Trim(); //  空白は無視する
          } else
          {
            scenename = line.Substring(i + 2, j - i - 1).Trim(']').Trim();
          }

          // Logs.writelog("シーン名[" + scenename + ":");
          // これで]以降に#があっても無視されるはず

          s = new Scenes();
          s.scenename = scenename;
          Datas.addscene(scenename, s);
          continue;
        }
        if (mode == parsetype.global)
        {
          // globalセクションの解析
          // title,subtitile,baseを想定
          string[] ary = line.Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
          // List<string> b = new List<string>(line.Split(new[] { '=' }));
          if (ary[0] == "")
          {
            continue;
          }
          if (ary.Length != 2)
          {
            //  globalセクションで=が含まれない場合は無視する
            continue;
          }
          if (ary[1] == "")
          {
            continue;
          }
          if (Globals.globalsection.ContainsKey(ary[0]) == false)
          {
            Globals.globalsection.Add(ary[0], ary[1]);
          }
          if (ary[0] == "config")
          {
            // baseの設定ファイルが指定されているのでこれを読み込む
            buff = System.IO.Path.GetDirectoryName(infile);
            buff += "/" + ary[1];
            infileparser.parse(buff);
          }
          if (ary[0] == "charbasedir")
          {
            // キャラ素材ディレクトリの指定
            // char a = System.IO.Path.DirectorySeparatorChar;
            buff = ary[1].Replace("/", "\\");
            // aviutildirの別名置換
            string dirs = System.IO.Path.GetDirectoryName(Globals.envini["avipath"]);

            buff = buff.Replace("%AVIUTILDIR%", dirs);
            // dirの存在チェック
            if (System.IO.Directory.Exists(buff) == true)
            {
              Globals.envini["chardir"] = buff;
            }
          }
          continue;
        }

        if (mode == parsetype.globalvoice)
        {
          //if (line.Contains("deley"))
          //{
          //  // 音声
          //}
          //i = line.IndexOf("=");
          //if (i == 0)
          //{
          //  continue;
          //}


          //// Voicedef a = new Voicedef();
          //string code = l.Substring(0, i);
          //buff = l.Substring(i + 1);
          //// Logs.writelog("バッファ" + buff);
          //// Logs.write("省略文字 " + a.code);
          //// a.volume = Voice.defaultvolume;
          //// a.speed = Voice.defaultspeed;
          //// a.tone = Voice.defaulttone;
          //i = buff.IndexOf("{");
          //if (i == 0)
          //{
          //  Logs.write("ボイス定義に表示名がありません");
          //  continue;
          //}
          //string sourcename = buff.Substring(0, i); 
          //// 表示名の切り出し
          //                                          // Logs.write("code,source[" + code + "," + sourcename);
          //// code=台本上でのキャラ名=声質名
          //// sourcename=aquestalkのvoiceid F1とか
          //// 台本(senario)で定義している、、、
          //// Voice a = new Voice(code, sourcename);
          //// voiceidはどこで定義するべきか？
          //// 台本 or 事前に定義　事前定義が自然だねー


          //// =以降の処理
          //// Logs.write("ソース名 " + a.sourcename);
          //// 
          //buff = buff.Substring(i + 1);
          //// Logs.writelog("バッファ" + buff);


          //i = buff.IndexOf("}");
          //a.dispname = a.sourcename;
          //// Datas.voicelist.Add(a); なぜコメントアウト？
          //if (i == 0)
          //{
          //  continue;
          //}
          //a.dispname = buff.Substring(0, i); // 声質表示名の切り出し
          //                                   // Logs.write("表示名 " + a.dispname);
          //buff = buff.Substring(i + 1);
          //// Logs.write("バッファ" + buff);
          //if (buff.Length == 0)
          //{
          //  continue;
          //}
          //List<string> b = new List<string>(buff.Split(new[] { ',' }));
          //// Logs.write(b.Count.ToString());

          //if (b.Count == 0)
          //{
          //  continue;
          //}

          //foreach (string param in b)
          //{
          //  if (param.Length == 0)
          //  {
          //    continue;
          //  }
          //  // Logs.write(param);
          //  i = param.IndexOf("=");
          //  if (i == 0)
          //  {
          //    continue;
          //  }
          //  if (param.Contains("TONE"))
          //  {
          //    try
          //    {
          //      a.tone = int.Parse(param.Substring(i + 1));
          //    }
          //    catch
          //    {
          //      Logs.write("infileparser tone paseint error");
          //    }
          //  }
          //  if (param.Contains("SPEED"))
          //  {
          //    try
          //    {
          //      a.speed = int.Parse(param.Substring(i + 1));
          //    }
          //    catch
          //    {
          //      Logs.write("inifileparset speed paseint error");
          //    }

          //  }

          //}
          //// Logs.write("tone " + a.tone.ToString());


        }
        // voice effectの解析
        if (mode == parsetype.globalvoiceEffect)
        {
          //// コメントの切り出し #以降は無視
          //i = 0;
          //i = line.IndexOf("#");
          //if (i > 0)
          //{
          //  line = line.Substring(0, i);
          //}
          //i = line.IndexOf("＃");
          //if (i > 0)
          //{
          //  line = line.Substring(0, i);
          //}
          //i = line.IndexOf("=");
          //if (i == -1)
          //{
          //  // =がないは解析不能と判断し、スキップ
          //  continue;
          //}
          //// iがちゃんと先頭から確認されているか？
          //string key = line.Substring(0, i);
          //buff = line.Substring(i + 1);
          //// valueを解析 ,で切り分け
          //string[] ary = buff.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
          //VoiceEffect ve = new VoiceEffect();
          //foreach (string sl in ary)
          //{
          //  i = sl.IndexOf("=");
          //  if (i == -1)
          //  {
          //    continue; // 音声の解析不能
          //  }
          //  string param = sl.Substring(0, i);
          //  string val = sl.Substring(i + 1);
          //  bool ret;
          //  switch (param.ToLower())
          //  {
          //    case "speed":
          //      ret = int.TryParse(val, out ve.speed);
          //      // 失敗しても無視
          //      break;
          //    case "tone":
          //      ret = int.TryParse(val, out ve.tone);
          //      break;
          //  }
          //}
          //// 発声　効果の登録
          //VoiceEffects.add(key, ve);

        }
        if (mode == parsetype.globalfaceexp)
        {
          // r=まりさ 立ちえのサブディレクトリ名と略記号の設定
          // これって、voiceと別にする必要あるのかなー？
          // キャラの立ちえっていう感じだよねー
          // voiceとひとまとめにすると、base.configがややこしくなりすぎるな
          // voiceの表示名ってつかってないから、これに立ちえを設定すればよさそう

        }
        if (mode == parsetype.scene)
        {
          // シーンの解析 シーンとメッセージ、発音文字、顔省略文字を登録していく
          if (s == null)
          {
            Logs.write("not current scene");
            continue;
          }
          // Logs.write(l);
          i = line.IndexOf("）");
          if (i == -1)
          {
            i = line.IndexOf(")");
            if (i == -1)
            {
              continue;
            }
          }
          Quote m = new Quote();
          m.voicecode = line.Substring(0, i);
          //if (Globals.voicelist.ContainsKey(m.voicecode) != true)
          bool ret = VoiceGlobal.Contains(m.voicecode);
          if (ret != true)
          {
              Logs.write("キャラコードが定義されていません");
            continue;
          }
          // Logs.write("voicecode" + m.voicecode);
          s.messages.Add(m);
          buff = line.Substring(i + 1).Trim(); // 全角大文字がある？必要あるか？ trimで削除されていると思う

          // ：はコマンド、；は発音記号、＠は顔文字 ばらばらに配置される可能性がある

          if (buff.Contains("："))
          {
            i = buff.IndexOf("：");
            m.othercommand = buff.Substring(i + 1);
            buff = buff.Substring(0, i);
          }
          if (buff.Contains("＠"))
          {
            i = buff.IndexOf("＠");
            m.charafacestr = buff.Substring(i + 1);
            buff = buff.Substring(0, i);
            Logs.write("表情=" + m.charafacestr);
          }



          string[] ary = buff.Split(new string[] { "；" }, StringSplitOptions.RemoveEmptyEntries);
          if (ary.Length == 0)
          {
            continue;
          }
          buff = ary[0];
          //m.message = ary[0]; // ；で区切った先頭の要素はメッセージ
          m.message = buff.Replace("￥", Environment.NewLine);
          if (ary.Length > 1)
          {
            // 先頭にキャラ記号がある場合にははじく
            buff = ary[1];
            // m.pronmessage = buff;
            if (l.Substring(0, 2) == buff.Substring(0, 2))
            {
              buff = buff.Substring(2);
            }
            //m.pronmessage = buff;
            m.pronmessage = buff.Replace("￥", ""); // 発音部分の￥は改行せずに空文字にし、そのまま発音させる
                                                   // \の改行が入り、必ず区切られる いや、buffを設定しているから、￥はそのまま発音されるはずだ
                                                   // ：が入る可能性がある
                                                   // ぼうよみちゃんでも登録しなくてすむように、エコーとスピード、ボリューム、音程はコマンドで指定したい
                                                   //  だが、ipcではコマンドを認識しないので、aquestalkのdllを直接呼ぶ必要がある
                                                   // その場合、リバーブは自作する必要がある


          }
          // if(ary.Length>2)
          // {
          //     m.charaexpression = ary[2]; // 次（最後）の要素は表情記号
          // }
          // Logs.write("message" + m.message);
          // Logs.write("hatuon" + m.pronmessage);
          // Logs.write("charaexp" + m.charaexpression);

        }

      }
      return true;
    }
  }
}
