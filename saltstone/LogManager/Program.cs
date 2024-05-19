using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;


// TODO C++ memory map file utf8? utf16? sjis? utf8で統一したい

namespace saltstone
{
  static class Program
  {
    // private static string _taskid;

    /// <summary>
    /// アプリケーションのメイン エントリ ポイントです。
    /// </summary>
    [STAThread]
    static void Main(string[] args)
    {
      try
      {
        // exe singleton
        int count = System.Diagnostics.Process.GetProcessesByName(System.IO.Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetEntryAssembly().Location)).Count();
        if (count > 1)
        {
          // exe is already running
          // if arg is pass -> log send by ipc
          // arg1 -> message only
          // arg2 -> error , message
          // arg3 -> exe,error,message
          // arg4 -> exe,error,message,stacktrace ? 
          // arg1 and file.json -> json parse and send log by ipc
          return;
        }
        Globals.init();
        // Logs.init();

        // revloopは必要ないかも -> event riseされるのでは？
        // _taskid = Tasks.createTask(revloop);
        // Logs.IPCReieveLog();
        LogServer ls = LogServer.getInstance();
        // ls.addevent();
        ls.initServer();
        // ipcのsend revがうまく動いていないみたい


        // 既に起動している場合はlogだけ書き込む
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new frmLogView());

        // logserverのclose処理
        ls.Dispose();

      }
      catch (Exception e)
      {
        Logs.write(e);
        // logsのququeを確実に書き出す
        // global dispose -> logs dispose -> ququqがemptyになるまで書き出すはず
      }
      finally
      {
        Globals.Dispose();
      }
    }
    /*
    public static void revloop()
    {
      while (true)
      {
        IPCLog l =  Logs.IPCReieveLog();
        // Ipclogjob_recieveevent　ここでwrite処理を行う
        Utils.sleep(500); // 0.5sに一度、low level logのqueueを処理する
      }
    }
    */

  }


}
