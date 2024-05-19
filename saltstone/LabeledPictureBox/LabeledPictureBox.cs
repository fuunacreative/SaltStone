using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace saltstone_customcontrol
{
  public partial class LabeledPictureBox : UserControl
  {
    private bool pAnimaflag = false;
    public LabeledPictureBox()
    {
      InitializeComponent();
      lblPicturetext.Text = "";
      // panel.BorderStyle = BorderStyle.FixedSingle;
      pcturebox.Image = null;
      panel.BorderStyle = BorderStyle.FixedSingle;

      //splitContainer1.Dock = DockStyle.None;
      //splitContainer1.Width = panel.Width - 1;
      //splitContainer1.Height = panel.Height - 1;
    }

    public PictureBoxSizeMode SizeMode {
      set {
        pcturebox.SizeMode = value;
      }
    }
    

  public override string Text {
      get
      {
        return lblPicturetext.Text;
      }
      set 
      {
        lblPicturetext.Text = value;
      }
    }

    public Image Image 
    {
      set {
        pcturebox.Image = value;
      }
              
    }
    public bool Animeflag {
      get {
        return pAnimaflag;
      }
      set {
        pAnimaflag = value;
      }
    }


    private void pcturebox_Click(object sender, EventArgs e)
    {
      this.OnClick(e);
    }

    private void lblPicturetext_Click(object sender, EventArgs e)
    {
      this.OnClick(e);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
      // base.OnPaint(e);
      // base.OnPaint(e);
      //if (pAnimaflag == true)
      //{

      //  int right = this.ClientRectangle.Right -1;
      //  int bottom = this.ClientRectangle.Bottom-1;

      //  Pen pen = new Pen(Color.Orange);
      //  Graphics g = this.CreateGraphics();
      //  g.DrawLine(pen, 0, 0, right, 0); // 上辺
      //  g.DrawLine(pen, 0, 0, 0, bottom); // 左辺
      //  g.DrawLine(pen, right, 0, right, bottom); // 右辺
      //  g.DrawLine(pen, 0, bottom, right, bottom); // 下辺
      //  //splitContainer1.BorderStyle = BorderStyle.FixedSingle;

      //}

    }

    private void pcturebox_Paint(object sender, PaintEventArgs e)
    {
      if (pAnimaflag == true)
      {
        pcturebox.BorderStyle = BorderStyle.None;
      } else
      {
        // ctlpicture.BorderStyle = BorderStyle.FixedSingle;
      }
      base.OnPaint(e);
      if (pAnimaflag == true)
      {
        ControlPaint.DrawBorder(e.Graphics, e.ClipRectangle, Color.Orange, ButtonBorderStyle.Solid);
      }

    }
  }

}
