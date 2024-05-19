
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmLogView));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lstLog = new System.Windows.Forms.DataGridView();
            this.logdate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.exename = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.logtype = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.message = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.trace = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.ファイルToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.保存ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.testToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mmftestToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.lblMessage = new System.Windows.Forms.ToolStripStatusLabel();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.lstLog)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lstLog);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 30);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.groupBox1.Size = new System.Drawing.Size(975, 482);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            // 
            // lstLog
            // 
            this.lstLog.AllowUserToAddRows = false;
            this.lstLog.AllowUserToDeleteRows = false;
            this.lstLog.AllowUserToResizeRows = false;
            this.lstLog.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCells;
            this.lstLog.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.lstLog.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.logdate,
            this.exename,
            this.logtype,
            this.message,
            this.trace});
            this.lstLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstLog.Location = new System.Drawing.Point(5, 30);
            this.lstLog.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.lstLog.Name = "lstLog";
            this.lstLog.RowHeadersVisible = false;
            this.lstLog.RowTemplate.Height = 21;
            this.lstLog.Size = new System.Drawing.Size(965, 446);
            this.lstLog.TabIndex = 0;
            // 
            // logdate
            // 
            this.logdate.HeaderText = "日時";
            this.logdate.Name = "logdate";
            this.logdate.ReadOnly = true;
            this.logdate.Width = 67;
            // 
            // exename
            // 
            this.exename.HeaderText = "exe";
            this.exename.Name = "exename";
            this.exename.Width = 62;
            // 
            // logtype
            // 
            this.logtype.HeaderText = "種別";
            this.logtype.Name = "logtype";
            this.logtype.Width = 67;
            // 
            // message
            // 
            this.message.HeaderText = "メッセージ";
            this.message.Name = "message";
            this.message.Width = 115;
            // 
            // trace
            // 
            this.trace.HeaderText = "trace";
            this.trace.Name = "trace";
            this.trace.Width = 74;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ファイルToolStripMenuItem,
            this.testToolStripMenuItem,
            this.mmftestToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(10, 4, 0, 4);
            this.menuStrip1.Size = new System.Drawing.Size(975, 30);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // ファイルToolStripMenuItem
            // 
            this.ファイルToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.保存ToolStripMenuItem});
            this.ファイルToolStripMenuItem.Name = "ファイルToolStripMenuItem";
            this.ファイルToolStripMenuItem.Size = new System.Drawing.Size(68, 22);
            this.ファイルToolStripMenuItem.Text = "ファイル";
            // 
            // 保存ToolStripMenuItem
            // 
            this.保存ToolStripMenuItem.Name = "保存ToolStripMenuItem";
            this.保存ToolStripMenuItem.Size = new System.Drawing.Size(100, 22);
            this.保存ToolStripMenuItem.Text = "保存";
            // 
            // testToolStripMenuItem
            // 
            this.testToolStripMenuItem.Name = "testToolStripMenuItem";
            this.testToolStripMenuItem.Size = new System.Drawing.Size(43, 22);
            this.testToolStripMenuItem.Text = "test";
            this.testToolStripMenuItem.Click += new System.EventHandler(this.testToolStripMenuItem_Click);
            // 
            // mmftestToolStripMenuItem
            // 
            this.mmftestToolStripMenuItem.Name = "mmftestToolStripMenuItem";
            this.mmftestToolStripMenuItem.Size = new System.Drawing.Size(71, 22);
            this.mmftestToolStripMenuItem.Text = "mmftest";
            this.mmftestToolStripMenuItem.Click += new System.EventHandler(this.mmftestToolStripMenuItem_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblMessage});
            this.statusStrip1.Location = new System.Drawing.Point(0, 489);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Padding = new System.Windows.Forms.Padding(2, 0, 23, 0);
            this.statusStrip1.Size = new System.Drawing.Size(975, 23);
            this.statusStrip1.TabIndex = 2;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // lblMessage
            // 
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(134, 18);
            this.lblMessage.Text = "toolStripStatusLabel1";
            // 
            // frmLogView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(975, 512);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.menuStrip1);
            this.Font = new System.Drawing.Font("メイリオ", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.Name = "frmLogView";
            this.Text = "SaltStoneログビューワー";
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.lstLog)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

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
  }
}

