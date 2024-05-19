using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using System.Security.Permissions;

namespace saltstone
{
  static class Program
  {
    // dllで発生したexceptionは設定しない限り補足されない
    // attribute や error event handlerを設定しておく必要がある


    /// <summary>
    /// アプリケーションのメイン エントリ ポイントです。
    /// </summary>
    [STAThread]
    [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.ControlAppDomain)]
    static void Main()
    {
      Utils.mouseCursor.useWait();
      try
      {
        // Add the event handler for handling UI thread exceptions to the event.
        Application.ThreadException += new ThreadExceptionEventHandler(Application_ThreadException);

        // Set the unhandled exception mode to force all Windows Forms errors to go through
        // our handler.
        Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);

        // Add the event handler for handling non-UI thread exceptions to the event.
        AppDomain.CurrentDomain.UnhandledException +=
            new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

        // Runs the application.
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new frmCharadefine());

      }
      catch (Exception ex)
      {
        string msg = ex.Message;
      }
      finally
      {
        Utils.Dispose();
        Globals.Dispose(); 
        // utilsにまとめたいけど、、、使ってないpgもあるよのよねー
        // 問題はcharadbが開きっぱなしになってる事
        // data getとかsetしてるから、速度面を考えると開きっぱなしのほうがよい
      }

    }

    //ThreadExceptionイベントハンドラ
    private static void Application_ThreadException(object sender,
        System.Threading.ThreadExceptionEventArgs e)
    {
      try
      {
        //エラーメッセージを表示する
        string buff = e.Exception.Message;
        buff += "\r\n" + e.Exception.StackTrace;
        MessageBox.Show(buff, "エラー" );
      }
      finally
      {
        //アプリケーションを終了する
        Application.Exit();
      }
    }
    private static void CurrentDomain_UnhandledException(Object sender, UnhandledExceptionEventArgs e)
    {
      try
      {
        Exception ex = (Exception)e.ExceptionObject;
        string buff = ex.Message;
        buff += "\r\n" + ex.StackTrace;
        MessageBox.Show(buff);
      }
      catch (Exception exc)
      {
        try
        {
          MessageBox.Show("Fatal exception happend inside UnhadledExceptionHandler: \n\n" + exc.Message);
        }
        finally
        {
          Application.Exit();
        }
      }

      // It should terminate our main thread so Application.Exit() is unnecessary here
    }

  }
}
