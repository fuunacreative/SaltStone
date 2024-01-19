﻿using saltstone;
using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace saltstone
{
  /// <summary>
  ///  named pipe server and client
  ///  送受信するのはstringに限定する
  ///  これ以外もbyteで送受信できるが、serializeなどの処理が必要になる
  ///   NG 一括してMAX_PATHを読み込んだ方が早い or 先頭２バイトを文字列長にして、それからreadで指定lenだけ読み込むか？   
  /// </summary>
  public class SNamedpipeServer
  {
    public NamedPipeServerStream pNpServer;
    public delegate void del_pipereaded(string arg);
    public del_pipereaded evt_pipereaded;
    public const string CMD_TERMINATE = "TERMINATE";
    private string _taskid;
    private string _pipename;

    public SNamedpipeServer(string pipename)
    {
      _pipename = pipename;
    }

    public void initServer(del_pipereaded evtfunc)
    {
      try
      {
        pNpServer = new NamedPipeServerStream(_pipename, PipeDirection.In);
      }
      catch (Exception e)
      {
        Logs.write(e);
      }
      // new System.IO.Pipes.NamedPipeServerStream("saltstonevoice_aq", System.IO.Pipes.PipeDirection.In);
      evt_pipereaded = evtfunc;
    }

    public void Dispose()
    {
      STasks.cancelTask(_taskid);
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
      BinaryReader? bs = null;
      UnicodeEncoding encode = new UnicodeEncoding();
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
        // clientがdisconectされると、isconnectedはfalseになる
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
      }
      finally
      {
        bs?.Dispose();
        // encode = null;
      }

      fret = true;
      return fret;
    }

    public void proctask()
    {
      _taskid = STasks.createTask(waitprocess);
    }

    public void cancelTask()
    {
      STasks.cancelTask(_taskid);
    }

    public bool isRunning()
    {
      return STasks.isRunning(_taskid);
    }

    public void waitprocess()
    {

      bool fret = waitConnect();
      if (fret == false)
      {
        return;
      }
      fret = readloop();


      return;
    }

  }
}
