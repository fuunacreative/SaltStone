using System;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

// log4net
// log rotationができるとよい
// 出力先を変更できるとよい
// ファイル名をymd_hmsにできるとよい
// 過去のログを削除できるとよい
// https://freeit.hatenablog.com/entry/2018/03/30/193945

/*
logmanagerが起動しているかどうかをチェックするフラグをmemberとしてもつ
logs class以外に、logmanagerを起動するクラスを持つ？
いや、logsにもたせるべきか。 logs.startLogManager()などとする
iniファイルの読み込みをここで行いたいな、、、
いや、iniファイルの整理を行うべきだな log4netは開発終了している -> 設定ファイルの設計を行う
exeini   , logini
inifileをutilsへ移動
グローバルini -> settings.ini
exeini -> exename.ini
保存はどうする？ iniファイルに保存する？ -> ok 問題なし
最近のはやりはxmlファイルでsettingを書くこと -> xmlファイルは可読性がよくないので使わない
 */

namespace saltstone
{
  /// <summary>
  /// jsonファイルに保存する log message情報
  /// logserverの起動は実exe側で行う
  /// dispinfoのために、msgcontrolをmemgerとして保持する
  /// </summary>
  /// 
  [Serializable()]
  public class Logs : INamedpipedata
  {


    /// <summary>
    /// serializeをしてバイトでデータ送受信を行う際に使用する５バイトのheader 
    /// </summary>
    public const string Const_SerializeID = "LOG__";
    // logserverのprocess存在判定をおこなためのconst
    // TODO iniファイルに記載したいが、globalsを含むとどうなるか？
    // utilsは単体で動作させたいんだよなー
    // [System.Text.Json.Serialization.JsonIgnore]
    // private const string Const_LogserverExe = "logserver.exe";

    /// <summary>
    /// named pipe clientはprivateにして開きっぱなしにする
    /// </summary>
    private static SNamedpipeClient cpipe = null;


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

    #region static_member
    /// <summary>
    /// 直近のlogfileを保存しておく
    /// </summary>
    [System.Text.Json.Serialization.JsonIgnore]
    public static string logfile;

    [System.Text.Json.Serialization.JsonIgnore]
    public static string tracefile;

    /// <summary>
    /// msgcontrolは必要なのか、、、
    /// 場合によるが、logsはutilsにあるので統一して出せるとpgしやすい
    /// </summary>
    [System.Text.Json.Serialization.JsonIgnore]
    public static MsgControl _msgconrol = null;

    /// <summary>
    /// logmanagerが起動しているかどうか 
    /// serializerから除外
    /// </summary>
    [System.Text.Json.Serialization.JsonIgnore]
    private static bool _startLogmanager;

    [System.Text.Json.Serialization.JsonIgnore]
    private static string _logserer_exename;

    [System.Text.Json.Serialization.JsonIgnore]
    private static string _pipename;

    #endregion

    #region member

    /// <summary>
    /// readするバイト数とデータの種類（この場合はlog)を保存したいな、、、
    /// 自動ではnamed pipe recieveではやってくれない 
    /// namedpipeでやるので必要ない
    /// </summary>
    // public int header;

    public string logdate
    {
      get; set;
    } // yyyymmdd_hhmmss
    public string exename
    {
      get; set;
    }
    public string exever
    {
      get; set;
    }
    public string message
    {
      get; set;
    }


    public string method
    {
      get; set;
    }

    public string sourcefile
    {
      get; set;
    }

    public int sourceline
    {
      get; set;
    }

    /// <summary>
    /// exceptionのstacktrace
    /// </summary>
    public string trace
    {
      get; set;
    }

    public Logs.Logtype logtype;


    #endregion

    // interfaceにするべきか or abstract classにするべきか？
    public string DataID
    {
      get
      {
        return Const_SerializeID;
      }
    }

    #region constructor
    public Logs()
    {
      logtype = Logtype.info;
      logdate = Utils.getNowDatetime();
      exename = "";
      exever = "";
      message = "";
      method = "";
      sourcefile = "";
      trace = "";
    }

    public Logs(Exception ex) : this()
    {

      // 先にlogsのconstructerを動かす
      logtype = Logtype.error;
      exename = ex.Source;

      string buff = Utils.Sysinfo.getCurrentExepath() + "\\" + exename + ".exe";
      exever = Utils.Files.getExeVersion(buff);
      message = ex.Message;
      trace = ex.StackTrace;
      // exceptionのtraceを別classにする必要があるか？　別pjで使うか？ -> おそらく使わない      
      // まず、trace file nameを作成する -> writeで作成する

      // TODO method , source  file,lineを取得できるが、取得してlog出力するか?
      // method <- stack trace
      // sourceline  <- stack trace
      // trace
      // source file ? 必要か？  <- stack trace
      // memo ?  必要か？
      //string a = ex.Source; // 多分 exename
      //string b = ex.StackTrace;  // sourcefile ,method , exception line
      /* 
       * ? a  "LogManager_test"
?b  " at LogManager_test.Form1.cmdLogWrite_Click(Object sender, EventArgs e) in 
C:\\Users\\yasuhiko\\source\\saltstone\\Logmanager_test\\LogManager_test\\Form1.cs
:line 26"
*/

    }

    #endregion

    public static void Dispose()
    {
      cpipe?.Dispose();
    }

    public static bool startLogManger()
    {
      // TODO  logmanagerのexeを起動する

      return true;
    }

    public string getSerialize()
    {
      // DataIDをセットするかどうか？
      // 特にセットしない
      string buff = JsonSerializer.Serialize(this);
      return buff;
    }

    public bool setSerialize(string buff)
    {
      // json serialied string to logs object
      // string json = System.Text.Encoding.UTF8.GetString(value);
      // 先頭5バイトはdataIDとして読み飛ばす
      // string buff = value.Substring(5);
      Logs l = JsonSerializer.Deserialize<Logs>(buff);
      // memberをcopy
      this.logdate = l.logdate;
      this.exename = l.exename;
      this.exever = l.exever;
      this.message = l.message;
      this.method = l.method;
      this.sourcefile = l.sourcefile;
      this.trace = l.trace;
      this.logtype = l.logtype;

      return true;
    }

    // TODO logserverのexeがあればnamed pipeでsend、なければfile writeする
    // 検討事項：名前を何にするか？ sendで named pipe write or file write する
    // 検討事項：namedpipeで何を送信するか？ jsonfile or serialized_log?


    public bool send()
    {
      // logserverが起動しているか確認する
      // logserver.exeが実行されているかどうか
      // bool ret = Utils.checkrunexe(Const_LogserverExe);
      if (_startLogmanager == false)
      {
        // writteのみにする
        write();
        return true;
      }

      // named pipeへ送信する



      // TODO named pipe + json fileを使い、send処理を行う


      string buff = JsonSerializer.Serialize(this);
      // ファイルへ書き込む
      string fname = Utils.Sysinfo.getLogdir();

      return true;
    }

    public static bool send(string arg)
    {
      // logmanagerへ送信
      return true;
    }

    public static bool send(Exception ex)
    {
      // logmanagerへ送信
      // traceファイルを別に作成する
      // logs\20240120_1159.txt
      // logs\20240120_1159_trace.txt
      Logs l = new Logs(ex);

      // logserverが起動していないときは単純にwriteして終わる
      if (Logs._startLogmanager == false)
      {
        l.write();
        return true;
      }

      //string buff = "";
      //try
      //{
      //  // TODO 異常終了する、、、なぜ？
      //  //buff = JsonSerializer.Serialize<Logs>(l);
      //  buff = l.data;
      //} catch (Exception inex)
      //{
      //  string err = inex.Message;
      //}
      SNamedpipes.getClient(_pipename, out cpipe);
      // TODO cpipeをopenしっぱなしだとうまく送信できない
      // かといってdisposeしてしまうと、うまく動かない
      // 一度、private memberにして開きっぱなしにしてみる
      //using (cpipe) {
      //  cpipe.connect();
      //  cpipe.send(l);
      //}
      cpipe.connect();
      cpipe.send(l);

      return true;
    }

    public bool write()
    {
      // logfile nameを作成する
      // exedir\logs\20240101_1000.txt
      string logf = Utils.Sysinfo.getLogfile();
      Logs.logfile = logf;


      // fileへ書き込む
      // logmanagerへの画面表示は行わない -> logserver側で処理する
      if (Logs.logfile == null)
      {
        return false;
      }

      // traceをどうするか？
      // exceptionを発生させ、強制的にtraceを作成し、ファイルに書き込む？
      // trace (ex.stacktrace)がある場合には、ファイルを作成しておく
      if (string.IsNullOrEmpty(trace) == false)
      {
        tracefile = Utils.Sysinfo.getTracefile();
      }

      // utilsからslib　slibglobalを参照したくない。　
      string buff = getlogtext();
      using (StreamWriter fs = new StreamWriter(logf, true, Utils.getEncodingUTF8()))
      {
        fs.WriteLineAsync(buff);
      }

      if (string.IsNullOrEmpty(trace) == false)
      {
        using (StreamWriter fs = new StreamWriter(tracefile, true, Utils.getEncodingUTF8()))
        {
          fs.WriteAsync(trace);
        }
      }


      return true;
    }

    public static bool write(string arg)
    {
      // fileへ書き込む
      // logmanagerへの画面表示は行わない -> logserver側で処理する
      if (Logs.logfile != null)
      {
        return false;
      }
      string buff = getlogtext(arg);

      // StreamWriter は、特に指定がない限り、 のインスタンスを UTF8Encoding 使用するように既定で設定されます。
      using (var fs = new StreamWriter(logfile, true, Utils.getEncodingUTF8()))
      {
        fs.WriteLineAsync(buff);
      }
      return true;
    }

    public static bool write(Exception ex)
    {
      Logs l = new Logs(ex);

      l.write();

      return true;
    }

    public static bool write(string arg, [System.Runtime.CompilerServices.CallerMemberName] string memberName = "", [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0)
    {
      if (Logs.logfile != null)
      {
        return false;
      }
      string buff = getlogtext(arg, memberName, sourceLineNumber);

      using (var fs = new StreamWriter(logfile, true, Utils.getEncodingUTF8()))
      {
        fs.WriteLineAsync(buff);
      }

      return true;
    }

    private static string getlogtext(string arg, string method = "", int lineno = 0)
    {
      string menthodname = System.Reflection.MethodBase.GetCurrentMethod().Name;

      // log内容
      // logdate ,loglevel , exename ,ver ,  message , errorsourceloc , trace
      Logs lbuff = new Logs();
      lbuff.logdate = Utils.getNowDatetime();
      lbuff.exename = Utils.Sysinfo.getExeName();
      lbuff.logtype = Logtype.error;
      lbuff.message = arg;
      // lbuff.exever = 
      string buff = getlogtext(lbuff);
      return buff;
    }

    private string getlogtext()
    {
      string buff = "";
      // log fileに書き出す line textを編集する
      // buff += logtype.ToString();

      // TODO buffを組み立てる前に、properyを設定する
      // exename-> 設定がなければ現在実行中のものをセットなど
      buff += Enum.GetName(typeof(Logtype), logtype);
      buff += ",";
      if (logdate == null || logdate.Length == 0)
      {
        logdate = Utils.getNowDatetime();
      }
      buff += logdate;
      buff += ",";
      // exenameは指定がされていないければ、現在のexeを取得
      if (string.IsNullOrEmpty(exename) == true)
      {
        exename = Utils.Sysinfo.getExeName();
      }
      buff += exename;
      buff += ",";
      // exeverをどう取得するか？
      // asyncでwriteしたいな、、、
      if (exename != null && exename.Length > 0)
      {
        if (exever == null || exever.Length == 0)
        {
          exever = Utils.Files.getExeVersion(exename);
        }
      }
      if (exever != null && exever.Length > 0)
      {
        buff += "ver:" + exever;
      }
      buff += ",";
      buff += message;

      buff += "," + method;
      // sourcelineは0の場合、skipする
      buff += "," + sourceline.ToString();
      buff += "," + tracefile;


      return buff;

    }



    private static string getlogtext(Logs arg)
    {
      return arg.getlogtext();
    }


    private static System.Timers.Timer _timer;


    /// <summary>
    /// logmanagerを使用する場合にinit()をcallする
    /// </summary>
    public static void init()
    {
      //       init();
      // logsでmsg controlはstaticにするべきか？ or obj memberにするべきか
      // statisにすると、ひとつのメインファイルにしかログ出力ができない、、、
      // 逆に考えて、２つのformにmessageを出力するケースがあるか？
      // 当然あるよね、、、　となると、logsのインスタンスと、メッセージ表示用のclassを別にしたほうが綺麗にできるか、、、
      // msgcontrolclassがあるのだから、これで表示すればいいのでは？
      // となると、ここで表示するのは、formの指定なし。main windowに表示することを想定すればよい
      if (_msgconrol != null)
      {
        _msgconrol = null;
      }
      Inifile inifile = new Inifile("logs.ini");
      if (inifile == null)
      {
        return;
      }
      _logserer_exename = inifile.get("Logserver");
      _pipename = inifile.get("pipename");
      if (string.IsNullOrEmpty(_logserer_exename))
      {
        return;
      }
      checkLogserver();

      // logserverが起動しているかどうかをチェックする
      // タイミングは、、、send or timer
      // utilsにtimer classがあり、一括で管理している。 このため、global disposeで一括してdropしている
      // logs単体では、、、 5sに一時、タイマーを使って無条件で起動チェックを行う
      _timer = new System.Timers.Timer();
      _timer.Interval = 5000; // every 5s
      _timer.Elapsed += p_checkLogserver;
      _timer.AutoReset = true; // event repeatly
      _timer.Enabled = true;

    }


    /// <summary>
    /// init()を使いlogserverを使用する際にtimerで実行される
    /// 5sに一度、logserverが起動しているかどうかをチェックする
    /// </summary>
    /// <param name="source"></param>
    /// <param name="e"></param>
    private static void p_checkLogserver(Object source, System.Timers.ElapsedEventArgs e)
    {
      checkLogserver();
    }
    private static void checkLogserver()
    {
      _startLogmanager = false;
      bool ret = Utils.checkrunexe(_logserer_exename);
      if (ret == true)
      {
        _startLogmanager = true;
        return;
      }
    }

    public static string getPipename()
    {
      Inifile inifile = new Inifile("logs.ini");
      if (inifile == null)
      {
        return "";
      }
      string buff = inifile.get("pipename");
      if (string.IsNullOrEmpty(_logserer_exename))
      {
        return "";
      }
      return buff;
    }

    /// <summary>
    /// main windowのmsg_control
    /// 通常はwindow毎にmsgcontrolを作成する
    /// </summary>
    /// <param name="_msg"></param>
    public static void init(MsgControl _msg)
    {
      init();
      if (Logs.logfile == null)
      {
        Logs.logfile = Utils.Sysinfo.getLogfile();
        return;
      }

      // msgcontrolが指定されている場合には、log出力に使用するため保存しておく
      _msgconrol = _msg;
    }
  }
}
