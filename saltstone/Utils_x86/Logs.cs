using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;
using System.Threading;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;
using System.Runtime.Remoting;

// ３つの方法でlogを受け取る
// 1 c# ipc
// 2 mmf + semaphore
// 3 file + filenmae + named pipe
// 4 arg log file



// globalに登録し、globalから呼び出せるようにする -> 循環参照となるため単体で利用する
// formへのgui表示、progressbarの表示にも対応
// -> librarymessageを統合する
// 問題はasyncで登録したいこと
// dataはymd hms,errorなどの種類、メッセージ、stacktrace
// logは別exeで実行するべき？
// logは別exeで実行する
// 監視用のフォルダをつくり、logテキストが書き込まれたら実行ファイル直下のlogフォルダに保存する
// また、数十件はGUIで参照できるようにする
// named pipe, sharememでも受け付けるようにし、さらにクラスメソッドでも書き込みできるようにしておく
// あとipcも
namespace saltstone
{

  public static class Logs
  {

    #region member
    [Serializable]
    public enum Logtype
    {
      dispwarn = 1,
      dispinfo = 2,
      disperror = 3,
      debug = 10,
      info = 11,
      warn = 12,
      error = 13,
      fatal = 14
    }


    public const string EXE_LOGMANAGER = "LogManager.exe";

    // public const string FILE_logfile = "saltstone.log";
    public const string DIR_log = "log";

    // public const string logFile = "saltstone.log";

    public static string ipc_clientChannelName = "";

    public static string logfile = "";
    public static string logpath = "";
    public static StreamWriter logwriter;

    // デフォルトのログレベル
    public static Logtype defaultloglevel = Logtype.error;


    // guiへの通知delegate
    // public delegate void del_notifygui(IPCLog l);
    // public static del_notifygui evt_notifygui;


    // log書き込みを行うかどうかのフラグ
    public static bool writelogflag = true;

    // プロセスのexename
    private static string _exename = "";
    // asyncでlogをlogを書き出すためののjob queue
    // private static Queue<LogJob> _jobqueue;
    // squeueに変更する
    private static SQueue<LogJob> _logqueue;
    // write main thread

    // private static string _logfile;
    public static MsgControl messagectl;
    // job lock mutex
    // mutex よりもsemaphoreのほうが早い
    // semaphoreを一元管理し、終了時にリリースするクラスがほしい
    // private static string semkey_cllient_Queue_lock; -> squeueに統合
    // private static string writertaskguid;

    // ipc client側の初期化処理(ipc channel regist)が終了したかのフラグ
    private static bool iniClientflag = false;
    // private static LogServer.IpcRemoteLog ipclogobj;
    private static IPCLog ipclogobj; // for client




    #endregion

    public static void init(ToolStripStatusLabel lblmes, ToolStripProgressBar pgbar)
    {
      Logs.init();
      messagectl = new MsgControl(lblmes, pgbar);
    }


    // logファイル名をどうするか？
    // exe毎に別にするか、それともまとめてsaltstone.logのようにシステムにひとつにするか？

    public static void init()
    {
      // guiがないアプリではguiへのメッセージ表示は行わない
      _exename = Utils.Sysinfo.getExeName();
      // append = trueでstremwriterを開く
      // clientの場合は不要

      // traceは長いので別ファイルに書き出す
      // semkey_cllient_Queue_lock = Semaphores.createSemaphore();
      // 別スレッドでlogをtxtに書き出す
      // 別exeの場合はipcやsharememで書き出しを行う
      // 同じexeの場合はguiへメッセージを表示し、さらに別exeへ書き出しを行う
      // writertaskguid = Tasks.createTask(writethread);
      _logqueue = new SQueue<LogJob>();
      _logqueue.evt_queueobjadded += ipc_sendLog;
      _logqueue.proctask();
      // _jobqueue = new Queue<LogJob>();

      // log fileはサーバ・クライアント関係なしに作成する
      // log manager (ipc log server)
      string ymd = Utils.getNowDatetime();
      // string logpath = Utils.Sysinfo.getCurrentExepath() + "\\" + DIR_log;
      logpath = Utils.Files.getfilepath(_exename) + "\\" + DIR_log;
      Utils.Files.mkdir(logpath);
      //logfile = logpath + "\\" + ymd + "_" + Utils.Files.getbasename(_exename) + ".log"; 
      // logfile = logpath + "\\" + Logs.LOGFile;
      logfile = logpath + "\\" + DateTime.Now.ToString("yyyyMMdd") + ".log";

      // logmanagerが起動していない場合、起動する
      // 自プロセスがlogmanagerの場合はスキップ
      if (Utils.Files.getfilename(_exename) != EXE_LOGMANAGER)
      {
        // logを書き込む側 通常のクライアント（プログラム）
        // logmanager.exeが起動していない場合には起動する

        // logmanagerの起動チェック
        // bool ret = Utils.Sysinfo.
        string logexe = Utils.Sysinfo.getexepath(EXE_LOGMANAGER);
        if (logexe.Length != 0)
        {
          return;
        }
        // logmanager processの存在チェック
        logexe = Utils.Files.getfilepath(_exename) + "\\" + EXE_LOGMANAGER;
        // option なし つきはなしで起動
        Utils.runexec(logexe, "", Utils.enum_runmode.start_async);
        // ipc remote objが登録されるまで待機
      } else
      {

        // exe用のlogとsaltstone全体のlogを区別する？


        logwriter = new StreamWriter(logfile, true);

      }

    }


    private static bool ipc_sendLog(LogJob log)
    {
      if (Logs.messagectl != null && Logs.messagectl.checkformclosing() == false)
      {
        switch (log.logtype)
        {
          case Logs.Logtype.dispwarn:
          case Logs.Logtype.dispinfo:
          case Logs.Logtype.disperror:
            Logs.messagectl.showMessage(log.message);
            Utils.sleep(500); // 500ms待機
            break;
        }

      }
      // IPCLog l = new IPCLog(log);

      if (iniClientflag == false)
      {
        //_sendlog = new IPCLog();
        iniClientflag = true;
        // クライアントチャンネル生成
        IpcClientChannel channel = new IpcClientChannel();
        ipc_clientChannelName = "ipc://" + LogServer.IPC_ChannelName + "/" + LogServer.IPC_objectName;
        // チャンネル登録
        // security = rue
        ChannelServices.RegisterChannel(channel, true);
        // RemotingServices.Marshal(lsrv.ipclogobj, LogServer.IPC_objectName, typeof(LogServer.IpcRemoteLog));
        //RemotingServices.Marshal(ipclogobj, ipc_clientChannelName, typeof(LogServer.IpcRemoteLog));
        // emoteObject = Activator.GetObject(typeof(IpcRemoteObject), "ipc://ipcSample/test") as IpcRemoteObject;
        ipclogobj = (IPCLog)Activator.GetObject(typeof(IPCLog), ipc_clientChannelName);
      }
      // lがだめなんでは？

      // logjobのarg logを元にipclogobjを設定しないといけない
      // ipclogobjはremoteだから、remote methodにinternal instanceは渡せない
      // serverは起動していると思うが、書き込みに失敗する ので、commentout
      // 別だてできれいに作り直す必要があるね

      // ipclogobj.copy(log);

      // ipclogojbに変換できないとエラーが発生する？
      try
      {

        ipclogobj.logdate = log.logdate;
        ipclogobj.exename = log.exename;
        ipclogobj.message = log.message;
        ipclogobj.logtypeint = (int)log.logtype;
        ipclogobj.tracefile = "";
        // enumのserializeはできない
        // ipclogobj.logtype = log.logtype;
        string tfile = "";
        // ququeの中のタスクでのsem待機ループの中なので、ここでasyncせずに保存する
        bool ret = log.saveStackTrace(out tfile);
        if (ret == true)
        {
          ipclogobj.tracefile = tfile;
        }
        ipclogobj.sendData();

      }
      catch (RemotingException ex)
      {
        // remoteが切断されたらどうするか？
        // TODO logmanagerの再起動 or ファイルへ書き出しし、logmanagerで読み込み
        string buff = log.message;
      }
      // ipclogobj.tracefile = log.tracefile;
      // ipclogobj.tracesub = log.trace;
      
      // ipclogobj.message = log.message;
      // string buff = "aa";
      // ipclogobj.sendData();
      // ipclogobj.sendData(buff);
      // ipclogobjがnullになってる？
      // security errorがでる ちょっと、テストpgを作って検証するべきだな
      // l = ipclogがだめだ シリアライズできていない -> deserializeもできない
      // stringでひとつづつ渡すべきかもねー
      // [SecurityPermission(SecurityAction.Demand)]

      


      // mutex <- semaphore どっちがいいの？

      return true;
    }



    // TODO env.iniよりログレベルを取得し、記録するもののみipcでsendする


    public static void Dispose()
    {
      // _logqueueは常にwaitして動かしているので dispose処理は特に行わない
      _logqueue?.Dispose();
      // ret = Semaphores.release(semaphokey);
      // Semaphores.deleteSemaphore(semkey_cllient_Queue_lock);

      logwriter?.Dispose();
    }

    // 表示内容は、
    // exe,エラーソース、エラー行番号,type,メッセージ
    // テキストに書き込む場合は履歴管理が必要
    // キューを作り、それに登録する
    // log queue classとjob data classが必要

    // スレッドで書き込み処理を待機しておき、log jobが登録されたらasyncで書き込みを行う
    // 別プロセスのpipe or sharemme書き込みも対応しておくと、ロガーとして別exeで実行できる
    // pipeにするか、セマフォにしてsharememにするか、テキストにするか？



    // 画面表示を含めログを書き出す
    // logs write -> _jobquque -> 画面へ書き出し + ipcでlogsend

    public static void write(string mess)
    {
      if (writelogflag == false) { return; }
      // Console.WriteLine(arg);
      LogJob l = new LogJob();
      l.message = mess;
      l.exename = _exename;
      Logs.messagectl?.showMessage(mess);
      write(l);
    }

    public static void write(string mess, Logtype lgtype = Logtype.info)
    {
      if (writelogflag == false) { return; }
      // string exe = getexename();
      LogJob l = new LogJob();
      l.exename = _exename;
      l.message = mess;
      l.logtype = lgtype;
      Logs.messagectl?.showMessage(mess);
      write(l);
    }
    public static void write(string mess, Logtype lgtype, Exception e)
    {
      if (writelogflag == false) { return; }
      LogJob l = new LogJob();
      l.message = mess;
      l.exename = _exename;
      l.logtype = lgtype;
      l.e = e;
      Logs.messagectl?.showMessage(mess);
      write(l);
    }
    public static void write(Exception e)
    {
      if (writelogflag == false) { return; }
      LogJob l = new LogJob();
      l.message = e.Message;
      l.exename = _exename;
      l.logtype = Logtype.error;
      l.e = e;
      Logs.messagectl?.showMessage(e.Message);
      write(l);

    }

    /// <summary>
    /// ここの処理はクライアントから直接呼び出すのではなく、各write処理からcallされる
    /// </summary>
    /// <param name="l"></param>
    private static void write(LogJob l)
    {
      if (writelogflag == false) { return; }
      l.exename = _exename;
      _logqueue.addObj(l);
    }

    // utilにあるはず
    /*
    private static string getexename()
    {
      if (_exename == "")
      {
        // dllではなくprocessのexenameを取得する
        _exename = Path.GetFileName(System.Reflection.Assembly.GetCallingAssembly().Location);
      }
      return _exename;
    }
    */

    private static void getexetraceinfo(StackFrame sf, out string methodname, out int souceline)
    {
      System.Reflection.MethodBase callm = sf.GetMethod();
      methodname = callm.Name;
      souceline = sf.GetFileLineNumber(); // 呼び出し元の行番号を表示

    }

    /*
    public static void dispmessage()
    {
      string exe = getexename();
      const int findex = 2;
      StackFrame sf = new StackFrame(findex);
      // 呼び出し元のメソッド名を取得
    }
    */


    public static string getlogtypename(Logtype l)
    {
      switch (l)
      {
        case Logtype.dispinfo:
          return "画面情報";
        case Logtype.dispwarn:
          return "画面警告";
        case Logtype.disperror:
          return "画面エラー";
        case Logtype.debug:
          return "デバッグ";
        case Logtype.info:
          return "情報";
        case Logtype.warn:
          return "警告";
        case Logtype.error:
          return "エラー";
      }
      return "情報";
    }

    // このクラスをipcとして利用する
    // exception eがserializeできない
    // logに登録するjobのobjデータ
    public class LogJob
    {
      public string logdate;
      public string message;
      public Logs.Logtype logtype;
      public int logtypeint;
      public string exename;
      public string tracefile;
      // exception eがremotingに対応していないため、ipc remoteは使用できない
      public Exception e;

      public LogJob()
      {
        logdate = Utils.getNowDatetime();
        message = "";
        logtype = Logs.Logtype.info;
        e = null;
      }

      public bool saveStackTrace(out string tracefile)
      {
        tracefile = "";
        if (e == null)
        {
          return false;
        }
        if (e.StackTrace == null)
        {
          return false;
        }
        // globalのlog directoryが参照できないため、パスを決定できない
        string tfile = Utils.Files.getfilepath(logfile);

        tfile += Utils.getNowDatetime() + "_trace_" + Guid.NewGuid().ToString() + ".log";
        File.WriteAllText(tfile, e.StackTrace.ToString());
        tracefile = tfile;
        return true;
      }

    }


  }
  public class MsgControl
  {
    private ToolStripStatusLabel _label;
    private ToolStripProgressBar _progressbar;
    private ToolStrip _toolstrip;

    // GUIへのメッセージ表示コントロールの登録
    public MsgControl(ToolStripStatusLabel lblmsg, ToolStripProgressBar pgbar)
    {
      _label = lblmsg;
      _progressbar = pgbar;
      // invokeがtoolstripに対してしかできないため、toolstripの参照を保存しておく
      _toolstrip = lblmsg.GetCurrentParent();
    }

    public bool checkformclosing()
    {
      if (_label.GetCurrentParent().FindForm().IsDisposed == true)
      {
        return true;
      }

      return false;
    }

    public void showMessage(string mes)
    {
      if (_toolstrip.InvokeRequired == true)
      {
        _toolstrip.BeginInvoke((MethodInvoker)(() => {
          _label.Text = mes;
          // Utils.setProgressbarColor(this.pbDisk, diskvalue);
          // Utils.setProgressbarColor(this.pbMemory, memvalue);
        }));
        return;
      }
      _label.Text = mes;
    }
    public void setProgress(int val)
    {
      if (val < 0)
      {
        val = 0;
      }
      if (val > 100)
      {
        val = 100;
      }
      if (_toolstrip.InvokeRequired == true)
      {
        _toolstrip.BeginInvoke((MethodInvoker)(() => {
          _progressbar.Value = val;
        }));
        return;
      }
    }



  }

}

