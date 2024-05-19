using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using System.Text.RegularExpressions;

// drawruleを設定したら、compositeorderとhiddenpartsidが決まる
// drawrule tableのorderid -> ruleのをdictionaryで保存する仕組みが必要
// composite orderと hidden partsを分けるのではなく、同じclassで保管するべきか？


namespace saltstone
{

  // 立ち絵ディレクトリと
  // aviu側への立ちえ設定、字幕設定、
  // 棒読みちゃんへの発声設定
  // これらの情報を保持する
  // TODO ybcharaを検索し、必要なパーツを検索するためのアクセサクラス
  // ぼうよみちゃんへの発声も必要ないかも


  /// <summary>
  /// 個別のキャラを管理するクラス
  /// psd or dirの量対応とする
  /// 音声、立ち絵、略記号を管理する
  /// 略記号はaviutl側でどのキャラの音声・立ち絵を使用するか判断するために使用する
  /// またシナリオファイルの先頭のキャラ指定文字としても使用する
  /// 画面のキャラ定義リストと対応
  /// dbに保存し、どこからでも読み込めるようにしておく
  /// </summary>
  public class Chara
  {

    // public Dictionary<string, string> partdirpair = null;

    public const string CHAR_Cachedir = "cache";

    public string charaid; // れいむとかまりさ  // -> rとかm シナリオファイル上のＩＤ
    public string charakey; // r or mなど １文字
    public string dispname;

    public string charadir;
    public int width;  //  画像の横幅
    public int height; // 画像のたて
    public double aspectrate; // よこにたいするたてのひりつ w300/h400 = 0.75
                              // よこをかければたてがでてほしいんだから、300/400か

    public Image modelpicture;
    public string modelpicture_filename;
    // public static Dictionary<string, string> partsids; // 体->Bの対応表
    

    // 立ち絵関係
    // 立ち絵自体に名前をつけられるべき 神威式とか新とかある
    public string picturename; // 立ち絵名 れいむ or まりさ 
    public string picturefile_representation; // 代表の立ち絵
    // public CharaPictures.enum_CharapictureType charatype;

    // 各パーツを保持するmember
    //　これがキャッシュになってる
    // B -> 00 -> charaparts
    public Dictionary<string, SortedDictionary<string, CharaParts>> partslist;

    // c++ image processへのcmd.txtをこのclassで作成する
    // 出力ファイルをどうするかなどの問題がある
    // ファイル名はchar basedir \ cacheにするか
    // B00F00など
    // draworderが指定されていない場合には、標準のorderを使用する
    // どのパーツが選択されているかをここで持たせるか、、、

    // 描画順序 <- パーツ選択した際にcparts.compositorderをセット 描画順序はパーツに紐づく
    public CharaDraworder pCharaDrawOrder;
    // キャラ素材直下 or dbに存在する描画ルール
    // rFAB -> charadraworder class
    public Dictionary<string, CharaDraworder> compositorder;

    // 目パーツのxz（眉・口の非描画）、x(眉の緋描画)はどうするか？
    // 別ルールになる、、、描画順とは別のlogicで描画される　
    // public string phiddenParts; // 目\??x->眉・口が非描画、目\??z.png->眉が非描画 "ML" or "M"など
    // ChaaraHiddenParts
    
    // public Dictionary<string, string> pHiddenParts;

    // 口パーツによる目・眉の描画位置調整


    // 選択されたパーツ B->00.png charaparts 他は複数もてる
    public Dictionary<string, List<CharaParts>> selectedparts;

    // c++ image exeに渡す引数ファイル　テキスト
    // public string outfname; // memoryの場合、共有メモリに書きだし　どうやって？ 

    // getpartsをスレッド実行するためのlock処理
    public static SMutex pMutexGetparts;
    private const string MUTEX_GetParts = "MUTEXGETPARTS";

    

    // private static SMutex pMutexBacktask;
    // private const string MUTEX_BackTask = "MUTEXBACKTASK";

    

    public void Dispose()
    {
      if (modelpicture != null)
      {
        modelpicture.Dispose();
        modelpicture = null;
      }
      //if (partsids != null)
      //{
      //  partsids.Clear();
      //  partsids = null;
      //}
      if (pCharaDrawOrder != null)
      {
        pCharaDrawOrder.Dispose();
        pCharaDrawOrder = null;
      }
      if (selectedparts != null)
      {
        foreach (List<CharaParts> cp in selectedparts.Values)
        {
          cp.Clear();
        }
        selectedparts.Clear();
        selectedparts = null;
      }
      if (partslist != null)
      {
        partslist.Clear();
        partslist = null;
      }
      if (compositorder != null)
      {
        foreach (CharaDraworder c in compositorder.Values)
        {
          c.Dispose();
        }
        compositorder.Clear();
        compositorder = null;
      }
      pMutexGetparts.Dispose();
      // pMutexBacktask.Dispose();

    }

    // dir or psd 形式
    // file , anime , originalfile
    // charapictureclassを作ったほうがよい
    // なぜなら、立ち絵は複数あり、dir、psdで保存されているから
    // charapictures のようなまとめて管理できるクラスを作るべき？
    // dirのparse、はcharasで行ってる
    // じゃあ、ボイスは？
    // 棒読みちゃんだけじゃない 
    // voice classも再構成するべきだな

    // 略記号 -> 立ち絵、ボイスにアクセスできるようにしたい
    // ようはaviutl側から呼び出しやすいように作ればいい
    // 立ち絵 = 略記号 , パーツ文字 ,frame位置 -> bitmapを合成
    // ボイス = voicesoftで保存してあるwavを参照
    // じゃあ、ボイスはセリフ画面で使いやすいようにすればいい
    // wavは8KHz or 16KHz 16bitPCM 
    // cevio 48kHz おそらく voicroidも
    // 60fps(frame per sec)だから、0.016s = 16.6mill sec
    // aviutlからwavのv　olumeを参照して口の開度を決定する
    // dll側で一括して行いたいので,,,
    // wavファイルをaviutlからdllへ渡す必要がある
    // layerがわかればaviu pluginでwavファイルパスがわかる
    // とりあえず、wavファイルをカスタムオブジェクトで設定してる
    // wavと字幕・立ち絵は同じフレームから開始する前提

    // 立ち絵取得
    // bitmap b = charas["r"]["set:1"].getBitmap(string wavfile, int startmilsec)
    // bitmap b = charas["r"]["set:1"].getBitmap(string wavfile, int frameloc,int totalframe)
    // bitmap b = charas["r"]["set:1"].getBitmap(string wavfile, double location);
    // どれにするかはwavファイルの構造による

    // セリフ画面にてのボイス・txt保存
    // string file = charas["r"].savevoice("ゆっくりれいむだよ",voiceeffect e)
    // ボイスソフトにより発音記号がある
    // さらには、調声をしようとすると、ボイスソフト側で詳細に行う必要がある
    // んで、セリフ画面では保存されたwav,txtを取得して名前を変更・移動し、aviuに貼り付ける？
    // んー。ボイスソフトで保存したテキストとwavをどうひもづけるか？
    // txt/wav両方を保存してくれるものは、windowをfontにして保存したファイルをセリフとして登録する
    // 対応していないものは、txtをvoice windowにはりつけ、保存してもらう
    // -> wavファイルを監視し、新規ファイルのファイル名と一致しているか確認して字幕・音声のセリフとして扱う

    // 立ち絵の管理
    // charapicturesでまとめて行う
    // internal cがかけるんでは？
    // charaクラスで[]メソッドをオーバーライドする
    // charapictureクラスを作る
    // saveserif method を作成
    // アニメの場合、wavfileとdouble locationが必要
    // arg wavfile , double location or else を元にwavを解析
    // 解析結果はキャッシュしておく
    // bitmap b = charas["r]["set:1"].getBitmap()

    // charas class でcharapicture classを保管する
    // これにmethod []を定義して、charapicutre ["set:1"]をcall

    // 













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
      if (pMutexGetparts == null)
      {
        pMutexGetparts = new SMutex(MUTEX_GetParts);
      }
      //if (pMutexBacktask == null)
      //{
      //  // mutexに文字指定は必須かな? 自動化できないか？
      //  pMutexBacktask = new SMutex(MUTEX_BackTask);
      //}


      partslist = new Dictionary<string, SortedDictionary<string, CharaParts>>();
      
      // argは立ち絵名
      // じゃあボイスは？
      // fullpath = arg; // れいむとかまりさ
      charaid = Path.GetFileName(arg);

      // charaidを元にcharakey(symbolstr)を検索
      DB.Query q = new DB.Query(Charas.table_charactor);
      // q.select = "charakey,name,speed,tone,volume";
      q.select = "charakey,name,speed,tone,volume,height,width,modelpicture";
      q.where("name", charaid);

      // charakey = Globals.charadb.getonefield(q);
      // DB.DBRecord rec = Globals.charadb.getrecord(q);
      DB.DBRecord rec = Charas.charadb.getrecord(q);

      // TODO charakey ,nameをどこで設定するか?
      // charactorの定義をどこか別の場所で定義する必要がある？
      charakey = rec.getstring(0);
      dispname = rec.getstring(1);
      // default_speed = rec.getnum(2);
      // default_tone = rec.getnum(3);
      // default_volume = rec.getnum(4);
      height = rec.getnum(5);
      width = rec.getnum(6);
      aspectrate = 1.0 * height / width;
      modelpicture_filename = rec.getstring(7);

      charadir = arg;
      // partdirpair = new Dictionary<string, string>();
      // partsdata = new Dictionary<string, CharaParts>();
      // 画像のたて・よこを取得する


      // STasks.addOrderedTask(5, init_background);


      // このinit_backgroundは modelpictureを読み込むので優先して実行する
      // STasks.createTask(init_background);
      // cdo; // default orderを設定
      // exe内部にtxtを作成し、これから定義を行いたいね
      STasks.addOrderedTask(1,init_background);
      STasks.run();
      

    }

    public void init_background()
    {
      // bool ret = pMutexBacktask.lockmutex();
      modelpicture = null;
      if (Utils.Files.exist(modelpicture_filename) == true)
      {
        modelpicture = Image.FromFile(modelpicture_filename);
      }

      // 縦横サイズ、比率を取得するため、pngファイルを取得 まず全を検索
      if (modelpicture == null)
      {
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
        if (string.IsNullOrEmpty(png) == false)
        {
          modelpicture = Image.FromFile(pngf.First());
          width = modelpicture.Width;
          height = modelpicture.Height;
          aspectrate = 1.0 * height / width;
        }
      }

      // pMutexBacktask.releasemutex();

      #region memo
      // composite orderはdrawruleの読み込み(Charas class)で行うので
      // 外部から設定する -> ここでは何もしない
      //if (compositorder == null)
      //{
      //  compositorder = new Dictionary<string, CharaDraworder>();
      //  compositorder = pRuleoverlayorders[charaid];
      //}
      //pCharaDrawOrder = CharaDraworder.pCharadraworderdefault;
      //if (pHiddenParts == null)
      //{
      //  pHiddenParts = new Dictionary<string, string>();
      //}

      // default orderを設定
      // pCharaDrawOrder = null;
      // pCharaDrawOrder = CharaDraworder.defaultorder;
      // pCharaDrawOrder = CharaDraworder.defaultorder;

      //// composite order と　非表示パーツなどの drawruleを検索
      //DB.Query q = new DB.Query();
      //q.table = "drawrule";
      //q.where("charaid", charakey);
      //q.select = "ruletype,ruleid";
      //q.orderby = "orderno";
      //DB.DBRecord rec = Globals.charadb.getrecord(q);
      //string buff;
      //CharaDraworder cdo;
      //do
      //{
      //  buff = rec.getstring(0);
      //  if (buff == "overlayorder")
      //  {
      //    cdo = new CharaDraworder();
      //    cdo.orderid = rec.getstring(1);
      //    compositorder[cdo.orderid] = cdo;
      //  } else if (buff == "hiddendrawparts")
      //  {
      //    pHiddenParts[rec.getstring(1)] = "";
      //  }
      //} while (rec.Read() == true);
      //rec.clear();
      //rec = null;
      //q.clear();

      // drawruleより描画順序の読み込み
      //CharaDraworderItem cdoi;
      //bool multiplyflag = false;
      //int orderno;
      //foreach (KeyValuePair<string, CharaDraworder> kv in compositorder)
      //{
      //  q.table = "drawruleitem";
      //  q.where("charaid", charakey);
      //  q.where("orderid", kv.Key);
      //  q.select = "orderno,partsid,filepattern,multiplyflag";
      //  q.orderby = "orderno";
      //  rec = Globals.charadb.getrecord(q);
      //  do
      //  {
      //    cdoi = new CharaDraworderItem();
      //    cdo = kv.Value;
      //    orderno = rec.getnum(0);
      //    if (cdo == null)
      //    {
      //      // 通らないはず
      //      cdo = new CharaDraworder();
      //      compositorder[kv.Key] = cdo;
      //    }
      //    cdo.orderitems[orderno] = cdoi;
      //    cdoi.orderno = orderno;
      //    cdoi.partsid = rec.getstring(1);
      //    cdoi.partsdir = Charas.partsdirbykey[cdoi.partsid];
      //    cdoi.filepattern = rec.getstring(2);
      //    multiplyflag = false;
      //    if (rec.getnum(3) == 1)
      //    {
      //      multiplyflag = true;
      //    }

      //  } while (rec.Read() == true);

      //}


      // drawruleより他パーツを非表示にする場合のルールの読み込み
      //string hiddenparts;
      //foreach (string ordernokey in pHiddenParts.Keys)
      //{
      //  q.table = "drawruleitem";
      //  q.where("charaid", charaid);
      //  q.where("orderid", ordernokey);
      //  q.select = "partsid";
      //  q.orderby = "orderno";
      //  rec = Globals.charadb.getrecord(q);
      //  hiddenparts = "";
      //  do
      //  {
      //    hiddenparts += rec.getstring(0);
      //  } while (rec.Read() == true);
      //  pHiddenParts[ordernokey] = hiddenparts;

      //}
      // TODO 可能であれば、不要な領域を削除したい
      // しかしこれはかなり難しそう ピクセルを操作し、最初のたて位置、最後のたていち、最初のよこいち、最後のよこいちを検索し
      // imageをクリッピングする cropする startx y  と crop width heightの４つが必要
      #endregion
    }

    // アスペクト比率にもとづいた縦幅を返す
    public int getaspectheight(int width)
    {
      if (aspectrate == 0)
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
      // DB.DBRecord rec = Globals.charadb.getrecord(q);
      DB.DBRecord rec = Charas.charadb.getrecord(q);
      SortedDictionary<int, string> outpng = new SortedDictionary<int, string>();
      // List<string> outpng = new List<string>();
      do
      {
        outpng.Add(rec.getnum(0), rec.getstring(1));
      } while (rec.Read() == true);
      return outpng;
    }

    // パーツの検索条件を元にsetnumをｄｂから検索して返す
    public List<int> getoutsetnum(string charaid, List<string> where)
    {
      // where にはF01とかの検索用キーが入っている
      DB.Query q = new DB.Query(Charas.table_charaset);
      q.select = "setnum";
      q.orderby = "setnum";
      if (where != null)
      {
        foreach (string s in where)
        {
          q.like("searchkey", s);
        }
      }
      DB.DBRecord rec = Charas.charadb.getrecord(q);
      List<int> ret = new List<int>();
      do
      {
        ret.Add(rec.getnum(0));
      } while (rec.Read() == true);
      return ret;
    }

    public string getpsdtoolkitstr(string charaid, int setnum)
    {
      // psdtoolkitstrって何?

      DB.Query q = new DB.Query(Charas.table_charaset);
      q.select = "partsstr";
      q.where("charaid", charaid);
      q.where("setnum", setnum);
      string buff = "";
      bool ret = Charas.charadb.getonefield(q, out buff);
      if (ret == false)
      {
        return "";
      }
      return buff;
    }



    //private void initpartsids()
    //{
    //  if (partsids == null || partsids.Count == 0)
    //  {
    //    // 体->Bの対応表を読み込む
    //    DB.Query q = new DB.Query(CharaPortraits.table_division);
    //    q.select = "value,name";
    //    q.where("division", "charadir");
    //    if (partsids == null)
    //    {
    //      partsids = new Dictionary<string, string>();
    //    }
    //    partsids.Clear();
    //    DB.DBRecord rec = Globals.charadb.getrecord(q);
    //    do
    //    {
    //      partsids.Add(rec.getstring(0), rec.getstring(1));
    //    } while (rec.Read() == true);
    //  }
    //}

    public bool parse()
    {
      // initpartsids();

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

      string setfpath = charadir + "\\" + CharaPortraits.CONST_partsfile; // セット.txtを解析
      if (Utils.Files.exist(setfpath) == false)
      {
        return false;
      }


      DB.DBRecord rec;
      DB.Query q;

      // table削除
      q = new DB.Query(CharaPortraits.table_partsname);
      q.where("charaid", charaid);
      Charas.charadb.delete(q);

      string alltext = Utils.readAllText(setfpath);
      List<string> lines = new List<string>(alltext.Replace("\r\n", "\n").Split(new[] { '\n', '\r' }));
      string sectionname = "";
      string buff;
      int i;
      string[] ary;
      // rec.clear();
      rec = new DB.DBRecord(CharaPortraits.table_partsname);
      bool ret;
      foreach (string s in lines)
      {
        if (s.Length == 0)
        {
          continue;
        }
        if (s.Substring(0, 2) == "//")
        {
          continue;
        }
        if (s.Substring(0, 4) == "[sec")
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
        // rec["partsid"] = partsids[sectionname];
        rec["partsid"] = Charas.partskeybydir[sectionname];
        rec["filename"] = ary[0];
        rec["name"] = ary[1];
        ret = int.TryParse(buff, out i);
        rec.setint("disporder", i);
        rec["searchkey"] = Charas.partskeybydir[sectionname] + buff; // F00とか
        Charas.charadb.Write(rec);
      }
      return true;
    }


    public bool parsesets()
    {
      // セット.txtの解析を行う 
      // 今はrubyで合成したoutディレクトリがあるが、最終的にはc++の合成ルーチンで画像合成を行い
      // outディレクトリへ出力する

      string setfpath = charadir + "\\" + CharaPortraits.CONST_setfname; // セット.txtを解析
      if (Utils.Files.exist(setfpath) == false)
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
      string outdirbuff = charadir + "\\" + CharaPortraits.CONST_outdir;
      string buff;
      DB.DBRecord rec = new DB.DBRecord(Charas.table_charaset);
      string[] parts; // 顔00-眉08-目00-口02-体00を切り分けたリスト

      // charaidを切り出し
      string charaid = Utils.Files.getfilename(charadir); // 最後のサブディレクトリ名をキャラIDとして使用する

      // charasetのれいむを削除
      DB.Query q = new DB.Query(Charas.table_charaset);
      q.where("charaid", charaid);
      Charas.charadb.delete(q);


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
        rec["psdstr"] = buff;
        rec["searchkey"] = setstrtosearchkey(buff);

        // TODO searchkeyを編集する
        // 顔00目00口06眉04 -> F00E00L06Y04に変更
        Charas.charadb.Write(rec);
      }
      return true;
    }

    private string setstrtosearchkey(string setstr)
    {
      string buff = setstr;
      foreach (KeyValuePair<string, string> p in Charas.partskeybydir)
      {
        buff = buff.Replace(p.Key, p.Value);
      }

      return buff;

    }

    public SortedDictionary<string, CharaParts> getparts(string partskey)
    {
      // initpartsids();
      // partsname は体とか顔とか
      // string partskey = partsids[partsname];
      
      // bkのtasksが終了していないのに、getpartsがcallされている


      // partskeyから体を取得
      string subdir = Charas.partsdirbykey[partskey];
      string bdir = charadir + "\\" + subdir;
      string fname = "";

      //   bool fret = false;
      // bool ret;

      SortedDictionary<string, CharaParts> buff; // = new SortedDictionary<string, CharaParts>();
      if (partslist.ContainsKey(partskey) == true)
      {
        // おそらく、dbにmがあるが、キャラ素材dirが存在しないのでnullになる
        return partslist[partskey];
      }
      // mutexでlock
      bool ret = pMutexGetparts.lockmutex();

      if (ret == false)
      {
        return null;
      }
      // ここでlockをかけても ruleoverlayが読み込みされない時がある？
      // どーしたものか、、、優先順位をつければいいんだが、、、
      // どーやって優先順位をつける？
      //ret = Charas.pMutexBacktask.lockmutex(SMutex.enum_mutexwaitmode.waitever);
      //if (ret == false)
      //{
      //  return null;
      //}
      //ret = pMutexBacktask.lockmutex(SMutex.enum_mutexwaitmode.waitever);


      // buff = 00 -> charaparts (partsnum -> キャラパーツ
      buff = new SortedDictionary<string, CharaParts>();
      partslist[partskey] = buff;
      DB.Query q = new DB.Query(CharaPortraits.table_partsname);
      // q.where("charid", charaid);
      q.where("charid", charakey);
      q.where("partsid", partskey);
      q.select = "partsnum,partsfilename,animationcount,aimationflag,memo,partsfilename,overlayrule,partsubid,partsdir";
      DB.DBRecord rec = Charas.charadb.getrecord(q);
      if (rec == null)
      {
        
        pMutexGetparts.releasemutex();
        // pMutexBacktask.releasemutex();
        Charas.pMutexBacktask.releasemutex();

        return null;
      }
      CharaParts cp;
      bool animeflag;
      do
      {
        cp = new CharaParts();
        cp.partsid = partskey;
        cp.partsubid = rec.getstring(7);
        cp.partsnum = rec.getstring(0);
        animeflag = false; 
        if (rec.getnum(3) == 1)
        {
          animeflag = true;
        }
        cp.animeflag = animeflag;
        cp.animecount = rec.getnum(2);
        cp.memo = rec.getstring(4);
        fname = rec.getstring(5);
        cp.shortfilename = rec.getstring(8) + "\\" + fname;
        cp.filename = bdir + "\\" + fname;
        cp.drawruleid = rec.getstring(6);
        ret = cp.getanimefiles();

        buff[rec.getstring(0)] = cp;
        // 保存していない、、、
        //       partslist[partskey] = buff;で保存している


        #region memo

        // パーツへのdrawrleの適合を、ルール読み込み後ろにすればよい

        //cp.compositorder = pCharaDrawOrder;
        //if (string.IsNullOrEmpty(cp.drawruleid) != true)
        //{
        //  if (compositorder.ContainsKey(cp.drawruleid))
        //  {
        //    cp.compositorder = compositorder[cp.drawruleid];
        //  } else
        //  {
        //    // for debug nullの場合がある
        //    // 場所が悪い、処理順が悪い、ロックが悪い
        //    // charas -> initbacktask
        //    // chara -> init_background
        //    // これが終了していないと難しい
        //    if (Globals.charadb == null)
        //    {
        //      int wxxa = 10;
        //    }
        //    if (Globals.charas.pRuleoverlayorders == null)
        //    {
        //      int wxxb = 10;
        //    }
        //    if (Globals.charas.pRuleoverlayorders["default"] == null)
        //    {
        //      int wxxb = 10;
        //    }
        //    cp.compositorder = Globals.charas.pRuleoverlayorders["default"][cp.drawruleid];
        //  }
        //}
        #endregion
      } while (rec.Read() == true);

      // pMutexBacktask.releasemutex();
      pMutexGetparts.releasemutex();
      // Tasks.createTask(pFunc_partsCheckrule);
      Charas.pMutexBacktask.releasemutex();

      return buff;
    }

    public bool setpartsDrawrule(string partsid)
    {
      bool fret = false;

      foreach (CharaParts cp in partslist[partsid].Values)
      {
        cp.compositorder = pCharaDrawOrder;
        if (string.IsNullOrEmpty(cp.drawruleid) != true)
        {
          if (compositorder.ContainsKey(cp.drawruleid))
          {
            cp.compositorder = compositorder[cp.drawruleid];
          } else
          {
            // for debug nullの場合がある
            // 場所が悪い、処理順が悪い、ロックが悪い
            // charas -> initbacktask
            // chara -> init_background
            // これが終了していないと難しい
            //if (Globals.charadb == null)
            //{
            //  int wxxa = 10;
            //}
            //if (Globals.charas.pRuleoverlayorders == null)
            //{
            //  int wxxb = 10;
            //}
            //if (Globals.charas.pRuleoverlayorders["default"] == null)
            //{
            //  int wxxb = 10;
            //}
            // charas
            // cp.compositorder = Globals.charas.pRuleoverlayorders["default"][cp.drawruleid];
            cp.compositorder = CharaGlobals.charas.pRuleoverlayorders["default"][cp.drawruleid];
          }
        }

      }
      // drawruleが終わっているかどうかをどうやって判断するの？
      // mutexでは限界がありそーだなー
      // semaphoreか？
      fret = true;
      return fret;

    }


    // partsをloopし、どのルールが一致するのかを判定し charaparts classへ保存
    public void pFunc_partsCheckrule()
    {
      // うーん。時間がかかりすぎる気がする

      bool ret;
      foreach (SortedDictionary<string, CharaParts> parts in partslist.Values)
      {
        foreach (CharaParts cp in parts.Values)
        {
          // 描画順(非表示を含む)、目・口の位置調整、色変える
          foreach (CharaDraworder cd in compositorder.Values)
          {
            ret = cp.checkrule(cd);
            if (ret == true)
            {
              break;
            }
          }
        }
      }
    }

    public string getPNGfile(string partsid)
    {
      string fname = "";
      if (partsid == null)
      {
        return fname;
      }
      if (partsid.Length == 0)
      {
        return fname;
      }
      if (partsid.Substring(0, 3) == "set")
      {
        int setnum = Utils.toint(partsid.Substring(4));
        if (setnum == Utils.INT_NOPARSE)
        {
          return fname;
        }
        // whereはcharakeyではなく、まりさとか
        DB.Query q = new DB.Query("charaset");
        q.select = "filename";
        q.where("charaid", dispname);
        q.where("setnum", setnum);
        fname = Charas.charadb.getonefield(q);
      }

      return fname;
    }

    public bool setSelectedParts(CharaParts arg)
    {
      bool fret = false;

      if (selectedparts == null)
      {
        selectedparts = new Dictionary<string, List<CharaParts>>();
      }
      List<CharaParts> plist;
      if (selectedparts.ContainsKey(arg.partsid) == false)
      {
        plist = new List<CharaParts>();
        // selectedparts[arg.partsid] = plist;
        selectedparts[arg.partsubid] = plist;
      } else
      {
        plist = selectedparts[arg.partsid];
      }
      // 複数パーツは考慮していない table divisionに定義をもたせ　他　は複数もてるようにする
      plist.Clear();
      plist.Add(arg);
      if (arg.compositorder != null)
      {
        pCharaDrawOrder = arg.compositorder;
      }

      fret = true;
      return fret;
    }

    private static string mergeargcmd;

    public bool makemergecmd(string partsstr, out string outpctname, out string cmdarg)
    {
      bool fret = false;

      if (string.IsNullOrEmpty(mergeargcmd) == true)
      {
        string exepath = Utils.Sysinfo.getCurrentExepath();
        mergeargcmd = exepath + "mergepctcmd.txt";
      }
      outpctname = "";
      cmdarg = "";

      // partsstr B00E00から画像ファイルの合成を行う
      // K00u,HCU00,BU00, いや、これじゃだめだな
      // B00F00E00Y00L00K00O00 
      // baskの場合、複数パーツの選択が必要、、、
      // どうする？ K01K02とか書く？ 記述できる前提で組み込む
      Dictionary<string, List<string>> targetparts = new Dictionary<string, List<string>>();
      // B -> 00
      List<string> partslistidx = new List<string>();
      partslistidx.Add("B");
      partslistidx.Add("F");
      partslistidx.Add("E");
      partslistidx.Add("Y"); // 眉
      partslistidx.Add("L"); // Lip 口
      partslistidx.Add("K"); // 後ろ bacK
      partslistidx.Add("O"); // 他
      string buff;
      foreach (string ps in partslistidx)
      {
        buff = ps + "[0-9][0-9]";
        Match m = Regex.Match(partsstr, buff);
        if (m.Success == true)
        {
          if (targetparts[ps] == null)
          {
            targetparts[ps] = new List<string>();
          }
          targetparts[ps].Add(m.Value.Substring(1)); // B00-> 00
        }
      }
      // charapartsを検索し、subidを取得しlocal dicを構成
      // partssubidでのdicがほしい
      // わざわざpartsをlistにしているのは、
      // 後ろなどのように複数のパーツを選択する場合があるため
      Dictionary<string, List<CharaParts>> targets = new Dictionary<string, List<CharaParts>>();
      CharaParts p;
      foreach (KeyValuePair<string,List<string>> kv in targetparts)
      {
        p = null;
        // keyに対して00 partsnumを検索し、合成するpartslistに組み込む
        foreach (string pinumst in kv.Value)
        {
          p = partslist[kv.Key][pinumst];
          if(targets[kv.Key] == null)
          {
            targets[kv.Key] = new List<CharaParts>();
          }
          targets[p.partsubid].Add(p);
        }
      }



      // CharaParts a = partslist["B"]["00"];
      List<CharaParts> plist;
      string fname = "";
      string outfname = ""; // 出力するpngファイル
      List<string> mergefiles = new List<string>();
      foreach (CharaDraworderItem porder in pCharaDrawOrder.orderitems.Values)
      {
        if (targetparts.ContainsKey(porder.partsubid) == false)
        {
          continue;
        }
        plist = selectedparts[porder.partsubid];
        foreach (CharaParts cparts in plist)
        {
          fname = cparts.filename;
          if (porder.multiplyflag == true)
          {
            fname += " [multiply]";
          }
          // レイヤーによりalpha 0.25にする場合がある（上書きする髪など）
          if (string.IsNullOrEmpty(porder.command) == false)
          {
            fname += " " + porder.command;
          }
          mergefiles.Add(fname);
          outfname += porder.partsid + cparts.partsnum;
        }
      }
      outfname = charadir + "\\" + Chara.CHAR_Cachedir + "\\" + outfname + ".png";
      outpctname = outfname;
      fret = true;
      return fret;

    }


    /// <summary>
    /// 画像合成のc++ exeに引き渡すincmd.txtを作成
    /// </summary>
    /// <param name="outpctname"></param>
    /// <param name="cmdarg"></param>
    /// <returns></returns>
    public bool makemergecmd(out string outpctname, out string cmdarg)
    {
      bool fret = false;

      if (string.IsNullOrEmpty(mergeargcmd) == true)
      {
        string exepath = Utils.Sysinfo.getCurrentExepath();
        mergeargcmd = exepath + "mergepctcmd.txt";
      }

      // cmdfile = "";
      // outをどうするか？
      // incmdのfpatをどうするか？
      // 最終的には共有メモリ、セマフォ、mutexでメモリ上で引数受け渡しを行う
      // incmdはc++の独自仕様だから、、、formで定義する必要はない
      outpctname = "";
      cmdarg = mergeargcmd;

      // fpathに対し、c++ merge処理を実行するためのincmd.txtを作成する
      // charas.charaにアクセスしたいが、、、
      StreamWriter fs = null;
      string buff;
      string fname;
      string outfname = ""; // 出力するpngファイル
      List<CharaParts> plist;
      List<string> mergefiles = new List<string>();

      //string hiddenparts = "";
      //foreach (string hidpart in pHiddenParts.Values)
      //{
      //  hiddenparts += hidpart;
      //}


      // 実行時には設定されているが pCharaorderがnull errorになる
      foreach (CharaDraworderItem porder in pCharaDrawOrder.orderitems.Values)
      {
        // 現在選択中のパーツの種類 
        //         if (selectedparts.ContainsKey(porder.partsid) == false)
        if (selectedparts.ContainsKey(porder.partsubid) == false)
        {
          continue;
          // 未選択のパーツがあっても合成処理は続行する
        }
        //phiddenParts
        // 非描パーツをスキップ
        //if (hiddenparts.IndexOf(porder.partsid) > 0)
        //{
        //  continue;
        //}

        // file patternをfilenameに変換する

        // 改変した最終版のcomposite order(overlayorder,draworder)にはBでも複数のパーツが存在する
        // 00u.png 00.pnb 00b.png , 00w.png
        // これに一致しているパーツかどうかを判断する必要がある
        // -> dbに保存しておきたいねー partsubidとかつくるか？


        // partsid -> filenameにする
        plist = selectedparts[porder.partsubid];
        foreach (CharaParts cparts in plist)
        {
          fname = cparts.filename;
          if (porder.multiplyflag == true)
          {
            fname += " [multiply]";
          }
          // レイヤーによりalpha 0.25にする場合がある（上書きする髪など）
          if (string.IsNullOrEmpty(porder.command) == false)
          {
            fname += " " + porder.command;
          }
          mergefiles.Add(fname);
          outfname += porder.partsid + cparts.partsnum;
        }
      }
      outfname = charadir + "\\" + Chara.CHAR_Cachedir + "\\" + outfname + ".png";
      outpctname = outfname;
      try
      {
        fs = new StreamWriter(mergeargcmd);
        // out fileはどう編集するか？
        buff = "out=" + outfname;
        fs.WriteLine(buff);
        buff = "size=" + width.ToString() + "x" + height.ToString();
        fs.WriteLine(buff);
        buff = "base=" + charadir;
        fs.WriteLine(buff);
        buff = "picturelist";
        fs.WriteLine(buff);
        foreach (string f in mergefiles)
        {
          // multiplyの指定を追加する
          buff = f.Replace(charadir, "").Substring(1);
          fs.WriteLine(buff);
        }
        fs.Close();

        // charにアクセスできるが、画面で選択されたパーツの指定が必要なので、
        // ここではchara partsにはアクセスしない

      }
      catch (Exception e)
      {
        Logs.write(e);
        return fret;
      }
      finally
      {
        if (fs != null)
        {
          fs.Dispose();
        }
        fs = null;
      }
      // cmdfile = cmdf;

      return true;
    }


    /// <summary>
    /// 立ち絵を合成する
    /// </summary>
    /// <param name="mergepng"></param>
    /// <returns></returns>
    public bool getmergefile(out string mergepng)
    {
      bool fret = false;
      mergepng = "";

      string fname = "";
      string cmdarg = "";
      bool ret = makemergecmd(out fname, out cmdarg);
      if (ret == false)
      {
        return fret;
      }
      ret = Utils.Files.exist(fname);
      if (ret == true)
      {
        mergepng = fname;
        return true;
      }




      // string exef = Appinfo.exeimageproces + " " + fname;
      string exef = Appinfo.exeimageprocess;
      // ret = Utils.exeshellcmd(exef, cmdf);

      // asyncで実行すると、ファイル作成が完了しない場合がある、、、、
      // ただし、debg modeの場合は実行中のままになる
      // 最終的には実行しっぱなしになる
      // 作成終了をどう判定するか？

      // mutexで終了を通知 or ファイルが作成されるまでループ

      ret = Utils.runexec(exef, cmdarg, Utils.enum_runmode.start_async);
      if (ret == false)
      {
        // 作成失敗
        return fret;
      }
      ret = false;
      int i = 0;
      do
      {
        ret = Utils.Files.exist(fname);
        i++;
        Utils.sleep(20);
        if (i > 10)
        {
          break;
        }
      } while (ret == false);
      if (ret == false)
      {
        return ret;
      }

      mergepng = fname;
      return true;
    }


  }
}



