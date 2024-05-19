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
  public partial class frmDirectory : Form
  {
    public DataTable directorytable;
    public install inst;
    public bool flagok;
    public frmDirectory()
    {
      InitializeComponent();
      flagok = false;
    }

    private void frmDirectory_Load(object sender, EventArgs e)
    {
      lstDirectory.DataSource = directorytable;
      lstDirectory.Columns[0].Visible = false;
      lstDirectory.Columns[1].ReadOnly = true;
      lstDirectory.Columns[3].ReadOnly = true;
      // this.DialogResult = DialogResult.Cancel;
    }

    private void cmdDirset_Click(object sender, EventArgs e)
    {
      string key;
      directory d;
      foreach (DataGridViewRow r in lstDirectory.Rows)
      {
        key = (string)r.Cells[0].Value;
        d = inst.directorys[key];
        d.path = (string)r.Cells[2].Value;
      }
      flagok = true;
      this.DialogResult = DialogResult.OK;
      this.Close();
    }

    private void frmDirectory_FormClosing(object sender, FormClosingEventArgs e)
    {
      if (flagok == true)
      {
        this.DialogResult = DialogResult.OK;
      } else
      {
        Application.Exit();
        // this.DialogResult = DialogResult.Cancel;
      }
    }

  }
}
