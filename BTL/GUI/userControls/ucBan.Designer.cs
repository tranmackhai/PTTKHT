
namespace BTL
{
    partial class ucBan
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnInfoTable = new Guna.UI2.WinForms.Guna2Button();
            this.SuspendLayout();
            // 
            // btnInfoTable
            // 
            this.btnInfoTable.Animated = true;
            this.btnInfoTable.CheckedState.Parent = this.btnInfoTable;
            this.btnInfoTable.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnInfoTable.CustomImages.Parent = this.btnInfoTable;
            this.btnInfoTable.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnInfoTable.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnInfoTable.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnInfoTable.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnInfoTable.DisabledState.Parent = this.btnInfoTable;
            this.btnInfoTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnInfoTable.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnInfoTable.ForeColor = System.Drawing.Color.White;
            this.btnInfoTable.HoverState.Parent = this.btnInfoTable;
            this.btnInfoTable.Location = new System.Drawing.Point(0, 0);
            this.btnInfoTable.Name = "btnInfoTable";
            this.btnInfoTable.ShadowDecoration.Parent = this.btnInfoTable;
            this.btnInfoTable.Size = new System.Drawing.Size(125, 80);
            this.btnInfoTable.TabIndex = 2;
            this.btnInfoTable.Text = "guna2Button1";
            this.btnInfoTable.Click += new System.EventHandler(this.btnInfoTable_Click);
            // 
            // ucBan
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.btnInfoTable);
            this.Margin = new System.Windows.Forms.Padding(10);
            this.Name = "ucBan";
            this.Size = new System.Drawing.Size(125, 80);
            this.Load += new System.EventHandler(this.ucBan_Load);
            this.ResumeLayout(false);

        }

        #endregion
        private Guna.UI2.WinForms.Guna2Button btnInfoTable;
    }
}
