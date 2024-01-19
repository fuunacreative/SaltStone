using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.IO;

namespace saltstone
{
  class Aviutl
  {

    public const string PROTOTYPE_AUP = "prototype.aup";

    #region dll define
    [DllImport("User32.dll", EntryPoint = "SendMessage")]
    public static extern Int32 SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);
    // public static extern int SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

    [DllImport("User32.dll", EntryPoint = "PostMessage")]
    public static extern bool PostMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

    [DllImport("user32.dll", SetLastError = true)]
    static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
    #endregion

    public const int WM_COMMAND = 0x0111;
    public const int WM_USER_NEWPROJ = 0x0465;
    public const int WM_USER_NEWPROJ2 = 0x0470;
    public const int BM_CLICK = 0x00F5;
    public const int MP4FILE_ADDRESS = 0x12C; // -> 0x22F
    public const int MP4FILE_LENGTH = 0x22f - 0x12c;

    public string projectdir;
    public string aupfile;
    public string senariofile;
    public string mp4file;
    public string voicedir; // wav,txtを保存する場所

    public Aviutl(string argsenariofile)
    {
      senariofile = argsenariofile;
      string buff = Utils.Files.getfilename(senariofile);
      string ext = Utils.Files.getextention(senariofile);
      string psubdir = Utils.Files.getbasename(buff);
      string basename = Utils.Files.getbasename(buff);
      projectdir = Utils.Files.getfilepath(senariofile);
      // projectdir part18\part18\になる
      // aupファイルがある場所を基準とする
      // buff = projectdir 

      buff = projectdir + "\\" + basename + ".aup";
      if (Utils.Files.exist(buff, Utils.Files.filesearchmode.fileonly) == true)
      {
        aupfile = buff;
        // projectdirはaupファイルが存在する場所
        voicedir = projectdir + "\\voice";
        return;
      }

      
      projectdir = projectdir + "\\" + psubdir;
      voicedir = projectdir + "\\voice";

      // projectdirが存在しない場合、mkdirする
      if (Utils.Files.exist(projectdir) == false)
      {
        Utils.Files.mkdir(projectdir);
        // System.IO.Directory.CreateDirectory(projectdir);
        Utils.Files.mkdir(voicedir);
      }
      aupfile = projectdir + "\\" + psubdir + ".aup";
      // TODO voiceは変更できるようにする
    }

    public bool existaup()
    {
      return Utils.Files.exist(aupfile, Utils.Files.filesearchmode.fileonly);
    }

    public static void sendtest()
    {
      IntPtr hwnd = new IntPtr(0x00170D70);
      //msg = 
      // int32だから、４バイトのはず
      // spyでみると８バイトになってる
      // 候補
      // WM_COMMAND wparam 0x00000406 lparam 0xffffffff
      // wnotify 0x0000 wid 0x0405 hwndctl 0xffffffff
      // aviが32bitだから、４バイトのはずだ
      // 何が問題かというと、spy++で取得したwm_commandのパラメータは８バイト
      // なのに、sendmessageのdefでは４バイトしか遅れないってことだ
      hwnd = FindWindow("AviUtl", "拡張編集");

      IntPtr wparam = new IntPtr(0x00000406);
      IntPtr lparam = new IntPtr(0xffffffff);

      // Int64 wparam = 0x00000406;
      // Int64 lparam = 0xffffffff;

      // int ret = SendMessage(hwnd,WM_COMMAND, wparam, lparam);
      bool ret = PostMessage(hwnd, WM_COMMAND, wparam, lparam);
      // ダイアログなのかな？ dlgが閉じるまでabendする
      // sendmessageではwaitする postmessageだとasyncになるはずだがうまくいかいない
      // dllのsignatureの問題だった


      // wparam = 0;
      // lparam = 0;
      wparam = new IntPtr(0);
      lparam = new IntPtr(0);
      //ret = SendMessage(hwnd, WM_USER_NEWPROJ, wparam, lparam);
      // ret = PostMessage(hwnd, WM_USER_NEWPROJ, wparam, lparam);
      // ret = PostMessage(hwnd, WM_USER_NEWPROJ2, wparam, lparam);

      // このメッセージを送ってもだめ
      // btn_clickをしないとだめっぽいな
      // btnのhwndをfindしないとだめ

      Utils.sleep(500);

      // IntPtr dlghwnd = FindWindowEx(IntPtr.Zero, IntPtr.Zero, "#32770", "新規プロジェクト");
      // IntPtr dlghwnd = FindWindowEx(IntPtr.Zero, IntPtr.Zero, "", "新規プロジェクト");
      hwnd = FindWindow("#32770", "新規プロジェクトの作成");
      // ウィンドウ名は*は使えない 完全一致する必要がある
      if (hwnd == IntPtr.Zero)
      {
        // logs.write("新規プロジェクトダイアログのhwnd取得に失敗しました")
        return;
      }


      IntPtr btnhwnd = FindWindowEx(hwnd, IntPtr.Zero, "button", "OK");
      if (hwnd == IntPtr.Zero)
      {
        // logs.write("OKボタンのhwnd取得に失敗しました")
        return;
      }
      PostMessage(btnhwnd, BM_CLICK, IntPtr.Zero, IntPtr.Zero);



      // hwnd = findwindowex(hwnd,IntPtr.Zero,"button","OK")

    }

    public bool copyprototype()
    {
      // aviu plugin側でprojectdirを保存しておく?
      // こちら側でもproject dirは判定できる
      // prototype.aupをコピーしたフォルダ

      // 現在のprojectdirを取得
      string src = Utils.getexecdir() + "\\" + PROTOTYPE_AUP;


      // 存在する場合は上書き
      System.IO.File.Copy(src, aupfile, true);


      //string dst = Globals.envini[PGInifile.INI_Aupdir];

      return true;
    }

    public bool setmp4file()
    {
      if (aupfile.Length == 0)
      {
        return false;
      }
      // binaryでaupファイルを編集し、mp4ファイルを設定する
      // int a = Aviutl.MP4FILE_ADDRESS;
      // 一度tmp.aupへrename -> binaryreaderで読み込み -> mp4部分を変更 -> binarywriterで書き込み
      string tempfile = projectdir + "\\temp.aup";
      mp4file = projectdir + "\\" + Utils.Files.getbasename(aupfile) + ".mp4";
      // fixed byte outbuff[10];
      MemoryStream outbuffstream = null;
      BinaryWriter outbuff = null;
      byte[] mp4binary;

      try
      {
        outbuffstream = new MemoryStream(MP4FILE_LENGTH);
        outbuff = new BinaryWriter(outbuffstream);
        outbuff.Write(mp4file.ToArray());
        // long len = MP4FILE_LENGTH - outbuffstream.Length;
        outbuffstream.Position = MP4FILE_LENGTH;
        outbuff.Write(((char)'\0'));
        mp4binary = outbuffstream.ToArray();
      }
      finally
      {
        if (outbuffstream != null)
        {
          outbuffstream.Close();
          outbuffstream.Dispose();
          outbuffstream = null;
        }
        if (outbuff != null)
        {
          outbuff.Close();
          outbuff.Dispose();
          outbuff = null;
        }
      }
      File.Delete(tempfile);
      File.Move(aupfile, tempfile);

      FileStream infs = null;
      BinaryReader inf = null;
      FileStream outfs = null;
      BinaryWriter outf = null;
      try
      {
        infs = new FileStream(tempfile, FileMode.Open);
        inf = new System.IO.BinaryReader(infs);
        outfs = new FileStream(aupfile, FileMode.Create);
        outf = new BinaryWriter(outfs);
        infs.Position = 0;
        byte[] header = inf.ReadBytes(MP4FILE_ADDRESS);
        outf.Write(header);
        outf.Write(mp4binary);
        infs.Position = MP4FILE_ADDRESS + MP4FILE_LENGTH + 1;
        infs.CopyTo(outfs);
      }
      finally
      {
        #region file close

        if (infs != null)
        {
          infs.Close();
          infs.Dispose();
          infs = null;

        }
        if (inf != null)
        {
          inf.Close();
          inf.Dispose();
          inf = null;
        }
        if (outfs != null)
        {
          outfs.Close();
          outfs.Dispose();
          outfs = null;
        }
        if (outf != null)
        {
          outf.Close();
          outf.Dispose();
          outf = null;
        }
        #endregion
      }
      System.IO.File.Delete(tempfile);


      return true;
    }

    public bool startupAviutl()
    {
      if (aupfile == null)
      {
        return false;
      }
      if (aupfile.Length == 0)
      {
        return false;
      }
      // TODO aviuのprocessが走っているか確認
      // 走っていればそこで開いているaupを確認
      // aupが同じであれば、それをactiveにする？
      if (Utils.checkrunexe("aviutl") == true)
      {
        return true; 
      }
      string cmd = Globals.envini[PGInifile.INI_Avipath];
      bool ret = Utils.runexec(cmd, aupfile);
      Utils.sleep(5000);
      // force parserの監視がいまいち
      return ret;
    }

  }

}
