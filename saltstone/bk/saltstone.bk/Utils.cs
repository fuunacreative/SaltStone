using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace saltstone
{
    internal static class Utils
    {
        public enum filesearchmode
        {
            fileonly,
            directoryonly,
            all
        }

        public static bool runexec(string processname)
        {
            bool runflag = false;
            System.Diagnostics.Process[] procs = System.Diagnostics.Process.GetProcessesByName(processname);
            if (procs.Length != 0)
            {
                runflag = true;
            }
            return runflag;
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

        public static string getlastdirectoryname(string path)
        {
            return new DirectoryInfo(System.IO.Path.GetDirectoryName(path)).Name;

        }

        public static  List<string> getdirectory(string path)
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

        public static string getfilename(string path)
        {
            return Path.GetFileName(path);
        }
        public static bool fileexist(string path,filesearchmode mode = filesearchmode.all)
        {
            // ディレクトリがある場合は失敗する
            // modeで切り替えできるようにしておく
            bool fret = File.Exists(path);
            if(mode == filesearchmode.fileonly)
            {
                return fret;
            }
            bool dret = Directory.Exists(path);
            if(mode == filesearchmode.directoryonly)
            {
                return dret;
            }
            return fret | dret;



        }
        public static string readAllText(string filepath)
        {
            // encording 処理
            FileInfo finfo = new FileInfo(filepath);
            Encoding e;
            using (Hnx8.ReadJEnc.FileReader f = new Hnx8.ReadJEnc.FileReader(finfo))
            {
                Hnx8.ReadJEnc.CharCode c = f.Read(finfo);
                e = c.GetEncoding();
            }
            finfo = null;
            return File.ReadAllText(filepath, e);
        }

        public static List<string> searchfile(string path , string filepattern)
        {
            if(fileexist(path) == false)
            {
                return new List<string>(); // emptyのlistを返す
            }
            IEnumerable<string> files = Directory.EnumerateFiles(path, filepattern, SearchOption.TopDirectoryOnly);
            return files.ToList();

            //            return null;
        }

        public static bool existFile(string path)
        {
            return File.Exists(path);
        }

        public static string getexecdir()
        {
            string buff = System.AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\');
            return buff;
        }


    }


}
