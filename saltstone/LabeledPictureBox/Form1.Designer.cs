﻿
namespace LabeledPictureBox
{
  partial class Form1
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
      this.colorPanel1 = new saltstone_customcontrol.ColorPanel();
      this.SuspendLayout();
      // 
      // colorPanel1
      // 
      this.colorPanel1.BorderColor = System.Drawing.Color.Red;
      this.colorPanel1.Location = new System.Drawing.Point(78, 47);
      this.colorPanel1.Name = "colorPanel1";
      this.colorPanel1.Size = new System.Drawing.Size(319, 269);
      this.colorPanel1.TabIndex = 0;
      // 
      // Form1
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(635, 388);
      this.Controls.Add(this.colorPanel1);
      this.Name = "Form1";
      this.Text = "Form1";
      this.ResumeLayout(false);

    }

    #endregion

    private saltstone_customcontrol.ColorPanel colorPanel1;
  }
}