
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
			this.corePanel = new System.Windows.Forms.Panel();
			this.SuspendLayout();
			// 
			// corePanel
			// 
			this.corePanel.Location = new System.Drawing.Point(0, 65);
			this.corePanel.Name = "corePanel";
			this.corePanel.Size = new System.Drawing.Size(1792, 1071);
			this.corePanel.TabIndex = 0;
			// 
			// CoreForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
			this.ClientSize = new System.Drawing.Size(1792, 1137);
			this.Controls.Add(this.corePanel);
			this.Name = "CoreForm";
			this.Text = "Form1";
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Panel corePanel;
	}
}

