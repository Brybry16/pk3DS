namespace pk3DS.Subforms.Gen7
{
    partial class Randomizer7
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
            this.B_Randomize = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // B_Randomize
            // 
            this.B_Randomize.Location = new System.Drawing.Point(102, 226);
            this.B_Randomize.Name = "B_Randomize";
            this.B_Randomize.Size = new System.Drawing.Size(75, 23);
            this.B_Randomize.TabIndex = 0;
            this.B_Randomize.Text = "Randomize !";
            this.B_Randomize.UseVisualStyleBackColor = true;
            this.B_Randomize.Click += new System.EventHandler(this.B_Randomize_Click);
            // 
            // Randomizer7
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.B_Randomize);
            this.Name = "Randomizer7";
            this.Text = "Gen 7 Randomizer";
            this.Load += new System.EventHandler(this.Randomizer7_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button B_Randomize;
    }
}