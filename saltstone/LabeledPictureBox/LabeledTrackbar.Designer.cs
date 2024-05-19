
namespace saltstone_customcontrol
{
  partial class LabeledTrackbar
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
      this.splitContainer2 = new System.Windows.Forms.SplitContainer();
      this.lblbarname = new System.Windows.Forms.Label();
      this.txtbarvalue = new System.Windows.Forms.TextBox();
      this.ctlBar = new System.Windows.Forms.HScrollBar();
      this.ctlpicture = new System.Windows.Forms.PictureBox();
      this.lblpicturename = new System.Windows.Forms.Label();
      this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
      this.splitContainer2.Panel1.SuspendLayout();
      this.splitContainer2.Panel2.SuspendLayout();
      this.splitContainer2.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.ctlpicture)).BeginInit();
      this.tableLayoutPanel1.SuspendLayout();
      this.SuspendLayout();
      // 
      // splitContainer2
      // 
      this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
      this.splitContainer2.Location = new System.Drawing.Point(0, 137);
      this.splitContainer2.Margin = new System.Windows.Forms.Padding(0);
      this.splitContainer2.Name = "splitContainer2";
      // 
      // splitContainer2.Panel1
      // 
      this.splitContainer2.Panel1.Controls.Add(this.lblbarname);
      // 
      // splitContainer2.Panel2
      // 
      this.splitContainer2.Panel2.Controls.Add(this.txtbarvalue);
      this.splitContainer2.Size = new System.Drawing.Size(141, 27);
      this.splitContainer2.SplitterDistance = 89;
      this.splitContainer2.SplitterWidth = 1;
      this.splitContainer2.TabIndex = 0;
      // 
      // lblbarname
      // 
      this.lblbarname.AutoSize = true;
      this.lblbarname.Font = new System.Drawing.Font("メイリオ", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
      this.lblbarname.Location = new System.Drawing.Point(3, 5);
      this.lblbarname.Name = "lblbarname";
      this.lblbarname.Size = new System.Drawing.Size(61, 20);
      this.lblbarname.TabIndex = 2;
      this.lblbarname.Text = "肌カラー";
      // 
      // txtbarvalue
      // 
      this.txtbarvalue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.txtbarvalue.Font = new System.Drawing.Font("メイリオ", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
      this.txtbarvalue.Location = new System.Drawing.Point(11, 0);
      this.txtbarvalue.Margin = new System.Windows.Forms.Padding(3, 3, 1, 3);
      this.txtbarvalue.Name = "txtbarvalue";
      this.txtbarvalue.Size = new System.Drawing.Size(42, 25);
      this.txtbarvalue.TabIndex = 1;
      this.txtbarvalue.Text = "120";
      // 
      // ctlBar
      // 
      this.ctlBar.Location = new System.Drawing.Point(0, 164);
      this.ctlBar.Maximum = 300;
      this.ctlBar.Minimum = -1;
      this.ctlBar.Name = "ctlBar";
      this.ctlBar.Size = new System.Drawing.Size(141, 20);
      this.ctlBar.TabIndex = 2;
      this.ctlBar.Value = 100;
      this.ctlBar.Scroll += new System.Windows.Forms.ScrollEventHandler(this.ctlBar_Scroll);
      // 
      // ctlpicture
      // 
      this.ctlpicture.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ctlpicture.Image = global::LabeledPictureBox.Properties.Resources._200;
      this.ctlpicture.Location = new System.Drawing.Point(0, 0);
      this.ctlpicture.Margin = new System.Windows.Forms.Padding(0);
      this.ctlpicture.Name = "ctlpicture";
      this.ctlpicture.Size = new System.Drawing.Size(141, 117);
      this.ctlpicture.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
      this.ctlpicture.TabIndex = 2;
      this.ctlpicture.TabStop = false;
      this.ctlpicture.Paint += new System.Windows.Forms.PaintEventHandler(this.ctlpicture_Paint);
      // 
      // lblpicturename
      // 
      this.lblpicturename.AutoSize = true;
      this.lblpicturename.Location = new System.Drawing.Point(3, 120);
      this.lblpicturename.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
      this.lblpicturename.Name = "lblpicturename";
      this.lblpicturename.Size = new System.Drawing.Size(55, 12);
      this.lblpicturename.TabIndex = 3;
      this.lblpicturename.Text = "体\\00.png";
      // 
      // tableLayoutPanel1
      // 
      this.tableLayoutPanel1.ColumnCount = 1;
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel1.Controls.Add(this.splitContainer2, 0, 2);
      this.tableLayoutPanel1.Controls.Add(this.ctlpicture, 0, 0);
      this.tableLayoutPanel1.Controls.Add(this.ctlBar, 0, 3);
      this.tableLayoutPanel1.Controls.Add(this.lblpicturename, 0, 1);
      this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
      this.tableLayoutPanel1.Name = "tableLayoutPanel1";
      this.tableLayoutPanel1.RowCount = 4;
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 27F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 21F));
      this.tableLayoutPanel1.Size = new System.Drawing.Size(141, 185);
      this.tableLayoutPanel1.TabIndex = 4;
      // 
      // LabeledTrackbar
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.tableLayoutPanel1);
      this.Name = "LabeledTrackbar";
      this.Size = new System.Drawing.Size(141, 185);
      this.splitContainer2.Panel1.ResumeLayout(false);
      this.splitContainer2.Panel1.PerformLayout();
      this.splitContainer2.Panel2.ResumeLayout(false);
      this.splitContainer2.Panel2.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
      this.splitContainer2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.ctlpicture)).EndInit();
      this.tableLayoutPanel1.ResumeLayout(false);
      this.tableLayoutPanel1.PerformLayout();
      this.ResumeLayout(false);

    }

    #endregion
    private System.Windows.Forms.SplitContainer splitContainer2;
    private System.Windows.Forms.TextBox txtbarvalue;
    private System.Windows.Forms.Label lblbarname;
    private System.Windows.Forms.HScrollBar ctlBar;
    private System.Windows.Forms.PictureBox ctlpicture;
    private System.Windows.Forms.Label lblpicturename;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
  }
}
