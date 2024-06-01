using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;
using static Utils.Util;

namespace Utils
{
  public class Sysinfo
  {
    public static string homedir = null;
    public static string downloaddir = null;
    public static string desktopdir = null;
    public static string videodir = null;
    /// <summary>
    /// log出力を行うディレクトリの定義
    /// 実行ファイル \ logsにするか、、、
    /// </summary>
    public static string logdir = null;

    /// <summary>
    /// logs.csで出力するlogfile name  不要では？
    /// </summary>
    // public static string logfile = null;

    // const string HOME_VIDEODIR = "Video";

    // TODO installed c++ runtime check
    // TODO installed pg check

    //public string getVideodir()
    //{
    //  return getHomedir() + "\\" + HOME_VIDEODIR;
    //}

    public static string getExeVersion()
    {
      string ver = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
      return ver;
    }

    // 現在実行中のexeファイルのfullpathを返す
    public static string getExeName()
    {
      //　string exefile = System.Reflection.Assembly.GetEntryAssembly().Location;
      // System.Reflection.Assembly exeobj = System.Reflection.Assembly.GetExecutingAssembly();
      // string exefile = exeobj.FullName;
      Process p = Process.GetCurrentProcess();
      string exefile = p.MainModule.FileName;
      return exefile;
    }

    public static string getCurrentExepath()
    {
      // dllの場合、dll名がきてしまう
      // string exefile = System.Reflection.Assembly.GetExecutingAssembly().GetName().FullName;
      string exefile = System.Reflection.Assembly.GetEntryAssembly().Location;

      // return Utils.Files.getfilename(exefile);
      return Utils.Files.getfilepath(exefile);
    }


    public static string getexepath(string processname)
    {
      string path = "";

      System.Diagnostics.Process[] procs = System.Diagnostics.Process.GetProcessesByName(processname);
      if (procs.Length == 0)
      {
        // this.Log("not run BouyomiChan");
        // iniに棒読みは不要 Yb_guiでしか使用しない
        // globalsには登録しておきたい
        //  execBouyomichan = false;
        return path;
      }
      System.Diagnostics.Process p = procs[0];
      path = p.Modules[0].FileName;

      return path;
    }




    // %HOME%\Downloadsのfullpathを取得する
    public static string getDownloaddir()
    {

      if (downloaddir != null)
      {
        return downloaddir;
      }
      // vmwareだとdownload先はshare folderになってしまう
      Guid guid_download = new Guid("374DE290-123F-4565-9164-39C4925E467B");

      // string downloaddir = Environment.GetFolderPath(Environment.SpecialFolder.down)
      string buff = "";
      IntPtr pPath = IntPtr.Zero;
      int hr = win32api.SHGetKnownFolderPath(guid_download, 0, IntPtr.Zero, out pPath);
      if (hr == 0)
      {
        buff = Marshal.PtrToStringUni(pPath);
      }
      if (buff.Substring(0, 1) == "\\")
      {
        // homeを元にdownload dirを決定
        string home = getHomedir();
        buff = home + "\\Downloads";

      }
      downloaddir = buff;
      return buff;
    }

    public static string getDesktopdir()
    {
      if (desktopdir != null)
      {
        return desktopdir;
      }
      desktopdir = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
      return desktopdir;

    }

    public static string getHomedir()
    {
      if (homedir != null)
      {
        return homedir;
      }
      homedir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
      // homedir = Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%");
      return homedir;
    }

    public static string getLogdir()
    {
      if (logdir != null)
      {
        return logdir;
      }
      string buff = Sysinfo.getCurrentExepath();
      buff += "\\logs";
      logdir = buff;
      Utils.Files.mkdir(logdir);
      return logdir;
    }

    public static string getTracefile()
    {
      string buff = getLogdir();
      buff += "\\" + getNowDatetime() + "_trace.txt";
      return buff;
    }

    public static string getLogfile()
    {
      // logfileとは何か？
      string buff = getLogdir();
      buff += "\\" + getNowDatetime() + ".txt";
      return buff;
    }

    public static Diskinfo getDiskUsageInfo(string driveletter = "C")
    {
      // inst先のディスクの全体容量を取得
      DriveInfo[] drives = DriveInfo.GetDrives();

      Diskinfo f = new Diskinfo();

      foreach (DriveInfo drive in drives)
      {
        if (drive.IsReady == false)
        {
          continue;
        }
        if (drive.Name.Contains(driveletter) == true)
        {
          f.totalsize = drive.TotalSize;
          f.freespace = drive.AvailableFreeSpace;
          f.usagepercent = ((f.totalsize - f.freespace) / f.totalsize) * 100;
          break;
        }
      }
      // G表示にするべきだよね
      f.totaosize_str = (f.totalsize / (1024 * 1024 * 1024)).ToString("F2");
      f.totaosize_str += "G";
      f.freespace_str = (f.freespace / (1024 * 1024 * 1024)).ToString("F2");
      f.freespace_str += "G";
      return f;
    }

    [SupportedOSPlatform("windows")]
    public static MemoryInfo getMemoryInfo()
    {
      MemoryInfo mem = new MemoryInfo();

      /*
      *PerformanceCounter pccounter = new PerformanceCounter("Memory", "Total Physical Memory");
      *float totalram = pccounter.NextValue();
      *mem.totalsize = totalram;
      // return (int)totalram;
      */
      // mem.totalsize = GC.GetTotalMemory(false);
      win32api.MEMORY_INFO mi = new win32api.MEMORY_INFO();
      mi.dwLength = (uint)System.Runtime.InteropServices.Marshal.SizeOf(mi);
      win32api.GlobalMemoryStatusEx(ref mi);
      mem.totalsize = mi.ullTotalPhys;

      PerformanceCounter pcounter = new PerformanceCounter();
      pcounter.CounterName = "% Committed Bytes In Use";
      pcounter.CategoryName = "Memory";
      float ramusage = pcounter.NextValue();
      mem.usagepercent = ramusage;

      mem.freespace = (mem.totalsize * ((100 - mem.usagepercent) / 100));
      mem.totalsize_str = (mem.totalsize / (1024 * 1024 * 1024)).ToString("F2");
      mem.totalsize_str += "G";
      mem.freespace_str = (mem.freespace / (1024 * 1024 * 1024)).ToString("F2");
      mem.freespace_str += "G";

      return mem;
    }

    [SupportedOSPlatform("windows")]
    public static bool regstryKeyExist(string arg)
    {
      // https://social.msdn.microsoft.com/Forums/azure/zh-CN/353210bd-02fd-4975-b431-3294439ab4d6/visual-c-redistributable?forum=AzureFunctions
      Microsoft.Win32.RegistryKey regkey = Microsoft.Win32.Registry.LocalMachine
        .OpenSubKey(@"SOFTWARE\Classes\Installer\Products\1af2a8da7e60d0b429d7e6453b3d0182");
      if (regkey == null)
      {
        return false;
      }
      return true;

      // HKEY_LOCAL_MACHINE\SOFTWARE\Classes\Installer\Products\1af2a8da7e60d0b429d7e6453b3d0182
    }
  }

}
