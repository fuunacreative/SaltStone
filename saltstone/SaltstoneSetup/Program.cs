using System;
using System.Data;
using System.Windows.Forms;

// TODO ver表記
// どうしてもinstallしながらURLを開くと bringToFrontが動作しない環境がある
// 特にupdateしてないWinで見られる
// URLは最後にまとめてひらくようにする
// ダウンロードを別スレッドで行う
// 画面からキャラを削除
// timerが残り　プロセスが停止しないときがある -> timerを一元管理
// c++ runtime check  + installed check
// install logicを別スレッドで行う



namespace saltstone
{
  static class Program
  {
    /// <summary>
    /// アプリケーションのメイン エントリ ポイントです。
    /// </summary>
    [STAThread]
    static void Main()
    {
      // error report system
      // 全体をtry catchし、exceptionを得る
      // txtをつくり、これをupする
      // どこへ？ とりま、メールで送ってもらうか
      frmInstall f = null;
      try
      {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        Utils.Sound.play(SaltstoneSetup.Properties.Resources.start_program);

        // System.Media.SystemSounds.Asterisk.Play();

        Utils.mouseCursor.wait();
        bool netret = Utils.isInternetConnect();
        if (netret == false)
        {
          System.Windows.Forms.MessageBox.Show("ネットワークno");
          return;
        }

        f = new frmInstall();

        install inst = new install();
        //inst.showMessage = f.pf_showMessage;
        inst.readInstallhtml();
        f.inst = inst;

        frmDirectory frm = new frmDirectory();

        DataTable dirs = new DataTable();
        DataColumn c = new DataColumn();
        DataRow r;

        c = new DataColumn("id");
        dirs.Columns.Add(c);
        c = new DataColumn("name");
        dirs.Columns.Add(c);
        c = new DataColumn("path");
        dirs.Columns.Add(c);
        c = new DataColumn("memo");
        dirs.Columns.Add(c);
        foreach (directory d in inst.directorys.Values)
        {
          r = dirs.NewRow();
          r[0] = d.id;
          r[1] = d.name;
          r[2] = d.path;
          r[3] = d.memo;
          dirs.Rows.Add(r);
        }
        frm.directorytable = dirs;
        frm.inst = inst;

        // Utils.CursorDefault();
        DialogResult ret = frm.ShowDialog();
        if (ret != DialogResult.OK)
        {
          return;
        }

        Application.Run(f);

      }
      catch (Exception e)
      {
        // formがcloseする前にtimer disposeをおこなってしまう
        Utils.Dispose();
        if (f != null)
        {
          f.Dispose();
          f = null;
        }
        string filename = "debuginfo.txt";
        string dir = System.IO.Directory.GetCurrentDirectory();
        string[] ary = new string[4];
        string message = e.Message;
        string line = e.Source;
        string trace = e.StackTrace;
        ary[0] = "エラーにより中断しました\r\nこのファイル[" + dir + "\\" + filename + "]を"
          + SaltstoneSetup.Properties.Resources.errreportmail + "に送付してください";
        ary[1] = e.Message;
        ary[2] = e.Source;
        ary[3] = e.StackTrace;
        string buff = string.Join(Environment.NewLine, ary);
        // Console.WriteLine(buff);
        //System.IO.TextWriter()
        System.IO.File.WriteAllText(filename, buff);
        System.Diagnostics.Process.Start("notepad.exe", filename);
        System.Media.SystemSounds.Hand.Play();
      }
      finally
      {
        Utils.Dispose();
      }




    }
  }
}
