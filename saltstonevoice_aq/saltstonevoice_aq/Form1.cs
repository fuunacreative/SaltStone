using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using saltstone;

namespace saltstonevoice_aq
{
  public partial class Form1 : Form
  {
    public Form1()
    {
      InitializeComponent();
      txtPhonetic.Text = "あー。いいひだったーー";
    }

    public void showmsg(string arg)
    {
      string bb = arg;
    }


    private void button1_Click(object sender, EventArgs e)
    {
      AqcmdMemstrucure amem = new AqcmdMemstrucure();
      amem.command = "aqmemcmd";
      amem.voiceid = "AQF1";
      amem.speed = 100;
      amem.jobid = "AQ10";
      amem.phonetictext = txtPhonetic.Text;
      amem.outwavfile = @"C:\Users\fuuna\a.wav";
      amem.orderdate = DateTime.Now;
      AQGlobal.aqserver.mf.write<AqcmdMemstrucure>(amem);

      // nameedpipeにcmdを送る
      string sendmsg = "aqmemcmd,jobid=AQ10";

      string pipename = "saltstonevoice_aq_x86";
      saltstone.SNamespipeClient npc = new saltstone.SNamespipeClient(pipename);
      // npc.init(pipename);
      npc.connect();
      npc.send(sendmsg); // named pipeにcmdをsendし、AQserverにwav createを依頼する

      // これがないとresultがとれない -> semがうまく動いていない
      // write semで待機するはずだｇあ、していないってことか、、、
      // saltstone.Utils.sleep(100);

      // resultを受け取る
      // mf.write<AqcmdMemstrucure>(amem);
      AqcmdMemstrucure revmem;
      string sharemem_key = "saltstonevoice_aq_x86_sharemem";
      saltstone.exMemoryFile<AqcmdMemstrucure> clientmf = new saltstone.exMemoryFile<AqcmdMemstrucure>();
      // 内部で同じsemaphores 統合管理クラスを使っているため、
      // serverと同じ名前でsemを登録しようとしてエラー
      clientmf.init(sharemem_key);
      clientmf.read(out revmem);

      if(revmem.resultcode?.IndexOf("OK") >= 0)
      {
        System.Media.SoundPlayer p = new System.Media.SoundPlayer(amem.outwavfile);
        p.Play();
        p.Dispose();

      } else
      {
        string err = "";
      }

      // saltstone.Utils.sleep(50);


      // toneの変更は sample per secを変更する
      // wavファイルを解析する必要があるねー

      // headerのsamping rateを変更するだけでいいのでは？
      // ということは、toneはaquestalkの基本パラメータではないということ
      // sound effectと同じ エコー・ビブラートと同様の扱いになる

      // りばーぶ　＝　集合した残響音の集合　部屋の大きさなどによりパラメータが違う
      // エコー・でぃれい　単体の音を遅くして上書きするもの　でぃれいを長くしたものがエコーとよばれるもよう
      // ビブラート 音声の周波数を波打つように変化させるもの サンプリングレートを変化させればよいのだから、
      //   1s単位とかで区切り、サンプリングレートを集約して落としたり、下に戻したりすればいいのでは？ 音程を上下にゆらす 

      // naudioという c# libraryがあるもよう mit ライセンス 商用可能 ライセンス表記　必

      // string semkey = saltstone.Semaphores.create();

      // server側(x86)でセマフォをたてる
      // sharememでinfileを受け取る
      // sem.waitoneをloopし、signalを受け取ったらinfileをmmfより読み込みwav処理を行う
      // (speedのみ。toneはsound effect classにまかせる)
      // おわったら？ どうやって通知を渡す？ sem, named_pipe?
      // semはsharememの排他制御にも使用する

      // named pipeでincmdを受け取ったら、semをlock -> wav作成 -> sem開放
      // sharememにresultを返す
      // 本体(saltstone側)では、namedpipeを送ったら、少しまってsemをwaitし開放するのを待つ
      //   resultがok or errorになるまでloop
      // これを繰り返す

      // 発音記号への変換は？
      // -> 呼び出し元で行う予定

      // named pipe test
      // https://learn.microsoft.com/ja-jp/dotnet/standard/io/how-to-use-named-pipes-for-network-interprocess-communication
      //System.IO.Pipes.NamedPipeServerStream nps = new System.IO.Pipes.NamedPipeServerStream("saltstonevoice_aq", System.IO.Pipes.PipeDirection.In);
      //nps.WaitForConnection();
      //while(nps.IsConnected == true)
      //{
      //  // System.IO.StreamReader sreader = new System.IO.StreamReader(nps);
      //  System.IO.BinaryReader sreader = new System.IO.BinaryReader(nps);
      //  UnicodeEncoding streamEncoding = new UnicodeEncoding();
      //  int readlen = 256;
      //  byte[] readbuff = new byte[readlen];
      //  // clientからsendされるまでここで待機する
      //  sreader.Read(readbuff, 0, readlen);
      //  // sem wait
      //  string a = streamEncoding.GetString(readbuff);
      //  a = a.Trim('\0');
      //  if (a == "END")
      //  {
      //    break;
      //  }
      //}






    }
  }
}
