namespace NET
{
	partial class VolumeControl
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
			if(disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.tkbrVolume = new System.Windows.Forms.TrackBar();
			this.lblName = new System.Windows.Forms.Label();
			this.lblVolume = new System.Windows.Forms.Label();
			this.bindingSource1 = new System.Windows.Forms.BindingSource(this.components);
			((System.ComponentModel.ISupportInitialize)(this.tkbrVolume)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).BeginInit();
			this.SuspendLayout();
			// 
			// tkbrVolume
			// 
			this.tkbrVolume.Location = new System.Drawing.Point(39, 37);
			this.tkbrVolume.Margin = new System.Windows.Forms.Padding(10, 10, 0, 0);
			this.tkbrVolume.Maximum = 100;
			this.tkbrVolume.MaximumSize = new System.Drawing.Size(45, 134);
			this.tkbrVolume.MinimumSize = new System.Drawing.Size(45, 134);
			this.tkbrVolume.Name = "tkbrVolume";
			this.tkbrVolume.Orientation = System.Windows.Forms.Orientation.Vertical;
			this.tkbrVolume.Size = new System.Drawing.Size(45, 134);
			this.tkbrVolume.TabIndex = 0;
			this.tkbrVolume.TickFrequency = 10;
			this.tkbrVolume.TickStyle = System.Windows.Forms.TickStyle.Both;
			this.tkbrVolume.Scroll += new System.EventHandler(this.OnScroll);
			// 
			// lblName
			// 
			this.lblName.Location = new System.Drawing.Point(3, 0);
			this.lblName.Name = "lblName";
			this.lblName.Size = new System.Drawing.Size(116, 36);
			this.lblName.TabIndex = 1;
			this.lblName.Text = "{Program Name}";
			this.lblName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// lblVolume
			// 
			this.lblVolume.Location = new System.Drawing.Point(3, 171);
			this.lblVolume.Name = "lblVolume";
			this.lblVolume.Size = new System.Drawing.Size(116, 17);
			this.lblVolume.TabIndex = 2;
			this.lblVolume.Text = "0";
			this.lblVolume.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// VolumeControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.lblVolume);
			this.Controls.Add(this.lblName);
			this.Controls.Add(this.tkbrVolume);
			this.MaximumSize = new System.Drawing.Size(122, 188);
			this.MinimumSize = new System.Drawing.Size(122, 188);
			this.Name = "VolumeControl";
			this.Size = new System.Drawing.Size(122, 188);
			((System.ComponentModel.ISupportInitialize)(this.tkbrVolume)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TrackBar tkbrVolume;
		private System.Windows.Forms.BindingSource bindingSource1;
		private System.Windows.Forms.Label lblName;
		private System.Windows.Forms.Label lblVolume;
	}
}
