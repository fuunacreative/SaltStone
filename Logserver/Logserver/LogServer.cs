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

    SNamedpipeServer _pipeserver;


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
      STasks.createTask(readloop);

      return true;
    }
    
    public void readloop()
    {
      _pipeserver.readloop();
    }

    public void evt_piperecieve(string arg)
    {
      string buff = arg;
    }


  }
}

