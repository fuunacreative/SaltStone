
namespace saltstone
{
    partial class frmSenario_bak
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージド リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("オープニング");
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("茶番.1");
            System.Windows.Forms.TreeNode treeNode3 = new System.Windows.Forms.TreeNode("茶番", new System.Windows.Forms.TreeNode[] {
            treeNode2});
            System.Windows.Forms.TreeNode treeNode4 = new System.Windows.Forms.TreeNode("発電比較");
            System.Windows.Forms.TreeNode treeNode5 = new System.Windows.Forms.TreeNode("原子炉ボーナス");
            System.Windows.Forms.TreeNode treeNode6 = new System.Windows.Forms.TreeNode("水流量");
            System.Windows.Forms.TreeNode treeNode7 = new System.Windows.Forms.TreeNode("蒸気流量");
            System.Windows.Forms.TreeNode treeNode8 = new System.Windows.Forms.TreeNode("原子炉", new System.Windows.Forms.TreeNode[] {
            treeNode5,
            treeNode6,
            treeNode7});
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSenario_bak));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lstSubtitle = new System.Windows.Forms.DataGridView();
            this.charid = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.charaface = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.subtitle = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pronun = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.speed = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tone = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.menAviutilStart = new System.Windows.Forms.GroupBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.button4 = new System.Windows.Forms.Button();
            this.checkBox3 = new System.Windows.Forms.CheckBox();
            this.button2 = new System.Windows.Forms.Button();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.button1 = new System.Windows.Forms.Button();
            this.lstVoiceEffect = new System.Windows.Forms.ListBox();
            this.pctFace = new System.Windows.Forms.PictureBox();
            this.trkImgsize = new System.Windows.Forms.HScrollBar();
            this.lblTone = new System.Windows.Forms.Label();
            this.lblVolume = new System.Windows.Forms.Label();
            this.lblSpeed = new System.Windows.Forms.Label();
            this.cmdAdd = new System.Windows.Forms.Button();
            this.trkVolume = new System.Windows.Forms.TrackBar();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.trkTone = new System.Windows.Forms.TrackBar();
            this.trkSpeed = new System.Windows.Forms.TrackBar();
            this.txtPronun = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtSubtitle = new System.Windows.Forms.TextBox();
            this.txtCharID = new System.Windows.Forms.ComboBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.lstScene = new System.Windows.Forms.TreeView();
            this.cmdSceneAdd = new System.Windows.Forms.Button();
            this.txtScene = new System.Windows.Forms.ComboBox();
            this.menMain = new System.Windows.Forms.MenuStrip();
            this.menFile = new System.Windows.Forms.ToolStripMenuItem();
            this.menOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.menSave = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.menNewAup = new System.Windows.Forms.ToolStripMenuItem();
            this.menSetting = new System.Windows.Forms.ToolStripMenuItem();
            this.menWindow = new System.Windows.Forms.ToolStripMenuItem();
            this.menCharaselect = new System.Windows.Forms.ToolStripMenuItem();
            this.menVoice = new System.Windows.Forms.ToolStripMenuItem();
            this.menVoicePlay = new System.Windows.Forms.ToolStripMenuItem();
            this.menVoiceExtract = new System.Windows.Forms.ToolStripMenuItem();
            this.動画管理ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dlgOpenFile = new System.Windows.Forms.OpenFileDialog();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.button3 = new System.Windows.Forms.Button();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblCharadrawMode = new System.Windows.Forms.ToolStripStatusLabel();
            this.separator1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.separator2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblMessage = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.tsbOpen = new System.Windows.Forms.ToolStripButton();
            this.tsbSave = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.cmdAviutlStartup = new System.Windows.Forms.ToolStripButton();
            this.cmdReadoutScene = new System.Windows.Forms.ToolStripButton();
            this.cmdExtract = new System.Windows.Forms.ToolStripButton();
            this.lblDatagridviewFont = new System.Windows.Forms.Label();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripProgressBar1 = new System.Windows.Forms.ToolStripProgressBar();
            this.toolStripStatusLabel3 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripProgressBar2 = new System.Windows.Forms.ToolStripProgressBar();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.lstSubtitle)).BeginInit();
            this.menAviutilStart.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pctFace)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trkVolume)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trkTone)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trkSpeed)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.menMain.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lstSubtitle);
            this.groupBox1.Location = new System.Drawing.Point(168, 29);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.groupBox1.Size = new System.Drawing.Size(582, 720);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "セリフ一覧";
            // 
            // lstSubtitle
            // 
            this.lstSubtitle.AllowDrop = true;
            this.lstSubtitle.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCells;
            this.lstSubtitle.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCells;
            this.lstSubtitle.ColumnHeadersHeight = 26;
            this.lstSubtitle.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.charid,
            this.charaface,
            this.subtitle,
            this.pronun,
            this.speed,
            this.tone});
            this.lstSubtitle.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnKeystroke;
            this.lstSubtitle.Location = new System.Drawing.Point(8, 27);
            this.lstSubtitle.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.lstSubtitle.Name = "lstSubtitle";
            this.lstSubtitle.RowHeadersWidth = 24;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("メイリオ", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lstSubtitle.RowsDefaultCellStyle = dataGridViewCellStyle1;
            this.lstSubtitle.RowTemplate.Height = 27;
            this.lstSubtitle.Size = new System.Drawing.Size(564, 690);
            this.lstSubtitle.TabIndex = 0;
            this.lstSubtitle.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.lstSubtitle_CellDoubleClick);
            this.lstSubtitle.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.lstSubtitle_RowEnter);
            this.lstSubtitle.RowLeave += new System.Windows.Forms.DataGridViewCellEventHandler(this.lstSubtitle_RowLeave);
            this.lstSubtitle.RowsAdded += new System.Windows.Forms.DataGridViewRowsAddedEventHandler(this.lstSubtitle_RowsAdded);
            this.lstSubtitle.DragDrop += new System.Windows.Forms.DragEventHandler(this.lstSubtitle_DragDrop);
            this.lstSubtitle.DragEnter += new System.Windows.Forms.DragEventHandler(this.lstSubtitle_DragEnter);
            // 
            // charid
            // 
            this.charid.HeaderText = "キャラ";
            this.charid.MinimumWidth = 8;
            this.charid.Name = "charid";
            this.charid.Width = 83;
            // 
            // charaface
            // 
            this.charaface.HeaderText = "立ち絵";
            this.charaface.MinimumWidth = 8;
            this.charaface.Name = "charaface";
            this.charaface.Width = 83;
            // 
            // subtitle
            // 
            this.subtitle.HeaderText = "字幕";
            this.subtitle.MinimumWidth = 8;
            this.subtitle.Name = "subtitle";
            this.subtitle.Width = 67;
            // 
            // pronun
            // 
            this.pronun.HeaderText = "発声";
            this.pronun.MinimumWidth = 8;
            this.pronun.Name = "pronun";
            this.pronun.Width = 67;
            // 
            // speed
            // 
            this.speed.HeaderText = "速度";
            this.speed.MinimumWidth = 8;
            this.speed.Name = "speed";
            this.speed.Width = 67;
            // 
            // tone
            // 
            this.tone.HeaderText = "トーン";
            this.tone.MinimumWidth = 8;
            this.tone.Name = "tone";
            this.tone.Width = 83;
            // 
            // menAviutilStart
            // 
            this.menAviutilStart.Controls.Add(this.textBox1);
            this.menAviutilStart.Controls.Add(this.button4);
            this.menAviutilStart.Controls.Add(this.checkBox3);
            this.menAviutilStart.Controls.Add(this.button2);
            this.menAviutilStart.Controls.Add(this.checkBox2);
            this.menAviutilStart.Controls.Add(this.button1);
            this.menAviutilStart.Controls.Add(this.lstVoiceEffect);
            this.menAviutilStart.Controls.Add(this.pctFace);
            this.menAviutilStart.Controls.Add(this.trkImgsize);
            this.menAviutilStart.Controls.Add(this.lblTone);
            this.menAviutilStart.Controls.Add(this.lblVolume);
            this.menAviutilStart.Controls.Add(this.lblSpeed);
            this.menAviutilStart.Controls.Add(this.cmdAdd);
            this.menAviutilStart.Controls.Add(this.trkVolume);
            this.menAviutilStart.Controls.Add(this.label5);
            this.menAviutilStart.Controls.Add(this.label4);
            this.menAviutilStart.Controls.Add(this.label3);
            this.menAviutilStart.Controls.Add(this.trkTone);
            this.menAviutilStart.Controls.Add(this.trkSpeed);
            this.menAviutilStart.Controls.Add(this.txtPronun);
            this.menAviutilStart.Controls.Add(this.label2);
            this.menAviutilStart.Controls.Add(this.label1);
            this.menAviutilStart.Controls.Add(this.txtSubtitle);
            this.menAviutilStart.Controls.Add(this.txtCharID);
            this.menAviutilStart.Location = new System.Drawing.Point(758, 52);
            this.menAviutilStart.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.menAviutilStart.Name = "menAviutilStart";
            this.menAviutilStart.Padding = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.menAviutilStart.Size = new System.Drawing.Size(466, 623);
            this.menAviutilStart.TabIndex = 1;
            this.menAviutilStart.TabStop = false;
            this.menAviutilStart.Text = "セリフ";
            // 
            // textBox1
            // 
            this.textBox1.Font = new System.Drawing.Font("メイリオ", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.textBox1.Location = new System.Drawing.Point(12, 556);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(104, 27);
            this.textBox1.TabIndex = 25;
            // 
            // button4
            // 
            this.button4.Font = new System.Drawing.Font("メイリオ", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.button4.Location = new System.Drawing.Point(268, 294);
            this.button4.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(64, 27);
            this.button4.TabIndex = 24;
            this.button4.Text = "詳細";
            this.button4.UseVisualStyleBackColor = true;
            // 
            // checkBox3
            // 
            this.checkBox3.AutoSize = true;
            this.checkBox3.Location = new System.Drawing.Point(162, 296);
            this.checkBox3.Name = "checkBox3";
            this.checkBox3.Size = new System.Drawing.Size(109, 28);
            this.checkBox3.TabIndex = 23;
            this.checkBox3.Text = "ビブラート";
            this.checkBox3.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.Font = new System.Drawing.Font("メイリオ", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.button2.Location = new System.Drawing.Point(91, 294);
            this.button2.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(64, 27);
            this.button2.TabIndex = 22;
            this.button2.Text = "詳細";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // checkBox2
            // 
            this.checkBox2.AutoSize = true;
            this.checkBox2.Location = new System.Drawing.Point(12, 296);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(77, 28);
            this.checkBox2.TabIndex = 21;
            this.checkBox2.Text = "エコー";
            this.checkBox2.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("メイリオ", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.button1.Location = new System.Drawing.Point(11, 587);
            this.button1.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(105, 32);
            this.button1.TabIndex = 20;
            this.button1.Text = "Effect追加";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // lstVoiceEffect
            // 
            this.lstVoiceEffect.Font = new System.Drawing.Font("メイリオ", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lstVoiceEffect.FormattingEnabled = true;
            this.lstVoiceEffect.ItemHeight = 20;
            this.lstVoiceEffect.Items.AddRange(new object[] {
            "エコー",
            "0.5倍速"});
            this.lstVoiceEffect.Location = new System.Drawing.Point(12, 330);
            this.lstVoiceEffect.Name = "lstVoiceEffect";
            this.lstVoiceEffect.Size = new System.Drawing.Size(104, 224);
            this.lstVoiceEffect.TabIndex = 19;
            // 
            // pctFace
            // 
            this.pctFace.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pctFace.Location = new System.Drawing.Point(128, 330);
            this.pctFace.Name = "pctFace";
            this.pctFace.Size = new System.Drawing.Size(328, 259);
            this.pctFace.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pctFace.TabIndex = 18;
            this.pctFace.TabStop = false;
            // 
            // trkImgsize
            // 
            this.trkImgsize.LargeChange = 40;
            this.trkImgsize.Location = new System.Drawing.Point(128, 592);
            this.trkImgsize.Maximum = 240;
            this.trkImgsize.Name = "trkImgsize";
            this.trkImgsize.Size = new System.Drawing.Size(329, 18);
            this.trkImgsize.SmallChange = 10;
            this.trkImgsize.TabIndex = 16;
            this.trkImgsize.Value = 100;
            this.trkImgsize.Scroll += new System.Windows.Forms.ScrollEventHandler(this.trkImgsize_Scroll);
            // 
            // lblTone
            // 
            this.lblTone.AutoSize = true;
            this.lblTone.Location = new System.Drawing.Point(53, 225);
            this.lblTone.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblTone.Name = "lblTone";
            this.lblTone.Size = new System.Drawing.Size(40, 24);
            this.lblTone.TabIndex = 15;
            this.lblTone.Text = "100";
            // 
            // lblVolume
            // 
            this.lblVolume.AutoSize = true;
            this.lblVolume.Location = new System.Drawing.Point(53, 261);
            this.lblVolume.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblVolume.Name = "lblVolume";
            this.lblVolume.Size = new System.Drawing.Size(40, 24);
            this.lblVolume.TabIndex = 14;
            this.lblVolume.Text = "100";
            // 
            // lblSpeed
            // 
            this.lblSpeed.AutoSize = true;
            this.lblSpeed.Location = new System.Drawing.Point(53, 187);
            this.lblSpeed.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblSpeed.Name = "lblSpeed";
            this.lblSpeed.Size = new System.Drawing.Size(40, 24);
            this.lblSpeed.TabIndex = 13;
            this.lblSpeed.Text = "100";
            // 
            // cmdAdd
            // 
            this.cmdAdd.Location = new System.Drawing.Point(210, 26);
            this.cmdAdd.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.cmdAdd.Name = "cmdAdd";
            this.cmdAdd.Size = new System.Drawing.Size(107, 32);
            this.cmdAdd.TabIndex = 12;
            this.cmdAdd.Text = "追加(ctl+s)";
            this.cmdAdd.UseVisualStyleBackColor = true;
            this.cmdAdd.Click += new System.EventHandler(this.cmdAdd_Click);
            // 
            // trkVolume
            // 
            this.trkVolume.Location = new System.Drawing.Point(91, 261);
            this.trkVolume.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.trkVolume.Maximum = 100;
            this.trkVolume.Name = "trkVolume";
            this.trkVolume.Size = new System.Drawing.Size(373, 45);
            this.trkVolume.TabIndex = 11;
            this.trkVolume.TickFrequency = 5;
            this.trkVolume.Value = 100;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(7, 261);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(42, 24);
            this.label5.TabIndex = 10;
            this.label5.Text = "音量";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(7, 225);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(42, 24);
            this.label4.TabIndex = 9;
            this.label4.Text = "音程";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 187);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(42, 24);
            this.label3.TabIndex = 8;
            this.label3.Text = "速度";
            // 
            // trkTone
            // 
            this.trkTone.Location = new System.Drawing.Point(91, 225);
            this.trkTone.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.trkTone.Maximum = 200;
            this.trkTone.Name = "trkTone";
            this.trkTone.Size = new System.Drawing.Size(373, 45);
            this.trkTone.TabIndex = 7;
            this.trkTone.TickFrequency = 10;
            this.trkTone.Value = 100;
            this.trkTone.ValueChanged += new System.EventHandler(this.trkTone_ValueChanged);
            // 
            // trkSpeed
            // 
            this.trkSpeed.Location = new System.Drawing.Point(91, 187);
            this.trkSpeed.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.trkSpeed.Maximum = 200;
            this.trkSpeed.Name = "trkSpeed";
            this.trkSpeed.Size = new System.Drawing.Size(373, 45);
            this.trkSpeed.TabIndex = 6;
            this.trkSpeed.TickFrequency = 10;
            this.trkSpeed.Value = 100;
            this.trkSpeed.ValueChanged += new System.EventHandler(this.trkSpeed_ValueChanged);
            // 
            // txtPronun
            // 
            this.txtPronun.Location = new System.Drawing.Point(57, 121);
            this.txtPronun.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.txtPronun.Multiline = true;
            this.txtPronun.Name = "txtPronun";
            this.txtPronun.Size = new System.Drawing.Size(400, 54);
            this.txtPronun.TabIndex = 5;
            this.txtPronun.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtPronun_KeyDown);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 124);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(42, 24);
            this.label2.TabIndex = 4;
            this.label2.Text = "発音";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 68);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(42, 24);
            this.label1.TabIndex = 3;
            this.label1.Text = "字幕";
            // 
            // txtSubtitle
            // 
            this.txtSubtitle.Location = new System.Drawing.Point(58, 65);
            this.txtSubtitle.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.txtSubtitle.Multiline = true;
            this.txtSubtitle.Name = "txtSubtitle";
            this.txtSubtitle.Size = new System.Drawing.Size(400, 54);
            this.txtSubtitle.TabIndex = 2;
            this.txtSubtitle.Text = "今となっては\r\n何故投稿してしまったのか後悔してるのよー";
            this.txtSubtitle.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtSubtitle_KeyDown);
            this.txtSubtitle.Leave += new System.EventHandler(this.txtSubtitle_Leave);
            // 
            // txtCharID
            // 
            this.txtCharID.FormattingEnabled = true;
            this.txtCharID.Items.AddRange(new object[] {
            "れいむ",
            "まりさ"});
            this.txtCharID.Location = new System.Drawing.Point(8, 27);
            this.txtCharID.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.txtCharID.Name = "txtCharID";
            this.txtCharID.Size = new System.Drawing.Size(194, 32);
            this.txtCharID.TabIndex = 0;
            this.txtCharID.Text = "れいむ(ctl+r)";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.lstScene);
            this.groupBox3.Controls.Add(this.cmdSceneAdd);
            this.groupBox3.Controls.Add(this.txtScene);
            this.groupBox3.Location = new System.Drawing.Point(12, 29);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(149, 662);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "シーン";
            // 
            // lstScene
            // 
            this.lstScene.Font = new System.Drawing.Font("メイリオ", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lstScene.Location = new System.Drawing.Point(7, 23);
            this.lstScene.Name = "lstScene";
            treeNode1.Name = "node0";
            treeNode1.Text = "オープニング";
            treeNode2.Name = "node2";
            treeNode2.Text = "茶番.1";
            treeNode3.Name = "node1";
            treeNode3.Text = "茶番";
            treeNode4.Name = "node7";
            treeNode4.Text = "発電比較";
            treeNode5.Name = "node4";
            treeNode5.Text = "原子炉ボーナス";
            treeNode6.Name = "node5";
            treeNode6.Text = "水流量";
            treeNode7.Name = "node6";
            treeNode7.Text = "蒸気流量";
            treeNode8.Name = "node3";
            treeNode8.Text = "原子炉";
            this.lstScene.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode1,
            treeNode3,
            treeNode4,
            treeNode8});
            this.lstScene.Size = new System.Drawing.Size(136, 539);
            this.lstScene.TabIndex = 6;
            this.lstScene.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.lstScene_AfterSelect);
            // 
            // cmdSceneAdd
            // 
            this.cmdSceneAdd.Location = new System.Drawing.Point(7, 615);
            this.cmdSceneAdd.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.cmdSceneAdd.Name = "cmdSceneAdd";
            this.cmdSceneAdd.Size = new System.Drawing.Size(86, 31);
            this.cmdSceneAdd.TabIndex = 13;
            this.cmdSceneAdd.Text = "追加";
            this.cmdSceneAdd.UseVisualStyleBackColor = true;
            this.cmdSceneAdd.Click += new System.EventHandler(this.cmdSceneAdd_Click);
            // 
            // txtScene
            // 
            this.txtScene.FormattingEnabled = true;
            this.txtScene.Location = new System.Drawing.Point(7, 571);
            this.txtScene.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.txtScene.Name = "txtScene";
            this.txtScene.Size = new System.Drawing.Size(136, 32);
            this.txtScene.TabIndex = 1;
            // 
            // menMain
            // 
            this.menMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menFile,
            this.menSetting,
            this.menWindow,
            this.menVoice,
            this.動画管理ToolStripMenuItem});
            this.menMain.Location = new System.Drawing.Point(0, 0);
            this.menMain.Name = "menMain";
            this.menMain.Size = new System.Drawing.Size(1234, 26);
            this.menMain.TabIndex = 3;
            this.menMain.Text = "menuStrip1";
            // 
            // menFile
            // 
            this.menFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menOpen,
            this.menSave,
            this.toolStripMenuItem1,
            this.menNewAup});
            this.menFile.Name = "menFile";
            this.menFile.Size = new System.Drawing.Size(85, 22);
            this.menFile.Text = "ファイル(&F)";
            // 
            // menOpen
            // 
            this.menOpen.Name = "menOpen";
            this.menOpen.Size = new System.Drawing.Size(121, 22);
            this.menOpen.Text = "開く(&O)";
            this.menOpen.Click += new System.EventHandler(this.menOpen_Click);
            // 
            // menSave
            // 
            this.menSave.Name = "menSave";
            this.menSave.Size = new System.Drawing.Size(121, 22);
            this.menSave.Text = "保存";
            this.menSave.Click += new System.EventHandler(this.menSave_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(118, 6);
            // 
            // menNewAup
            // 
            this.menNewAup.Name = "menNewAup";
            this.menNewAup.Size = new System.Drawing.Size(121, 22);
            this.menNewAup.Text = "新規aup";
            // 
            // menSetting
            // 
            this.menSetting.Name = "menSetting";
            this.menSetting.Size = new System.Drawing.Size(44, 22);
            this.menSetting.Text = "設定";
            // 
            // menWindow
            // 
            this.menWindow.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menCharaselect});
            this.menWindow.Name = "menWindow";
            this.menWindow.Size = new System.Drawing.Size(44, 22);
            this.menWindow.Text = "表示";
            // 
            // menCharaselect
            // 
            this.menCharaselect.Name = "menCharaselect";
            this.menCharaselect.Size = new System.Drawing.Size(136, 22);
            this.menCharaselect.Text = "立ち絵選択";
            // 
            // menVoice
            // 
            this.menVoice.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menVoicePlay,
            this.menVoiceExtract});
            this.menVoice.Name = "menVoice";
            this.menVoice.Size = new System.Drawing.Size(50, 22);
            this.menVoice.Text = "Voice";
            // 
            // menVoicePlay
            // 
            this.menVoicePlay.Name = "menVoicePlay";
            this.menVoicePlay.Size = new System.Drawing.Size(145, 22);
            this.menVoicePlay.Text = "読み上げ";
            // 
            // menVoiceExtract
            // 
            this.menVoiceExtract.Name = "menVoiceExtract";
            this.menVoiceExtract.Size = new System.Drawing.Size(145, 22);
            this.menVoiceExtract.Text = "Aviutlへ展開";
            // 
            // 動画管理ToolStripMenuItem
            // 
            this.動画管理ToolStripMenuItem.Name = "動画管理ToolStripMenuItem";
            this.動画管理ToolStripMenuItem.Size = new System.Drawing.Size(68, 22);
            this.動画管理ToolStripMenuItem.Text = "動画管理";
            this.動画管理ToolStripMenuItem.ToolTipText = "作成した動画の管理+予測時間の計算に使用する";
            // 
            // dlgOpenFile
            // 
            this.dlgOpenFile.FileName = "openFileDialog1";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.button3);
            this.groupBox4.Controls.Add(this.checkBox1);
            this.groupBox4.Location = new System.Drawing.Point(758, 684);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(464, 66);
            this.groupBox4.TabIndex = 6;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "展開";
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(351, 21);
            this.button3.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(107, 32);
            this.button3.TabIndex = 16;
            this.button3.Text = "統一obj展開";
            this.button3.UseVisualStyleBackColor = true;
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(8, 30);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(157, 28);
            this.checkBox1.TabIndex = 15;
            this.checkBox1.Text = "統一立ち絵を使用";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.lblCharadrawMode,
            this.separator1,
            this.lblStatus,
            this.separator2,
            this.lblMessage,
            this.toolStripStatusLabel2,
            this.toolStripProgressBar1,
            this.toolStripStatusLabel3,
            this.toolStripProgressBar2});
            this.statusStrip1.Location = new System.Drawing.Point(0, 754);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1234, 23);
            this.statusStrip1.TabIndex = 7;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(85, 18);
            this.toolStripStatusLabel1.Text = "立ち絵モード:";
            // 
            // lblCharadrawMode
            // 
            this.lblCharadrawMode.Name = "lblCharadrawMode";
            this.lblCharadrawMode.Size = new System.Drawing.Size(56, 18);
            this.lblCharadrawMode.Text = "動画全体";
            // 
            // separator1
            // 
            this.separator1.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
            this.separator1.BorderStyle = System.Windows.Forms.Border3DStyle.Etched;
            this.separator1.Name = "separator1";
            this.separator1.Size = new System.Drawing.Size(4, 18);
            // 
            // lblStatus
            // 
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(134, 18);
            this.lblStatus.Text = "toolStripStatusLabel1";
            // 
            // separator2
            // 
            this.separator2.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
            this.separator2.BorderStyle = System.Windows.Forms.Border3DStyle.Etched;
            this.separator2.Name = "separator2";
            this.separator2.Size = new System.Drawing.Size(4, 18);
            // 
            // lblMessage
            // 
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Padding = new System.Windows.Forms.Padding(500, 0, 0, 0);
            this.lblMessage.Size = new System.Drawing.Size(560, 18);
            this.lblMessage.Text = "message";
            this.lblMessage.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbOpen,
            this.tsbSave,
            this.toolStripSeparator1,
            this.cmdAviutlStartup,
            this.cmdReadoutScene,
            this.cmdExtract});
            this.toolStrip1.Location = new System.Drawing.Point(0, 26);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(1234, 25);
            this.toolStrip1.TabIndex = 8;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // tsbOpen
            // 
            this.tsbOpen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbOpen.Image = ((System.Drawing.Image)(resources.GetObject("tsbOpen.Image")));
            this.tsbOpen.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbOpen.Name = "tsbOpen";
            this.tsbOpen.Size = new System.Drawing.Size(23, 22);
            this.tsbOpen.Text = "toolStripButton1";
            this.tsbOpen.Click += new System.EventHandler(this.tsbOpen_Click);
            // 
            // tsbSave
            // 
            this.tsbSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbSave.Image = ((System.Drawing.Image)(resources.GetObject("tsbSave.Image")));
            this.tsbSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbSave.Name = "tsbSave";
            this.tsbSave.Size = new System.Drawing.Size(23, 22);
            this.tsbSave.Text = "toolStripButton2";
            this.tsbSave.Click += new System.EventHandler(this.tsbSave_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // cmdAviutlStartup
            // 
            this.cmdAviutlStartup.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.cmdAviutlStartup.Image = ((System.Drawing.Image)(resources.GetObject("cmdAviutlStartup.Image")));
            this.cmdAviutlStartup.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.cmdAviutlStartup.Name = "cmdAviutlStartup";
            this.cmdAviutlStartup.Size = new System.Drawing.Size(23, 22);
            this.cmdAviutlStartup.Text = "Aviutl起動";
            this.cmdAviutlStartup.Click += new System.EventHandler(this.cmdAviutlStartup_Click);
            // 
            // cmdReadoutScene
            // 
            this.cmdReadoutScene.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.cmdReadoutScene.Image = ((System.Drawing.Image)(resources.GetObject("cmdReadoutScene.Image")));
            this.cmdReadoutScene.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.cmdReadoutScene.Name = "cmdReadoutScene";
            this.cmdReadoutScene.Size = new System.Drawing.Size(23, 22);
            this.cmdReadoutScene.Text = "セリフの読み上げ";
            this.cmdReadoutScene.ToolTipText = "セリフの読み上げ";
            // 
            // cmdExtract
            // 
            this.cmdExtract.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.cmdExtract.Image = ((System.Drawing.Image)(resources.GetObject("cmdExtract.Image")));
            this.cmdExtract.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.cmdExtract.Name = "cmdExtract";
            this.cmdExtract.Size = new System.Drawing.Size(23, 22);
            this.cmdExtract.Text = "Aviutlへ展開";
            this.cmdExtract.Click += new System.EventHandler(this.cmdExtract_Click);
            // 
            // lblDatagridviewFont
            // 
            this.lblDatagridviewFont.AutoSize = true;
            this.lblDatagridviewFont.Location = new System.Drawing.Point(15, 715);
            this.lblDatagridviewFont.Name = "lblDatagridviewFont";
            this.lblDatagridviewFont.Size = new System.Drawing.Size(273, 24);
            this.lblDatagridviewFont.TabIndex = 9;
            this.lblDatagridviewFont.Text = "gridview_Columnheadeのフォント";
            // 
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            this.toolStripStatusLabel2.Size = new System.Drawing.Size(31, 18);
            this.toolStripStatusLabel2.Text = "disk";
            // 
            // toolStripProgressBar1
            // 
            this.toolStripProgressBar1.Name = "toolStripProgressBar1";
            this.toolStripProgressBar1.Size = new System.Drawing.Size(100, 17);
            // 
            // toolStripStatusLabel3
            // 
            this.toolStripStatusLabel3.Name = "toolStripStatusLabel3";
            this.toolStripStatusLabel3.Size = new System.Drawing.Size(58, 18);
            this.toolStripStatusLabel3.Text = "memory";
            // 
            // toolStripProgressBar2
            // 
            this.toolStripProgressBar2.Name = "toolStripProgressBar2";
            this.toolStripProgressBar2.Size = new System.Drawing.Size(100, 17);
            // 
            // frmSenario
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1234, 777);
            this.Controls.Add(this.lblDatagridviewFont);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.menAviutilStart);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.menMain);
            this.Font = new System.Drawing.Font("メイリオ", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.KeyPreview = true;
            this.MainMenuStrip = this.menMain;
            this.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.Name = "frmSenario";
            this.Text = "シナリオ作成";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmSenario_FormClosing);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmSenario_KeyDown);
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.lstSubtitle)).EndInit();
            this.menAviutilStart.ResumeLayout(false);
            this.menAviutilStart.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pctFace)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trkVolume)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trkTone)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trkSpeed)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.menMain.ResumeLayout(false);
            this.menMain.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.DataGridView lstSubtitle;
        private System.Windows.Forms.GroupBox menAviutilStart;
        private System.Windows.Forms.ComboBox txtCharID;
        private System.Windows.Forms.TextBox txtPronun;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtSubtitle;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TrackBar trkTone;
        private System.Windows.Forms.TrackBar trkSpeed;
        private System.Windows.Forms.TrackBar trkVolume;
        private System.Windows.Forms.Button cmdAdd;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.ComboBox txtScene;
        private System.Windows.Forms.Button cmdSceneAdd;
        private System.Windows.Forms.MenuStrip menMain;
        private System.Windows.Forms.ToolStripMenuItem menFile;
        private System.Windows.Forms.ToolStripMenuItem menOpen;
        private System.Windows.Forms.OpenFileDialog dlgOpenFile;
        private System.Windows.Forms.Label lblTone;
        private System.Windows.Forms.Label lblVolume;
        private System.Windows.Forms.Label lblSpeed;
        private System.Windows.Forms.HScrollBar trkImgsize;
        private System.Windows.Forms.TreeView lstScene;
        private System.Windows.Forms.ToolStripMenuItem menSetting;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.ToolStripMenuItem menWindow;
        private System.Windows.Forms.ToolStripMenuItem menCharaselect;
        private System.Windows.Forms.PictureBox pctFace;
        private System.Windows.Forms.ToolStripMenuItem menSave;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel lblStatus;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton cmdReadoutScene;
        private System.Windows.Forms.ToolStripButton cmdExtract;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem menNewAup;
        private System.Windows.Forms.ToolStripButton tsbOpen;
        private System.Windows.Forms.ToolStripButton tsbSave;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripStatusLabel separator1;
        private System.Windows.Forms.ToolStripStatusLabel lblMessage;
        private System.Windows.Forms.ListBox lstVoiceEffect;
        private System.Windows.Forms.DataGridViewTextBoxColumn charid;
        private System.Windows.Forms.DataGridViewTextBoxColumn charaface;
        private System.Windows.Forms.DataGridViewTextBoxColumn subtitle;
        private System.Windows.Forms.DataGridViewTextBoxColumn pronun;
        private System.Windows.Forms.DataGridViewTextBoxColumn speed;
        private System.Windows.Forms.DataGridViewTextBoxColumn tone;
    private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
    private System.Windows.Forms.ToolStripStatusLabel lblCharadrawMode;
    private System.Windows.Forms.ToolStripStatusLabel separator2;
    private System.Windows.Forms.Button button1;
    private System.Windows.Forms.ToolStripMenuItem menVoice;
    private System.Windows.Forms.ToolStripMenuItem menVoicePlay;
    private System.Windows.Forms.ToolStripMenuItem menVoiceExtract;
    private System.Windows.Forms.CheckBox checkBox3;
    private System.Windows.Forms.CheckBox checkBox2;
    private System.Windows.Forms.Button button4;
    private System.Windows.Forms.Button button2;
    private System.Windows.Forms.TextBox textBox1;
    private System.Windows.Forms.Label lblDatagridviewFont;
    private System.Windows.Forms.ToolStripButton cmdAviutlStartup;
    private System.Windows.Forms.ToolStripMenuItem 動画管理ToolStripMenuItem;
    private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
    private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar1;
    private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel3;
    private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar2;
  }
}

