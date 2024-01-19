using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace saltstone
{

  /// <summary>
  /// 別ファイルにすると、なぜかうまく動かない、、、なぜなのか？ だからvsは嫌いなんだよなー
  /// クラスを追加するのNG。元projで追加する必要があるのか、、、
  /// </summary>
  public class SNamedpipeClient : IDisposable
  {
    public NamedPipeClientStream pNpClient;
    public string _pipename;

    public SNamedpipeClient(string pipename)
    {
      _pipename = pipename;
    }

    public void init()
    {
      // _pipename = pipename;
      pNpClient = new NamedPipeClientStream(".", _pipename, PipeDirection.Out);
    }

    public void Dispose()
    {
      pNpClient?.Dispose();
    }

    public bool connect()
    {
      bool fret;
      if (pNpClient == null)
      {
        init();
      }
      if (pNpClient.IsConnected == true)
      {
        return true;
      }


      pNpClient.Connect();

      fret = true;
      return fret;
    }

    public bool send(string arg)
    {
      bool fret = false;
      if (pNpClient == null)
      {
        init();
      }
      if (pNpClient == null)
      {
        return fret;
      }
      if (pNpClient.IsConnected == false)
      {
        connect();
      }

      System.IO.BinaryWriter bs;
      UnicodeEncoding streamEncoding = new UnicodeEncoding();

      int maxpath = Utils.Files.MAX_PATH;
      UnicodeEncoding encode = new UnicodeEncoding();
      // TODO named pipeで 先頭にバイト数を書き込めばいいのでは？
      // ファイル名を書き込む前提になっているな、、、 ここから作り直しする必要がある
      byte[] buff = new byte[maxpath];
      //string intext = "";

      try
      {
        // binarywriterは使えない、、、
        // stringで送るしかないが、、、 logsをserializeして、json stringで送付
        // 受け取る側は、データがlogsのserializeかどうかわからない、、、

        // 方法としては、２回に分けて送る。１回目はデータ型
        // ２回目はjson string、、、
        // ここまでやる必要があるか？ logmanagerのpipe recieverはlogs json serializeがくる前提
        // 汎用性を持たせようとするとおかしくなるんだな、、、
        // SerializationFormat.Binaryは廃止
        // binary writerはok
        // binaryreader.readsringが使える
        // named pipeで送受信するデータ型をint32でもてば汎用性があがる。
        // logs=100とかにすればよい
        // binarywriter.writeint(100)
        // binarywriter.writestring(jsonstring)
        //で２回writeすればいいのでは？  stringのバイト長は考慮しなくてよい


        // string argをserializeして送る必要はない
        // sendline()で多分、newlineつきで送付する
        // 問題はやはり、先頭にデータタイプ、全体のバイト数のヘッダをつけるかどうか、、、

        bs = new System.IO.BinaryWriter(pNpClient);
        byte[] outBuffer = streamEncoding.GetBytes(arg);
        Array.Resize<byte>(ref outBuffer, maxpath);
        // sem lock
        bs.Write(outBuffer, 0, 256);

      }
      catch (Exception e)
      {
        Logs.write(e);
        return fret;
      }

      fret = true;
      return fret;
    }
  }
}
