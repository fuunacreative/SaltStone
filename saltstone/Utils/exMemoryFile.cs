using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.MemoryMappedFiles;
// using System.Runtime.Serialization.Formatters.Binary;
using System.Text.Json;
using System.Web;
using static System.Windows.Forms.Design.AxImporter;


namespace saltstone
{
  public class exMemoryFile<T> : MemoryFile
  {
    public new delegate void del_sharememrev(T obj);
    public new del_sharememrev evt_sharememrev;
    private string _taskid;
    private bool lockfalg; // recieveloop内部で使用
    // eve call時にもsherememに簡易にアクセスできるようにするため


    public exMemoryFile()
    {
    }

    public exMemoryFile(string mmfkey = "", int size = MemoryFiles.DefaultSize)
    {

      base.init(mmfkey, size);
    }


    public new bool proctask()
    {
      // taskを作成し、delegateを実行する
      // semaphore開放時にtaskが実行される
      _taskid = STasks.createTask(_recieveloop);
      return true;
    }
    public void cancelTask()
    {
      STasks.cancelTask(_taskid);
    }
    public bool isRunning()
    {
      return STasks.isRunning(_taskid);
    }

    // ここをきれいにできないか？
    // base classでも同じ処理を実装している
    // 基本はgenericを使ったexMemoryFileを使用する
    // objectの型を決めずにsharememを使うことはないし
    // 使うときもgeneric=objectにすればよい
    // きれいにするときにbaseのMemoryFile classは削除する
    private new void _recieveloop()
    {
      T obj;
      if (evt_sharememrev == null)
      {
        return;
      }
      while (true)
      {

        bool ret = Semaphores.waitone(sem_sharemewrite, Semaphores.enum_SemaphoreWait.NoLimit);
        if (ret == false)
        {
          Utils.sleep(WAIT_TaskSleep);
          continue;
        }
        ret = Semaphores.waitone(sem_sharemem);
        if (ret == false)
        {
          Utils.sleep(WAIT_TaskSleep);
          continue;
        }
        lockflag = true; // ロック中の状態にする -> read,writeでsem checkしない
        ret = read(out obj);
        if (ret == false)
        {
          Utils.sleep(WAIT_TaskSleep);
          continue;
        }
        // ここだけbaseと違ううんだよねー
        // flagをたてて、semlockする必要がないことを内部状態に保存する
        evt_sharememrev(obj);
        // sem_lockqueueは常にrelease状態になる
        lockfalg = false;
        Semaphores.release(sem_sharemem);
        // 再びserver sem queue waitをロック状態にし、待機するようにしておく
        // ここがおかしい気がするな -> waitoneでいいの？
        // どこでsemをlock状態にするのか？
        // Semaphores.waitone(sem_addqueue);

      }

    }
    //public bool readnolock(out T obj)
    //{
    //  bool fret = false;
    //  obj = default(T);

    //  BinaryFormatter bf = new BinaryFormatter();
    //  MemoryStream ms = new MemoryStream();
    //  BinaryReader br;
    //  bool ret = getreader_nolock(out br);
    //  // 先頭４バイトはintとして読み込む
    //  int len = br.ReadInt32();
    //  byte[] bytebuff = new byte[len];
    //  bytebuff = br.ReadBytes(len);

    //  // 先頭4バイトをmem file sizeにするか？
    //  // そともそんな事考えなくても問題ない？
    //  // そもそも、sharemewm に格納するものはこの時点で確定している

    //  ms.Write(bytebuff, 0, len);
    //  ms.Seek(0, SeekOrigin.Begin);
    //  obj = (T)bf.Deserialize(ms);


    //  fret = true;
    //  return fret;
    //}

    public bool read(out T obj)
    {
      bool fret = false;
      bool ret;
      obj = default(T);
      if (_mmf == null)
      {
        return false;
      }

      if (lockflag == false)
      {
        ret = Semaphores.waitone(sem_sharemem);
        if (ret == false)
        {
          return fret;
        }
      }

      // BinaryFormatter bf = new BinaryFormatter();
      MemoryStream ms = new MemoryStream();
      BinaryReader br　= new BinaryReader(ms);
      // ret = getreader_nolock(out br);

      _memstream = _mmf.CreateViewStream();
      br = new BinaryReader(_memstream);

      // 先頭４バイトはintとして読み込む
      int len = br.ReadInt32();
      if (len == 0)
      {
        return fret;
      }
      byte[] bytebuff = new byte[len];
      bytebuff = br.ReadBytes(len);
      string readstr = System.Text.Encoding.UTF8.GetString(bytebuff);
      obj = System.Text.Json.JsonSerializer.Deserialize<T>(readstr);


      // 先頭4バイトをmem file sizeにするか？
      // そともそんな事考えなくても問題ない？
      // そもそも、sharemewm に格納するものはこの時点で確定している

      // ms.Write(bytebuff, 0, len);
      // ms.Seek(0, SeekOrigin.Begin);
      // obj = (T)bf.Deserialize(ms);
      // obj = br.ReadBytes(len);
      // TODO json serializerを使う



      // sem_lockqueueは常にrelease状態になる
      if (lockflag == false)
      {
        Semaphores.release(sem_sharemem);
      }

      ms.Close();
      ms.Dispose();
      br.Close();
      br.Dispose();
      _memstream.Close();
      _memstream.Dispose();

      fret = true;
      return fret;

    }

    public bool write(T obj)
    {
      bool fret = false;

      if (_mmf == null)
      {
        return false;
      }


      // BinaryFormatter bf = new BinaryFormatter();
      // MemoryStream ms = new MemoryStream();
      BinaryWriter bw;

      if (lockflag == false)
      {
        bool ret = Semaphores.waitone(sem_sharemem);
        if (ret == false)
        {
          return fret;
        }
      }
      _memstream = _mmf.CreateViewStream();
      string jsondata = System.Text.Json.JsonSerializer.Serialize(obj);
      

　　　bw = new BinaryWriter(_memstream);

      //bf.Serialize(ms, obj);
      //// ms -> byte[] -> binarywriter
      //byte[] bytebuff = new byte[ms.Length];
      //bytebuff = ms.GetBuffer();
      bw.Write((int)jsondata.Length);
      bw.Write(jsondata);
      //bw.Write(bytebuff.Length); // ここが問題 ms.lengthをどうやってreader側に伝えるか？
      //bw.Write(bytebuff);
      //ms.Close();
      //ms.Dispose();

      bw?.Close();
      bw?.Dispose();
      _memstream?.Close();
      _memstream?.Dispose();
      Semaphores.release(sem_sharemem);

      // semaphore unlockしwriteをeveloopへ通知
      if (lockflag == false)
      {
        Semaphores.release(sem_sharemewrite);
      }



      fret = true;
      return fret;

    }

    public bool write_nolock(T obj)
    {
      // sem lockする場合としない場合の処理がうまくかけない
      // 同じことをするんだけど、
      // 違うのは、createviewstream->bytewriterの間
      // sem lockするかどうかの違い
      // recieve event内部でwriteしようとするからこーなる
      bool fret = false;

      // BinaryFormatter bf = new BinaryFormatter();
      MemoryStream ms = new MemoryStream();
      BinaryWriter bw;
      _memstream = _mmf.CreateViewStream();
      
      string jsondata = System.Text.Json.JsonSerializer.Serialize(obj);


      // bf.Serialize(ms, obj);
      // byte[] bytebuff = new byte[ms.Length];
      // bytebuff = ms.GetBuffer();
      bw = new BinaryWriter(_memstream);
      // bw.Write(bytebuff.Length); // ここが問題 ms.lengthをどうやってreader側に伝えるか？
      // bw.Write(bytebuff);

      bw.Write((int)jsondata.Length);
      bw.Write(jsondata);

      ms.Close();
      ms.Dispose();
      bw.Close();
      bw.Dispose();
      _memstream.Close();
      _memstream.Dispose();

      fret = true;
      return fret;
    }




  }
}
