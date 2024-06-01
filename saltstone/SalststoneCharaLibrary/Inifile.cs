using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Utils;

namespace saltstone
{
  /// <summary>
  /// iniファイルを読み込み・保存するクラス
  /// </summary>
    public class Inifile : IDisposable
    {
        // public const string defaultinidir = @"C:\Users\yasuhiko\program\YukkuriBatch";
        // public const string regkey = "SOFTWARE\\YukkuriBatch"; // YB用のキー
        public const string pginifile = "setting.ini";  // pg.iniのファイル名
        // public const string regkeypgini = "pgini"; // キーの中のpg.iniのサブキー名
        // public const string aupinifile = "%PROJECTFILEBASE%.ybini";
        // public const string sqldbfile = "wavesamples.db";
        // wavesampleのdirectoryは?
        // voiceと同じ場所に保存したい
        // aup,iniとpg.iniを分けるべきか？
        // 特に分ける必要はないかが、初期値の設定を考えると分けるべき


        public Dictionary<string, string> settings;
        public static string inifilename;

        // pg用のiniファイル
        // 保存されるものは
        // ぼうよみちゃん exeへのパス
        // かんしくん exe へのパス
        // yb_guiの最後に開いたファイルへのパス
        // yu_guiの最後に選択したセクションの文字列
        // aviutilのexeへのパス

        // aup.iniファイル
        // aupファイルのパス
        // aviプロジェクトのディレクトリ
        // voieファイルのパス 固定する
        // wavesampleのsqldbのパス

        public string get(string key)
        {
            if (settings.ContainsKey(key))
            {
                return settings[key];
            }
            return "";
        }

        public void set(string key,string value)
        {
            if (settings.ContainsKey(key))
            {
                settings[key] = value;
            } 
            else
            {
                settings.Add(key, value);
            }
            
            settings[key] = value;
        }


        public Inifile()
        {
            init();
        }

        private void init()
        {
            if (settings != null)
            {
                settings.Clear();
                settings = null;
            }
            settings = new Dictionary<string, string>();

        }

        public Inifile(string arg)
        {
            init();
            load(arg);
        }

        public Inifile load(string arg)
        {

            // iniファイルはaviutil,ybのｐｇ系統の設定と
            // aupのプロジェクトの系統の２つある
            // 最後に開いたaupはaviutilのexeと同じディレクトリにあるaviutl.iniに保存されている
            // レジストリに保管するか？

            // pg.iniに保存するのは
            // aviutlのexeの場所
            // ぼうよみexeの場所
            // かんしくんの場所
            // yb_guiのlast file
            // yu_guiのlast section
            // yb_exeの場所
            // wavesamplingのexeの場所

            // proj.iniに保存するのは？
            // last aupファイル
            // last out mp4ファイル
            // wavesamplingのdbの場所 // voice配下固定？




            // iniファイルは実行時ディレクトリを既定とする
            // string path = System.Reflection.Assembly.GetEntryAssembly().Location;
            // path = System.IO.Path.GetDirectoryName(path);
            // path += @"\" + arg;
            
            // pg.iniファイルはprogram\yukkuribachとする
            // レジストリに書き込み、グローバル化する

            // aup.iniはaviutilのgcmzのapiより取得し、aupと同じ場所に保存する
            // どうやってaupファイルの場所を特定するか？
            // aviutilが実行されていないと取得できない

            // c++側ではbouyomiチャンは不要
            // voiceも不要
            // とうやって区分けするか？
            // コンパイルのUSEVOICEフラグ
            // pg.iniとaup.iniは必要になる？

            Inifile.inifilename = arg;

            string buff;

            bool b = File.Exists(arg);
            if (b == false)
            {
                // 存在しない場合、空ファイルを作成しておく
                buff = "[global]\r\n";
                File.WriteAllText(arg,buff);
                return null;

            }
            buff = File.ReadAllText(arg);

            // string[] ary = buff.Split(new string[] { "；" }, StringSplitOptions.RemoveEmptyEntries);
            //string[] ary = buff.Split(new string[] { "\r" }, StringSplitOptions.RemoveEmptyEntries);
            string[] ary = buff.Replace("\r\n", "\n").Split(new[] { '\n', '\r' });
            string[] line;
            foreach (string l in ary)
            {
                if (l == "") continue;
                if (l.Substring(0,1) == "[")
                {
                    continue;
                }
                line = l.Split(new string[] { "=" }, StringSplitOptions.None);
                if (line.Length != 2)
                {
                    Logs.write("ini file parser failed[" + line);
                    continue;
                }
                settings[line[0]] = line[1];
            }
            return this;
            /*
            Globals.envini = this;

            return true;
            */
        }
        ~Inifile()
        {

        }

        public void Dispose()
        {
             // write();
             // 保存処理はini exeのみで行う
            if (this.settings != null)
            {
                this.settings.Clear();
                this.settings = null;
            }
            Globals.envini = null;
        }

        public bool write()
        {
            // 保存処理は行わない 基本的に定義だけを行い、変更された場合はpg用のdb(yb.db)に保存する
            // ｐｇの状態により、enviniにすべてが読み込まれず、保存されない設定がでてきてエラーとなるため
            if (Inifile.inifilename == null)
            {
                return false;
            }
            if (this.settings ==  null)
            {
                return false;
            }

            /*
            using (StreamWriter fs = new StreamWriter(Inifile.inifilename))
            {
                string buff = "[global]";
                fs.WriteLine(buff);
                buff = "";
                foreach (KeyValuePair<string,string> kvp in this.settings)
                {
                    buff = kvp.Key + "=" + kvp.Value;
                    fs.WriteLine(buff);
                }

            }
            */
            // dbにsetting table用のメソッドが必要
            // db自体のcreateってできるのかな？


            return true;
        }

        public string this[string index]
        {
            get
            {
                if(this.settings.ContainsKey(index) == false)
                {
                    return "";
                }
                return this.settings[index];
            }
            set
            {
                this.settings[index] = value;
            }

        }



    }
}
