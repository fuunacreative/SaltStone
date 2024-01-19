using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace DB
{
  public class Query
  {
    public string table;
    public string select;
    public string _where;
    public string orderby;
    public string sql;
    public Sqlite db; // queryを実行するdb


    public Query()
    {

    }

    public Query(string tablearg)
    {
      table = tablearg;
    }

    public void clear()
    {
      table = "";
      select = "";
      _where = "";
      orderby = "";
      sql = "";
    }

    public string where(string col, string val)
    {
      if (string.IsNullOrEmpty(_where) == false)
      {
        _where += " AND ";
      }
      _where += col + "=" + setcol(val);
      return _where;
    }
    public string where(string col, int val)
    {
      if (string.IsNullOrEmpty(_where) == false)
      {
        _where += " AND ";
      }
      _where += col + "=" + val.ToString();
      return _where;
    }
    public string like(string col, string val)
    {
      if (string.IsNullOrEmpty(_where) == false)
      {
        _where += " AND ";
      }
      _where += col + " LIKE " + setcol("%" + val + "%");
      return _where;
    }

    // where の直接指定
    public string where(string arg)
    {
      _where = arg;
      return arg;
    }
    private string setcol(string col)
    {
      return "'" + col + "'";
    }

    public DataTable getDataTable()
    {
      DataTable tbl = new DataTable();
      DBRecord rec = this.db.getrecord(this);
      if (rec == null)
      {
        // 空のtableを返す
        return tbl;
      }
     
      // recのrecordの名前がわからないとだめ
      // tbl.Columns.Add("");

      // for(int i = 0; i< rec.count)
      foreach (string col in rec.colname.Keys)
      {
        tbl.Columns.Add(col);
      }
      do
      {
        DataRow r = tbl.NewRow();
        foreach (string col in rec.colname.Keys)
        {
          r[col] = rec[col];
        }
        tbl.Rows.Add(r);
      } while (rec.Read() == true);

        return tbl;

    }

    public bool getOneField(out string field)
    {
      string buff = "";
      field = null;
      if (db == null)
      {
        return false;
      }
      bool ret = db.getonefield(this,out buff);
      if (ret == false)
      {
        return false;
      }
      field = buff;
      return true;
    }
  }
}
