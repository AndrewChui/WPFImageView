namespace ImageViewer
{
    partial class SingleForm
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
            this.SuspendLayout();
            // 
            // SingleForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(889, 844);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "SingleForm";
            this.Text = "SingleForm";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SingleForm_FormClosing);
            this.Load += new System.EventHandler(this.SingleForm_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.SingleForm_Paint);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.SingleForm_KeyDown);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.SingleForm_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.SingleForm_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.SingleForm_MouseUp);
            this.ResumeLayout(false);

        }

        #endregion
    }
}