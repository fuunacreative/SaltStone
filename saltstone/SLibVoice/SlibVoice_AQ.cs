using MeCab.Core;
using System;
using Utils;

namespace saltstone
{
  // saltstonelibrary voiceeffectと同じ動きをする
  public class SlibVoice_AQ : Voics
  {
    
    public const int DEFAULT_Speed = 100;
    static string CONST_pipename = "saltstonevoice_aq_x86";
    static string CONST_sharemem_key = "saltstonevoice_aq_x86_sharemem";

    // x86用のaquestalk voiceのfunc
    // in phonetictext,speed -> out wav
    // x86の場合はsaltstonevoice_aqを起動してsharemem write -> namesppied send 
    //

    // voiceidは関係ないね。各dllの実態化はx86側で対応している。
    // 単純にinitでexeを実行して、sharemem write -> namedpipe sendすればよい
    // パラメータspeedはどうするか？
    // aq用のphonetic textをどう取得するか? <- aq用のphoneticが必要になるので、
    // このクラス内部で管理する
    // public SlibPhonetic_AQ convPhonetic;

    public exMemoryFile<AqcmdMemstrucure> mf;
    public SNamedpipeClient npc;


    public override void init()
    {
      if( inited == true)
      {
        return;
      }
      // exeを実行する
      if (string.IsNullOrEmpty(exefilename) == false)
      {
        // exeが実行されているかcheck
        bool ret = Util.checkrunexe(exefilename);
        if(ret == false)
        {
          Util.runexec(exefilename, "", Util.enum_runmode.start_async, Util.enum_runwinfront.background);

        }
      }
      if(mf == null)
      {
        mf = new exMemoryFile<AqcmdMemstrucure>();
        mf.init(CONST_sharemem_key);

      }
      if (npc == null)
      {
        // AQF1,AQF2で同じpipenameを持つクライアントが必要になるので
        // namepipeをとりまとめ、重複してname pipe clientを作成しないようにする
        bool fet = SNamedpipes.getClient(CONST_pipename,out npc);
        // npc = new SNamespipeClient(CONST_pipename);

      }

      // npc.init(pipename);

      if(phoneticcnv == null)
      {
        phoneticcnv = new Phonetic_aqmecab();

      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="intext">セリフtxt(発声用のtextへは内部で変換する)</param>
    /// <returns></returns>
    public override bool makeWave(VoiceText arg)
    {
      bool fret = false;
      bool ret;
      if(string.IsNullOrEmpty(arg.phonetic) == true)
      {
        arg.phonetic = phoneticcnv.getPhonetic(arg.text);
      }
      string ptext = arg.phonetic;
       

      init();

      // TODO bug ２回目を実行しようとすると x86.exeがおちている

      // x86 aq exe用に引き渡すため
      //  namedpipeに命令を送り
      //  sharememに書き込む
      AqcmdMemstrucure amem = new AqcmdMemstrucure();
      amem.command = "aqmemcmd";
      // amem.voiceid = "AQF1";
      amem.voiceid = voiceid;
      // parameterをどう受け取るか？
      int speed = DEFAULT_Speed;
      if (parameter.ContainsKey("speed") == true)
      {
        ret = int.TryParse(parameter["speed"], out speed);
      }
      amem.speed = speed;
      amem.jobid = "AQ10";
      amem.phonetictext = ptext;
      // outfnameをどう受け取るか?
      if (string.IsNullOrEmpty(arg.outwavefile) == true)
      {
        return fret;
      }
      Utils.Files.delete(arg.outwavefile);
      // amem.outwavfile = @"C:\Users\fuuna\a.wav";
      amem.outwavfile = arg.outwavefile;
      amem.orderdate = DateTime.Now;
      // amem.jobid = "AQ10";
      mf.init("saltstonevoice_aq_x86_sharemem");

      mf.write<AqcmdMemstrucure>(amem);

      string sendmsg = "aqmemcmd,jobid=AQ10";

      // saltstone.SNamespipeClient npc = new saltstone.SNamespipeClient();

      //AqcmdMemstrucure revmem; // amemと同じにできる
      // saltstone.exMemoryFile<AqcmdMemstrucure> clientmf = new saltstone.exMemoryFile<AqcmdMemstrucure>();

      // npc serverは動いているのに、connectできない
      // 二重でコネクトしようとしているか？
      // サーバ側が落ちる？なぜ？
      // npc.connect();
      npc.send(sendmsg); // named pipeにcmdをsendし、AQserverにwav createを依頼する

      // 結果を受け取る
      // wavが作成されるまでwaitされるはずが、されていない
      // どーやって、作成されることを確認するか？
      int i;
      int cnt = 0;
      while(cnt < 100)
      {
        fret = mf.read(out amem);
        if (fret == false)
        {
          return fret;
        }
        if (amem.resultcode == null)
        {
          Util.sleep(10);
          cnt += 1;
          // debugだとうまく動くが、exeにすると作成されない？
          // TODO 2024/01/08 wavを作成するnpcサーバーが動いていないっぽい
          continue;
        }
        i = amem.resultcode.IndexOf("OK");
        if (i >= 0)
        {
          break;
        }
        cnt += 1;
      }
      if(cnt == 100)
      {
        Logs.write("voice 作成がタイムアウトしました");
        
        return false;
      }
      // share memのサーバをどっち側にするか？
      // 普通に考えて、namedpipe serverと同じにする

      // ファイルの存在チェック
      if (Utils.Files.exist(amem.outwavfile) == false)
      {
        return false;
      }



      //System.Media.SoundPlayer p = new System.Media.SoundPlayer(amem.outwavfile);
      //p.Play();
      //p.Dispose();

      fret = true;
      return fret;
    }
  }

}
