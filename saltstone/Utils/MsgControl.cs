﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace saltstone
{
  public class MsgControl
  {
    private ToolStripStatusLabel _label;
    private ToolStripProgressBar _progressbar;
    private ToolStrip _toolstrip;

    // GUIへのメッセージ表示コントロールの登録
    [SupportedOSPlatform("windows")]
    public MsgControl(ToolStripStatusLabel lblmsg, ToolStripProgressBar pgbar)
    {
      _label = lblmsg;
      _progressbar = pgbar;
      // invokeがtoolstripに対してしかできないため、toolstripの参照を保存しておく
      _toolstrip = lblmsg.GetCurrentParent();
    }

    [SupportedOSPlatform("windows")]
    public bool checkformclosing()
    {
      if (_label.GetCurrentParent().FindForm().IsDisposed == true)
      {
        return true;
      }

      return false;
    }

    [SupportedOSPlatform("windows")]
    public void showMessage(string mes)
    {
      if (_toolstrip.InvokeRequired == true)
      {
        _toolstrip.BeginInvoke((MethodInvoker)(() => {
          _label.Text = mes;
          // Utils.setProgressbarColor(this.pbDisk, diskvalue);
          // Utils.setProgressbarColor(this.pbMemory, memvalue);
        }));
        return;
      }
      _label.Text = mes;
    }

    [SupportedOSPlatform("windows")]
    public void setProgress(int val)
    {
      if (val < 0)
      {
        val = 0;
      }
      if (val > 100)
      {
        val = 100;
      }
      if (_toolstrip.InvokeRequired == true)
      {
        _toolstrip.BeginInvoke((MethodInvoker)(() => {
          _progressbar.Value = val;
        }));
        return;
      }
    }



  }

}
