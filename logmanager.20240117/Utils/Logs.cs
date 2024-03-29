﻿using log4net.Appender;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.Json;

// log4net
// log rotationができるとよい
// 出力先を変更できるとよい
// ファイル名をymd_hmsにできるとよい
// 過去のログを削除できるとよい
// https://freeit.hatenablog.com/entry/2018/03/30/193945



namespace saltstone
{
  /// <summary>
  /// jsonファイルに保存する log message情報
  /// logserverの起動は実exe側で行う
  /// dispinfoのために、msgcontrolをmemgerとして保持する
  /// </summary>
  /// 
  [Serializable()]
  public class Logs
  {

    public const string Const_LogserverExe = "logserver.exe";

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

    #region member
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
    /// named pipeで送信する場合のdatatype "log"固定 
    /// 場合により、固定長で10バイトとする
    /// TODO named pipe recieveでバイト数を指定しなくてもすべてのデータを読み込みできるのか？
    /// </summary>
    public string datatype;

    /// <summary>
    /// readするバイト数とデータの種類（この場合はlog)を保存したいな、、、
    /// 自動ではnamed pipe recieveではやってくれない
    /// </summary>
    public int header;

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

    #region constructor
    public Logs()
    {
      logtype = Logtype.info;
      logdate = Utils.getNowDatetime();

    }

    public Logs(Exception ex) : this()
    {
      // 先にlogsのconstructerが動く
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


    // TODO logserverのexeがあればnamed pipeでsend、なければfile writeする
    // 検討事項：名前を何にするか？ sendで named pipe write or file write する
    // 検討事項：namedpipeで何を送信するか？ jsonfile or serialized_log?


    public bool send()
    {
      // logserverが起動しているか確認する
      // logserver.exeが実行されているかどうか
      bool ret = Utils.checkrunexe(Const_LogserverExe);
      if (ret == false)
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

      string buff = "";
      buff = JsonSerializer.Serialize(l);

      SNamedpipeClient cpipe = null;
      SNamedpipes.getClient("pipename", out cpipe);
      using (cpipe)
      {
        cpipe.send(buff);
      }

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


    public static void Dispose()
    {

    }

    public static void init(MsgControl _msg = null)
    {
      if (Logs.logfile != null)
      {
        return;
      }
      Logs.logfile = Utils.Sysinfo.getLogfile();

      // msgcontrolが指定されている場合には、log出力に使用するため保存しておく
      if (_msg != null)
      {
        _msgconrol = _msg;
      }
    }
  }
}
