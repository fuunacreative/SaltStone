using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace saltstone
{
  class Senariotxtparser
  {
    private enum parsetype
    {
      global, globalvoice, globalvoiceEffect, globalfaceexp, globalcredit, memo, chara, charapicture, scene
    }

    public const string MessageNewLine = "￥￥";

    public static bool parse(string infile)
    {
      // Logs.write(infile);
      if (Utils.Files.exist(infile) == false)
      {
        // logs.write("解析するtxtファイルが見つかりません");
        return false;
      }
      string alltext = System.IO.File.ReadAllText(infile);
      // Logs.writelog(alltext);
      List<string> lines = new List<string>(alltext.Replace("\r\n", "\n").Split(new[] { '\n', '\r' }));
      // string[] ary = buff.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

      Datas.Clear();
      string scenename = "";
      string buff;
      string line;
      int i;
      parsetype mode = parsetype.global;
      // string voicecode;

      Scenes s = null;
      foreach (string l in lines)
      {
        line = l;
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
        if (line.Contains("[scene") || line.Substring(0, 1) == "「")
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
          // シーンが重複している場合はどうなるか？
          // trailに1をつけて別名で登録
          Datas.addscene(scenename, s);
          continue;
        }
        if (mode == parsetype.scene)
        {
          // シーンの解析 シーンとメッセージ、発音文字、顔省略文字を登録していく
          if (s == null)
          {
            // logs.write("シーンが指定されていないのにセリフが存在します")
            // Logs.write("not current scene");
            continue;
          }
          i = line.IndexOf("）");
          if (i == -1)
          {
            i = line.IndexOf(")");
            if (i == -1)
            {
              continue;
            }
          }
          buff = line.Substring(0, i);
          bool ret = VoiceGlobal.Contains(buff);
          if (ret != true)
          {
            // Logs.write("キャラコードが定義されていません");
            continue;
          }
          Quote m = new Quote();
          m.voicecode = buff;
          s.messages.Add(m); // 作った空のQuoteをSceneに登録
          buff = line.Substring(i + 1).Trim();
          // 全角大文字がある？必要あるか？ trimで削除されていると思う
          // 顔文字、発音をtxtで定義できるようにするか？

          // 全角\\は改行文字として扱う
          m.message = buff.Replace(MessageNewLine, Environment.NewLine);
        }

      }
      return true;
    }
  }
}
