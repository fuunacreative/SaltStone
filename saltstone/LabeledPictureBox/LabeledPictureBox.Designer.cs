namespace saltstone_customcontrol
{
    partial class LabeledPictureBox
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

        #region コンポーネント デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を 
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
      this.splitContainer1 = new System.Windows.Forms.SplitContainer();
      this.pcturebox = new System.Windows.Forms.PictureBox();
      this.lblPicturetext = new System.Windows.Forms.Label();
      this.panel = new System.Windows.Forms.Panel();
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
      this.splitContainer1.Panel1.SuspendLayout();
      this.splitContainer1.Panel2.SuspendLayout();
      this.splitContainer1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.pcturebox)).BeginInit();
      this.panel.SuspendLayout();
      this.SuspendLayout();
      // 
      // splitContainer1
      // 
      this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
      this.splitContainer1.IsSplitterFixed = true;
      this.splitContainer1.Location = new System.Drawing.Point(0, 0);
      this.splitContainer1.Name = "splitContainer1";
      this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
      // 
      // splitContainer1.Panel1
      // 
      this.splitContainer1.Panel1.Controls.Add(this.pcturebox);
      // 
      // splitContainer1.Panel2
      // 
      this.splitContainer1.Panel2.Controls.Add(this.lblPicturetext);
      this.splitContainer1.Panel2MinSize = 20;
      this.splitContainer1.Size = new System.Drawing.Size(126, 146);
      this.splitContainer1.SplitterDistance = 120;
      this.splitContainer1.SplitterWidth = 1;
      this.splitContainer1.TabIndex = 2;
      // 
      // pcturebox
      // 
      this.pcturebox.Dock = System.Windows.Forms.DockStyle.Fill;
      this.pcturebox.Image = global::LabeledPictureBox.Properties.Resources._200;
      this.pcturebox.Location = new System.Drawing.Point(0, 0);
      this.pcturebox.Name = "pcturebox";
      this.pcturebox.Size = new System.Drawing.Size(126, 120);
      this.pcturebox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
      this.pcturebox.TabIndex = 0;
      this.pcturebox.TabStop = false;
      this.pcturebox.Click += new System.EventHandler(this.pcturebox_Click);
      this.pcturebox.Paint += new System.Windows.Forms.PaintEventHandler(this.pcturebox_Paint);
      // 
      // lblPicturetext
      // 
      this.lblPicturetext.AutoSize = true;
      this.lblPicturetext.Dock = System.Windows.Forms.DockStyle.Left;
      this.lblPicturetext.Font = new System.Drawing.Font("メイリオ", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
      this.lblPicturetext.Location = new System.Drawing.Point(0, 0);
      this.lblPicturetext.Name = "lblPicturetext";
      this.lblPicturetext.Size = new System.Drawing.Size(47, 20);
      this.lblPicturetext.TabIndex = 1;
      this.lblPicturetext.Text = "label1";
      this.lblPicturetext.Click += new System.EventHandler(this.lblPicturetext_Click);
      // 
      // panel
      // 
      this.panel.Controls.Add(this.splitContainer1);
      this.panel.Dock = System.Windows.Forms.DockStyle.Fill;
      this.panel.Location = new System.Drawing.Point(0, 0);
      this.panel.Name = "panel";
      this.panel.Size = new System.Drawing.Size(126, 146);
      this.panel.TabIndex = 3;
      // 
      // LabeledPictureBox
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.panel);
      this.Name = "LabeledPictureBox";
      this.Size = new System.Drawing.Size(126, 146);
      this.splitContainer1.Panel1.ResumeLayout(false);
      this.splitContainer1.Panel2.ResumeLayout(false);
      this.splitContainer1.Panel2.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
      this.splitContainer1.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.pcturebox)).EndInit();
      this.panel.ResumeLayout(false);
      this.ResumeLayout(false);

        }

    #endregion

    private System.Windows.Forms.SplitContainer splitContainer1;
    private System.Windows.Forms.PictureBox pcturebox;
    private System.Windows.Forms.Label lblPicturetext;
    private System.Windows.Forms.Panel panel;
  }
}
