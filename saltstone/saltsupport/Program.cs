using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using saltstone;

// saltstoneのsupport exe
// luadllからの呼び出しに対し、c++でrgbaの描画を行う
// 引数がなければ設定画面を開く -> iniファイルの書き込み、db,charadbの作成
// chara dirの解析も行う


namespace saltsupport
{
  static class Program
  {
    /// <summary>
    /// アプリケーションのメイン エントリ ポイントです。
    /// </summary>
    [STAThread]
    static void Main(string[] arg)
    {
      if (arg.Length == 0)
      {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new Form1());
      } 
      else
      {
        switch (arg[0])
        {
          case "rebuildcharadb":
            Globals.init();
            saltstone.Charas c = Globals.charas;
            // c.parse();
            // rebuildの指定なので、立ち絵dirを全部解析しなおしする？
            // これって使うのかな？
            // Globals.charapotrait.pa
            break;
        
        }
      }

    }
  }
}
