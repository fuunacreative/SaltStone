
namespace SaltstoneFace
{
  partial class frmCharalist
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmCharalist));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.stbStatuslabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.stpProgressbar = new System.Windows.Forms.ToolStripProgressBar();
            this.panel1 = new System.Windows.Forms.Panel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.txtCharbasedir = new System.Windows.Forms.TextBox();
            this.splitContainer_lv2_middle = new System.Windows.Forms.SplitContainer();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lstCharalist = new System.Windows.Forms.DataGridView();
            this.dirname = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.charaid = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.chartype = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.disporder = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cmdParse = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.menuStrip1.SuspendLayout();
            this.toolStrip.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer_lv2_middle)).BeginInit();
            this.splitContainer_lv2_middle.Panel1.SuspendLayout();
            this.splitContainer_lv2_middle.Panel2.SuspendLayout();
            this.splitContainer_lv2_middle.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.lstCharalist)).BeginInit();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(8, 3, 0, 3);
            this.menuStrip1.Size = new System.Drawing.Size(570, 28);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 22);
            this.fileToolStripMenuItem.Text = "file";
            // 
            // toolStrip
            // 
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton1});
            this.toolStrip.Location = new System.Drawing.Point(0, 28);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(570, 25);
            this.toolStrip.TabIndex = 1;
            this.toolStrip.Text = "toolStrip1";
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton1.Text = "toolStripButton1";
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.stpProgressbar,
            this.stbStatuslabel});
            this.statusStrip.Location = new System.Drawing.Point(0, 323);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Padding = new System.Windows.Forms.Padding(1, 0, 19, 0);
            this.statusStrip.Size = new System.Drawing.Size(570, 23);
            this.statusStrip.TabIndex = 2;
            this.statusStrip.Text = "statusStrip1";
            // 
            // stbStatuslabel
            // 
            this.stbStatuslabel.Name = "stbStatuslabel";
            this.stbStatuslabel.Size = new System.Drawing.Size(301, 18);
            this.stbStatuslabel.Text = "キャラ識別子を指定してください(台本で使用します）";
            // 
            // stpProgressbar
            // 
            this.stpProgressbar.Name = "stpProgressbar";
            this.stpProgressbar.Size = new System.Drawing.Size(100, 17);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.splitContainer1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(570, 270);
            this.panel1.TabIndex = 3;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.groupBox2);
            this.splitContainer1.Panel1MinSize = 40;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer_lv2_middle);
            this.splitContainer1.Size = new System.Drawing.Size(570, 270);
            this.splitContainer1.SplitterDistance = 58;
            this.splitContainer1.TabIndex = 2;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.txtCharbasedir);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(0, 0);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox2.Size = new System.Drawing.Size(570, 58);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "キャラ素材ディレクトリ";
            // 
            // txtCharbasedir
            // 
            this.txtCharbasedir.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtCharbasedir.Location = new System.Drawing.Point(4, 25);
            this.txtCharbasedir.Name = "txtCharbasedir";
            this.txtCharbasedir.Size = new System.Drawing.Size(562, 27);
            this.txtCharbasedir.TabIndex = 1;
            // 
            // splitContainer_lv2_middle
            // 
            this.splitContainer_lv2_middle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer_lv2_middle.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer_lv2_middle.Location = new System.Drawing.Point(0, 0);
            this.splitContainer_lv2_middle.Name = "splitContainer_lv2_middle";
            this.splitContainer_lv2_middle.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer_lv2_middle.Panel1
            // 
            this.splitContainer_lv2_middle.Panel1.Controls.Add(this.groupBox1);
            // 
            // splitContainer_lv2_middle.Panel2
            // 
            this.splitContainer_lv2_middle.Panel2.Controls.Add(this.cmdParse);
            this.splitContainer_lv2_middle.Size = new System.Drawing.Size(570, 208);
            this.splitContainer_lv2_middle.SplitterDistance = 167;
            this.splitContainer_lv2_middle.TabIndex = 1;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lstCharalist);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox1.Size = new System.Drawing.Size(570, 167);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "キャラ素材一覧";
            // 
            // lstCharalist
            // 
            this.lstCharalist.AllowUserToAddRows = false;
            this.lstCharalist.AllowUserToDeleteRows = false;
            this.lstCharalist.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.lstCharalist.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dirname,
            this.charaid,
            this.chartype,
            this.disporder});
            this.lstCharalist.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstCharalist.EnableHeadersVisualStyles = false;
            this.lstCharalist.Location = new System.Drawing.Point(4, 25);
            this.lstCharalist.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.lstCharalist.Name = "lstCharalist";
            this.lstCharalist.RowHeadersVisible = false;
            this.lstCharalist.RowTemplate.Height = 21;
            this.lstCharalist.Size = new System.Drawing.Size(562, 137);
            this.lstCharalist.TabIndex = 0;
            this.lstCharalist.CellValidated += new System.Windows.Forms.DataGridViewCellEventHandler(this.lstCharalist_CellValidated);
            this.lstCharalist.DataBindingComplete += new System.Windows.Forms.DataGridViewBindingCompleteEventHandler(this.lstCharalist_DataBindingComplete);
            // 
            // dirname
            // 
            this.dirname.HeaderText = "ディレクトリ名";
            this.dirname.Name = "dirname";
            this.dirname.ReadOnly = true;
            this.dirname.Width = 140;
            // 
            // charaid
            // 
            this.charaid.HeaderText = "キャラID";
            this.charaid.Name = "charaid";
            // 
            // chartype
            // 
            this.chartype.HeaderText = "キャラ種別";
            this.chartype.Name = "chartype";
            this.chartype.ReadOnly = true;
            // 
            // disporder
            // 
            this.disporder.HeaderText = "表示順";
            this.disporder.Name = "disporder";
            this.disporder.ReadOnly = true;
            // 
            // cmdParse
            // 
            this.cmdParse.Location = new System.Drawing.Point(9, 8);
            this.cmdParse.Name = "cmdParse";
            this.cmdParse.Size = new System.Drawing.Size(140, 24);
            this.cmdParse.TabIndex = 0;
            this.cmdParse.Text = "ディレクトリ解析";
            this.cmdParse.UseVisualStyleBackColor = true;
            this.cmdParse.Click += new System.EventHandler(this.cmdParse_Click);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.panel1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 53);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(570, 270);
            this.panel2.TabIndex = 4;
            // 
            // frmCharalist
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(570, 346);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.toolStrip);
            this.Controls.Add(this.menuStrip1);
            this.Font = new System.Drawing.Font("メイリオ", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "frmCharalist";
            this.Text = "キャラ素材一覧";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.splitContainer_lv2_middle.Panel1.ResumeLayout(false);
            this.splitContainer_lv2_middle.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer_lv2_middle)).EndInit();
            this.splitContainer_lv2_middle.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.lstCharalist)).EndInit();
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.MenuStrip menuStrip1;
    private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
    private System.Windows.Forms.ToolStrip toolStrip;
    private System.Windows.Forms.ToolStripButton toolStripButton1;
    private System.Windows.Forms.StatusStrip statusStrip;
    private System.Windows.Forms.ToolStripStatusLabel stbStatuslabel;
    private System.Windows.Forms.Panel panel1;
    private System.Windows.Forms.GroupBox groupBox1;
    private System.Windows.Forms.DataGridView lstCharalist;
    private System.Windows.Forms.GroupBox groupBox2;
    private System.Windows.Forms.TextBox txtCharbasedir;
    private System.Windows.Forms.SplitContainer splitContainer1;
    private System.Windows.Forms.Panel panel2;
    private System.Windows.Forms.DataGridViewTextBoxColumn dirname;
    private System.Windows.Forms.DataGridViewTextBoxColumn charaid;
    private System.Windows.Forms.DataGridViewTextBoxColumn chartype;
    private System.Windows.Forms.DataGridViewTextBoxColumn disporder;
    private System.Windows.Forms.ToolStripProgressBar stpProgressbar;
    private System.Windows.Forms.SplitContainer splitContainer_lv2_middle;
    private System.Windows.Forms.Button cmdParse;
  }
}