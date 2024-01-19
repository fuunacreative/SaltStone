using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
// TODO でもに向けて
// 保存メッセージ -> json or xml export

// 内部構造定義
// float windowの実装の検証
// レイヤーの管理



// 課題 pgbarが黄色にならない（disk free space)


namespace saltstone
{

  public partial class frmCharadefine : Form, Inf_FormSetFace
  {
    // 上半分の左右分割度合いの保存用 gui
    private int gui_toppanel_splitdistance;

    // 上段に配置する、今回定義するchara
    private DataTable table_chara;

    // 下段の立ち絵
    public DataTable table_charapicture;

    // キャラ設定で現在選択されている行
    private int selectedchara_rowindex = -1;

    // キャラ選択画面
    public FrmCharaSelect form_charasect;

    // 右上サイド（キャラ定義セット、キャラレイヤー設定）からコールされるevent
    public delegate void del_righttopClose();

    // キャラ定義セットが変更された時に実行されるevent
    public delegate void del_charasetChangeed();
    // 検討 メインフォームを各子フォームに持たせてpublic funcをよぶか？それともメインのdelを定義させておいてメインのfuncをよぶか？
    // floatingも検討しないといけないね

    CheckBox dwWatch; // chara picture directory 監視用

    Utils.Timer systeminfowatchtimer; // disk memory 監視タイマー
    private bool formCloseingflag = false; // formがclose中かどうかのフラグ

    // libraryMessage libmessage;

    public frmCharadefine()
    {
      InitializeComponent();
    }

    private void evt_setSystemStatus(object sender, System.Timers.ElapsedEventArgs e)
    {
      pf_systemstatus_setpgbar();
    }
    private void pf_systemstatus_setpgbar()
    {
      if (formCloseingflag == true)
      {
        return;
      }
      Utils.MemoryInfo mem = Utils.Sysinfo.getMemoryInfo();
      Utils.Diskinfo disk = Utils.Sysinfo.getDiskUsageInfo();
      int diskvalue = (int)disk.usagepercent;
      int memvalue = (int)mem.usagepercent;
      // カプセル化できないか？
      if (InvokeRequired == true)
      {
        // TODO はきされたobject frmcharadefineにアクセスできません
        this.Invoke((MethodInvoker)(() => {
          Utils.setProgressbarColor(this.pbDisk, diskvalue);
          Utils.setProgressbarColor(this.pbMemory, memvalue);
          //IntPtr h = this.pbDisk.ProgressBar.Handle;
          //Utils.win32api.SendMessage(h, Utils.win32api.WMMessage.BM_SETBARCOLOR, 3, 0);
          //this.pbDisk.Value = diskvalue;
          //this.pbMemory.Value = memvalue;
        }));
        return;
      }
      // this.pbDisk.Value = diskvalue;
      // this.pbMemory.Value = memvalue;
      Utils.setProgressbarColor(this.pbDisk, diskvalue);
      Utils.setProgressbarColor(this.pbMemory,memvalue);
    }

    private void frmCharadefine_Load(object sender, EventArgs e)
    {
      pf_init();
    }

    private void evt_DownloaWatchStart(object sender, EventArgs s)
    {
      if (sender.GetType() != typeof(CheckBox))
      {
        return;
      }
      CheckBox chk = (CheckBox)sender;
      if (chk.Checked == false)
      {
        _downloadwatcher.Dispose();
        return;
      }
      string dwdir = Utils.Sysinfo.getDownloaddir();
      _downloadwatcher = new Utils.directoryWatcher(dwdir);
      _downloadwatcher.fileextension = "*.zip|*.psd";
      _downloadwatcher.evt_filecreated += evt_downloaded;
      _downloadwatcher.start();
   }
    private Utils.directoryWatcher _downloadwatcher;
    private bool evt_downloaded(string filename)
    {
      // charasは識別子 r or mの一文字をもった立ち絵＋音声
      // じゃあ、立ち絵(portrait)はどうやって識別するの？
      // dirname or psdname?
      // portraitは別に管理しなきゃいけないかも
      string buff = filename + "をダウンロードを検出しました。解析開始を開始します";
      Logs.write(buff, Logs.Logtype.dispinfo);
      // Globals.messagectl.showMessage(buff);
      // libmessage.showMessage(buff);
      // 解析処理をasyncで行う
      // 処理中のprogressと終了時のimage表示処理
      // asyncで行う必要ある？ -> 解析にどのぐらいかかるかによる
      string portraitid = "";
      bool ret = Globals.charapotrait.addChara_portrait(filename,out portraitid);
      // 正常に追加されたらaddedcharaに登録したchara objを返す
      // 代表立ち絵はpicturefile_representationに保存される
      /*
      this.Invoke((MethodInvoker)(() => {
        lblMessage.Text = buff;
      }));
      */
      Image repcharapicture = null;
      // キャラ解析が正常に行え、addedcharaがちゃんと設定された場合、代表立ち絵をpctboxに表示
      if (portraitid != null)
      {
        // TODO charasではなくportraitsから代表立ち絵を表示する
        repcharapicture = Bitmap.FromFile(CharaGlobals.charas[portraitid].picturefile_representation);
      }
      this.Invoke((MethodInvoker)(() => {
        lblMessage.Text = buff;
        pctCharaPicture.Image = repcharapicture;
      }));

      return true;
    }

    private void pf_init()
    {
      /*
      lstCharaset.Rows.Clear();
      lstCharaset.Columns.Clear();
      DataTable charaset = new DataTable();
      charaset.Columns.Add("charaset");
      DataRow r = charaset.NewRow();
      r[0] = "Default";
      charaset.Rows.Add(r);
      lstCharaset.DataSource = charaset;
      */

      Globals.init();

      // for test
      // Logs.write("test dayo from sschara");
      // Exception e =  new Exception("this is exceptions");
      // Logs.write(e);

      // init control
      this.pctCharaPicture.Image = null;

      int i = splitContainer_top.Width;
      // gui_toppanel_left = splitContainer_top.Panel1.Width;
      // gui_toppanel_right = splitContainer_top.Panel2.Width;
      // splitContainer_top.Panel1.Width = i;
      gui_toppanel_splitdistance = splitContainer_top.SplitterDistance;
      //  左側のキャラ一覧を引き伸ばす
      splitContainer_top.SplitterDistance = splitContainer_top.Width;
      lblCharadefMessage.Text = "";
      Control ctl = (Control)pctCharaPicture;
      ctl.AllowDrop = true;

      lstDefinedCharaSet.AutoGenerateColumns = false;

      // libraryからのメッセージを表示させるためのobjを初期化
      
      // libmessage = new libraryMessage(lblMessage, pbProgress);
      // Globals.charas.setMainFormMessage(libmessage);

      // TODO disk memoryのpgbarは右端に。しかも空き容量により色を変更したい

      // ダウンロード監視のcheckbox追加
      dwWatch = new CheckBox();
      dwWatch.Text = "ダウンロード監視";
      dwWatch.CheckedChanged += evt_DownloaWatchStart;
      // dwを監視し、downloadフォルダにファイルができたら立ち絵として取り込みを試す
      // dw中はファイルのサイズが０ もしくは、徐々に増えていくと思われる
      // firefoxでは２つのファイルが作成され、partとexeの２つ
      // dw完了とともにpartファイルは削除され、exeが残る
      // zip or psdを監視すればよい sizeが0以上、timerで1sたってもサイズが変化しない
      // これが条件だな -> d&dの手間を省くことができる
      int chkins = toolStrip_charapicture.Items.Count;
      ToolStripControlHost host = new ToolStripControlHost(dwWatch);
      // chkins++;
      toolStrip_charapicture.Items.Insert(chkins, host);


      // ディスク容量・メモリ使用量の監視タイマー
      systeminfowatchtimer = Utils.Timer.getTimer();
      systeminfowatchtimer.evt_timer += evt_setSystemStatus;
      systeminfowatchtimer.start(5000, Utils.Timer.enum_repeatTimer.repeat);


      // splitContainer_top.Panel2.Width = 0;

      // defined charasetの定義
      // webよりjsonをダウンロード
      // -> 内部dbに保存
      // table publicdefinitionをどうやってdatatableにするか？
      //DataTable dt = new DataTable();
      //DataColumn dc;

      //foreach (DataGridViewColumn c in lstDefinedCharaSet.Columns)
      //{
      //  dc = new DataColumn(c.HeaderText);
      //  dt.Columns.Add(dc);
      //}
      DataTable dt = chara_publicdefinitions.getAllList();
      lstDefinedCharaSet.Rows.Clear();
      lstDefinedCharaSet.DataSource = dt;

      // TODO datatable に rowindexを追加　ここにdatasourceとgridviewのindexを保存し、searchできるようにしておく
      Dictionary<int, int> rowindexbyid = new Dictionary<int, int>();

      foreach (DataGridViewRow dr in lstDefinedCharaSet.Rows)
      {
        int id = Utils.toint(dr.Cells["id"].Value);
        rowindexbyid.Add(id, dr.Index);
      }

      foreach (DataRow dr in dt.Rows)
      {
        string hash = Utils.tostr(dr["picturecharahash"]);
        bool ret = Globals.charapotrait.isInstalled(hash);
        if (ret == false)
        {
          int id = Utils.toint(dr["id"]);
          lstDefinedCharaSet.Rows[rowindexbyid[id]].Cells["picturechara"].Style.BackColor = Color.Red;
        }
      }

      // 二次配布禁止なんだから、ダウンロードさせないとだめ
      string selects = "id = 2";
      DataRow[] srow = dt.Select(selects);
      // DataGridView1.Columns("Column1").DataPropertyName = "Col1"
      // sstDefinedCharaSet.Columns[0].DataPropertyName = "charaname";
      // table columnとgridviewの紐付けを簡単にできないか？


      // TODO javascriptでれいむのキャラまでページ移動できないか？


      /*
      dt.Columns.Add("キャラ");
      dt.Columns.Add("作成者");
      dt.Columns.Add("タグ");
      dt.Columns.Add("立ち絵");
      dt.Columns.Add("絵師");
      dt.Columns.Add("ボイスソフト");
      dt.Columns.Add("ボイスキャラ");
      dt.Columns.Add("エンジン");

      */


      //r = null;
      //r = dt.NewRow();
      //r[0] = "うｐ主";
      //r[1] = "fuuna";
      //r[2] = "Factorio";
      //r[3] = ""; // 立ち絵素材
      //r[4] = "";
      //r[5] = "AquesTalk";
      //r[6] = "中性";
      //r[7] = "AquesTalk";
      //dt.Rows.Add(r);

      // lstDefinedCharaSet.Columns.Clear();

      lstVoicesoft.SelectedIndex = 0;
      dt = null;
      DataRow r;
      DataColumn dc;

      r = null;


      lstVoicetype.Rows.Clear();
      lstVoicetype.Columns.Clear();

      dt = new DataTable();
      dt.Rows.Clear();
      dt.Columns.Clear();
      dt.Columns.Add("声質");
      r = dt.NewRow();
      r[0] = "女性１";
      dt.Rows.Add(r);
      r = dt.NewRow();
      r[0] = "女性２";
      dt.Rows.Add(r);
      r = dt.NewRow();
      r[0] = "男性１";
      dt.Rows.Add(r);
      r = dt.NewRow();
      r[0] = "男性２";
      dt.Rows.Add(r);
      r = dt.NewRow();
      r[0] = "中性";
      dt.Rows.Add(r);
      r = dt.NewRow();
      r[0] = "ロボット";
      dt.Rows.Add(r);
      r = dt.NewRow();
      r[0] = "機械１";
      dt.Rows.Add(r);
      r = dt.NewRow();
      r[0] = "機械２";
      dt.Rows.Add(r);

      lstVoicetype.DataSource = dt;
      dt = null;
      r = null;





      r = null;

      // 本logi
      table_chara = new DataTable();
      // lstChara.DataSource = table_chara;
      // int ii = 0;
      foreach (DataGridViewColumn cc in lstChara.Columns)
      {
        dc = new DataColumn(cc.HeaderText);
        table_chara.Columns.Add(dc);
        // buttonをcellに追加しようとしたら、
        // columnに対してaddする必要がある
        /*
        if (ii == 5)
        {
          lstChara.Columns.Add(cc.HeaderText, cc.HeaderText);
        } else
        {
          lstChara.Columns.Add(cc.HeaderText, cc.HeaderText);
        }
        ii++;
        */

      }
      lstChara.Rows.Clear();
      // lstChara.Columns.Clear();
      // lstChara.DataSource = table_chara;
      // lstChara.Columns[1].CellType = GetType(DataGridViewImageCell);
      // lstChara.Columns[1].columntype
      dc = null;

      table_charapicture = new DataTable();
      table_charapicture.Columns.Add("立ち絵名");
      table_charapicture.Columns.Add("形式");
      table_charapicture.Columns.Add("ファイル");
      table_charapicture.Columns.Add("アニメ");
      table_charapicture.Columns.Add("オリジナル");
      table_charapicture.Columns.Add("コメント");
      lstCharapicture.Rows.Clear();
      lstCharapicture.Columns.Clear();

      // test data
      // pf_addChara("れいむ");
      // pf_addChara("まりさ");

      pf_systemstatus_setpgbar();

      Utils.mouseCursor.clear();

    }

    private void cmdOpen_Click(object sender, EventArgs e)
    {
      // this.dlgDirectory.RootFolder = Environment.SpecialFolder.MyComputer;
      // this.dlgDirectory.SelectedPath = "C:\\Users\\yasuhiko";
      // this.dlgDirectory.SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
      // this.dlgDirectory.RootFolder = Environment.SpecialFolder.MyVideos;
      FolderBrowserDialog d = new FolderBrowserDialog();
      // d.RootFolder = Environment.SpecialFolder.MyComputer;
      d.SelectedPath = "C:\\Users\\yasuhiko\\Videos";
      DialogResult ret = d.ShowDialog(this);
      if (ret != DialogResult.OK)
      {
        return;
      }
      // キャラ素材(dir)を読み込み
      DataTable dt = new DataTable();
      dt.Columns.Add("name");
      dt.Columns.Add("type");
      dt.Columns.Add("path");
      dt.Columns.Add("コメント");
      DataRow r = dt.NewRow();
      r[0] = "新れいむ";
      r[1] = "dir";
      r[2] = @"C:\Users\fuuna\Videos\chara\新れいむ";
      dt.Rows.Add(r);
      lstCharapicture.Rows.Clear();
      lstCharapicture.Columns.Clear();
      lstCharapicture.DataSource = dt;

      pf_addnewchara("れいむ");
    }

    private void pf_addnewchara(string charaname)
    {
      DataTable dt = new DataTable();
      dt.Columns.Add("キャラ名");
      dt.Columns.Add("キャラ", typeof(Image));
      dt.Columns.Add("略文字");
      dt.Columns.Add("立ち絵ID"); //dir れいむとか
      dt.Columns.Add("標準立ち絵"); //image



    }

    private void cmdCharaFavorite_Click(object sender, EventArgs e)
    {
      // FrmCharaSelect.func_setChara("れいむ");
      FrmCharaSelect.showwindows("れいむ");
      FrmCharaSelect.exf_getImageSelected = evt_charaselect;

    }

    private void evt_charaselect(string charaid, string oartsid, Image img)
    {
      // キャラ選択画面でキャラセレクトが行われた
      pctCharaPicture.Image = img;
      // セットされたキャラのidを取得
      string buff = FrmCharaSelect.selectedCharaID;
      int row = selectedchara_rowindex;
      lstChara.Rows[row].Cells[4].Value = buff;
    }

    public void dock(frmFloat_CharaSet f)
    {
      f.TopLevel = false;
      f.Width = 30;
      f.FormBorderStyle = FormBorderStyle.None;
      splitContainer_top.Panel1.Controls.Add(f);
      f.Dock = DockStyle.Fill;
    }

    private void toolStripContainer1_RightToolStripPanel_Click(object sender, EventArgs e)
    {

    }

    private void cmdShowCharaset_Click(object sender, EventArgs e)
    {
      pf_float_showCharaSet();
    }

    private void pf_float_showCharaSet()
    {
      // splitContainer_top.Panel2.Width = 0;
      splitContainer_top.SplitterDistance = gui_toppanel_splitdistance;
      // TODO キャラセットをflotingにし、別winにする
      // dockできないか確認
      frmFloat_CharaSet f = new frmFloat_CharaSet();
      /*
       * 
       * floatingさせる場合
      f.mainform = this;

      f.Owner = this;
      f.Show();
      */
      f.TopLevel = false;
      f.FormBorderStyle = FormBorderStyle.None;
      // f.Owner = this;
      splitContainer_top.Panel2.Controls.Clear();
      splitContainer_top.Panel2.Controls.Add(f);
      f.Visible = true;
      f.Dock = DockStyle.Fill;
    }

    private void lstDefinedCharaSet_MouseDown(object sender, MouseEventArgs e)
    {
      // row selectにしたのはいいが、column headerがselectされる?　
      // enable header visual styleにするとどうしても headerがselect状態になる
      DataGridView.HitTestInfo info = lstDefinedCharaSet.HitTest(e.X, e.Y);
      if (info.RowIndex < 0)
      {
        return;
      }
      if (info.ColumnIndex < 0)
      {
        return;
      }
      // このイベントが走っていdef,1るので、clicdef,1k eventが走らない
      // e.clicksは必ず1になるな
      //if (e.Clicks == 1)
      // {
      //  return;
      //}
      // TODO mousedown eventを実装すると click eventが発生しない
      // doDragDropを実行する前に処理を行い、d&ddef,1とclickが正常に動作するような仕組みが必要
      if (info.ColumnIndex == 5)
      {
        // link textが押された -> urlを開く
        pf_definedchara_openurl(info.RowIndex, info.ColumnIndex);
        return;
      }

      string text = (String)lstDefinedCharaSet.Rows[info.RowIndex].Cells[0].Value;
      if (text != null)
      {
        text = "def," + text;
        lstDefinedCharaSet.DoDragDrop(text, DragDropEffects.Copy);
      }

    }

    private void lstChara_DragEnter(object sender, DragEventArgs e)
    {
      e.Effect = DragDropEffects.Copy;
    }

    private void lstChara_DragDrop(object sender, DragEventArgs e)
    {
      string buff = (string)e.Data.GetData(typeof(string));

      if (buff.Contains("def") == true)
      {
        string[] ary = buff.Split(',');
        buff = ary[1];
        pf_addChara(buff);
        return;
      }

      if (lstChara.SelectedCells == null)
      {
        return;
      }
      if (lstChara.SelectedCells.Count == 0)
      {
        return;
      }
      // これから後ろはlstcharaに行がある状態で動作する
      int row = lstChara.SelectedCells[0].RowIndex;
      if (buff.Contains("pct") == true)
      {
        string[] ary = buff.Split(',');
        buff = ary[1];
        lstChara.Rows[row].Cells[3].Value = "dir," + buff;

      } else if (buff.Contains("voice") == true)
      {
        string[] ary = buff.Split(',');
        buff = ary[1];
        // int row = lstChara.SelectedRows[0].Index;
        lstChara.Rows[row].Cells[5].Value = "AquesTalk," + buff;
      } else if (buff.Contains("charaselect") == true)
      {
        string[] ary = buff.Split(',');
        // charaselect,れいむ,set:1"
        buff = ary[2];
        lstChara.Rows[row].Cells[4].Value = buff;
        // chara pictureの変更
        Image img = FrmCharaSelect.exf_getSelectedCharaImage();
        pctCharaPicture.Image = img;
      }

    }
    private void pf_addChara(string charaname)
    {
      if (charaname != "れいむ" && charaname != "まりさ" && charaname != "ようむ")
      {
        return;
      }
      DataRow r = table_chara.NewRow();
      r[0] = charaname;
      r[6] = "Play"; // button
      r[8] = "";
      if (charaname == "れいむ")
      {
        pf_testreimu(ref r);
      } else if (charaname == "まりさ")
      {
        pf_testmarisa(ref r);
      } else if (charaname == "ようむ")
      {
        pf_testyoumu(ref r);
      }

      /*
      DataGridViewButtonCell bt = new DataGridViewButtonCell();
      bt.UseColumnTextForButtonValue = true;
      r[6] = bt;
      */
      table_chara.Rows.Add(r);

      pf_addcharadgrid(r); // datatableからgridへ展開する button,imgがあるためdatasourceによるdispはできない
      pf_setVoiceByChara();
      // datasourceを使うと全部stringとして扱われてしまうため
      // 自前でtableを表示する
      /*
      foreach (DataGridViewRow dr in lstChara.Rows)
      {
        // dr.Cells[6].Value = "Play";
        // dr.Cells[6] = bt;
      }
      // lstChara.DataSource = null;
      // lstChara.DataSource = table_chara;

      // 自前で表示したほうがよさげだねー
      */

      //立ち絵のセルを赤色に変えてメッセージを表示する

      //if (initdownload == false)
      //{
      //  System.Media.SystemSounds.Asterisk.Play();
      //  string buff = "立ち絵[" + charaname + "]がインストールされていませんダウンロードしてください";
      //  lblCharadefMessage.Text = buff;
      //  // MessageBox.Show();
      //  // buff = "http://www.nicotalk.com/charasozai.html";
      //  // Utils.openURL(buff);
      //  // initdownload = true;
      //}
      //// Utils.setFormForground(this);
      //tabControl.SelectedTab = tabpCharPicture;
      //lblMessage.Text = "ダウンロードしたキャラ素材を立ち絵リストにDropしてください";

    }
    // private bool initdownload = false; // for test

    private void pf_addcharadgrid(DataRow r)
    {
      int i = lstChara.Rows.Add();
      DataGridViewRow dest = lstChara.Rows[i];
      dest.Cells[0].Value = r[0];
      // dest.Cells[1].Value = r[1]; bitmap
      dest.Cells[2].Value = r[2];
      dest.Cells[3].Value = r[3];
      dest.Cells[4].Value = r[4];
      dest.Cells[5].Value = r[5];
      // dest.Cells[6].Value = r[6]; //  btnだが表示させるテキストを設定
      dest.Cells[7].Value = r[7];
      dest.Cells[8].Value = r[8];

      // column 1 => bitmap
      DataGridViewImageCell imgcell = new DataGridViewImageCell();
      imgcell.Value = new Bitmap((string)r[1]);
      imgcell.ImageLayout = DataGridViewImageCellLayout.Zoom;
      dest.Cells[1] = imgcell;
      // column 6 button
      DataGridViewButtonCell btn = new DataGridViewButtonCell();
      btn.Value = "Play";

      // btn.UseColumnTextForButtonValue = true;
      dest.Cells[6] = btn;

      // 問題はセルの値が変更されてもdatasourceには反映されないってこと
      // 表示はgridview,内部保持はtableで別にしなきゃいけない
      // 保存のタイミングも問題

    }


    private void pf_testreimu(ref DataRow r)
    {
      // r[1] = img;
      r[1] = @"C:\Users\fuuna\source\saltstone\resource\れいむ.png";
      r[2] = "r";
      r[3] = "dir,れいむ";
      r[4] = "set:1";//  default立ちえ
      r[5] = "AquesTalk,女性１";
      // r[6]  play button
      r[7] = "100"; // tone
      r[8] = "110"; // speed
    }
    private void pf_testmarisa(ref DataRow r)
    {
      r[1] = @"C:\Users\fuuna\source\saltstone\resource\まりさ.png";
      r[2] = "m";
      r[3] = "dir,まりさ";
      r[4] = "set:1";//  default立ちえ
      r[5] = "AquesTalk,女性２";
      // r[6]  play button
      r[7] = "95"; // tone
      r[8] = "115"; // speed

    }
    private void pf_testyoumu(ref DataRow r)
    {
      r[1] = @"C:\Users\fuuna\source\saltstone\resource\ようむ.png";
      r[2] = "y";
      r[3] = "dir,ようむ";
      r[4] = "set:1";//  default立ちえ
      r[5] = "AquesTalk,女性１";
      // r[6]  play button
      r[7] = "130"; // tone
      r[8] = "76"; // speed

    }

    private void cmdOpenCharadir_Click(object sender, EventArgs e)
    {
      dlgDirectory.SelectedPath = "";
    }

    private void lstCharapicture_DragEnter(object sender, DragEventArgs e)
    {
      e.Effect = DragDropEffects.Copy;
    }

    private void lstCharapicture_DragDrop(object sender, DragEventArgs e)
    {
      // 正しいdataがdropされたかチェック
      // zipだったら展開
      if (e.Data.GetDataPresent(DataFormats.FileDrop))
      {
        string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
        pf_Charapicture_drop(files);
      }
    }

    private void pf_Charapicture_drop(string[] files)
    {
      // TODO originalを保存
      // メモを追加 編集可能とする
      // 展開したものをVideos\CharaPictureに展開
      // どのたち絵師が作ったのか？判断できない
      // また、URLの表示もほしい
      // hashを計算し、ファイルのダウンロード先を特定し、立ち絵師とurlを特定するか？

      foreach (string f in files)
      {
        pf_dropPicture(f);
      }
      if (lstCharapicture.Rows.Count == 0)
      {
        return;
      }
      lstCharapicture.Rows[0].Selected = true;

    }


    private void pf_dropPicture(string filename)
    {
      string fext = Utils.Files.getextention(filename);
      if (fext.ToLower() != ".zip" || fext.ToLower() != ".psd")
      {
        return;
      }

      // 元ファイルを退避
      // globals.charpicturedir_original
      // ちょーっとまてよ？ install.htmlを触るとreleaseに影響がでる
      // -> ということは、WEBシステムから作らないとまずいことになる
      // いやいや。とりあえずはインストールはできてるから後回しでいい
      // common libraryに対しファイルの追加処理を依頼
      // 追加した結果としてcharapictureクラスをもらう



      string file = Utils.Files.getfilename(filename);
      string charaname = Utils.Files.getbasename(file);
      // string buff = @"C:\Users\fuuna\Downloads\新れいむ.zip";
      DataRow r = table_charapicture.NewRow();
      r[0] = charaname;
      r[1] = "dir";
      r[2] = charaname;
      r[3] = "ON";
      r[4] = filename;
      table_charapicture.Rows.Add(r);
      lstCharapicture.DataSource = table_charapicture;

      // TODO originalをどうするか？
      // charaディレクトリにoriginalとして保存するか？それとも削除？
      // そもそもoriginalいる？


      //if (buff.Contains("れいむ") != true)
      //{
      //  return;
      //}
      //DataRow r = table_charapicture.NewRow();
      //r[0] = "新れいむ"; // 立ち絵
      //r[1] = "dir"; // キャラ素材形式
      //r[2] = "新れいむ"; // dirname
      //r[3] = "ON"; // アニメするかどうか
      //r[4] = buff; // original filename;
      //table_charapicture.Rows.Add(r);
      //lstCharapicture.DataSource = table_charapicture;

    }

    private void lstChara_CellContentClick(object sender, DataGridViewCellEventArgs e)
    {
      int row = e.RowIndex;
      int col = e.ColumnIndex;
      if (col != 6)
      {
        return;
      }
      string chara = (string)lstChara.Rows[row].Cells[0].Value;
      test_setpicture(chara);
      btnPlay_Click(chara);
    }

    private void test_setpicture(string chara)
    {
      string fname = @"C:\Users\fuuna\source\saltstone\resource\";
      if (chara == "れいむ")
      {
        fname = fname + "れいむ.png";
      } else
      {
        fname = fname + "まりさ.png";
      }
      pctCharaPicture.ImageLocation = fname;
    }

    private void btnPlay_Click(string charaid)
    {
      string fname = "";
      if (charaid == "れいむ")
      {
        fname = @"C:\Users\fuuna\source\saltstone\resource\samplevoice_reimu.wav";
      } else if (charaid == "まりさ")
      {
        fname = @"C:\Users\fuuna\source\saltstone\resource\samplevoice_marisa.wav";
      }
      // test sound play
      Utils.Sound.play(fname);

    }

    private void lstChara_RowEnter(object sender, DataGridViewCellEventArgs e)
    {
      this.selectedchara_rowindex = e.RowIndex;
      // ボイスバーを動かす
      pf_setVoiceByChara(e.RowIndex);
      if (lstCharapicture.Rows.Count == 0)
      {
        return;
      }
      string fname = @"C:\Users\fuuna\source\saltstone\resource\れいむ.png";
      Image img = Image.FromFile(fname);
      pctCharaPicture.Image = img;
      img = null;

    }

    private void pf_setVoiceByChara(int rowid = -1)
    {
      int row = rowid;
      if (row == -1)
      {
        row = selectedchara_rowindex;
      }
      if (row == -1)
      {
        return;
      }


      string buff = (string)lstChara.Rows[row].Cells[5].Value;
      if (buff == null)
      {
        return;
      }
      string voicesoft = "";
      string voicetype = "";
      if (buff.Contains(",") == true)
      {
        string[] ary = buff.Split(',');
        voicesoft = ary[0];
        voicetype = ary[1];
      }
      buff = (string)lstChara.Rows[row].Cells[7].Value;
      // buff はstring or intが入ってる
      int tone = 100;
      bool ret = int.TryParse(buff, out tone);
      buff = (string)lstChara.Rows[row].Cells[8].Value;
      int speed = 100;
      ret = int.TryParse(buff, out speed);
      // 本来であれば内部にvoicesoftのclass dictionaryを保存しておくべき
      pf_setListbox(lstVoicesoft, voicesoft);
      foreach (DataGridViewRow r in lstVoicetype.Rows)
      {
        if ((string)r.Cells[0].Value == voicetype)
        {
          r.Selected = true;
          break;
        }
      }
      trkTone.Value = tone;
      trkSpeed.Value = speed;
    }

    private void pf_setListbox(ListBox lb, string item)
    {
      int i = 0;
      foreach (string ls in lb.Items)
      {
        if (ls == item)
        {
          i = lstVoicesoft.Items.IndexOf(ls);
          break;
        }
      }
      lstVoicesoft.SelectedIndex = i;
    }

    private void lstCharapicture_MouseDown(object sender, MouseEventArgs e)
    {
      DataGridView.HitTestInfo info = lstCharapicture.HitTest(e.X, e.Y);
      if (info.RowIndex >= 0)
      {
        string text = (String)lstCharapicture.Rows[info.RowIndex].Cells[0].Value;
        text = "pct," + text;
        if (text != null)
        {
          lstCharapicture.DoDragDrop(text, DragDropEffects.Copy);
        }
      }

    }

    private void lstVoicetype_MouseDown(object sender, MouseEventArgs e)
    {
      DataGridView.HitTestInfo info = lstVoicetype.HitTest(e.X, e.Y);
      if (info.RowIndex >= 0)
      {
        string text = (String)lstVoicetype.Rows[info.RowIndex].Cells[0].Value;
        text = "voice," + text;
        if (text != null)
        {
          lstVoicetype.DoDragDrop(text, DragDropEffects.Copy);
        }
      }

    }

    private void trkTone_ValueChanged(object sender, EventArgs e)
    {
      txtTone.Text = trkTone.Value.ToString();
      if (selectedchara_rowindex == -1)
      {
        return;
      }
      lstChara.Rows[selectedchara_rowindex].Cells[7].Value = trkTone.Value.ToString();

    }


    private void trkSpeed_ValueChanged(object sender, EventArgs e)
    {
      txtSpeed.Text = trkSpeed.Value.ToString();
      if (selectedchara_rowindex == -1)
      {
        return;
      }
      lstChara.Rows[selectedchara_rowindex].Cells[8].Value = trkSpeed.Value.ToString();

    }

    private void lstVoicetype_RowEnter(object sender, DataGridViewCellEventArgs e)
    {
      int row = e.RowIndex;
      string voicetype = (string)lstVoicetype.Rows[row].Cells[0].Value;
      if (selectedchara_rowindex == -1)
      {
        return;
      }
      string buff = "";
      buff = "AquesTalk," + voicetype;

      lstChara.Rows[selectedchara_rowindex].Cells[5].Value = buff;

    }

    private void cmdSave_Click(object sender, EventArgs e)
    {
      lblMessage.Text = "保存しました";
      System.Media.SystemSounds.Asterisk.Play();

    }

    public void setFace(string charaid, string partsstr)
    {
      // partsidからpngを取得しpictureboxにセット
      // deletege -> eventかする
      // これはメイン側が主導権を握る場合に返された値を元に編集する場合
      // 単純な情報取得はeventで行う
    }


    private void pctCharaPicture_DragDrop(object sender, DragEventArgs e)
    {
      string buff = (string)e.Data.GetData(typeof(string));

      if (lstChara.SelectedCells == null)
      {
        return;
      }
      if (lstChara.SelectedCells.Count == 0)
      {
        return;
      }

      int row = lstChara.SelectedCells[0].RowIndex;
      if (buff.Contains("charaselect") == true)
      {
        string[] ary = buff.Split(',');
        // charaselect,れいむ,set:1"
        buff = ary[2];
        lstChara.Rows[row].Cells[4].Value = buff;
        // chara pictureの変更
        Image img = FrmCharaSelect.exf_getSelectedCharaImage();
        pctCharaPicture.Image = img;
      }

    }

    private void pctCharaPicture_DragEnter(object sender, DragEventArgs e)
    {
      e.Effect = DragDropEffects.Copy;

    }

    private void cmdExport_Click(object sender, EventArgs e)
    {
      pf_export();
    }
    private void pf_export()
    {
      SaveFileDialog sav = new SaveFileDialog();
      // キャラ定義をjsonにexportする
      sav.Filter = "json file (*.json)|*.json|All files (*.*)|*.*";
      sav.InitialDirectory = @"C:\Users\fuuna\Videos";
      sav.FileName = "CharaDef.json";
      sav.ShowDialog();
      string src = @"C:\Users\fuuna\source\saltstone\resource\" + "charadef.json";
      string dest = @"C:\Users\fuuna\Videos\" + "CharaDef.json";
      System.IO.File.Copy(src, dest);
      string buff = "ファイル[" + "CharaDef.json" + "]を保存しました";
      System.Media.SystemSounds.Asterisk.Play();

      lblMessage.Text = buff;

    }

    private void cmdCharapictureSetting_Click(object sender, EventArgs e)
    {
      // 立ち絵で選択されているキャラ素材を設定する
      // PSD形式の場合はpsd用の画面を開く
      // -> お気に入りの登録
      if (lstCharapicture.SelectedRows == null)
      {
        lblMessage.Text = "立ち絵が選択されていないため、お気に入り登録できません";
        return;
      }
      if (lstCharapicture.SelectedRows.Count == 0)
      {
        lblMessage.Text = "立ち絵が選択されていないため、お気に入り登録できません";
        return;
      }

      int row = lstCharapicture.SelectedRows[0].Index;
      // 内部で立ち絵データを保存する


    }

    private void cmdCharaSetting_Click(object sender, EventArgs e)
    {
      pf_float_showCharaLayer();
    }
    private void pf_float_showCharaLayer()
    {
      // splitContainer_top.Panel2.Width = 0;
      splitContainer_top.SplitterDistance = gui_toppanel_splitdistance;
      // TODO キャラレイヤー設定をflotingにし、別winにする
      frmFloat_CharaSetting f = new frmFloat_CharaSetting();
      // dockできないか確認
      /*
       * 
       * floatingさせる場合
      f.mainform = this;

      f.Owner = this;
      f.Show();
      */
      f.TopLevel = false;
      f.FormBorderStyle = FormBorderStyle.None;
      // f.Owner = this;
      splitContainer_top.Panel2.Controls.Clear();
      splitContainer_top.Panel2.Controls.Add(f);
      f.Visible = true;
      f.Dock = DockStyle.Fill;


      // f.ShowDialog();
      // 閉じるのはどうすればいいの？ delegate or event?

    }

    private void cmdTest_Click(object sender, EventArgs e)
    {
      // drag drop test to aviu
      // aviutilcoop.test();
      string fname = @"C:\Users\fuuna\Videos\resource\chara\れいむ\口\" + "00.png";
      string hash = Utils.Files.getHash(fname);
      BitmapST bmp = new BitmapST(fname);
      Bitmap outb = null;
      bool ret = bmp.getClipping(out outb);
      if (outb == null)
      {
        return;
      }
      string destf = @"C:\Users\fuuna\Videos\a.bmp";
      Utils.Files.delete(destf);
      outb.Save(destf);
      outb.Dispose();
      outb = null;
      /*
      pctCharaPicture.Image = Bitmap.FromFile(fname);
      Bitmap a;
      CharaPictures.test(out a);
      pctCharaPicture.Image = a;
      string destf = @"C:\Users\fuuna\Videos\a.bmp";
      Utils.Files.delete(destf);
      a.Save(destf);
      */

    }

    private void charapicture_Click(object sender, EventArgs e)
    {
      frmCharaPicture f = frmCharaPicture.getInstance();
      f.init();
      f.Show();
    }




    private void pf_definedchara_openurl(int row, int col)
    {
      if (row < 0)
      {
        return;
      }
      if (col < 0)
      {
        return;
      }
      if (col != 5)
      {
        return;
      }

      // 4 = linked column
      pf_charapicture_openurl(row);

    }

    private void cmdSearchCharaPicture_Click(object sender, EventArgs e)
    {
      pf_charapicture_openurl();
    }

    // 立ち絵ページを表示
    private void pf_charapicture_openurl(int row = 0)
    {

      DataTable dt = (DataTable)lstDefinedCharaSet.DataSource;
      DataRow r = dt.Rows[row];
      string url = Utils.tostr(r["picturecureator_link"]);
      string rule = Utils.tostr(r["picturecreator_rule"]);
      Logs.write("立ち絵ダウンロードページを開いています:" + url);
      if (url.Length == 0)
      {
        string buff = CharaPortraits.URL_charapicture;
        Utils.openURL(buff);
        return;
      }

      if (rule != null && rule.Length != 0)
      {
        Utils.openURL(rule);
        Utils.sleep(100);
      }
      Utils.openURL(url);
      Logs.write("利用規約をよくお読みください");
      this.tabControl.SelectedTab = this.tabpCharPicture;
      dwWatch.Checked = true;
    }

    private void frmCharadefine_FormClosing(object sender, FormClosingEventArgs e)
    {
      formCloseingflag = true;
      // 何をしてもutilsのtimerが残り、最後にfinalizeが実行され、argument null exceptionが発生する
      // null判定が間違えていただけだった
      systeminfowatchtimer.Close();
    }

    private void frmCharadefine_Shown(object sender, EventArgs e)
    {
      //if (Globals.charapictures.Count() == 0)
      //{      //  Utils.sleep(500);
      //  Utils.Sound.playSystemSound();
      //  string buff = "立ち絵が登録されていません。ダウンロードしD&Dしてください";
      //  lblMessage.Text = buff;
      //  tabControl.SelectedTab = tabpCharPicture;
      //  Utils.openURL(CharaPictures.URL_charapicture);
      //  // tabpCharPicture.Focus();
      //}

    }

    private void testToolStripMenuItem_Click(object sender, EventArgs e)
    {
      /*
      string src = @"C:\Users\fuuna\Downloads\れいむ";
      string dst = @"C:\Users\fuuna\Videos\resource\chara\れいむ";
      Utils.Files.rsync(src, dst);
      */

      // zipファイルを展開 -> 解析
      CharaPortraits c = CharaPortraits.GetInstance();
      string fullfilename = @"C:\Users\fuuna\Downloads\れいむ.zip";
      string portraitid = "";
      c.addChara_portrait(fullfilename, out portraitid);
    }
  }
}
