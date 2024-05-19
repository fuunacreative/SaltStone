using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB
{
    public class Query
    {
        public string table;
        public string select;
        public string _where;
        public string orderby;

        public Query()
        {

        }

        public Query(string tablearg)
        {
            table = tablearg;
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
        public string where(string col ,int val)
        {
            if (string.IsNullOrEmpty(_where) == false)
            {
                _where += " AND ";
            }
            _where += col + "=" + val.ToString();
            return _where;
        }
        public string like(string col , string val)
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
        
    }
}
