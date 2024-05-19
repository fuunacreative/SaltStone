using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using saltstone;
using System.IO;
using System.Text.RegularExpressions;

namespace SLibChara_Make
{
  public class CharaMake
  {
    public const string CHARDEF_Favorite = "favorite.txt"; // お気に入りの登録 charadef.txtに統合
    public const string CHARDEF_SETDEF = "全\\セット.txt"; // キャラ素材に用意されているセット.txt
    public const string CHARDEF_Chardef = "charadef.txt"; // キャラ定義のtxt dbに保存されているものを外だししたもの
    public static DB.Sqlite charadb;
    public static Dictionary<string, string> partsdirbyname;
    public static Dictionary<string, string> charaids; // キャラ識別子 れいむ -> r

    //public static List<Regex> drawrules;
    // rule.txtを解析した描画順、非表示パーツのtable drawruleのidとregexを保存
    // public static Dictionary<string, Regex> drawrules;
    public static Dictionary<string, Regex> drawoverlayrules;
    public static Dictionary<string, Regex> drawlocationadjustrules;
    public static Dictionary<string, Regex> drawcolorchangerules;

    public static string pDefaultOverlayrule;

    // パーツの描画順を保持するファイル 通常はchardir\に格納されている
    // public const string CHAR_PETTERNFILE = "charadraworder.txt";
    public const string RULEFILE = "rule.txt";

    // パーツ サブ idを保管する KK -> 後\[0-9]+.png
    public static Dictionary<string, string> subpartsids;
    // 高速化のためregexで保管する
    public static Dictionary<string, Regex> subpartsidsreg;



    public static void Dispose()
    {
      if (charadb != null)
      {
        charadb.Dispose();
        charadb = null;
      }
      if (partsdirbyname != null)
      {
        partsdirbyname.Clear();
        partsdirbyname = null;
      }
      if (charaids != null)
      {
        charaids.Clear();
        charaids = null;
      }
      if (drawoverlayrules != null)
      {
        drawoverlayrules.Clear();
        drawoverlayrules = null;
      }
      if (drawlocationadjustrules != null)
      {
        drawlocationadjustrules.Clear();
        drawlocationadjustrules = null;
      }
      if (drawcolorchangerules != null)
      {
        drawcolorchangerules.Clear();
        drawcolorchangerules = null;
      }

  }

    // 解析が必要かどうかの判断
    public static bool checkreparse()
    {
      bool fret = true;
      // for debug

       // return false;
      string w = Appinfo.charabasedir;
      // false=parse not need
      // true=force parse

      fret = false;
      return fret; 
      // 内部ディレクトリを検索 validateをやってokなら、charadef.txtが存在するかチェック
      // なければ再解析が必要と判断




      // return fret;
    }

    public static bool setCharaID(string dirname, string charaid)
    {
      if(charaids == null)
      {
        charaids = new Dictionary<string, string>();
      }
      string basename = Utils.Files.getfilename(dirname);
      charaids[basename] = charaid;
      string buff;
      buff = dirname + "\\" + CHARDEF_Chardef;
      if (Utils.Files.exist(buff) == false)
      {
        // charddef.txtを作成する
        // これが存在する場合、解析済みと判定
        // 最終的にはこのファイルに子階層内の最新ファイルの日時
        // をもたせ、再解析が必要かどうか判定する
        buff = "ID=" + charaid;
        StreamWriter fs = new StreamWriter(buff);
        fs.WriteLine(buff);
        // fs.Write(buff);
        fs.Close();
        fs.Dispose();
        fs = null;
      }

      return true;
    }
    public static bool Makedb_dirtype(string charadirpath)
    {
      // wの中のdirectoryを検索
      List<string> dirs = saltstone.Utils.Files.getdirectory(charadirpath);
      // originalが存在する zipファイルの格納場所

      // 全\セット.txtがある or 体\*.png or 画像ファイルがある -> 正常なdir形式 キャラ素材と判定
      // -> copy時にfavorite.txtを作成する これをもって正常なキャラ素材と判定する
      // or set.txt chara.txt を作成する

      // db rec insertにも使用するので、ここで初期化してしまう
      charadb = new DB.Sqlite(saltstone.Appinfo.charadbfname);

      // divisionsを取得 体->Bを取得
      DB.Query q = new DB.Query();
      q.table = "division";
      q.select = "name,value";
      q.where("division", "charadir");
      q.orderby = "orderno";
      DB.DBRecord rec = charadb.getrecord(q);
      string dirname; // division.value 体
      string dirid; // division.name B 
      if (partsdirbyname != null)
      {
        partsdirbyname.Clear();
      }
      partsdirbyname = new Dictionary<string, string>();
      do
      {
        dirname = rec.getstring(1);
        dirid = rec.getstring(0);
        partsdirbyname[dirname] = dirid;

      } while (rec.Read() == true);

      // dirname "れいむ" に対してidをどうわりふるか？
      // 自動でIDを割り振る or popup windowを表示して手動設定する
      // ユーザに指定させる必要がある -> popup windowを表示させる
      //  自動で候補となるIDを設定 後でキャラ定義でユーザー定義させる
      // https://api.excelapi.org/language/kanji2kana
      // dirnameの先頭一文字を使用する
      // 漢字の場合はそのまま使用か、上記の漢字toひらながapiでひらがなに変換させる
      // 別exeのキャラセット定義画面でも使用できるように、basedirを１キャラづつ処理する






      bool fret;
      // string buff;
      foreach (string sdir in dirs)
      {
        // buff = sdir;
        fret = checkvalidate_typedir(sdir);
        if (fret == false)
        {
          continue;
        }
        fret = Makedb_dirtype_onechar(sdir);

        // チェックがとおり、正常なキャラ素材と判定できたので、dbに登録 正規キャラ素材画等として扱う
        // Chara myc = new Chara();
        // dir , fileからdbを作成し、登録する処理が必要
        // まずはパーツ解析

        // 解析をどうするか？ 最終的にほしいのは各パーツのファイル名
        // dbサイズに問題ないか？
        // db構造をもう一度再検討する必要があるね 

        // なにがしたいかというと、ふくわらいをまず作る
        // -> psd形式,ymm4charadir形式への変換,outpngの出力

        // chara.dbを開いてdivisionsを検索、name , value , ORDER BY ordernoで検索し、
        // value=subdirname , name=識別子 Bを検索

        // anime partsの切り分け
        // -> listboxに表示


      }

      // -> chara classをつくり、登録する
      // table charactor ins record
      // 各パーツ
      // １、体→顔(通常)→髪(不透明)→眉→目→口→髪（半透明） Aパターン
      // ２，体→髪(不透明)→眉→目→口→髪（半透明）→　他 →顔（乗算） -> もう一度髪 後ろは？


      return true;
    }

    public static bool checkvalidate_typedir(string basedir)
    {
      // 全\セット.txtがある or 体\*.png or 画像ファイルがある -> 正常なdir形式 キャラ素材と判定
      // -> copy時にfavorite.txtを作成する これをもって正常なキャラ素材と判定する
      // or set.txt chara.txt を作成する

      string buff;
      bool fret;

      buff = basedir + "\\" + CHARDEF_Chardef; // charadef.txt
      fret = saltstone.Utils.Files.exist(buff);
      if (fret == true)
      {
        return true;
      }


      buff = basedir + "\\" + CHARDEF_Favorite; // favorite.txt
      fret = saltstone.Utils.Files.exist(buff);
      if (fret == true)
      {
        return true;
      }

      // buff = basedir + "\\全\\セット.txt";
      buff = basedir + "\\" + CHARDEF_SETDEF; // 全\セット.txt
      fret = saltstone.Utils.Files.exist(buff);
      if (fret == true)
      {
        return true;
      }

      // ここまででなかったら、あとは、体\00.pngなどの画像があるか判定するしかない



      return false;
    }


    public static bool Makedb_dirtype_onechar(string charadir)
    {
      // IDとしては、charadirの先頭一文字”れいむ” -L "れ”とする
      string cid = Utils.Files.getbasename(charadir).Substring(0, 1);

      // partsdirbyname に登録されているディレクトリを検索し　パーツに分解
      //      CREATE TABLE "parts"(
      //  "id"  INTEGER NOT NULL UNIQUE,
      //  "charid"  TEXT,　 // <- charactor.id 後ほど手動でcharaidを設定してからupdate
      // chardirkey れ
      //  "partsid" TEXT, // B 体
      //  "partsnum"  TEXT, // 00
      //   partskey B00
      //  "partsfilename" TEXT, <- 00.png fullpath
      //  partsanimefilebase 体\01km??.png , 後\01m1kt??.png
      //  "disporder" INTEGER, // order by partsnum
      // animationflag true or false
      // animationkey 体00 -> B90

      //)


      // string a = "顔\\[0-9]+a.png"; // "(顔?)(\\.?)([0-9]+)(a.png)"
      //string a = "顔?\\\\([0-9]+)(a.png)";
      //string b = "顔\\00a.png";
      //string c = a.Replace("\\", "/");
      //Regex testrgx = new Regex(a);
      //MatchCollection textres = testrgx.Matches(b);
      //int xxa = textres.Count;
      //// Regex.IsMatch("顔\\00a.png","顔\\\\([0-9]+)a.png")
      //// Regex.IsMatch("顔\\00a.png","顔\\\\[0-9]+a.png")
      //int xxb = 10;
      // \\だとマッチしない？ /,aとか別の文字にするとマッチする
      // ? Regex.IsMatch("\t00a.png","\t[0-9]+a.png") -> true
      // ? Regex.IsMatch("\\00a.png","\\[0-9]+a.png") -> false
      // \\を使うととたんにマッチしなくなる


      // デフォルト　ルールの読み込み
      drawoverlayrules = new Dictionary<string, Regex>();
      drawlocationadjustrules = new Dictionary<string, Regex>();
      drawcolorchangerules = new Dictionary<string, Regex>();

      // ルールを先に解析 -> パーツに紐付けする -> draulesに保存
      parserule(charadir);
      parserule();  // exepath \ setting \ drawrule.txt



      // divisionよりpartsubidを読み込み保存しておく
      // partsubid KK , regexp ,後\[0-9]+.png 
      // フロント側でも使用するなら charasで読み込ませるべき
      // 後ろで読み込みファイルのファイル名とregexpを一致させ、一致したらそのpartsubidをつけてparts tableに保存
      // フロント側でpartsubidを読み込み、そのパーツがdraworderのどの位置にくるのかを判断できるようにする
      subpartsids = new Dictionary<string, string>();
      subpartsidsreg = new Dictionary<string, Regex>();


      string buff;
      DB.Query q = new DB.Query();
      DB.DBRecord rec;
      q.table = "division";
      q.select = "name,value";
      q.orderby = "orderno";
      q.where("division", "partsubid");
      bool ret = charadb.getrecord_noread(q,out rec);
      string partsubid;
      string regstr;
      while (rec.Read() == true)
      {
        partsubid = rec.getstring(0);
        regstr = rec.getstring(1);
        subpartsids[partsubid] = regstr;
        buff = regstr.Replace("\\", "\\\\"); // regexでは\\ではエスケープできず、\\\\としなければならない
        subpartsidsreg[partsubid] = new Regex(buff);
      }






      string dpath;
      string headnumber;
      // string contstr;
      string partsfname;
      string charid;
      int i;
      // パーツ番号"00"を保持する
      List<string> partsno = new List<string>();
      // アニメ　ファイルのファイル数
      Dictionary<string, int> animecounter = new System.Collections.Generic.Dictionary<string, int>();
      // アニメファイルのfnameを保持する
      Dictionary<string, List<string>> animefiles = new Dictionary<string, List<string>>();
      Dictionary<string, string> partsrealfname = new Dictionary<string, string>();
      List<string> files; // filesをsearchするさいの work 変数
      // List<string> partsnumfiles; // パーツの候補となるファイル　アニメファイルを除く
      // charactor (親レコード)を作成する必要がある
      // dlete or insするか　or updateするか？
      string charaname;
      charaname = Utils.Files.getbasename(charadir);
      // string sql;
      // sql = " DELETE FROM charactor WHERE name = '" + charaname + "'";
      // bool fret = charadb.Exec(sql);
      // DB.Query q = 

      charid = charaids[charaname];
      //q.table = "charactor";
      //q.where("charakey", charid);

      rec = new DB.DBRecord();
      rec.table = "charactor";
      rec["name"] = charaname;
      rec["charakey"] = charid;
      rec["chartype"] = "dir";
      rec["directory"] = charadir;
      charadb.InsertAndUpdate(rec,"name");
      rec.clear();
      rec = null;

      // なんかおかしいな、 前のrecordの値を引き継いでいない気がする
      // updateする　sqlite classでrecを読み込み、引数のarg recの値で上書き  その後、dleete insertする

      // charaidを一括して更新するfuncを用意する
      // -> allcharaidupdate

      // hcara れいむ -> パーツ（体、眉、目） -> パーツファイル 体\00.png
      // 中間のパーツがいるかどうか? db divisionsで定義はしているが、、、
      // overlayの描画方法にも関係してくる a,bパターン　顔とか、u(background）とか
      // 結局、パターンを変えないといけない -> pgに埋め込むのはナンセンス -> txtとして保存する
      // 


      int disporder = 1;
      int animeorder = 1;
      string overlayrule = "";

      // パーツ番頭 01.png->01 02.png->02 200,png->200の判定用のregexp
      Regex rgx = new Regex(@"^\d*");
      MatchCollection matches;



      foreach (string pdir in partsdirbyname.Keys)
      {
        
        // char内部の一部のパーツ　体とか、目とか、眉
        // partsのfnameにはフルパスは設定しない

        // ファイル一覧をパーツとパーツ構成アニメに分解する


        dpath = charadir + "\\" + pdir;
        // dpath = pdir;
        // 体\*の全部のファイルを取得
        files = Utils.Files.getFiles(dpath);
        // partsnumfiles = new List<string>();
        // 一度 scan 存在するパーツID 00,01を検索する
        foreach (string f in files)
        {
          if (f.IndexOf(".txt") > 0)
          {
            continue;
          }

          // 体\00.png
          // pngを消す
          // buff = f.Replace(charadir, "");
          // buff = buff.Replace(".png", "");
          // buff = buff.Substring(1); // 先頭の\を削除
          // partsfname = Utils.Files.getfilename(f);  // 00a.pngなど
          // 00mk01.png -> 00 , mk01に分解
          // chara base dirをtable "caractor"に格納するか？ or partsに格納するか
          // 04u1kw14.png
          //if (pdir == "目")
          //{
          //  int xx = 10;
          //  if (f.IndexOf("07-15") > 0)
          //  {
          //    int yy = 10;
          //  }
          //  // -15はオフセット
          //}
          buff = Utils.Files.getbasename(f);

          // 全の場合など200,201のように３桁数値の場合はそれぞれひとつのぱーつとして取り扱う
          // headnumberは３桁数値となるので、先頭２文字だけでは判定できない
          // 先頭から数値であるファイル名をregexpで判定する必要がある
            
          matches = rgx.Matches(buff);
          if (matches.Count == 0)
          {
            // パーツ番号を含むファイルと認識しなかったため、skip
            continue;
          }
          headnumber = matches[0].Value;

          // headnumber = buff.Substring(0, 2); // 00などの先頭２文字

          // やっぱ２回 scanする必要があるな。
          // 体\00をdbに登録する
          // filesにはアニメファイルも含まれているので、初回のみ登録を行う
          if (partsno.Contains(headnumber) == true)
          {
            // 既に登録済みのパーツ番号なのでskip anime処理は次のループで行う
            continue;
          }
          partsfname = Utils.Files.getfilename(f);
          animecounter[headnumber] = 0; // 
          partsno.Add(headnumber);
          // パーツの実ファイル名
          partsrealfname[headnumber] = partsfname; // 00 -> 00.pngを登録
          animefiles[headnumber] = new List<string>(); // anime用のcollectionを準備
          // partsnumfiles.Add(partsfname); // 00.pngをパーツファイル（Noアニメ）として登録

        }
        // さらにanimeがある場合のファイルカウント数を検索しておく
        foreach (string f in files)
        {
          if (f.IndexOf(".txt") > 0)
          {
            continue;
          }
          partsfname = Utils.Files.getfilename(f);

          // 全などで、200.pngなどの数値２桁のみではないファイルはひとつのパーツとして扱い、アニメとしては扱わない
          matches = rgx.Matches(partsfname);
          if (matches.Count == 0)
          {
            // パーツ番号を含むファイルと認識しなかったため、skip
            continue;
          }
          headnumber = matches[0].Value;

          // headnumber = partsfname.Substring(0, 2);
          animecounter[headnumber] += 1; // animecounterは１回目のscanで宣言＆初期化されている
          // animecounterが1ならアニメなし >1ならアニメあり

          //contstr = "";
          //if (partsfname.Length > 2)
          //{
          //  // 00lm01などの00の後ろに続くファイル名
          //  contstr = partsfname.Substring(3);
          //}
          // constrがあればアニメと判断できる
          // partsnoに登録済みの場合はアニメファイルと判断できる
          // string animefile = partsfname;
          // アニメかどうかはanime fileが２つ以上あるときにする
          // if (animecounter[headnumber] == 1)
          // {
          //   continue;
          // }
          animefiles[headnumber].Add(partsfname); // anime fileを登録していく
        }
        // animeがある場合、ファイル名を取得しておく -> tableに保存

        files.Clear();
        files = null;
        i = 1;
        // q = new DB.Query();
        q.clear();
        q.table = "parts";
        q.where("charid", charid);
        q.where("partsid", partsdirbyname[pdir]);
        charadb.delete(q);
        foreach (string pnum in partsno)
        {
          // partsnoでloop
          if (rec != null)
          {
            rec.clear();
          }
          rec = new DB.DBRecord();
          rec.table = "parts";
          // キーが少ないいきがするう "れB01"などがほしくないか？
          rec["charid"] = charid;
          rec["partsid"] = partsdirbyname[pdir]; // B
          rec["partsnum"] = pnum; // 00など
          rec["partsfilename"] = partsrealfname[pnum];
          rec["animationcount"] = animecounter[pnum];
          int animeflag = 0;
          if (animecounter[pnum] > 1)
          {
            animeflag = 1;
          }
          // animation counterをdbに追加
          rec["aimationflag"] = animeflag;
          rec["disporder"] = disporder;
          // ruleidをどう保存するか？ 別col or ,区切り
          buff = pdir + "\\" + partsrealfname[pnum];
          // buff = getruleid(pdir + "\\" + partsrealfname[pnum]);
          overlayrule = getoverlayruleid(buff);
          if (string.IsNullOrEmpty(overlayrule) == true)
          {
            overlayrule = pDefaultOverlayrule;
          }
          rec["overlayrule"] = overlayrule;

          rec["partsadjustrule"] = getpartsadjustruleid(buff);
          rec["colorrule"] = getcolorchangetruleid(buff);

          // partsdir,partsubidを追加
          // partsubidをどうやって判定するの？
          // divisonsでpartsubidをもたせ、regexpを持たせる
          rec["partsdir"] = pdir;
          buff = pdir + "\\" + partsrealfname[pnum];
          //  subpartsids  key value val->regexp
          rec["partsubid"] = getSubpartsid(buff);
          disporder++;
          charadb.Write(rec);
        }
        // animeの保存
        //DB.Query q = new DB.Query();
        // q = null;
        // q = new DB.Query();
        q.clear();
        q.table = "partsanime";
        q.where("charid", charid);
        q.where("partsid", partsdirbyname[pdir]);
        charadb.delete(q);
        foreach (string pnum in animefiles.Keys)
        {
          if (animefiles[pnum].Count == 1)
          {
            // ひとつしか登録していない -> animefileがひとつしかない = アニメがない => db登録しない
            continue;
          }
          i = 0;
          int animecount = animecounter[pnum];
          foreach (string animef in animefiles[pnum])
          {
            rec.clear();
            rec = new DB.DBRecord();
            rec.table = "partsanime";
            // anime partsの保存
            rec["charid"] = charid;
            rec["partsid"] = partsdirbyname[pdir]; // B
            rec["partsnum"] = pnum;
            rec["animecount"] = animecount;
            rec["animeopencount"] = (int)((100 / animecount) * i);
            i++;
            rec["partsfilename"] = animef;
            rec["disporder"] = animeorder;
            animeorder++;
            charadb.Write(rec);
            // animecounter
            // anime 開度
            // anime file
          }
        }
        partsrealfname.Clear();
        // partsrealfname = null;
        foreach (string skey in animefiles.Keys)
        {
          animefiles[skey].Clear();
          // animefiles[skey] = null;
        }
        animefiles.Clear();
        // animefiles = null;
        partsno.Clear();
        animecounter.Clear();
        // 目 07-15.png 補正 -15　Y軸位置調整
        // 07-15a~eまでしかアニメでは使用しない

      }
      partsno.Clear();
      partsno = null;
      animecounter.Clear();
      animecounter = null;



      charadb.Dispose();





      #region
      // "."までを検索
      // "00","01","01km01"などとなるはず
      // 先頭の00の数値２桁でパーツを決定する あくまで ver4fのみ

      // アニメパーツがあるかどうかは次のアニメパーツファイルがあるかどうかで判断する必要がある
      // -> どーしたものか、、
      // filesを２回 scan する必要がある


      // karada m -> 前描画 01m,png -> 前描画
      // 01km01.png ~ 01km05.png = animation



      // ファイル名を解析 00.pngの場合はひとつしかなく、アニメなしと判断
      // 00km01.pngの場合はアニメありと判断
      // 00a,00bなどもある
      // 体\01km01.pngは体\01.pngのアニメの一部
      // 01.pngと01km01.pngを分けて考える

      // これとは別に描画順序を決定するorder指示が必要

      // 体\01km01.png kmは定期アニメーションの指定 拡張アニメーション.lua
      #endregion


      // 全\セット.txtを検索
      return true;
    
    }

    public static string getSubpartsid(string arg)
    {
      bool ret = false;
      string subid = "";
      foreach (KeyValuePair<string, Regex> kvp in subpartsidsreg)
      {
        ret = kvp.Value.IsMatch(arg);
        if (ret == true)
        {
          subid = kvp.Key;
          break;
        }
      }
      return subid;

    }

    public static string getoverlayruleid(string arg)
    {
      string ret = "";
      

      // drawrulesのitemがキャラなし->キャラありになっているので
      // キャラなしの定義が先にヒットしてしまう
      // FABとJ01,CH01は１パーツに同居する
      // 別々のruletypeとしてわけるので、３つのcollectionを持つべき
    
      MatchCollection mch;
      Regex rgx;
      foreach (KeyValuePair<string, Regex> kv in drawoverlayrules)
      {
        
        rgx = kv.Value;
        // mch = rgx.Match(arg);
        mch = rgx.Matches(arg);
        if(mch.Count > 0)
        {
          return kv.Key;
        }
      }

      return ret;
    }

    public static string getpartsadjustruleid(string arg)
    {
      string ret = "";

      MatchCollection mch;
      Regex rgx;
      foreach (KeyValuePair<string, Regex> kv in drawlocationadjustrules)
      {
        rgx = kv.Value;
        // mch = rgx.Match(arg);
        mch = rgx.Matches(arg);
        if (mch.Count > 0)
        {
          return kv.Key;
        }
      }
      return ret;
    }

    public static string getcolorchangetruleid(string arg)
    {
      string ret = "";

      MatchCollection mch;
      Regex rgx;
      foreach (KeyValuePair<string, Regex> kv in drawcolorchangerules)
      {
        rgx = kv.Value;
        // mch = rgx.Match(arg);
        mch = rgx.Matches(arg);
        if (mch.Count > 0)
        {
          return kv.Key;
        }
      }
      return ret;
    }

    //public bool allcharaidupdate(string org, string dst)
    //{
    //  bool fret = true;
    //  // 仮でつけた”れ”のcharaidを一括で更新する

    //  // table charactor

    //  return fret;


    //}

    // キャラの描画順・非表示のルールを解析->global compositordersへ保存
    public static bool parserule(string charadir = "")
    {
      bool fret = false;

      // string orderfile = Appinfo.charabasedir + "\\" + CHAR_PETTERNFILE;
      // キャラ素材 配下に rule.txtがあればそれを使用
      // なければexe配下のsetting\rule.txtを使用 (default rule)

      string buff = "";
      string charaid = "";


      // キャラ配下の 素材設定.txtも考慮が必要
      string rulefile = charadir + "\\" + RULEFILE;
      bool defaultflag = false;
      if (string.IsNullOrEmpty(charadir) == true &&  Utils.Files.exist(rulefile) == false)
      {
        // exe\setting\rule.txtをコピー
        rulefile = Utils.Sysinfo.getCurrentExepath() + "\\" + "setting" + "\\" + RULEFILE;
        defaultflag = true;  // デフォルトとして使用する exe\setting\rule.txt
        charaid = ""; // default ruleではcharaidは空白にする
      } else
      {
        buff = Utils.Files.getbasename(charadir);
        // れいむ,まりさのdirnameがあったとして、対応するcharaidsがあるかどうか？
        // 前画面で設定しているから、ないはずはないのだがが、、
        // 一音、ない場合も考慮して、dirnameの先頭一文字を使う
        charaid = buff.Substring(0, 1);
        if (charaids.ContainsKey(buff) == true)
        {
          charaid = charaids[buff];
        }

      }
      // string orderfile = charadir + "\\" + RULEFILE;
      // charadraworder.txtはキャラ素材直下ではなく、キャラ素材\れいむ　などのキャラdir直下に移動
      // キャラ素材毎時に変化すると思われるため



      // compositorders = new Dictionary<string, CharaDraworder>();
      if (Utils.Files.exist(rulefile) == false)
      {
        return fret;
      }


      string allbuff;
      StreamReader fs = new StreamReader(rulefile, System.Text.Encoding.GetEncoding("utf-8"));
      // "\"が正常に読み込めない encodingの問題では？
      // macでは\は0xc2a5となるため
      allbuff = fs.ReadToEnd();
      fs.Close();
      fs.Dispose();
      fs = null;

      allbuff = File.ReadAllText(rulefile);

      // string[] ary = buff.Split(new string[] { "；" }, StringSplitOptions.RemoveEmptyEntries);
      //string[] ary = buff.Split(new string[] { "\r" }, StringSplitOptions.RemoveEmptyEntries);
      string[] ary = allbuff.Replace("\r\n", "\n").Split(new[] { '\n', '\r' });
      string[] col;
      string ruleid = "";
      int drawruleorderno = 10;
      int orderno = 0;
      // default(charaidなし)とcharaid=rとで別々に呼び出されるためリセットされる
      string pregexp;
      string memo = "";
      // string buff = "";
      int intbuff = 0;
      int condition;
      string dirname;

      DB.Query q = new DB.Query();
      DB.DBRecord rec = new DB.DBRecord();
      string sql = "";

      // 削除をどうするか？
      // dir形式 で nicotalkのver形式のものを削除 ->  再構築

      Logs.write("test dayo---");

      if (defaultflag == true)
      {
        // exe直下の場合、drawruleと緋もづく drawruleitem,drawrulecolortableを削除
        sql = " DELETE from drawruleitem WHERE ruleid IN ";
        sql += " ( SELECT ruleid FROM drawrule WHERE original = 1)";
        charadb.Exec(sql);

        sql = "";
        sql = " DELETE from drawrulecolortable WHERE ruleid IN ";
        sql += " ( SELECT ruleid FROM drawrule WHERE original = 1)";
        charadb.Exec(sql);


        // デフォルトルールを削除
        q.table = "drawrule";
        q.where("original", 1);
        charadb.delete(q);
        // charadb.Exec(q);
        // charadb.delete(q);
        q.clear();

        // drawruleitem,drawrulecolortableより delete drawrule(where default=1)


      } else
      {
        // CharaDraworder cdo;
        // CharaDraworderItem cdoi;
        q.table = "drawrule";
        q.where("charaid", charaid);
        charadb.delete(q);
        q.clear();

        // q = new DB.Query();
        q.table = "drawruleitem";
        q.where("charaid", charaid);
        charadb.delete(q);
        q.clear();

        q.table = "drawrulecolortable";
        q.where("charaid", charaid);
        charadb.delete(q);
        q.clear();


      }


      int i;
      string line;
      bool ret;
      bool multiplyflag = false;
      string ruletype = ""; // orverlayorder or hiddendrawparts
      bool ruleorderderaultflag = false;
      string ruleno;
      string rulecommand; // drawlocadsjct locaition adjustで使用
      string parameter = ""; // coloruleを適用するparameter
      string partsubid = "";
      string overlaycmd = ""; 
      string[] cparams;
      // string ruleid;
      Regex reg = null;
      // drawrulecolortableに保存するitem
      Dictionary<string, string> colortable = new Dictionary<string, string>();
      int j = 1;
      foreach (string l in ary)
      {
        j++;
        if (l == "") continue;
        if (l.Substring(0, 2) == "//")
        {
          memo = l;
          continue;
        }
        if (l.Substring(0, 1) == "[")
        {
          #region rule string parse

          // [overlayorder:FB]
          line = l.Substring(1);
          // defaultかどうかの判断

          col = line.Split(new[] { ']' });
          ruleorderderaultflag = false;
          if (col[1].IndexOf("default") > 0)
          {
            // overlayorderでdefaultが指定されている場合、これを基本として描画順を決定する
            ruleorderderaultflag = true;
          }
          line = col[0];


          // col = line.Replace("]", "").Split(new[] { ':' });
          col = line.Split(new[] { ':' });
          ruleid = col[1];
          //if (ruleid == "EX")
          //{
          //  int zaaa = 10;
          //}
          // cdo = new CharaDraworder();
          // cdo.orderid = orderid;
          // cdo.orderitems = new SortedList<int, CharaDraworderItem>();
          // compositorders[orderid] = cdo;
          ruletype = col[0]; // overlayorder or hiddendrawparts
          #endregion
          continue;
        }
        if (string.IsNullOrEmpty(ruleid) == true)
        {
          continue;
        }
        // cdo = compositorders[orderid];
        if (l.Length > 13 && l.Substring(0, 13) == "patternregexp")
        {
          col = l.Split(new[] { '=' });
          pregexp = col[1];
          // cdo.patternregexp = pregexp;
          rec.clear();
          rec["charaid"] = charaid; // れいむ->r
          rec["ruletype"] = ruletype; // orverlayorder or daraparts
          if (defaultflag == false)
          {
            // exe直下のdrawruleでない場合、rE01のようにキャラIDを先頭につける
            ruleid = charaid + ruleid;
          } 
          rec["ruleid"] = ruleid; // FA,FB
          rec["orderno"] = drawruleorderno; // 10,20
          pregexp = pregexp.Replace("\\", "\\\\"); // regexp上では\\だけだとエスケープとみなす ２つ重ねて\４つつないではじめて"\"と認識する
          rec["regexp"] = pregexp; // 顔\[0-9]+a.png
          rec["memo"] = memo;
          // exe直下のdrawruleの場合、default=1にする
          // exe直下でdefaultflag=trueになるか、dbには直接、影響を与えない
          i = 0;
          if (ruleorderderaultflag == true)
          {
            i = 1;
          }
          rec["defaultflag"] = i;
          i = 0;
          if (defaultflag == true)
          {
            i = 1; // exe直下の設定
          }
          rec["original"] = i;
         

          rec.table = "drawrule";
          charadb.Write(rec);
          if (ruleorderderaultflag == true)
          {
            // 条件に一致しない場合のoverlayorderをdefaualtが指定されているもの
            // この場合 FABに指定する
            pDefaultOverlayrule = ruleid;
          }
          rec.clear();
          drawruleorderno += 10;
          memo = "";
          reg = new Regex(pregexp);
          if (ruletype == "overlayorder")
          {
            drawoverlayrules[ruleid] = reg;
          } else if (ruletype == "drawlocadjust")
          {
            drawlocationadjustrules[ruleid] = reg;
          } else if (ruletype == "drawlocadjust")
          {
            drawcolorchangerules[ruleid] = reg;
          }

          continue;

        }
        line = l;
        i = line.IndexOf("//");
        if (i > 0)
        {
          memo = line.Substring(i);
          line = line.Substring(0, i - 1);
        }
        // ここから実ruleの解析 orverlayorderの場合もあれば、違い場合のあるので注意が必要
        // ruletypeで場合わけが必要 , drawadjuct , colorrule
        if (ruletype == "overlayorder")
        {
          #region overlayorder
          line = line.Trim();
          col = line.Split(new[] { ' ' });
          multiplyflag = false;
          overlaycmd = "";
          if (col.Length >= 2)
          {
            i = line.IndexOf("[multiply]");
            if (i > 0)
            {
              multiplyflag = true;
              // line = line.Substring(0, i - 1);
            }
            overlaycmd = col[1];
            line = col[0];
          }

          col = line.Split(new[] { '=' });
          ruleno = col[0];
          ret = int.TryParse(ruleno, out i);
          if (ret == false)
          {
            continue;
          }
          line = col[1];

          // partsubid 100=BB,体\[0-9]+pngを追加したため修正
          col = line.Split(new[] { ',' });
          partsubid = "";
          if (col.Length > 1)
          {
            partsubid = col[0];
            line = col[1];
          }

          // char[] separator = new char[] { '\\' };
          // col = line.Split(separator);
          // なぜか"\"でのsplitができない
          col = line.Split(new char[] { Path.DirectorySeparatorChar }, StringSplitOptions.None);
          //i = line.IndexOf("\\");
          //col = new string[2];
          //col[0] = line.Substring(0, i);
          //col[1] = line.Substring(i + 1);
          // line = l.Split(new string[] { "=" }, StringSplitOptions.None);
          // 005=後¥*.png を分解して、CharaDraworder,CharaDraworderItemを作成
          // collectionに保存
          //cdoi = new CharaDraworderItem();
          //cdo.orderitems[i] = cdoi;
          //cdoi.orderno = i;
          rec.clear();
          rec["charaid"] = charaid; // れいむ->r
          rec["ruleid"] = ruleid;
          rec["ruleorder"] = i;
          rec["ruleno"] = ruleno; // 同一もありえる。同じ場合、ruleorderの順番に一致したらokとし、後ろはskipする
          rec["dirname"] = col[0];
          rec["partsid"] = partsdirbyname[col[0]];
          rec["partsubid"] = partsubid;
          rec["filepattern"] = line;
          intbuff = 0;
          if (multiplyflag == true)
          {
            intbuff = 1;
          }
          rec["multiplyflag"] = intbuff;
          rec["command"] = overlaycmd;
          rec["memo"] = memo;
          rec.table = "drawruleitem";
          charadb.Write(rec);
          rec.clear();
          #endregion
        } else if (ruletype == "drawlocadjust")
        {
          #region drawlocadjust
          col = line.Split(new[] { ' ' });
          rulecommand = "";
          if (col.Length > 0)
          {
            rulecommand = col[1];
          }
          line = col[0];
          col = line.Split(new[] { '=' });
          ruleno = col[0];
          ret = int.TryParse(ruleno, out i);
          if (ret == false)
          {
            continue;
          }
          line = col[1];

          col = line.Split(new char[] { Path.DirectorySeparatorChar }, StringSplitOptions.None);
          rec.clear();
          rec["charaid"] = charaid; // れいむ->r
          rec["ruleid"] = ruleid;
          rec["ruleorder"] = i;
          rec["ruleno"] = ruleno; // 同一もありえる。同じ場合、ruleorderの順番に一致したらokとし、後ろはskipする
          rec["dirname"] = col[0];
          rec["partsid"] = partsdirbyname[col[0]];
          rec["partsubid"] = partsubid;
          rec["filepattern"] = line;
          rec["command"] = rulecommand;
          rec["memo"] = memo;
          rec.table = "drawruleitem";
          charadb.Write(rec);
          rec.clear();

          #endregion

        } else if (ruletype == "colorrule")
        {
          #region colorrule
          // parameter = "";
          if (line.Substring(0,9) == "parameter" )
          {
            col = line.Split(new[] { '=' });
            parameter = col[1];

            continue;

          }
          col = line.Split(new[] { ' ' });
          line = col[0];
          colortable.Clear();
          for (int ii = 1; ii < col.Length; ii++)
          {
            buff = col[ii];
            cparams = buff.Split(new[] { ':' });
            // drawrulecolortableに保存する brgiht,contrast,hue,saturation
            colortable[cparams[0]] = cparams[1];
          }
          // 010=kamicolor=000,髪\*cu.png bright:80 contrast:115 hue:0]
          // 010=kamicolor=000,髪\*cu.png
          // これがあるので
          col = line.Split(new[] { ',' });
          pregexp = col[1];
          line = col[0];
          col = pregexp.Split(new[] { '\\' });
          dirname = col[0];
          // 010=kamicolor=000
          i = line.IndexOf("=");
          orderno = 0;
          ruleno = "0";
          if (i > 0)
          {
            ruleno = line.Substring(0, i);
          }
          ret = int.TryParse(ruleno, out orderno);
          if (ret == false)
          {
            continue;
          }
          // kamicolor=000
          line = line.Substring(i + 1);
          // regexpを使って数値のみ判定できないか？
          MatchCollection mchs = Regex.Matches(line, parameter + "[=<]+([0-9]+)");
          if (mchs.Count == 0)
          {
            continue;
          }
          buff = mchs[0].Groups[1].Value;
          ret = int.TryParse(buff, out condition);
          if (ret == false)
          {
            continue;
          }
          rec["charaid"] = charaid; // れいむ->r
          rec["ruleid"] = ruleid;
          rec["ruleorder"] = orderno;
          rec["ruleno"] = ruleno;
          rec["dirname"] = dirname;
          rec["partsid"] = partsdirbyname[dirname];
          rec["filepattern"] = pregexp;
          rec["parameter"] = parameter;
          rec["condition"] = condition;
          rec["memo"] = memo;
          rec.table = "drawruleitem";
          charadb.Write(rec);
          rec.clear();

          foreach (KeyValuePair<string, string> kv in colortable)
          {
            rec["colorchangeid"] = ruleid + "_" + ruleno;
            rec["ruleid"] = ruleid;
            rec["ruleno"] = ruleno;
            rec["charaid"] = charaid;
            rec["parameter"] = parameter;
            rec["condition"] = condition;
            rec["coloreffect"] = kv.Key;
            rec["value"] = kv.Value;
            buff = "";
            if (kv.Value.Contains("lua") == true)
            {
              buff = kv.Value;
            }
            rec["luascript"] = buff;
            rec["memo"] = memo;
            rec.table = "drawrulecolortable";
            charadb.Write(rec);
            rec.clear();

          }
          rec.clear();
          // これはdrawrulecolorの定義 drawruleには何をいれるか？


          #endregion
        }


        // キャラ素材 れいむ\素材設定.txtが存在する
        // ここに目ぱち時間などの基本設定が定義されている場合がある
        // キャラ固有の設定
        // これがある場合にはこちらを優先
        // ただし、使えるもののみ。他で定義した方がよいと思われるものは他で取得

      }


      return true;
    }

  }
}
