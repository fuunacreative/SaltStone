using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace saltstone
{
  public interface INamedpipedata
  {
    /// <summary>
    /// ５バイトの識別子
    /// </summary>
    string DataID { get; }


    public string getSerialize();
    public bool setSerialize(string buff);



  }
}
