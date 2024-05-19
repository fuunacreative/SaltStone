using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;


// TODO 付属するdllも一緒にコピーしないとうまく雨後かないっぽい
// おそらくglobalが２つ作られてる なのでうまくアクセスできない
// globals整理する必要がある このままではまずい
// messageboxを使ってたり、わちゃわちゃすぎる
// globalsのiniファイル読み込みをどうするかだなー
// 基本、globalsは使用しない 


// TODO サービス層を作るかどうか


namespace DB
{
  public class Sqlite : IDisposable
  {
    public string sqlfilename;
    public SQLiteConnection conn;

    public Sqlite()
    {
      // ファイル名の指定がない場合、
      // regファイルのiniで設定されているdbを読み込む
      // string dbfname
      // globalsをインポートするとエラーとなる
      // Globals.enviniがnull？なぜ？
      // たぶん、ybharaとsqliteで２つGlobalsが定義されているからだ
      // どう解決すればいいの？
      //string fname = Globals.envini["pgdb"];
      // これはちょっと問題だな
      // exeの実行時ディレクトリに設定ファイルを書くべきだが、、、
      // 設定を分散するべきか、ひとつのディレクトリにまとめるべきか？
      // aviutilのpluginとして作成するのなら、aviutil配下にsubdirを作って、そこにまとめておくべき
      // しかし、このタイミングでglobas.iniは呼び出されているか？
      //string fname = @"C:\Users\yasuhiko\program\yb.db";
      string fname = "";
      // TODO　regに登録されているiniファイルからdbfnameのみを取得する処理が必要
      // regの使用はさけたい セキュリティが複雑すぎて、pgからの登録がまともに行えない
      init(fname);
      // Sqlite(fname);

    }

    public Sqlite(string dbname)
    {
      bool ret = init(dbname);
      if (ret == false)
      {
        throw new Exception("db:" + dbname + "が存在しません");
      }
    }

    public bool init(string dbname)
    {
      if (saltstone.Utils.Files.exist(dbname) == false)
      {
        // throw new Exception("db:" + dbname + "が存在しません");
        return false;
      }
      // dbはyb.db（ｐｇ設定用のdb）とybcharar.db(キャラ素材ディレクトリ用）の２つある
      // なので、ここではdbのcreateは行えない
      // yb.dbとybchara.db用のクラスを作るべきなのだが、、、
      sqlfilename = dbname;
      if (conn == null)
      {
        conn = new SQLiteConnection();
        conn.ConnectionString = "Data Source=" + dbname;
        conn.Open();
        // 開きっぱなしにするのではなく、selectのたびにcloseする？
        // filehandleが開きっぱなしになっていいか？
        // 問題ないな 
        // https://stackoverflow.com/questions/10325683/can-i-read-and-write-to-a-sqlite-database-concurrently-from-multiple-connections
        // multi connは許可されている
      }
      return true;
    }

    public void Dispose()
    {
      if (conn != null)
      {
        conn.Close();
        conn.Dispose();
        conn = null;
      }
    }

    /*
    public Globals.Charapicturetype getcharapicturetype()
    {
        // キャッシュがほしいな
        // いや、必要ないか
        Globals.Charapicturetype c = Globals.Charapicturetype.DIR;

        // テーブルの存在チェック
        string tbname = "globalsetting";
        if(tableexists(tbname) == false )
        {
            // create table
            // 存在しない場合はキャラ素材をDIRとして判定
            return c; 
        }
        // dbからの読み出しはうまくいった
        // sqlビルダー
        string sql = "SELECT value from globalsetting where name = " + strcolname("charatype") + ";";
        DBRecord rec =  getrecord(sql);
        string buff = rec["value"];
        if (buff.ToLower() == "psd")
        {
            c = Globals.Charapicturetype.PSD;
        }



        return c;

    }
    */

    public string strcolname(string arg)
    {
      return "'" + arg + "'";
    }

    private bool tableexists(string tablename)
    {
      string sql = "";
      sql += "SELECT name FROM sqlite_master WHERE type = 'table' AND name = '" + tablename + "';";
      DBRecord rec = getrawrecord(sql);
      bool ret = rec.Read();
      if (ret == false)
      {
        return false;
      }

      return true;
    }

    public DBRecord getrecord(string sql)
    {
      // 最初の１レコードのみ返す
      // 複数の列を取得する場合、どのように返すか？
      // SQLiteDataReader rec = conn.exe
      // これっていらないかも
      SQLiteCommand com = new SQLiteCommand(sql, conn);
      // SQLiteDataReader rec = com.ExecuteReader();
      DBRecord rec = new DBRecord(com.ExecuteReader());
      com.Dispose();
      rec.Read();
      return rec;
    }


    public DBRecord getrecord(Query q)
    {
      string sql = "";
      if (q.select == null || q.select.Length == 0)
      {
        q.select = "*";
      }
      sql += "SELECT " + q.select;
      sql += " FROM " + q.table;
      if (string.IsNullOrEmpty(q._where) == false)
      {
        sql += " WHERE " + q._where;
      }
      if (string.IsNullOrEmpty(q.orderby) == false)
      {
        sql += " ORDER BY " + q.orderby;
      }
      // conn null

      SQLiteCommand com = new SQLiteCommand(sql, conn);
      // SQLiteDataReader rec = com.ExecuteReader();
      DBRecord rec = new DBRecord(com.ExecuteReader());
      com.Dispose();
      if (rec == null)
      {
        return null;
      }
      bool ret = rec.Read(); //　ここで一回目のreadを行っている なので、戻るrecには１レコード目をさしている
      if (ret == false)
      {
        return null;
      }
      return rec;
    }

    

    /// <summary>
    /// getrecordと同じだが、最初のreadを実行しない 
    ///   -> while(rec.read() == true)でループできる
    /// </summary>
    /// <param name="q"></param>
    /// <returns></returns>
    public bool getrecord_noread(Query q,out DBRecord rec)
    {
      string sql = "";
      if (q.select == null || q.select.Length == 0)
      {
        q.select = "*";
      }
      sql += "SELECT " + q.select;
      sql += " FROM " + q.table;
      if (string.IsNullOrEmpty(q._where) == false)
      {
        sql += " WHERE " + q._where;
      }
      if (string.IsNullOrEmpty(q.orderby) == false)
      {
        sql += " ORDER BY " + q.orderby;
      }
      // conn null

      SQLiteCommand com = new SQLiteCommand(sql, conn);
      // SQLiteDataReader rec = com.ExecuteReader();
      rec = new DBRecord(com.ExecuteReader());
      com.Dispose();
      if (rec == null)
      {
        return false;
      }
      return true;
    }


    public bool getonefield(Query q,out string field)
    {
      string sql = "";
      sql += "SELECT " + q.select;
      sql += " FROM " + q.table;
      sql += " WHERE " + q._where;
      q.sql = sql;
      field = "";

      try
      {

        SQLiteCommand com = new SQLiteCommand(sql, conn);
        // SQLiteDataReader rec = com.ExecuteReader();
        DBRecord rec = new DBRecord(com.ExecuteReader());
        com.Dispose();
        bool ret = rec.Read();
        if (ret == false)
        {
          return true;
        }
        if (rec.rec.GetFieldType(0).Name == "String")
        {
          field = rec.rec.GetString(0);
          return true;
        }
        if (rec.rec.GetFieldType(0).Name == "Integer")
        {
          field = rec.rec.GetInt32(0).ToString();
          return true;
        }
      }
      catch (Exception e)
      {
        // error をguiに表示するか、logに保存したい
        saltstone.Logs.write(e);
        return false;
      }
      /*
      if(rec.rec.GetFieldType(0) == )
      {

      }*/

      return true;

    }

    public string getonefield(Query q)
    {
      string sql = "";
      sql += "SELECT " + q.select;
      sql += " FROM " + q.table;
      sql += " WHERE " + q._where;
      q.sql = sql;
      SQLiteCommand com = new SQLiteCommand(sql, conn);
      DBRecord rec = new DBRecord(com.ExecuteReader());
      com.Dispose();
      bool ret = rec.Read();
      if (ret == false)
      {
        return "";
      }
      if (rec.rec.GetFieldType(0).Name == "String")
      {
        return rec.rec.GetString(0);
      }
      if (rec.rec.GetFieldType(0).Name == "Integer")
      {
        return rec.rec.GetInt32(0).ToString();
      }
      return "";

    }

    public DBRecord getrawrecord(string sql)
    {
      SQLiteCommand com = new SQLiteCommand(sql, conn);
      // SQLiteDataReader rec = com.ExecuteReader();
      DBRecord rec = new DBRecord(com.ExecuteReader());

      com.Dispose();
      return rec;
    }

    public DBRecord getrawrecord(Query q)
    {
      string sql = "";
      sql += "SELECT " + q.select;
      sql += " FROM " + q.table;
      if (String.IsNullOrEmpty(q._where) == false)
      {
        sql += " WHERE " + q._where;
      }
      SQLiteCommand com = new SQLiteCommand(sql, conn);
      // SQLiteDataReader rec = com.ExecuteReader();
      DBRecord rec = new DBRecord(com.ExecuteReader());

      com.Dispose();
      return rec;
    }

    // sqlを簡単に組み立てられる方法がないか？
    // sqlbuilderのようなクラスとか
    // レコードをそのまま扱えればいい
    // stringとして取り出すか、intとして取り出すかによるから、
    // レコード用のクラスを作ってやればいいんだ

    public bool Write(DBRecord rec)
    {
      if (rec == null)
      {
        return false;
      }
      // sqlの組み立て
      // insert or update
      // insert into 
      // insert into user values (2, 'hoge', 'hogefuga') on conflict(id, name) do update set description = 'fugafuga';
      string sql = "";
      sql += "INSERT INTO ";
      string cols = "(";
      string values = "(";
      foreach (KeyValuePair<string, object> p in rec.cols)
      {
        // id は数値
        // dbcolを格納するクラスが必要なのでは？


        cols += p.Key + ",";
        // System.String or System.Int32
        string coltype = p.Value.GetType().ToString();
        if (p.Value.GetType() == typeof(string))
        {
          values += "'" + p.Value + "',";
        } else
        {
          values += p.Value.ToString() + ",";
        }
      }
      // cols ,valuesの最後の、を削る
      cols = cols.Substring(0, cols.Length - 1) + ")";
      values = values.Substring(0, values.Length - 1) + ")";
      sql += rec.table + " " + cols + " VALUES " + values;

      SQLiteCommand com = new SQLiteCommand(sql, conn);
      int i = com.ExecuteNonQuery();
      if (i == 0)
      {
        return false;
      }
      com.Dispose();

      /*
      string sql = "SELECT last_insert_rowid()";
      SqlCommand cmd = new SqlCommand(sql, conn);
      conn.Open();
      int lastID = (Int32)cmd.ExecuteScalar();
      */
      // IDはテーブル名を示すCHとか、CIとかCPとかにしたい -> ちょっと無理があるかも

      return true;
    }

    public bool InsertAndUpdate(DBRecord rec, string key)
    {
      bool fret = true;

      //　キーはひとつを想定
      Query q = new Query(rec.table);
      q.where(key, (string)rec[key]);
      DBRecord srec = getrecord(q);
      if (srec == null)
      {
        // insert
        fret = Write(rec);
        q = null;
        return fret;
      }
      srec.table = rec.table;
      // srec(getしたもの)をbaseにarg recのparamで上書き
      // updateがあるかどうか　-> ない なので、delete insertする
      foreach (string colname  in rec.cols.Keys)
      {
        srec[colname] = rec[colname];
      }
      q = new Query(rec.table);
      q.where(key, (string)rec[key]);
      fret = delete(q);
      fret = Write(srec);




      return fret;
    }

    public bool Exec(string sql)
    {
      SQLiteCommand com = new SQLiteCommand(sql, conn);
      int i = com.ExecuteNonQuery();
      if (i > 0)
      {
        return true;
      }
      return false;
    }

    public bool delete(Query q)
    {
      string sql = "";
      if (string.IsNullOrEmpty(q.table) == true)
      {
        return false;
      }
      sql = "DELETE FROM " + q.table;
      if (string.IsNullOrEmpty(q._where) != true)
      {
        sql += " WHERE " + q._where;
      }
      SQLiteCommand com = new SQLiteCommand(sql, conn);
      int i = com.ExecuteNonQuery();
      if (i > 0)
      {
        return true;
      }

      return true;
    }

    public Query newQuery()
    {
      Query q = new Query();
      q.db = this;
      return q;
    }

    public Query newQuery(string table)
    {
      Query q = new Query();
      q.db = this;
      q.table = table;
      return q;
    }


  }

}
