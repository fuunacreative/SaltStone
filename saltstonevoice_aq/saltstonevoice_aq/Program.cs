using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;



// TODO logの簡易版作成
// named pipe,semaphore,sharemem, loop処理の作成

// named pipeがclientより切断されると、pgが終了する


namespace saltstonevoice_aq
{
  static class Program
  {
    /// <summary>
    ///  aquestalkをcallするための32bit exe
    ///  saltstone(x64)とrpcにて連携し、wavファイルを作成する
    ///  in cmd.txt(outfname,phoneticstring) , semaphore countup
    ///  out out.txt(outfname,resultcode), semaphore countup
    ///  sharemem使うか、今後の課題洗い出しにもなるしねー
    ///  named pipeでincmd.txtを受け取り -> semaphore lock&sharememory access -> 
    ///  -> aquestalk dll call -> wavファイル作成
    ///  exeを実行中のままにするか or wav作成のたびにexeをstartするか？
    /// </summary>
    [STAThread]
    static void Main(string[] args)
    {
      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);
      if (args.Length == 1)
      {
        makewave(args[0]);
        // Application.Exit();
        // Environment.Exit(0);
      }
      
      // namedpipe,sharememのlisten server起動
      AQGlobal.aqserver = new AQProcess();
      AQGlobal.aqserver.init();
      AQGlobal.aqserver.startNamedpipeloop();

      // Application.Run(new Form1());
      AQGlobal.aqserver.Dispose();
    }

    // sharemem
    // 1. jobid
    // 2, phonecit string
    // 3, outfname
    // 4, result
    // 5, order timestamp
    // 6. result timestamp

  

    // 第一段階 arg cmdを受け取り、wavを返す
    private static void makewave(string argfname)
    {
      AQvoices.init();


      AQcmd cmd = new AQcmd();
      cmd.parse(argfname);
      cmd.createwav();

      // settings.dbより作成するclass nameを取得



    }
 
  }
}
