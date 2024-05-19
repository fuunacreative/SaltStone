using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace saltstone
{
  public partial class frmFloat_CharaSet : Form
  {
    public frmCharadefine mainform;

    public frmFloat_CharaSet()
    {
      InitializeComponent();
    }

    private void button1_Click(object sender, EventArgs e)
    {
      mainform.dock(this);

    }

    private void frmFloting_Load(object sender, EventArgs e)
    {
      pf_init();
    }

    private void pf_init()
    {
      // 初期値の設定
      lstCharaset.Rows.Clear();
      lstCharaset.Columns.Clear();
      DataTable dt = new DataTable();
      dt.Columns.Add("キャラセット");
      DataRow r = dt.NewRow();
      r[0] = "いつもの";
      dt.Rows.Add(r);
      lstCharaset.DataSource = dt;
      
    
    }
  }
}
