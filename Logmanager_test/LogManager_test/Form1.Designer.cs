namespace LogManager_test
{
  partial class Form1
  {
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            cmdLogWrite = new Button();
            SuspendLayout();
            // 
            // cmdLogWrite
            // 
            cmdLogWrite.Location = new Point(82, 61);
            cmdLogWrite.Name = "cmdLogWrite";
            cmdLogWrite.Size = new Size(120, 58);
            cmdLogWrite.TabIndex = 0;
            cmdLogWrite.Text = "log_wrte";
            cmdLogWrite.UseVisualStyleBackColor = true;
            cmdLogWrite.Click += cmdLogWrite_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(cmdLogWrite);
            Name = "Form1";
            Text = "frmLogtest";
            ResumeLayout(false);
        }

        #endregion

        private Button cmdLogWrite;
    }
}
