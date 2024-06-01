using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;

namespace Utils
{
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
      if (Files.exist(file, Files.Filesearchmode.fileonly) == true)
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

}
