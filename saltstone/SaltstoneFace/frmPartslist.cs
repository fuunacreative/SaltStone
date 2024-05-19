using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using saltstone;
using System.Data;






// TODO 起動までに時間がかかりすぎる


// d&dでaviutlへ貼り付け



// 最初はアニメなし 立ち絵のみ使う
// 台本-> wav/png出力 -> aviutlへ貼り付け
// ここまでをとりあえず作る
// ごちゃまぜdropで配置 -> 最終的には自作する？
// 台本がメインになるなー。立ち絵はsaltstoneface（このｐｇ）ができればなんとかなりそう




namespace SaltstoneFace
{
  public partial class frmPartslist : Form
  {
    private const int CONST_Anime_interval = 500;

    private saltstone.Charas charas;
    private Dictionary<int, string> comboboxcharalist;

    // アニメ用のタイマー
    private Utils.Timer pAnimeTimer;
    // アニメ処理に使用するタイマー
    private SMutex animemutex;

    // 画像一覧（フローレイアウト）で選択されているpictを保持する
    private Dictionary<FlowLayoutPanel, List<saltstone_customcontrol.LabeledPictureBox>> selectedpictures;
    // 他画像などは複数のpictureboxを持つ　なので、最初から複数選択できるようにデータを保持する
    // また、体などはひとつしか選択を許さない　table divisionでデータを管理し、一つしか許さないようにする
    // charasに持たせる？ そうするとマージ処理はclass charasに出せる
    // しかし、gui関係のものはformに持たせる

    // 画面上部の選択されたパーツ画像　一覧　と　file名を表示するラベル
    Dictionary<string, saltstone_customcontrol.LabeledTrackbar> uppartslist;
    // 主要画像毎時にラベルを作るのではなく、まとめて右上にfile名を表示し、別名もつけられるようにするため、削除
    // Dictionary<string, Label> uppartslistlabel;

    // pictureboxに対応するパーツを保存 -> ここからファイル名を取得 merge処理を行う
    Dictionary<saltstone_customcontrol.LabeledPictureBox, CharaParts> partslists;

    // 現在選択されている描画順序 現在のところ顔からのみ指定できる
    CharaDraworder partsdraworder;

    // 現在選択されているキャラ
    saltstone.Chara currentchar;

    // 現在選択中の各パーツの CharaParts
    Dictionary<string, CharaParts> currentparts;

    // 画面右上の選択パーツリストに表示するパーツのpartsid一覧
    List<string> listselectedparts;

    // アニメ用　コントロール、パーツ保存
    Dictionary<saltstone_customcontrol.LabeledPictureBox, CharaParts> animepartsscontrols_tab;
    // 主要パーツ表示部のコントロールはどうするか？
    Dictionary<PictureBox, CharaParts> animepartscontrols_upparts;
    Dictionary<int, List<saltstone_customcontrol.LabeledPictureBox>> tabpicureconttols;
    // modelpicture + 右上主要パーツ　＋　選択されているタブで表示されるアニパーツ
    // 選択されているタブというのがネック
    // tab -> pictureboxを保管しておいて
    // タブ選択がされたときにanimepartscontrolsを追加
    // 選択解除されたものをクリア
    // どうやってanimepartscontrolsに保管されているものがタブ由来のものかを
    // 判断するか？
    // 主要パーツ用とタブ用で２つのanimepartscontrolsをもつのがよいかな
    // partslistsでtab用のpictureboxとcharapartsは保管されているので、
    // これからcharapartsへアクセスできる

    // anime parts tabを管理するために、flow layout panelに配置したpictureboxを管理しておかないといけない
    // tab control indexでflow layout panelにアクセスできないか?

    // flow panelの管理 partsid B-> flowpanel
    private Dictionary<string, FlowLayoutPanel> pctlflowpanels;
    // flow panelの管理 tab selected index -> flowpanel
    private Dictionary<int, FlowLayoutPanel> pctlflopanelsbyid;

    // task用のmutex picutureboxのサイズ変更用事
    private static SMutex pmux_func_pctsizeimage_bk;

    // パーツ一覧表示のデータグリッドview
    private DataTable datatable_parts;


    public new void Dispose()
    {
      if (selectedpictures != null)
      {
        selectedpictures.Clear();
        selectedpictures = null;
      }
      if (comboboxcharalist != null)
      {
        comboboxcharalist.Clear();
        comboboxcharalist = null;
      }
      if (uppartslist != null)
      {
        uppartslist.Clear();
        uppartslist = null;

      }
      if (partslists != null)
      {
        partslists.Clear();
        partslists = null;
      }
      if (currentparts != null)
      {
        currentparts.Clear();
        currentparts = null;
      }
      if (listselectedparts != null)
      {
        listselectedparts.Clear();
        listselectedparts = null;
      }
      if (pAnimeTimer != null)
      {
        pAnimeTimer.Dispose();
        pAnimeTimer = null;
      }
      if (animemutex != null)
      {
        animemutex.Dispose();
      }
      if (animepartsscontrols_tab != null)
      {
        animepartsscontrols_tab.Clear();
        animepartsscontrols_tab = null;
      }
      if (animepartscontrols_upparts != null)
      {
        animepartscontrols_upparts.Clear();
        animepartscontrols_upparts = null;
      }
      if (tabpicureconttols != null)
      {
        foreach (List<saltstone_customcontrol.LabeledPictureBox> plst in tabpicureconttols.Values)
        {
          plst.Clear();
        }
        tabpicureconttols.Clear();
        tabpicureconttols = null;
      }
      if (pctlflowpanels != null)
      {
        pctlflowpanels.Clear();
        pctlflowpanels = null;
      }
      if (pctlflopanelsbyid != null)
      {
        pctlflopanelsbyid.Clear();
        pctlflopanelsbyid = null;
      }
      if (pmux_func_pctsizeimage_bk != null)
      {
        pmux_func_pctsizeimage_bk.Dispose();
      }

      if (charas != null)
      {
        charas.Dispose();
      }
      STasks.Dispose();
      this.Dispose();
    }
    

    public frmPartslist()
    {
      InitializeComponent();
      init();
    }

    public void init()
    {


      Logs.init(lblStatus, stpprogressbar);


      charas = CharaGlobals.charas;

      tabpicureconttols = new Dictionary<int, List<saltstone_customcontrol.LabeledPictureBox>>();
      animepartsscontrols_tab = new Dictionary<saltstone_customcontrol.LabeledPictureBox, CharaParts>();
      animepartscontrols_upparts = new Dictionary<PictureBox, CharaParts>();



      // キャラ combo boxの初期化
      // Chara libを参照
      lstSelectedParts.Rows.Clear();
      lstSelectedParts.Columns.Clear();
      datatable_parts = new DataTable();
      datatable_parts.Columns.Add("partsfile");
      datatable_parts.Columns.Add("memo");
      datatable_parts.Columns.Add("partsid");
      lstSelectedParts.DataSource = datatable_parts;
      lstSelectedParts.Columns["partsid"].Visible = false;
      lstSelectedParts.Columns["partsfile"].HeaderText = "パーツ名";
      lstSelectedParts.Columns["memo"].HeaderText = "別名";

      ctlBody.Text   = "肌カラー";
      ctlBody.Minvalue = -100;
      ctlBody.Maxvalue = 100;
      ctlBody.Value = 0;
      ctlEye.Text = "目カラー";
      ctlEye.Minvalue = -1;
      ctlEye.Maxvalue = 300;
      ctlEye.Value = -1;
      ctlhair.Text = "髪カラー";
      ctlhair.Minvalue = -1;
      ctlhair.Maxvalue = 300;
      ctlhair.Value = -1;

      ctlFukuTop.Text = "服上カラー";
      ctlFukuTop.Minvalue = -1;
      ctlFukuTop.Maxvalue = 300;
      ctlFukuTop.Value = -1;
      ctlFukuBottom.Text = "服下カラー";
      ctlFukuBottom.Minvalue = -1;
      ctlFukuBottom.Maxvalue = 300;
      ctlFukuBottom.Value = -1;

      // charas = new Charas();
      string buff;
      int i;
      comboboxcharalist = new Dictionary<int, string>();


      lstCharaID.Items.Clear();
      foreach (KeyValuePair<string, Chara> kv in charas.chara)
      {
        buff = kv.Value.dispname;
        i = lstCharaID.Items.Add(buff);
        comboboxcharalist[i] = kv.Value.charakey;
        // txtCharaID.Items.
      }
      lstCharaID.SelectedIndex = 0; //　for test

      buff = comboboxcharalist[lstCharaID.SelectedIndex];
      currentchar = charas[buff];

      // flowパネル毎時の選択されたpictureboxを管理ｘ
      selectedpictures = new Dictionary<FlowLayoutPanel, List<saltstone_customcontrol.LabeledPictureBox>>();
      pctlflopanelsbyid = new Dictionary<int, FlowLayoutPanel>();
      List<FlowLayoutPanel> fps = new List<FlowLayoutPanel>();
      fps.Add(flp_01Body);
      fps.Add(flp_02Face);
      fps.Add(flp_03Eye);
      fps.Add(flp_04Lip);
      fps.Add(flp_05Mayu);
      fps.Add(flp_06All);
      fps.Add(flp_07Hair);
      List<saltstone_customcontrol.LabeledPictureBox> p;
      foreach (FlowLayoutPanel fp in fps)
      {
        p = new List<saltstone_customcontrol.LabeledPictureBox>();
        selectedpictures[fp] = p;
      }
      // flow layout panelを登録する　最初に登録すればよいのでここに移動 ctlflowpanelsを追加する
      pctlflowpanels = new Dictionary<string, FlowLayoutPanel>();
      foreach (FlowLayoutPanel fp in selectedpictures.Keys)
      {
        buff = (string)fp.Tag;
        pctlflowpanels[buff] = fp;
      }
      foreach (FlowLayoutPanel fp in fps)
      {
        TabPage tp = (TabPage)fp.Parent;
        i = ctlTabpanel.TabPages.IndexOf(tp);
        pctlflopanelsbyid[i] = fp;

      }

      uppartslist = new Dictionary<string, saltstone_customcontrol.LabeledTrackbar>();
      List<saltstone_customcontrol.LabeledTrackbar> upcts = new List<saltstone_customcontrol.LabeledTrackbar>();
      string partsid;
      upcts.Add(ctlBody);
      upcts.Add(ctlEye);
      upcts.Add(ctlhair);
      upcts.Add(ctlFukuTop);
      upcts.Add(ctlFukuBottom);
      foreach (saltstone_customcontrol.LabeledTrackbar upct in upcts)
      {
        partsid = (string)upct.Tag;
        uppartslist[partsid] = upct;
      }
      upcts.Clear();
      upcts = null;
      //List<PictureBox> upcts = new List<PictureBox>();
      //// upcts.Add(pctUppartsFace);
      //upcts.Add(pctUppartsEye);
      //string partsid;
      //foreach (saltstone.LabeledPictureBox upct in upcts)
      //{
      //  partsid = (string)upct.Tag;
      //  uppartslist[partsid] = upct;
      //}
      //upcts.Clear();
      //upcts = null;


      partslists = new Dictionary<saltstone_customcontrol.LabeledPictureBox, CharaParts>();
      currentparts = new Dictionary<string, CharaParts>(); // 現在選択中のパーツ by partsid

      listselectedparts = new List<string>();
      listselectedparts.Add("B");
      listselectedparts.Add("F");
      listselectedparts.Add("H");
      listselectedparts.Add("K");
      listselectedparts.Add("O");
      listselectedparts.Add("E");
      listselectedparts.Add("L");
      listselectedparts.Add("SET"); // set ,favはいいんだけど、各パーツはどう表示するの？
      listselectedparts.Add("FAV");

      foreach (TabPage tabp in ctlTabpanel.TabPages)
      {
        i = ctlTabpanel.TabPages.IndexOf(tabp);
        tabpicureconttols[i] = new List<saltstone_customcontrol.LabeledPictureBox>();
      }


      // charaid れいむ=rを元にパーツリストをlib charasより読み込む
      // -> 各datagridに展開

      // ここでsetpartslistを読み込むと、全部のflowlayoutpanelにpctを読み込んでしまうのでは？
      pFunc_setPartslist(enum_Flowlayoutpanel_loadmode.loadcurrentdisplay);

      // アニメ処理のtimer作成 + eve登録
      pAnimeTimer = new Utils.Timer();
      pAnimeTimer.evt_timer += pFunc_anime;
      pAnimeTimer.start(CONST_Anime_interval, Utils.Timer.enum_repeatTimer.repeat);
      animemutex = new SMutex("frmpartslist_anime");

      // 表示中のタブのflowlayoutpanelを先に表示、後ろに隠れているものは後で追加
      // パーツ数が多すぎるので、起動が遅くなると考えられる

      pmux_func_pctsizeimage_bk = new SMutex("func_pctsizeimage_bk");


      // SortedDictionary<string, CharaParts> parts = currentchar.getparts("B"); // "F"
      currentchar.getparts("B"); // "F"

      this.Show();

      //STasks.init();
      // Tasks.createTask((Action)(() => init_background()));
      //Utils.sleep(100); // begininvokeが失敗する場合があるので(formの描画前に処理が走る）wait処理を追加
      int wh = this.Handle.ToInt32(); // handleを参照するとwin handleが作成されて後続のinvokeが失敗しなくなる

      // STasks.createTask(init_background);
      // 裏でのパーツ読み込み（非表示タブ）なので、最後でいい
      STasks.addOrderedTask(100, pFunc_init_background);
      // STasks.addOrderedTask(100, setPartslist(enum_Flowlayoutpanel_loadmode.loadother));
      // STasks.addOrderedTask(200,(Action)(() => setPartslist(enum_Flowlayoutpanel_loadmode.loadother)));

      // STasks.addOrderedTask(210, setTabAnimeControls);

      // 裏でSTasksが動いているので、mutex lockしないといけない
      // というか、mutexが多すぎ！！


      STasks.run();
    }

    private void pFunc_init_background()
    {
      // これは、他のタブの画像読み込みなので、
      // charasのパーツ読み込みが完了していないと実行できない
      // bool fret = setPartslist();
      this.BeginInvoke((Action)(() => pFunc_setPartslist( enum_Flowlayoutpanel_loadmode.loadother)));
      this.BeginInvoke((Action)(() => setTabAnimeControls()));
      //this.BeginInvoke(setTabAnimeControls);



    }

    private enum enum_Flowlayoutpanel_loadmode
    {
      loadcurrentdisplay,
      loadother
    }

    // 現在選択中のタブのflowlayoutのものを優先してpicturebox load
    private bool pFunc_setPartslist(enum_Flowlayoutpanel_loadmode loadmode = enum_Flowlayoutpanel_loadmode.loadcurrentdisplay)
    {
      bool fret = false;

      int i;
      Dictionary<string, FlowLayoutPanel> processpanesl;

      //string charakey = comboboxcharalist[lstCharaID.SelectedIndex];
      //saltstone.Chara c = charas[charakey];
      Chara c = currentchar;
      pctModelPicture.Image = c.modelpicture;
      processpanesl = pctlflowpanels;
      if (loadmode == enum_Flowlayoutpanel_loadmode.loadcurrentdisplay)
      {
        processpanesl = new Dictionary<string, FlowLayoutPanel>();
        i = ctlTabpanel.SelectedIndex;
        FlowLayoutPanel fp = pctlflopanelsbyid[i];
        string partsid = (string)fp.Tag;
        processpanesl[partsid] = fp;
      }



      // setTabAnimeControls

      // アニメ処理のため、flowlayoutに追加したpicturecontrolをtab indexで選択できるようにする
      List<saltstone_customcontrol.LabeledPictureBox> tabpctlist;
      // ctlflowpanelsがnullになる initで登録しているのに、、、
      foreach (KeyValuePair<string, FlowLayoutPanel> kv in processpanesl)
      {
        if (kv.Value.Controls.Count > 0)
        {
          continue;
          // 再読み込みを考慮していない
        }
        SortedDictionary<string, CharaParts> parts = c.getparts(kv.Key); // "F"
        if (parts == null)
        {
          // 同期処理の関係で nullになる場合がある
          // この時点で"B"のパーツを読み込んでいないのはおかしい
          // タイミングの問題 step実行ではうまく動く
          // 根本的な見直しが必要そーだねー
          // 表示するキャラクターの"B"を先読みすることで対応
          // TODO ここは通ってはいけなロジック ここを通る前に読み込み処理が実行されていなければならない
          // さて、、、どうするか？
          return fret;
        }
        saltstone_customcontrol.LabeledPictureBox ctlp;
        int pw = 100;  // 100に対してaspectrateを設定
        int ph = (int)(pw * c.aspectrate);
        // flp_01bodyにimageをcontrol addしながら追加する

        // 
        i = ctlTabpanel.TabPages.IndexOf((TabPage)kv.Value.Parent);
        tabpctlist = tabpicureconttols[i];


        //  表示するのは、partsnbum(00),animeflag , memo
        // この３つを取得する関数が必要
        foreach (CharaParts cp in parts.Values)
        {
          // string f = cp.
          // ここを saltstone_customcontrol.LabeledPictureBoxにしたい

          ctlp = new saltstone_customcontrol.LabeledPictureBox();
          ctlp.SizeMode = PictureBoxSizeMode.Zoom;
          // ctlp.BorderStyle = BorderStyle.FixedSingle;
          ctlp.Height = ph;
          ctlp.Width = pw;
          ctlp.Click += evt_pctctl_Click; 
          ctlp.Margin = new Padding(1, 1, 1, 1);
          ctlp.Image = Image.FromFile(cp.filename);
          ctlp.Text = cp.shortfilename;
          ctlp.Animeflag = cp.animeflag;
          // imageをキャッシュしてメモリ保存する
          // ctlp.Image = SImages.getImage(cp.filename);
          //ctlp.BackgroundImage = SImages.getImage(cp.filename);
          //ctlp.BackgroundImageLayout = ImageLayout.Center;
          //ctlp.BackColor = Color.Blue;

          // flp_01Body.Controls.Add(ctlp);
          kv.Value.Controls.Add(ctlp);
          // パーツリスト picturebox -> charaparsをコレクションに保存
          partslists[ctlp] = cp;
          // tab indexに対するctlを登録する
          tabpctlist.Add(ctlp);
          // setTabAnimeControlsで行うため不要
          //if (cp.animeflag == true)
          //{
          //  // animepctist.Add(ctlp);
          //  animepartsscontrols_tab[ctlp] = cp;
          //}

        }

      }

      
      // アニメ処理の追加
      // charaparts.animeflagがtrueのものに対して controlをcollection化する
      // anime eventでは、アニメ表示ctlのみアニメ処理を行う
      // list<control> animectls;
      // ただし、表示中のtabのみ
      // それとmodelpictureと上部の主要パーツ
      // anime controlsの解除をどうするか？


      fret = true;
      // control.handleがidとして使える 8byte
      return fret;
    }

    private void pctModelPicture_Click(object sender, EventArgs e)
    {
      // double clickで拡大画像用winを表示
    }

    // tab flow layoutに追加されたpictureboxのclick event handler
    private void evt_pctctl_Click(object sender, EventArgs e)
    {
      pFunc_flowpictureclick(sender);
    }

    private void pFunc_flowpictureclick(object sender)
    {
      // どうやって親の顔、体を判断するか？
      // ctl tag? or private member 変数

      // 部位を判断し、画像を合成する -> modelpictureに表示
      // 画像をcache
      // hwnd -> 部位 Bとか
      // 画像ファイルの確認 hwnd -> file
      // 画像合成ファイル incmd.txtを作成
      // c++ image合成処理 (out file)
      // orderの処理も考慮が必要
      // orderをどれにひもづけするか？　顔？　dbに保存？
      // １、体→顔(通常)→髪(不透明)→眉→目→口→髪（半透明） Aパターン
      // ２，体→髪(不透明)→眉→目→口→髪（半透明）→　他 →顔（乗算） -> もう一度髪 後ろは？
      // パターンマッチングと合成順序を設定する　テキストファイル？
      // なくても描画できるように

      lblStatus.Text = "";

      // labeledpictureboxにした事でevetまわりがうまく動作しなくなった
      // 単純にcustom control += clickではイベントが発生しなくなった
      saltstone_customcontrol.LabeledPictureBox pc = (saltstone_customcontrol.LabeledPictureBox)sender;
      // click処理
      // 既に選択されているものをクリア
      pc.BorderStyle = BorderStyle.Fixed3D;
      // 選択されているかどうかをどう判断するか？
      // パーツによりひとつしか選択を許さない場合がある
      FlowLayoutPanel fp = (FlowLayoutPanel)pc.Parent;
      // string w = pc.Parent.Name;
      List<saltstone_customcontrol.LabeledPictureBox> pcts = selectedpictures[fp];
      bool singleselect = true;
      // charasより取得 table division
      string partsid = (string)fp.Tag;

      // パーツ（部位）毎にひとつしか選択できないもの、複数選択できるものと２つある
      // TODO 複数選択できるかどうかはtable divisionで定義する パーツによる
      if (singleselect == true)
      {
        foreach (saltstone_customcontrol.LabeledPictureBox ctlp in pcts)
        {
          ctlp.BorderStyle = BorderStyle.FixedSingle;
        }
        pc.BorderStyle = BorderStyle.Fixed3D;
        selectedpictures[fp].Clear();
      }
      selectedpictures[fp].Add(pc);

      string buff;
      CharaParts cparts;
      // 選択したパーツをcpartsに格納　
      cparts = partslists[pc];
      //      currentchar.setselectparts(cparts);
      bool ret = currentchar.setSelectedParts(cparts);

      // filenameそのままだとフルパスになる。相対パスにしたい
      //pctSelectedParts.Text = cparts.shortfilename;
      //pctSelectedParts.Image = SImages.getImage(cparts.filename);
      ctlSelectedParts.Text = cparts.shortfilename;
      ctlSelectedParts.Image = SImages.getImage(cparts.filename);
      ctlSelectedParts.Animeflag = cparts.animeflag;

      currentparts[partsid] = cparts;
      if (cparts.compositorder != null)
      {
        partsdraworder = cparts.compositorder;
      }
      // 上部のパーツ画像　一覧の表示
      if (uppartslist.ContainsKey(partsid))
      {
        // PictureBox upct = uppartslist[partsid];
        saltstone_customcontrol.LabeledTrackbar upct = uppartslist[partsid];
        upct.Image = SImages.getImage(cparts.filename);
        upct.setPictureName = cparts.shortfilename;
        upct.Animeflag = cparts.animeflag;

        //if (animepartscontrols_upparts.ContainsKey(upct))
        //{
        //  animepartscontrols_upparts.Remove(upct);
        //}

        //if (cparts.animeflag == true)
        //{
        //  // animepartscontrols_uppartsから現在のパーツを廃棄
        //  animepartscontrols_upparts[upct] = cparts;

        //}
        //string path = @"C:\Users\fuuna\Videos\charas\れいむ\顔\00a.png";
        // upct.Image = Image.FromFile(path);
        // filenameも表示したい どこかに保存しておく必要がある
        // pictureboxに対してfilenameの保存が必要
        // dictionary<picturebox,filename>

        // 右上のdata grid viewにまとめて表示するため削除
        //if (uppartslistlabel.ContainsKey(partsid))
        //{
        //  // 上部の特徴顔パーツのラベル名編集
        //  Label ctllb = uppartslistlabel[partsid];
        //  buff = Utils.Files.getfilename(cparts.filename);
        //  buff = Charas.partsdirbykey[partsid] + "\\" + buff;
        //  ctllb.Text = buff;
        //}
      }

      // draworderを使えないか？
      //CharaDraworder co = currentchar.pCharaDrawOrder;

      //lstSelectedParts.Columns.Clear();
      //DataTable dt = new DataTable();
      //dt.Columns.Add("partsid");
      //dt.Columns.Add("filename");
      DataRow dr;
      // DataRow[] drs;

      // 上部のdata grid viweにパーツを表示する
      buff = "";
      foreach (string pid in Charas.partsdirbykey.Keys)
      {
        // 選択されているもの
        if (currentparts.ContainsKey(pid))
        {
          // 行に存在する 
          // update or del->ins　どっち？
          if (datatable_parts.Select("partsid = '" + pid + "'").Length == 0)
          {
            dr = datatable_parts.NewRow();
            buff = cparts.filename;
            buff = buff.Replace(currentchar.charadir, "").Substring(1);

            dr[0] = buff;
            dr[1] = cparts.memo;
            dr[2] = cparts.partsid;
            datatable_parts.Rows.Add(dr);

          } else
          {
            dr = datatable_parts.Select("partsid = '" + pid + "'")[0];

            dr = datatable_parts.NewRow();
            buff = cparts.filename;
            buff = buff.Replace(currentchar.charadir, "").Substring(1);

            dr[0] = buff;
            dr[1] = cparts.memo;
          }
          // datatableには表示されているものが保管されている
          // なければ追加

        }
      }

      #region memo
      //if (listselectedparts.Contains(partsid))
      //{
      //  lstSelectedParts.Columns.Clear();
      //  // 右上のdata grid viewに選択したパーツを表示
      //  buff = Utils.Files.getfilename(cparts.filename);
      //  buff = Charas.partsdirbykey[partsid] + "\\" + buff;
      //  lstSelectedParts_bk.Items.Add(buff);
      //  //  datatableにselected partsを保管し、datagridviewをredrawする
      //  DataTable dt = new DataTable();
      //  dt.Columns.Add("partsid");
      //  dt.Columns.Add("filename");
      //  DataRow dr = dt.NewRow();

      //  dr["partsid"] = partsid;
      //  dr["filename"] = buff;
      //  dt.Rows.Add(dr);
      //  lstSelectedParts.DataSource = dt;
      //  lstSelectedParts.Columns["filename"].HeaderText = "ファイル名";
      //  //lstSelectedParts.Refresh();
      //}


      // default orderを使用するので問題ない
      //if (partsdraworder == null)
      //{
      //  lblStatus.Text = "顔パーツなどが選択されていないため、表示順が決定できません";
      //  // default orderを定義するか、、、、
      //  return;
      //}


      // chara classへ移動
      //SortedDictionary<string, CharaParts> partslist;
      //List<string> mergefiles = new List<string>();
      //foreach (CharaDraworderItem porder in partsdraworder.orderitems.Values)
      //{
      //  if (currentparts.ContainsKey(porder.partsid) == false)
      //  {
      //    continue;
      //    // 未選択のパーツがあっても合成処理は続行する
      //  }
      //  // file patternをfilenameに変換する
      //  // partsid -> filenameにする
      //  partslist = currentchar.getparts(partsid);
      //  // 現在選択中のパーツの種類 
      //  // fname = partslist[]
      //  cparts = currentparts[porder.partsid];
      //  fname = cparts.filename;
      //  mergefiles.Add(fname);
      //}
      // 選択された画像は selectedpictures
      // 表示順序でincmd.txtを作成しc++に渡す
      // 現在選択されている 顔　の表示順を元にリストを構成する

      #endregion
      string fname;
      // string cmdarg;
      // ret = currentchar.makemergecmd(out fname,out cmdarg);

      Utils.mouseCursor.wait();
      ret = currentchar.getmergefile(out fname);
      Utils.mouseCursor.clear();
      if (ret == false)
      {
        return;
      }
      // pctModelPicture.Image = Image.FromFile(fname);
      // imageのcache
      pctModelPicture.Image = SImages.getImage(fname);


      // cacheをどう組み込むか？
      // fnameが存在していたら、c++ image exeを走らせる必要はない
      // charaにもたせるか？ or 別classにして管理するか？
      // 最終的にほしいのはmergeされたfname -> charaにc++ image exeもたせてしまう



      //// string exef = Appinfo.exeimageproces + " " + fname;
      //string exef = Appinfo.exeimageprocess;
      //// ret = Utils.exeshellcmd(exef, cmdf);
      //ret = Utils.runexec(exef, cmdarg, Utils.enum_runmode.start_async);
      //if (ret == false)
      //{
      //  // 作成失敗
      //  return;
      //}
      //pctModelPicture.Image = Image.FromFile(fname);


      // appinfoにc++ image exeのpathをセット
      // utilsを使って実行 終了まで停止

      // 処理されたpngファイルをrenameしてキャッシュとして保存
      // pngファイルをmodelpictureにdraw

      // 選択した画像をc++でマージする
      // cacheがあるか確認

      // selectedpictures
      return;

    }

    private void pFunc_anime(object sender, System.Timers.ElapsedEventArgs e)
    {
      // anime処理
      // 選択されているパーツがアニメを含んでいる場合、
      // pictureboxの画像を入れ替える
      // 表示されているタブ・上部の主要パーツ・モデル画像のみアニメ処理を行う
      // c.selectedparts;

      // アニメ処理に必要なcontrolを登録する


      bool ret;
      ret = animemutex.lockmutex();
      if (ret == false)
      {
        return;
      }
      // mutexでlockする
      try
      {
        this.BeginInvoke((Action)(() => pfunc_evt_drawanimefiles()));


      }
      catch (Exception ex)
      {
        string error = ex.ToString();

      } finally
      {
        animemutex.releasemutex();
      }


    }


    // flowlayoutpanelのアニメパーツの描画
    private void pfunc_evt_drawanimefiles()
    {
      Chara c = currentchar;
      CharaPartsAnime cpa;
      // CharaParts cp;
      // アニメをどう処理するか？
      string animef;
      int i;
      string basedir = c.charadir;

      foreach (KeyValuePair<saltstone_customcontrol.LabeledPictureBox, CharaParts> kv in animepartsscontrols_tab)
      {
        i = kv.Value.currentAnimeindes;
        //if (i == 6)
        //{
        //  int xxx = 10;
        //}
        if (i >= kv.Value.animecount)
        {
          kv.Value.currentAnimeindes = 0;
          i = 0;
        }
        cpa = kv.Value.animefiles[i];
        string pdir = Charas.partsdirbykey[kv.Value.partsid];
        animef = basedir + "\\" + pdir + "\\" + cpa.animefname;
        //kv.Key.Image = Image.FromFile(animef);
        kv.Key.Image = SImages.getImage(animef);
        kv.Value.currentAnimeindes += 1;

      }

      // 別スレッドになるので、invokeが必要

      //// 表示されているタブ flow contolrのpict anime
      //int i = ctlTabpanel.SelectedIndex;
      //// List<PictureBox> pcts = animepartsscontrols_tab[i];
      //List<PictureBox> pcts = tabpicureconttols[i];
      //foreach (PictureBox ctlpct in pcts)
      //{
      //  CharaParts cp = partslists[ctlpct];
      //  // アニメをどう処理するか？
      //  // cp.getanimefiles // 現在のアニメ itemsの位置を保管しておかないといけない、、、
      //  // イテレーターでもつか、or  indexでもつか？
      //  i = cp.currentAnimeindes;
      //  if (i >= cp.animefiles.Count)
      //  {
      //    continue;
      //  }
      //  cpa = cp.animefiles[i];


      //}


      //// まず、上部の主要パーツ
      //string partsid;
      //foreach (KeyValuePair<string, PictureBox> kv in uppartslist)
      //{
      //  partsid = kv.Key;
      //  if (c.selectedparts == null)
      //  {
      //    continue;
      //  }
      //  if (c.selectedparts.ContainsKey(partsid) == false)
      //  {
      //    continue;
      //  }
      //  List<CharaParts> cp = c.selectedparts[partsid];
      //}

    }



    private void ctlTabpanel_SelectedIndexChanged(object sender, EventArgs e)
    {
      setTabAnimeControls();
    }
    private bool setTabAnimeControls()
    {
      // tab選択が変更された -> animeparts_tabをクリア->再登録
      bool fret = false;
      int i = ctlTabpanel.SelectedIndex;

      // animepartsscontrols_tabを再設定して timer eventでアニメ処理を行う
      animepartsscontrols_tab.Clear();
      // tabに配置されているflowlayoutpanelに配置されているpictureboxを取得したい
      foreach (saltstone_customcontrol.LabeledPictureBox pctl in tabpicureconttols[i])
      {
        if (partslists[pctl].animeflag == false)
        {
          continue;
        }
        animepartsscontrols_tab[pctl] = partslists[pctl];
      }

      fret = true;
      return fret;
    }


    private void func_pctsizeimage()
    {
      // flow laout panel -> picturebox sizeを変更する

      // 現在表示中のタブ -> bkで非表示のタブ
      int i = ctlTabpanel.SelectedIndex;
      int nw = vsbImageSize.Value * -1;
      
      int nh = (int)(currentchar.aspectrate * nw);
      // flowlayoutpalen suspendcontrol
      // サイズ変更を行うと、表示されなくなる
      // どーすればよか？　おそらく、widthを大きくした場合に右側のpctと干渉し表示しきれなくなっているのでは？
      pctlflopanelsbyid[i].SuspendLayout();
      foreach (saltstone_customcontrol.LabeledPictureBox pctl in tabpicureconttols[i])
      {
        if (nw != pctl.Width)
        {
          // pctl.Anchor = AnchorStyles.Left;
          pctl.Width = nw;
          pctl.Height = nh;
        }
      }
      pctlflopanelsbyid[i].ResumeLayout();
      //ctlTabpanel.TabPages[i].Refresh();

      // taskで非表示のタブのflowlayout panel -> picturebox ctlのサイズ変更
      // currentselectedtabctlindex = -1;
      // currentselectedtabctlindex = ctlTabpanel.SelectedIndex;
      STasks.createTask(func_pctsizeimage_bk);

    }

    // private int currentselectedtabctlindex;


    private void func_pctsizeimage_bk()
    {
      bool fret = pmux_func_pctsizeimage_bk.lockmutex();
      if (fret == false)
      {
        return;
      }
      // 別タスクより実行してるので、form ctrlにアクセスできず、invokeが必要
      // int i = ctlTabpanel.SelectedIndex;
      // int i = currentselectedtabctlindex;
      // ctlTabpanel.BeginInvoke((Action)(() => { i = ctlTabpanel.SelectedIndex; }) );
      int i = (int)ctlTabpanel.Invoke(new Func<int>(() => ctlTabpanel.SelectedIndex));
      if (i == -1)
      {
        return;
      }
      int nw;
      nw = (int)vsbImageSize.Invoke(new Func<int>(() => vsbImageSize.Value ));
      nw = nw * -1;
      // int nw = tcbImagesize.Value
      int nh = (int)(currentchar.aspectrate * nw);

      for (int j = 0; j < ctlTabpanel.TabPages.Count; j++)
      {
        if (j == i)
        {
          // 現在、表示中のタブは処理済なのでスキップ
          continue;
        }
        foreach (saltstone_customcontrol.LabeledPictureBox pctl in tabpicureconttols[j])
        {
          if (nw != pctl.Width)
          {
            pctl.Invoke(new Action(() => pctl.Width = nw));
            pctl.Invoke(new Action(() => pctl.Height = nh));
            // pctl.Width = nw;
            // pctl.Height = nh;
          }
        }

      }
      pmux_func_pctsizeimage_bk.releasemutex();

    }

    private void vsbImageSize_Scroll(object sender, ScrollEventArgs e)
    {
      // flowlayout panel -> picturecontrolのサイズを変更する
      func_pctsizeimage();
    }
  }
}
