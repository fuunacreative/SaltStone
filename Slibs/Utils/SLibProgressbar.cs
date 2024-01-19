using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace saltstone
{
  public class SLibProgressbar
  {
    // public delegate void deleg_progres_update(int i);
    public delegate void deleg_Progressbar_update();
    public deleg_Progressbar_update func_progressbarupdate;
    public ProgressBar pBar;
    public System.Timers.Timer pTimer;
    public int barVal;

    //public ref int BarVal;

    [SupportedOSPlatform("windows")]
    public bool init(ToolStripProgressBar argctl)
    {
      bool fret = pInitTimer();

      pBar = argctl.ProgressBar;

      return true;
    
    }

    [SupportedOSPlatform("windows")]
    public bool init(ProgressBar argctl)
    {
      bool fret = pInitTimer();
      pBar = argctl;

      return true;

    }

    [SupportedOSPlatform("windows")]
    private bool pInitTimer()
    {
      pTimer = new System.Timers.Timer();
      pTimer.Enabled = false;
      pTimer.Interval = 100; // mil sec
      pTimer.AutoReset = true; // repeat after fire
      pTimer.Elapsed += evt_timer;
      return true;

    }


    [SupportedOSPlatform("windows")]
    public void evt_timer(object sender, System.Timers.ElapsedEventArgs e)
    {
      // exception throwされたときもtimerはうごいている

      int per = 0; // TODO perの計算処理
      // int per = inst.getDownloadPercent();
      // todo dir毎にPROGRESSPBARを表示 内包しているファイル名を元にPROGRESSBARを動かす
      // Console.WriteLine("tick raise" + per.ToString());
      // Utils.sleep(10);
      // ToolStripProgressBar b = stpProgressbar;
      // ProgressBar b = stpProgressbar.ProgressBar;
      if (pBar.InvokeRequired)
      {
        pBar.Invoke(new deleg_Progressbar_update(func_progressbarupdate), new object[] { per });
      } else
      {
        if (pBar == null)
        {
          return;
        }
        try
        {
          pBar.Value = per;
          pBar.Refresh();
        }
        catch (Exception ex)
        {
          string buff = ex.Message;
        }

      }
    }

    [SupportedOSPlatform("windows")]
    public void setBarval(int i)
    {
      // やっぱりうまく動かないや
      // downloadが終わってから急に動き出すかんじだよねー
      if (i < 0)
      {
        // -4000とかになったときがあった
        // 再現しないな-
        i = 0;
      }
      if (i > 100)
      {
        i = 100;
      }
      barVal = i;
      try
      {
        pBar.Value = barVal;
        pBar.Update();
        Utils.sleep(10);
      }
      catch (Exception e)
      {
        string buff = e.Message;
        buff = "";
      }
    }


  }
}
