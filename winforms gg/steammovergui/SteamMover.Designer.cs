namespace SteamMoverGui
{
    partial class SteamMover
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
            if (threadRealSizeOnDisk != null)
            {
                threadRealSizeOnDisk.Abort();
            }
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
            this.comboBoxLeft = new System.Windows.Forms.ComboBox();
            this.buttonLeft = new System.Windows.Forms.Button();
            this.buttonRight = new System.Windows.Forms.Button();
            this.comboBoxRight = new System.Windows.Forms.ComboBox();
            this.dataGridViewLeft = new System.Windows.Forms.DataGridView();
            this.Game = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Size = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewRight = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewLeft)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewRight)).BeginInit();
            this.SuspendLayout();
            // 
            // comboBoxLeft
            // 
            this.comboBoxLeft.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxLeft.FormattingEnabled = true;
            this.comboBoxLeft.Location = new System.Drawing.Point(12, 103);
            this.comboBoxLeft.Name = "comboBoxLeft";
            this.comboBoxLeft.Size = new System.Drawing.Size(302, 21);
            this.comboBoxLeft.TabIndex = 0;
            this.comboBoxLeft.SelectedIndexChanged += new System.EventHandler(this.comboBoxLeft_SelectedIndexChanged);
            // 
            // buttonLeft
            // 
            this.buttonLeft.ForeColor = System.Drawing.SystemColors.ControlText;
            this.buttonLeft.Location = new System.Drawing.Point(334, 236);
            this.buttonLeft.Name = "buttonLeft";
            this.buttonLeft.Size = new System.Drawing.Size(75, 23);
            this.buttonLeft.TabIndex = 4;
            this.buttonLeft.Text = "<--";
            this.buttonLeft.UseVisualStyleBackColor = true;
            this.buttonLeft.Click += new System.EventHandler(this.buttonLeft_Click);
            // 
            // buttonRight
            // 
            this.buttonRight.Location = new System.Drawing.Point(334, 278);
            this.buttonRight.Name = "buttonRight";
            this.buttonRight.Size = new System.Drawing.Size(75, 23);
            this.buttonRight.TabIndex = 5;
            this.buttonRight.Text = "-->";
            this.buttonRight.UseVisualStyleBackColor = true;
            this.buttonRight.Click += new System.EventHandler(this.buttonRight_Click);
            // 
            // comboBoxRight
            // 
            this.comboBoxRight.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxRight.FormattingEnabled = true;
            this.comboBoxRight.Location = new System.Drawing.Point(424, 103);
            this.comboBoxRight.Name = "comboBoxRight";
            this.comboBoxRight.Size = new System.Drawing.Size(280, 21);
            this.comboBoxRight.TabIndex = 6;
            this.comboBoxRight.SelectedIndexChanged += new System.EventHandler(this.comboBoxRight_SelectedIndexChanged);
            // 
            // dataGridViewLeft
            // 
            this.dataGridViewLeft.AllowUserToAddRows = false;
            this.dataGridViewLeft.AllowUserToDeleteRows = false;
            this.dataGridViewLeft.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewLeft.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Game,
            this.Size});
            this.dataGridViewLeft.Location = new System.Drawing.Point(12, 130);
            this.dataGridViewLeft.Name = "dataGridViewLeft";
            this.dataGridViewLeft.ReadOnly = true;
            this.dataGridViewLeft.Size = new System.Drawing.Size(302, 301);
            this.dataGridViewLeft.TabIndex = 7;
            // 
            // Game
            // 
            this.Game.HeaderText = "Game";
            this.Game.Name = "Game";
            this.Game.ReadOnly = true;
            // 
            // Size
            // 
            this.Size.HeaderText = "Size";
            this.Size.Name = "Size";
            this.Size.ReadOnly = true;
            // 
            // dataGridViewRight
            // 
            this.dataGridViewRight.AllowUserToAddRows = false;
            this.dataGridViewRight.AllowUserToDeleteRows = false;
            this.dataGridViewRight.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewRight.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn1,
            this.dataGridViewTextBoxColumn2});
            this.dataGridViewRight.Location = new System.Drawing.Point(424, 130);
            this.dataGridViewRight.Name = "dataGridViewRight";
            this.dataGridViewRight.ReadOnly = true;
            this.dataGridViewRight.Size = new System.Drawing.Size(280, 301);
            this.dataGridViewRight.TabIndex = 7;
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.HeaderText = "Game";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.HeaderText = "Size";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.ReadOnly = true;
            // 
            // SteamMover
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Highlight;
            this.ClientSize = new System.Drawing.Size(740, 469);
            this.Controls.Add(this.dataGridViewRight);
            this.Controls.Add(this.dataGridViewLeft);
            this.Controls.Add(this.comboBoxRight);
            this.Controls.Add(this.buttonRight);
            this.Controls.Add(this.buttonLeft);
            this.Controls.Add(this.comboBoxLeft);
            this.Name = "SteamMover";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewLeft)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewRight)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox comboBoxLeft;
        private System.Windows.Forms.Button buttonLeft;
        private System.Windows.Forms.Button buttonRight;
        private System.Windows.Forms.ComboBox comboBoxRight;
        private System.Windows.Forms.DataGridView dataGridViewLeft;
        private System.Windows.Forms.DataGridViewTextBoxColumn Game;
        private System.Windows.Forms.DataGridViewTextBoxColumn Size;
        private System.Windows.Forms.DataGridView dataGridViewRight;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
    }
}

