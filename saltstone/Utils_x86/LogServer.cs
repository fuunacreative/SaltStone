using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;
using System.Runtime.Remoting;
using System.Security.Permissions;

// logManager(ログの受け取り＋書き出し側)
// で使用する処理をまとめるクラス
namespace saltstone
{
  public class LogServer : IDisposable
  {
    // singleton modelにする
    private static LogServer _server;

    // log managerへ表示を行うためのdelegate
    public delegate void del_displog(IPCLog l);
    public del_displog evt_displog = null;


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

    public const string IPC_ChannelName = "ipcsaltstone";
    public const string IPC_objectName = "LogObj";

    //public IpcRemoteLog ipclogobj;
    public IPCLog ipclogobj;
    // public Queue<IPCLog> _log_serverqueue;
    // queueを排他制御するためのsemaphore
    public string sem_queuelock;
    // ququeの処理を開始するためのsemaphore
    public string sem_ququeproc;
    public SQueue<IPCLog> _logqueue;

    // ququeを排他するのはいいんだけど、まとめられないかな？

    public void Dispose()
    {
      // ipcサーバのクローズはしなくてもよさそう
      _logqueue.Dispose();

    }

    

    public bool initServer()
    {
      // log.exeが起動されていなければ起動し
      // log.exeでipc recieve logを待機する
      // string buff = "ipc://" + IPC_ChannelName + "/" + IPC_objectName;
      IpcServerChannel servChannel = new IpcServerChannel(IPC_ChannelName);
      //IpcServerChannel servChannel = new IpcServerChannel(IPC_ChannelName,null,new );

      // リモートオブジェクトを登録
      ChannelServices.RegisterChannel(servChannel, true);
      LogServer s = LogServer.getInstance();
      /*
      s.ipclogobj = new IpcRemoteLog();
      recieveevent += evt_logRev;
      */
      s.ipclogobj = new IPCLog();
      s.ipclogobj.recieveevent += evt_logRev;
      // eventは起動するか、queueのwriteが動作しない

      // recievelog.recieveevent += Ipclogjob_recieveevent;
      RemotingServices.Marshal(s.ipclogobj, IPC_objectName, typeof(IPCLog));
      // loopはどうするの？
      // たぶんmarshrlは一度でよい -> 
      _logqueue = new SQueue<IPCLog>();
      _logqueue.evt_queueobjadded += evt_queuelogrev;
      _logqueue.proctask();

      // memory mapped fileによるログの受付
      // c++からの呼び出しを考慮してfilenameとsemaphoreを共通にしとかいなとだめ
      // Squeueのような仕組みがほしいな -> いや無理か
      // semaphoreにeventを登録できるとよい
      // share mem 名 "local/saltstonelog"
      // semaphore name "saltstonelog"
      // TODO mmf create時にsem nameを指定できるように

      MemoryFile mmf =  MemoryFiles.create("local/saltstonelog", 1024);
      // mmfにどうやってlogを格納するか？
      // marshalできるlogが必要 ipclog？
      return true;
    }

    public void evt_logRev(IPCLog.LogEvnetArg arg)
    {
      // server側で受信処理を行ったときの処理
      // recievelogにsendしたlogが張っているはず

//      IPCLog l = arg._log;
      // 高速処理に対応するため、queueに追加する
//      l.write();
      // 高速化のため、ここではfile writeせず、いったんサーバ側queueに登録する
      _logqueue.addObj(arg._log);
      // _logqueueのadd時のイベントが発生していない

    }

    public bool evt_queuelogrev(IPCLog obj)
    {
      obj.write();
      // exceptionのeがちゃんと受け取れてるかどうか
      // Exception le = obj.e;
      obj.logtype = (Logs.Logtype)Enum.ToObject(typeof(Logs.Logtype), obj.logtypeint);
      if (evt_displog == null)
      {
        return true;
      }
      evt_displog(obj);
      return true;
    }
      





    // ipcで送信された場合、受信側にイベントという形で通知される
    // 受信したものをqueueに保存し、別スレッドで書き出しを行う

    // 問題は、
    // 1. sharememを使用したログ受信に対応できるかどうか
    // 1. exception eがipcで送信できるか？
    // 1. traceファイルを別で書き出ししたい -> exceptionのシリアライズが必要？
    //   or send側で別タスクで書き出しを行う？
  }
}
