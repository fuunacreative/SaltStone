using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

using saltstone;

namespace SaltstoneFace
{
  public partial class frmCharalist : Form
  {
    SLibProgressbar pgbar;
    // frmPartsSelect mainform;

    public frmCharalist()
    {
      InitializeComponent();
      init();
    }
    public void init()
    {
      pgbar = new SLibProgressbar();
      pgbar.init(stpProgressbar);
      Logs.init(stbStatuslabel, stpProgressbar);

      string w = saltstone.Appinfo.charabasedir;
      txtCharbasedir.Text = w;

      Dictionary<string, string> charaids = new Dictionary<string, string>();

      string buff;
      int i = 1;
      string charaid;
      List<string> dirs = Utils.Files.getdirectory(w);

      // dir内部にcharadef.txtが存在するか確認
      foreach (string cdir in dirs)
      {
        buff = cdir + "\\" + SLibChara_Make.CharaMake.CHARDEF_Chardef;
        if (Utils.Files.exist(buff) == false)
        {
          continue;
        }
        StreamReader fs = new StreamReader(buff);
        buff = fs.ReadLine();
        fs.Close();
        fs.Dispose();
        fs = null;
        i = buff.IndexOf("=");
        charaid = buff.Substring(i + 1);
        buff = Utils.Files.getfilename(cdir);
        charaids[buff] = charaid;
      }

      i = 1;

      // 解析を開始するにはどうするか？
      // 判定が必要だねー どーするか？

      DataTable dt = new DataTable();
      // DataTable dt = lstCharalist.DataSource;
      dt.Columns.Add("ディレクトリ名");
      dt.Columns.Add("キャラID");
      dt.Columns.Add("キャラ素材種別");
      dt.Columns.Add("表示順");
      DataRow dr;
      bool fret;
      lstCharalist.Columns.Clear();
      lstCharalist.Rows.Clear();
      foreach (string cdir in dirs)
      {
        fret = SLibChara_Make.CharaMake.checkvalidate_typedir(cdir);
        if (fret == false)
        {
          continue;
        }
        dr = dt.NewRow();
        buff = Utils.Files.getbasename(cdir);
        dr[0] = buff;
        charaid = buff.Substring(0, 1);
        if (charaids.ContainsKey(buff) == true)
        {
          charaid = charaids[buff];
        }
        dr[1] = charaid;
        dr[2] = "chardir";
        dr[3] = i.ToString();
        i++;
        dt.Rows.Add(dr);
        pgbar.setBarval(i);
      }
      lstCharalist.DataSource = dt;
      //foreach (DataGridViewRow drow in lstCharalist.Rows)
      //{
      //  var x = drow.Cells[1].Value;
      //  // drow.Cells[1].Style.BackColor = Color.Red;
      //  drow.DefaultCellStyle.BackColor = Color.Red;
      //}
    }

    private void lstCharalist_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
    {
      // キャラIDを変更した場合にdbを更新する
      /// lstCharalist.Rows[0].Cells[1].Style.BackColor = Color.Red;
    }


    private void lstCharalist_CellValidated(object sender, DataGridViewCellEventArgs e)
    {
      // セル入力が完了した　-> キャラ識別IDをすべて更新する
    }

    private void cmdParse_Click(object sender, EventArgs e)
    {
      func_parsechara();
    }

    public static frmCharalist thisfrm;
    public static void showwindow()
    {
      thisfrm = new frmCharalist();
      thisfrm.Show();
    }

    // TODO util class化かしたい refでintvalを参照わたしする
    // timerはclass instanceの内部menmberで保持する

    private void func_parsechara()
    {
      stbStatuslabel.Text = "キャラ素材の解析を実行します";
      Utils.sleep(10);
      // キャラ識別子をdbに登録 or txtファイルに保存　"chardef.txt”
      // うーん。char directoryに定義を保存したほうがよいねー
      string dirname;
      string charaid;
      foreach (DataGridViewRow dr in lstCharalist.Rows)
      {
        dirname = (string)dr.Cells[0].Value;
        charaid = (string)dr.Cells[1].Value;
        dirname = txtCharbasedir.Text + "\\" + dirname;
        SLibChara_Make.CharaMake.setCharaID(dirname, charaid);
      }


      string dpath = txtCharbasedir.Text;
      Utils.mouseCursor.wait();
      SLibChara_Make.CharaMake.Makedb_dirtype(dpath);
      Utils.mouseCursor.clear();
      stbStatuslabel.Text = "解析が終了しました";
      Utils.sleep(100);

      SLibChara_Make.CharaMake.Dispose();
      // this.Hide();
      this.Close();
      // メインフォームを表示 -> パーツリストを選択しふくわらいを実行
      // frmPartsSelect.showwindow();

    }

  }

}
