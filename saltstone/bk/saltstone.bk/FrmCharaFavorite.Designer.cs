
namespace saltstone
{
    partial class lstLip
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("ほっぺ赤み");
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("はずかしい＿中");
            System.Windows.Forms.TreeNode treeNode3 = new System.Windows.Forms.TreeNode("はずかしい＿大");
            System.Windows.Forms.TreeNode treeNode4 = new System.Windows.Forms.TreeNode("赤み顔", new System.Windows.Forms.TreeNode[] {
            treeNode1,
            treeNode2,
            treeNode3});
            System.Windows.Forms.TreeNode treeNode5 = new System.Windows.Forms.TreeNode("青顔");
            System.Windows.Forms.TreeNode treeNode6 = new System.Windows.Forms.TreeNode("黒顔");
            this.imgsizebar = new System.Windows.Forms.TrackBar();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lstFavorite = new System.Windows.Forms.ListBox();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label6 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.statsbar = new System.Windows.Forms.StatusStrip();
            this.StatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.lstFace = new System.Windows.Forms.ListBox();
            this.lstEye = new System.Windows.Forms.ListBox();
            this.LstMayu = new System.Windows.Forms.ListBox();
            this.flowimglist = new System.Windows.Forms.FlowLayoutPanel();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.lstChara = new System.Windows.Forms.ListBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.groupbox5 = new System.Windows.Forms.GroupBox();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)(this.imgsizebar)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.statsbar.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupbox5.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.SuspendLayout();
            // 
            // imgsizebar
            // 
            this.imgsizebar.LargeChange = 50;
            this.imgsizebar.Location = new System.Drawing.Point(223, 791);
            this.imgsizebar.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.imgsizebar.Maximum = 400;
            this.imgsizebar.Minimum = 50;
            this.imgsizebar.Name = "imgsizebar";
            this.imgsizebar.Size = new System.Drawing.Size(685, 45);
            this.imgsizebar.SmallChange = 10;
            this.imgsizebar.TabIndex = 1;
            this.imgsizebar.TickFrequency = 10;
            this.imgsizebar.Value = 50;
            this.imgsizebar.Scroll += new System.EventHandler(this.imgsizebar_Scroll);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lstFavorite);
            this.groupBox1.Location = new System.Drawing.Point(12, 436);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.groupBox1.Size = new System.Drawing.Size(207, 377);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "お気に入り";
            // 
            // lstFavorite
            // 
            this.lstFavorite.FormattingEnabled = true;
            this.lstFavorite.ItemHeight = 24;
            this.lstFavorite.Items.AddRange(new object[] {
            "デフォルト",
            "くるくる目",
            "困り顔"});
            this.lstFavorite.Location = new System.Drawing.Point(10, 25);
            this.lstFavorite.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.lstFavorite.Name = "lstFavorite";
            this.lstFavorite.Size = new System.Drawing.Size(185, 340);
            this.lstFavorite.TabIndex = 0;
            this.lstFavorite.Click += new System.EventHandler(this.lstFavorite_Click);
            // 
            // treeView1
            // 
            this.treeView1.Location = new System.Drawing.Point(12, 30);
            this.treeView1.Name = "treeView1";
            treeNode1.Name = "ノード3";
            treeNode1.Text = "ほっぺ赤み";
            treeNode2.Name = "ノード4";
            treeNode2.Text = "はずかしい＿中";
            treeNode3.Name = "ノード5";
            treeNode3.Text = "はずかしい＿大";
            treeNode4.Name = "ノード0";
            treeNode4.Text = "赤み顔";
            treeNode5.Name = "ノード1";
            treeNode5.Text = "青顔";
            treeNode6.Name = "ノード2";
            treeNode6.Text = "黒顔";
            this.treeView1.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode4,
            treeNode5,
            treeNode6});
            this.treeView1.Size = new System.Drawing.Size(187, 206);
            this.treeView1.TabIndex = 10;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Controls.Add(this.button2);
            this.groupBox3.Controls.Add(this.treeView1);
            this.groupBox3.Location = new System.Drawing.Point(12, 136);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(209, 291);
            this.groupBox3.TabIndex = 11;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "分類";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(55, 253);
            this.label6.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(186, 24);
            this.label6.TabIndex = 19;
            this.label6.Text = "追加は別画面を表示する";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(12, 243);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(57, 34);
            this.button2.TabIndex = 14;
            this.button2.Text = "追加";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(219, 826);
            this.label3.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(382, 24);
            this.label3.TabIndex = 13;
            this.label3.Text = "パーツ選択をどうするか？ -> frmcharaで設定する";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(623, 826);
            this.label2.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(186, 24);
            this.label2.TabIndex = 14;
            this.label2.Text = "画像のダブルクリック時";
            // 
            // statsbar
            // 
            this.statsbar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.StatusLabel});
            this.statsbar.Location = new System.Drawing.Point(0, 828);
            this.statsbar.Name = "statsbar";
            this.statsbar.Size = new System.Drawing.Size(1158, 23);
            this.statsbar.TabIndex = 15;
            // 
            // StatusLabel
            // 
            this.StatusLabel.Name = "StatusLabel";
            this.StatusLabel.Size = new System.Drawing.Size(43, 18);
            this.StatusLabel.Text = "aaaaa";
            // 
            // lstFace
            // 
            this.lstFace.FormattingEnabled = true;
            this.lstFace.ItemHeight = 24;
            this.lstFace.Items.AddRange(new object[] {
            "普通",
            "赤み顔",
            "大赤み顔",
            "青顔",
            "黒顔"});
            this.lstFace.Location = new System.Drawing.Point(6, 22);
            this.lstFace.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.lstFace.Name = "lstFace";
            this.lstFace.Size = new System.Drawing.Size(177, 124);
            this.lstFace.TabIndex = 1;
            this.lstFace.SelectedIndexChanged += new System.EventHandler(this.lstFace_SelectedIndexChanged);
            // 
            // lstEye
            // 
            this.lstEye.FormattingEnabled = true;
            this.lstEye.ItemHeight = 24;
            this.lstEye.Items.AddRange(new object[] {
            "通常",
            "半とじ目",
            "いやな感じの目",
            "白目点",
            "黒目点",
            "上方向目"});
            this.lstEye.Location = new System.Drawing.Point(8, 20);
            this.lstEye.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.lstEye.Name = "lstEye";
            this.lstEye.Size = new System.Drawing.Size(175, 220);
            this.lstEye.TabIndex = 16;
            this.lstEye.SelectedIndexChanged += new System.EventHandler(this.lstEye_SelectedIndexChanged);
            // 
            // LstMayu
            // 
            this.LstMayu.FormattingEnabled = true;
            this.LstMayu.ItemHeight = 24;
            this.LstMayu.Items.AddRange(new object[] {
            "つり眉",
            "たれ眉"});
            this.LstMayu.Location = new System.Drawing.Point(8, 33);
            this.LstMayu.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.LstMayu.Name = "LstMayu";
            this.LstMayu.Size = new System.Drawing.Size(175, 124);
            this.LstMayu.TabIndex = 17;
            // 
            // flowimglist
            // 
            this.flowimglist.AutoScroll = true;
            this.flowimglist.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.flowimglist.Location = new System.Drawing.Point(223, 14);
            this.flowimglist.Name = "flowimglist";
            this.flowimglist.Size = new System.Drawing.Size(685, 778);
            this.flowimglist.TabIndex = 19;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.lstChara);
            this.groupBox2.Location = new System.Drawing.Point(12, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(171, 118);
            this.groupBox2.TabIndex = 20;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "キャラ";
            // 
            // lstChara
            // 
            this.lstChara.FormattingEnabled = true;
            this.lstChara.ItemHeight = 24;
            this.lstChara.Location = new System.Drawing.Point(6, 20);
            this.lstChara.Name = "lstChara";
            this.lstChara.Size = new System.Drawing.Size(159, 76);
            this.lstChara.TabIndex = 1;
            this.lstChara.SelectedIndexChanged += new System.EventHandler(this.lstChara_SelectedIndexChanged);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.lstFace);
            this.groupBox4.Location = new System.Drawing.Point(914, 2);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(191, 156);
            this.groupBox4.TabIndex = 21;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "顔";
            // 
            // groupbox5
            // 
            this.groupbox5.Controls.Add(this.lstEye);
            this.groupbox5.Location = new System.Drawing.Point(914, 155);
            this.groupbox5.Name = "groupbox5";
            this.groupbox5.Size = new System.Drawing.Size(191, 251);
            this.groupbox5.TabIndex = 22;
            this.groupbox5.TabStop = false;
            this.groupbox5.Text = "目";
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.listBox1);
            this.groupBox6.Location = new System.Drawing.Point(914, 405);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(191, 180);
            this.groupBox6.TabIndex = 23;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "口";
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.ItemHeight = 24;
            this.listBox1.Items.AddRange(new object[] {
            "つり眉",
            "たれ眉"});
            this.listBox1.Location = new System.Drawing.Point(8, 22);
            this.listBox1.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(175, 148);
            this.listBox1.TabIndex = 18;
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.LstMayu);
            this.groupBox7.Location = new System.Drawing.Point(914, 583);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(191, 171);
            this.groupBox7.TabIndex = 24;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "眉";
            // 
            // lstLip
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1158, 851);
            this.Controls.Add(this.groupBox7);
            this.Controls.Add(this.groupBox6);
            this.Controls.Add(this.groupbox5);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.flowimglist);
            this.Controls.Add(this.statsbar);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.imgsizebar);
            this.Controls.Add(this.groupBox3);
            this.Font = new System.Drawing.Font("メイリオ", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.Name = "lstLip";
            this.Text = "キャラ素材リスト";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmCharaFavorite_FormClosing);
            this.Load += new System.EventHandler(this.frmCharaFavorite_Load);
            ((System.ComponentModel.ISupportInitialize)(this.imgsizebar)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.statsbar.ResumeLayout(false);
            this.statsbar.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupbox5.ResumeLayout(false);
            this.groupBox6.ResumeLayout(false);
            this.groupBox7.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TrackBar imgsizebar;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ListBox lstFavorite;
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.StatusStrip statsbar;
        private System.Windows.Forms.ListBox lstFace;
        private System.Windows.Forms.ListBox lstEye;
        private System.Windows.Forms.ListBox LstMayu;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.FlowLayoutPanel flowimglist;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ListBox lstChara;
        private System.Windows.Forms.ToolStripStatusLabel StatusLabel;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.GroupBox groupbox5;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.GroupBox groupBox7;
    }
}