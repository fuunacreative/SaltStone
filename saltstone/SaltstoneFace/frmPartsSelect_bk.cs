using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using saltstone;

namespace SaltstoneFace
{
  public partial class frmPartsselect : Form
  {
    private saltstone.Charas charas;
    private Dictionary<int, string> comboboxcharalist;

    public frmPartsselect()
    {
      InitializeComponent();
      init();  
    }

    public void init()
    {
      // キャラ combo boxの初期化
      // Chara libを参照
      charas = new Charas();
      string buff;
      int i;
      comboboxcharalist = new Dictionary<int, string>();
      lstCharaID.Items.Clear();
      foreach (KeyValuePair<string,Chara> kv in charas.chara)
      {
        buff = kv.Value.dispname;
        i = lstCharaID.Items.Add(buff);
        comboboxcharalist[i] = kv.Value.charakey;
        // txtCharaID.Items.
      }
      lstCharaID.SelectedIndex = 1; //　for test

      // charaid れいむ=rを元にパーツリストをlib charasより読み込む
      // -> 各datagridに展開
      bool fret = setPartslist();

      // timerを作成し、アニメ画像のアニメ処理を行う



    }

    private bool setPartslist()
    {
      bool fret = false;



      string charakey = comboboxcharalist[lstCharaID.SelectedIndex];
      saltstone.Chara c = charas[charakey];
      pctModelPicture.Image = c.modelpicture;
      SortedDictionary<string, CharaParts> p = c.getparts("F");
      //  表示するのは、partsnbum(00),animeflag , memo
      // この３つを取得する関数が必要

      fret = true;
      return fret;
    }


    private void frmPartsSelect_Load(object sender, EventArgs e)
    {

    }

    private void frmPartsselect_FormClosing(object sender, FormClosingEventArgs e)
    {
      // charas.
      charas.Dispose();
      charas = null;
      comboboxcharalist.Clear();
      comboboxcharalist = null;
    }

    private bool mergeimage()
    {

      // charapartsのcompositorderidで描画順が決定できるので
      // c++ image処理でincmd.txtに描画順を書き込み
      // 画像合成したものを出力する
      // すべてのパーツがそろっているものはキャッシュする
      // セット.txtにあるものはout\*.pngに出力＆キャッシュ
      // favoriteにあるものも

      // まず、描画順を決める顔から処理を行う?
      // 体から？


      return true;
    }
  }
}
