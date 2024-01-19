using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

// TODO 課題 立ち絵(portrait)が変更された場合、再解析が必要 どうやって実現するか？

namespace saltstone
{
  public partial class frmSenario_bak : Form,Inf_FormSetFace
  {
    // public Charas charas;
    Dictionary<string, SortedDictionary<int, PictureBox>> pngdata;
    private const int Icon_Defaultwidth = 100;
    private List<TreeNode> initdata_scene;

    // private DataTable seriflist;
    // lstsubtitleのrow enter eventをキャンセルするかどうか
    private bool handlerowadd = false;

    private string facekey = "";
    // #pragma warning disable 0649
    private PictureBox currentface = null;
    private string current_senariofile; // 現在開いているシナリオファイル txt or md

    private Aviutl aviuobj; // aviutl連携用 object

    private Scenes current_scene; // 現在選択中のシーンobj
    private Quote current_quote; // 現在選択中のgridviewのquote
    private string current_charaid; // 現在選択中のcharaid r or m








    public frmSenario_bak()
    {
      InitializeComponent();
      Globals.init();
      init();
      initdata_scene = null;
      initdata_scene = new List<TreeNode>();
      // treenodeを簡単にコピーできないか？
      TreeNode n;
      foreach (TreeNode p in lstScene.Nodes)
      {
        n = new TreeNode(p.Text);
        initdata_scene.Add(n);
        // initdata_scene[p.Index].Text = p.Text;
        // initdata_scene.Add(p.Text);
        if (p.Nodes.Count > 0)
        {
          foreach (TreeNode ch in p.Nodes)
          {
            n.Nodes.Add(ch.Text);
          }
        }
      }
      lstScene.Nodes.Clear();
      /*
      foreach(string o in lstScene.Items)
      {
          initdata_scene.Add(o);
      }
      lstScene.Items.Clear();
      */

      // seriflist = new DataTable();
      /*
      seriflist.Columns.Add("charaid"); // キャラID れいむ,まりさ
      seriflist.Columns.Add("faceid"); // 立ち絵  セット00 or B00E00など
      seriflist.Columns.Add("subtitle"); // 字幕
      seriflist.Columns.Add("pronun"); // 発音記号
      seriflist.Columns.Add("speed"); // 発声スピード
      seriflist.Columns.Add("tone"); // 発声トーン
      seriflist.Columns.Add("excmd"); // 追加の貼り付け指示
      */

      /*
      seriflist.Columns.Add("キャラ"); // キャラID れいむ,まりさ
      seriflist.Columns.Add("立ち絵"); // 立ち絵  セット00 or B00E00など
      seriflist.Columns.Add("字幕"); // 字幕
      seriflist.Columns.Add("発音"); // 発音記号
      seriflist.Columns.Add("スピード"); // 発声スピード
      seriflist.Columns.Add("トーン"); // 発声トーン
      seriflist.Columns.Add("素材"); // 追加の貼り付け指示
      */
      // initdata_scene = lstScene.Items

      // seriflist.TableNewRow += new DataTableNewRowEventHandler(dt_TableNewRow);

      lstSubtitle.ColumnHeadersDefaultCellStyle.Font = lblDatagridviewFont.Font;
      lblDatagridviewFont.Visible = false; // gridview column headerのfont

      FrmCharaSelect f = FrmCharaSelect.showwindows();
      f.mainform = this;

      txtSubtitle.Text = "";
      lblStatus.Text = Globals.envini[PGInifile.INI_Aupdir];
      lblMessage.Text = " ";

    }


    /*
    private void dt_TableNewRow(object sender, DataTableNewRowEventArgs e)
    {
        string a = "";
        seriflist.Rows.Add(e.Row);

        MessageBox.Show("Event Raised...");
    }
    */


    public void init()
    {
      // drawpng();
      // TODO charakeyはdbから取得できるはず
      CharaGlobals.charas["れいむ"].charakey = "r";
      CharaGlobals.charas["まりさ"].charakey = "m";
      // voiceを保存するフォルダの設定
      // projectdir + voice

      // でも用
      current_charaid = "r";
      // TODO dbよりcharaを読み込み、コンボボックスを設定
      // 一番先頭のキャラを選択中のキャラとして選択状態にする


    }

    /*
    public void drawpng()
    {
        flowimglist.Controls.Clear();
        currentface = null;

        // lstcharaで選択されたキャラのoutディレクトリのpngをdrawする
        string charaid = "れいむ";
        // Chara c = charas.chara[charaid];
        Chara c = Globals.charas[charaid];



        SortedDictionary<int, PictureBox> png = getpngdata(charaid);

        // string where = "";
        List<string> where = null;
        List<int> dispsetnum = c.getoutsetnum(charaid, where);
        foreach (int j in dispsetnum)
        {
            // png[j]
            flowimglist.Controls.Add(png[j]);

        }

    }
    */

    private SortedDictionary<int, PictureBox> getpngdata(string charaid)
    {
      //  string charaid = lstChara.SelectedItem.ToString();

      if (pngdata == null)
      {
        pngdata = new Dictionary<string, SortedDictionary<int, PictureBox>>();
      }
      if (pngdata.ContainsKey(charaid) == true)
      {
        return pngdata[charaid];
      }

      // ｄｂからの読み込み処理
      Chara c = CharaGlobals.charas.chara[charaid];
      SortedDictionary<int, string> outpng = c.getoutpng(charaid);
      SortedDictionary<int, PictureBox> pngs = new SortedDictionary<int, PictureBox>();
      pngdata[charaid] = pngs;
      foreach (KeyValuePair<int, string> s in outpng)
      {
        // pictureboxへの参照を保存する
        // 何度もpictureboxのインスタンスを作るのは処理が重い
        // キャラid（れいむ、まりさ）が変更になった場合どうするか？
        // 全部のキャラのpictureboxを保持する


        PictureBox p = new PictureBox();
        // 縦横比率がおかしい 400x320 => 150 * 120
        p.Width = Icon_Defaultwidth;
        p.Height = c.getaspectheight(p.Width);
        // p.Height = 120;
        // p.SizeMode = PictureBoxSizeMode.StretchImage;
        // widhとheighを管理する必要がある
        p.SizeMode = PictureBoxSizeMode.Zoom;
        p.ImageLocation = s.Value;
        p.BorderStyle = BorderStyle.FixedSingle;
        p.Margin = new Padding(0);
        p.MouseClick += new MouseEventHandler(charaface_Click);
        //                p.Click += evt_img_Click;
        //                p.DoubleClick += evt_img_DoubleClick;
        //p.Tag = 1; // setnumをいれる
        p.Tag = s.Key;
        pngs[s.Key] = p;

        // this.flowimglist.Controls.Add(p);
      }
      return pngs;
    }
    private void charaface_Click(object sender, EventArgs e)
    {
      PictureBox o = (PictureBox)sender;
      if (o.BorderStyle == BorderStyle.FixedSingle)
      {
        o.BorderStyle = BorderStyle.Fixed3D;
        o.BackColor = Color.OrangeRed;
        facekey = ((int)o.Tag).ToString();
        currentface = o;
      } else
      {
        o.BorderStyle = BorderStyle.FixedSingle;
        o.BackColor = SystemColors.Control;
        facekey = "";
        currentface = null;
      }
    }


    private void txtSubtitle_Leave(object sender, EventArgs e)
    {
      if (txtPronun.Text.Length > 0) return;
      // txtPronun.Text = Yomigana.getYomigana(txtSubtitle.Text);
      // 選択中のcharaより voice -> phoetic -> text cnv phoetic
    }

    private void menOpen_Click(object sender, EventArgs e)
    {
      pf_OpenFile();

    }
    private void pf_OpenFile()
    {

      this.dlgOpenFile.InitialDirectory = Globals.envini[PGInifile.INI_Aupdir];
      this.dlgOpenFile.FileName = "part16.txt";
      this.dlgOpenFile.Filter = @"シナリオ|*.txt;*.md";
      // this.dlgOpenFile.Filter = @"シナリオ|*.txt;*.md|台本(*.md)|*.md";
      this.dlgOpenFile.FilterIndex = 1;
      this.dlgOpenFile.ShowDialog();

      // 選択されたtxtよりシーン・セリフを展開
      // mdが指定されたら、それは台本ファイル 発音、立ち絵つき 
      // md=saltstone独自の保存形式
      // txt=手入力された簡単なテキストファイル
      string f = dlgOpenFile.FileName;
      if (Utils.Files.exist(f) == false)
      {
        lblMessage.Text = "ファイルが選択されませんでした";
        return;
      }
      current_senariofile = f;
      // シナリオファイルを開いたのでaviuobjが設定できる
      pf_aviuobj();

      // Global.voicelistの定義をどこからかひっぱってくる必要がある
      // charadbに定義を持たせる default speedなども
      string fext = Utils.Files.getextention(f);
      if (fext.ToLower() == ".txt")
      {
        Senariotxtparser.parse(f);
      } else if (fext.ToLower() == ".md")
      {
        SenarioMDFile mf = new SenarioMDFile(this.aviuobj);
        mf.Load(f);
        mf = null;
      } else
      {
        lblMessage.Text = "無効な拡張子のファイルが選択されました";
        return;
      }

      // lblstatusにprojectdirを表示
      string buff = "プロジェクトディレクトリ:" + this.aviuobj.projectdir;
      // lblStatus.Text = f; // statusに選択したシナリオファイルを表示
      lblStatus.Text = buff;
      lblCharadrawMode.Text = "動画全体";

      // Datasに読み込んだシーン・字幕が設定されている


      lstScene.Nodes.Clear();
      if (Datas.scenes.Count == 0)
      {
        return;
      }

      /*
      foreach (TreeNode n in initdata_scene)
      {
          TreeNode buff = new TreeNode();
          buff.Text = n.Text;
          lstScene.Nodes.Add(buff);
          // lstScene.Nodes.Add(n.Text);
          if (n.Nodes.Count > 0)
          {
              foreach (TreeNode ch in n.Nodes)
              {
                  buff.Nodes.Add(ch.Text);
              }
          }
      }
      */

      TreeNode n;
      foreach (KeyValuePair<string, Scenes> kvp in Datas.scenes)
      {
        n = new TreeNode();
        n.Text = kvp.Key;
        lstScene.Nodes.Add(n);
        // TODO  subtreeの設定はどうする？
        // Datasのscene上でsubnodeを保存できるようにする
        // mdファイルでsubnodeの設定ができるようにする
        // mdファイルの読み込みでDatas.sceneのsubnodeを保存する
      }

      // とりあえず一番先頭を選択状態にする
      // 選択されたシーンのセリフをlstsubtitleに展開する
      handlerowadd = false; // rowadd rowenterをキャンセルする
      lstScene.SelectedNode = lstScene.Nodes[0];
      lstScene.Focus();

      // ここでイベントが発生しpf_SelectScene();が呼ばれているはず
      handlerowadd = true;

      // TODO pg settingを参照し、前回終了時のノードを選択する
      // ファイル名が同じかどうかチェックする必要がある

      // 

    }

    private void pf_SelectScene()
    {
      DataTable dt = pf_ClearSubtitleDatatable();

      string sckey = lstScene.SelectedNode.Text;
      Scenes sc = Datas.scenes[sckey];
      current_scene = sc;
      DataRow d;
      foreach (Quote q in sc.messages)
      {
        // キャラ
        // 立ち絵
        // 字幕
        // 発声記号
        // 速度
        // トーン
        // 素材 これは必要ない
        d = dt.NewRow();
        //d[0] = q.charaname;
        d[1] = q.charafacestr;
        d[2] = q.message;
        d[3] = q.pronmessage;
        d[4] = q.speed;
        d[5] = q.tone;
        d[6] = q.id;
        d[7] = q.charaid;
        dt.Rows.Add(d);
        // int id = dt.Rows.IndexOf(d);
        // q.idに登録された順番にidが設定されている
        // これをhideでdatatableに保存しておき
        // Datasから一括してaccessできるようにする？

      }
      lstSubtitle.DataSource = dt;
      lstSubtitle.Columns["id"].Visible = false;
      lstSubtitle.Columns["charaid"].Visible = false;
      // datasourceはlstsubtitleに対し１個だけ持つ
      // scene毎にはもたない
      dt.ColumnChanged += evt_dt_Column_Changed;
      dt.TableNewRow += evt_dt_TableNewRow;

      // セリフリストの先頭を選択状態にする
      lstSubtitle.Rows[0].Selected = true;
      // frmcharaselectのcharaidの設定
      string charaid = Utils.tostr(dt.Rows[0][7]);
      FrmCharaSelect.func_setChara(charaid);


      // datagridviewのrowenter,rowaddイベントを処理する
      handlerowadd = true;
    }

    private void evt_dt_TableNewRow(object sender, DataTableNewRowEventArgs e)
    {
      // int i = 0;
      // MessageBox.Show("Event Raised...");
    }

    private void evt_dt_Column_Changed(object sender, DataColumnChangeEventArgs e)
    {
      if (handlerowadd == false)
      {
        return;
      }
      DataRow r = e.Row;
      string col = e.Column.ColumnName;
      // DataTable dt = (DataTable)lstSubtitle.DataSource;
      // SceneのQuoteの値を変更する
      // どのQuoteを変更するのか？
      string sckey = lstScene.SelectedNode.Text;
      // Scenes sc = Datas.scenes[sckey];
      // string buff = (string)r["id"];
      /*
      if (buff == null)
      {
        // 行を追加した直後はidには何も設定されてない？
        return;
      
      }
      */

      // 
      // int id = (int)r["id"];
      int id = Utils.toint(r["id"]);
      if (id == Utils.INT_NOPARSE)
      {
        // quote idが設定されていない -> quoteが作られていないと判断
        return;
      }
      // bool ret = int.TryParse(buff, out id);
      Quote q = Datas.quotes[id];
      string buff;
      // TODO tone,messageなどをquoteへ保存
      switch (col)
      {
        case "charaid":
          q.voicecode = (string)r[col];
          break;
        case "face":
          q.charafacestr = (string)r[col];
          break;
        case "subtitle":
          // 発音の設定
          buff = Utils.tostr(r[col]);
          q.message = buff;
          // buff = Yomigana.getYomigana(buff);
          q.pronmessage = buff;
          handlerowadd = false;
          r["pronmessage"] = buff;
          handlerowadd = true;

          break;
      }
      // 編集した発音が表示されないときがある
      lstSubtitle.Refresh();
    }

    private DataTable pf_ClearSubtitleDatatable()
    {
      if (lstSubtitle.DataSource == null)
      {
        lstSubtitle.Rows.Clear();
        lstSubtitle.Columns.Clear();
      }
      DataTable dt = (DataTable)lstSubtitle.DataSource;
      if (dt == null)
      {
        dt = new DataTable();
      }
      dt.Clear();
      DataColumn c;
      if (dt.Columns.Count == 0)
      {
        c = new DataColumn("charaname");
        c.Caption = "キャラ"; // 0
        dt.Columns.Add(c);
        // dt.Columns.Add("キャラ");
        c = new DataColumn("face");
        c.Caption = "立ち絵"; // 1
        dt.Columns.Add(c);
        // dt.Columns.Add("立ち絵");
        c = new DataColumn("subtitle");
        c.Caption = "字幕"; // 2
        dt.Columns.Add(c);
        // dt.Columns.Add("字幕");
        c = new DataColumn("pronmessage");
        c.Caption = "発音"; // 3
        dt.Columns.Add(c);
        // dt.Columns.Add("発音");
        c = new DataColumn("speed");
        c.Caption = "速度"; // 4
        dt.Columns.Add(c);
        // dt.Columns.Add("速度");
        c = new DataColumn("tone"); // 5
        c.Caption = "トーン";
        dt.Columns.Add(c);
        // dt.Columns.Add("トーン");


        // quote id 6
        c = new DataColumn("id");
        c.DataType = System.Type.GetType("System.Int32");
        dt.Columns.Add(c);

        // charaid 7
        c = new DataColumn("charaid");
        dt.Columns.Add(c);

        // TODO なぜかreaonlyにすると、立ち絵のdragdrop時にfalseにしてもエラーとなる
        // おそらく、rowごとではなく、columnに対してreadonlyを解除しないとだめなんだろう
        // dt.Columns[1].ReadOnly = true;


        // lstSubtitle.Rows[rowid].Cells[1].ReadOnly = false;


      }
      // dt.Columns.Add("素材");
      return dt;
    }

    private void cmdSceneAdd_Click(object sender, EventArgs e)
    {
      lstScene.Nodes.Add(txtScene.Text);
      // lstScene.Items.Add(txtScene.Text);
    }




    private void lstSubtitle_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
    {
      // TODO lstのeventではなく、datarowのeventで行う
      if (handlerowadd == false) return;
      int i = e.RowIndex - 1;

      // datagridviewのaddイベントじゃなくてdatasourceのadd eventのほうがいいかも

      // ２行目以降に追加した場合にのみ処理する
      // TODO １行目に追加した場合はどうする？ charaの指定をどうやって行う？
      // 字幕入力画面で追加した場合は別途処理を行う
      if (i < 2) return;
      // １行目の情報をコピーし、デフォルト値を設定する

      // lstsubtitleのevent cancelを設定し、
      // evet処理をスキップさせる
      handlerowadd = false;
      DataGridViewRow d = lstSubtitle.Rows[i];
      DataGridViewRow prevrow = lstSubtitle.Rows[i - 1];
      string charaname = Utils.tostr(prevrow.Cells[0].Value); // 字幕
      string buff;
      // bool ret;
      int speed = Utils.toint(prevrow.Cells[4].Value);
      int tone = Utils.toint(prevrow.Cells[5].Value);
      string charaid = Utils.tostr(prevrow.Cells[7].Value);
      /*
      buff = Utils.tostr(prevrow.Cells[4].Value);
      ret = int.TryParse(buff, out speed);
      buff = (string)prevrow.Cells[5].Value;
      ret = int.TryParse(buff, out tone);
      charaid = (string)prevrow.Cells[7].Value;
      */
      d.Cells[0].Value = charaname; // 前の行のキャラidを設定

      // 0=charaid 1=立ちえ 2=字幕 3=発音 
      d.Cells[4].Value = speed;
      d.Cells[5].Value = tone;

      // TODO 既にq.idが存在している場合にはそのquoteを使う
      Quote q;

      int id = Utils.toint(d.Cells[7].Value);
      if (id != -1)
      {
        q = Datas.quotes[id];
      } else
      {
        q = new Quote();
        // 現在選択されてるシーンにquoteを追加
        // TODO scとnodeをひもづける仕組みがほしいな
        // もしくは、現在選択されてるシーンをglobalにする
        string sckey = lstScene.SelectedNode.Text;
        Scenes sc = Datas.scenes[sckey];
        sc.messages.Add(q);

      }

      // Quote q = new Quote();
      // TODO 追加した行をQuoteに保存 さらにsceneにも保存
      q.charaid = charaid; // charaidでの指定が必要
      q.speed = speed;
      q.tone = tone;
      // TODO quoteのidをdatatableに保存
      // datasourceとの関係はどうなるんだろ？
      d.Cells[6].Value = q.id; // hideされてるけど設定できるか？
      d.Cells[7].Value = Utils.tostr(prevrow.Cells[7].Value); // charaid;
      // 発音を編集
      buff = Utils.tostr(d.Cells[2].Value);
      // buff = Yomigana.getYomigana(buff);
      d.Cells[3].Value = buff;
      q.pronmessage = buff;



      // DataTable dt = (DataTable)lstSubtitle.DataSource;

      // DataRow dr = dt.NewRow();
      /*
      for (i = 0; i < dt.Columns.Count; i++)
      {
        dr[i] = d.Cells[i].Value;
      }
      
      dr[0] = "あ";
      
      */
      // dt.Rows.Add(dr); // drを追加すると、３行追加される？
      // もとからある２行と、gridviewにある３行で合計５行追加されているっぽい
      // drがおかしい?
      // まったく意味わからんな？　なんで？


      // lstSubtitle.Refresh();
      /*
      lstSubtitle.DataSource = null;
      lstSubtitle.Refresh();
      lstSubtitle.DataSource = dt;
      lstSubtitle.Refresh();  // いや、dt自体が５行もってるから意味ないよな
      */
      handlerowadd = true;

    }

    private void lstSubtitle_RowEnter(object sender, DataGridViewCellEventArgs e)
    {
      // lstsubtitle で 行が選択された場合

      // ここでevent cancelを判断する
      // TODO 場合によってはcurrent_quoteを選択した状態にしたい
      if (handlerowadd == false)
      {
        current_quote = null;
        return;
      }
      int i = e.RowIndex;
      DataGridViewRow d = lstSubtitle.Rows[i];
      i = Utils.toint(d.Cells[6].Value);
      if (i == Utils.INT_NOPARSE)
      {
        // quoteが設定されていないrowの場合、スキップする
        // 新規行以外はありえないはず
        return;
      }
      // TODO current_quoteが選択できない状態はありえないはずだが、、、
      current_quote = Datas.quotes[i];
      // 新規行だとid nullはありえる

      string charaid = Utils.tostr(d.Cells[7].Value);


      string buff = (string)d.Cells[0].Value;
      // 新規追加時はcells 0=charanamaはnull
      if (buff == null)
      {
        return;
      }
      txtCharID.Text = buff + "(ctl+" + charaid + ")";
      // null処理 null or dbnull;
      txtSubtitle.Text = Utils.tostr(d.Cells[2].Value);
      txtPronun.Text = Utils.tostr(d.Cells[3].Value);
      // bool ret;
      // ret = int.TryParse((string)d.Cells[4].Value, out i);
      // trkSpeed.Value = i;
      // trkSpeed.Value = Utils.toint(d.Cells[4].Value, Voice.defaultspeed);
      // ret = int.TryParse((string)d.Cells[5].Value, out i);
      // trkTone.Value = i;
      // trkTone.Value = Utils.toint(d.Cells[5].Value, Voice.defaulttone);
      // TODO volumeもdatagridbiewに表示する

      // frmcharaselectに対し、行のcharaを選択状態にする
      FrmCharaSelect.func_setChara(charaid);

      // TODO chara faceの画像を読み込む
      buff = current_quote.charafacestr;
      string pngf = CharaGlobals.charas.charabyid[charaid].getPNGfile(buff);
      if (pngf.Length == 0)
      {
        return;
      }
      pctFace.ImageLocation = pngf;
    }

    private void trkSpeed_ValueChanged(object sender, EventArgs e)
    {
      lblSpeed.Text = trkSpeed.Value.ToString();
    }

    private void trkTone_ValueChanged(object sender, EventArgs e)
    {
      lblTone.Text = trkTone.Value.ToString();
    }

    private void frmSenario_FormClosing(object sender, FormClosingEventArgs e)
    {
      handlerowadd = false;
      // seriflist.Clear();
      // flowimglist.Controls.Clear();
    }


    private void handlecharakey(object sender, KeyEventArgs e)
    {
      if ((Control.ModifierKeys & Keys.Control) == Keys.Control)
      {
        if (e.KeyCode == Keys.R)
        {
          e.Handled = true;
          e.SuppressKeyPress = true;
          clearsubtitle("れいむ");
        }
        if (e.KeyCode == Keys.M)
        {
          e.Handled = true;
          e.SuppressKeyPress = true;
          clearsubtitle("まりさ");
        }
      }
    }

    public void clearsubtitle(string charaid = "れいむ")
    {
      // 字幕入力画面のクリア
      if (!(currentface is null))
      {
        currentface.BackColor = SystemColors.Control;
        currentface.BorderStyle = BorderStyle.FixedSingle;
        currentface = null;

      }
      if (charaid == "れいむ")
      {
        txtCharID.Text = "れいむ(ctl+r)";
        txtSubtitle.Text = "";
        txtPronun.Text = "";
        trkSpeed.Value = 110;
        trkTone.Value = 100;

      } else if (charaid == "まりさ")
      {
        txtCharID.Text = "まりさ(ctl+m)";
        txtSubtitle.Text = "";
        txtPronun.Text = "";
        trkSpeed.Value = 115;
        trkTone.Value = 95;
      }

    }


    private void frmSenario_KeyDown(object sender, KeyEventArgs e)
    {
      handlecharakey(sender, e);
    }

    private void txtSubtitle_KeyDown(object sender, KeyEventArgs e)
    {
      if ((Control.ModifierKeys & Keys.Control) == Keys.Control)
      {
        if ((e.KeyCode & Keys.S) == Keys.S)
        {
          e.Handled = true;
          e.SuppressKeyPress = true;
          pf_addserif();
        }
      }
    }

    private void cmdAdd_Click(object sender, EventArgs e)
    {
      pf_addserif();
    }

    private void pf_addserif()
    {
      // テキストボックスのセリフを追加する
      if (txtPronun.Text.Length == 0)
      {
        // txtPronun.Text = Yomigana.getYomigana(txtSubtitle.Text);
      }
      //DataTable dt = pf_ClearSubtitleDatatable();
      // datatableを使用した追加はうまく動作しない
      // 全面的に見直す必要がある
      // datasourceが設定されている場合はgridviewのadd rowはうまく動作しない
      // datasourceを設定する必要がある
      /*
      int i = lstSubtitle.Rows.Add();
      DataGridViewRow r = lstSubtitle.Rows[i];
      // 字幕入力のコンボボックスでれいむが指定されたらrを設定
      string buff = txtCharID.Text;
      i = buff.IndexOf("(");
      if (i > 0)
      {
        buff = buff.Substring(0, i);
      }
      string charaname = buff;
      handlerowadd = false;
      r.Cells[0].Value = charaname;
      // TODO facekeyって何？
      string facekey = "";
      if (facekey.Length > 0)
      {
        facekey = "set:" + facekey;
      }
      r.Cells[1].Value = facekey;
      // r.Cells[1].Value = 
      // d[1] = "set:" + facekey;
      // d[2] = txtSubtitle.Text;
      r.Cells[2].Value = txtSubtitle.Text;
      // d[3] = txtPronun.Text;
      r.Cells[3].Value = txtPronun.Text;
      //d[4] = trkSpeed.Value.ToString();
      r.Cells[4].Value = trkSpeed.Value.ToString();
      //d[5] = trkTone.Value.ToString();
      r.Cells[5].Value = trkTone.Value.ToString();
      //d[7] = current_charaid; // charaid
      string charaid = Globals.charas.chara[charaname].charaid;
      r.Cells[7].Value = charaid;
      Quote q = new Quote();
      r.Cells[6].Value = q.id;
      handlerowadd = true;

      // quoteの組み立て
      q.charaid = charaid;
      q.charaface = facekey;
      q.message = txtSubtitle.Text;
      q.pronmessage = txtPronun.Text;
      q.speed = trkSpeed.Value;
      q.tone = trkTone.Value;
      // quoteをsceneに追加
      current_scene.messages.Add(q);
      */

      int i;
      Quote q = new Quote();
      current_scene.messages.Add(q);
      string buff = txtCharID.Text;
      i = buff.IndexOf("(");
      if (i > 0)
      {
        buff = buff.Substring(0, i);
      }
      // q.charaname = buff;
      // string charaname = buff;
      q.charaid = CharaGlobals.charas.chara[buff].charakey;
      string facekey = "";
      if (facekey.Length > 0)
      {
        facekey = "set:" + facekey;
      }
      q.charafacestr = facekey;
      q.message = txtSubtitle.Text;
      q.pronmessage = txtPronun.Text;
      q.speed = trkSpeed.Value;
      q.tone = trkTone.Value;
      // q.volume = Voice.defaultvolume;

      DataTable dt = (DataTable)lstSubtitle.DataSource;
      DataRow d = dt.NewRow();
      // =  seriflist.NewRow();
      handlerowadd = false;
      // d[0] = q.charaname;
      d[1] = q.charafacestr;
      d[2] = q.message;
      d[3] = q.pronmessage;
      d[4] = q.speed;
      d[5] = q.tone;
      d[6] = q.id;
      d[7] = q.charaid;

      dt.Rows.Add(d);
      handlerowadd = true;
      

      clearsubtitle(buff); // 字幕入力画面のクリア
      txtSubtitle.Focus();
      FrmCharaSelect.clearselect(); // 立ち絵選択で選択状態をクリア
      // lstSubtitle.Refresh();

    }

    private void txtPronun_KeyDown(object sender, KeyEventArgs e)
    {
      if ((Control.ModifierKeys & Keys.Control) == Keys.Control)
      {
        if ((e.KeyCode & Keys.S) == Keys.S)
        {
          e.Handled = true;
          e.SuppressKeyPress = true;
          pf_addserif();
        }
      }

    }

    private void trkImgsize_Scroll(object sender, ScrollEventArgs e)
    {
      // TODO deleyしてスクロールバーの値がおちついてから
      // サイズ変更を実行する
      /*
      if(trkImgsize.Value == 100)
      {
          return;
      }
      double scale = (double)trkImgsize.Value / 100.0;
      int width = (int)(Icon_Defaultwidth * scale);
      int height = Globals.charas["れいむ"].getaspectheight(width);
      foreach (PictureBox p in flowimglist.Controls)
      {
          p.Width = width;
          p.Height = height;
      }
      */


    }
    public void setFace(string charaid, string partsid)
    {
      string buff = "";
      if (partsid.Substring(0, 3) == "set")
      {
        // セット.txtによる立ち絵指定
        int setnum = 0;
        buff = partsid.Substring(3);
        bool ret = int.TryParse(buff, out setnum);
        // setnumより立ち絵のファイルを取得
        DB.Query q = new DB.Query("charaset");
        q.select = "filename";
        q.where("setnum", setnum);
        q.where("charaid", charaid);
        string imgfile = Globals.charadb.getonefield(q);
        pctFace.ImageLocation = imgfile;
      }
      return ;
    }

    private void tsbOpen_Click(object sender, EventArgs e)
    {
      pf_OpenFile();

    }

    private void lstScene_AfterSelect(object sender, TreeViewEventArgs e)
    {
      handlerowadd = false; // datagridviewのeventをcancelするｃ
      pf_SelectScene();
      handlerowadd = true;
    }


    private void lstSubtitle_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
    {
      /*
      // string a = "";
      // a = "b";
      int col = lstSubtitle.SelectedCells[0].ColumnIndex;
      if(col != 1)
      {
          return;
      }
      int row = lstSubtitle.SelectedCells[0].RowIndex;
      // 立ち絵のセルが選択された場合、立ち絵選択画面を表示
      // モードを切り替え、立ち絵選択モードにする
      // 立ち絵選択モードの時はクリックでその画像のパーツを
      // セリフリストの立ち絵にペースト
      */

    }

    private void lstSubtitle_DragEnter(object sender, DragEventArgs e)
    {
      if (!e.Data.GetDataPresent(DataFormats.StringFormat))
      {
        return;
      }
      e.Effect = DragDropEffects.Copy;
    }

    private void lstSubtitle_DragDrop(object sender, DragEventArgs e)
    {
      if (!e.Data.GetDataPresent(DataFormats.StringFormat))
      {
        return;
      }
      if (lstSubtitle.SelectedCells.Count == 0)
      {
        return;
      }
      // TODO mouse pointerよりdropされた行を判定しそこへ立ち絵をセットする
      // e.Effect = DragDropEffects.Copy;
      string buff = (string)e.Data.GetData(DataFormats.StringFormat);
      string celltext = buff;
      // れいむ,set:0
      int rowid = lstSubtitle.SelectedCells[0].RowIndex;
      // TODO cell 0がnull -> 新規行でキャラ選択がされていない
      string charaid = (string)lstSubtitle.Rows[rowid].Cells[0].Value;
      string[] s = buff.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
      // 選択行のキャラとdropされた立ち絵のキャラが同一ならキャラ文字ははぶく
      if (s[0] == charaid && s.Length > 1)
      {
        celltext = s[1];
      }
      // lstSubtitle.Rows[rowid].Cells[1].ReadOnly = false;

      // cols 1に立ち絵指定文字 (set:4)を設定
      lstSubtitle.Rows[rowid].Cells[1].Value = celltext;
      charaid = Utils.tostr(lstSubtitle.Rows[rowid].Cells[7].Value);
      string fname = CharaGlobals.charas.charabyid[charaid].getPNGfile(celltext);
      pctFace.ImageLocation = fname;

      // lstSubtitle.Rows[rowid].Cells[1].ReadOnly = true;
      // 立ち絵はreadonlyだった
      //lstSubtitle.Rows[rowid].Cells[1].Value = celltext;
      // gridviewのdatasourceは異常な動きをする
      // gridviewに対してデータを設定するように変更
      /*
      DataTable dt = (DataTable)lstSubtitle.DataSource;
      // lstSubtitle.DataSource
      dt.Columns[1].ReadOnly = false;
      // TODO error 位置３に行がありません？　なぜ？
      // おそらく、lstsubtitleで行を追加しているから、datasourceに反映されてない
      dt.Rows[rowid][1] = celltext;
      dt.Columns[1].ReadOnly = true;
      */
    }

    private void cmdExtract_Click(object sender, EventArgs e)
    {
      pf_AviutlExtract();
    }

    private void pf_AviutlExtract()
    {
      // Aviutl.sendtest();


      // aviutlへ出力する
      // 初回出力時、aupファイルを作成する
      // できればout.mp4まで設定したい
      // 読み込んだシナリオファイル part18.txt
      // これから "part18"フォルダを作成

      // aviutlの起動はaviutl.exe part18.aup
      // 新規プロジェクトをsdkで作成できるか？
      // sdkではできないので、win messageを使う必要がある

      // win messageを使ってaviutlを操作できるか調べておく
      // exwinで右クリックで新規プロジェクト 
      // これはOK 
      /*
      if (current_senariofile == null)
      {
        // logs.write シナリオファイルが選択されていないためaupファイルを出力できません
        return;

      }
      if (current_senariofile.Length == 0)
      {
        // logs.write シナリオファイルが選択されていないためaupファイルを出力できません
        return;
      }
      if (aviuobj == null)
      {
        aviuobj = new Aviutl(current_senariofile);
        // TODO aupファイルがあればそれを開く
        // なければコピー
        if (aviuobj.existaup() == false)
        {
          aviuobj.copyprototype();
          aviuobj.setmp4file();

        }
        // aupファイルのmp4出力ファイルの設定
        // TODO voiceの定義は$(PROJECTDIR)\voiceとしたい
        // なので、enviniより定義を取得し、PROJECTDIRを書き換える
        // Globals.envini[PGInifile.INI_Voicedir] = aviuobj.projectdir + "\\voice";
        Globals.voicefolder = aviuobj.voicedir;
        // Utils.sleep(5000);
      }
      */
      if (this.aviuobj == null)
      {
        showstatus("Aviutlが起動されていません");
        return;
      }
      Aviutl au = this.aviuobj;

      // au.senariofile = current_senariofile;
      // au.c
      // Aviutl.copyprototype(current_senariofile);

      // aviu 起動
      // au.startupAviutl();
      // aviuは別で起動 フレーム位置を設定したいため
      // TODO 新規でframe0から展開する場合はどうするか？


      // aviuへcustom obj貼り付け
      string scenekey = lstScene.SelectedNode.Text;
      Datas.Play(scenekey);
      // TODO aviutlをfrontへ
      Processinfo.brintToFrontProcess("aviutl");


    }
    private Aviutl pf_aviuobj()
    {
      if (current_senariofile == null)
      {
        // logs.write シナリオファイルが選択されていないためaupファイルを出力できません
        return null;

      }
      if (current_senariofile.Length == 0)
      {
        // logs.write シナリオファイルが選択されていないためaupファイルを出力できません
        return null;
      }

      if (aviuobj == null)
      {
        aviuobj = new Aviutl(current_senariofile);
        // TODO aupファイルがあればそれを開く
        // なければコピー
        if (aviuobj.existaup() == false)
        {
          aviuobj.copyprototype();
          aviuobj.setmp4file();

        }
        // aupファイルのmp4出力ファイルの設定
        // TODO voiceの定義は$(PROJECTDIR)\voiceとしたい
        // なので、enviniより定義を取得し、PROJECTDIRを書き換える
        // Globals.envini[PGInifile.INI_Voicedir] = aviuobj.projectdir + "\\voice";
        Globals.voicefolder = aviuobj.voicedir;
        // Utils.sleep(5000);
      }
      Aviutl au = aviuobj;
      return aviuobj;
    }

    private void tsbSave_Click(object sender, EventArgs e)
    {
      pf_Save();
    }

    private void pf_Save()
    {
      // シナリオファイルをmdとして保存
      // # シーン
      // ## 子供のシーン
      // r)セリフ[[発音,S100,T100]]@@立ち絵
      // mdfileクラスにまとめる
      // save and load

      // 画面のscene,subtitleをDatasに保存
      // lstsubtitleに保存されていないquoteはDatasにしか存在しない
      // なので、保存のタイミングはここではない
      // タイミングとしてはlstsubtitleのdatasource change時?
      // or sceneの変更時

      // 画面の変更を含め、Datasにすべて保存されている
      // たんにDatasからmdファイルに保存すればよい

      // TODO aviuobjの初期化が必要
      if (this.aviuobj == null)
      {
        // TODO 新規作成の場合どうするか？
        lblMessage.Text = "シナリオファイルを開いていないため保存できません";
        return;
      }
      SenarioMDFile fs = new SenarioMDFile(this.aviuobj);
      fs.Save();
      string buff = "ファイルを保存しました:" + Utils.Files.getfilename(fs.mdfile);
      // lblMessage.AutoSize = true;
      showstatus(buff);
      /*
      statusStrip1.Items[4].Text = buff;
      // lblMessage.Text = buff;
      statusStrip1.Refresh();
      */
    }

    private void showstatus(string arg)
    {
      statusStrip1.Items[4].Text = arg;
    }

    private void menSave_Click(object sender, EventArgs e)
    {
      pf_Save();
    }

    private void lstSubtitle_RowLeave(object sender, DataGridViewCellEventArgs e)
    {
      // datasourceへrowを追加する？
    }

    private void cmdAviutlStartup_Click(object sender, EventArgs e)
    {
      if (current_senariofile == null && current_senariofile.Length == 0)
      {
        showstatus("シナリオファイルを開いていないためAviutlを起動できません");
      }
      this.aviuobj = pf_aviuobj();
      this.aviuobj.startupAviutl();
      this.BringToFront();
    }
  }

}
