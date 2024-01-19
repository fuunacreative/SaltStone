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
  public partial class frmFloat_CharaSetting : Form
  {
    public frmFloat_CharaSetting()
    {
      InitializeComponent();
    }

    private void frmFloat_CharaSetting_Load(object sender, EventArgs e)
    {
      pf_init();
    }

    private void pf_init()
    {
      lstCharaLayer.Columns.Clear();
      lstCharaLayer.Rows.Clear();
      DataTable dt = new DataTable();
      dt.Columns.Add("レイヤー");
      dt.Columns.Add("No");

      DataRow r = dt.NewRow();
      r[0] = "WAV";
      r[1] = 10;
      dt.Rows.Add(r);

      r = dt.NewRow();
      r[0] = "字幕";
      r[1] = 11;
      dt.Rows.Add(r);

      r = dt.NewRow();
      r[0] = "立ち絵";
      r[1] = 12;
      dt.Rows.Add(r);

      lstCharaLayer.DataSource = dt;
      r = null;
      dt = null;

    }
  }
}
