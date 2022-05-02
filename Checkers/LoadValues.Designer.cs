namespace Checkers
{
    partial class LoadValues
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
            this.lblRed = new System.Windows.Forms.Label();
            this.lblBlack = new System.Windows.Forms.Label();
            this.lblKings = new System.Windows.Forms.Label();
            this.txtRed = new System.Windows.Forms.TextBox();
            this.txtBlack = new System.Windows.Forms.TextBox();
            this.txtKings = new System.Windows.Forms.TextBox();
            this.btnLoad = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblRed
            // 
            this.lblRed.AutoSize = true;
            this.lblRed.Location = new System.Drawing.Point(5, 25);
            this.lblRed.Name = "lblRed";
            this.lblRed.Size = new System.Drawing.Size(30, 13);
            this.lblRed.TabIndex = 0;
            this.lblRed.Text = "Red:";
            // 
            // lblBlack
            // 
            this.lblBlack.AutoSize = true;
            this.lblBlack.Location = new System.Drawing.Point(5, 62);
            this.lblBlack.Name = "lblBlack";
            this.lblBlack.Size = new System.Drawing.Size(37, 13);
            this.lblBlack.TabIndex = 1;
            this.lblBlack.Text = "Black:";
            // 
            // lblKings
            // 
            this.lblKings.AutoSize = true;
            this.lblKings.Location = new System.Drawing.Point(6, 95);
            this.lblKings.Name = "lblKings";
            this.lblKings.Size = new System.Drawing.Size(36, 13);
            this.lblKings.TabIndex = 2;
            this.lblKings.Text = "Kings:";
            // 
            // txtRed
            // 
            this.txtRed.Location = new System.Drawing.Point(59, 22);
            this.txtRed.Name = "txtRed";
            this.txtRed.Size = new System.Drawing.Size(166, 20);
            this.txtRed.TabIndex = 3;
            // 
            // txtBlack
            // 
            this.txtBlack.Location = new System.Drawing.Point(59, 59);
            this.txtBlack.Name = "txtBlack";
            this.txtBlack.Size = new System.Drawing.Size(166, 20);
            this.txtBlack.TabIndex = 4;
            // 
            // txtKings
            // 
            this.txtKings.Location = new System.Drawing.Point(59, 92);
            this.txtKings.Name = "txtKings";
            this.txtKings.Size = new System.Drawing.Size(166, 20);
            this.txtKings.TabIndex = 5;
            // 
            // btnLoad
            // 
            this.btnLoad.Location = new System.Drawing.Point(59, 133);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(75, 23);
            this.btnLoad.TabIndex = 6;
            this.btnLoad.Text = "&Load";
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(150, 133);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "&Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // LoadValues
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(238, 182);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnLoad);
            this.Controls.Add(this.txtKings);
            this.Controls.Add(this.txtBlack);
            this.Controls.Add(this.txtRed);
            this.Controls.Add(this.lblKings);
            this.Controls.Add(this.lblBlack);
            this.Controls.Add(this.lblRed);
            this.Name = "LoadValues";
            this.Text = "LoadValues";
            this.Load += new System.EventHandler(this.LoadValues_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblRed;
        private System.Windows.Forms.Label lblBlack;
        private System.Windows.Forms.Label lblKings;
        private System.Windows.Forms.TextBox txtRed;
        private System.Windows.Forms.TextBox txtBlack;
        private System.Windows.Forms.TextBox txtKings;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.Button btnCancel;
    }
}