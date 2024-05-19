using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace saltstone
{
  public enum enum_partscolorchange
  {
    bright,
    contrast,
    hue,
    saturation
  }

  public class CharaColoChange
  {
    public string ruleid; // CH01
    public string ruleno; // 010~
    public string parameter; // 色変えに使うパラメーター kamicolor
    // kamicolor,hadacolor,mecolor
    // fukuuecolor,fukushitacolor
    public string partsid; // H(髪)とか
    public string patternregexp;
    public string memo;
    public SortedDictionary<string, CharaColorChangeItem> coloritems;
    public void Dispose()
    {
      foreach (CharaColorChangeItem ci in coloritems.Values)
      {
        ci.Dispose();
      }
      coloritems = null;
    }
  }

  public class CharaColorChangeItem
  {
    public string conditionstr; // 0 とか <=0で条件一致とみなす
    public int condition;
    // 代替ファイルをどう保持するか？
    // List<CharaColorChangeItemColor> files;
    // string CH01 ruleno 010で複数のcolorchangefileをもつ
    // CH01_010 => 00bu.png  , nextは CH01_010 => 00c.png + colorparam
    SortedDictionary<string, List<CharaColorChangeItemColor>> colorchangefiles;
    public void Dispose()
    {
      foreach (List<CharaColorChangeItemColor> item in colorchangefiles.Values)
      {
        item.Clear();
      }
      colorchangefiles = null;
    }

  }

  public class CharaColorChangeItemColor
  {
    // colorrule -> kamicolor<=0の場合のfile or 代価ファイルを保持する
    public string colorchangefile;
    public enum_partscolorchange colorchangetype;
    public int colorparam;
  }


}
