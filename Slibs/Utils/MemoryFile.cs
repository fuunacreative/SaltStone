using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Versioning;


namespace saltstone
{
  // memorymappedfile,queue,semaphoreを結びつけ
  // lockしながらqueueからとりだし、
  // evt_sharememrevを実行する
  public class MemoryFile : IDisposable
  {
    public const int WAIT_TaskSleep = 500;

    public string mmfkey;
    public MemoryMappedFile _mmf;
    public string sem_sharemem;
    public string sem_sharemewrite;
    public MemoryMappedViewStream _memstream;
    public delegate void del_sharememrev(object obj);
    // public delegate void del_sharememrev(capselclass< obj);
    // うーん。汎用でT(aqcmdmemstructureとか)を受け取って引き渡すevtがほしいんだが、、、
    // うまく定義できないなー genericを使ってコンパイル時にevet out argの型を確定するのは無理そーだね
    public del_sharememrev evt_sharememrev;
    protected bool lockflag;

    //  何がしたいかというと、genericを使って、evtをcalしたい
    // eventもgenericを受け付けて、外部からイベントとして登録できるといいんだが、、、
    // public event del_sharememrev<Task>;




    // memf m = new memf()
    // m.evt_sharemem += eventmethod
    // m.proctask()

    [SupportedOSPlatform("windows")]
    public MemoryFile(string argmmfkey = "", int size = MemoryFiles.DefaultSize)
    {
      mmfkey = argmmfkey;
      init(mmfkey, size);
    }

    [SupportedOSPlatform("windows")]
    public MemoryFile()
    {
      init();
    }

    [SupportedOSPlatform("windows")]
    public bool init(string mmfkey = "", int size = MemoryFiles.DefaultSize)
    {
      if (mmfkey == null || mmfkey.Length == 0)
      {
        mmfkey = Guid.NewGuid().ToString();
      }
      this.mmfkey = mmfkey;
      _mmf = MemoryMappedFile.CreateOrOpen(mmfkey, size);
      // sharemem lock and write用のkeyが統一されていないので、
      // clientとserverで別々のsemが使用され、結果、ロックがかからず、
      // write eventも発生しない
      // 統一キーを使うと、問題ありそーなきがするな、、
      // sharemem毎に統一キーが必要
      // たとえばAQcmd用とか
      // なんのためのsharmemなのかを外部から渡す必要がある、
      sem_sharemem = mmfkey + "_lock";
      sem_sharemewrite = mmfkey + "_wirte";
      Semaphores.create(sem_sharemem);
      Semaphores.create(sem_sharemewrite);
      // waitoneを実行し、write sem countを0にする
      Semaphores.waitone(sem_sharemewrite);
      return true;
    }

    [SupportedOSPlatform("windows")]
    public bool proctask()
    {
      // taskを作成し、delegateを実行する
      // semaphore開放時にtaskが実行される
      string taskid = STasks.createTask(_recieveloop);
      return true;
    }

    // queueを使う理由は？
    // 連続してsharememに書き込みが行われた場合に、
    // evt_sherememrevを実行するため？
    // queueは使ってない。名前がまぎらわしいだけ
    [SupportedOSPlatform("windows")]
    protected void _recieveloop()
    {
      object obj;
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
        lockflag = true;
        // ret = readnolock(out obj);
        ret = read(out obj);
        if (ret == false)
        {
          Utils.sleep(WAIT_TaskSleep);
          continue;
        }
        // flagをたてる？
        evt_sharememrev(obj);
        lockflag = false;
        // sem_lockqueueは常にrelease状態になる
        Semaphores.release(sem_sharemem);
        // 再びserver sem queue waitをロック状態にし、待機するようにしておく
        // ここがおかしい気がするな -> waitoneでいいの？
        // どこでsemをlock状態にするのか？
        // Semaphores.waitone(sem_addqueue);

      }
    }

    public void Dispose()
    {
      _memstream?.Dispose();
      _mmf?.Dispose();
    }


    public bool getreader_nolock(out BinaryReader breder)
    {
      breder = null;
      return true;
    }

    public bool getreader_withlock(out BinaryReader breder)
    {
      breder = null;
      bool ret = Semaphores.waitone(sem_sharemem);
      if (ret == false)
      {
        return ret;
      }
      if (_mmf == null)
      {
        return false;
      }
      _memstream = _mmf.CreateViewStream();
      breder = new BinaryReader(_memstream);
      return true;
    }

    // MemoryFile(このclass)にwrite/read処理を持たせる
    public bool write<T>(T obj)
    {
      bool fret = false;

      if (_mmf == null)
      {
        return false;
      }


      // BinaryFormatter bf = new BinaryFormatter();
      // net8ではbinaryformatterは使用不可 binarywriterにmemorystreamをコンストラクタで与え
      // 直接書き込む
      MemoryStream ms = new MemoryStream();
      BinaryWriter bw = new BinaryWriter(ms);

      bool ret = Semaphores.waitone(sem_sharemem);
      if (ret == false)
      {
        return fret;
      }
      _memstream = _mmf.CreateViewStream();
      bw = new BinaryWriter(_memstream);

      // bf.Serialize(ms, obj);
      // ms -> byte[] -> binarywriter
      byte[] bytebuff = new byte[ms.Length];
      bytebuff = ms.GetBuffer();
      
      bw.Write((Int32)bytebuff.Length); // ここが問題 ms.lengthをどうやってreader側に伝えるか？
      bw.Write(bytebuff);
      ms.Close();
      ms.Dispose();

      bw?.Close();
      bw?.Dispose();
      _memstream?.Close();
      _memstream?.Dispose();
      Semaphores.release(sem_sharemem);

      // semaphore unlockしwriteをeveloopへ通知
      Semaphores.release(sem_sharemewrite);


      // bf = null;
      return fret;
    }

    // evet loop内部から呼び出すことを想定
    // share memのsemaphore lockは行わない
    public bool read(out object obj)
    {
      bool fret = false;
      obj = null;

      // BinaryFormatter bf = new BinaryFormatter();
      MemoryStream ms = new MemoryStream();
      BinaryReader br = new BinaryReader(ms);
      if (_mmf == null)
      {
        return false;
      }

      if (lockflag == true)
      {
        bool ret = Semaphores.waitone(sem_sharemem);
        if (ret == false)
        {
          return fret;
        }
      }

      _memstream = _mmf.CreateViewStream();
      br = new BinaryReader(_memstream);

      // bool ret = getreader_nolock(out br);

      // 先頭４バイトはintとして読み込む
      int len = br.ReadInt32();
      byte[] bytebuff = new byte[len];
      bytebuff = br.ReadBytes(len);

      if (lockflag == true)
      {
        Semaphores.release(sem_sharemem);
      }
      // 先頭4バイトをmem file sizeにするか？
      // そともそんな事考えなくても問題ない？
      // そもそも、sharemewm に格納するものはこの時点で確定している

      ms.Write(bytebuff, 0, len);
      ms.Seek(0, SeekOrigin.Begin);
      // obj = bf.Deserialize(ms);
      obj = br.Read();

      ms.Close();
      ms.Dispose();
      br.Close();
      br.Dispose();
      _memstream.Close();
      _memstream.Dispose();

      fret = true;
      return fret;
    }

  }
}
