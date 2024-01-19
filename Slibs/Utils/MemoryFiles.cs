using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Versioning;

// これもIPCじゃないからたぶん不要 dllならね
// class pointerをc++ funcに渡せばいいだけ

// memorymappedfileを統合管理するもの
// すべてのmemorymappedfileを管理し、disposeする
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

    [SupportedOSPlatform("windows")]
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




}
