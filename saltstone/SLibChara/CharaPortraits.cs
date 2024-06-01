using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Runtime.InteropServices;
using System.IO;
using Utils;

// 画像データの自動クリッピングができないか？
// 特にパーツ

// 立ち絵全部を管理するクラス
// dwされ展開されているかどうか？ 
// 登録されているキャラ素材のfile hashなど


// TODO chara関係のtableをvoiceとportraitにわけて定義するべき？
// それともCharasで一括して定義するべき
// TODO charaの中でportraitの解析を行っている
// TODO charapictureはcharaportraitに変更 portraits内部に移動するべき？
namespace saltstone
{
  /// <summary>
  /// キャラの立ち絵を管理する
  /// dir and psdなどすべての立ち絵をキーでアクセスできるようにする
  /// dir形式のparse、psdのparseを行う
  /// singleton
  /// </summary>
  public class CharaPortraits : IDisposable
  {
    // データはdb saltstonechara.dbに保存
    // table charactorpicture
    // file hashを計算し、立ち絵師、urlを判定する
    // 入力したデータはwebシステムにアップ
    // 存在しない場合はwebシステムより情報を取得する
    // -> jsonをparseする共通の仕組みがほしいな
    // refrectionでclassのメンバーを取得できないか？
    // Type.GetMembers

    #region const
    public const string URL_charapicture = "http://www.nicotalk.com/charasozai.html";
    public const string table_charapicture = "charactorpicture";
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
    public const string table_pictureAll = "allpictures"; // わかっているすべてのキャラ素材（立ち絵）を管理する
    public const string table_picture = "picture"; // インストール済みのキャラ素材（立ち絵）を管理する
    public const string CONST_setfname = @"全\セット.txt";
    public const string CONST_outdir = "out"; // rubyで処理したセット.txtから合成した画像ファイルの保存先
    public const string CONST_partsfile = "chara.txt";
    public const string CONST_favoritefile = "favorite.txt";
    #endregion

    #region c++_inteface
    //   static bool getPctStartXY(char* imeges,int width,int height,int* x, int* y);
    [DllImport("imaging.dll", EntryPoint = "getPctStartXY")]
    public static extern void getPctStartXY(IntPtr images, int width, int height, ref int x, ref int y, ref int endx, ref int endy);


    [DllImport("imaging.dll", EntryPoint = "getPctStartXY2")]
    public static extern void getPctStartXY2(BitmapInfo bmp, ref int x, ref int y);
    // static bool getPctStartXY2(BitmapInfo* image, int* x, int* y);

    [DllImport("imaging.dll", EntryPoint = "clipping")]
    public static extern bool clipping(BitmapInfo src, BitmapInfo dst);
    #endregion

    #region enum
    public enum enum_CharapictureType
    {
      dir, // キャラ素材スクリプト形式
      psdtoolkit, // psdtoolkit対応のpsd
      psd // 上記以外のpsd
    }
    #endregion

    // install済みのキャラ
    // keyがint?
    // stringにしてdirnameにしたい
    // 問題はpsdとdirで同名がありえ るということ
    // dirname or guidがいいな どっちにするべきか？
    // guidにする dirnameはcharapicture内部で保管している
    // 参照するときは、charas内部でdbを検索し、portrait objを作成
    // いや、dbに保存するのだから、dirname or filename.psdにするべきだな
    private Dictionary<string, CharaPortrait> _portraits;
    private Dictionary<string, CharaPortrait> _portraits_byhash;
    public static string charadir = ""; // キャラ素材ディレクトリ


    private static CharaPortraits _instance;

    public static CharaPortraits GetInstance()
    {
      if (_instance == null)
      {
        _instance = new CharaPortraits();
      }
      return _instance;
    }



    public CharaPortraits()
    {
      // dbより登録されているcharactorpictureを読み込み
      // private memberに保存
      if (_portraits == null)
      {
        _portraits = new Dictionary<string, CharaPortrait>();
      }
    }

    public void Dispose()
    {
      _portraits?.Clear();
      _portraits = null;
    }

    public int Count()
    {
      return _portraits.Count;
    }


    // public CharaPicture [](string arg)
    public CharaPortrait_dir this[string arg] {
      get {
        // hash = kKkiRJInvXca2JxPHstbbyzpv2HNJkVnf0FznuTl5Bs=
        // 略文字 = １文字
        // 10文字以内 = キャラ素材の名前
        return null;
      }
    }

    public bool isInstalled(string hash)
    {
      // hashはどうやって計算する？
      // psdの場合はfilenameでok
      // dirの場合は？ dirnameでhashを計算する
      // TODO 管理している立ち絵について、hashを計算しdbに保存しておく
      bool ret = false;
      return ret;
    }

    // table charactorpictureを参照
    // saltstonechara.db
    // 立ち絵の一覧を取得


    public enum_CharapictureType getType(string fullfilename)
    {
      string fext = Files.getextention(fullfilename).ToLower();
      if (fext == ".zip")
      {
        return enum_CharapictureType.dir;
      } else if (fext == ".psd")
      {
        return enum_CharapictureType.psd;
      }
      // psdtoolkitのタイプもある
      // これはpsdを解析して、レイヤーの先頭記号があるかどうかで判定する
      return enum_CharapictureType.dir;
    }

   
    /// <summary>
    /// 立ち絵管理を追加
    /// filename が zip or psdを判断し、zipなら正常なキャラ素材か判定
    /// フォルダ・レイヤー解析を行いdbに保存
    /// charapictureクラスを作成し、これを返す
    /// 内部にdictionaryをもってキーでアクセスできるようにする
    /// charaomikey r or れいむ どちらでも検索できるようにしたいな
    /// パーツ解析は、salts独自の分類を間にかます
    /// 画像のキャッシュ用の合成(一覧画面で使用）もbg threadで行う
    /// c++で合成できるといいな
    /// portraitidをどうするか？ "Pまりさ","Pまりさ.psd"とかかな
    public bool addChara_portrait(string fullfilename,out string portraitid)
    {
      // add charaでいいか？ 立ち絵とvoiceの追加が必要
      // 立ち絵がpictureなのがなー
      // fileが正常なキャラ素材かチェックする
      // charaじゃなくて、キャラ識別子を返せばいいのでは？
      // chara idはどうやって判断する？ファイル名の先頭文字？で仮名する
      // 後ろで画面で識別子を再定義する
      // これらはzip用のクラスで処理を行う

      // zip or psdで処理をわける 
      // zipの場合、validateで展開してしまうので、展開したdirを受け取る

      portraitid = "";

      // TODO ここでvalidateをするのではなくて、charaportraitの個別の立ち絵クラスでやるべきなのでは？
      // それに、psd用とdir用でcharaportraitクラスを分ける必要があるのでは？
      CharaPortrait c = null;
      if (getType(fullfilename) == enum_CharapictureType.dir)
      {
        c = (CharaPortrait)new CharaPortrait_dir();
      }
      bool ret = c.validate(fullfilename);
      // bool ret = validate(fullfilename,out outdir);
      if (ret == false)
      {
        return false;
      }
      // 既に登録済みでないかチェック -> validateで行っている
      //  string filename = Utils.Files.getfilename(fullfilename);
      // string hash = Utils.Files.getHash(filename);

      // if (this.isExistByHash(hash) == true)
      // {
      //   portraitid = _portraits_byhash[hash].id;
      //   return true;
      // }

      // outdirにtempに展開されたdirが入っている
      // これをキャラ素材ディレクトリにコピーする
      // dirを比較する必要があるか？
      // キャッシュを保存してれば比較の必要があるが、キャッシュの日付・期間などでチェックすればよいのでやらない
      // Utils.Files.rsync(outdir, Globals.Directory.charapicture);
      // string dirname = Utils.Files.getfilename(outdir);
      // string srcdir = Globals.Directory.charapicture + "\\" + dirname;

      // 内部でoudirから立ち絵ディレクトリにコピーする
      ret = c.parse(fullfilename);
      if (c == null)
      {
        Logs.write("立ち絵の登録・解析処理に失敗しました");
        // ここで画面にも表示させたい
        return false;
      }
      return true;


      /*
      // filenameが zip or padのチェック
      string fext = Utils.Files.getextention(filename);
      if (fext == ".psd")
      {
        // TODO psd用の登録を行う
      } else if (fext == ".zip")
      {
        ret = pf_addCharaPicture(filename);
        if (ret == false)
        {
          // errorメッセージはどう表示させるの？
          // ライブラリからフォームへメッセージやprogressbarの進捗を更新するform のinterfaceを作る
          Logs.write("立ち絵の追加に失敗しました", Logs.Logtype.disperror);
          return ret;
        }
      } else
      {
        return false;
      }
      */





      // パーツを解析 -> salts用に分類 -> 各パーツを分類 -> ファイルをtableに登録
      // 初期立ち絵を作成し、立ち絵として定義画面に表示
      // gridviewのセルの赤をリセット


      // IDはC0001とかにする -> これは難しいのでやらない sqliteではかなり無理がある


      // dbを検索 同一のものがあればそれを使用


      // なければ登録
      // zipなら展開 -> charafolderへ originalをコピー

      // dir形式ならパーツ解析、 psdならレイヤー解析

    }







    public bool isExistByHash(string hash)
    {
      // dbを検索し、hashで既に登録があるか確認
      // 全部のキャラ素材を管理するtableと、インストール済みのもののみ管理するtableに分ける
      DB.Query q = Charas.charadb.newQuery(table_picture);
      q.select = "hash";
      q.where("hash", hash);
      string buff = "";
      bool ret = q.getOneField(out buff);
      if (ret == false)
      {
        return false;
      }
      if (buff.Length > 0)
      {
        return true;
      }
      return false;
    }




    // キャラ素材ディレクトリの全部を解析する
    // 全体的に見直しをする必要がある
    // parseは使用しない -> dbに保存されてる <- guiより定義をdbに保存
    
    public bool reparse()
    {
      // キャラ素材ディレクトリをすべて検索しdbを更新する

      // 初期化処理
      // charadir = Globals.envini[Charas.envsetting];
      // charadbfullpath = charadir + "\\" + Charas.charadb;
      // charadir = Globals.envini[PGInifile.INI_Chardir];
      charadir = Appinfo.charabasedir;
      //charadbfullpath = Globals.envini[PGInifile.INI_CharaDB];

      /*
      if (this.db == null)
      {
        Globals.charadb.Dispose();
        Globals.charadb = null;

        this.db = new DB.Sqlite(charadbfullpath);
        Globals.charadb = db;
      }
        */
      // string sql = "";


      if (Directory.Exists(charadir) == false)
      {
        // キャラ素材dirがなければ何もしない
        return false;
      }

      _portraits.Clear();

      // charactor tableを削除
      DB.Query q = new DB.Query(Charas.table_charactor);
      // dbがlockしてる おそらく２重に開いている
      Charas.charadb.delete(q);

      // サブディレクトリ名を検索
      List<string> dirs = Utils.Files.getdirectory(charadir);
      // これではdir形式しか対応しない
      // TODO psd形式を追加する必要がある


      // DB.DBRecord rec = new DB.DBRecord("charactor");
      DB.DBRecord rec = new DB.DBRecord(Charas.table_charactor);
      int i = 1;
      string charaid;
      CharaPortrait c;
      foreach (string s in dirs)
      {
        charaid = Path.GetFileName(s);
        // これでdirname or file.psdになるはず
        rec["name"] = charaid;
        rec["directory"] = s;
        rec.setint("disporder", i);
        Charas.charadb.Write(rec);
        // 内部のcharaを再構築する
        //c = new Chara(s);
        // TODO reparseするとき、dirのみとは限らず、psdもありえる
        // tableよりtypeを取得し、これにより場合わけする
        c = (CharaPortrait) new CharaPortrait_dir();
        bool ret = c.parse(s);
        _portraits.Add(charaid, c);
        c.filename = s;
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
      foreach (CharaPortrait_dir cs in _portraits.Values)
      {
        // cs.parseset(); // set.txtを解析
        // cs.parsefavorite(); // favorite.txtの解析
        // cs.parsepartsdef(); // chara.txt パーツの表示名
        // cs.parseparts(); // 各パーツのサブディレクトリを解析
        // cs.parse() // 上記の全部を行う
        cs.reparse();
      }

      return true;
    }
    


    unsafe public static void test(out Bitmap outimg)
    {
      string fname = @"C:\Users\fuuna\Videos\resource\chara\れいむ\口\" + "00.bmp";
      // 以下のロジックは、クラスにまとめられたはず
      SLibBitmap bi = new SLibBitmap(fname);
      // bi.setBitmap(fname);

      // Bitmap orgb = new Bitmap(fname);
      // Rectangle rect = new Rectangle(0, 0, orgb.Width, orgb.Height);
      // Bitmap b = orgb.Clone(rect, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

      // cloneしてもほぼ同じだな
      // 先頭にffのalphaまわりがきて、00のかたまり -> 400pixel後ろに再度ffのalphaのかたまり


      // System.Drawing.Imaging.BitmapData bmpData =
      //     b.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadOnly,
      //     System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
      // System.Drawing.Imaging.BitmapData bmpData =
      //    b.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite,b.PixelFormat);

      // Get the address of the first line.
      // IntPtr ptr = bmpData.Scan0;

      // little endian ？でも全部0だから黒になっちゃんだよね
      // 画像を確認しても、全部透過 だけど、xy= 0,8からalphaが0になる
      // 何が問題なのか？
      // 少なくとも、想定しているような４バイト毎のpixeldataにはなってない
      // 先頭にalphaが集中し、400pixel後ろに再度alphaデータがでてきている

      // この方法では想定した4バイト毎のpixel bitmapを取得できてない
      // pngデータとしてのバイト配列を持っているのかもな
      // ３１バイト目まで
      // ff ff ff 00のくりかえし
      // それから00が並ぶ
      // 先頭にalphaがきてるとしか思えない
      // どこで間違えてるの？
      // 32bppArgbになってるしなー
      // byte orderはbgraであってそうだけどなー
      // んー。まったく原因がわからん

      // 0=透過 255=透過なし
      // 255が透過だと勘違いしていた



      // このあたりでbmpのraw dataがちゃんとbuffにコピーされてない？
      // managed memory pointer を unmanagedから使うのは問題あるのでは？ marshal copyした方が安全だが、、、
      // TODO そのまま使えるのであれば、copy logic を skipできるので高速化できる

      // Declare an array to hold the bytes of the bitmap.
      // int bytes = Math.Abs(bmpData.Stride) * b.Height;
      // byte[] rgbValues = new byte[bytes];
      // Marshal.Copy(ptr, 0, rgbValues, bytelength);
      // Marshal.Copy(ptr, rgbValues, 0, bytes);
      // IntPtr buff = Marshal.AllocHGlobal(bytes);
      // Marshal.Copy(ptr, 0, buff, bytes);
      // CopyMemory(buff, ptr, (uint)bytes);
      // Buffer.MemoryCopy(ptr.ToPointer(), buff, bytes, bytes);
      // Copy the RGB values into the array.
      // System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);

      // dllの呼び出し
      int x = 0, y = 0;
      // int endx = 0, endy = 0;
      int width = bi.width;
      int height = bi.height;
      // getPctStartXY(bi.data, width, height, ref x, ref y,ref endx,ref endy);
      // 0startなので、実際のポイントは+1
      // BitmapInfo bir = (BitmapInfo)bi;
      // IntPtr p = Marshal.AllocHGlobal(sizeof(BitmapInfo));
      // Marshal.StructureToPtr<BitmapInfo>(bir, p, true);


      getPctStartXY2(bi, ref x, ref y);
      BitmapInfo dst = new BitmapInfo(bi);
      bool ret = clipping((BitmapInfo)bi, dst);

      // TODO bitmap cacheクラスを作成 bitmapをメモリ上に保存するためのクラス
      // c#でbitmapを作成すると内部でscn0のメモリを確保する
      // 問題なのは、いちいちmallocして問題ないかどうか？

      // 呼び出しから変更する必要があるな
      // bitmapinfo classを引数に変更する
      // getClippedBitmap(Bitmapinfo bmp,ref x , ref y)

      bi.Dispose();

      //Bitmap output = new Bitmap(width, height, stride, System.Drawing.Imaging.PixelFormat.Format32bppArgb, myPointer);
      // Bitmap realOutput = (Bitmap)output.Clone();
      // https://social.msdn.microsoft.com/Forums/vstudio/en-US/de9ee1c9-16d3-4422-a99f-e863041e4c1d/reading-raw-rgba-data-into-a-bitmap?forum=netfxbcl
      // strideは１ラインあたりのバイト数っぽいけどな
      // Bitmap clipimg = new Bitmap(dst.height, dst.width, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
      nint dstptr = (nint)dst.data;
      Bitmap clipimg = new Bitmap(dst.width, dst.height, dst.width * 4, System.Drawing.Imaging.PixelFormat.Format32bppArgb, dstptr);
      outimg = new Bitmap(dst.width, dst.height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
      // dst.dataが崩壊している
      // cloneではデータがコピーされない？



      Rectangle rect = new Rectangle(0, 0, dst.width, dst.height);
      System.Drawing.Imaging.BitmapData bmpData =
         outimg.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, outimg.PixelFormat);
      // Utils.memory.copy(dst.data, bmpData.Scan0, bmpData.Stride * dst.height);
      SLibMemory.Memory.Copy(dst.data,bmpData.Scan0, bmpData.Stride * dst.height);

      outimg.UnlockBits(bmpData);

      clipimg.Dispose();
      dst.Dispose();

      // Bitmap clipimg = new Bitmap(dst.width, dst.height, dst.width * 4, System.Drawing.Imaging.PixelFormat.Format32bppArgb, dst.data);
      // ほかにはgetしたときと同じようにlockbitをする方法があるっぽいな
      // このあたりがおかしい -> おそらくdstをdisposeしてるからでは？

      // outimg = clipimg;

      // Marshal.FreeHGlobal(buff);
      // b.UnlockBits(bmpData);

      // さて、dll呼び出しは実証できた
      // 何をやりたいかというとクリッピングがしたい
      // in src(image,w,h) -> dst(image,w,h)
      // クラスを引数として渡せるか？というより、mmfを使いたいな





      // memory mapped file
      // mutex
      // pipe and wait
      // progress




    }

  }

}

