namespace GSM.Forms
{
    partial class MonitorForm
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
            this.atRx = new System.Windows.Forms.TextBox();
            this.atRxLabel = new System.Windows.Forms.Label();
            this.atTxLabel = new System.Windows.Forms.Label();
            this.atTx = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // atRx
            // 
            this.atRx.AcceptsReturn = true;
            this.atRx.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.atRx.Font = new System.Drawing.Font("Lucida Console", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.atRx.Location = new System.Drawing.Point(12, 197);
            this.atRx.Multiline = true;
            this.atRx.Name = "atRx";
            this.atRx.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.atRx.Size = new System.Drawing.Size(701, 301);
            this.atRx.TabIndex = 1;
            this.atRx.WordWrap = false;
            // 
            // atRxLabel
            // 
            this.atRxLabel.AutoSize = true;
            this.atRxLabel.Location = new System.Drawing.Point(12, 181);
            this.atRxLabel.Name = "atRxLabel";
            this.atRxLabel.Size = new System.Drawing.Size(119, 13);
            this.atRxLabel.TabIndex = 4;
            this.atRxLabel.Text = "Monitor GSM responses";
            // 
            // atTxLabel
            // 
            this.atTxLabel.AutoSize = true;
            this.atTxLabel.Location = new System.Drawing.Point(9, 9);
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
            this.atTx.Location = new System.Drawing.Point(12, 25);
            this.atTx.Multiline = true;
            this.atTx.Name = "atTx";
            this.atTx.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.atTx.Size = new System.Drawing.Size(701, 153);
            this.atTx.TabIndex = 6;
            this.atTx.WordWrap = false;
            // 
            // MonitorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(723, 510);
            this.Controls.Add(this.atTx);
            this.Controls.Add(this.atTxLabel);
            this.Controls.Add(this.atRxLabel);
            this.Controls.Add(this.atRx);
            this.Name = "MonitorForm";
            this.Text = "Monitor GSM communication";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox atRx;
        private System.Windows.Forms.Label atRxLabel;
        private System.Windows.Forms.Label atTxLabel;
        private System.Windows.Forms.TextBox atTx;
    }
}

