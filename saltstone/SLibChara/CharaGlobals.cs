using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace saltstone
{
  public static class CharaGlobals
  {
    // charasはdisposeを使いたいためだけに staticにしていない
    public static saltstone.Charas _charas;
    public static Charas charas {
      get {
        if (charas == null)
        {
          _charas = new saltstone.Charas();
          STasks.createTask(_charas.init_background); // charasに必要な裏の処理はtask処理にまかせる

        }
        return _charas;

      }
    }

  }
}
