using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace saltstone
{
  public class CharaPortrait
  {
    // 立ち絵の管理
    // 基底クラス zip psdに派生させる


    // 立ち絵のキャラ名
    // portraitのid (Pまりさ or Pまりさ.psd)
    public string id;
    public string name;
    public CharaPortraits.enum_CharapictureType charaPictureType;
    public string charaPictureTypeName;
    public string filename; // dirの場合はディレクトリ名、psdの場合はpsd file name
    public bool includeAnimeParts; // data grid viewでcheckboxで表示させたい
    // public string includeAnimePartsName;
    public string creator;
    public string url;
    public string originalFile;
    public string filepath; // dirならディレクトリ れいむ psdならpsdファイル名
    public string hash;
    public string memo; // 立ち絵につける、動画作成者独自のメモ

    virtual public bool parse(string filename)
    {
      return true;
    }

    virtual public bool validate(string filename)
    {
      return true;
    }

    public bool isExistHash(string filename)
    {
      // fullpath -> filename
      string fname = Files.getfilename(filename);

      string hash = Files.getHash(fname);
      DB.Query q = Charas.charadb.newQuery(CharaPortraits.table_charapicture);
      q.where("hash", hash);
      q.select = "hash";
      string buff = "";
      bool ret = q.getOneField(out buff);
      // recordがない場合とエラーとの区別がつけられてない
      // -> retをboolにして、outでrecordを返すようにするか？
      if (ret == false)
      {
        return false;
      }
      if (buff.Length == 0)
      {
        return false;
      }
      return true;

    }

  }
}
