using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;

namespace saltstone
{

    // 立ち絵ディレクトリと
    // aviu側への立ちえ設定、字幕設定、
    // 棒読みちゃんへの発声設定
    // これらの情報を保持する
    // TODO ybcharaを検索し、必要なパーツを検索するためのアクセサクラス
    // ぼうよみちゃんへの発声も必要ないかも



    public class Chara
    {

        // public Dictionary<string, string> partdirpair = null;

        public string charaid = ""; // れいむとかまりさ  // -> rとかm シナリオファイル上のＩＤ
        public string charakey; // r or mなど １文字
        public string charadir;
        public int width;  // 画像の横幅
        public int height; // 画像のたて
        public double aspectrate; // よこにたいするたてのひりつ w300/h400 = 0.75
        // よこをかければたてがでてほしいんだから、300/400か

        public static Dictionary<string, string> partsids; // 体->Bの対応表


        // public string fullpath = "";
        // public Dictionary<string, CharaParts> partsdata = null;
        /*
        public Dictionary<string, CharaParts> all = null;
        public Dictionary<string, CharaParts> eye = null;
        public Dictionary<string, CharaParts> mayu = null;
        public Dictionary<string, CharaParts> body = null;
        public Dictionary<string, CharaParts> back = null;
        public Dictionary<string, CharaParts> hair = null;
        public Dictionary<string, CharaParts> face =     public Dictionary<string, CharaParts> lip = null;
        */

        // public Voice voice; // 発声する場合のぼうよみちゃんへのアクセスオブジェクト

        public Chara(string arg)
        {
            // fullpath = arg; // れいむとかまりさ
            charaid = Path.GetFileName(arg);
            charadir = arg;
            // partdirpair = new Dictionary<string, string>();
            // partsdata = new Dictionary<string, CharaParts>();
            // 画像のたて・よこを取得する

            // まず全を検索
            string path = charadir + @"\全";
            string searchfile = "*.png";
            List<string> pngf = Utils.searchfile(path, searchfile);
            string png = "";
            if (pngf.Count > 0)
            {
                png = pngf.First();
            }
            // 全がなければ体を検索
            if (png == "")
            {
                path = charadir + @"\体";
                pngf = Utils.searchfile(path, searchfile);
                if (pngf.Count > 0)
                {
                    png = pngf.First();
                }
            }
            if(string.IsNullOrEmpty(png) == false)
            {
                Image img = Image.FromFile(pngf.First());
                width = img.Width;
                height = img.Height;
                aspectrate = 1.0 * height / width;
            }

            // TODO 可能であれば、不要な領域を削除したい
            // しかしこれはかなり難しそう ピクセルを操作し、最初のたて位置、最後のたていち、最初のよこいち、最後のよこいちを検索し
            // imageをクリッピングする cropする startx y  と crop width heightの４つが必要
        }

        // アスペクト比率にもとづいた縦幅を返す
        public int getaspectheight(int width)
        {
            if(aspectrate == 0)
            {
                return width;
            }
            return (int)(aspectrate * width);
        }

        // dbはこのクラスで持っているのでここで処理するのがいいと思う
        // 本来であれば、dbをglobal化して、Charaクラスで処理するべきだと思う
        public SortedDictionary<int, string> getoutpng(string charaid, List<string> where = null)
        {
            // TODO ソートされていない
            DB.Query q = new DB.Query(Charas.table_charaset);
            q.select = "setnum,filename";
            q.where("charaid", charaid);
            q.orderby = "setnum";
            // string buff = "";
            if (where != null)
            {
                foreach (string s in where)
                {
                    q.like("searchkey", s);
                }

                // q.like("searchkey",)
                // q.where("searchkey", "");
            }
            DB.DBRecord rec = Globals.charadb.getrecord(q);
            SortedDictionary<int, string> outpng = new SortedDictionary<int, string>();
            // List<string> outpng = new List<string>();
            do
            {
                outpng.Add(rec.getnum(0), rec.getstring(1));
            } while (rec.Read() == true);
            return outpng;
        }

        // パーツの検索条件を元にsetnumをｄｂから検索して返す
        public List<int> getoutsetnum(string charaid,List<string> where)
        {
            // where にはF01とかの検索用キーが入っている
            DB.Query q = new DB.Query(Charas.table_charaset);
            q.select = "setnum";
            q.orderby = "setnum";
            if(where != null)
            {
                foreach(string s in where)
                {
                    q.like("searchkey", s);
                }
            }
            DB.DBRecord rec = Globals.charadb.getrecord(q);
            List<int> ret = new List<int>();
            do
            {
                ret.Add(rec.getnum(0));
            } while (rec.Read() == true);
            return ret;
        }

        public string getpsdtoolkitstr(string charaid,int setnum)
        {
            DB.Query q = new DB.Query(Charas.table_charaset);
            q.select = "psdtoolkitstr";
            q.where("charaid", charaid);
            q.where("setnum", setnum);
            string ret = Globals.charadb.getonefield(q);
            return ret;
        }

        private void initpartsids()
        {
            if (partsids == null || partsids.Count == 0)
            {
                // 体->Bの対応表を読み込む
                DB.Query q = new DB.Query(Charas.table_division);
                q.select = "value,name";
                q.where("division", "charadir");
                if (partsids == null)
                {
                    partsids = new Dictionary<string, string>();
                }
                partsids.Clear();
                DB.DBRecord rec = Globals.charadb.getrecord(q);
                do
                {
                    partsids.Add(rec.getstring(0), rec.getstring(1));
                } while (rec.Read() == true);
            }
        }

        public bool parse()
        {
            initpartsids();

            // セット.txtの解析
            bool ret = parsesets();
            if (ret == false)
            {
                return false;
            }
            // chara.txtの解析 // 各パーツの表示名
            ret = parsepartsname();
            if (ret == false)
            {
                return false;
            }



            return true;
        }

        /* CREATE TABLE "division" (
	"id"	INTEGER NOT NULL,
	"division"	TEXT NOT NULL,
	"name"	TEXT,
	"value"	TEXT,
	"order"	INTEGER,
	PRIMARY KEY("id" AUTOINCREMENT)
)*/


        // 07a-15.pngとかいうファイルがある
        // mayuiciposで眉の描画位置をずらしているみたい
        // -15があると固定でmayuiciposを-10にしてる
        // -15は仕様なので、無視 先頭の07のみを使用する -> psdtoolkitのスライダーの番号に紐付けられる


        public bool parsepartsname()
        {

            string setfpath = charadir + "\\" + Charas.partsfile; // セット.txtを解析
            if (Utils.fileexist(setfpath) == false)
            {
                return false;
            }


            DB.DBRecord rec;
            DB.Query q;

            // table削除
            q = new DB.Query(Charas.table_partsname);
            q.where("charaid", charaid);
            Globals.charadb.delete(q);

            string alltext = Utils.readAllText(setfpath);
            List<string> lines = new List<string>(alltext.Replace("\r\n", "\n").Split(new[] { '\n', '\r' }));
            string sectionname = "";
            string buff;
            int i;
            string[] ary;
            // rec.clear();
            rec = new DB.DBRecord(Charas.table_partsname);
            bool ret;
            foreach (string s in lines)
            {
                if(s.Length == 0)
                {
                    continue;
                }
                if(s.Substring(0,2) == "//")
                {
                    continue;
                }
                if(s.Substring(0,4) == "[sec")
                {
                    i = s.IndexOf(" ");
                    buff = s.Substring(i + 1);
                    ary = buff.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                    sectionname = ary[0];
                    continue;
                }
                buff = s;
                i = buff.IndexOf("//");
                if (i > 0)
                {
                    buff = buff.Substring(0, i).Trim();
                }
                ary = buff.Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                // 固定で先頭２文字をパーツ番号とする
                // -15とかzとかはアニメスクリプトの固定仕様
                // psdtoolkitとは非常に相性が悪い
                // 先頭の２文字を固定で問題なさそう
                // -> psdtoolkitの番号とひもづけさせる
                buff = ary[0].Substring(0, 2);
                rec["partsnum"] = buff;
                rec["charaid"] = charaid;
                rec["partsid"] = partsids[sectionname];
                rec["filename"] = ary[0];
                rec["name"] = ary[1];
                ret = int.TryParse(buff,out i);
                rec.setint("disporder", i);
                rec["searchkey"] = partsids[sectionname] + buff; // F00とか
                Globals.charadb.Write(rec);
            }
            return true;
        }


            public bool parsesets()
        {
            // セット.txtの解析を行う 
            // 今はrubyで合成したoutディレクトリがあるが、最終的にはc++の合成ルーチンで画像合成を行い
            // outディレクトリへ出力する

            string setfpath = charadir + "\\" + Charas.setfname; // セット.txtを解析
            if (Utils.fileexist(setfpath) == false)
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
            string outdirbuff = charadir + "\\" +  Charas.outdir;
            string buff;
            DB.DBRecord rec = new DB.DBRecord(Charas.table_charaset);
            string[] parts; // 顔00-眉08-目00-口02-体00を切り分けたリスト

            // charaidを切り出し
            string charaid = Utils.getfilename(charadir); // 最後のサブディレクトリ名をキャラIDとして使用する

            // charasetのれいむを削除
            DB.Query q = new DB.Query(Charas.table_charaset);
            q.where("charaid", charaid);
            Globals.charadb.delete(q);


            foreach (string s in lines)
            {
                // "="で分割
                ary = s.Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                if (ary.Length != 2)
                {
                    // =がない場合、正常に解析できないと判断
                    continue;
                }
                ret = int.TryParse(ary[0], out setnum);
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
                    buff = buff.Substring(0, i).Trim();
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
                if (files.Count > 0)
                {
                    outpng = files.First();
                }
                // psdtoolkitの多目的スライダーで使用するための文字列を編集
                // 顔00目00口00眉00
                // 顔00-眉00-目00-口00-体00
                // splitで分ける
                parts = partsstr.Split(new string[] { "-" }, StringSplitOptions.RemoveEmptyEntries);
                buff = parts[0];
                buff += parts[2];
                buff += parts[3];
                buff += parts[1];



                // dbへ保存
                rec["charaid"] = charaid;
                rec.setint("setnum", setnum);
                rec["partsstr"] = partsstr;
                rec["filename"] = outpng;
                rec["psdtoolkitstr"] = buff;
                rec["searchkey"] = setstrtosearchkey(buff);

                // TODO searchkeyを編集する
                // 顔00目00口06眉04 -> F00E00L06Y04に変更
                Globals.charadb.Write(rec);
            }
            return true;
        }

        private string setstrtosearchkey(string setstr)
        {
            string buff = setstr;
            foreach(KeyValuePair<string,string> p in partsids)
            {
                buff = buff.Replace(p.Key, p.Value);
            }

            return buff;

        }

        public SortedDictionary<string,string> getparts(string charaid , string partsname)
        {
            initpartsids();
            // partsname は体とか顔とか
            string partskey = partsids[partsname];
            SortedDictionary<string, string> buff = new SortedDictionary<string, string>();
            DB.Query q = new DB.Query(Charas.table_partsname);
            q.where("charaid", charaid);
            q.where("partsid", partskey);
            q.select = "searchkey,name";
            DB.DBRecord rec = Globals.charadb.getrecord(q);
            do
            {
                buff[rec.getstring(0)] = rec.getstring(1);
            } while (rec.Read() == true);
            return buff;
        }

    }






}
