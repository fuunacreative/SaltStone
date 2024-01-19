using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace saltstone
{
  public class InstallMethod
  {
    public enum enum_cmd
    {
      exe,
      filecopy
    }

    public enum_cmd cmd;
    public string exefile;
    public string source; // file or directory
    public string destdir; // directory
  }
  // exe実行かファイルコピー

}
