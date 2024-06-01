using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Utils;


// キャラdirのoutのファイルを読み込み、listviewにlargiconで表示
// パーツ番号を調べるのに使う
// これがあるだけでも、パーツスライダーでの選択が簡単にできるはず
// 最終的にはybchara.exeと連携してファイルを作成
// favorite.txtの編集 -> お気に入り追加
// chara.txtの編集 -> パーツ名の編集
// aviutilへ送る -> 現在の立ち絵objに対し、パーツ番号を指定する -> dialogのコンロトールをMSGで変更しないといけないかも

// 最低、 顔、目、口、眉の４つが選択できればかなり違う
// これでフィルタリングをかけられるといい
// 別画面にしたほうがすっきりするかも
// 選択した立ちえのパーツ番号がわかればとりあえず使えるようになる
// 後は後ろと全部

// 何がしたいか？
// outに出力されているセット.txtから合成された画像を選択し
// それに設定されているパーツ番号を取得する -> aviutlのマルチスライダーに番号を設定
// statusバーに番号が表示されるので、それでとりあえずはOK
// 順番は顔、目、口、眉 これをどこかに表示すればいい
// statusバーにこの順番で表示させる <- db処理しておき、setnumから取得する？
// あとはフィルターだな いちいち別画面を表示するのは面倒
// しかし、リスト画面にコントロールを配置するのも画面の大きさ的にちょっと難しいかな

// TODO 開度によりアニメパーツはどのようにｄｂに保持するか？




namespace saltstone
{
  public partial class FrmCharaSelect : Form
  {
    private const int Icon_Defaultwidth = 200;
    // アスペクト比率の計算 
    public const string envcharaselected = "lastselectedchara";

    private string charaid; // れいむとかまりさ
    private string windowtitle; // 初期値のタイトル タイトルバーに立ちえのパーツ番号を表示させる
                                // private string charadir; // キャラ素材のサブディレクトリを含むキャラのディレクトリ

    private static FrmCharaSelect f;
    // フォームのsingleton
    // private static Dictionary<int, string> outfdic; // imglistのindexとファイルの先頭文字 114を保存
    // private static Dictionary<string ,string> outfname; // ファイル先頭文字 114に対してファイル名を保持
    // これがわかればパーツ番号がわかる

    // イメージが選択された時のfunc
    // public void setFace(string charaid, string partsid)
    public delegate void delg_getImageSelected(string charaid , string partsid,Image img);
    public static delg_getImageSelected exf_getImageSelected;
    // delegateを設定してeventかする方法と、interfaceを実装してmethodでmainから取り出す方法と２つある
    // main側から主導して情報取得したい場合にはinterface
    // きゃら選択画面で勝手に動かす場合はeventか

    public Charas charas;

    // pictureboxの保持用の変数
    Dictionary<string, SortedDictionary<int, PictureBox>> pngdata;
    // れいむ -> setnum , pngのpath
    // 再検索時、setnumの一覧をlistでもらいたい
    // dictonaryのkey指定して一括して抜き出せないか

    public Form mainform; // inf_form_setface

    public static string selectedCharaID; //set01とか

    // 現在選択中のキャラのpicturebox dragd時も有効
    public PictureBox currentselectpct;

    public FrmCharaSelect()
    {
      InitializeComponent();
      this.lstChara.MouseWheel += evt_MouseWheel;
      this.StatusLabel.Text = "";
      imgsizebar.Value = FrmCharaSelect.Icon_Defaultwidth;
      lstFace.MouseWheel += evt_MouseWheel;
      // this.lstFace2.MouseWheel += evt_MouseWheel;
    }

    // mouse wheelのイベント処理
    public void evt_MouseWheel(object sender, MouseEventArgs e)
    {
      HandledMouseEventArgs eventArgs = e as HandledMouseEventArgs;
      eventArgs.Handled = true;
      VScrollProperties scroll = this.flowimglist.VerticalScroll;
      int i = scroll.Value;
      if (e.Delta < 0)
      {
        i += 640;
      } else
      {
        i -= 640;
      }
      if (i < 0) i = 0;
      if (i > scroll.Maximum) i = scroll.Maximum;
      scroll.Value = i;
    }

    private void frmCharaFavorite_FormClosing(object sender, FormClosingEventArgs e)
    {
      e.Cancel = true;
      this.Hide();
    }
    public static FrmCharaSelect showwindows(string charaid = "")
    {
      // formのsingleton
      if (f == null)
      {
        f = new FrmCharaSelect();
        foreach (Control c in f.Controls)
        {
          c.MouseWheel += f.evt_MouseWheel;
        }
      }
      f.charaid = "";
      if (charaid != "")
      {
        f.charaid = charaid;
      }

      f.Show();
      f.init();
      return f;
    }

    /*
    public static FrmCharaSelect SelectCharaPng()
    {
        // １つの立ち絵選択モードにする
        // delegateを使いコールバックする？
        // modalにすればよいのだが、２つのキャラ選択画面が表示されるのは具合よくない
        // 修正モードにすればよいんだ

        return f;
    }
    */


    private void frmCharaFavorite_Load(object sender, EventArgs e)
    {

      // partslist.Rows.Add({ "顔","00","困り顔"});
      // imglist.Columns.Add("name"); 

    }

    public void init()
    {
      // キャラ素材dirのybchar.dbより取得したいな
      // table char ( name fullpath, psdfile) // fullpathしか管理しない 必要ないのでは？
      // flag use とか、何に使うの？
      // tableにする必要性が薄い
      // 一応作るか
      // キャラ素材ディレクトリのybchara.dbよりキャラ名の一覧を取得


      // 問題としてサービス層がない
      // orマッピングとか、必要な情報を取得するAPI層がほしい

      // TODO ｄｂより設定されたキャラ名を取得
      // シナリオファイルで指定されているものだけ表示したほうがよいかもしれない

      if (charas == null)
      {

        // charas = new Charas();
        charas = CharaGlobals.charas;
      }
      this.windowtitle = this.Text; // タイトルを初期値として保存する -> 立ちえ選択時にタイトルバーにパーツ番号を表示する

      /*
      string charadbname = charas.charadbfullpath;

      DB.Sqlite db = new DB.Sqlite(charadbname);
      // DB.DBRecord rec = DB
      // DB.Query q = new DB.Query("charactor");
      DB.Query q = new DB.Query(Charas.table_charactor);
      q.select = "name";
      DB.DBRecord rec = db.getrecord(q);
      while(rec.Read() == true)
      {
          txtChara.Items.Add(rec["name"]);
          // recにout file pngが入っているので
          // flow panelでpictureboxを追加していく
          // tagにsetnumをセットして、ダブルクリックでパーツ番号が判定できるようにしたい
      }
      db.Dispose();
      */
      string charaid = f.charaid;
      if (charaid == "")
      {
        charaid = Globals.envini[FrmCharaSelect.envcharaselected];
      }
      int i = 0;
      int sidx = 0;
      lstChara.Items.Clear();
      foreach (string s in charas.chara.Keys)
      {
        // sにはれいむ or まりさが入る
        // TODO 最終的には charasのkeyはcharaidにする
        lstChara.Items.Add(s);
        if (s == charaid)
        {
          sidx = i;
        }
        i++;
        //txtChara.Items.Add(s);
      }
      lstChara.SelectedIndex = sidx;
      pf_drawpng();
      // TODO lstboxで、indexとcharaid,dispname,charaobjを対応できるようにする

      // 顔・目・口・眉の検索用リストボックスをｄbよりreadする
      /*
      Chara c = charas.chara[lstChara.Text];
      // List<string> lst = c.getparts("顔"); // 顔用パーツリストを読み込み
      Dictionary<string, ListBox> filterctl = new Dictionary<string, ListBox>();
      filterctl["顔"] = lstFace;
      filterctl["目"] = lstEye;
      ListBox lctl;
      foreach(KeyValuePair<string,ListBox> pctl in filterctl)
      {
          SortedDictionary<string, string> lst = c.getparts(charaid, pctl.Key);
          // lstには key=00,name=通常とかがはいっている
          lctl = pctl.Value;
          lctl.Items.Clear();
          // lstFace.Items.Clear();
          foreach (KeyValuePair<string, string> p in lst)
          {
              // buff = p.Key + ":" + p.Value;
              lctl.Items.Add(p);
              // lstFace.Items.Add(p);
              // 顔フィルターをクリックされたら、それをキーにしてセット.txtを検索し
              // 一致するもののみpngをリスト表示したい
              // 検索するキーを
          }
          lctl.DisplayMember =  "Value";
          // lstFace.DisplayMember
      }
      */

      lstChara.Focus();
      // txtChara.Text = txtChara.Items[0].ToString();
    }

    /*
    public bool searchoutfile()
    {
        // outディレクトリを検索してファイル名のリストを取得
        // ファイル名で昇順ソート
        // string path = @"F:\yukkuri_spoon\キャラ素材\れいむ\out";
        Globals.envini.get("chardir");


        // IEnumerable<string> fs = Directory.EnumerateFiles(path, "*.png");

        if (outfname == null)
        {
            outfname = new Dictionary<string, string>();
        }

        // ファイル名を先頭の_前 113を数値に変換してorderbyしないといけない
        Dictionary<int, string> fsdic = new Dictionary<int, string>();
        string fname;
        int i;
        int fnum;
        string buff;
        // 検索されたファイルをまわし、先頭文字をintに変換してfsdcに保存 -> 後でソートしてimglistに登録する際に使用する
        foreach (string pngf in fs)
        {
            fname = Path.GetFileName(pngf);
            i = fname.IndexOf("_");
            if(i == -1)
            {
                continue;
                // _がないファイルは登録しない
            }
            buff = fname.Substring(0, i);
            fnum = int.Parse(buff);
            fsdic[fnum] = pngf;
            outfname[buff] = pngf;


        }

        ImageList ls = imglist.LargeImageList;
        if (ls == null)
        {
            ls = new ImageList();
            ls.ImageSize = new Size(Icon_Defaultwidth, Icon_Defaultwidth);
        }

        if (outfdic == null)
        {
            outfdic = new Dictionary<int, string>();
        }

        int count = 0;
        // string buff;
        string s;
        IOrderedEnumerable<KeyValuePair<int,string>> orderdpng = fsdic.OrderBy(selector => { return selector.Key; });
        // 先頭文字をint変換してソートされた orderdpngに対してまわす imglistに登録
        foreach (KeyValuePair<int,string> item in orderdpng)
        {
            s = item.Value;
            Image a = Image.FromFile(s);
            Image dst = imgresize(a);
            //imglist.LargeImageList.add(dst);
            // dst.Tag = s; // tagを追加してもimglistからは取得できない 別に管理する必要がある
            ls.Images.Add(dst);
            //                 pctchara.Image = dst;
            // break;

            // ファイル名の先頭 _の前、 113_B*の113を取り出す
            // selecteditem のimageindexでindexが取得できる
            // lsのaddでは0から順番に追加されると思われる
            // index -> filename先頭 113をdictionaryで管理
            // 選択されたitemからファイル名を特定できるようにする

            buff = Path.GetFileName(s);
            i = buff.IndexOf("_");
            if (i == -1)
            {
                count++;
                continue;
            }
            buff = buff.Substring(0, i);
            outfdic[count] = buff;



            count++;
        }
        imglist.LargeImageList = ls;

        for (i = 0; i < ls.Images.Count; i++)
        {
            imglist.Items.Add(outfdic[i], i);
        }
        //             imglist.Items.Add("name", 0);
        //imglist.View = View.LargeIcon;
        return true;

    }
*/




    private void button1_Click(object sender, EventArgs e)
    {
      ImageList c = new ImageList();
      // int i = 0;

      for (int j = 0; j < 10; j++)
      {
        Image a = Image.FromFile(@"F:\yukkuri_spoon\キャラ素材\れいむ\a.png");

        c.ImageSize = new Size(100, 100);
        c.Images.Add(a);
      }


      // imglist.LargeImageList.Images.Add(a);
    }

    private void txtChara_SelectedIndexChanged(object sender, EventArgs e)
    {

    }

    private void lstChara_SelectedIndexChanged(object sender, EventArgs e)
    {
      // 選択されたキャラをiniに書き込む
      string buff = lstChara.SelectedItem.ToString();
      Globals.envini[FrmCharaSelect.envcharaselected] = buff;
      // flowpanelへの追加処理
      pf_drawpng();
    }

    private void pf_drawpng()
    {
      // 初期化
      flowimglist.Controls.Clear();

      // lstcharaで選択されたキャラのoutディレクトリのpngをdrawする
      string charaid = lstChara.SelectedItem.ToString();
      Chara c = charas.chara[charaid];
      // 検索機能を実装する
      // どうやって渡すか？
      // Dictionary<string,string> で体、00とかかな
      // Dictionary<string, string> where = new Dictionary<string, string>();
      List<string> where = new List<string>();
      if (lstFace2.SelectedItem != null)
      {
        where.Add(getkey(lstFace2));
        // where. (getkey(lstFace));
        //where["顔"] = getkey(lstFace);
      }

      // Chara c = charas[charaid];
      // pngdataから、getoutpngで取得できたsetnumの一覧のみのデータを抽出したい
      //            int[] arys = { 1, 2 };
      // IEnumerable<int> results = outpng.Keys.ToArray().Intersect(arys);
      // いずれにしても、dbから取得したsetnumの一覧を元にflowlauoutに並べる必要がある
      // SortedDictionary<int, PictureBox> png = pngdata[charaid];
      SortedDictionary<int, PictureBox> png = getpngdata(charaid);

      // pictureboxのclick not fire test
      /*
      PictureBox a = new PictureBox();
      a.Click += evt_img_Click;
      a.BorderStyle = BorderStyle.Fixed3D;

      flowimglist.Controls.Add(a);
      */

      /*
       * foreachを使ったほうが簡潔なのでロジック変更
      List<int> dispsetnum = c.getoutsetnum(charaid, where);
      foreach (int j in dispsetnum)
      {
        // png[j]
        flowimglist.Controls.Add(png[j]);

      }
      */
      // なぜかpngdata内部でimgにpctを追加しても、countはあがるが表示されない？
      foreach (PictureBox p in png.Values)
      {
        // p.Click += evt_img_Click; dragdではイベントは発生しない
        flowimglist.Controls.Add(p);
      }
      


    }

    // charaidを元にdbを検索し、pngへのパスを取得する
    // 内部変数にデータとして保存
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
      Chara c = charas.chara[charaid];
      // intはセット.txtのsetnum
      SortedDictionary<int, string> outpng = c.getoutpng(charaid);
      SortedDictionary<int, PictureBox> pngs = new SortedDictionary<int, PictureBox>();
      pngdata[charaid] = pngs;
      string buff;
      foreach (KeyValuePair<int, string> s in outpng)
      {
        // pictureboxへの参照を保存する
        // 何度もpictureboxのインスタンスを作るのは処理が重い
        // キャラid（れいむ、まりさ）が変更になった場合どうするか？
        // 全部のキャラのpictureboxを保持する


        PictureBox p = new PictureBox();
        // 縦横比率がおかしい 400x320 => 150 * 120
        p.Width = FrmCharaSelect.Icon_Defaultwidth;
        p.Height = c.getaspectheight(FrmCharaSelect.Icon_Defaultwidth);
        // p.Height = 120;
        // p.SizeMode = PictureBoxSizeMode.StretchImage;
        // widhとheighを管理する必要がある
        p.Enabled = true;
        p.SizeMode = PictureBoxSizeMode.Zoom;
        p.ImageLocation = s.Value;
        p.BorderStyle = BorderStyle.FixedSingle;
        p.Margin = new Padding(0);
        p.Click += evt_img_Click;
        p.DoubleClick += evt_img_DoubleClick;
        // mouse downでdrag effectをかけると clickイベントが発生しない
        p.MouseDown += evt_MouseDown;
        //p.Tag = 1; // setnumをいれる
        //p.Tag = s.Key;
        buff = charaid + ",set:" + s.Key.ToString();
        p.Tag = buff;
        pngs[s.Key] = p;
        // flowimglist.Controls.Add(pngs[s.Key]);
        // img listにcontrolsは追加されているが、表示されない



        // this.flowimglist.Controls.Add(p);
      }
      // flowimglist.Refresh();
      return pngs;


    }



    private SortedDictionary<string, PictureBox> getcharaoutpng(string charaid)
    {
      // 内部変数として保持するべきだな

      // キャラいｄで指定されたすべてのoutのpictureboxを取得するれいむ,set:1
      Chara c = charas.chara[charaid];
      return null;


    }

    private void evt_MouseDown(object sender, MouseEventArgs e)
    {
      PictureBox p = (PictureBox)sender;
      if (p == null)
      {
        return;
      }
      currentselectpct = p;
      /*            Image img = p.Image;
                  if(img == null)
                  {
                      return;
                  }
      */
      string buff = (string)p.Tag;
      if (buff == null)
      {
        return;
      }
      buff = "charaselect," + buff;
      // TODO buffにはキャラ選択画面からのdragdであることがわかるように指定文字をいれる
      // TODO 選択されたimageが判定できるように内部に選択されたものを保存しておく -> 後でmainからgetする

      // imgはデータ set:00などの文字列にする
      // どの立ち絵を使うかがわかるようにすればよい
      // dragdropのデータにはbuffを入れる
      DragDropEffects ret = DoDragDrop(buff, DragDropEffects.Copy);

      // string buff = ret.ToString();

      /*
          if (DoDragDrop(img, DragDropEffects.Move) == DragDropEffects.Move)
      {
          pictureBox1.Image = null;
      }
      */
    }

    private void evt_img_Click(object sender, EventArgs e)
    {
      PictureBox p = (PictureBox)sender;
      string buff = p.ImageLocation;
      // buff = Utils.getfilename(buff);
      // this.StatusLabel.Text = buff;


      // imgbox.ImageLocation = p.ImageLocation;
      // 別のキャラ立ちえフォームをあげたほうがよさそう

      // setnumを指定したら顔、目、口、眉のパーツ番号が取得できるようにしたい
      // setnumはセット.txtで指定された番号 
      // 独自定義はどうするか？
      // intにしないと順番に表示されない 
      // 別のキーを追加すればいいか
      // psdtoolkitstrの列を追加 ここにB00E00L00Y00を編集しておく
      // setnumをキーにしてこの文字を取得し、yb_gui側で解析してstatusbarに表示
      // そのまま顔00目00口00眉00というように編集しておけばよいか

      //{ "ID", "B", "F", "E", "Y", "L", "K", "T", "H", "" };
      //B = body,F = 顔,E = 目,Y = 眉,L = 口,K = 後,H = 髪
      int setnum = (int)p.Tag;
      string charaid = lstChara.SelectedItem.ToString();

      // パーツ情報をstatusバーに表示
      Chara c = charas.chara[charaid];
      buff = c.getpsdtoolkitstr(charaid, setnum);
      this.StatusLabel.Text = buff;

      this.Text = this.windowtitle + " " + buff;

      // 選択した画像にのパーツをフィルターとして表示されている顔、目に反映させてはどうか？
      // setnumが取得できている
      // Charaクラスのcも取得できている
      // 

      // 選択状態をわかるようにする
      if (p.BorderStyle == BorderStyle.None || p.BorderStyle == BorderStyle.FixedSingle)
      {
        if (currentselectpct != null)
        {
          currentselectpct.BorderStyle = BorderStyle.FixedSingle;
          currentselectpct.BackColor = SystemColors.Control;
        }
        p.BorderStyle = BorderStyle.Fixed3D;
        p.BackColor = Color.OrangeRed;
        currentselectpct = p;

      } else
      {
        p.BorderStyle = BorderStyle.FixedSingle;
        p.BackColor = SystemColors.Control;
        currentselectpct = null;
      }

      // 選択されたセット.txtのsetnumを立ち絵として使用する
      // frmsenario側にpublic funcを用意し、それをコール
      buff = "set" + setnum.ToString();
      selectedCharaID = buff;
      // mainform.setFace(lstChara.Text, buff);
      // p.Image;
      if (FrmCharaSelect.exf_getImageSelected == null)
      {
        return;
      }
      if (exf_getImageSelected != null)
      {
        exf_getImageSelected(lstChara.Text,buff, p.Image);
      }

    }

    public static void clearselect()
    {

      if (f.currentselectpct != null)
      {
        f.currentselectpct.BorderStyle = BorderStyle.FixedSingle;
        f.currentselectpct.BackColor = SystemColors.Control;
        f.currentselectpct = null;
      }
    }
    private void evt_img_DoubleClick(object sender, EventArgs e)
    {
      // TODO dragdを発生させるとclickイベントがうごかない
      // double clickで選択状態にさせる
      // 別画面の立ち絵編集はボタンにするか、右クリックにする


      // 別画面の立ちえ詳細画面を表示する
      PictureBox p = (PictureBox)sender;
      string buff = p.ImageLocation;
      buff = Utils.Files.getfilename(buff);
      this.StatusLabel.Text = buff;
      // imgbox.ImageLocation = p.ImageLocation;
      // 別のキャラ立ちえフォームをあげたほうがよさそう
      frmCharaPicture f = frmCharaPicture.getInstance();
      f.Show(p.ImageLocation);

    }


    // お気に入りの名前を変更する場合にはlstboxじゃなくlstviewにしないといけないみたい

    private void lstFavorite_Click(object sender, EventArgs e)
    {

    }

    private void lstFace_SelectedIndexChanged(object sender, EventArgs e)
    {
      KeyValuePair<string, string> p = (KeyValuePair<string, string>)lstFace2.SelectedItem;

      this.StatusLabel.Text = getkey(lstFace2);
      // 顔のキー F00が得られるのでこれで絞込みする
      // charaset where searchkey = "*F00"
      pf_drawpng();
    }

    // 指定されたリストボックスで選択されている行のキーを取得する
    // lフィルタ用のリストボックスにはitemにdictonaryがはいっていて
    // itemはkeyvaluepairになっている
    private string getkey(System.Windows.Forms.ListBox l)
    {
      KeyValuePair<string, string> p = (KeyValuePair<string, string>)l.SelectedItem;
      return p.Key;
    }

    private void imgsizebar_Scroll(object sender, EventArgs e)
    {

      // 保持しているpictureboxのサイズをすべて変更
      string charaid = lstChara.SelectedItem.ToString();
      Chara c = charas[charaid];
      int width = imgsizebar.Value;
      int height = c.getaspectheight(width);

      // TODO 処理が遅い
      // pictureboxでzoomで処理しているからだと思われる
      // どうすればいいか？
      // 表示しているもののみ変更し、後ろは別スレッドで処理するとか
      SortedDictionary<int, PictureBox> ps = getpngdata(charaid);
      foreach (PictureBox p in ps.Values)
      {
        p.Width = width;
        p.Height = height;
      }

    }

    private void lstEye_SelectedIndexChanged(object sender, EventArgs e)
    {
      // クリックで選択されたパーツで絞込みを行いたい
      // どうすればいいか？パーツ番号をどうやって取り出して、どうやって検索をかけるか？
      KeyValuePair<string, string> p = (KeyValuePair<string, string>)lstEye.SelectedItem;
      // p.key = "E04" が入ってる
      // outディレクトリ内を検索していると思うので
      // こいつからE04で検索をかければいい？
      // outには目04ではいってる
      // となると、合成処理を作ってdbに保存をかける？
      // dbにfavoriteがはいってるか？


    }

    public static void func_setChara(string charaid)
    {
      // charaidからlistのキャラを選択する
      // int i = 0;
      if (f == null)
      {
        return;
      }
      // f.lstChara.Items
      // charaid (r or m)からlstcharaのselect状態を変更する
      // f.lstChara.SelectedItem = Globals.charas[charaid]
      string buff = CharaGlobals.charas.charabyid[charaid].dispname;
      f.lstChara.SelectedItem = buff;
    }

    public static Image exf_getSelectedCharaImage()
    {
      if (f == null)
      {
        return null;
      }
      if (f.currentselectpct == null)
      {
        return null;
      }
      Image img = f.currentselectpct.Image;
      return img;

    }

  }
}
