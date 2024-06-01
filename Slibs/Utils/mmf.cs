using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.MemoryMappedFiles;

// memory mapped file
namespace Utils
{
  public static class SLMemoryMappedFile
  {
    private static Dictionary<string, MemoryMappedFile> _mmf;
    private static List<MemoryMappedViewStream> _mmfView;
    private static Dictionary<string, string> _semaphore;
    
    public static void init()
    {
      if (_mmf == null)
      {
        _mmf = new Dictionary<string, MemoryMappedFile>();
      }
      if (_semaphore == null)
      {
        _semaphore = new Dictionary<string, string>();
      }
      if (_mmfView == null)
      {
        _mmfView = new List<MemoryMappedViewStream>();
      }
    }
    public static void Dispose()
    {
      if (_mmf != null)
      {
        foreach (MemoryMappedFile m in _mmf.Values)
        {
          m.Dispose();
        }
      }
      _mmf.Clear();
      _mmf = null;
      if (_semaphore != null)
      {
        foreach (string s in _semaphore.Values)
        {
          Semaphores.release(s);
        }
      }
      _semaphore.Clear();
      _semaphore = null;
      if (_mmfView != null)
      {
        foreach (MemoryMappedViewStream v in _mmfView)
        {
          v.Dispose();
        }
      }
      _mmfView.Clear();
      _mmfView = null;

    }
    
    // create -> get guid
    // create view -> sem lock & get view
    // view write or read
    // releaseview(guid) -> sem lock release
    // finally delete view mmfを使用しなくなったとき
    // end dispose sem & mmf delete

    public static string create(long size = 1024)
    {
      string guid = Guid.NewGuid().ToString();
      MemoryMappedFile m = MemoryMappedFile.CreateNew(guid, size);
      _mmf.Add(guid, m);
      string sem = Semaphores.create();
      _semaphore.Add(guid, sem);
      return guid;
    }
    public static MemoryMappedViewStream getview(string guid)
    {
      MemoryMappedViewStream ms = _mmf[guid].CreateViewStream();
      _mmfView.Add(ms);
      bool ret = Semaphores.waitone(_semaphore[guid]);
      return ms;
    }
    public static bool releaseview(string guid)
    {
      bool ret = Semaphores.release(guid);
      return ret;
    }
    public static bool deleteview(MemoryMappedViewStream ms)
    {
      _mmfView.Remove(ms);
      return true;
    }

  }
}
