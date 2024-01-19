using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#if USEVOICE
using FNF.Utility;
#endif
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

    

    public class Globals : IDisposable
    {

        public enum Filesavemode { nosave, save } // 棒読みでファイル保存を行うかどうか

        public enum Charapicturetype { PSD, DIR } // 立ち絵がpsdtkか通常のディレクトリ素材か
        public enum Makemode { noplay, withplay } // 声を出しながら挿入するかどうか


        // private static Globals g;
        // singleto用のグローバル変数
        // staticにする必要あるのか？
        // 別のクラスライブラリにしたほうがよいかもしれない
        // yb_guiとybcharaでしか使用しない

        public const string SettingDB = "saltstone.db";

        // projectdirはgcmzのapiより取得するほうがいいかも
        // gcmzでmemory map fileにaupのデータを保存してある
        // そこからデータを取得したほうがよさげ fpsも取得できる
        // public const string DefaultDestfolder = @"F:\yukkuri_spoon\voice"; // これはaupの場所を元に演算
        // public const string DefaultDestfolder = @"C:\Users\yasuhiko\Documents\yukkuritest\voice";
        // public const string DefaultDestsubfolder = @"\voice";
        public const string DefaultDestfolder = "";
        public const string DefaultDestsubfolder = @"\voice";
        public static string destfolder = ""; //voiceの保存フォルダ
#if USEVOICE
        // 棒読みちゃんを使う場合 dllでは使用しないため、コンパイルエラーとなる
        public static BouyomiChanClient boyomi = null;
        // public static FNF.Utility.BouyomiChanClient boyomi = null;

        public static Dictionary<string, Voice> voicelist;
#endif
        public static Inifile envini = null; // 実行時ディレクトリにあるsettings.ini
        // public static Inifile aupini = null; // aupファイルと同じ場所にあるaup.ini // 未使用
        public static bool batchstop = false;
        public enum ePlaywait { wait, nowait }

        public static Dictionary<string, string> globalsection;

        // logを出力するメインフォーム
        // public static System.Windows.Forms.Form frm;

        // ybcharaで使用する args
        public static string[] cmdargs;

        // 立ち絵のタイプ psd or dir
        public static Charapicturetype charapicturetype;

        // public static Charas charas; // キャラの立ちえを管理するクラスのインスタンス
        public static Charas charas;
        public static DB.Sqlite charadb;
        
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

        public static void init()
        {


#if USEVOICE
            if (voicelist != null)
            {
                voicelist.Clear();
                voicelist = null;
            }
            if (boyomi != null)
            {
                // 棒読みIPCを再初期化する
                boyomi.Dispose();

            }

#endif
            if (globalsection != null)
            {
                globalsection.Clear();
                globalsection = null;
            }
            GC.Collect();
#if USEVOICE
            boyomi = new FNF.Utility.BouyomiChanClient();
            voicelist = new Dictionary<string, Voice>();
#endif
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
            string buff = envini[PGInifile.INI_SettingDB];
            if(buff.Length == 0)
            {
                buff = Utils.getexecdir() + "\\" + SettingDB;
                envini[PGInifile.INI_SettingDB] = buff;
            }

            // envini.set("pgdb",pgdbfname);


            // TODO テストパスを使ってる これはiniファイルに設定が必要
            DB.Sqlite db = new DB.Sqlite(buff);

            // charapicturetype  = db.getcharapicturetype();
            // TODO psd or dirの設定をdbから取得?

            if (Globals.envini[PGInifile.INI_Chardir] == "")
            {
                // exitをdelegateするとか、メッセージ表示をdelegateするとかしないといけない

                System.Windows.Forms.MessageBox.Show("キャラクタディレクトリがsetting.iniで定義されていません");
                System.Windows.Forms.Application.Exit();
                return;
            }


            // Charaクラスを作成
            charas = new Charas(); // ここでdbからキャラ情報を取得する

            // 別プロセスでパースを行う
        }

        public static void clear()
        {
#if USEVOICE
            voicelist.Clear();
#endif
        }

        public void Dispose()
        {
#if USEVOICE
            if (boyomi != null)
            {
                boyomi.Dispose();
                boyomi = null;
            }
#endif
            if(charadb != null)
            {
                charadb.Dispose();
                charadb = null;
            }
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

    }
}

