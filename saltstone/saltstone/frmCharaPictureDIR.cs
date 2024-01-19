using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;


// 処理内容
// dirキャラ素材 -> パーツ解析など -> psd or png
// パーツ解析結果はdbに保存
// アニメ処理するとか、非表示パーツ
// 顔・眉・目・口・体、前パーツ、後ろパーツ
// お気に入り
// aviulのluaから呼び出し、立ち絵bitmapをrender
// できれば、aviuのスライダーを動かしたいな


namespace saltstone
{
    public partial class frmCharaPictureDIR : Form
    {

        // singleton
        private static frmCharaPictureDIR f;

        public static frmCharaPictureDIR getInstance()
        {
            if(f == null)
            {
                f = new frmCharaPictureDIR();
            }
            return f;
        }


        public string chardirectory = "";
        public Charas charas = null;
        public frmCharaPictureDIR()
        {
            InitializeComponent();
        }



        private void frmChara_Load(object sender, EventArgs e)
        {
            //　https://www.atmarkit.co.jp/ait/articles/0508/12/news091.html
            // listviewでのアイコン表示 お気に入りはアイコンから選択したほうが早い
            StatusStrip s = new StatusStrip();
            chardirectory = Globals.envini.get("chardir");
            statusbar.Items.Add(chardirectory);

            // キャラ素材ディレクトリの解析
            // 各パーツとアニメするかどうか

            init();

        }

        public void init()
        {
            // キャラディレクトリを取得し、キャラリストに設定
            // string path = "";

            // FolderBrowserDialog d = new FolderBrowserDialog();
            // d.ShowDialog();

            /*
            charas = new Charas();
            // charas.directoryparse();
            lstChara.Items.Clear();
            foreach(string s in charas.chara.Keys)
            {
                lstChara.Items.Add(s);
            }
            */
        }

        public void Show(string filepath)
        {
            img.ImageLocation = filepath;
            frmCharaPictureDIR.f.Show();
            // this.Show();

        }

        private void frmChara_FormClosing(object sender, FormClosingEventArgs e)
        {
            f.Hide();
            e.Cancel = true;
        }
    }
}
