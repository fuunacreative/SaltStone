using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace saltstone
{
  public class chara_publicdefinitions
  {
    public const string table_charadefinision = "publicdefinition";

    // datatableを返す

    public static DataTable getAllList()
    {
      DataTable tbl = null;

      //DB.Query q = new DB.Query(table_charadefinision); 
      // DB.DBRecord rec = q.getrecord();
      // tbl = q.getDataTable();
      // どのdbに対して？
      // Globals.charadb.newQuery();
      DB.Query q = Globals.charadb.newQuery(table_charadefinision);
      tbl = q.getDataTable();

      return tbl;
    }
  }
}
