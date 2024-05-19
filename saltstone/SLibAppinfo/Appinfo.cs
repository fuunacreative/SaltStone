using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace saltstone
{
  public class Appinfo
  {
    private const string pSettingsDB = "settings.db";
    private const string EXE_ImageProcess = "saltstoneimage.exe";
    
    // static stringは pgが終了しても残り続ける？
    private static string pHomedir;
    private static string pProgmamdir; // 独自pgのdir %home%\program
    private static string pCharabasedir; // キャラ素材dir %home%\videos\charas
    private static string pCharaOriginaldir; // キャラ立ち絵のzipファイル置き場
    private static string pExecfilename; // 実行時exe fileの filename
    private static string pTempdir;

    private static string pAviutilexefile; // aviutlのexe full path
    // private static string pBouyomiexefile; // ぼうよみちゃん exe full path
    private static string pForceparserexefile; // かんしくん exe full path

    private static string pCharadbfname; // chara.db full path
    private static string pAviuworkspacedir; // 動画作成のワークスペース directory

    private static string pSettingsdbfname;
    // private static string ;


    public static bool init()
    {
      pSettingsdbfname = Utils.Sysinfo.getCurrentExepath();
      pSettingsdbfname += "\\" + pSettingsDB;
      DB.Sqlite sdb = new DB.Sqlite(pSettingsdbfname);
      Dictionary<string, string> replacestring = new Dictionary<string, string>(); 


      string sql = "";
      sql += "SELECT pathid,pathtype,fullpath,alias FROM pathsetting ORDER BY setorder";
      DB.DBRecord rec = sdb.getrecord(sql);
      string pathid;
      string pathtype;
      string fullpath;
      string alias;
      string realpath;
      do
      {
        pathid = rec.getstring(0);
        pathtype = rec.getstring(1);
        fullpath = rec.getstring(2);
        alias = rec.getstring(3);
        realpath = fullpath;
        if (realpath == "API")
        {
          switch (pathid)
          {
            case "homedir":
              realpath = Utils.Sysinfo.getHomedir();
              break;
            case "exedir":
              realpath = Utils.Sysinfo.getCurrentExepath();
              break;
          }
        }
        realpath = realpath.Replace("/", "\\");
        // aliasをdictionaryに登録
        if (string.IsNullOrEmpty(alias) == false)
        {
          replacestring[alias] = realpath;
        }

        // fullpathをalias dictionaryで置換
        foreach (string repsrc in replacestring.Keys)
        {
          if(realpath.IndexOf(repsrc) == -1)
          {
            continue; 
          }
          realpath = realpath.Replace(repsrc, replacestring[repsrc]);
        }

        // directory separator "/" を "\\"に変換

        switch (pathid)
        {
          case "homedir":
            pHomedir = realpath;
            break;
          case "charabasedir":
            pCharabasedir = realpath;
            break;
          case "charadb":
            pCharadbfname = realpath;
            break;
          case "tempdir":
            pTempdir = Utils.Files.createTempDirectory();
            break;
          case "charapicture_original":
            pCharaOriginaldir = realpath;
            break;
        }

        // path内部のaliasを置換する必要があるため、dbに置換用 stringをもうける必要がある

      } while (rec.Read() == true);
      rec.clear();
      sdb.Dispose();
      return true;

    }


    public static string charabasedir
    {
      get {
        if (string.IsNullOrEmpty(pCharabasedir))
        {
          init();
        }

        return pCharabasedir;
      }
    }

    public static string charadbfname 
    {
      get 
      {
        if (string.IsNullOrEmpty(pCharabasedir))
        {
          init();
        }

        return pCharadbfname;

      }
    }

    public static string exeimageprocess
    {
      get {
        return Utils.Sysinfo.getCurrentExepath() + "\\" + EXE_ImageProcess;
      }
    }

    public static string tempdir 
    {
      get {
        if (string.IsNullOrEmpty(pTempdir) == true)
        {
          pTempdir = Utils.Files.createTempDirectory();
        }
        return pTempdir;
      }
    }
    // DBを読み出すたびに設定する
    public static string CharaOriginaldir 
    {
      get {
        if (string.IsNullOrEmpty(pCharaOriginaldir))
        {
          init();
        }

        return pCharaOriginaldir;
      }
    }

  }
}
