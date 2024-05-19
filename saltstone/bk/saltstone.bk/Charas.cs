using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace saltstone
{
    // 全体のキャラ素材情報を保持するクラス
    // 指定されたキャラ素材ディレクトリ内のすべてのディレクトリ、ファイル、chara.txt、favorite.txtを解析し、データとして保存する
    // どこでインスタンスを作成し、保持するべきか？
    // form? global? ybcharaでも使用するか
    // そうすると、ディレクトリ解析も必要になるな
    // キャラ全部を管理 キャラ素材ディレクトリを解析
    // キャラ素材ディレクトリ\yub.charaにデータを保存


    public class Charas
    {

        // 別名やお気に入りはテキストファイルで管理 dbで管理すると編集画面が必要になるから

        // public static Sqlite db; // キャラ用のdb キャラ素材dir \ ybchara.db
        // staticではdisposeできない 
        // 使うところでdbを呼び出す必要がある
        // だが、他から参照できない staticにしたいなー

        // singletonにすれば問題なさそう
        // https://qiita.com/haniwo820/items/ba0ab725c25673c20383


        public const string setfname = @"全\セット.txt";
        public const string outdir = "out"; // rubyで処理したセット.txtから合成した画像ファイルの保存先
        public const string partsfile = "chara.txt";
        public const string favoritefile = "favorite.txt";
        public const string charadb = "saltstonechara.db";
        // public const string setfile = @"全\セット.txt";
        // public const string outdir = "out";
        // public const string envsetting = "chardir"; // iniファイルに書き込まれるキャラ素材ディレクトリのpath
        // public const string charatable = ""
        public static string charadir = ""; // キャラ素材ディレクトリ
        public string charadbfullpath;
        public DB.Sqlite db;

        public const string table_charactor = "charactor"; // れいむとかまりさ
        public const string table_charaset = "charaset"; // セット.txt outディレクトリ内のpngを含む
        // id , charaid(れいむ) , setnum(int 123など) , partsstr(顔00-眉04-目00-口06-体00) , filename(out/*.png)
        public const string table_parts = "partsset"; // セット.txtのsetnumに紐付けれらたパーツ
        // id , charaid,setnum , partsid , partsnum , animationflag
        // partsid = B F E Yなど
        // partsnum = 00など
        // animationflag = A or null nullの場合、アニメなし
        // 
        public const string table_favorite = "favoite"; // favorite.txtの保存
        public const string table_partsname = "parts"; // chara.txtのパーツ名表示 パーツの表示名を保持する
        // id , charaid , partsid(B F E Y) , partsnum(00 or 01) , partsfilename
        public const string table_division = "division"; // 区分ファイル division=charadirはB->体の対応表


        // れいむ -> データ、まりさ -> データ
        public Dictionary<string, Chara> chara = null;

        public Charas()
        {
            // ybchara.dbを検索し、キャラの名前を取得
            // charaに展開しておいて、名前の一覧を取得できるようにする
            // favorite.txtも、パーツの別名定義であるchara.txtもすべてdbに保存されているものとする
            // 解析はybhcara.exe側で行う

            chara = new Dictionary<string, Chara>();

            // charadbfullpath = Globals.envini[Charas.envsetting] + "\\" + Charas.charadb;
            //charadbfullpath = Globals.envini[Charas.envsetting] + "\\" + Charas.charadb;
            charadbfullpath = Globals.envini[PGInifile.INI_CharaDB];

            db = new DB.Sqlite(charadbfullpath);
            Globals.charadb = db;
            DB.Query q = new DB.Query(Charas.table_charactor);
            q.select = "name,directory";
            DB.DBRecord rec = db.getrecord(q);
            string buff;
            string charadir;
            Chara c;
            do
            {
                buff = rec.getstring();

                charadir = rec.getstring(1); // fullpath
                c = new Chara(charadir);
                if (chara.ContainsKey(buff))
                {
                    continue;
                }
                chara.Add(buff, c);
            } while (rec.Read() == true);
            // dbと実際のディレクトリ構造が一致しているかどうかのチェックが必要
            // ybchara側で処理する
        }
        ~Charas()
        {
            if (chara != null)
            {
                // TODO charaに登録されているCharaクラスのインスタンスもクリアしないといけないかも
                chara.Clear();
                chara = null;
            }
        }

        public Chara this[string arg]
        {
            get
            {
                return chara[arg];
            }

        }


        // 立ち絵のディレクトリを解析し、存在するパーツ、アニメするパーツを解析
        // アニメの開度におけるパーツファイル名を保持
        // これって、解析プログラムを別プロセスで走らせたほうがよい
        // yb_guiで走らせるべき？
        // どのタイミングで実行するのがベストなのか？
        // 素材をコピーした段階で実行するべきだ aviu側では絶対に実行してはならない
        // メイン画面でキャラディレクトリを選択した段階


        // 問題となるのは、キャラ識別子 r -> れいむをどこで管理するか？
        // 現在はシナリオファイルｆで識別している
        // そうか、rである必要はないのだ
        // サブディレクトリを検索し、すべてのキャラ素材を解析する
        // それぞれについてcharaparseを呼び出す
        // exeのcmdargにrを指定しているな
        // あと、ファイルが変更されたかどうかの判定も必要になるのでは？
        // そうだな、aviuからはIDれいむで渡されてくる
        // rはあくまでシナリオファイル上での略記号にすぎない
        // gui上でキャラ指定はどうするか？
        // キャラテーブルがあればいいのか
        // string buff = "";
        // chardirの全部のサブディレクトリを検索
        // List<string> dirs = YukkuriBatch.Utils.getdirectory(charadir);
        // やはり、リンクでソースを持ってくると警告がでる
        // なぜか？ sqlite自体がdllで元のexeのファイルと競合を起こすため
        // List<string> chars = new List<string>();

        // キャラ素材ディレクトリの全部を解析する
        public bool parse()
        {
            // キャラ素材ディレクトリをすべて検索しdbを更新する

            // 初期化処理
            // charadir = Globals.envini[Charas.envsetting];
            // charadbfullpath = charadir + "\\" + Charas.charadb;
            charadir = Globals.envini[PGInifile.INI_Chardir];
            charadbfullpath = Globals.envini[PGInifile.INI_CharaDB];
            this.db =  new DB.Sqlite(charadbfullpath);
            // string sql = "";


            if (Directory.Exists(charadir) == false)
            {
                // キャラ素材dirがなければ何もしない
                return false;
            }

            // charactor tableを削除
            DB.Query q = new DB.Query(table_charactor);
            db.delete(q);

            // private charasをクリア 、再登録する
            if(chara == null)
            {
                chara = new Dictionary<string, Chara>();
            }
            chara.Clear();

            // サブディレクトリ名を検索
            List<string> dirs = Utils.getdirectory(charadir);
            

            // DB.DBRecord rec = new DB.DBRecord("charactor");
            DB.DBRecord rec = new DB.DBRecord(Charas.table_charactor);
            int i = 1;
            string charaid;
            Chara c;
            foreach (string s in dirs)
            {
                charaid = Path.GetFileName(s);
                rec["name"] = charaid;
                rec["directory"] = s;
                rec.setint("disporder", i);
                db.Write(rec);
                // 内部のcharaを再構築する
                c = new Chara(s);
                chara.Add(charaid, c);
            }

            // dirsのサブディレクトリごとにcharasetに解析をさせる
            // セット.txtの解析
            // favorite.txtの解析
            // chara.txtの解析
            // ファイル名によるパーツ番号とアニメーションするかどうかの解析
            // -> table charaset ,parts,favoiteの保存

            // 解析処理はどこで行うか？
            // このクラスで一括して行うのがよさそうだな
            // 使い方を考えるとセット.txtの解析が優先だな
            // dbをglobalに保存するので、charaクラスで解析するべきだと思う
            foreach(Chara cs in chara.Values)
            {
                // cs.parseset(); // set.txtを解析
                // cs.parsefavorite(); // favorite.txtの解析
                // cs.parsepartsdef(); // chara.txt パーツの表示名
                // cs.parseparts(); // 各パーツのサブディレクトリを解析
                // cs.parse() // 上記の全部を行う
                cs.parse();
            }
            /*
            foreach (string s in dirs)
            {
                parseset(s);
            }
            */
            db.Dispose();



            return true;
        }

        public bool parseset(string charadir)
        {
            // charadirにはれいむのフルパスが入ってくる
            string setfpath = charadir + "\\" + Charas.setfname; // セット.txtを解析
            if(Utils.fileexist(setfpath) == false)
            {
                return false;
            }
            string alltext = Utils.readAllText(setfpath);
            // 一行ごとに処理
            // public const string table_charaset = "charaset"; // セット.txt outディレクトリ内のpngを含む
            // id , charaid(れいむ) , setnum(int 123など) , partsstr(顔00-眉04-目00-口06-体00) , filename(out/*.png)
            // partsset -> setnumに紐付けされたパーツ
            // id , charaid , setnum , partsid( B F E Y) , partsfilesub(00 or 01), animeaion(A or NULL)
            List<string> lines = new List<string>(alltext.Replace("\r\n", "\n").Split(new[] { '\n', '\r' }));
            int i;
            string[] ary;
            int setnum;
            string partsstr;
            string outpng = null;
            bool ret;
            List<string> files;
            string outdirbuff = charadir + "\\" + outdir;
            string buff;
            DB.DBRecord rec = new DB.DBRecord(Charas.table_charaset);

            // charaidを切り出し
            string charaid = Utils.getfilename(charadir); // 最後のサブディレクトリ名をキャラIDとして使用する

            // charasetのれいむを削除
            DB.Query q = new DB.Query(Charas.table_charaset);
            q.where("charaid", charaid);
            db.delete(q);

            foreach (string s in lines)
            {
                // "="で分割
                ary = s.Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                if(ary.Length != 2)
                {
                    // =がない場合、正常に解析できないと判断
                    continue;
                }
                ret = int.TryParse(ary[0],out setnum);
                if (ret == false)
                {
                    // 先頭のセット番号が数値でない場合は解析不能と判断
                    continue;
                }
                // コメントがある "//"
                buff = ary[1];
                i = buff.IndexOf("//");
                if (i > 0)
                {
                    buff = buff.Substring(0, i).Trim() ;
                }
                partsstr = buff;
                // setnumよりoutディレクトリのpngを検索
                // setnumが１桁の場合は01だが、３桁の場合は111となる
                // いや、セット.txtの問題だな

                buff = ary[0] + "_*.png";
                // ファイル検索
                // セット番号10の場合、ファイルが10_と100_の両方がヒットする
                files = Utils.searchfile(outdirbuff, buff);
                outpng = "";
                if(files.Count > 0)
                {
                    outpng = files.First();
                }
                // dbへ保存
                rec["charaid"] = charaid;
                rec.setint("setnum", setnum);
                rec["partsstr"] = partsstr;
                rec["filename"] = outpng;
                db.Write(rec);
            }

            return true;
        }
    }

}
