﻿
namespace IFME
{
    partial class frmProgressBar
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
            this.pbLoading = new System.Windows.Forms.ProgressBar();
            this.lblStatus = new System.Windows.Forms.Label();
            this.lblSeparator = new System.Windows.Forms.Label();
            this.lblFFmpeg = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // pbLoading
            // 
            this.pbLoading.Location = new System.Drawing.Point(12, 52);
            this.pbLoading.Name = "pbLoading";
            this.pbLoading.Size = new System.Drawing.Size(616, 23);
            this.pbLoading.TabIndex = 0;
            // 
            // lblStatus
            // 
            this.lblStatus.Location = new System.Drawing.Point(12, 9);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(616, 40);
            this.lblStatus.TabIndex = 1;
            this.lblStatus.Text = "Loading...";
            // 
            // lblSeparator
            // 
            this.lblSeparator.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblSeparator.Location = new System.Drawing.Point(118, 93);
            this.lblSeparator.Name = "lblSeparator";
            this.lblSeparator.Size = new System.Drawing.Size(510, 2);
            this.lblSeparator.TabIndex = 2;
            // 
            // lblFFmpeg
            // 
            this.lblFFmpeg.Enabled = false;
            this.lblFFmpeg.Location = new System.Drawing.Point(12, 80);
            this.lblFFmpeg.Name = "lblFFmpeg";
            this.lblFFmpeg.Size = new System.Drawing.Size(128, 24);
            this.lblFFmpeg.TabIndex = 3;
            this.lblFFmpeg.Text = "FFmpeg MediaInfo";
            this.lblFFmpeg.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // frmProgressBar
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(640, 128);
            this.ControlBox = false;
            this.Controls.Add(this.lblFFmpeg);
            this.Controls.Add(this.lblSeparator);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.pbLoading);
            this.Font = new System.Drawing.Font("Tahoma", 8F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmProgressBar";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "frmProgressBar";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.frmProgressBar_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ProgressBar pbLoading;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Label lblSeparator;
        private System.Windows.Forms.Label lblFFmpeg;
    }
}