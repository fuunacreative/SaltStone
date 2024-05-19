
namespace saltstonevoice_aq
{
  partial class Form1
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
      this.button1 = new System.Windows.Forms.Button();
      this.txtPhonetic = new System.Windows.Forms.TextBox();
      this.cmdTerminate = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // button1
      // 
      this.button1.Location = new System.Drawing.Point(309, 71);
      this.button1.Name = "button1";
      this.button1.Size = new System.Drawing.Size(108, 52);
      this.button1.TabIndex = 0;
      this.button1.Text = "button1";
      this.button1.UseVisualStyleBackColor = true;
      this.button1.Click += new System.EventHandler(this.button1_Click);
      // 
      // txtPhonetic
      // 
      this.txtPhonetic.Location = new System.Drawing.Point(156, 210);
      this.txtPhonetic.Name = "txtPhonetic";
      this.txtPhonetic.Size = new System.Drawing.Size(334, 19);
      this.txtPhonetic.TabIndex = 1;
      // 
      // cmdTerminate
      // 
      this.cmdTerminate.Location = new System.Drawing.Point(383, 301);
      this.cmdTerminate.Name = "cmdTerminate";
      this.cmdTerminate.Size = new System.Drawing.Size(107, 74);
      this.cmdTerminate.TabIndex = 2;
      this.cmdTerminate.Text = "button2";
      this.cmdTerminate.UseVisualStyleBackColor = true;
      // 
      // Form1
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(800, 450);
      this.Controls.Add(this.cmdTerminate);
      this.Controls.Add(this.txtPhonetic);
      this.Controls.Add(this.button1);
      this.Name = "Form1";
      this.Text = "Form1";
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Button button1;
    private System.Windows.Forms.TextBox txtPhonetic;
    private System.Windows.Forms.Button cmdTerminate;
  }
}

