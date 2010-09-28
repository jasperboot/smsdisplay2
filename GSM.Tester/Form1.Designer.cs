namespace GSM.Tester
{
    partial class GSMcomDbg
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
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.atRx = new System.Windows.Forms.TextBox();
            this.btnTest = new System.Windows.Forms.Button();
            this.btnSend = new System.Windows.Forms.Button();
            this.atRxLabel = new System.Windows.Forms.Label();
            this.atTxLabel = new System.Windows.Forms.Label();
            this.atTx = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.Location = new System.Drawing.Point(107, 12);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(720, 21);
            this.textBox1.TabIndex = 0;
            // 
            // atRx
            // 
            this.atRx.AcceptsReturn = true;
            this.atRx.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.atRx.Font = new System.Drawing.Font("Lucida Console", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.atRx.Location = new System.Drawing.Point(12, 226);
            this.atRx.Multiline = true;
            this.atRx.Name = "atRx";
            this.atRx.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.atRx.Size = new System.Drawing.Size(896, 430);
            this.atRx.TabIndex = 1;
            this.atRx.WordWrap = false;
            // 
            // btnTest
            // 
            this.btnTest.Location = new System.Drawing.Point(12, 12);
            this.btnTest.Name = "btnTest";
            this.btnTest.Size = new System.Drawing.Size(82, 23);
            this.btnTest.TabIndex = 2;
            this.btnTest.Text = "(Dis)Connect";
            this.btnTest.UseVisualStyleBackColor = true;
            this.btnTest.Click += new System.EventHandler(this.btnTest_Click);
            // 
            // btnSend
            // 
            this.btnSend.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSend.Location = new System.Drawing.Point(833, 12);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(75, 23);
            this.btnSend.TabIndex = 3;
            this.btnSend.Text = "Send";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // atRxLabel
            // 
            this.atRxLabel.AutoSize = true;
            this.atRxLabel.Location = new System.Drawing.Point(12, 210);
            this.atRxLabel.Name = "atRxLabel";
            this.atRxLabel.Size = new System.Drawing.Size(119, 13);
            this.atRxLabel.TabIndex = 4;
            this.atRxLabel.Text = "Monitor GSM responses";
            // 
            // atTxLabel
            // 
            this.atTxLabel.AutoSize = true;
            this.atTxLabel.Location = new System.Drawing.Point(12, 38);
            this.atTxLabel.Name = "atTxLabel";
            this.atTxLabel.Size = new System.Drawing.Size(104, 13);
            this.atTxLabel.TabIndex = 5;
            this.atTxLabel.Text = "Monitor PC requests";
            // 
            // atTx
            // 
            this.atTx.AcceptsReturn = true;
            this.atTx.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.atTx.Font = new System.Drawing.Font("Lucida Console", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.atTx.Location = new System.Drawing.Point(12, 54);
            this.atTx.Multiline = true;
            this.atTx.Name = "atTx";
            this.atTx.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.atTx.Size = new System.Drawing.Size(896, 153);
            this.atTx.TabIndex = 6;
            this.atTx.WordWrap = false;
            // 
            // GSMcomDbg
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(918, 668);
            this.Controls.Add(this.atTx);
            this.Controls.Add(this.atTxLabel);
            this.Controls.Add(this.atRxLabel);
            this.Controls.Add(this.btnSend);
            this.Controls.Add(this.btnTest);
            this.Controls.Add(this.atRx);
            this.Controls.Add(this.textBox1);
            this.Name = "GSMcomDbg";
            this.Text = "Monitor GSM communication";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.GSMcomDbg_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox atRx;
        private System.Windows.Forms.Button btnTest;
        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.Label atRxLabel;
        private System.Windows.Forms.Label atTxLabel;
        private System.Windows.Forms.TextBox atTx;
    }
}

