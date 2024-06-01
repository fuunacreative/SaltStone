using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Windows.Forms;
using Utils;
// using System.Windows.Forms;
// using System.Data.SQLite;
// globalsにsqliteいるか？
// いるな。ybcharaから参照する

// 個別にsqlite用のiniファイルを書くべきだが、、、
// Globalsの各種設定をどうやって読み込むか
// いずれにせよ、public statcは必要になる
// envクラスを作成、ここにすべての設定を集約する
// ということは、ybchara用のenvクラスを作成する必要がある

// ybcharaとyb_guiの両方で使用するものはGlobalsで定義する


namespace saltstone
{



  public partial class Globals
  {
    // memberのほぼすべてがstaticなので、disposeは実装しなくていい

    #region enum
    public enum Filesavemode
    {
      nosave, save
    } // 棒読みでファイル保存を行うかどうか

    public enum Charapicturetype
    {
      PSD, DIR
    } // 立ち絵がpsdtkか通常のディレクトリ素材か
    public enum Makemode
    {
      noplay, withplay
    } // 声を出しながら挿入するかどうか
    public enum ePlaywait
    {
      wait, nowait
    }

    #endregion

    #region const
    public const string SettingDB = "settings.db";
    public const string DefaultDestfolder = "";
    public const string DefaultDestsubfolder = @"\voice";
    public const string table_directory = "directorys";
    public const string PIPENAME_AQprocess = "saltstonevoice_aq_x86";
    public const string SHAREMEMAME_AQprocess = "saltstonevoice_aq_x86_sharemem";

    #endregion

    // private static Globals g;
    // singleto用のグローバル変数
    // staticにする必要あるのか？
    // 別のクラスライブラリにしたほうがよいかもしれない
    // yb_guiとybcharaでしか使用しない


    // projectdirはgcmzのapiより取得するほうがいいかも
    // gcmzでmemory map fileにaupのデータを保存してある
    // そこからデータを取得したほうがよさげ fpsも取得できる
    // public const string DefaultDestfolder = @"F:\yukkuri_spoon\voice"; // これはaupの場所を元に演算
    // public const string DefaultDestfolder = @"C:\Users\yasuhiko\Documents\yukkuritest\voice";
    // public const string DefaultDestsubfolder = @"\voice";
    //public static string destfolder = ""; //voiceの保存フォルダ
    public static string voicefolder = ""; //voiceの保存フォルダ
    // 棒読みちゃんを使う場合 dllでは使用しないため、コンパイルエラーとなる
    // public static BouyomiChanClient boyomi = null;
    // public static FNF.Utility.BouyomiChanClient boyomi = null;

    // public static Dictionary<string, Voice> voicelist;
    public static Inifile envini = null; // 実行時ディレクトリにあるsettings.ini
                                         // public static Inifile aupini = null; // aupファイルと同じ場所にあるaup.ini // 未使用
    public static bool batchstop = false;

    public static Dictionary<string, string> globalsection;

    // logを出力するメインフォーム
    // public static System.Windows.Forms.Form frm;

    // ybcharaで使用する args
    public static string[] cmdargs;

    public static string tempdir;

    /// <summary>
    /// メインフォームのmsgcontrol  status bar / progress barなど
    /// </summary>
    public static MsgControl main_messagectl;

    // 立ち絵のタイプ psd or dir
    public static Charapicturetype charapicturetype;

    // public static Charas charas; // キャラの立ちえを管理するクラスのインスタンス
    // public static Charas charas; // キャラ定義の一括管理
    // public static CharaPortraits charapotrait; // 立ち絵の一括管理
    
    // TODO chardbが開きっぱなしになる どうする？
    public static DB.Sqlite charadb;

    public static List<Action> _dispose;

    // guiへ表示するためのクラス message control + pgbar
    // public static libraryMessage messagectl;
    // logsに統合


    // globalsにいれると、ybcharaでも使用することになる
    // ybchara側では解析処理がメイン、yb_gui側ではdbからの情報取得がメインになる



    // sqliteのオブジェクト
    // public static Sqlite db;
    // globalで持つ必要はない
    // つど、開いて利用する

    static Globals()
    {
      // init();
    }

    public static void init(ToolStripStatusLabel lblmes, ToolStripProgressBar pgbar)
    {
      Globals.init();
      // globalにmessagectlを追加すると、formひとつしか対応できない
      // メインformのmessagectlをglobalに設定しておき、ログ出力が行えるようにする
      main_messagectl = new MsgControl(lblmes, pgbar);
      // Logs.messagectl = new MsgControl

    }

    public static void init()
    {


      if (globalsection != null)
      {
        globalsection.Clear();
        globalsection = null;
      }
      GC.Collect();
      globalsection = new Dictionary<string, string>();

      if (envini == null)
      {
        PGInifile i = new PGInifile();
        // PGInifile i = new Inifile();
        i.load();
        //　ここでGlobals.enviniを設定している

      }


      // pg内部で使用するｄｂ、現在は区分（体->B）を管理している
      // これはiniファイルで指定されるべき

      // string pgdbfname = @"C:\Users\yasuhiko\program\yb.db";

      // envini.set("pgdb",pgdbfname);


     
      //  DB.Sqlite db = new DB.Sqlite(buff);

      // charapicturetype  = db.getcharapicturetype();
      // TODO psd or dirの設定をdbから取得?

      //if (Globals.envini[PGInifile.INI_Chardir] == "")
      //{
      //  // exitをdelegateするとか、メッセージ表示をdelegateするとかしないといけない

      //  System.Windows.Forms.MessageBox.Show("キャラクタディレクトリがsetting.iniで定義されていません");
      //  System.Windows.Forms.Application.Exit();
      //  return;
      //}


      // Charaクラスを作成
      // charas = new Charas();
      // STasks.createTask(charas.init_background); // charasに必要な裏の処理はtask処理にまかせる
      // ここでdbからキャラ情報を取得する
      // charapotrait = new CharaPortraits();


      // 各種directoryの設定
      // dbに保存されてるdirectory設定を読み込み globals.directoryに設定
      // slibappinfoに統合
      // Directory.init();

      // for test
      // Directory.charpicture = @"";

      
      Semaphores.init();
      STasks.init();

      // log serverの場合はwriterはopen不要
      // どうやって判断するか？
      // Logs.init();

      SLMemoryMappedFile.init();
      _dispose = new List<Action>();
      _dispose.Add(SLMemoryMappedFile.Dispose);

      // logmanager.exeが起動されていなければ起動する


      // 別プロセスでパースを行う
    }

    public static DB.Sqlite getSettingDB()
    {
      string buff = envini[PGInifile.INI_SettingDB];
      if (buff.Length == 0)
      {
        buff = Util.getexecdir() + "\\" + SettingDB;
        envini[PGInifile.INI_SettingDB] = buff;
      }
      DB.Sqlite db = new DB.Sqlite(buff);
      return db;

    }

    //    public static void clear()
    //    {
    //#if USEVOICE
    //      voicelist.Clear();
    //#endif
    //    }

    public static void Dispose()
    {
      if (tempdir != null && tempdir.Length > 0)
      {
        Utils.Files.delete(tempdir);
        Utils.Files.rmdir(tempdir);
      }
      //if (replacestr != null)
      //{
      //  replacestr.Clear();
      //  replacestr = null;
      //}

      if (charadb != null)
      {
        charadb.Dispose();
        charadb = null;
      }
      STasks.Dispose();
      if (_dispose != null)
      {
        foreach (Action a in _dispose)
        {
          a.Invoke();
        }
      }

      Logs.Dispose();
      Semaphores.Dispose();
      STasks.Dispose();

      // taskをdisposeしているのに、waitoneが走ったままになってる
    }


    /*
    public void showdlgmessage(string arg)
    {
        // MessageBox.Show(arg);
        return;
    }
    */

    public static bool loadpgini()
    {
      PGInifile pgini = new PGInifile();
      pgini.load();
      envini = pgini;
      // envini.load();
      return true;
    }

    // 問題は、pginiでexeの実行を促してパスを取得していいかどうか？
    // 別クラスにするべきか？それともこのまま使うか？別exeにするべきか？

    //public class Directory
    //{
    //  public const string DIR_CharaPicture_CACHE = "cache";
    //  // utilsからglobalsは呼び出せない -> 循環参照になるため
    //  // public const string DIR_Log = "log";
    //  // Saltstoneで使用するディレクトリを一括管理するクラス
    //  // 元データはwebより取得
    //  public static Dictionary<string, string> replacestr;

    //  // system系統
    //  public static string home;
    //  public static string tempdir;
    //  public static string download;

    //  public static string workspace;

    //  public static string exepath_aviutl;
    //  public static string program;
    //  public static string voicesoft;
    //  // public static string log;
    //  // aviutlのaupファイルなどの作業を行う %HOME%\Videos
    //  // 棒読みちゃんはVoicesクラスで管理
    //  // キャラ素材配下におかれ、高速化のために使用する
    //  // これがほしい Vidoes\chara\れいむ\cache\
    //  // clipped parts and 合成後のimg
    //  // png or bmp? bmpはでかい あとあとのことを考えるとpngで保存したほうがよさげ
    //  // pngは圧縮されているので小さいが、展開処理が入るので遅い



    //  // png or bmp どっちが早い?
    //  // ディスク容量の監視
    //  public static string charapicture; // Videos\chara
    //  public static string charapicture_original; // Videos\chara\original 元のzip or psdをそのまま保存
    //  public static string materialImage;
    //  public static string materialBGM;
    //  public static string materialSondEffect;
    //  public const string material = "resource";
      

    //  public static void init()
    //  {
    //    replacestr = new Dictionary<string, string>();
    //    home = Utils.Sysinfo.getHomedir();
    //    replacestr.Add("%HOME%", home);
    //    tempdir = Utils.Files.createTempDirectory();
    //    download = Utils.Sysinfo.getDownloaddir();
    //    // log = Utils.Sysinfo.getCurrentExepath() + "\\" + DIR_Log;
    //    // Utils.Files.mkdir(log);

    //    // dbより各ディレクトリを設定
    //    DB.Sqlite db = getSettingDB();
    //    DB.Query q = db.newQuery(Globals.table_directory);
    //    q.orderby = "orders";
    //    DataTable dt = q.getDataTable();
    //    foreach (DataRow r in dt.Rows)
    //    {
    //      object o = r[2];
    //      // とりま、chara dirのみ設定する
    //      // video\resource\chara?
    //      switch (r["directoryname"])
    //      {
    //        case "workspace":
    //          workspace = replaceENVstr(Utils.tostr(o));
    //          // replacestr.Add("%WORKSPACE%", workspace);
    //          Utils.Files.mkdir(Directory.charapicture);
    //          break;
    //        case "chara":
    //          Directory.charapicture = Directory.replaceENVstr(Utils.tostr(o));
    //          Utils.Files.mkdir(Directory.charapicture);
    //          break;
    //        case "charapicture_original":
    //          Directory.charapicture_original = Directory.replaceENVstr(Utils.tostr(o));
    //          break;

    //      }
    //      object eo = r["ENV"];
    //      if(eo != null && Utils.tostr(eo).Length > 0)
    //      {
    //        string buff = "%" + Utils.tostr(eo) + "%";
    //        replacestr.Add(buff, Utils.tostr(o));
    //      }
    //    }

    //  }

      //public static string replaceENVstr(string arg)
      //{
      //  // %HOME%を入れ替える
      //  // \r とかになってるんだ、、、
      //  string buff = System.Text.RegularExpressions.Regex.Unescape(arg);
      //  buff = arg.Replace(@"\", "\\"); // \表記を\\に変換し、
      //  buff = buff.Replace("\x5c", "\\"); // \表記を\\に変換し、
      //  buff = buff.Replace("\xa5", "\\");
      //  foreach (KeyValuePair<string, string> kp in replacestr)
      //  {
      //    buff = buff.Replace(kp.Key, kp.Value);
      //  }
      //  return buff;
      //}

  }
}

