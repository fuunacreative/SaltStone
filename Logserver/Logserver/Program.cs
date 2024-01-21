using saltstone;
using System;
using System.Threading;


namespace logserver
{
  /*
  internal static class Program
  {
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
      // To customize application configuration such as set high DPI settings or default font,
      // see https://aka.ms/applicationconfiguration.
      ApplicationConfiguration.Initialize();
      Application.Run(new Form1());
    }
  }　
  */
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
        // mutexを使うか、、、 getprocessbynameを使うか、、、
        // ひとつを実行した場合、getprosessbynameはひとつがヒットする

        // すでにlogserverが実行されているか確認する
        string exename = Utils.Sysinfo.getExeName();
        int cnt = Utils.checkrunexec(exename);
        Logs lbuff = null;
        bool alreadyexec = false;
        if (cnt > 1) {
          alreadyexec = true;
        }

        #region argument 処理
        if (args.Length > 0)
        {
          lbuff = new Logs();
          lbuff.logdate = Utils.getNowDatetime();
          // 起動時引数
          // message , exename , logtype, method, sourceline, trace,
          // string , string , (info,warn,debug,error,fatal) , 
          // exenameより exe verを取得する
          // log dateはcurrent_date
          // argsは実行exeは含まない
          for (int i = 0; i < args.Length; i++)
          {
            switch (i)
            {
              case 1:
                lbuff.message = args[i];
                break;
              case 2:
                lbuff.exename = args[2];
                break;
              case 3:
                // logtype
                switch (args[3])
                {
                  case "debug":
                    lbuff.logtype = Logs.Logtype.debug;
                    break;
                  case "info":
                    lbuff.logtype = Logs.Logtype.info;
                    break;
                  case "warn":
                    lbuff.logtype = Logs.Logtype.warn;
                    break;
                  case "error":
                    lbuff.logtype = Logs.Logtype.error;
                    break;
                  case "fatal":
                    lbuff.logtype = Logs.Logtype.fatal;
                    break;
                }
                break;
              case 4:
                lbuff.method = args[4];
                break;
              case 5:
                int j;
                bool ret = int.TryParse(args[5], out j);
                if (ret == true)
                {
                  lbuff.sourceline = j;
                }
                break;
              case 6:
                lbuff.trace = args[6];
                break;
            }
          }
        }
        // 引数がある + exeが動いている -> ログ出力 + 画面表示
        // processが動いている場合はlogs.sendが行われる。
        // ここでは、引数が指定されている場合、logserverを起動し
        // 引数のファイルを書き込む
        if (lbuff != null)
        {
          // exeが動いている場合はsend , 動いていない場合はwrite
          if (alreadyexec == true) {
            // send内部でも判定しているが、
            // named pipeを使用して実行中のlogserverに送り、終了する
            lbuff.send();
            return;
          }
          // exeが動いてなければ直接書き込む
          lbuff.write();
        }
        #endregion


        // logserverを起動 namedpipeno無限ループに入る
        // 終了は画面でcloseボタンが押されるまで
        // ここからはwin formの動作となる

        // 
        #region logserver loop
        // logmanagerのログ受付ループを実行

        // global initが必要か、、、、
        Globals.init();
        // Logs.init();

        // revloopは必要ないかも -> event riseされるのでは？
        // _taskid = Tasks.createTask(revloop);
        // Logs.IPCReieveLog();
        using (LogServer ls = LogServer.getInstance())
        {
          // ls.addevent();
          // 内部でnamed pipeのreceive処理を実行する
          // 無限待機する
          ls.initServer();
          // ipcのsend revがうまく動いていないみたい

        }

        // 既に起動している場合はlogだけ書き込む
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new frmLogView());


        // logserverのclose処理
        // ls.Dispose();
        #endregion

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


