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

namespace Utils
{

  // internalにするのは、sqliteでも同じclassを使用しているため
  public static class Util
  {
    // open urlした時に、自分をbring to frontするための変数

    // public static IntPtr mainhWnd;
    // public static System.Diagnostics.Process browser;
    // public static IntPtr browserhWnd;

    public class Timer : IDisposable
    {
      // 内部でタイマーを保管し、終了時に一括して停止させる
      private static List<Timer> _timers;

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


      public static Timer getTimer()
      {
        Timer t = new Timer();

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
            Timer.disabletimer(this);
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
          foreach (Timer t in _timers)
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
          _timers = new List<Timer>();
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
        // TODO craete eventはfile create , file updateで作成する。一度のみとはmicrosoft documentでは保障されていない
        // このため、一度作成済みのものはskipするなどの処理が必要になる
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
        // TODO delegate eventをfireする

      }
      private Dictionary<string, long> createfiles;
      private Dictionary<string, long> createfiletimes;
      // 作成された時間のticks
      private Timer watchtimer;
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
        sleep(500);
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
            if (mainForm != null)
            {
              setWindowForground(mainForm);
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
      string buff = Files.getbasename(processname);
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

    private static Timer sleeptimer;

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
      sleep(100);
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
        sleep(500);
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

          sleep(10);
        }
        IntPtr activewinp = getForgroundWindow();
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
      string url = webconnecturl;
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
