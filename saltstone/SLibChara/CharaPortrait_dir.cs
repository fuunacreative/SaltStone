using System;
using System.Collections.Generic;
using Utils;

namespace saltstone
{
  // 規定クラス
  // これを元にdir ,psdtoolkit , psdの派生クラスを作成する
  public class CharaPortrait_dir : CharaPortrait
  {

    // charaid? charadefineとは別 ここではもてないかも
    // dir or psdtoolkit or psd
    // セット.txtの階層
    // saltstone独自のパーツ分類
    // 各パーツ
    // アニメファイル or layerを含んでいるかどうかのふらぐ

    private string extractpath;


    // refrectionでmemberを取得できる
    // -> dbと1:1で対応させることはできるが、、、追加のメンバーをどうするかが問題
    // dbのtable columnを検索して一致するもののみ取得？


    /*
    // このままの形式でdbへ保存し、どこからでも読み出せるようにしておく
    // parseはCharaPicturesで行う

    public string getType()
    {

    }
    string getBitmap();
    string setCharaID();
    string getCharaID();


    bool setType(string type);
    bool setCharaid();
    */

    // properyをinterfaceで宣言できるか？
    // typeをenumにできるか？
    // classにするべきかもな

    public bool reparse()
    {
      // TODO db tableを廃棄し、再解析する
      return true;
    }

    /// <summary>
    /// dir形式の場合、全\セット.txtを解析する
    /// </summary>
    /// <param name="charadir"></param>
    /// <returns></returns>
    public bool parseset(string charadir)
    {
      
      // charadirにはれいむのフルパスが入ってくる
      string setfpath = charadir + "\\" + CharaPortraits.CONST_setfname; // セット.txtを解析
      if (Utils.Files.exist(setfpath) == false)
      {
        return false;
      }
      string alltext = Util.readAllText(setfpath);
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
        files = Util.searchfile(outdirbuff, buff);
        outpng = "";
        if (files.Count > 0)
        {

          // outpng = files.First();
          outpng = files[0];
        }
        // dbへ保存
        rec["charaid"] = charaid;
        rec.setint("setnum", setnum);
        rec["partsstr"] = partsstr;
        rec["filename"] = outpng;
        Charas.charadb.Write(rec);
      }

      return true;
    }

    override public bool parse(string fullportraifile)
    {
      // 先にzipを展開してdirがtempに保存されてる
      // なので、temp\まりさを立ち絵ディレクトリにコピーして？
      // zipはoriginalに保存したい、、、、



      // argにはdownloadしたfull file nameがはっている
      // argのportraitfileはdir or zipの両方がありえる

      // filenameはfullpathが入っている
      // filename のみを portraitfilenameに切り出し
      string portraitfilename = Utils.Files.getfilename(fullportraifile);
      // string orgfile = Globals.Directory.charapicture_original + "\\" + portraitfilename;
      string orgfile = Appinfo.CharaOriginaldir + "\\" + portraitfilename;

      // originalを退避(orig) -> charaportrait_originaldirectory
      // Videos\chara\original
      Utils.Files.Copy(fullportraifile, orgfile);


      string fext = Utils.Files.getextention(portraitfilename);
      bool ret = false;
      if (fext.ToLower() != ".zip")
      {
        return ret;
      }
      // zipfile用の追加処理を行う
      /*
      ret = pf_addzipportraito(fullportraifile);
      if (ret == false)
      {
        return false;
      }
      }*/

      // argにはtempへのportrait full file nameが渡される
      // zip only
      // zipを立ち絵dirへ展開
      // キャラ名 れいむを取得
      // 各テーブルのidをどうするか？ まりさではかぶる可能性がある mも同様
     
      string chardirname = Utils.Files.getbasename(fullportraifile);

      // path -> originalへコピー

      // validateで展開しているはず
      // original -> tempへコピー
      string tempfile = Appinfo.tempdir + "\\" + portraitfilename;
      // string tempfile = Globals.Directory.tempdir + "\\" + portraitfilename;
      Utils.Files.Copy(fullportraifile, tempfile);

      // zipfileはtempdirのpathになっている
      // Zip z = new Zip(fullportraifile);
      // temp dirでextract
      // z.extract();
      // Utils.Files.delete(zipfile);

      // これをtempで展開し、chardirへrsyncでコピーする
      // string charportraitfile = Globals.Directory.charapicture + "\\" + chardirname;
      string charportraitfile = Appinfo.charabasedir + "\\" + chardirname;
      Utils.Files.rsync(portraitfilename, charportraitfile);

      // idはPまりさ_XESD (random 4文字) or PまりさPSD_DSX1
      id = "P" + chardirname + "_" + Util.SGuid.getShort();

      // TODO アニメ用のファイルをどうするか？
      // 別にテーブルが必要 親はpartsファイル これに対し、子供のアニメパーツレコードを用意 
      // volume 開度 + まばたき + アルファ
      // アルファのアニメって何があるかな？


      // charportraitfileに立ち絵用のディレクトリ＋zipfileのbase部分が入っている
      // C:\Users\fuuna\Videos\resource\chara\れいむ
      // これを解析する
      // ファイル名をすべてdbに保存 -> パーツ（体）毎にtableに保存 -> アニメパーツの解析 -> hashを計算し登録
      // -> 代表立ち絵の作成
      // idはPまりさ_guid

      // charasを作るか？ -> guiでどうなってるか？
      // 同名ディレクトリがある場合どうするか？
      // 立ち絵(portrait)はキャラとは別で管理する
      // 立ち絵ディレクトリのディレクトリ名 or psdのファイル名で識別
      // tempにzipを展開 -> dir比較し、上書き確認 -> timestampが最近のものであれば上書き
      // utils.messegebox.show("上書きしますか？")


      return true;
    }

    // これはparseにまとめられる
    // private bool pf_addzipportraito(string zipfile)

    override public bool validate(string filename)
    {
      extractpath = "";
      string fext = Utils.Files.getextention(filename).ToLower();
      if (fext != ".zip")
      {
        return false;
      }
      // hashに登録されているか確認
      bool bHash = isExistHash(filename);
      // hashに登録されていても展開は必ず行う

      // hashがなくても展開して全があればOKとする
      // 一度,tmpdirに展開し、確認が終われば削除する
      // tempに展開する必要ある？ 展開するのなら、それをそのまま立ち絵dirに移動すればいいのでは？
      // string tempdir = Globals.Directory.tempdir;
      string tempdir = Appinfo.tempdir;
      // 一度、tempにコピー -> そこで展開 -> 展開したファイルを返す

      string tempfile = tempdir + "\\" + Utils.Files.getfilename(filename);
      // 一応、dir makeしておく
      Utils.Files.mkdir(tempdir);
      bool ret = Utils.Files.Copy(filename, tempfile);
      string extpath = tempdir + "\\" + Utils.Files.getbasename(tempfile);
      // tempにないとは思うが、展開されるdirを削除しておく
      ret = Utils.Files.delete(extpath);
      Zip zfile = new Zip(tempfile);
      zfile.extract(tempdir);

      if (bHash == true)
      {
        extractpath = extpath;
        return true;
      }

      string path = tempdir + "\\全";
      if (Utils.Files.exist(path) == false)
      {
        return false;
      }
      // tempに展開したdirを内部 memberに保存しておく
      extractpath = extpath;
      // Utils.Files.delete(tempdir);
      return true;
    }


  }
}
