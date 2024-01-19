using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Pipes;

namespace saltstone
{
  /// <summary>
  ///  named pipe server and client
  ///  送受信するのはstringに限定する
  ///  これ以外もbyteで送受信できるが、serializeなどの処理が必要になる
  ///   NG 一括してMAX_PATHを読み込んだ方が早い or 先頭２バイトを文字列長にして、それからreadで指定lenだけ読み込むか？   
  /// </summary>
  public class SNamedpipeServer : IDisposable
  {
    public NamedPipeServerStream pNpServer;
    public delegate void del_pipereaded(string arg);
    public del_pipereaded evt_pipereaded;
    public const string CMD_TERMINATE = "TERMINATE";

    public void initServer(string pipename,del_pipereaded evtfunc)
    {
      pNpServer = new NamedPipeServerStream(pipename, PipeDirection.In);
      // new System.IO.Pipes.NamedPipeServerStream("saltstonevoice_aq", System.IO.Pipes.PipeDirection.In);
      evt_pipereaded = evtfunc;
    }

    public void Dispose()
    {
      pNpServer?.Dispose();
    }


    public bool waitConnect()
    {
      bool fret = false;

      pNpServer.WaitForConnection();
      
      fret = true;
      return fret;
    }

    public bool readloop()
    {
      bool fret = false;

      int maxpath = Utils.Files.MAX_PATH;
      BinaryReader bs = null;
      UnicodeEncoding encode  = new UnicodeEncoding();
      byte[] buff = new byte[maxpath];
      string intext = "";

      if (evt_pipereaded == null)
      {
        return fret;
      }

      try
      {
        bs = new BinaryReader(pNpServer);
        // readできたらdeletageで指定したfuncをcall?
        // "TERMINATE"が送られてきたら終了
        while (pNpServer.IsConnected == true)
        {
          bs.Read(buff, 0, maxpath);
          intext = encode.GetString(buff).Trim('\0');
          if (intext == CMD_TERMINATE)
          {
            break;
          }
          evt_pipereaded(intext);
          intext = "";
        }
      }
      catch (Exception e)
      {
        string aa = e.Message;
        Logs.write(e);
        return fret;
      } finally
      {
        bs?.Dispose();
        // encode = null;
      }

      fret = true;
      return fret;
    }

    public bool waitprocess()
    {
      bool fret;

      fret = waitConnect();
      if (fret == false)
      {
        return fret;
      }
      fret = readloop();


      return fret;
    }

  }

  public class SNamespipeClient : IDisposable
  {
    public NamedPipeClientStream pNpClient;

    public void init(string pipename)
    {
      pNpClient = new NamedPipeClientStream(".", pipename, PipeDirection.Out);
    }

    public void Dispose()
    {
      pNpClient?.Dispose();
    }

    public bool connect()
    {
      bool fret;

      pNpClient.Connect();

      fret = true;
      return fret;
    }

    public bool send(string arg)
    {
      bool fret = false;

      if (pNpClient.IsConnected == false)
      {
        connect();
      }

      System.IO.BinaryWriter bs;
      UnicodeEncoding streamEncoding = new UnicodeEncoding();

      int maxpath = Utils.Files.MAX_PATH;
      UnicodeEncoding encode = new UnicodeEncoding();
      byte[] buff = new byte[maxpath];
      string intext = "";

      try
      {
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
