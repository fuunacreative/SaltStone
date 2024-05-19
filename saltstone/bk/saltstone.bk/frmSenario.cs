using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace saltstone
{
    public partial class frmSenario : Form
    {
        // public Charas charas;
        Dictionary<string, SortedDictionary<int, PictureBox>> pngdata;
        private const int Icon_Defaultwidth = 100;
        private List<string> initdata_scene;

        private DataTable seriflist;
        private bool handlerowadd = false;

        private string facekey = "";
// #pragma warning disable 0649
        private PictureBox currentface = null;

        public frmSenario()
        {
            InitializeComponent();
            Globals.init();
            init();
            initdata_scene = new List<string>();
            foreach(string o in lstScene.Items)
            {
                initdata_scene.Add(o);
            }
            lstScene.Items.Clear();
            

            seriflist = new DataTable();
            /*
            seriflist.Columns.Add("charaid"); // キャラID れいむ,まりさ
            seriflist.Columns.Add("faceid"); // 立ち絵  セット00 or B00E00など
            seriflist.Columns.Add("subtitle"); // 字幕
            seriflist.Columns.Add("pronun"); // 発音記号
            seriflist.Columns.Add("speed"); // 発声スピード
            seriflist.Columns.Add("tone"); // 発声トーン
            seriflist.Columns.Add("excmd"); // 追加の貼り付け指示
            */
            
            seriflist.Columns.Add("キャラ"); // キャラID れいむ,まりさ
            seriflist.Columns.Add("立ち絵"); // 立ち絵  セット00 or B00E00など
            seriflist.Columns.Add("字幕"); // 字幕
            seriflist.Columns.Add("発音"); // 発音記号
            seriflist.Columns.Add("スピード"); // 発声スピード
            seriflist.Columns.Add("トーン"); // 発声トーン
            seriflist.Columns.Add("素材"); // 追加の貼り付け指示
            // initdata_scene = lstScene.Items
            
            // seriflist.TableNewRow += new DataTableNewRowEventHandler(dt_TableNewRow);

            lstSubtitle.ColumnHeadersDefaultCellStyle.Font = lblDatagridviewFont.Font;
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
            drawpng();
            Globals.charas["れいむ"].charakey = "r";
            Globals.charas["まりさ"].charakey = "m";


        }

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
            Chara c = Globals.charas.chara[charaid];
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
            if(o.BorderStyle == BorderStyle.FixedSingle)
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
            txtPronun.Text = Yomigana.getYomigana(txtSubtitle.Text);
        }

        private void menOpen_Click(object sender, EventArgs e)
        {
            this.dlgOpenFile.ShowDialog();
            foreach(string s in initdata_scene)
            {
                lstScene.Items.Add(s);
            }
        }

        private void cmdSceneAdd_Click(object sender, EventArgs e)
        {
            lstScene.Items.Add(txtScene.Text);
        }

        private void setsampledata_1()
        {
            seriflist.Rows.Clear();
            DataRow d = seriflist.NewRow();
            d[0] = "れいむ";
            d[1] = "";
            d[2] = "スプーンゆっくり";
            d[3] = "[エコー]スプーンゆっくり";
            d[4] = "110";
            d[5] = "115";
            d[6] = "";
            seriflist.Rows.Add(d);

            d = seriflist.NewRow();
            d[0] = "まりさ";
            d[1] = "";
            d[2] = "パート１６です";
            d[3] = "パート１６です";
            d[4] = "95";
            d[5] = "115";
            d[6] = "";
            seriflist.Rows.Add(d);




            /*
            seriflist.Columns.Add("charaid"); // キャラID れいむ,まりさ
            seriflist.Columns.Add("faceid"); // 立ち絵  セット00 or B00E00など
            seriflist.Columns.Add("subtitle"); // 字幕
            seriflist.Columns.Add("pronun"); // 発音記号
            seriflist.Columns.Add("speed"); // 発声スピード
            seriflist.Columns.Add("tone"); // 発声トーン
            seriflist.Columns.Add("excmd"); // 追加の貼り付け指示
            */



        }

        private void lstScene_SelectedIndexChanged(object sender, EventArgs e)
        {
            setsampledata_1();
            lstSubtitle.Columns.Clear();
            lstSubtitle.DefaultCellStyle.Font = lblDatagridviewFont.Font;
            lstSubtitle.ColumnHeadersDefaultCellStyle.Font = lblDatagridviewFont.Font;
            lstSubtitle.DataSource = seriflist;
            lstSubtitle.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            lstSubtitle.Columns[0].Width = 50;
            lstSubtitle.Columns[1].Width = 50;
            lstSubtitle.Columns[4].Width = 50;
            lstSubtitle.Columns[5].Width = 50;
            handlerowadd = true;
        }

        private void lstSubtitle_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            if (handlerowadd == false) return;
            int i = e.RowIndex - 1;

            // ２行目以降に追加した場合にのみ処理する
            if (i < 2) return;
            // １行目の情報をコピーし、デフォルト値を設定する


            DataGridViewRow d =  lstSubtitle.Rows[i];
            DataGridViewRow prevrow = lstSubtitle.Rows[i - 1];
            d.Cells[0].Value = prevrow.Cells[0].Value; // 前の行のキャラidを設定
            // 0=charaid 1=立ちえ 2=字幕 3=発音 
            d.Cells[4].Value = prevrow.Cells[4].Value;
            d.Cells[5].Value = prevrow.Cells[5].Value;
        }

        private void lstSubtitle_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (handlerowadd == false) return;
            int i = e.RowIndex;
            DataGridViewRow d = lstSubtitle.Rows[i];
            string buff = (string)d.Cells[0].Value;
            txtCharID.Text = buff + "(ctl+" + Globals.charas[buff].charakey;
            txtSubtitle.Text = (string)d.Cells[2].Value;
            txtPronun.Text = (string)d.Cells[3].Value;
            bool ret;
            ret = int.TryParse((string)d.Cells[4].Value, out i);
            trkSpeed.Value = i;
            ret = int.TryParse((string)d.Cells[5].Value, out i);
            trkTone.Value = i;
            
            

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
            seriflist.Clear();
            flowimglist.Controls.Clear();
        }


        private void handlecharakey(object sender , KeyEventArgs e)
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

            } else if(charaid == "まりさ")
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
            if((Control.ModifierKeys & Keys.Control) == Keys.Control)
            {
                if((e.KeyCode & Keys.S) == Keys.S)
                {
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    addserif();
                }
            }
        }

        private void cmdAdd_Click(object sender, EventArgs e)
        {
            addserif();
        }

        private void addserif()
        {
            // テキストボックスのセリフを追加する
            if(txtPronun.Text.Length == 0)
            {
                txtPronun.Text = Yomigana.getYomigana(txtSubtitle.Text);
            }
            DataRow d =  seriflist.NewRow();
            string buff = txtCharID.Text;
            int i = buff.IndexOf("(");
            if(i > 0)
            {
                buff = buff.Substring(0, i);
            }
            d[0] = buff;
            // d[1] = ""; // face
            if (facekey.Length > 0)
            {
                d[1] = "set:" + facekey;

            } else
            {
                d[1] = "";
            }
            d[2] = txtSubtitle.Text;
            d[3] = txtPronun.Text;
            d[4] = trkSpeed.Value.ToString();
            d[5] = trkTone.Value.ToString();
            seriflist.Rows.Add(d);
            clearsubtitle(buff);
            txtSubtitle.Focus();

            
        }

        private void txtPronun_KeyDown(object sender, KeyEventArgs e)
        {
            if ((Control.ModifierKeys & Keys.Control) == Keys.Control)
            {
                if ((e.KeyCode & Keys.S) == Keys.S)
                {
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    addserif();
                }
            }

        }

        private void trkImgsize_Scroll(object sender, ScrollEventArgs e)
        {
            // TODO deleyしてスクロールバーの値がおちついてから
            // サイズ変更を実行する
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
        }
    }

}
