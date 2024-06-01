using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;


// first in firstout でのタスク処理
// 優先順位による割り込み

// tasksがstaticのせいか、他から変更されてしまう？
// _tasksallをさわる時はlockをかけているはずだが、、、

// threadを一括管理するクラス
namespace Utils
{
  public static class STasks
  {
    // タスクの優先順位つき登録と実行を含む
    private static SortedDictionary<int, Action> _tasksall;
    private static Dictionary<string,Task> _tasks;
    private static bool inited = false; // 初期化されたかどうかのフラグ
    private static string guid;

    // private static SMutex pMutexRunLock;

    private static Dictionary<string, CancellationTokenSource> _taskcancel;

    public delegate void del_tasks(CancellationToken ct);

    private static SSemaphore pRunLock;

    public static void init()
    {
      if (inited == true)
      {
        return;
      }
      _tasks = new Dictionary<string, Task>();
      _taskcancel = new Dictionary<string, CancellationTokenSource>();
      _tasksall = new SortedDictionary<int, Action>();

      guid = Guid.NewGuid().ToString();
      // pMutexRunLock = new SMutex("MUTEX_STASKS");
      pRunLock = new SSemaphore("STASKS_Run");
      inited = true;
    }
    public static void Dispose()
    {
      if (_tasks != null)
      {
        foreach (KeyValuePair<string, Task> kvp in _tasks)
        {
          if (_taskcancel == null)
          {
            continue;
          }
          CancellationTokenSource ct = _taskcancel[kvp.Key];

          ct.Cancel();
        }
        _tasks.Clear();
        _taskcancel?.Clear();
        _tasks = null;
        _taskcancel = null;
      }
      if (_tasksall != null)
      {
        _tasksall.Clear();
        _tasksall = null;
      }
      pRunLock?.Dispose();
    }

    public static bool addOrderedTask(int priority, Action act)
    {
      bool fret = false;
      if (inited == false)
      {
        init();
      }
      // 何かがおかしい。　disposeしてないのに破棄されたと判断されている？　なぜか？別スレッドだから？
      // たぶん、main global.initでtasks.initをcallして mutexを作成している
      // それに対し別スレッドでlockmutexをしようとしているのでは？
      // んーー。プロセス間通信なのに、スレッドが違うとlockできないってどーなのよ？
      // 別の仕組みを考えたほうがよさそうだなー
      pRunLock.waitOne();
      // pMutexRunLock.lockmutex();
      _tasksall[priority] = act;
      pRunLock.release();
      // pMutexRunLock.releasemutex();

      fret = true;
      return fret;
    }

    public static  void run()
    {
      // bool fret = false;
      // _tasksall clearを行っているな、、、
      // pMutexRunLock.lockmutex();
      pRunLock.waitOne();
      lock (_tasksall)
      {
        // private static SortedDictionary<int, Action> _tasksall;
        SortedDictionary<int, Action> worktasks = new SortedDictionary<int, Action>(_tasksall);


        foreach (Action act in worktasks.Values)
        {
          // task newにはactionが必須
          CancellationTokenSource ct = new CancellationTokenSource();
          CancellationToken token = ct.Token;
          Task t = new Task(act, ct.Token);
          t.RunSynchronously();
          // t.Start();
          // t.Wait(token);
        }
        _tasksall.Clear();
      }
      // pMutexRunLock.releasemutex();
      pRunLock.release();


      // fret = true;
      return ;
    }

    public static string createTask(del_tasks arg)
    {
      if (inited == false)
      {
        init();
      }
      string guid = Guid.NewGuid().ToString();
      // task newにはactionが必須
      CancellationTokenSource ct = new CancellationTokenSource();
      CancellationToken token = ct.Token;
      // Task t = new Task(act, ct.Token);
      // _tasks.Add(guid, t);
      _taskcancel.Add(guid, ct);
      // Task t = Task.Run(() => { act; },ct);
      Task t = new Task(new Action(() => arg(ct.Token)));
      t.Start();
      // t.Start();
      _tasks.Add(guid, t);
      return guid;

    }



    public static string createTask(Action act)
    {
      if (inited == false)
      {
        init();
      }
      string guid = Guid.NewGuid().ToString();
      // task newにはactionが必須
      CancellationTokenSource ct = new CancellationTokenSource();
      CancellationToken token = ct.Token;
      // Task t = new Task(act, ct.Token);
      // _tasks.Add(guid, t);
      _taskcancel.Add(guid, ct);
      // Task t = Task.Run(() => { act; },ct);
      Task t = new Task(act, ct.Token);
      t.Start();
      // t.Start();
      _tasks.Add(guid, t);
      return guid;
    }

    public static bool cancelTask(string taskid)
    {
      if (string.IsNullOrEmpty(taskid) == true)
      {
        return true;
      }
      if (taskid?.Length == 0)
      {
        return true;
      }
      if (_taskcancel.ContainsKey(taskid) == false)
      {
        return true;
      }
      _taskcancel[taskid].Cancel();
      return true;
    }
    public static bool isRunning(string taskid)
    {
      if (string.IsNullOrEmpty(taskid) == true)
      {
        return false;
      }
      Task t = _tasks[taskid];
      if (t.IsCompleted == true)
      {
        return false;
      }
      return true;
    }
  }
}
