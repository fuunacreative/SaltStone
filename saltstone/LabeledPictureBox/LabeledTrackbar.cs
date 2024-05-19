using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;



// TODO max 100にしているのに96までしか動かない


namespace saltstone_customcontrol
{
  public partial class LabeledTrackbar : UserControl
  {
    private int _minvalue;
    private int _maxvalue;
    private bool pAnimaflag = false;

    public LabeledTrackbar()
    {
      InitializeComponent();
      lblbarname.Text = "";
      txtbarvalue.Text = "";
      ctlpicture.Image = null;
      lblpicturename.Text = "";

    }
    public override string Text {
      set {
        lblbarname.Text = value;
      }
    }
    public string setPictureName {
      set {
        lblpicturename.Text = value;
      }
    }
    public Image Image {
      set {
        ctlpicture.Image = value;
      }
    }
    public int Minvalue {
      set {
        _minvalue = value;
        ctlBar.Minimum = value;
        ctlBar.Value = value;
      }
    }
    public int Maxvalue {
      set {
        // なぞ仕様により、largechangeが関係してくる
        // 100をとろうとすると、 maximum 109にしないといけない val(max) = maximum - largechange(10) + 1となるので
        // 100を設定したら、scroll maxを100とれるようにしたい
        _maxvalue = value; // value は100
        int i = value;
        i = i + ctlBar.LargeChange - 1;
        ctlBar.Maximum = i;
      }
    }
    public int Value {
      get {
        int i = ctlBar.Value;
        if (i > _maxvalue)
        {
          i = _maxvalue;
        }
        return ctlBar.Value;
      }
      set {
        ctlBar.Value = value;
        txtbarvalue.Text = value.ToString();
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


    private void ctlBar_Scroll(object sender, ScrollEventArgs e)
    {
      txtbarvalue.Text = ctlBar.Value.ToString();
    }

    private void ctlpicture_Paint(object sender, PaintEventArgs e)
    {
      if (pAnimaflag == true)
      {
        ctlpicture.BorderStyle = BorderStyle.None;
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
