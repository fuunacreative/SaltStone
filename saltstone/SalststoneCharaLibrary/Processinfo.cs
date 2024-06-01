using System;
using System.Runtime.InteropServices;
using System.Diagnostics;
using Utils;



namespace saltstone
{
  public static class Processinfo
  {

    [DllImport("User32.DLL")]
    public static extern int SendMessage(IntPtr hWnd, UInt32 Msg, Int32 wParam, Int32 lParam);

    /*
    [DllImport("user32.dll")]
    private static extern Boolean ShowWindow(IntPtr hWnd, Int32 nCmdShow);
    */

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool GetWindowPlacement(
        IntPtr hWnd, ref struct_windowplacement lpwndpl);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    static extern bool IsIconic(IntPtr hWnd);



    public enum ShowWindowCommand
    {
      HIDE = 0,
      SHOWNORMAL = 1,
      SHOWMINIMIZED = 2,
      SHOWMAXIMIZED = 3,
    }
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    // public struct WINDOWPLACEMENT
    public struct struct_windowplacement
    {
      public int length;
      public int flags;
      public ShowWindowCommand showCmd;
      public System.Drawing.Point minPosition;
      public System.Drawing.Point maxPosition;
      public System.Drawing.Rectangle normalPosition;
    }


    public const Int32 WM_SYSCOMMAND = 0x112;
    public const Int32 SC_MINIMIZE = 0xF020;

    [DllImport("USER32.DLL")]
    public static extern bool SetForegroundWindow(IntPtr hWnd);

    private static System.Diagnostics.Process p;
    public static bool existProc(string arg)
    {
      if (p != null)
      {
        p.Dispose();
        p = null;
      }
      // System.Diagnostics.Process[] procs = System.Diagnostics.Process.GetProcessesByName(arg);
      System.Diagnostics.Process[] procs = getProcessByName(arg);
      if (procs.Length == 0)
      {
        return false;
      }
      p = procs[0];
      return true;
    }
    public static System.Diagnostics.Process[] getProcessByName(string arg)
    {
      return System.Diagnostics.Process.GetProcessesByName(arg);
    }

    public static bool brintToFrontProcess(string arg)
    {
      // bool ret = false;
      System.Diagnostics.Process[] procs = getProcessByName(arg);
      if (procs.Length == 0)
      {
        return false;
      }
      Process p = procs[0];
      IntPtr s = p.MainWindowHandle;
      SetForegroundWindow(s);
      return true;

    }



    public static string Procexename()
    {
      if (p == null)
      {
        return "";
      }
      string f = p.Modules[0].FileName;
      p.Dispose();
      p = null;
      return f;

    }

    public static void Dispose()
    {
      if (p != null)
      {
        p.Dispose();
        p = null;
      }

    }

    public static void execute(string arg)
    {
      if (System.IO.File.Exists(arg) == false)
      {
        return;
      }
      Logs.write(arg);
      System.Diagnostics.ProcessStartInfo p = new System.Diagnostics.ProcessStartInfo();
      p.FileName = arg;
      // かんしくんはうまくいくが、ぼうよみちゃんがおかしい
      // タスクバーに格納されない
      // 無理やりだが、exe名で場合分けする
      if (arg.Contains("force") == true)
      {
        p.WindowStyle = System.Diagnostics.ProcessWindowStyle.Minimized;
      }
      p.RedirectStandardError = false;
      p.RedirectStandardInput = false;
      p.RedirectStandardOutput = false;
      // p.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
      // p.UseShellExecute = false;
      System.Diagnostics.Process ps = System.Diagnostics.Process.Start(p);
      if (arg.Contains("Bouyom") == false)
      {
        return;
      }
      System.Threading.Thread.Sleep(2000);

      bool ret;
      ret = ps.WaitForInputIdle();
      /*
      if (ret == true)
      {
          break;
      }
      */

      int i = 0;
      // int reti;
      IntPtr hwnd = ps.MainWindowHandle;
      struct_windowplacement winstate = new struct_windowplacement();
      winstate.length = Marshal.SizeOf(winstate);
      do
      {
        System.Threading.Thread.Sleep(1000);
        if (hwnd == IntPtr.Zero)
        {
          hwnd = ps.MainWindowHandle;
          continue;
        }
        SendMessage(hwnd, WM_SYSCOMMAND, SC_MINIMIZE, 0);
        ret = IsIconic(hwnd);
        // ret = GetWindowPlacement(hwnd, ref winstate);
        // retを見る必要はないかも 必ずfalseがかえる

        if (ret == true)
        {
          break;
        }
        i++;
      } while (i < 50);
      System.Threading.Thread.Sleep(1000);
      // p.WindowStyle = System.Diagnostics.ProcessWindowStyle.Minimized;



      // 実行したぼうよみちゃんを最小化する
      // windowstyleではうまく動かないのでwinapiのpostmessageを使う
      // これでもうまく動かない
      //

      /*
      System.Diagnostics.Process.Start(arg);
      System.Threading.Thread.Sleep(50);
      */
    }




  }
}
