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

    /// <summary>
    /// 実データ 長さはstringで送受信するのだから、必要ないのでは？
    /// 実データは必ずstring形式で送る
    /// 必ずserialize(json)を行う
    /// </summary>
    string data { get; set; }
  }
}
