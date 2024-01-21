using saltstone;
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
    /// <summary>
    /// named pipeの先頭５バイトで送られてくるdataID
    /// </summary>
    public string datatype;

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

      //int maxpath = Utils.Files.MAX_PATH;
      //BinaryReader bs = null;
      //UnicodeEncoding encode = new UnicodeEncoding();
      //byte[] buff = new byte[maxpath];
      //string intext = "";

      if (evt_pipereaded == null)
      {
        return fret;
      }

      byte[] dataid_byte = new byte[5];
      try
      {
        // IsConnectedがfalseの場合、named_pipeのrecieveを待機せずに終了してしまう
        while (true)
        {
          // 接続を常に待機する

          waitConnect();

          if (pNpServer.IsConnected == false)
          {
            Utils.sleep(500);
            continue;
          }
          if (pNpServer.Length < 5)
          {
            // 5バイト以内であれば、不正データとして扱う
            // データを破棄しないといけないのでは？
            Utils.sleep(500);
            continue;
          }
          // 頭5バイトがちゃんとstringに変換できるか？ 
          // named pipe data id "LOG__"など
          // ５バイト固定なので、binary readerのread stringは使えない
          // pNpServerはdisposeでcloseする。call側でコントロールする
          int i = pNpServer.Read(dataid_byte, 0, 5);
          string dataid = System.Text.Encoding.UTF8.GetString(dataid_byte);
          // dataid と logsのdataidが一致しているかを確認

          // ６バイト目からstring dataを読み込む
          // pNpServer.Position = 6;
          using (BinaryReader bs = new BinaryReader(pNpServer))
          {

            bs.BaseStream.Position = 6;
            string buff = bs.ReadString();
            evt_pipereaded(buff);
          }

          //// readできたらdeletageで指定したfuncをcall?
          //// "TERMINATE"が送られてきたら終了
          //// clientがdisconectされると、isconnectedはfalseになる
          //bs.Read(buff, 0, maxpath);
          //intext = encode.GetString(buff).Trim('\0');
          //if (intext == CMD_TERMINATE)
          //{
          //  break;
          //}
          //evt_pipereaded(intext);
          //intext = "";
        }
      }
      catch (Exception e)
      {
        string aa = e.Message;
        Logs.write(e);
        return fret;
      }
      //finally
      //{
      //  bs?.Dispose();
      //  // encode = null;
      //}
      // return true;
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
