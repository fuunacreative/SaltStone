using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

// 処理内容
// dirキャラ素材 -> パーツ解析など -> psd or png
// パーツ解析結果はdbに保存
// アニメ処理するとか、非表示パーツ
// 顔・眉・目・口・体、前パーツ、後ろパーツ
// お気に入り
// aviulのluaから呼び出し、立ち絵bitmapをrender
// できれば、aviuのスライダーを動かしたいな


namespace saltstone
{
  public partial class frmCharaPicture : Form
  {

    // singleton
    private static frmCharaPicture f;

    // favorite datatable
    private DataTable table_favorite;

    public static frmCharaPicture getInstance()
    {
      if (f == null)
      {
        f = new frmCharaPicture();
      }
      return f;
    }


    public string chardirectory = "";
    public Charas charas = null;
    public frmCharaPicture()
    {
      InitializeComponent();
    }



    private void frmChara_Load(object sender, EventArgs e)
    {

      // init();

    }

    public void init()
    {

      //　https://www.atmarkit.co.jp/ait/articles/0508/12/news091.html
      // listviewでのアイコン表示 お気に入りはアイコンから選択したほうが早い
      // trFavorite.Parent = splitContainer_favorite.Panel1;
      // lstFavorite.Parent = splitContainer_favorite.Panel2;
      StatusStrip s = new StatusStrip();
      chardirectory = Globals.envini.get("chardir");
      statusbar_form.Items.Add(chardirectory);

      // お気に入りのtreeを全部展開
      trFavorite.ExpandAll();

      // lstFavoirteのtableをtable_favoriteに展開
      if (table_favorite == null)
      {
        table_favorite = new DataTable();
      }
      table_favorite.Clear(); // rowsがクリアされるっぽい？
      // table_favorite.Rows.Clear();
      if(table_favorite.Columns.Count == 0)
      {
        // table_favorite.Columns.Clear(); // tableへのcolumn登録は一度だけでいい
        foreach (DataGridViewColumn c in lstFavorite.Columns)
        {
          DataColumn dc = new DataColumn();
          dc.ColumnName = c.Name;
          dc.Caption = c.HeaderText;
          table_favorite.Columns.Add(dc);
        }
      }
      DataRow r = table_favorite.NewRow();
      r[0] = 1;
      r[1] = "デフォルト";
      table_favorite.Rows.Add(r);
      lstFavorite.Columns.Clear();
      lstFavorite.DataSource = table_favorite;
      foreach (DataGridViewColumn c in lstFavorite.Columns)
      {
        c.HeaderText = table_favorite.Columns[c.HeaderText].Caption;
      }


      // キャラ素材ディレクトリの解析
      // 各パーツとアニメするかどうか

      // キャラディレクトリを取得し、キャラリストに設定
      // string path = "";

      // FolderBrowserDialog d = new FolderBrowserDialog();
      // d.ShowDialog();

      /*
      charas = new Charas();
      // charas.directoryparse();
      lstChara.Items.Clear();
      foreach(string s in charas.chara.Keys)
      {
          lstChara.Items.Add(s);
      }
      */

      // for test parts picture box
      string fname = @"C:\Users\fuuna\Videos\resource\chara\れいむ\口\";
      fname += "00e.png";
      Image a = Image.FromFile(fname);
      pctCharaParts.Width = a.Width;
      pctCharaParts.Height = a.Height;
      pctCharaParts.Image = a;
    }

    public void Show(string filepath)
    {
      img.ImageLocation = filepath;
      frmCharaPicture.f.Show();
      // this.Show();

    }

    private void frmChara_FormClosing(object sender, FormClosingEventArgs e)
    {
      f.Hide();
      table_favorite.Clear();
      e.Cancel = true;
    }


    private void tbCharaPartsScale_Scroll(object sender, EventArgs e)
    {
      // trackbarのscaleにあわせパーツ画像を拡大する
      double scale = tbCharaPartsScale.Value / 100.0;
      int width = pctCharaParts.Width;
      int height = pctCharaParts.Height;
      double aspect = (double)height / (double)width;
      Size s = new Size();
      s.Width = (int)(width * scale);
      s.Height = (int)(s.Width * aspect);
      Image img = pctCharaParts.Image;
      Bitmap bmp = new Bitmap(img, s);

      pctCharaParts.Width = s.Width;
      pctCharaParts.Height = s.Height;
      // pctCharaParts.SizeMode = PictureBoxSizeMode.AutoSize;
      pctCharaParts.Image = bmp;

      // centerを維持
      // 現在のpanel
      int x = panelCharaParts.HorizontalScroll.Value;
      int y = panelCharaParts.VerticalScroll.Value;
      width = panelCharaParts.Width;
      height = panelCharaParts.Height;
      // これでセンターのpointがわかるから、これにscaleをかければ新しいセンターが割り出せる



    }


  }
}
