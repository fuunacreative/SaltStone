using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.IO;

namespace saltstone
{
  public class CharaParts
  {
    public string partsid; // B
    public string partsubid; // BBなど divisionで定義
    public string partsnum; // 00
    public bool animeflag; // true or false DB 0=false 1=true
    public int animecount; // anime file count
    public string memo; // memo
    public string filename; // filename parts baseからの相対パス 絶対パスが入っている
    public string shortfilename; // 体\00.pngなど
    public CharaDraworder compositorder; // table compositorderのobject
    public string pHiddenParts; // 非描画パーツの保持 目\??z.png -> 眉・口を描画しない
    public List<CharaPartsAnime> animefiles;
    public int currentAnimeindes;
    public string drawruleid; // 描画順 or 非表示ルールのID
    // public string hiddenpartsid; // Y,Lなどの他パーツを非表示にするpartdid
    // public List<string> hiddenpartsid; // 非表示にする他のパーツ

    public bool checkrule(CharaDraworder argdraworder)
    {
      bool fret = false;

      // 顔¥[0-9]+a.png
      string buff;
      string[] l;
      // "\"が含まれていると正常にsplitできない
      // どーしたものか、、、
      l = argdraworder.patternregexp.Split(new[] { Path.DirectorySeparatorChar,Path.AltDirectorySeparatorChar  });
      //if (Charas.partsdirbykey(l[0]) == false)
      if (Charas.partskeybydir.ContainsKey(l[0]) == false)
      {
          return fret;
      }
      buff = l[1];
      Match m = Regex.Match(filename, buff, RegexOptions.IgnoreCase);
      if (m.Success == false)
      {
        return fret;
      }
      compositorder = argdraworder;

      return true;
    }

    // アニメ用のパーツの読み込み
    // table  partsanime

    public bool getanimefiles()
    {
      bool fret = false;

      if (animeflag == false)
      {
        return fret;
      }
      animefiles = new List<CharaPartsAnime>();
      DB.DBRecord rec;
      DB.Query q = new DB.Query();
      q.table = "partsanime";
      q.select = "animeopencount,partsfilename";
      q.where("partsid", partsid);
      q.where("partsnum", partsnum);
      rec = Charas.charadb.getrecord(q);
      q = new DB.Query();
      if (rec == null)
      {
        return fret;
      }
      CharaPartsAnime item;
      do
      {
        item = new CharaPartsAnime();
        animefiles.Add(item);
        item.animeopencount = rec.getnum(0);
        item.animefname = rec.getstring(1);

      } while (rec.Read() == true);


      return true;
    }
  }

  public class CharaPartsAnime
  {
    public int animeopencount; // アニメの開度 声の大きさやまぶたの開き具合など
    public string animefname;

  }
}
