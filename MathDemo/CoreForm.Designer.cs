
namespace MathDemo
{
	partial class CoreForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CoreForm));
            this.corePanel = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.corePanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // corePanel
            // 
            this.corePanel.Controls.Add(this.label1);
            this.corePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.corePanel.Location = new System.Drawing.Point(0, 0);
            this.corePanel.Name = "corePanel";
            this.corePanel.Size = new System.Drawing.Size(1792, 1362);
            this.corePanel.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.SystemColors.Window;
            this.label1.Location = new System.Drawing.Point(3, 1072);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(286, 300);
            this.label1.TabIndex = 2;
            this.label1.Text = resources.GetString("label1.Text");
            // 
            // CoreForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ClientSize = new System.Drawing.Size(1792, 1362);
            this.Controls.Add(this.corePanel);
            this.Name = "CoreForm";
            this.Text = "Form1";
            this.corePanel.ResumeLayout(false);
            this.corePanel.PerformLayout();
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Panel corePanel;
        private System.Windows.Forms.Label label1;
    }
}

