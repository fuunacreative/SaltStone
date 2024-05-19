using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.MemoryMappedFiles;

// これもIPCじゃないからたぶん不要 dllならね
// class pointerをc++ funcに渡せばいいだけ


namespace saltstone
{
  public static class MemoryFiles
  {
    public const int DefaultSize = 1024;

    public static Dictionary<string, MemoryFile> _mmf;

    public static bool init()
    {
      if (_mmf == null)
      {
        _mmf = new Dictionary<string, MemoryFile>();
      }
      return true;
    }
    public static void Dispose()
    {
      if (_mmf == null)
      {
        return;
      }
      foreach (MemoryFile m in _mmf.Values)
      {
        m.Dispose();
      }
      _mmf.Clear();
      _mmf = null;
    }

    public static MemoryFile create(string mmfkey = "",int size = DefaultSize)
    {
      init();
      MemoryFile m = new MemoryFile(mmfkey, size);
      _mmf.Add(m.mmfkey, m);
      return m;
    }

    public static bool remove(string mmfkey = "")
    {
      init();
      if (_mmf.ContainsKey(mmfkey) == false)
      {
        return false;
      }
      _mmf[mmfkey].Dispose();
      _mmf.Remove(mmfkey);
      return true;
    }

    public static bool remove(MemoryFile arg)
    {
      return remove(arg.mmfkey);
    }

    
  }

  public class MemoryFile : IDisposable
  {
    public const int WAIT_TaskSleep = 1000;

    public string mmfkey;
    public MemoryMappedFile _mmf;
    public string sem_sharemem;
    public string sem_sharemewrite;
    public MemoryMappedViewStream _memstream;
    public delegate void del_sharememrev();
    public del_sharememrev evt_sharememrev;

    // memf m = new memf()
    // m.evt_sharemem += eventmethod
    // m.proctask()


    public MemoryFile(string mmfkey = "", int size = MemoryFiles.DefaultSize)
    {
      init(mmfkey, size);
    }

    public MemoryFile()
    {
      init();
    }

    public bool init(string mmfkey = "", int size = MemoryFiles.DefaultSize)
    {
      if (mmfkey == null || mmfkey.Length == 0)
      {
        mmfkey = Guid.NewGuid().ToString();
      }
      this.mmfkey = mmfkey;
      _mmf = MemoryMappedFile.CreateOrOpen(mmfkey, size);
      sem_sharemem = Semaphores.create();
      sem_sharemewrite = Semaphores.create();
      Semaphores.waitone(sem_sharemewrite);
      return true;
    }

    public bool proctask()
    {
      // taskを作成し、delegateを実行する
      string taskid = STasks.createTask(_queueloop);
      return true;
    }

    private void _queueloop()
    {
      if (evt_sharememrev == null)
      {
        return;
      }
      while (true)
      {
        bool ret = Semaphores.waitone(sem_sharemewrite, Semaphores.enum_SemaphoreWait.NoLimit);
        if (ret == false)
        {
          Utils.sleep(WAIT_TaskSleep);
          continue;
        }
        ret = Semaphores.waitone(sem_sharemem);
        if (ret == false)
        {
          Utils.sleep(WAIT_TaskSleep);
          continue;
        }
        evt_sharememrev();
        // sem_lockqueueは常にrelease状態になる
        Semaphores.release(sem_sharemem);
        // 再びserver sem queue waitをロック状態にし、待機するようにしておく
        // ここがおかしい気がするな -> waitoneでいいの？
        // どこでsemをlock状態にするのか？
        // Semaphores.waitone(sem_addqueue);

      }
    }

    public void Dispose()
    {
      _memstream?.Dispose();
      _mmf?.Dispose();
    }

    public bool getreader(out BinaryReader memstream)
    {
      memstream = null;
      bool ret = Semaphores.waitone(sem_sharemem);
      if (ret == false)
      {
        return ret;
      }
      if (_mmf == null)
      {
        return false;
      }
      _memstream = _mmf.CreateViewStream();
      memstream = new BinaryReader(_memstream);
      return true;
    }

    public bool close(ref BinaryReader memstream)
    {
      Semaphores.release(sem_sharemem);
      memstream?.Close();
      memstream.Close();
      return true;
    }



    public bool getwriter(out BinaryWriter memstream)
    {
      memstream = null;
      bool ret = Semaphores.waitone(sem_sharemem );
      if (ret == false)
      {
        return ret;
      }
      if (_mmf == null)
      {
        return false;
      }
      _memstream = _mmf.CreateViewStream();
      memstream = new BinaryWriter(_memstream);
      return true;

    }

    public bool close(ref BinaryWriter memstream)
    {
      Semaphores.release(sem_sharemem);
      memstream?.Close();
      memstream.Close();
      return true;
    }


  }
}
