using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace saltstone
{
  // パーツにより他の描画位置を変更する場合がある
  // 目\01-15.pngなど これを保管するためのクラス
  public class CharaLocationAdjust
  {
    public string ruleid;
    public string patternregexp;
    public string memo;
    public List<CharaLocationAdjustitem> locationadjusts;
    public void Dispose()
    {
      locationadjusts.Clear();
    }
  }


  public class CharaLocationAdjustitem
  {
    public string partsid; // 口 , 眉
    public string adjustpartsfile; // 口\*.png , 眉\*.png
    public string adjustparam; // y-15
    public string yloc; // -15
  }
}
