using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SaltstoneFace
{
  static class Program
  {
    /// <summary>
    /// アプリケーションのメイン エントリ ポイントです。
    /// </summary>
    [STAThread]
    static void Main()
    {
      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);
      // ディレクトリ解析が終了していれば frmPartsselectをshow 
      // していなければ frmcharalistをshowして解析を促す
      // 解析が終了しているかどうかは、dirnameがdbにすべて登録されているかどうかで判定する
      // TODO and dir配下の最新ファイルがdbに登録されているかどうか

      //saltstone.Logs.init();
      // saltstone.Logs.write("aaa");
      // bool forceparse = true;
      bool forceparse = false;
      if (SLibChara_Make.CharaMake.checkreparse() == true || forceparse == true)
      {
        frmCharalist f = new frmCharalist();
        f.ShowDialog();
        f.Dispose();
        f = null;
      }
      saltstone.Globals.init();
      // frmCharalist.showwindow();
      Application.Run(new frmPartslist());

      try
      {
      }
      catch (Exception ex)
      {
        saltstone.Logs.write(ex);
      }
      finally
      {
        saltstone.Globals.Dispose();
      }

      //saltstone.STasks.Dispose();
    }

  }
}
