using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Remoting.Channels.Ipc;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Lifetime;
using System.Runtime;
using System.IO;


// ３つの方法でlogを受け取る
// 1 c# ipc
// 2 mmf + semaphore
// 3 file + filenmae + named pipe
// 4 arg log file

// ipcでログを渡すためのクラス
// ipc経由でのlog送受信に使用
// 
namespace saltstone
{
  [Serializable]
  public class IPCLog : MarshalByRefObject,ISponsor
  {
    // ipcで送受信されるobj
    public static IPCLog ipclogobj;


    public string logdate 
    {
      get; set; 
    } // yyyymmdd_hhmmss
    public string exename 
    {
      get; set; 
    }
    public string message {
      get; set;
    }
    [NonSerialized]
    public Logs.Logtype logtype;
    //    { 
    //      get; set; 
    //    }

    [NonSerialized]
    private DateTime lastRenewal;

    public int logtypeint;

    public string tracefile {
      get; set;
    }
    public string tracesub {
      get; set;
    }
    // e.stacktraceの先頭の1k?バイト
    // public Exception e;

    

    public IPCLog()
    {
      logdate = "";
      exename = "";
      message = "";
      logtype = Logs.Logtype.info;
      tracefile = "";
      tracesub = "";
      // e = null;
      // init();
    }

    public IPCLog(Logs.LogJob l)
    {
      logdate = l.logdate;
      exename = l.exename;
      message = l.message;
      logtype = l.logtype;
      logtypeint = l.logtypeint;
      // traceは別ファイルに作成する
      // string logpath = Utils.Files.getfilepath(Logs.logfile);
      // e = l.e;
      // exceptionがipcで送受信できるかテスト

    }

    public TimeSpan Renewal(ILease lease)
    {

      lastRenewal = DateTime.Now;
      return TimeSpan.FromSeconds(20);
    }


    public void copy(Logs.LogJob l)
    {
      logdate = l.logdate;
      exename = l.exename;
      message = l.message;
      logtype = l.logtype;
      logtypeint = l.logtypeint;
      // e stack traceもほしい 文字列かファイルにする
      if (logtypeint == 0)
      {
        logtypeint = (int)l.logtype;
      }

    }


    // c# ipcをつかうより、mmfを使い、binary serializeしたほうがよいかも
    // https://thedeveloperblog.com/c-sharp/serialize-list


    // event handlerを作成してclientからsververへメッセージを送信するか？
    // それとも別の形式にするか？
    /*public class RObjEvnetArg : EventArgs
    {

    }*/
    /*
    public class LogEvnetArg : EventArgs
    {
      public IPCLog _log;
      public LogEvnetArg(IPCLog arg)
      {
        _log = arg;
      }
    }

    public delegate void CallEventHandler(LogEvnetArg e);
    public event CallEventHandler recieveevent;
    */
    /*
    public void sendData(IPCLog l)
    {
      if (recieveevent == null)
      {
        return;
      }
      recieveevent(new LogEvnetArg(l));
    }
    */

    // logsのlog reciver taskからcall
    // low levelのlogfile write + gui disp 処理
    // 実ファイルへの書き出し
    public void write()
    {
      // logs.logfileに対し書き込みを行う
      // 日付.log + 日付_ymdhms.trace
      string buff = logdate;
      buff += "," + exename;
      buff += "," + Logs.getlogtypename(logtype);
      buff += "," + message;
      buff += "," + Utils.Files.getfilename(tracefile);
      // traceファイルが問題 logjob -> ipclogで必ず作られるはずだから、ちゃんとtraceがymdhmd.logに保存され、filenameがtraceに入っているはず

      // 別スレッドで動かしているので、asyncは使用しない
      Logs.logwriter.WriteLine(buff);
      Logs.logwriter.Flush();
    }
    public class LogEvnetArg : EventArgs
    {
      public IPCLog _log;
      public LogEvnetArg(IPCLog arg)
      {
        _log = arg;
      }
    }

    public delegate void CallEventHandler(LogEvnetArg e);
    public event CallEventHandler recieveevent;


    public void sendData()
    {
      //LogServer s = LogServer.getInstance();
      /*
      if (recieveevent == null)
      {
        return;
      }
      */
      // IPCLog l = new IPCLog();
      // l.message = this.message;
      recieveevent(new LogEvnetArg(this));
      // ちゃんと動く
    }

    public void sendData(IPCLog l)
    {
    }


  }
}
