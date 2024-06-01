using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


// TODO serializeされてきたものの型を確認する方法はないか？


namespace saltstone
{

  public class LogServer : IDisposable
  {
    // singleton modelにする
    private static LogServer _server = null;
    public delegate void deg_recieveLog(Logs ls);
    public deg_recieveLog evt_recieveLog;

    SNamedpipeServer _pipeserver;

    /// <summary>
    /// log file 監視 object
    /// </summary>
    public System.IO.FileSystemWatcher watcher;

    public Dictionary<int, string> _logfiles;

    // log managerへ表示を行うためのdelegate
    // public delegate void del_displog(IPCLog l);
    // public del_displog evt_displog = null;


    public static LogServer getInstance()
    {
      if (_server == null)
      {
        _server = new LogServer();
      }
      // _server.ipclogobj = new IpcRemoteLog();
      // logmanagerが起動されていれば登録されているはず
      return _server;
    }
    public void Dispose()
    {
      // ipcサーバのクローズはしなくてもよさそう
      // _logqueue.Dispose();
      _logfiles?.Clear();
      _logfiles = null;
      watcher?.Dispose();
      watcher = null;
    }

    public bool initServer()
    {
      // named pipeの受信処理を行う
      // named pipeでは、logのファイル名を受信する
      // memory mapped fileは使用しない


      // traceは別ファイルにする logにはtrace file nameのみを記載

      // log rotation  log4netが使えるか？
      string pipename = Logs.getPipename();
      bool ret = SNamedpipes.getServer(pipename, out _pipeserver);

      // jsonでファイルを書き込む
      // named pipeでファイル名を連携する
      _pipeserver.initServer(evt_piperecieve);

      // read loopは別スレッドじゃないとだめなのでは？

      // string id = STasks.createTask(readloop);
      // pipeserver initServerでdelegateを登録しているのでskip
      // _pipeserver.evt_pipereaded += evt_piperecieve;
      _pipeserver.proctask();

      string buff = Utils.Sysinfo.getLogdir();
      watcher = new FileSystemWatcher(buff);
      watcher.NotifyFilter = NotifyFilters.FileName;
      watcher.Filter = "*.txt";
      watcher.Created += waitlogfile;
      watcher.EnableRaisingEvents = true;

      _logfiles = new Dictionary<int, string>();

      return true;
    }
    
    //public void readloop()
    //{
    //    _pipeserver.readloop();
    //}

    public void evt_piperecieve(Logs arg)
    {
      string buff = arg.message;
      if (evt_recieveLog != null)
      {
        evt_recieveLog(arg) ;
      }
    }

    /// <summary>
    /// log directoryを監視し、file createされたらaws log serverへ送る
    /// and formへevent fireする
    /// </summary>
    public void waitlogfile(object sender, FileSystemEventArgs arg)
    {
      string fname = arg.FullPath;

      // log fileと trace fileの２つ作成される
      // traceはformにevent fireしない
      if (fname.Contains("_trace") == true) {
        return;
      }

      // create file , write fileで２回発生する。
      // 一回発生は保証されていない
      // https://teratail.com/questions/152501
      Console.WriteLine("event");
      // 一度、log取得したfileを記録しておく
      if (_logfiles == null) {
        return;
      }
      int i = string.GetHashCode(fname);
      if (_logfiles.ContainsKey(i) == true) {
        return;
      }

      using (StreamReader ss = new StreamReader(fname)) {
        string buff = ss.ReadToEnd();
        Logs l = new Logs();
        l.setSerialize(buff);
        evt_recieveLog(l);
      }

      // log処理したfileは保存しておく

      _logfiles.Add(i, fname);


    }

  }


}

