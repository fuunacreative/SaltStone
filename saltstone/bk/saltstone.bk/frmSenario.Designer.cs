
namespace saltstone
{
    partial class frmSenario
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lstSubtitle = new System.Windows.Forms.DataGridView();
            this.charid = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.charaface = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.subtitle = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pronun = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.speed = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tone = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.excmd = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.trkImgsize = new System.Windows.Forms.HScrollBar();
            this.label6 = new System.Windows.Forms.Label();
            this.lblTone = new System.Windows.Forms.Label();
            this.lblVolume = new System.Windows.Forms.Label();
            this.lblSpeed = new System.Windows.Forms.Label();
            this.cmdAdd = new System.Windows.Forms.Button();
            this.flowimglist = new System.Windows.Forms.FlowLayoutPanel();
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
            this.cmdSceneAdd = new System.Windows.Forms.Button();
            this.lstScene = new System.Windows.Forms.ListBox();
            this.txtScene = new System.Windows.Forms.ComboBox();
            this.menMain = new System.Windows.Forms.MenuStrip();
            this.ファイルFToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.dlgOpenFile = new System.Windows.Forms.OpenFileDialog();
            this.lblDatagridviewFont = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.lstSubtitle)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trkVolume)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trkTone)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trkSpeed)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.menMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
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
            this.lstSubtitle.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCells;
            this.lstSubtitle.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCells;
            this.lstSubtitle.ColumnHeadersHeight = 26;
            this.lstSubtitle.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.charid,
            this.charaface,
            this.subtitle,
            this.pronun,
            this.speed,
            this.tone,
            this.excmd});
            this.lstSubtitle.Location = new System.Drawing.Point(8, 27);
            this.lstSubtitle.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.lstSubtitle.Name = "lstSubtitle";
            this.lstSubtitle.RowHeadersWidth = 24;
            dataGridViewCellStyle7.Font = new System.Drawing.Font("メイリオ", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lstSubtitle.RowsDefaultCellStyle = dataGridViewCellStyle7;
            this.lstSubtitle.RowTemplate.Height = 27;
            this.lstSubtitle.Size = new System.Drawing.Size(564, 690);
            this.lstSubtitle.TabIndex = 0;
            this.lstSubtitle.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.lstSubtitle_RowEnter);
            this.lstSubtitle.RowsAdded += new System.Windows.Forms.DataGridViewRowsAddedEventHandler(this.lstSubtitle_RowsAdded);
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
            // excmd
            // 
            this.excmd.HeaderText = "素材";
            this.excmd.Name = "excmd";
            this.excmd.Width = 67;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.trkImgsize);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.lblTone);
            this.groupBox2.Controls.Add(this.lblVolume);
            this.groupBox2.Controls.Add(this.lblSpeed);
            this.groupBox2.Controls.Add(this.cmdAdd);
            this.groupBox2.Controls.Add(this.flowimglist);
            this.groupBox2.Controls.Add(this.trkVolume);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.trkTone);
            this.groupBox2.Controls.Add(this.trkSpeed);
            this.groupBox2.Controls.Add(this.txtPronun);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.txtSubtitle);
            this.groupBox2.Controls.Add(this.txtCharID);
            this.groupBox2.Location = new System.Drawing.Point(758, 32);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.groupBox2.Size = new System.Drawing.Size(669, 720);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "セリフ";
            // 
            // trkImgsize
            // 
            this.trkImgsize.LargeChange = 40;
            this.trkImgsize.Location = new System.Drawing.Point(40, 696);
            this.trkImgsize.Maximum = 240;
            this.trkImgsize.Name = "trkImgsize";
            this.trkImgsize.Size = new System.Drawing.Size(618, 17);
            this.trkImgsize.SmallChange = 10;
            this.trkImgsize.TabIndex = 16;
            this.trkImgsize.Value = 100;
            this.trkImgsize.Scroll += new System.Windows.Forms.ScrollEventHandler(this.trkImgsize_Scroll);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("メイリオ", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label6.Location = new System.Drawing.Point(8, 696);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(28, 17);
            this.label6.TabIndex = 17;
            this.label6.Text = "size";
            // 
            // lblTone
            // 
            this.lblTone.AutoSize = true;
            this.lblTone.Location = new System.Drawing.Point(54, 189);
            this.lblTone.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblTone.Name = "lblTone";
            this.lblTone.Size = new System.Drawing.Size(40, 24);
            this.lblTone.TabIndex = 15;
            this.lblTone.Text = "100";
            // 
            // lblVolume
            // 
            this.lblVolume.AutoSize = true;
            this.lblVolume.Location = new System.Drawing.Point(54, 225);
            this.lblVolume.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblVolume.Name = "lblVolume";
            this.lblVolume.Size = new System.Drawing.Size(40, 24);
            this.lblVolume.TabIndex = 14;
            this.lblVolume.Text = "100";
            // 
            // lblSpeed
            // 
            this.lblSpeed.AutoSize = true;
            this.lblSpeed.Location = new System.Drawing.Point(54, 151);
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
            this.cmdAdd.Text = "追加";
            this.cmdAdd.UseVisualStyleBackColor = true;
            this.cmdAdd.Click += new System.EventHandler(this.cmdAdd_Click);
            // 
            // flowimglist
            // 
            this.flowimglist.AutoScroll = true;
            this.flowimglist.Location = new System.Drawing.Point(12, 259);
            this.flowimglist.Margin = new System.Windows.Forms.Padding(4, 10, 4, 6);
            this.flowimglist.Name = "flowimglist";
            this.flowimglist.Size = new System.Drawing.Size(646, 435);
            this.flowimglist.TabIndex = 1;
            // 
            // trkVolume
            // 
            this.trkVolume.Location = new System.Drawing.Point(92, 225);
            this.trkVolume.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.trkVolume.Maximum = 100;
            this.trkVolume.Name = "trkVolume";
            this.trkVolume.Size = new System.Drawing.Size(566, 45);
            this.trkVolume.TabIndex = 11;
            this.trkVolume.TickFrequency = 5;
            this.trkVolume.Value = 100;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(8, 225);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(42, 24);
            this.label5.TabIndex = 10;
            this.label5.Text = "音量";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(8, 189);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(42, 24);
            this.label4.TabIndex = 9;
            this.label4.Text = "音程";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(8, 151);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(42, 24);
            this.label3.TabIndex = 8;
            this.label3.Text = "速度";
            // 
            // trkTone
            // 
            this.trkTone.Location = new System.Drawing.Point(92, 189);
            this.trkTone.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.trkTone.Maximum = 200;
            this.trkTone.Name = "trkTone";
            this.trkTone.Size = new System.Drawing.Size(566, 45);
            this.trkTone.TabIndex = 7;
            this.trkTone.TickFrequency = 10;
            this.trkTone.Value = 100;
            this.trkTone.ValueChanged += new System.EventHandler(this.trkTone_ValueChanged);
            // 
            // trkSpeed
            // 
            this.trkSpeed.Location = new System.Drawing.Point(92, 151);
            this.trkSpeed.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.trkSpeed.Maximum = 200;
            this.trkSpeed.Name = "trkSpeed";
            this.trkSpeed.Size = new System.Drawing.Size(566, 45);
            this.trkSpeed.TabIndex = 6;
            this.trkSpeed.TickFrequency = 10;
            this.trkSpeed.Value = 100;
            this.trkSpeed.ValueChanged += new System.EventHandler(this.trkSpeed_ValueChanged);
            // 
            // txtPronun
            // 
            this.txtPronun.Location = new System.Drawing.Point(58, 108);
            this.txtPronun.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.txtPronun.Name = "txtPronun";
            this.txtPronun.Size = new System.Drawing.Size(600, 31);
            this.txtPronun.TabIndex = 5;
            this.txtPronun.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtPronun_KeyDown);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 111);
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
            this.txtSubtitle.Name = "txtSubtitle";
            this.txtSubtitle.Size = new System.Drawing.Size(600, 31);
            this.txtSubtitle.TabIndex = 2;
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
            this.groupBox3.Controls.Add(this.cmdSceneAdd);
            this.groupBox3.Controls.Add(this.lstScene);
            this.groupBox3.Controls.Add(this.txtScene);
            this.groupBox3.Location = new System.Drawing.Point(12, 29);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(149, 662);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "シーン";
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
            // lstScene
            // 
            this.lstScene.FormattingEnabled = true;
            this.lstScene.ItemHeight = 24;
            this.lstScene.Items.AddRange(new object[] {
            "オープニング",
            "茶番１",
            "原子炉解説１",
            "原子炉解説２"});
            this.lstScene.Location = new System.Drawing.Point(6, 30);
            this.lstScene.Name = "lstScene";
            this.lstScene.Size = new System.Drawing.Size(137, 532);
            this.lstScene.TabIndex = 2;
            this.lstScene.SelectedIndexChanged += new System.EventHandler(this.lstScene_SelectedIndexChanged);
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
            this.ファイルFToolStripMenuItem});
            this.menMain.Location = new System.Drawing.Point(0, 0);
            this.menMain.Name = "menMain";
            this.menMain.Size = new System.Drawing.Size(1464, 26);
            this.menMain.TabIndex = 3;
            this.menMain.Text = "menuStrip1";
            // 
            // ファイルFToolStripMenuItem
            // 
            this.ファイルFToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menOpen});
            this.ファイルFToolStripMenuItem.Name = "ファイルFToolStripMenuItem";
            this.ファイルFToolStripMenuItem.Size = new System.Drawing.Size(85, 22);
            this.ファイルFToolStripMenuItem.Text = "ファイル(&F)";
            // 
            // menOpen
            // 
            this.menOpen.Name = "menOpen";
            this.menOpen.Size = new System.Drawing.Size(119, 22);
            this.menOpen.Text = "開く(&O)";
            this.menOpen.Click += new System.EventHandler(this.menOpen_Click);
            // 
            // dlgOpenFile
            // 
            this.dlgOpenFile.FileName = "openFileDialog1";
            // 
            // lblDatagridviewFont
            // 
            this.lblDatagridviewFont.AutoSize = true;
            this.lblDatagridviewFont.Font = new System.Drawing.Font("メイリオ", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblDatagridviewFont.Location = new System.Drawing.Point(56, 713);
            this.lblDatagridviewFont.Name = "lblDatagridviewFont";
            this.lblDatagridviewFont.Size = new System.Drawing.Size(99, 17);
            this.lblDatagridviewFont.TabIndex = 4;
            this.lblDatagridviewFont.Text = "datagridviewfont";
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.SystemColors.Control;
            this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox1.Location = new System.Drawing.Point(12, 697);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(93, 79);
            this.pictureBox1.TabIndex = 5;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.WaitOnLoad = true;
            // 
            // frmSenario
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1464, 762);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.lblDatagridviewFont);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
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
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trkVolume)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trkTone)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trkSpeed)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.menMain.ResumeLayout(false);
            this.menMain.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.DataGridView lstSubtitle;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.FlowLayoutPanel flowimglist;
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
        private System.Windows.Forms.ListBox lstScene;
        private System.Windows.Forms.ComboBox txtScene;
        private System.Windows.Forms.Button cmdSceneAdd;
        private System.Windows.Forms.MenuStrip menMain;
        private System.Windows.Forms.ToolStripMenuItem ファイルFToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem menOpen;
        private System.Windows.Forms.OpenFileDialog dlgOpenFile;
        private System.Windows.Forms.DataGridViewTextBoxColumn charid;
        private System.Windows.Forms.DataGridViewTextBoxColumn charaface;
        private System.Windows.Forms.DataGridViewTextBoxColumn subtitle;
        private System.Windows.Forms.DataGridViewTextBoxColumn pronun;
        private System.Windows.Forms.DataGridViewTextBoxColumn speed;
        private System.Windows.Forms.DataGridViewTextBoxColumn tone;
        private System.Windows.Forms.DataGridViewTextBoxColumn excmd;
        private System.Windows.Forms.Label lblDatagridviewFont;
        private System.Windows.Forms.Label lblTone;
        private System.Windows.Forms.Label lblVolume;
        private System.Windows.Forms.Label lblSpeed;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.HScrollBar trkImgsize;
        private System.Windows.Forms.Label label6;
    }
}

