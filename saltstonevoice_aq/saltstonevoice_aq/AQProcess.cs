using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using saltstone;


// sharememからのcmd受付
// namedpipeからのmsg受付
// これをまとめるクラスが必要
// formではできない。exe自体がform不要となるので、、、
// named pipe -> sharemem -> aqvoice -> wavを処理する
// クラスを作るべきだねー


namespace saltstonevoice_aq
{
  public class AQProcess :IDisposable
  {
    public saltstone.exMemoryFile<AqcmdMemstrucure> mf;
    public saltstone.SNamedpipeServer nps;
    public const string PIPENAME_AQprocess = "saltstonevoice_aq_x86";
    public const string SHAREMEMAME_AQprocess = "saltstonevoice_aq_x86_sharemem";
    public string currentjobid;

    public void init()
    {
      mf = new saltstone.exMemoryFile<AqcmdMemstrucure>(SHAREMEMAME_AQprocess);
      // mf.evt_sharememrev = evt_revobj;
      //mf.proctask();
      // saltstone.Utils.sleep(200);

      nps = new saltstone.SNamedpipeServer(PIPENAME_AQprocess);
      nps.initServer(evt_namedpiperecieve);
      // nmaedp のconnectをwaitし、recieve loopをまわす
      // "TERMINATE"が送られてくるまでloopをまわす

      // named pipeが送られてきてから初めてsheremem proctaskをまわす？
      // named pipe revでmf procをstart?
      // share mem rev semaphoreをwaitするので、
      // releaseされていればすぐに処理が始まるはず、、、、
      nps.proctask();
      // namedpipeloop_taskid = saltstone.STasks.createTask(startNamedpipeloop);
      // evt_namedpiperecieveが異常終了している？


    }

    public void Dispose()
    {
      mf.cancelTask();
      nps.cancelTask();
      // saltstone.STasks.cancelTask(namedpipeloop_taskid);
      mf?.Dispose();
      nps?.Dispose();
    }
    

    // named pipeのrev event
    public void evt_namedpiperecieve(string msg)
    {
      // msgにはnamedpipeで送られてきたmsgが入る

      // msg形式 aqtxtcmd,10(jobid),incmdtxt = txtファイルによるwav出力指示
      //  or aqmemcmd
      if (msg == "TERMINATE")
      {
        // prrocess end処理
        Environment.Exit(0);
      }
      int i;
      i = msg.IndexOf("aqmemcmd");
      if (i == -1)
      {
        saltstone.Logs.write("aquestalk_x86:不正な命令が渡されました[" + msg + "]");
        return;
      }

      // aqmemcmd,jobid=A10
      string[] ary = msg.Split(',');
      if (ary.Length != 2)
      {
        msg = "aquestalk_x86" + "不正なmessageです[" + msg + "]";
        saltstone.Logs.write(msg);
        return;
      }
      string buff = ary[1];
      // jobid=A10
      ary = buff.Split('=');
      if (ary.Length != 2)
      {
        msg = "aquestalk_x86" + "jobidが不正です[" + msg + "]";
        saltstone.Logs.write(msg);
        return;
      }
      currentjobid = ary[1];

      // jobidをどこかに保存が必要
      // share memのevntを実行
      // mf.proctask();
      AqcmdMemstrucure aqcmd;
      bool fret = mf.read(out aqcmd);
      if (aqcmd.jobid != currentjobid)
      {
        // named pipeで送られてきたaqcmdと違うのでresultにerrorを返す
        aqcmd.resultcode = "WrongJOBID requested=[" + currentjobid + "]";
        aqcmd.resultdate = DateTime.Now;
        mf.write(aqcmd);
        // saltstone.STasks.createTask(new Action(() => { writeresult(aqcmd); }));

        // mf.write(aqcmd);
        // client側ではmf semaphore wait->result checkを繰り返す
        return;
      }

      //string aqclassname = "saltstonevoice_aq.Aquestalk_f1";
      //Type t = Type.GetType(aqclassname);
      // voiceinterface_aq voice = (voiceinterface_aq)Activator.CreateInstance(t);
      // voiceinterface_aq voice = AQvoices.getVoice(obj.voiceid);

      // TODO voiceをどうやってdbから読み込んで、インスタンス化するか？
      AQcmd cmd = new AQcmd(aqcmd);


      // voice = voices[cmd.voiceid];
      bool ret = cmd.createwav();
      if (ret == false)
      {
        // error
        msg = "Aquestalk wav出力に失敗しました";
        saltstone.Logs.write(msg);
        return;
      }
      aqcmd.resultcode = "OK_JOBID[" + currentjobid + "]";
      aqcmd.resultdate = DateTime.Now;
      // sem lock中なので、writeでlockをかけてデッドロックする
      // wirteすると、sharememのlockがはずれて eventが発生するんだ、、、
      // saltstone.STasks.createTask(new Action (() => { writeresult(aqcmd); }));
      // mf.write_nolock(aqcmd);
      mf.write(aqcmd);
      currentjobid = "";



      // aqmemcmd,jobid=10
      // AQcmd.execmemcmd(msg);


      return;
    }

    public void startNamedpipeloop()
    {
      // server側のloop処理
      // background task (namedpipe loop,share mem loop)が終了するまで
      // 待機する
      bool npsrunflag = true;
      bool memfilerunflag = true;
      while(npsrunflag == true || memfilerunflag == true)
      {
        saltstone.Utils.sleep(1000);
        npsrunflag = nps.isRunning();
        memfilerunflag = nps.isRunning();
      }
      string aaa = "aaa";
    }
  }
}


// 後ろで常に動作し、semaphore,mmfを使用してphoneticstring,outfnameを受け取りwavを作成する

// semaphore 待機
// sharemを参照し、cmdが渡されているか確認
//   -> 渡されていなければ再度 semaphore wait

// 渡されたsharemem内のcmd txtを参照 -> phoneticstring,outfnameを参照
// phoneticstringからwavを作成(file)

// sharememにresultを保存,wavを出力
// 処理済みのjobidを格納
// result timesampを保存
// semaaphore countup -> sharemem unlock

// これを無限に繰り返す
//  to phoneicstring,outfnameともに"end"が支持されるまで
//  or arg endでexe実行されるまで
