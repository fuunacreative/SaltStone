
namespace saltstone
{
    partial class frmLogView
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
      groupBox1 = new GroupBox();
      lstLog = new DataGridView();
      logdate = new DataGridViewTextBoxColumn();
      exename = new DataGridViewTextBoxColumn();
      logtype = new DataGridViewTextBoxColumn();
      message = new DataGridViewTextBoxColumn();
      trace = new DataGridViewTextBoxColumn();
      menuStrip1 = new MenuStrip();
      ファイルToolStripMenuItem = new ToolStripMenuItem();
      保存ToolStripMenuItem = new ToolStripMenuItem();
      testToolStripMenuItem = new ToolStripMenuItem();
      mmftestToolStripMenuItem = new ToolStripMenuItem();
      statusStrip1 = new StatusStrip();
      lblMessage = new ToolStripStatusLabel();
      splitContainer1 = new SplitContainer();
      comboBox1 = new ComboBox();
      label1 = new Label();
      groupBox1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)lstLog).BeginInit();
      menuStrip1.SuspendLayout();
      statusStrip1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
      splitContainer1.Panel1.SuspendLayout();
      splitContainer1.Panel2.SuspendLayout();
      splitContainer1.SuspendLayout();
      SuspendLayout();
      // 
      // groupBox1
      // 
      groupBox1.Controls.Add(splitContainer1);
      groupBox1.Dock = DockStyle.Fill;
      groupBox1.Location = new Point(0, 44);
      groupBox1.Margin = new Padding(5, 6, 5, 6);
      groupBox1.Name = "groupBox1";
      groupBox1.Padding = new Padding(5, 6, 5, 6);
      groupBox1.Size = new Size(1469, 976);
      groupBox1.TabIndex = 0;
      groupBox1.TabStop = false;
      // 
      // lstLog
      // 
      lstLog.AllowUserToAddRows = false;
      lstLog.AllowUserToDeleteRows = false;
      lstLog.AllowUserToResizeRows = false;
      lstLog.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
      lstLog.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      lstLog.Columns.AddRange(new DataGridViewColumn[] { logdate, exename, logtype, message, trace });
      lstLog.Dock = DockStyle.Fill;
      lstLog.Location = new Point(0, 0);
      lstLog.Margin = new Padding(5, 6, 5, 6);
      lstLog.Name = "lstLog";
      lstLog.RowHeadersVisible = false;
      lstLog.RowHeadersWidth = 82;
      lstLog.RowTemplate.Height = 21;
      lstLog.Size = new Size(1459, 831);
      lstLog.TabIndex = 0;
      // 
      // logdate
      // 
      logdate.HeaderText = "日時";
      logdate.MinimumWidth = 10;
      logdate.Name = "logdate";
      logdate.ReadOnly = true;
      logdate.Width = 129;
      // 
      // exename
      // 
      exename.HeaderText = "exe";
      exename.MinimumWidth = 10;
      exename.Name = "exename";
      exename.Width = 119;
      // 
      // logtype
      // 
      logtype.HeaderText = "種別";
      logtype.MinimumWidth = 10;
      logtype.Name = "logtype";
      logtype.Width = 129;
      // 
      // message
      // 
      message.HeaderText = "メッセージ";
      message.MinimumWidth = 10;
      message.Name = "message";
      message.Width = 225;
      // 
      // trace
      // 
      trace.HeaderText = "trace";
      trace.MinimumWidth = 10;
      trace.Name = "trace";
      trace.Width = 143;
      // 
      // menuStrip1
      // 
      menuStrip1.ImageScalingSize = new Size(32, 32);
      menuStrip1.Items.AddRange(new ToolStripItem[] { ファイルToolStripMenuItem, testToolStripMenuItem, mmftestToolStripMenuItem });
      menuStrip1.Location = new Point(0, 0);
      menuStrip1.Name = "menuStrip1";
      menuStrip1.Padding = new Padding(10, 4, 0, 4);
      menuStrip1.Size = new Size(1469, 44);
      menuStrip1.TabIndex = 1;
      menuStrip1.Text = "menuStrip1";
      // 
      // ファイルToolStripMenuItem
      // 
      ファイルToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { 保存ToolStripMenuItem });
      ファイルToolStripMenuItem.Name = "ファイルToolStripMenuItem";
      ファイルToolStripMenuItem.Size = new Size(102, 36);
      ファイルToolStripMenuItem.Text = "ファイル";
      // 
      // 保存ToolStripMenuItem
      // 
      保存ToolStripMenuItem.Name = "保存ToolStripMenuItem";
      保存ToolStripMenuItem.Size = new Size(195, 44);
      保存ToolStripMenuItem.Text = "保存";
      // 
      // testToolStripMenuItem
      // 
      testToolStripMenuItem.Name = "testToolStripMenuItem";
      testToolStripMenuItem.Size = new Size(73, 36);
      testToolStripMenuItem.Text = "test";
      testToolStripMenuItem.Click += testToolStripMenuItem_Click;
      // 
      // mmftestToolStripMenuItem
      // 
      mmftestToolStripMenuItem.Name = "mmftestToolStripMenuItem";
      mmftestToolStripMenuItem.Size = new Size(123, 36);
      mmftestToolStripMenuItem.Text = "mmftest";
      mmftestToolStripMenuItem.Click += mmftestToolStripMenuItem_Click;
      // 
      // statusStrip1
      // 
      statusStrip1.ImageScalingSize = new Size(32, 32);
      statusStrip1.Items.AddRange(new ToolStripItem[] { lblMessage });
      statusStrip1.Location = new Point(0, 978);
      statusStrip1.Name = "statusStrip1";
      statusStrip1.Padding = new Padding(2, 0, 23, 0);
      statusStrip1.Size = new Size(1469, 42);
      statusStrip1.TabIndex = 2;
      statusStrip1.Text = "statusStrip1";
      // 
      // lblMessage
      // 
      lblMessage.Name = "lblMessage";
      lblMessage.Size = new Size(239, 32);
      lblMessage.Text = "toolStripStatusLabel1";
      // 
      // splitContainer1
      // 
      splitContainer1.Dock = DockStyle.Fill;
      splitContainer1.Location = new Point(5, 54);
      splitContainer1.Name = "splitContainer1";
      splitContainer1.Orientation = Orientation.Horizontal;
      // 
      // splitContainer1.Panel1
      // 
      splitContainer1.Panel1.Controls.Add(label1);
      splitContainer1.Panel1.Controls.Add(comboBox1);
      // 
      // splitContainer1.Panel2
      // 
      splitContainer1.Panel2.Controls.Add(lstLog);
      splitContainer1.Size = new Size(1459, 916);
      splitContainer1.SplitterDistance = 81;
      splitContainer1.TabIndex = 1;
      // 
      // comboBox1
      // 
      comboBox1.FormattingEnabled = true;
      comboBox1.Location = new Point(193, 14);
      comboBox1.Name = "comboBox1";
      comboBox1.Size = new Size(571, 56);
      comboBox1.TabIndex = 0;
      // 
      // label1
      // 
      label1.AutoSize = true;
      label1.Location = new Point(7, 17);
      label1.Name = "label1";
      label1.Size = new Size(180, 48);
      label1.TabIndex = 1;
      label1.Text = "フィルター";
      // 
      // frmLogView
      // 
      AutoScaleDimensions = new SizeF(19F, 48F);
      AutoScaleMode = AutoScaleMode.Font;
      ClientSize = new Size(1469, 1020);
      Controls.Add(statusStrip1);
      Controls.Add(groupBox1);
      Controls.Add(menuStrip1);
      Font = new Font("メイリオ", 12F, FontStyle.Regular, GraphicsUnit.Point, 128);
      MainMenuStrip = menuStrip1;
      Margin = new Padding(5, 6, 5, 6);
      Name = "frmLogView";
      Text = "SaltStoneログビューワー";
      groupBox1.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)lstLog).EndInit();
      menuStrip1.ResumeLayout(false);
      menuStrip1.PerformLayout();
      statusStrip1.ResumeLayout(false);
      statusStrip1.PerformLayout();
      splitContainer1.Panel1.ResumeLayout(false);
      splitContainer1.Panel1.PerformLayout();
      splitContainer1.Panel2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
      splitContainer1.ResumeLayout(false);
      ResumeLayout(false);
      PerformLayout();
    }

    #endregion

    private System.Windows.Forms.GroupBox groupBox1;
    private System.Windows.Forms.DataGridView lstLog;
    private System.Windows.Forms.MenuStrip menuStrip1;
    private System.Windows.Forms.StatusStrip statusStrip1;
    private System.Windows.Forms.ToolStripMenuItem ファイルToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem 保存ToolStripMenuItem;
    private System.Windows.Forms.ToolStripStatusLabel lblMessage;
    private System.Windows.Forms.DataGridViewTextBoxColumn logdate;
    private System.Windows.Forms.DataGridViewTextBoxColumn exename;
    private System.Windows.Forms.DataGridViewTextBoxColumn logtype;
    private System.Windows.Forms.DataGridViewTextBoxColumn message;
    private System.Windows.Forms.DataGridViewTextBoxColumn trace;
    private System.Windows.Forms.ToolStripMenuItem testToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem mmftestToolStripMenuItem;
    private SplitContainer splitContainer1;
    private Label label1;
    private ComboBox comboBox1;
  }
}

