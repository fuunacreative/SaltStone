using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace vocalization
{

  public class Aquestalk_base
  {
    protected const string dlldir = "AquesTalk";
    protected const string dllfile = "AquesTalk.dll";

    // protected static abstract void AquesTalk_FreeWave();

    protected virtual IntPtr Synthe(string koe, int iSpeed, out int size)
    {
      size = 0;
      return IntPtr.Zero;
    }
    protected virtual void free(IntPtr pt)
    {

    }

    protected virtual MemoryStream getWav(string koe, int speed,MemoryStream data)
    {
      int size;
      IntPtr pt = Synthe(koe, speed, out size);
      getStream(pt, size, data);
      free(pt);
      return data;

    }

    protected MemoryStream getStream(IntPtr pt, int size,MemoryStream data)
    {
      // safe codeにするため、いったんbyte[]にデータをコピー
      // それからmemorystreamへコピーする
      byte[] buff = new byte[size];
      System.Runtime.InteropServices.Marshal.Copy(pt, buff, 0, size);
      data.Write(buff, 0, size);

      /*
      unsafe
      {
        byte* pnt = (byte*)pt.ToPointer();
        byte* buff = pnt;
        // UnmanagedMemoryStream writeStream
        // intptrからsizeを読み込みmemstremへコピー
        data.Write(buff, 0, size);
      }
      */
      /*
      for (int i = 0; i < size; i++)
      {
        data.WriteByte(pt);
      }
      */
      return data;
    
    }

    // 基底クラスで宣言しておき、baseにcastしても呼び出せるようにしておく
    public virtual bool createwav(string subtitle, MemoryStream data)
    {
      return false;
    }
  }
}
