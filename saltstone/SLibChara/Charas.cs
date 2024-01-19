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

  /// <summary>
  /// キャラ定義（音声、立ち絵、略記号、名前）をすべて管理する
  /// charactor defineとする
  /// charadefineとしてjsonで保存し、webシステムで管理できるようにする
  /// </summary>
  public class Charas
  {
    // Charaでボイス設定を管理している
    // 立ち絵も管理してる





    // 別名やお気に入りはテキストファイルで管理 dbで管理すると編集画面が必要になるから

    // public static Sqlite db; // キャラ用のdb キャラ素材dir \ ybchara.db
    // staticではdisposeできない 
    // 使うところでdbを呼び出す必要がある
    // だが、他から参照できない staticにしたいなー

    // singletonにすれば問題なさそう
    // https://qiita.com/haniwo820/items/ba0ab725c25673c20383
    // https://qiita.com/rohinomiya/items/6bca22211d1bddf581c4
    private static Charas _instance;

    private static Charas GetInstance()
    {
      if (_instance == null)
      {
        _instance = new Charas();
      }
      return _instance;
    }

    #region const
    // public const string charadb = "chara.db";
    // public const string setfile = @"全\セット.txt";
    // public const string outdir = "out";
    // public const string envsetting = "chardir"; // iniファイルに書き込まれるキャラ素材ディレクトリのpath
    // public const string charatable = ""

    // セット.txt outディレクトリ内のpngを含む
    // id , charaid(れいむ) , setnum(int 123など) , partsstr(顔00-眉04-目00-口06-体00) , filename(out/*.png)
    public const string table_charaset = "charaset";

    // れいむとかまりさ 
    // セット.txtはcharasetで管理、ここではお気に入りやセット.txtを管理するための親テーブルになる
    public const string table_charactor = "charactor";

    public string charadbfullpath;

    // table divisin B -> 体
    public static Dictionary<string, string> partsdirbykey;
    // table divison 体 -> B
    public static Dictionary<string, string> partskeybydir;

    // init backtaskの終了を判定するためのmutex
    public static SMutex pMutexBacktask;
    private const string MUTEX_Backtask = "CharasMUTEXBacktask";


    #endregion

    // 他パーツでもアニメするものとしないものとがある
    // しかも複数選択できるように進もうとしているみたいだ

    // public DB.Sqlite db;

    // formへメッセージなどを表示するためのobj
    // libraryMessage mainformmessage;

    // れいむ or まりさ -> Chara
    public Dictionary<string, Chara> chara = null;
    // r or m -> Chara
    public Dictionary<string, Chara> charabyid = null;

    // drawruleの読み込み -> 保存
    // overlayorder, drawlocadjust,coloruleを別々に保存するか？ 一緒くたに保存するか？
    // overleyorderをまとめて管理 default=1が優先 charaid指定が２番目 charaid=spaceが３番目
    // public Dictionary<string, CharaDraworder> pRuleoverlayorders;
    // r->dict ruleid -> charadraworder
    public Dictionary<string,Dictionary<string, CharaDraworder>> pRuleoverlayorders;
    // public static CharaDraworder pDefaultoverlayorder; // デフォルトで使用するもの
    // default orderはcharadraworderに統合

    // 他パーツの位置調整 目\01-15.png -> 口、眉:y-15(上へずらす)
    // J01 -> class
    // public Dictionary<string, CharaLocationAdjust> pRuleLocationAdjusts;
    // r->J01->class
    public Dictionary<string, Dictionary<string, CharaLocationAdjust>> pRuleLocationAdjusts;

    // 色変え可能なパーツ定義 体\??bu.pngが存在しない場合の??u.pngの色調補正も含む
    // -> bu.pngがない場合、u.pngを色調補正して代用する
    // CH01 -> class
    // public Dictionary<string, CharaColoChange> pRuleColorChanges;
    // r->CH01-> class
    public Dictionary<string, Dictionary<string, CharaColoChange>> pRuleColorChanges;
    // defaultの場合,charaid=defaultにする

    // global.charadbをここにも保持
    public static DB.Sqlite charadb;


    // このセマフォはdararule readの後ろにpartsへのrule適用、他パーツの読み込み
    // を実行するために存在する
    // 考え方をあらためて、taskをlistにし、優先順位毎に実行できるようにすればよいのでは？
    //public SSemaphore drawrulereaded;
    //public const string SEMAPHORE_Drawrulereaded = "SEMAPHORE_Drawrulereaded";

    public void Dispose()
    {
      if (charabyid != null)
      {
        charabyid.Clear();
        charabyid = null;
      }
      if (partsdirbykey != null)
      {
        partsdirbykey.Clear();
        partsdirbykey = null;
      }
      if (pRuleLocationAdjusts != null)
      {
        foreach (Dictionary<string, CharaLocationAdjust> rlds in pRuleLocationAdjusts.Values)
        {
          foreach(CharaLocationAdjust ld in rlds.Values)
          ld.Dispose();
        }
        pRuleLocationAdjusts.Clear();
        pRuleLocationAdjusts = null;
      }
      if (pRuleColorChanges != null)
      {
        foreach (Dictionary<string, CharaColoChange> crcs in pRuleColorChanges.Values)
        {
          foreach (CharaColoChange crc in crcs.Values)
          {
            crc.Dispose();
          }
        }
        pRuleColorChanges.Clear();
        pRuleColorChanges = null;
      }

      if (pMutexBacktask != null)
      {
        pMutexBacktask.releasemutex();
        pMutexBacktask.Dispose();
        pMutexBacktask = null;
      }


      if (chara != null)
      {
        foreach (Chara c in chara.Values)
        {
          c.Dispose();
        }
        chara.Clear();
        chara = null;
      }

      charadb = null;

    }


    public Charas()
    {
      // ybchara.dbを検索し、キャラの名前を取得
      // charaに展開しておいて、名前の一覧を取得できるようにする
      // favorite.txtも、パーツの別名定義であるchara.txtもすべてdbに保存されているものとする
      // 解析はybhcara.exe側で行う

      chara = new Dictionary<string, Chara>();
      charabyid = new Dictionary<string, Chara>();

      // charadbfullpath = Globals.envini[Charas.envsetting] + "\\" + Charas.charadb;
      //charadbfullpath = Globals.envini[Charas.envsetting] + "\\" + Charas.charadb;
      //charadbfullpath = Globals.envini[PGInifile.INI_CharaDB];
      charadbfullpath = Appinfo.charadbfname;

      pMutexBacktask = new SMutex(MUTEX_Backtask);
      // drawrulereaded = new SSemaphore(SEMAPHORE_Drawrulereaded);
      // initで1 -> １個のtaskが動作できる
      // drawrulereaded.waitOne(); // 0にし、だれも使用できないようにする


      //partsdirbykey
      if (partsdirbykey == null)
      {
        partsdirbykey = new Dictionary<string, string>();
      }
      if (partskeybydir == null)
      {
        partskeybydir = new Dictionary<string, string>();


      }

      DB.Sqlite db = new DB.Sqlite(charadbfullpath);
      // charadb = db;
      charadb = db;

      DB.Query q;

      DB.DBRecord rec;
      string buff;



      q = null;
      q = new DB.Query();
      q.table = "division";
      q.select = "name,value";
      rec = db.getrecord(q);
      string dkey;
      // string buff;
      do
      {
        dkey = rec.getstring(0);
        buff = rec.getstring(1);
        partsdirbykey[dkey] = buff;
        partskeybydir[buff] = dkey;
      } while (rec.Read() == true);
      q = null;
      rec.clear();


      q = new DB.Query(Charas.table_charactor);
      // q.select = "name,directory,charakey,voice";
      q.select = "name,directory,charakey,voice,speed,tone,volume";

      rec = db.getrecord(q);
      string charadir;
#if USEVOICE
      Voice voice;
#endif
      string charakey; // rとかmとかの略記号
      string voicename; // Bouyomi,女性２など
      Chara c;
      string[] cols;
      do
      {
        buff = rec.getstring(0);

        charadir = rec.getstring(1); // fullpath
        // charaのconstructが先にくるので、rule.txtの読み込みがすんでおらず
        // この時点ではdfaulrorderはdb readされていない
        c = new Chara(charadir);
        if (chara.ContainsKey(buff))
        {
          continue;
        }
        chara.Add(buff, c); // まりさ -> Chara obj
        charakey = rec.getstring(2);
        charabyid.Add(charakey, c);
        voicename = rec.getstring(3); // m -> Chara obj
        // ex  Bouyomi,女性２
        cols = voicename.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
#if USEVOICE
        if (cols[0] == Voice.DefineVoiceSoft_Bouyomi)
        {
          // new voiceの中でvoicelistに登録している
          voice = new Voice(charakey, cols[1]);
          voice.dispname = buff;
          voice.speed = rec.getnum(4);
          voice.tone = rec.getnum(5);
          voice.volume = rec.getnum(6);
        }
#endif

      } while (rec.Read() == true);
      // dbと実際のディレクトリ構造が一致しているかどうかのチェックが必要
      // ybchara側で処理する
      // db.Dispose();
      // db = null;



    }

    public void init_background()
    {
      Utils.sleep(10);
      // キャラ定義に含まれている or exedirにある描画順を解析
      // taskの状況によりcompositeorders collectionが初期化されていない <- 以下の処理が実行されていない

      // キャラの描画ルールを読み込み
      // parsdraweorder();
      // initbacktask();
      // 別タスクにすると、変数への格納がうまくいかない？
      // fromのgetpartsとどちらが先に動くかが変化してしまうため
      // 解消するためには、同期処理を行う必要がある
      // charasにパーツ読み込み処理が終了しているかどうか判定し
      // mutexやセマフォでロックする仕組みが必要
      // STasks.createTask(initbacktask);
      STasks.addOrderedTask(20, initbacktask_readdrarrule);
      // ルールの読み込み、compositeの適用、他パーツの読み込み
      // を分割してtask登録し、順番に実行する

      // initbacktask_setcompositeorder
      STasks.addOrderedTask(30, initbacktask_setcompositeorder);
      // initbacktask_loadotherparts
      // global.charasが設定されていないためエラーとなる
      STasks.addOrderedTask(40, initbacktask_loadotherparts);
      // ここで登録したtaskが実行されない?
      STasks.run();
    }

    // drawruleの読み込み
    // body picutreの読み込み
    // そのほかのパーツの読み込み


    private void initbacktask_readdrarrule()
    {

      // Utils.sleep(500);
      // mutex lockは不要か？
      // 複数回実行されている

      // init back処理が終了するまでlockする
      // pMutexBacktask.lockmutex();

      // drawruleの読み込み
      DB.Query q = new DB.Query();

      //      public Dictionary<string, CharaDraworder> pRuleoverlayorders;
      //public CharaDraworder pDefaultoverlayorder; // デフォルトで使用するもの
      q.table = "drawrule";
      q.select = "ruleid,charaid,defaultflag,regexp,memo";
      q.orderby = "charaid,ruleid,orderno";
      q.where("ruletype", "overlayorder");
      DB.DBRecord rec;
      bool ret = charadb.getrecord_noread(q, out rec);
      if (ret == false)
      {
        pMutexBacktask.releasemutex();
        return;

      }
      q.clear();
      string charaid = "";
      string ruleid = "";
      string regex = "";
      string memo = "";
      int defaultflag = 0;
      CharaDraworder cdo;
      CharaDraworderItem cdoi;
      pRuleoverlayorders = new Dictionary<string, Dictionary<string, CharaDraworder>>();
      Chara.pMutexGetparts.lockmutex();
      while (rec.Read() == true)
      {
        ruleid = rec.getstring(0);
        charaid = rec.getstring(1);
        defaultflag = rec.getnum(2);
        regex = rec.getstring(3);
        memo = rec.getstring(4);
        if (string.IsNullOrEmpty(charaid) == true)
        {
          charaid = "default";
        }
        if (pRuleoverlayorders.ContainsKey(charaid) == false)
        {
          pRuleoverlayorders[charaid] = new Dictionary<string, CharaDraworder>();
        }
        cdo = new CharaDraworder();
        pRuleoverlayorders[charaid][ruleid] = cdo;
        cdo.ruleid = ruleid;
        cdo.patternregexp = regex;
        cdo.memo = memo;
        // drawruleitemのread
        cdo.readDraworderItems();
        if (defaultflag == 1)
        {
          // pDefaultoverlayorder = cdo;
          CharaDraworder.pCharadraworderdefault = cdo;
        }
      }
      Chara.pMutexGetparts.releasemutex();

      // 1に戻し、誰でもアクセスできるようにする
      //drawrulereaded.release();

    }
    private void initbacktask_setcompositeorder()
    {
      // chara classに対してoverlayorderを適用
      // chara->getpartsが先にきてしまう
      // mutex lockするか？
      foreach (Chara cc in chara.Values)
      {
        // composite orderのderault ruleの設定が必要
        cc.pCharaDrawOrder = CharaDraworder.pCharadraworderdefault;
        if (pRuleoverlayorders.ContainsKey(cc.charakey) == true)
        {
          cc.compositorder = pRuleoverlayorders[cc.charakey];
        }
      }
      // semaphoreでdrawruleを使用するように変更するので、このmutexは師匠しなくなる？
      // ruleが読み込まれたかどうか
      // 他のpartsreadが実行されていないか
      // Chara.pMutexGetparts.releasemutex();

      //  pRuleoverlayorders = new Dictionary<string, Dictionary<string, CharaDraworder>>();


      //// 他パーツの位置調整 目\01-15.png -> 口、眉:y-15(上へずらす)
      //// J01 -> class
      //public Dictionary<string, CharaLocationAdjust> pRuleLocationAdjusts;
    }

    private void initbacktask_loadotherparts()
    {

      //// 色変え可能なパーツ定義 体\??bu.pngが存在しない場合の??u.pngの色調補正も含む
      //// -> bu.pngがない場合、u.pngを色調補正して代用する
      //// CH01 -> class
      //public Dictionary<string, CharaColoChange> pRuleColorChanges;
      // キャラ毎のパーツを読み込み
      foreach (KeyValuePair<string, Chara> kv in chara)
      {
        foreach (string partsid in partsdirbykey.Keys)
        {
          kv.Value.getparts(partsid);
          kv.Value.setpartsDrawrule(partsid);
        }
      }
      // どうも、ここで例外が発生する
      // 別タスクでlockしたmutexを別タスクでreleasするとエラーとなるもよう

      pMutexBacktask.releasemutex();



    }


    ~Charas()
    {
      if (chara != null)
      {
        // TODO charaに登録されているCharaクラスのインスタンスもクリアしないといけないかも
        chara.Clear();
        chara = null;
      }
      Dispose();
    }

    public Chara this[string arg] {
      get {
        // return chara[arg];
        return charabyid[arg];
      }

    }

    /*
    public void setMainFormMessage(libraryMessage arg)
    {
      this.mainformmessage = arg;
    }
    */

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

    /*
    private void pf_message(string arg)
    {
      if (mainformmessage == null)
      {
        return;
      }
      mainformmessage.showMessage(arg);
    }
    */

    /*
    public bool addChara_portrait(string filename)
    {
      string buff = "";
      addChara_portrait(filename, out buff);
      return true;

    }

    */




    /// <summary>
    /// enumを表示用文字に変換する
    /// </summary>
    /// <param name="arg"></param>
    /// <returns></returns>
    /// 
    /*
    public string getCharapictureType(Charas.enum_CharapictureType arg)
    {
      string buff = "";
      switch (arg)
      {
        case Charas.enum_CharapictureType.dir:
          buff = "キャラ素材";
          break;
        case Charas.enum_CharapictureType.psdtoolkit:
          buff = "PSDToolKit";
          break;
        case Charas.enum_CharapictureType.psd:
          buff = "PSD";
          break;

      }
      return buff;

    }
    */
  }



}
