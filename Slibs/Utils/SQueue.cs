using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Runtime.Versioning;


// ququqとsemphore排他ロックを取りまとめる
// que obj(que + sem) とsqueをまとめて管理するque manager class
namespace Utils
{
  public class SQueue<T> : IDisposable
  {
    public const int WAIT_TaskSleep = 100;

    public Queue<T> _queue;
    // que排他用セマフォ
    public string sem_lockqueue;
    // que登録時のイベント用セマフォ
    public string sem_addqueue;

    public delegate bool del_queueobjadded(T obj);
    public del_queueobjadded evt_queueobjadded;

    // queに登録 -> que登録シグナル


    // queから取出し
    // sq.add(logobj)
    // sq.evt += revfunc
    // revfunc(arg logobj) {}

    public SQueue()
    {
      _queue = new Queue<T>();
      sem_lockqueue = Semaphores.create();
      sem_addqueue = Semaphores.create();
      // add que semは待機状態にしておく
      Semaphores.waitone(sem_addqueue);
      // TODO semではなくmutexの方がよいかも
    }
    ~SQueue()
    {
      Dispose();
    }

    public void Dispose()
    {
      // Semaphores.waitone(sem_lockqueue);
      // Semaphores(sem_lockqueue);
      // queueの中身は常にからのはず
      if (_queue?.Count > 0)
      {
        // logが残っていても無条件に破棄する？
        // _queue.Dequeue();
        throw new Exception("client log queue not empty");
      }
      _queue?.Clear();
      _queue = null;
      // disposeしているが、pg終了時に再度実行されている？
      // globalでsem disposeしているため不要
      // Semaphores.delete(sem_lockqueue);
      // Semaphores.delete(sem_addqueue);
    }



    public bool addObj(T arg)
    {
      bool ret = Semaphores.waitone(sem_lockqueue);
      if (ret == false)
      {
        return false;
      }
      _queue.Enqueue(arg);
      Semaphores.release(sem_lockqueue);
      // queに登録されたことをセマフォを使って通知
      // ここで二重にreleaseしてる？
      // addobjが二重に走ってる？
      // logmanager 側のsqueueで不具合が起こっているっぽいな
      // ここでreleaseして、loop内でwaitしてるから問題ないはずだが？
      Semaphores.release(sem_addqueue);
      // ここでsem_addがcount 1になるはずだが、なっていない
      // 内部で0になっており、releaseが動かない
      // 内部のountは1になっており、queue処理が実行されていない
      // -> _queueloopでwaitoneでcountが0になってるはずだが、、、？
      return true;
    }

    [SupportedOSPlatform("windows")]
    public bool proctask()
    {
      // taskを作成し、delegateを実行する
      // queue loopをcancelしないといけない
      string taskid = STasks.createTask(_queueloop);
      return true;
    }

    [SupportedOSPlatform("windows")]
    private void _queueloop(CancellationToken ct)
    {
      // stasks createで作成しているが、canceltokenを使用してもtaskが停止しない
      if (evt_queueobjadded == null)
      {
        return;
      }
      while (true)
      {
        Util.sleep(100);
        if (ct.IsCancellationRequested == true)
        {
          return;
        }
        bool ret = Semaphores.waitone(sem_addqueue,Semaphores.enum_SemaphoreWait.NoLimit);
        if (ret == false)
        {
          Util.sleep(WAIT_TaskSleep);
          continue;
        }
        ret = Semaphores.waitone(sem_lockqueue);
        if (ret == false)
        {
          Util.sleep(WAIT_TaskSleep);
          continue;
        }
        while (_queue.Count > 0)
        {
          evt_queueobjadded(_queue.Dequeue());
        }
        // sem_lockqueueは常にrelease状態になる
        Semaphores.release(sem_lockqueue);
        // 再びserver sem queue waitをロック状態にし、待機するようにしておく
        // ここがおかしい気がするな -> waitoneでいいの？
        // どこでsemをlock状態にするのか？
        // Semaphores.waitone(sem_addqueue);

      }
    }

    /*
    public bool getObj(out T outobj)
    {
      outobj = default(T);
      bool ret = Semaphores.waitone(sem_addqueue);
      if (ret == false)
      {
        return false;
      }
      ret = Semaphores.waitone(sem_lockqueue);
      if (ret == false)
      {
        return false;
      }
      outobj = _queue.Dequeue();
      Semaphores.release(sem_lockqueue);

      return true;
    }
    */
    // この場合、getojbをcallしておく必要があり、getobj内部でwaitする
    // イベントの場合はeventにfuncを日も付けておけば処理が開始される
    // どちらがよいか？
    // たとえば、複数のobjがある場合、何度もgetObjが呼びだされる
    // それに何度もeventがraiseされる
    // event形式のほうがよさそうだな 何度もraiseされている間、queはlock状態になる




  }
}
