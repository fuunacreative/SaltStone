using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

/// <summary>
///  TODO 使用メモリの測定・定期的
///  ディスク開きスペースのチェック　定期的
///  選択パーツはラジオボタン化
///  http://www.ria-lab.com/archives/480
/// </summary>

// psd groupについて
// 必須選択は赤、選択不可能（必ず描画）はグレー
// group内に選択されてる項目がある場合はintermeditate
// 選択可能なレイヤーが含まれている場合には選択可能アイコン
// saltstone分類分け方が必要な場合にはびっくりアイコン


namespace saltstone
{
  public partial class frmCharaPicturePSD : Form
  {
    public frmCharaPicturePSD()
    {
      InitializeComponent();
    }

    private void frmCharaParts_Load(object sender, EventArgs e)
    {

    }

  }
}
