using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace saltstone
{
  public class CharaPortrait_psd : CharaPortrait
  {

    public override bool validate(string filename)
    {
      string fext = Files.getextention(filename);
      if (fext != ".psd")
      {
        return false;
      }
      bool ret = isExistHash(filename);
      if (ret == true)
      {
        return true;
      }

      // 有効なpsdか?
      // 一枚絵の場合は -> 立ち絵とはみなさない resouceのimgで貼り付ければよい
      // 先頭のヘッダ"8BPS0001"であれば有効とみなす
      // free の psd libraryはないっぽい
      // utils binary reader
      byte[] header = Util.binaryFile.getByteData(filename, 6);
      // headerが"8BPS0001"と同じかどうか、どうやって判定するの？
      byte[] psdheader = { (byte)'8', (byte)'B', (byte)'P', (byte)'S', 0x00, 0x01 };
      bool bret = psdheader.SequenceEqual<byte>(header);
      return bret;


    }
  }
}
