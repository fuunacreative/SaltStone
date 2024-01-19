using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

// using SaltstoneLibCharaPicture;


// global変数をどこから取得するか？ -> キャラディレクトリ、キャラ素材をどこから取得するのか？
// exeディレクトリにあるchara.dbのsqlite3 dbから取得するのがよさげだねー
// c#でキャラ素材ディレクトリを検索し、解析し、dbに保存するクラスを作るか？
// psd,ymm4の場合はどうするか？
// ほしいののは、各キャラのパーツ一覧、パーツを構成するファイル名


// settings.dbよりdbを検索
// キャラ素材ディレクトリを解析
// パーツ　ファイル名を解析
// chara.dbに展開
// arg 
//   キャラ名 or 全部
// TODO 更新されたかどうかはどうやって判定するか？
//   手動で再実行 or ファイルのtimestamp sizeを保存しておき、更新されたか判定  --> どう考えても前者
//   ただし、判定機能はつける
// キャラ素材（画像）を取りまとめる -> 各キャラを管理するclass -> 各パーツを管理するcollection or class
// saltstone libraryのcharasをかぶる -> formを含まず、画像処理のみを行うdll libraryを作成する




namespace SaltstoneFace
{
  public partial class frmFacepicture : Form
  {
    public frmFacepicture()
    {
      InitializeComponent();
      init();  
    }

    public void init()
    {
      // charabasedirを取得 <- settings.dbを読み込み appinfo classで解析
      string w = saltstone.Appinfo.charabasedir;

      string x = saltstone.Appinfo.charadbfname;
      // charadbのdivisionsをどこに持たせるか？


      // dbを利用するcharasとdbを組み立てるclassを別にする
      // Charas chrs = new Charas();
      // chardb fullpathが設定されていないため、null errorがおこる
      // bool ret = chrs.makedb_typedir_all(w);
      // bool ret = slib
      bool ret = SLibChara_Make.CharaMake.Makedb_dirtype(w);
    }

    private void listView1_SelectedIndexChanged(object sender, EventArgs e)
    {

    }
  }
}
