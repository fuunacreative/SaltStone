using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
// using System.Threading.Tasks;
using System.Threading;

// セマフォを一元管理するクラス
namespace saltstone
{
  public static class Semaphores
  {
    // singletonにするか？
    // static classにする
    public const int SEM_WaitSec = 1000;


    private static Dictionary<string,SSemaphore> _semaphores;

    public static void init()
    {
      if (_semaphores == null)
      {
        _semaphores = new Dictionary<string, SSemaphore>();
      }
    }
    public static void Dispose()
    {
      if(_semaphores != null) 
      {
        foreach (SSemaphore s in _semaphores.Values)
        {
          s.Dispose();
        }
        _semaphores.Clear();
        _semaphores = null;
      }
    }
    public static string create()
    {
      init();
      // string guid = Guid.NewGuid().ToString();
      SSemaphore s;
      // s = new Semaphore(1, 1, guid, out ret);
      s = new SSemaphore();
      // ret = false -> not created;
      _semaphores.Add(s.key,s);
      return s.key;
    }
    public static bool delete(string semkey)
    {
      if (_semaphores == null)
      {
        return false; // for debug
      }
      if (_semaphores.ContainsKey(semkey) == false)
      {
        return false;
      }
      SSemaphore s = _semaphores[semkey];
      s.clear();
      _semaphores.Remove(semkey);
      s.Dispose();
      s = null;
      return true;
    }

    public enum enum_SemaphoreWait
    {
      NoLimit,
      WaitMin
    }

    public static bool waitone(string key,enum_SemaphoreWait waitflag = enum_SemaphoreWait.WaitMin)
    {
      // null error
      // taskもdisposeしてるのに、waitoneが走る
      if (_semaphores == null)
      {
        return false;
      }
      SSemaphore s = _semaphores[key];
      // TODO timeoutは考慮していない
      // TODO form closeしてもwaitoneしているときがある
      bool ret = false;
      if (waitflag == enum_SemaphoreWait.WaitMin)
      {
        ret = s.waitOne(SEM_WaitSec);
      } else
      {
        ret = s.waitOne();
      }
      if (ret == false)
      {
        return false;
      }
      return true;
    }
    public static bool release(string key)
    {
      bool ret = _semaphores[key].release();
/*      Semaphore s = _semaphores[key];
      try
      {
        s.Release();
      }
      catch (Exception e)
      {
        // やはり重複開放エラーとなるな？　なぜだろう
        // Logs.write(e);
        string buff = e.Message;

       //  return false;
      }
*/
      return true;
    }
  }


  // 同じものが２個あるんじゃないな、Semaphoreを使い、ssemaphoreを作成している semaphoresが全部の集合を管理している
  public class SSemaphore : IDisposable
  {
    public Semaphore _semaphore;
    public int count;
    public int maxcount;
    public string key;
    public delegate void del_semaphoreproc();
    public del_semaphoreproc semaphoreproc;

    

    public SSemaphore(string arg = "")
    {
      key = arg;
      if (key.Length == 0)
      {
        key = Guid.NewGuid().ToString();
      }
      maxcount = 1;
      bool ret = false;
      _semaphore = new Semaphore(1, 1, key, out ret);
      // 初期状態でinit max 1にしておき、作成時に既に処理開始(no lock)状態にしておく
      // initial , mactount , sem_name,createdNew
      if (ret == false)
      {
        // 既にsemaphreが存在する
        count = 0;
        do
        {
          try
          {
            _semaphore.Release();
          }
          catch (Exception e)
          {
            break;
          }
            


        } while (true);
        return;

      }
      count = 1;
      // 初期状態でセマフォの数は１(最大値は１)
      return;
    }

    public void Dispose()
    {
      _semaphore.Dispose();
    }

    public bool clear()
    {
      // semのlockをすべて解除
      // countでclearするか、それともtryでexcepがでるまでクリアするか？
      // countだと漏れが出る可能性がある。excepだと、最後のrelで必ず少し停止する
      // 後者を採用 確実に全部 countを0にする
      bool ret = true;
      do
      {
        // 一応、開放がすべてすむませセマフォをreleaseする
        ret = forceRelease();
      } while (ret == false);
      count = 0;
      return true;
    }

    public bool waitOne(int waittime = 0)
    {
      bool ret = false;
      // waitした場合、count downされる
      if (waittime == 0)
      {
        // count=リソースの空状況-> 通常は1 , waitしているときは0
        //if (count == 0)
        //{
        //  return true;
        //}
        // countの考え方がおかしい、、、 1でスタートしているはずだが、、、 waitoneでちゃんとロック->解除される？
        // 無期限待機
        // ret = _semaphore.WaitOne();
        ret = _semaphore.WaitOne();
        // ret = true;
      } else
      {
        ret = _semaphore.WaitOne(waittime);
      }
      if (ret == true)
      {
        count--;
      }
      if (count < 0)
      {
        count = 0;
      }
      // delegateが定義されていたら、それをコールする
      if (semaphoreproc != null)
      {
        semaphoreproc();
      }
      return ret;
    }

    public bool release()
    {
      bool ret = false;
      // ここがおかしい
      // waitoneでcountup , releaseでcountdown
      // semaphoreのcount up&downを整理する必要がある
      if (count >= maxcount)
      {
        // maxcount以上はreleaseできない -> 0であればrelaseしてountupする
        // ここは通らないはず
        return true;
      }
      try
      {
        count++;
        _semaphore.Release();
        ret = true;
      }
      catch (Exception e)
      {
        // eがsemapho no countであることを確認
        string buff = e.Message;
      }
      if (count > maxcount)
      {
        count = maxcount;
      }
      return ret;

    }

    public bool forceRelease()
    {
      try
      {
        _semaphore.Release();
      }
      catch (Exception e)
      {
        string buff = e.Message;
      }
      return true;

    }






  }
}
