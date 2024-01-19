using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DB;

namespace saltstone
{
    class QueryChara : DB.Query
    {
        public enum Table
        {
            charactor,
            charaset
        }

        public QueryChara() : base()
        {

        }

        public QueryChara(Table table)
        {
            switch(table)
            {
                case Table.charactor:
                    this.table = Charas.table_charactor;
                    break;
                case Table.charaset:
                    this.table = Charas.table_charaset;
                    break;
            }

        }


        public DBRecord getrecord()
        {
            return Globals.charadb.getrecord(this);

        }
    }
}
