using System;
using System.Collections.Generic;
// using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;
using Utils;


namespace saltstone
{
  public partial class frmInstall : Form
  {

    public enum enum_InstallList
    {
      program,
      voice
    }
    public enum enum_InstallStatus
    {
      progress,
      success,
      error
    }


    System.Timers.Timer systimer;
    DataTable table_aviutl;

    public install inst;
    public string tempdir;

    // delegateを設定してダウンロードの進捗を表示させたい
    // install.htmlより読み込んだPG,Voiceを一覧表示させたい

    public delegate void deleg_progres_barval(int i);


    public frmInstall()
    {
      InitializeComponent();
    }

    private void frmInstall_Load(object sender, EventArgs e)
    {
      bool ret = pf_init();
      // fn_updateProgress = new UpdateProgress(this.updateProgress);
      if (ret == false)
      {
        this.Close();
      }

    }



    public void pf_showMessage(string message, int per)
    {
      if (this.IsDisposed == true)
      {
        return;
      }

      this.stpStatus.Items[this.lblMessage.Name].Text = message;
      int i = per;
      if (i > 100)
      {
        i = 100;
      }
      if (i < 0)
      {
        i = 0;
      }
      this.stpProgress.Value = i;
      this.stpStatus.Refresh();
      Utils.Utils.sleep(5);
      // this.Refresh();
    }
    
    /// <summary>
    /// インストールしているスレッドの進捗状況表示
    /// </summary>
    /// <param name="target"></param>
    /// <param name="rowindex"></param>
    /// <param name="status"></param>
    public void pf_setInstallPGStatus(enum_InstallList target, int rowindex,enum_InstallStatus status)
    {
      // TODO aviutl , bouyomiのpathが取得できる -> iniへ保存
      // aviutl,bouyomiのiniがある場合はスキップ -> logへ保存 -> 結果確認で確認できるように
      // logクラスの実装 画面表示用のログ（このPGだとpgのインストール結果)も保存できるように


      // 現在、instはguiと同じスレッドで動くのでinvokeの必要はない
      // 問題は、pg , voiceの２つのリストがある事
      // arg enumでわける
      // さらに進行中、完了、エラーの３つが必要
      // colorはgui側で設定したい

      // まず色の設定
      Color c = Color.White;
      switch (status)
      {
        case enum_InstallStatus.progress:
          c = Color.Turquoise;
          break;
        case enum_InstallStatus.success:
          c = Color.Orange;
          break;
        case enum_InstallStatus.error:
          c = Color.Red;
          break;
      }

      switch (target)
      {
        case enum_InstallList.program:
          lstAviutl.Rows[rowindex].DefaultCellStyle.BackColor = c;
          break;
        case enum_InstallList.voice:
          lstVoice.Rows[rowindex].DefaultCellStyle.BackColor = c;
          break;
      
      }
    }


    private bool pf_init()
    {

      // version check
      string exever = Utils.Sysinfo.getExeVersion();
      // todo htmlのverを"1.0.0.0"に変更
      if (exever != inst.html_pgver)
      {
        string msg = "Version is old";
        MessageBox.Show(msg);
        // TODO ver updateをどうするか？
      }


      string ver = typeof(frmInstall).Assembly.GetName().Version.ToString();
      if (ver != inst.html_pgver)
      {

        string msg = "バージョンが更新されています\r\n";
        msg += "下記よりダウンロードしてください\r\n";
        msg += "https://github.com/fuunacreative/SaltStone";
        System.Media.SystemSounds.Asterisk.Play();
        MessageBox.Show(msg);
      }

      inst.showMessage = this.pf_showMessage;
      inst.fnc_setInstallPGStatus = this.pf_setInstallPGStatus;
      inst.mainform = this;

      // 各ディレクトリの作成
      foreach (directory d in inst.directorys.Values)
      {
        Utils.Files.mkdir(d.path);
      }

      txtPGdir.Text = inst.directorys["program"].path;
      string buff = inst.directorys["voicesoft"].path;
      // txtVoicedir.Text = inst.directorys["voiesoft"].path;

      txtVoicedir.Text = buff;
      // なぜかここでformがshowされ、次のステップが実行されない
      // spell miss key not foundでエラーとなるはずが、すっきぷされ frm showされてた

      // tempdir = inst.directorys["tempdir"].path;
      tempdir = Utils.Files.createTempDirectory();


      table_aviutl = new DataTable();
      DataTable t = table_aviutl;
      DataColumn c = new DataColumn();
      c.ColumnName = "プログラム名";
      // c.Caption = "pluginsaaa";
      t.Columns.Add(c);

      c = new DataColumn();
      c.ColumnName = "インストール先";
      // c.Caption = "filenameaabbb";
      t.Columns.Add(c);

      c = new DataColumn();
      c.ColumnName = "URL";
      // c.Caption = "filenameaabbb";
      t.Columns.Add(c);

      // TODO install を実行し,htmlよりpgdataを読み込む
      /* Programsで既に読み込んでいる
      bool ret = inst.readInstallhtml();
      if (ret == false)
      {
        lblMessage.Text = "インストール定義ファイルの読み込みに失敗しました";
      }
      */
      DataRow r;
      // 親子を検索するためのdic
      Dictionary<string, program> dicprogram = new Dictionary<string, program>();
      // 最後の子を検索するwork
      // Dictionary<int, program> parentlistwork = new Dictionary<int, program>();
      foreach (program p in inst.programs)
      {
        if (p.dispflag == false)
        {
          // 親子の場合どうするの？
          if (p.partentname.Length > 0)
          {
            program parp = dicprogram[p.partentname];
            if (parp.childprogram == null)
            {
              parp.childprogram = new List<program>();
            }
            parp.childprogram.Add(p);
            // 最後だけparentindexを設定したい
            // p.partentrowindex = dicprogram[p.partentname].rowindex;
          }

          continue;
        }
        r = t.NewRow();
        r[0] = p.name;
        if (p.subdir != null && p.subdir.Contains("exe") == false)
        {
          r[1] = p.subdir;
        }
        r[2] = p.url;

        // pgdir を画面からとるので、ここでしか設定できない


        if (p.exefile.Length > 0)
        {
          // exefileが設定されている -> shortcutの作成候補
          buff = txtPGdir.Text + "\\" + p.subdir + "\\" + p.exefile;
          if (Utils.Files.exist(buff) == true)
          {
            // p.exefile = buff; 
            // 実パスを設定
                              // shortcutが作成できることをflagをたてる
            p.existfile = true;
            p.createshortcut_flag = true;
          }
        }
        dicprogram[p.id] = p;

        t.Rows.Add(r);
        p.rowindex = t.Rows.IndexOf(r);

      }
      foreach (program p in inst.programs)
      {
        if(p.childprogram == null)
        {
          continue;
        }
        // 子供がいる場合、最後の子にだけpartentrowindexをセット
        // これでinstall loopの中で最後の子のときだけrow colorが変わる
        p.childprogram.Last().partentrowindex = dicprogram[p.name].rowindex;
        // 子のpにはchildがないので、contされるはず
      }

      dicprogram.Clear();
      dicprogram = null;

      lstAviutl.DataSource = table_aviutl;
      lstAviutl.Refresh();
      lstAviutl.Columns[0].HeaderText = "プログラム";
      lstAviutl.Columns[1].HeaderText = "インストール先";
      lstAviutl.Columns[2].HeaderText = "URL";
      lstAviutl.ClearSelection();
      // TODO readonly lstaviutl

      DataTable dt_voice = new DataTable();
      c = new DataColumn("Install");
      c.DataType = typeof(bool);
      dt_voice.Columns.Add(c);
      c = new DataColumn("ID");
      dt_voice.Columns.Add(c);
      c = new DataColumn("URL");
      dt_voice.Columns.Add(c);
      foreach (voiceDefines v in inst.voiceurls.Values)
      {
        r = dt_voice.NewRow();
        r[0] = v.installflag;
        r[1] = v.name;
        r[2] = v.url;
        dt_voice.Rows.Add(r);
        v.rowindex = dt_voice.Rows.IndexOf(r);

        // voiceのdestdirはinstallが実行されるまで決定しない
        // if (v.exefile.Length > 0)
        //         {
        //  buff = this.txtVoicedir.Text + "\\" + v.destdir
        // }
      }
      lstVoice.DataSource = dt_voice;
      lstVoice.Columns[0].HeaderText = "inst";
      lstVoice.Columns[1].HeaderText = "ソフト名";
      lstVoice.Columns[2].HeaderText = "URL";
      lstVoice.ClearSelection();
      // TODO redonly lstvoice

      // timTimer.Enabled = false;

      this.lblMessage.Text = "";
      // statusbar 経由じゃないとtextがupdateされなかった気がするが,,,
      // this.stpStatus.Items["lblMessage"].Text = "";
      this.stpProgress.Value = 0;
      this.stpProgressDownload.Value = 0;

      systimer = new System.Timers.Timer();
      systimer.Enabled = false;
      systimer.Interval = 100; // mil sec
      systimer.AutoReset = true; // repeat after fire
      systimer.Elapsed += evt_timer;


      // instを作成
      // install.htmlより定義を読み込む
      // tempdir = Utils.getDownloaddir() + "\\" + "temp";
      // directory settingよりtempを削除 util内部で自動で作成する
      // buff = tempdir + "を作業ディレクトリとして使用します";
      // pf_showMessage(buff, 0);
      inst.tempdir = tempdir;
      // SetupGlobals.tempdir = tempdir;
      // どこからも使ってない -> createtempfileを使用するように変更した
      Utils.Files.delete(tempdir);
      Utils.Files.mkdir(tempdir);
      // TODO tempdirのdirを削除する必要がある

      System.Media.SystemSounds.Asterisk.Play();
      // System.Media.SystemSounds.Beep.Play();
      // MessageBox.Show("ディレクトリ:" + tempdir + "はすべて削除されます");

      // インストール先のディスク容量の空スペースをチェック
      Utils.Diskinfo f = Utils.Sysinfo.getDiskUsageInfo();
      lblDiskFreeSpace.Text = f.freespace_str;
      pbDisk.Value = (int)f.usagepercent;
      int ramusage = (int)Utils.Sysinfo.getMemoryInfo().usagepercent;
      pbMemoryzUsage.Value = ramusage;


      // c++ runtime check test
      bool regex = Utils.Sysinfo.regstryKeyExist("");




      return true;
    }



    // aviutlなどのinstallを行う
    // とりあえず、psdtoolkitを設定するところまで
    // aviutl,exedit,gcmzdrops,かんしくん,psdtoolkit
    // キャラ素材
    private void pf_install()
    {

      bool ret;
      stpProgress.Value = 0;
      // timTimer.Enabled = true;


      // throw new Exception("エラーが発生したよ！");

     // TODO timerはutil or timer classで一括管理 and at app exit -> stop all timer
     // installfunc を　threadに
      systimer.Start();

      // ここから後ろを別スレッドにしたほうがいいね
      // タスクにするとdebugがしにくいばかりか、messageが表示されなくなる
      // しかしタイマーが動かなくなる問題が発生する
      // 問題は、downloadのprogressがうまく取得できないこと

      // Task task = Task.Run(() => { ret = inst.installAviutl(); }); 
      inst.destdir = this.txtPGdir.Text;
      // Cursor.Current = Cursors.WaitCursor;
      Utils.mouseCursor.wait();
      // Utils.CursorWait();
      // System.Media.SystemSounds.Question.Play();
      // System.Media.SystemSounds.Asterisk.Play();
      System.Media.SoundPlayer sound = new System.Media.SoundPlayer(SaltstoneSetup.Properties.Resources.start);
      sound.Play();
      sound.Dispose();
      sound = null;

      // installのstepによりlstのrow backcolorを変更したい
      // lstAviutl.Rows[0].DefaultCellStyle.BackColor = Color.Turquoise;

      // TODO formがcloseしてもtimerは生きている
      // 1. install -> thread
      // 2. timer -> listで保管し確実にとめる
      ret = inst.installAviutl();
      systimer.Stop();
      if (ret == false)
      {
        // 処理失敗時は抜ける
        // メッセージはinst内部で設定される
        return;
      }
      // systimer.Stop();
      ret = inst.setupAviutl();
      if (ret == false)
      {
        // 処理失敗時は抜ける
        // メッセージはinst内部で設定される
        return;
      }

      if (this.chkShortcut.Checked == true)
      {
        ret = inst.createPGShortcut();
        if (ret == false)
        {
          // 処理失敗時は抜ける
          // メッセージはinst内部で設定される
          return;
        }
      }
      // Cursor.Current = Cursors.Default;
      // Utils.CursorDefault();
      Utils.mouseCursor.clear();

      // windowが前にでない
      Utils.sleep(500);
      Utils.sleep(500); // たぶん、早すぎる
      this.Focus();
      this.BringToFront();
      Utils.setWindowForground(this);
      Utils.sleep(500);

      sound = new System.Media.SoundPlayer(SaltstoneSetup.Properties.Resources.success);
      sound.Play();
      sound.Dispose();
      sound = null;

      string buff = "ブラウザで表示された各プログラムの注意事項をよくお読みください\r\n";
      buff += "Aviutl・ボイスソフトのインストール後は再起動してください";
      MessageBox.Show(buff);
      // System.Media.SystemSounds.Asterisk.Play();
      Utils.sleep(500);
      this.Focus();
      this.BringToFront();
      Utils.setWindowForground(this);

      //Utils.showBrowserWindow();
      // IntPtr w = this.Handle;

      // どうしてもだめならブラウザをhideに？
      // しかし、どのブラウザが動いているかわからないからな




      // task終了まで待機
      /*
      bool endflag = false;

      do
      {
        Utils.sleep(50);
        if (task.IsCompleted == true)
        {
          endflag = true;
          break;
        }
      } while (endflag == false);
      timTimer.Enabled = false;

      // ret = inst.setupAviutl();
      */
      // timTimer.Enabled = false;

      //intall inst = new intall(buff);
      //bool ret = install.installAviutl();

      // program install

      // HtmlAgilityPack.HtmlNodeCollection nodes = htm.DocumentNode.SelectNodes("//a[contains(@href , 'aviutl')]");
      // string buff = nodes[0].Attributes["href"].Value;

      // url = url + buff;
      // string filename = @"C:\Users\yasuhiko\" + buff;
      // web.DownloadFile(url, filename);

      // TODO 正常にdownloadできたかどうやって判定する？

      // ボイスソフト install

      // chara素材 download


      // web.Dispose();
      // web = null;

      // todo temp以下をすべて削除
      // TODO 専用のクラスを作るべきか？
      //  処理を分けないと可読性が悪い

      // System.IO.Compression.ZipFile.ExtractToDirectory(filename, @"C:\Users\yasuhiko\temp\");

    }

    private void evt_onCreated(object sender, FileSystemEventArgs e)
    {
      if (e.ChangeType != WatcherChangeTypes.Created)
      {
        return;
      }
      string filename = e.FullPath;
      string ext = Utils.Files.getextention(filename);
      if (ext != ".zip")
      {
        return;
      }
      // TODO psd形式のファイルにも対応させる

    }

    private void cmdSpeech_Click(object sender, EventArgs e)
    {
      string buff;
      System.Speech.Synthesis.SpeechSynthesizer synth = new System.Speech.Synthesis.SpeechSynthesizer();
      foreach (System.Speech.Synthesis.InstalledVoice v in synth.GetInstalledVoices())
      {
        System.Speech.Synthesis.VoiceInfo vi = v.VoiceInfo;
        buff = vi.Name;

      }
      string[] castsss = CeVIO.Talk.RemoteService.TalkerAgent.AvailableCasts;
      int i = castsss.Length;
      CeVIO.Talk.RemoteService.Talker a = new CeVIO.Talk.RemoteService.Talker();
      string casts = a.Cast;
      a.Cast = "さとうささら";
      a.Volume = 100;
      a.ToneScale = 100;
      CeVIO.Talk.RemoteService.SpeakingState st = a.Speak("こんにちわ");
      st.Wait();
      a.OutputWaveToFile("こんにちわ", @"C:\Users\yasuhiko\temp\c.wav");

    }

    public void evt_timer(object sender, System.Timers.ElapsedEventArgs e)
    {
      // exception throwされたときもtimerはうごいている

      if (this.IsDisposed == true)
      {
        return;
      }

      if (inst == null)
      {
        return;
      }
      int per = inst.getDownloadPercent();
      Console.WriteLine("tick raise" + per.ToString());
      // Utils.sleep(10);
      if (this.InvokeRequired)
      {
        // perにはちゃんと値が入っているが、progressbarに反映されない
        // stpprogressには値はちゃんと入っている
        /*stpProgress.ProgressBar.Invoke(new Action(
          () =>
          { stpProgress.Value = per; Application.DoEvents(); })
        );*/

        // this.Invoke((Action)delegate { stpProgress.Value = per; Application.DoEvents(); });
        // Utils.sleep(50);
        // this.Invoke((Action)delegate { this.Refresh(); });
        // 値をliteralにすると設定される
        // perがおかしい？
        /*
        progressBar1.Invoke(new Action(
          () => {
            progressBar1.Value = per; Application.DoEvents();
          })
        );
        */
        if (this.IsDisposed == true)
        {
          return;
        }
        ProgressBar b = stpProgress.ProgressBar;
        if (b == null)
        {
          return;
        }
        if (this.IsDisposed == true)
        {
          return;
        }

        b.Invoke(new deleg_progres_barval(progressbaract), new object[] { per });
        // stpProgressDownload.ProgressBar.Invoke(new deleg_progres_barval(progressbaract), new object[] { per });
      } else
      {
        if (stpProgress == null)
        {
          return;
        }
        try
        {
          stpProgress.Value = per;
          this.Refresh();
        }
        catch (Exception ex)
        {
          string buff = ex.Message;
        }

      }
    }

    void progressbaract(int i)
    {
      if (this.IsDisposed == true)
      {
        return;
      }
      // やっぱりうまく動かないや
      // downloadが終わってから急に動き出すかんじだよねー
      if (i < 0)
      {
        // -4000とかになったときがあった
        // 再現しないな-
        i = 0;
      }
      if (i > 100)
      {
        i = 100;
      }
      try
      {
        stpProgressDownload.ProgressBar.Value = i;
        stpProgressDownload.ProgressBar.Update();
        wait(10);
        if (stpProgressDownload.ProgressBar != null)
        {
          stpProgressDownload.ProgressBar.Refresh();
        }
        // this.Refresh();
        // Application.DoEvents();
        // wait(10);
        // this.Refresh();
        wait(10);
      }
      catch (Exception e)
      {
        string buff = e.Message;
        buff = "";
      }
    }


    public void wait(int milliseconds)
    {

      Utils.sleep(milliseconds);
    }

    private void frmInstall_FormClosing(object sender, FormClosingEventArgs e)
    {
      if (inst != null)
      {
        inst.Dispose();
        inst = null;
      }
      // Utils.Dispose(); -> Programsに移動
    }

    private bool pf_installVoice()
    {
      stpProgress.Value = 0;
      // timTimer.Enabled = true;
      systimer.Start();

      bool ret = false;
      Cursor.Current = Cursors.WaitCursor;
      System.Media.SoundPlayer sound = new System.Media.SoundPlayer(SaltstoneSetup.Properties.Resources.start);
      sound.Play();
      sound.Dispose();
      sound = null;

      ret = inst.installVoice(txtVoicedir.Text);
      if (ret == false)
      {
        return ret;
      }
      systimer.Stop();

      ret = inst.setupVoice();
      if (ret == false)
      {
        return ret;
      }

      if (this.chkVoiceShortcut.Checked == true)
      {
        inst.createVoiceShortcut();
      }
      Cursor.Current = Cursors.Default;
      sound = new System.Media.SoundPlayer(SaltstoneSetup.Properties.Resources.success);
      sound.Play();
      sound.Dispose();
      sound = null;

      Utils.setWindowForground(this);


      return ret;
    }

    /*
    private void cmdCharaInstall_Click(object sender, EventArgs e)
    {
      // download folderを取得
      string downloaddir = Utils.Sysinfo.getDownloaddir();

      // ファイルが作られた場合、そのファイルをキャラ素材として登録する
      // んー。これでいいのかな？
      System.IO.FileSystemWatcher watcher = new System.IO.FileSystemWatcher(downloaddir);
      watcher.NotifyFilter = System.IO.NotifyFilters.FileName
        | System.IO.NotifyFilters.CreationTime;
      watcher.Created += evt_onCreated;

    }
    */

    private void frmInstall_Shown(object sender, EventArgs e)
    {
      string buff = tempdir + "を作業ディレクトリとして使用します";
      pf_showMessage(buff, 0);

    }

    private void cmdMaual_Click(object sender, EventArgs e)
    {
      string url = SaltstoneSetup.Properties.Resources.manualurl;
      Utils.openURL(url);

    }

    private void frmInstall_FormClosed(object sender, FormClosedEventArgs e)
    {
      // Utils.Dispose();
      // Utils.Files.rmdir(tempdir);
      // この方法だと、at exceptionで残る
      // Programsのtry finallyでutils.dispose()を実行
      // timerが停止しないので、無理やりいれてる。しかしタイマーは動作しっぱなしになる
      // Application.Exit();
    }

    private void cmdInstallProgram_Click(object sender, EventArgs e)
    {
      pf_install();
    }

    private void cmdInstallPG_tsb_Click(object sender, EventArgs e)
    {
      pf_install();
    }

    private void cmdMenuInstallPG_Click(object sender, EventArgs e)
    {
      pf_install();
    }

    private void CmdMenuInstallVoice_Click(object sender, EventArgs e)
    {
      pf_installVoice();
    }
  }
}


/*
 *
 *0x0800ef02
08 -> major
00 -> minor
ef02 -> subminor?
8.0.61186

Microsoft Visual C++ 2005 Redistributable (x64)
 
 */