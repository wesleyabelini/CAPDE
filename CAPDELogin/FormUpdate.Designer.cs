namespace CAPDELogin
{
    partial class FormUpdate
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
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnDownload = new System.Windows.Forms.Button();
            this.btnCancelar = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.lblVersaoAtual = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.lblDataNew = new System.Windows.Forms.Label();
            this.lblVersaoNew = new System.Windows.Forms.Label();
            this.rickTxtObs = new System.Windows.Forms.RichTextBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.tsProgressBar = new System.Windows.Forms.ToolStripProgressBar();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.SystemColors.Control;
            this.pictureBox1.Image = global::CAPDELogin.Properties.Resources.logoCadQ;
            this.pictureBox1.Location = new System.Drawing.Point(12, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(163, 355);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(211, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(299, 20);
            this.label1.TabIndex = 1;
            this.label1.Text = "Uma nova versão CAPDE está disponível";
            // 
            // btnDownload
            // 
            this.btnDownload.Location = new System.Drawing.Point(459, 336);
            this.btnDownload.Name = "btnDownload";
            this.btnDownload.Size = new System.Drawing.Size(93, 31);
            this.btnDownload.TabIndex = 3;
            this.btnDownload.Text = "Download";
            this.btnDownload.UseVisualStyleBackColor = true;
            this.btnDownload.Click += new System.EventHandler(this.btnDownload_Click);
            // 
            // btnCancelar
            // 
            this.btnCancelar.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancelar.Location = new System.Drawing.Point(375, 340);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(75, 23);
            this.btnCancelar.TabIndex = 4;
            this.btnCancelar.Text = "Cancelar";
            this.btnCancelar.UseVisualStyleBackColor = true;
            this.btnCancelar.Click += new System.EventHandler(this.btnCancelar_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(20, 25);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(77, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Nome: CAPDE";
            // 
            // lblVersaoAtual
            // 
            this.lblVersaoAtual.AutoSize = true;
            this.lblVersaoAtual.Location = new System.Drawing.Point(20, 50);
            this.lblVersaoAtual.Name = "lblVersaoAtual";
            this.lblVersaoAtual.Size = new System.Drawing.Size(43, 13);
            this.lblVersaoAtual.TabIndex = 6;
            this.lblVersaoAtual.Text = "Versão:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.lblVersaoAtual);
            this.groupBox1.Location = new System.Drawing.Point(181, 55);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(180, 105);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Versão Atual";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.lblDataNew);
            this.groupBox2.Controls.Add(this.lblVersaoNew);
            this.groupBox2.Location = new System.Drawing.Point(375, 55);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(177, 105);
            this.groupBox2.TabIndex = 9;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Nova Versão";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(19, 25);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(77, 13);
            this.label5.TabIndex = 8;
            this.label5.Text = "Nome: CAPDE";
            // 
            // lblDataNew
            // 
            this.lblDataNew.AutoSize = true;
            this.lblDataNew.Location = new System.Drawing.Point(19, 77);
            this.lblDataNew.Name = "lblDataNew";
            this.lblDataNew.Size = new System.Drawing.Size(36, 13);
            this.lblDataNew.TabIndex = 10;
            this.lblDataNew.Text = "Data: ";
            // 
            // lblVersaoNew
            // 
            this.lblVersaoNew.AutoSize = true;
            this.lblVersaoNew.Location = new System.Drawing.Point(19, 50);
            this.lblVersaoNew.Name = "lblVersaoNew";
            this.lblVersaoNew.Size = new System.Drawing.Size(43, 13);
            this.lblVersaoNew.TabIndex = 9;
            this.lblVersaoNew.Text = "Versão:";
            // 
            // rickTxtObs
            // 
            this.rickTxtObs.Location = new System.Drawing.Point(181, 178);
            this.rickTxtObs.Name = "rickTxtObs";
            this.rickTxtObs.Size = new System.Drawing.Size(371, 152);
            this.rickTxtObs.TabIndex = 10;
            this.rickTxtObs.Text = "";
            // 
            // statusStrip1
            // 
            this.statusStrip1.BackColor = System.Drawing.Color.DimGray;
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsProgressBar});
            this.statusStrip1.Location = new System.Drawing.Point(0, 374);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.statusStrip1.Size = new System.Drawing.Size(564, 22);
            this.statusStrip1.TabIndex = 11;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // tsProgressBar
            // 
            this.tsProgressBar.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.tsProgressBar.Name = "tsProgressBar";
            this.tsProgressBar.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.tsProgressBar.Size = new System.Drawing.Size(550, 16);
            this.tsProgressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            // 
            // FormUpdate
            // 
            this.AcceptButton = this.btnDownload;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.DarkGray;
            this.CancelButton = this.btnCancelar;
            this.ClientSize = new System.Drawing.Size(564, 396);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.rickTxtObs);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnCancelar);
            this.Controls.Add(this.btnDownload);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pictureBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormUpdate";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FormUpdate";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnDownload;
        private System.Windows.Forms.Button btnCancelar;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblVersaoAtual;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label lblDataNew;
        private System.Windows.Forms.Label lblVersaoNew;
        private System.Windows.Forms.RichTextBox rickTxtObs;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripProgressBar tsProgressBar;
    }
}