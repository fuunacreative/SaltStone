using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace DB
{

  // sqlite用
  // レコードへのアクセスを簡略化するためのクラス
  public class DBRecord
  {
    public SQLiteDataReader rec;
    public string table; // 保存時に使用するテーブル

    // TODO insert ot updateをどう実装するか？


    // insertの処理もここで行う
    public Dictionary<string, object> cols; // insertするときの列名と値
    public Dictionary<string, int> colname; // indexとcolnameの対応

    public DBRecord()
    {
      init();
    }

    // 読み込み用の初期化
    public DBRecord(SQLiteDataReader arg)
    {
      init();
      rec = arg;
      // column name <-> indexの対応を保存
      // さらにrecのcolnameを取得できるようにする
      for (int i = 0; i < arg.FieldCount; i++)
      {
        colname[arg.GetName(i).ToString()] = i;
      }
    }
    private void init()
    {
      if (cols == null)
      {
        cols = new Dictionary<string, object>();
      }
      cols.Clear();
      if (colname == null)
      {
        colname = new Dictionary<string, int>();
      }
      colname.Clear();
    }

    // 書き込み用の初期化
    public DBRecord(string arg)
    {
      init();
      table = arg;
    }

    ~DBRecord()
    {
      if (cols != null)
      {
        cols.Clear();
        cols = null;
      }
    }

    public void clear()
    {
      init();
    }

    // db valを返すようにし、デフォルトでstring、キャストでintを返すようにする
    // colのtype情報が必要になるな
    // table情報をどこかに保存しておく必要がある もしくはdbから取得
    // objectを返すと、呼び出し元でstringにキャストしなければならない
    // stringを返すべきでは？
    public object this[string colname] {
      get {
        // dbがセットされていればread
        if (rec != null)
        {
          if (rec[colname] == DBNull.Value)
          {
            return "";
          }
          // return (string)rec[colname];  numberもあるので単純にstringに変換できない
          // もしくは全部 stringに変換して格納する方法もある 必要な場合にtointすればいい
          // もしくはintのまま返す?
          object obj = rec[colname];
          if (obj.GetType() == typeof(Int64))
          {
            obj = obj.ToString();
          }
          return obj;
          // return "";
        }
        return cols[colname];
      }
      set {
        cols[colname] = (object)value;
      }
    }

    /*
    public int getint(string colname)
    {
        return 0;
    }
    */

    public int setint(string colname, int val)
    {
      cols[colname] = val;
      return val;
    }

    public int getnum(string colname)
    {
      return (int)rec[colname];
    }
    public int getnum(int i)
    {
      if (rec.IsDBNull(i) == true)
      {
        return 0;
      }
      return (int)rec.GetInt32(i);
    }

    public bool Read()
    {
      return rec.Read();
    }

    // stringでキャストされたら１列目の値を文字として返す？
    // じゃあ、２文字目を求められたら？
    // stringを返すのを作ったほうがよさそうだな

    public string getstring(string colname = "")
    {
      // colnameと列名の変換が必要だな
      // select したときにdictionaryに保存した方がよいかも
      int i = 0;
      return rec.GetString(i);
    }
    public string getstring(int i)
    {
      // Type t = rec[i].GetType();
      if (rec[i].Equals(System.DBNull.Value))
      {
        return "";
      }
      return rec.GetString(i);
    }

  }
}

