using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace saltstone
{
  // シナリオファイルを保存するデータ

  class Datas
  {

    public static Dictionary<string, Scenes> scenes;
    public static List<Scenes> sceneidx;
    // すべてのQuoteを保持する
    public static Dictionary<int, Quote> quotes; 


    static Datas()
    {
      init();
    }


    // 内部データの初期化
    public static void init()
    {
      if (scenes != null)
      {
        scenes.Clear();
        scenes = null;
      }
      if (sceneidx != null)
      {
        sceneidx.Clear();
        sceneidx = null;
      }
      if (quotes != null)
      {
        quotes.Clear();
        quotes = null;
      }
      Scenes.Clear(); // 内部のstatic counterをクリア
      GC.Collect();

      scenes = new Dictionary<string, Scenes>();
      sceneidx = new List<Scenes>();
      quotes = new Dictionary<int, Quote>();
    }



    //　内部データの開放 IPCはクリアしない
    public static void Clear()
    {

      scenes.Clear();
      sceneidx.Clear();
      quotes.Clear();
      Scenes.Clear(); // 内部のcouterをクリア
      Quote.Clear();
    }

    // ｐｇ終了時の処理
    public static void dispose()
    {
      Clear();
      // boyomi.Dispose();
    }

    // シーンオブジェクトの追加 dictonaryとlistの両方に登録したいため、メソッドを使う
    public static void addscene(string key, Scenes arg)
    {
      // keyがすでにある場合は何もしない
      // keygが既にある場合、最後に1などの番号をつけて登録する
      string buff = key;


      if (scenes.ContainsKey(buff))
      {
        string candi;
        //return;
        int i = 1;
        // 最後に1の番号をつけて存在チェック
        // 必ず登録させる
        do
        {
          candi = buff + "." + i.ToString();
          if (scenes.ContainsKey(candi) == false)
          {
            buff = candi;
            break;
          }
          i++;
        } while (true);

      }
      scenes.Add(buff, arg);
      sceneidx.Add(arg);
    }

    public static bool Play(string scenekey)
    {
      scenes[scenekey].make();

      return true;
    }
  }

}
