using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.Net.Http;
using System.Runtime.Versioning;
using static System.ComponentModel.Design.ObjectSelectorEditor;

namespace saltstone
{

  // internalにするのは、sqliteでも同じclassを使用しているため
  public static class Utils
  {
    // open urlした時に、自分をbring to frontするための変数

    // public static IntPtr mainhWnd;
    // public static System.Diagnostics.Process browser;
    // public static IntPtr browserhWnd;

    public class Timer : IDisposable
    {
      // 内部でタイマーを保管し、終了時に一括して停止させる
      private static List<Utils.Timer> _timers;

      private System.Timers.Timer _timer;
      public delegate void del_evt_timer(object sender, System.Timers.ElapsedEventArgs e);
      public del_evt_timer evt_timer;
      public Timer()
      {
        init();
      }

      public Timer(int millsec, del_evt_timer eventfunc)
      {
        // TODO すぐにスタートする
      }

      ~Timer()
      {
        Close();
      }


      public static Utils.Timer getTimer()
      {
        Utils.Timer t = new Timer();

        return t;

      }
      public void Dispose()
      {
        // Close();
      }

      public bool Close()
      {
        try
        {
          if (_timers == null)
          {
            Utils.Timer.disabletimer(this);
            return true;
          }
          lock (_timers)
          {
            if (_timers?.Contains(this) == true)
            {
              _timers.Remove(this);
            }
          }

        }
        finally
        {
          _timers = null;
        }
        // Utils.Timer.disabletimer(this);
        return true;
      }

      private static void disabletimer(Timer tm)
      {
        try
        {
          System.Timers.Timer t = tm._timer;
          if (t != null)
          {
            t.Stop();
            t.Enabled = false;
            t.Dispose();
          }
        }
        catch (Exception e)
        {
          string buff = e.Message;
        }
        finally
        {
          if (tm._timer != null)
          {
            tm._timer = null;
          }
        }
      }

      public static void DisposeAll()
      {
        // lockしても_timersは変更される
        // form closした時にtimerがgcされるためと思われる
        // 内部でcoillectionをいじっていたためと思われる
        // clearをループの外に移動
        if (_timers == null)
        {
          return;
        }
        lock (_timers)
        {
          if (_timers == null)
          {
            return;
          }
          foreach (Utils.Timer t in _timers)
          {
            t.Close();
          }
          _timers.Clear();
          _timers = null;
        }
      }
      private void init()
      {
        _timer = new System.Timers.Timer();
        if (_timers == null)
        {
          _timers = new List<Utils.Timer>();
        }
        _timers.Add(this);
      }


      Timer(del_evt_timer evt)
      {
        init();
        // 引数で受け取ったevtとfuncをthreadを使って実行？

        // todo installerがbringtofrontするか？余計な処理を通っていないか？無駄なloopしてないか確認
        // todo site fail したら　continueするか？
        Timer t = getTimer();
        t.evt_timer += evt;
      }

      public enum enum_repeatTimer
      {
        repeat,
        norereat
      }

      public bool start(int milsec = 500, enum_repeatTimer rep = enum_repeatTimer.norereat)
      {
        // 0.5sを規定にする
        _timer.Interval = milsec;
        _timer.Elapsed += new System.Timers.ElapsedEventHandler(evt_timer);
        _timer.AutoReset = false;
        if (rep == enum_repeatTimer.repeat)
        {
          _timer.AutoReset = true;
        }
        _timer.Start();
        return true;
      }
      public bool stop()
      {
        _timer.Stop();
        _timer.Enabled = false;
        return true;
      }

    }

    public class directoryWatcher : IDisposable
    {
      // ディレクトリを監視し、指定された拡張子のファイルが作成されたら
      // size>0 , 正当なファイル？ , 1s待ってもサイズが変化しない
      // などの条件でファイルがdwできたと判断し、eventを発生させる
      // directorywatcher d  = new(path)
      // d.filter = "*.zip|*.psd";
      // d.event = evt_filecreated;
      // public string evt_filecreated(out filename){
      //  pf_addchar(filename)
      // }

      // 作られたファイルをどうやって監視側で受け取る？
      public delegate bool del_filecreated(string filename);
      public del_filecreated evt_filecreated;

      public string path;
      private string _fileextensions;
      private List<string> dic_fileextension;

      private List<FileSystemWatcher> watchers;

      // TODO timer をまとめるクラスを作る必要がある -> 確実に停止、破棄するため

      // constructor
      public directoryWatcher(string path)
      {
        dic_fileextension = new List<string>();
        this.path = path;
        watchers = new List<FileSystemWatcher>();
      }

      private FileSystemWatcher createWatcher()
      {
        FileSystemWatcher watcher = new FileSystemWatcher();
        watcher.Path = this.path;
        watcher.NotifyFilter = NotifyFilters.CreationTime
          | NotifyFilters.FileName
          | NotifyFilters.Size;
        watcher.Created += OnCreated;
        watcher.Changed += OnChanged;
        watchers.Add(watcher);
        return watcher;
      }

      public void Dispose()
      {
        if (watchers != null)
        {
          foreach (FileSystemWatcher f in watchers)
          {
            f.EnableRaisingEvents = false;
            f.Dispose();
          }
          watchers.Clear();
          watchers = null;
        }
        if (dic_fileextension != null)
        {
          dic_fileextension.Clear();
          dic_fileextension = null;
        }
      }

      public string fileextension
      {
        set
        {
          _fileextensions = value.ToLower();
          string[] list = value.Split('|');
          foreach (string s in list)
          {
            FileSystemWatcher w = createWatcher();
            w.Filter = s;
            // 複数フィルタを使う場合にはそれぞれにwatcherを作る必要がある
            dic_fileextension.Add(s);
          }
        }
      }


      public bool start()
      {
        foreach (FileSystemWatcher w in watchers)
        {
          w.EnableRaisingEvents = true;
        }
        return true;
      }

      public bool stop()
      {
        foreach (FileSystemWatcher w in watchers)
        {
          w.EnableRaisingEvents = false;
        }
        return true;
      }

      private void OnCreated(object sender, FileSystemEventArgs e)
      {
        string fname = e.FullPath;
        // filepathを候補に追加
        // size > 0 and 1sたってもサイズが変化しないを条件に delegate eventをraise
        // timerを作成 -> 1sたってもサイズが変化しない時はdelegate raise
        // change eventが発生したときはリセット

        // utils.files.file f = new file(fname);
        // if(f.size == 0) { return; }
        if (Utils.Files.getFileSize(fname) == 0)
        {
          return;
        }
      }
      private Dictionary<string, long> createfiles;
      private Dictionary<string, long> createfiletimes;
      // 作成された時間のticks
      private Utils.Timer watchtimer;
      private void OnChanged(object sender, FileSystemEventArgs e)
      {
        if (e.ChangeType != WatcherChangeTypes.Changed)
        {
          // The change of a file or folder.
          // not The types of changes include: changes to size, attributes, security settings, last write, and last access time.
          return;
        }
        // Console.WriteLine($"Changed: {e.FullPath}");
        string fname = e.FullPath;
        // file sizeが変更されたら、これが呼び出されると思う
        long size = Utils.Files.getFileSize(fname);
        if (size == 0)
        {
          return;
        }
        // file sizeを登録 -> 1s停止 -> ファイルサイズに変化がなければcreatedしたといしてevent rasise
        if (createfiles == null)
        {
          createfiles = new Dictionary<string, long>();
        }
        if (createfiletimes == null)
        {
          createfiletimes = new Dictionary<string, long>();
        }
        createfiles.Add(fname, size);
        createfiletimes.Add(fname, new FileInfo(fname).CreationTime.Ticks);
        if (watchtimer == null)
        {
          watchtimer = new Timer();
          watchtimer.evt_timer += evt_Sizecheck;
          watchtimer.start(200, Timer.enum_repeatTimer.repeat);
        }
      }
      private void evt_Sizecheck(object sender, EventArgs e)
      {
        List<string> createdfiles = new List<string>();
        if (evt_filecreated == null)
        {
          return;
        }
        if (createfiles == null)
        {
          return;
        }
        foreach (KeyValuePair<string, long> kv in createfiles)
        {
          string fname = kv.Key;
          if (Utils.Files.exist(fname) == false)
          {
            continue;
          }
          long size = kv.Value;
          long cursize = Utils.Files.getFileSize(fname);
          if (size != cursize)
          {
            createfiles[fname] = cursize;
            continue;
          }
          long curtick = DateTime.Now.Ticks;
          long createtick = createfiletimes[fname];
          if ((curtick - createtick) > 1000)
          {
            // これでfile毎にサイズ変更がなくて何秒たったかが判断できるはず
            evt_filecreated(fname);
            // ここでceatefilesをクリアしてるからおかしくなるんだ
            createdfiles.Add(fname);
          }
          createfiletimes[fname] = curtick; // 現在のsを代入し、サイズ変更監視間隔をリセットする

        }
        foreach (string f in createdfiles)
        {
          createfiles.Remove(f);
          createfiletimes.Remove(f);
        }
      }


    }


    public class binaryFile
    {
      public static BinaryReader getReader(string fname)
      {
        BinaryReader bst = new BinaryReader(File.OpenRead(fname));
        return bst;
      }
      public static byte[] getByteData(string fname, int length, int offset = 0)
      {
        byte[] retbyte = { };
        BinaryReader b = getReader(fname);
        if (offset > 0)
        {
          for (int i = 0; i < offset; i++)
          {
            byte buff = b.ReadByte();
          }
        }
        for (int j = 0; j < length; j++)
        {
          retbyte.Append<byte>(b.ReadByte());
        }

        return retbyte;
      }
    }

    public class win32api
    {
      [DllImport("User32.dll")]
      public static extern Int32 SetForegroundWindow(int hWnd);

      [DllImport("shell32.dll", CharSet = CharSet.Auto, SetLastError = true)]
      public static extern int SHGetKnownFolderPath(
        [MarshalAs(UnmanagedType.LPStruct)] Guid rfid,
        uint dwFlags, IntPtr hToken, out IntPtr pszPath);

      private const int SW_MINIMIZE = 6; // for showwindow
      private const int SW_SHOW = 5; // for showwindow

      [DllImport("user32.dll")]
      [return: MarshalAs(UnmanagedType.Bool)]
      public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

      [DllImport("user32.dll")]
      public static extern IntPtr GetForegroundWindow();


      [DllImport("user32.dll")]
      public static extern bool AllowSetForegroundWindow(int procID);

      [DllImport("kernel32.dll", EntryPoint = "CopyMemory", SetLastError = false)]
      public static extern void CopyMemory(IntPtr dest, IntPtr src, uint count);

      [StructLayout(LayoutKind.Sequential)]
      public struct MEMORY_INFO
      {
        public uint dwLength; //Current structure size
        public uint dwMemoryLoad; //Current memory utilization
        public ulong ullTotalPhys; //Total physical memory size
        public ulong ullAvailPhys; //Available physical memory size
        public ulong ullTotalPageFile; //Total Exchange File Size
        public ulong ullAvailPageFile; //Total Exchange File Size
        public ulong ullTotalVirtual; //Total virtual memory size
        public ulong ullAvailVirtual; //Available virtual memory size
        public ulong ullAvailExtendedVirtual; //Keep this value always zero
      }

      [DllImport("kernel32.dll")]
      [return: MarshalAs(UnmanagedType.Bool)]
      public static extern bool GlobalMemoryStatusEx(ref MEMORY_INFO mi);

      [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
      public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, Int32 w, Int32 l);

      public class WMMessage
      {
        public const int BM_SETBARCOLOR = 0x0410;
        public const int BM_SETBARCOLOR_Normal = 1;
        public const int BM_SETBARCOLOR_Warning = 3;
        public const int BM_SETBARCOLOR_Error = 2;
      }
    }

    [SupportedOSPlatform("windows")]
    public static void setProgressbarColor(ToolStripProgressBar b, int value)
    {
      setProgressbarColor(b.ProgressBar, value);
    }

    [SupportedOSPlatform("windows")]
    public static void setProgressbarColor(ProgressBar b, int value)
    {
      if (b == null)
      {
        return;
      }
      if (value < 0)
      {
        value = 0;
      }
      if (value > 100)
      {
        value = 100;
      }
      int color = win32api.WMMessage.BM_SETBARCOLOR_Normal;
      if (value > 50)
      {
        color = win32api.WMMessage.BM_SETBARCOLOR_Warning;
      }
      if (value > 90)
      {
        color = win32api.WMMessage.BM_SETBARCOLOR_Error;
      }
      win32api.SendMessage(b.Handle, win32api.WMMessage.BM_SETBARCOLOR, color, 0);
      sleep(50);
      b.Value = value;
      // progressBar1.CreateGraphics().DrawString(i.ToString() + "%", new Font("Arial",
      //  (float)10.25, FontStyle.Bold),
      //  Brushes.Red, new PointF(progressBar1.Width / 2 - 10, progressBar1.Height / 2 - 7));
      // }
      // statusbarの右端によせる
      // たぶん、panelをつくり、controlをのせ、marginをいじればいい
      // custom controlのほうがいいかもしれない

    }

    public class SLMemory
    {
      private static List<IntPtr> memallocs;

      public static bool copy(IntPtr src, IntPtr dest, int count)
      {
        if (count <= 0)
        {
          return false;
        }
        win32api.CopyMemory(dest, src, (uint)count);
        return true;
      }

      public static void Dispose()
      {
        if (memallocs != null)
        {
          foreach (IntPtr p in memallocs)
          {
            Marshal.FreeHGlobal(p);
            memallocs.Remove(p);
          }
          memallocs.Clear();
          memallocs = null;
        }
      }

      public static IntPtr alloc(int size)
      {
        IntPtr p = Marshal.AllocHGlobal(size);
        if (memallocs == null)
        {
          memallocs = new List<IntPtr>();
        }
        if (memallocs.Contains(p) == false)
        {
          memallocs.Add(p);
        }
        return p;
      }
      public static bool free(IntPtr p)
      {
        if (p == IntPtr.Zero)
        {
          return true;
        }
        Marshal.FreeHGlobal(p);
        if (memallocs.Contains(p) == true)
        {
          memallocs.Remove(p);
        }
        return true;

      }
    }






    public const string webconnecturl = "http://www.google.co.jp";
    public const int INT_NOPARSE = -1;

    public static Form mainForm = null;



    // private const int SW_MINIMIZE = 6;



    public static bool exeshellcmd(string cmd, string arg)
    {
      // TODO cmd windowがhideしない 一瞬表示される
      Process p = new Process();
      ProcessStartInfo pst = new ProcessStartInfo();
      pst.WindowStyle = ProcessWindowStyle.Hidden;
      // pst.WindowStyle = ProcessWindowStyle.Normal;
      pst.FileName = cmd;
      pst.Arguments = arg;
      // pst.RedirectStandardError = true;
      // pst.RedirectStandardOutput = true;
      pst.UseShellExecute = true;
      // pst.CreateNoWindow = false;
      pst.CreateNoWindow = true;
      // trueにするには shellexecuteをfalseにしないといけない
      p.StartInfo = pst;
      bool ret = false;
      try
      {
        ret = p.Start();
      }
      catch (Exception e)
      {
        string buff = e.Message;
      }
      if (ret == false)
      {
        return false;
      }
      p.WaitForExit();
      return ret;
    }


    public enum enum_runmode
    {
      start_async,
      waitforexit
    }

    public enum enum_runwinfront
    {
      bringToFront,
      background
    }

    [SupportedOSPlatform("windows")]
    public static bool runexec(string processname, string options = "", enum_runmode runmode = enum_runmode.start_async, enum_runwinfront winmode = enum_runwinfront.background)
    {
      // programの起動
      // 起動している場合は停止 or なにもしない
      // aviutlを起動する場合を想定すると、
      // TODO 複数起動に対応できるか？
      bool runflag = false;
      try
      {
        Process[] procs;
        runflag = checkrunexec(processname, out procs);
        // System.Diagnostics.Process[] procs = System.Diagnostics.Process.GetProcessesByName(processname);
        if (runflag == true)
        {
          return false;
        }
        Process ret = Process.Start(processname, options);
        if (ret == null)
        {
          return false;
        }
        if (runmode == enum_runmode.start_async)
        {
          return true;
        }
        // bring fontが動かないときがあったのを調整 また調整が必要かも
        Utils.sleep(500);
        // sleep(500);

        if (winmode == enum_runwinfront.bringToFront)
        {
          bool retflg = ret.WaitForInputIdle();
          int i = 0;
          IntPtr hwnd = getForgroundWindow();
          do
          {
            if (hwnd == ret.MainWindowHandle)
            {
              break;
            }
            if (Utils.mainForm != null)
            {
              setWindowForground(Utils.mainForm);
            }
            else
            {
              setWindowForground(ret.MainWindowHandle);
            }
            sleep(100);
            i++;
          } while (retflg == false && i < 50);
        }
        sleep(500);
        // sleep(500);
        if (runmode == enum_runmode.waitforexit)
        {
          ret.WaitForExit();
        }
      }
      catch (Exception e)
      {
        string buff = e.Message;
      }
      return runflag;
    }

    public static int checkrunexec(string processname)
    {
      int cnt = 0;
      Process[] procs;
      bool ret = checkrunexec(processname, out procs);
      if (ret == false)
      {
        return cnt;
      }
      cnt = procs.Length;
      return cnt;
    }

    public static bool checkrunexec(string processname, out Process[] procs)
    {
      bool runflag = false;
      string buff = Utils.Files.getbasename(processname);
      procs = System.Diagnostics.Process.GetProcessesByName(buff);
      if (procs.Length != 0)
      {
        runflag = true;
      }
      return runflag;

    }

    public static bool checkrunexe(string processname)
    {
      bool runflag = false;
      int i;

      i = processname.IndexOf(".");
      if (i > 0)
      {
        processname = processname.Substring(0, i);

      }

      Process[] procs = System.Diagnostics.Process.GetProcessesByName(processname);
      if (procs.Length != 0)
      {
        runflag = true;
      }
      return runflag;

    }





    // ファイル・ディレクトリの存在チェック
    public static string readAllText(string filepath)
    {
      string charaset = "";

      using (var fs = File.OpenRead(filepath))
      {
        Ude.CharsetDetector detector = new Ude.CharsetDetector();
        detector.Feed(fs);
        detector.DataEnd();
        charaset = detector.Charset;
        detector = null;
      }
      // TODO debug for ude check charactor encoding
      Encoding e = System.Text.Encoding.GetEncoding(charaset);



      // encording 処理
      // FileInfo finfo = new FileInfo(filepath);
      // Encoding e;
      // using (Hnx8.ReadJEnc.FileReader f = new Hnx8.ReadJEnc.FileReader(finfo))
      // {
      // Hnx8.ReadJEnc.CharCode c = f.Read(finfo);
      // e = c.GetEncoding();
      // }      // finfo = null;
      return File.ReadAllText(filepath, e);
    }

    public static List<string> searchfile(string path, string filepattern)
    {
      if (Files.exist(path) == false)
      {
        return new List<string>(); // emptyのlistを返す
      }
      IEnumerable<string> files = Directory.EnumerateFiles(path, filepattern, SearchOption.TopDirectoryOnly);
      return files.ToList();

      //            return null;
    }

    // ファイルの存在チェック（ディレクトリを除く）
    /*
    public static bool existFile(string path)
    {
        return File.Exists(path);
    }
    */

    public static string getexecdir()
    {
      string buff = System.AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\');
      return buff;
    }

    private static Utils.Timer sleeptimer;

    [SupportedOSPlatform("windows")]
    public static void sleep(int millsec)
    {
      if (millsec == 0 || millsec < 0) return;
      if (sleeptimer != null)
      {
        sleeptimer.Close();
        sleeptimer = null;
        //internaltimer.Stop();
        //internaltimer.Enabled = false;
        //internaltimer.Dispose();
        //internaltimer = null;
      }
      sleeptimer = new Timer();
      // internaltimer = new System.Windows.Forms.Timer();

      // Console.WriteLine("start wait timer");
      //internaltimer.Interval = millsec;
      //internaltimer.Enabled = true;
      //internaltimer.Start();

      //internaltimer.Tick += (s, e) => {
      //  internaltimer.Enabled = false;
      //  internaltimer.Stop();
      //  internaltimer.Dispose();
      //  internaltimer = null;
      //  // Console.WriteLine("stop wait timer");
      //};
      sleeptimer.evt_timer += (s, e) =>
      {
        sleeptimer.stop();
        sleeptimer.Close();
        sleeptimer = null;
        //  internaltimer.Enabled = false;
        //  internaltimer.Stop();
        //  internaltimer.Dispose();
        //  internaltimer = null;
      };

      int i = 0;
      //while (internaltimer != null && internaltimer.Enabled && i < millsec)
      while (sleeptimer != null && i < millsec)
      {
        // tickが動かない？　なのでずっとうごきっぱになる
        // 指定時間の２倍以上待っても停止しないときは強制停止する
        Application.DoEvents();
        System.Threading.Thread.Sleep(1);
        i++;
      }
    }

    // sleepに使用するタイマー
    // private static System.Windows.Forms.Timer internaltimer;

    public static void Dispose()
    {

      //if (internaltimer != null)
      //{
      //  internaltimer.Stop();
      //  internaltimer.Enabled = false;
      //  internaltimer.Dispose();
      //  internaltimer = null;
      //}
      sleeptimer?.Dispose();
      Timer.DisposeAll();
      Files.Dispose();

      SLMemory.Dispose();

      _utf8 = null;

      // Utils.internaltimer
      // Timer.dispose();
    }


    public static string tostr(object arg)
    {
      string buff = "";
      if (arg == null)
      {
        return buff;
      }

      if (arg.GetType() == typeof(string))
      {
        return (string)arg;
      }
      if (arg.GetType() == typeof(int))
      {
        return arg.ToString();
      }
      Type t = arg.GetType();
      /*
      if (t == DBNull)
      {
        return buff;
      }
      */
      if (t == typeof(System.DBNull))
      {
        return "";
      }
      return buff;
    }

    public static int toint(object arg, int defret = INT_NOPARSE)
    {
      if (arg == null)
      {
        return defret;
      }
      bool ret;
      int i = defret;
      if (arg.GetType() == typeof(int))
      {
        return (int)arg;
      }
      if (arg.GetType() == typeof(string))
      {
        ret = int.TryParse((string)arg, out i);
        if (ret == false)
        {
          return defret;
        }
        return i;
      }
      Type t = arg.GetType();
      if (t == typeof(System.DBNull))
      {
        return i;
      }


      return i;
    }


    public enum enum_openurl_mode
    {
      normal,
      minimized
    }

    [SupportedOSPlatform("windows")]
    public static bool openURL(string arg, enum_openurl_mode mode = enum_openurl_mode.normal)
    {
      /*
      Process p = System.Diagnostics.Process.Start(arg);
      if (p == null)
      {
        return false;
      }
      */
      // ProcessStartInfo ps = new ProcessStartInfo();
      Process p = new System.Diagnostics.Process();
      p.StartInfo.FileName = arg;
      p.StartInfo.UseShellExecute = true;
      mode = enum_openurl_mode.normal;
      if (mode == enum_openurl_mode.minimized)
      {
        p.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;
      }
      p.Start();
      Utils.sleep(100);
      // Utils.browserhWnd = p.MainWindowHandle;
      /*
      bool ret = false;
      int i = 0;
      do
      {
        ret = p.WaitForInputIdle();
        sleep(100);
        i++;
      } while (ret == false && i < 10);
      // どうしてもurlが前にでてしまう？
      if (Utils.mainhWnd != IntPtr.Zero)
      {
        setWinForground(Utils.mainhWnd);
      }
      */
      // やっぱりだめだな
      // debugだとうまくいくけど、本番ではNG
      // 1. urlをhideしてしまう
      // 2. url processをwaitする -> ng


      return true;
    }




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
        string buff = Utils.Sysinfo.getCurrentExepath();
        buff += "\\logs";
        logdir = buff;
        Utils.Files.mkdir(logdir);
        return logdir;
      }

      public static string getTracefile()
      {
        string buff = getLogdir();
        buff += "\\" + Utils.getNowDatetime() + "_trace.txt";
        return buff;
      }

      public static string getLogfile()
      {
        // logfileとは何か？
        string buff = getLogdir();
        buff += "\\" + Utils.getNowDatetime() + ".txt";
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

    public enum enum_RegistryRoot
    {
      LocalMachine
    }

    public class Diskinfo
    {
      public double totalsize;
      public double usagepercent;
      public double freespace;
      public string totaosize_str;
      public string freespace_str;
    }

    public class MemoryInfo
    {
      public double totalsize;
      public double usagepercent;
      public double freespace;
      public string totalsize_str;
      public string freespace_str;
    }


    public class Files
    {
      public const int MAX_PATH = 260;

      public enum Filesearchmode
      {
        fileonly,
        directoryonly,
        all
      }
      public static string getlastdirectoryname(string path)
      {
        return new DirectoryInfo(System.IO.Path.GetDirectoryName(path)).Name;

      }

      public static string getfilepath(string file)
      {
        string buff;
        if (Files.exist(file, Utils.Files.Filesearchmode.fileonly) == true)
        {
          buff = System.IO.Path.GetDirectoryName(file);
        }
        else if (Files.exist(file, Utils.Files.Filesearchmode.directoryonly) == true)
        {
          buff = file;
        }
        else
        {
          buff = "";
        }
        return buff;
      }

      public static List<string> getdirectory(string path)
      {
        // ディレクトリの存在チェック
        if (Directory.Exists(path) == false)
        {
          // 空のリストを返す
          return new List<string>();
        }

        IEnumerable<string> e = Directory.EnumerateDirectories(path, "*", SearchOption.TopDirectoryOnly);
        return e.ToList();

      }


      /// <summary>
      /// 指定されたディレクトリ内のファイルを検索し、listで返す
      /// </summary>
      /// <param name="path"></param>
      /// <returns></returns>
      public static List<string> getFiles(string path)
      {
        if (Directory.Exists(path) == false)
        {
          return new List<string>();
        }
        string[] fsary = Directory.GetFiles(path);
        List<string> fs = new List<string>(fsary);
        return fs;
      }




      public static string getfilename(string path)
      {
        return Path.GetFileName(path);
      }


      public static string getbasename(string path)
      {
        return Path.GetFileNameWithoutExtension(path);
      }


      public static string getextention(string path)
      {
        // pathチェックはスキップする

        // if (fileexist(path) == false)
        // {
        //   return "";
        // }
        // \nが入っているとエラー
        string buff = path.Replace("\n", "");
        return Path.GetExtension(buff).ToLower();
        // string f = getfilename(path);

      }

      // 作成したtempファイル・dirを保存
      // app exit時に削除するため
      public static List<string> tempfiles;


      public static string getHash(string file)
      {
        // hashはファイルを全部走査して計算する
        // 大きいファイルの場合は時間がかかる
        // れいむ.zipで2.5M
        byte[] hashbyte;
        string ret = "";
        FileStream stream = null;
        SHA256 sha = null;
        try
        {
          stream = File.OpenRead(file);
          sha = SHA256.Create();
          hashbyte = sha.ComputeHash(stream);
          ret = Convert.ToBase64String(hashbyte);
        }
        finally
        {
          if (stream != null)
          {
            stream.Dispose();
            stream = null;
          }
          if (sha != null)
          {
            sha.Dispose();
            sha = null;
          }
        }
        return ret;
      }


      public static bool exist(string path, Filesearchmode mode = Filesearchmode.all)
      {
        // ディレクトリがある場合は失敗する
        // modeで切り替えできるようにしておく
        bool fret = File.Exists(path);
        if (mode == Filesearchmode.fileonly)
        {
          return fret;
        }
        bool dret = Directory.Exists(path);
        if (mode == Filesearchmode.directoryonly)
        {
          return dret;
        }
        return fret | dret;



      }

      public static bool existDirectory(string path)
      {
        return Directory.Exists(path);
      }

      public static long getFileSize(string file)
      {
        if (file.Length == 0)
        {
          return 0;
        }
        if (Files.exist(file) == false)
        {
          return 0;
        }
        FileInfo f = new FileInfo(file);
        return f.Length;
      }

      public static bool Copy(string source, string dest)
      {
        try
        {
          // overrite true
          System.IO.File.Copy(source, dest, true);
        }
        catch (Exception e)
        {
          string buff = e.Message;
          return false;
        }

        return true;

      }

      public enum enum_rsyncoverrite
      {
        overrite,
        nooverrite,
        showmessage

      }
      public enum enum_rsync_nomessage
      {
        nomessage,
        showmessage
      }

      [SupportedOSPlatform("windows")]
      public static bool rsync(string path, string dest, enum_rsyncoverrite option = enum_rsyncoverrite.showmessage)
      {
        // enum_rsyncoverrite opt = enum_rsyncoverrite.showmessage;
        enum_rsyncoverrite opt = option;
        bool ret = rsync(path, dest, ref opt);
        return true;
      }

      [SupportedOSPlatform("windows")]
      public static bool rsync(string path, string dest, ref enum_rsyncoverrite option)
      {
        // recursiveでディレクトリを含めコピー
        // 最新のものがあれば上書き
        // 上書き確認 messeageboxを表示

        // なぜか複数タスクとして別々に動作しているっぽいよなー


        // pathの正規化 /*は不要 ディレクトリ名であることを確認
        // destも同様
        // Get information about the source directory
        if (existDirectory(path) == false)
        {
          return false;
        }
        // destは同じ名前か？それとも親か？
        string srcbase = getbasename(path);
        string dstbase = getbasename(dest);
        string srcdir = path;
        string destdir = dest; // コピー先のdir 含むsrcのディレクトリ名前
        if (srcbase != dstbase)
        {
          destdir += "\\" + srcbase;
        }

        DirectoryInfo srcinfo = new DirectoryInfo(srcdir);

        // Check if the source directory exists

        // Cache directories before we start copying
        DirectoryInfo[] srcdirs = srcinfo.GetDirectories();

        string buff;
        // Create the destination directory
        Utils.Files.mkdir(destdir);
        foreach (DirectoryInfo d in srcdirs)
        {
          buff = destdir + "\\" + d.Name;
          Utils.Files.mkdir(buff);
        }

        bool overwrite = false;
        // 上書き確認のメッセージ表示のフラグ（初回のみ）
        bool showoverrite = true;
        bool force_no_overwrite = false;
        if (option == enum_rsyncoverrite.overrite)
        {
          overwrite = true;
          showoverrite = false;
        }
        if (option == enum_rsyncoverrite.nooverrite)
        {
          force_no_overwrite = true;
          showoverrite = false;
        }

        // Get the files in the source directory and copy to the destination directory
        foreach (FileInfo f in srcinfo.GetFiles())
        {
          string targetFilePath = Path.Combine(destdir, f.Name);
          // file timestampを比較 最新なら上書き
          if (exist(targetFilePath) == false)
          {
            f.CopyTo(targetFilePath);
            continue;
          }
          if (force_no_overwrite == true)
          {
            continue;
          }
          if (showoverrite == true)
          {
            // 
            overwrite = Utils.Message.inquiry("同名ファイルが存在します。新しい場合に上書きしますか？");
            if (overwrite == true)
            {
              showoverrite = false;
              option = enum_rsyncoverrite.overrite;

            }
            else
            {
              option = enum_rsyncoverrite.nooverrite;
            }
          }
          if (overwrite == false)
          {
            continue;
          }
          // 同名ファイルがあった場合、どう処理するか？
          // 上書きするか？　タイムスタンプを比較して更新するか？

          // 同じ名前のファイルが存在している
          // 日付をチェック 最新なら上書き？もしくは無条件上書き？
          // 確認する必要あるか？
          FileInfo newf = new FileInfo(targetFilePath);
          if (f.LastWriteTime < newf.LastWriteTime)
          {
            // overrite
            f.CopyTo(targetFilePath, true);
          }
        }

        bool ret = false;
        // If recursive and copying subdirectories, recursively call this method
        foreach (DirectoryInfo subDir in srcdirs)
        {
          string newDestinationDir = Path.Combine(destdir, subDir.Name);
          /*          enum_rsyncoverrite opt = enum_rsyncoverrite.overrite;
                    if (overwrite == false)
                    {
                      opt = enum_rsyncoverrite.nooverrite;
                    }
                    if (showoverrite == true)
                    {
                      opt = enum_rsyncoverrite.showmessage;
                    }
          */
          ret = rsync(subDir.FullName, newDestinationDir, ref option);
          if (ret == false)
          {
            return false;
          }
        }
        return true;
      }


      public static string createTemp()
      {
        string buff = Path.GetTempFileName();
        if (tempfiles == null)
        {
          tempfiles = new List<string>();
        }
        tempfiles.Add(buff);
        return buff;
      }

      public static void Dispose()
      {
        if (tempfiles == null)
        {
          return;
        }
        foreach (string s in tempfiles)
        {
          Files.delete(s);
          Files.rmdir(s);
        }
        Files.tempfiles.Clear();
        Files.tempfiles = null;
      }

      public static string createTempDirectory()
      {
        string fname = createTemp();
        Utils.Files.delete(fname);
        Utils.Files.mkdir(fname);
        return fname;
      }

      public static bool rename(string source, string dest)
      {
        // TODO destの存在チェック
        System.IO.File.Move(source, dest);
        return true;
      }

      public static bool rmdir(string path)
      {
        if (Files.exist(path, Filesearchmode.directoryonly) != true)
        {
          return false;
        }
        delete(path);
        DirectoryInfo dir = new DirectoryInfo(path);
        dir.Delete();
        return true;
      }

      public static bool delete(string path)
      {
        try
        {
          // TODO pathがディレクトリの場合、内部はdelするが、path自体は残る
          if (Files.exist(path, Filesearchmode.directoryonly) == true)
          {
            DirectoryInfo dir = new DirectoryInfo(path);
            foreach (FileInfo f in dir.GetFiles())
            {
              f.Delete();
            }
            foreach (DirectoryInfo d in dir.GetDirectories())
            {
              delete(d.FullName);
              try
              {
                d.Delete(true); // recursive
              }
              catch (Exception e)
              {
                string msg = e.Message;
              }
            }
            // dir.Delete(); // 元ディレクトリも削除する -> rmdirで呼び出し元から削除する
          }
          else if (Files.exist(path, Filesearchmode.fileonly) == true)
          {
            File.Delete(path);
          }

        }
        catch (Exception e)
        {
          string buff = e.Message;
          return false;
        }
        return true;
      }

      public static string getExeVersion(string exepath)
      {
        if (Utils.Files.exist(exepath) == false)
        {
          return "null";
        }
        System.Diagnostics.FileVersionInfo verinfo = FileVersionInfo.GetVersionInfo(exepath);
        string ver = verinfo.FileVersion;
        return ver;
      }

      public static bool mkdir(string argdir)
      {
        if (argdir == null)
        {
          return false;
        }
        // dirが存在する場合にはtrueをreturn
        // ない場合にはmkdirし、falseをreturn
        if (Files.exist(argdir, Filesearchmode.directoryonly) == true)
        {
          return true;
        }
        Directory.CreateDirectory(argdir);

        return false;
      }

    }

    [SupportedOSPlatform("windows")]
    public static bool setWindowForground(Form mainform)
    {
      if (mainform == null)
      {
        return false;
      }
      int i = 0;
      bool ret = false;
      do
      {
        int processid = System.Diagnostics.Process.GetCurrentProcess().Id;
        win32api.AllowSetForegroundWindow(processid);
        Utils.sleep(500);
        if (mainform != null)
        {
          // やはり動かない？
          // broserの前にでない
          mainform.BringToFront();
          mainform.Activate();
          mainform.Focus();
          setWindowForground(mainform.Handle);
          // setforgroundは動いているっぽいんだけど
          // browserのほうが優先される？

          Utils.sleep(10);
        }
        IntPtr activewinp = Utils.getForgroundWindow();
        if (activewinp == mainform.Handle)
        {
          ret = true;
          break;
        }
        if (i > 100)
        {
          break;
        }
        i++;
      } while (true);
      return ret;
    }

    public static bool setWindowForground(IntPtr arg)
    {
      int j = arg.ToInt32();
      win32api.SetForegroundWindow(j);

      return true;
    }

    public static bool isInternetConnect()
    {
      // bool ret = false;

      // System.Net.NetworkInformation.Ping p = new System.Net.NetworkInformation.Ping();
      // p.Send()
      // 環境によってはpingを通していない
      // httpが通ればよい
      // string url = SaltstoneSetup.Properties.Resources.webconnect;
      string url = Utils.webconnecturl;
      // System.Net.WebRequest req = System.Net.WebRequest.Create(url);
      HttpClient req = new HttpClient();
      using (Task<HttpResponseMessage> reqret = req.GetAsync(url))
      {
        if (reqret == null)
        {
          return false;
        }
        bool ret = reqret.Wait(5000); // 5s待機
        if (ret == false)
        {
          return false;
        }

        //System.Net.WebResponse rep = req.GetResponse();
        //if (rep == null)
        //{
        //  return false;
        //}
      }


      return true;
    }

    /*
    public static void CursorWait()
    {
      Cursor.Current = Cursors.WaitCursor;
      sleep(10);
    }

    public static void CursorDefault()
    {
      Cursor.Current = Cursors.Default;
      sleep(10);
    }
    */

    public static IntPtr getForgroundWindow()
    {
      return win32api.GetForegroundWindow();
    }




    public enum enum_SystemSound
    {
      message, // asterisk
      beep, // 一般の警告 beep
      deeperror, // Hand
      question, // 問い合わせ
      error // exclamation
    }

    public class Sound
    {

      [SupportedOSPlatform("windows")]
      public static void playSystemSound(enum_SystemSound arg = enum_SystemSound.message)
      {
        switch (arg)
        {
          case enum_SystemSound.message:
            System.Media.SystemSounds.Asterisk.Play();
            break;
          case enum_SystemSound.beep:
            System.Media.SystemSounds.Beep.Play();
            break;
          case enum_SystemSound.error:
            System.Media.SystemSounds.Exclamation.Play();
            break;
          case enum_SystemSound.deeperror:
            System.Media.SystemSounds.Hand.Play();
            break;
          case enum_SystemSound.question:
            System.Media.SystemSounds.Question.Play();
            break;
        }
      }

      [SupportedOSPlatform("windows")]
      public static bool play(string arg)
      {
        System.Media.SoundPlayer sound = new System.Media.SoundPlayer(arg);
        sound.Play();
        sound.Dispose();
        sound = null;
        return true;

      }

      [SupportedOSPlatform("windows")]
      public static bool play(Stream arg)
      {
        System.Media.SoundPlayer sound = new System.Media.SoundPlayer(arg);
        sound.Play();
        sound.Dispose();
        sound = null;
        return true;

      }

    }

    [SupportedOSPlatform("windows")]
    public class Message
    {
      public static bool showInfo(string message)
      {
        DialogResult ret = System.Windows.Forms.MessageBox.Show(message);
        return true;
      }

      public static bool inquiry(string message)
      {
        DialogResult ret = System.Windows.Forms.MessageBox.Show(message, "問い合わせ", MessageBoxButtons.YesNo);
        if (ret == DialogResult.No)
        {
          return false;
        }
        return true;
      }
    }

    public static string getNowDatetime()
    {
      return DateTime.Now.ToString("yyyyMMdd_HHmmss");
    }

    [SupportedOSPlatform("windows")]
    public static class mouseCursor
    {
      public static void wait()
      {
        Cursor.Current = Cursors.WaitCursor;
      }
      public static void useWait()
      {
        Application.UseWaitCursor = true;
      }
      public static void clear()
      {
        Application.UseWaitCursor = false;
      }
    }
    public static class SGuid
    {
      public static string get()
      {
        return Guid.NewGuid().ToString();
      }
      public static string getShort()
      {
        return GenerateRandomId(4);
      }

      private static string GenerateRandomId(int length = 5)
      {
        string charset = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        char[] outputChars = new char[length];

        RandomNumberGenerator rng = null;
        try
        {
          rng = RandomNumberGenerator.Create();
          int minIndex = 0;
          int maxIndexExclusive = charset.Length;
          int diff = maxIndexExclusive - minIndex;

          long upperBound = uint.MaxValue / diff * diff;

          byte[] randomBuffer = new byte[sizeof(int)];

          for (int i = 0; i < outputChars.Length; i++)
          {
            // Generate a fair, random number between minIndex and maxIndex
            uint randomUInt;
            do
            {
              rng.GetBytes(randomBuffer);
              randomUInt = BitConverter.ToUInt32(randomBuffer, 0);
            }
            while (randomUInt >= upperBound);
            int charIndex = (int)(randomUInt % diff);

            // Set output character based on random index
            outputChars[i] = charset[charIndex];
          }
        }
        finally
        {
          rng?.Dispose();
        }

        return new string(outputChars);
      }
    }

    public static Encoding _utf8;

    public static Encoding getEncodingUTF8()
    {
      if (_utf8 == null)
      {
        _utf8 = System.Text.Encoding.UTF8;
      }
      return _utf8;
    }
  }




}
