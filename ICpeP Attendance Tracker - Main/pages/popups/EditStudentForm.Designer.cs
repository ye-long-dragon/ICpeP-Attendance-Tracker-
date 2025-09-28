namespace ICpeP_Attendance_Tracker___Main.pages.popups
{
    partial class EditStudentForm
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
            this.guna2Panel2 = new Guna.UI2.WinForms.Guna2Panel();
            this.welcome = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.btnUpdate = new Guna.UI2.WinForms.Guna2Button();
            this.lblYearLevel = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.cmbYearLevel = new Guna.UI2.WinForms.Guna2ComboBox();
            this.lblLastName = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.txtLastName = new Guna.UI2.WinForms.Guna2TextBox();
            this.lblFirstName = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.txtFirstName = new Guna.UI2.WinForms.Guna2TextBox();
            this.lblStudentId = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.txtStudentId = new Guna.UI2.WinForms.Guna2TextBox();
            this.lblRFID = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.txtRFID = new Guna.UI2.WinForms.Guna2TextBox();
            this.regis = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.guna2Panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // guna2Panel2
            // 
            this.guna2Panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(6)))), ((int)(((byte)(30)))), ((int)(((byte)(54)))));
            this.guna2Panel2.Controls.Add(this.btnUpdate);
            this.guna2Panel2.Controls.Add(this.lblYearLevel);
            this.guna2Panel2.Controls.Add(this.cmbYearLevel);
            this.guna2Panel2.Controls.Add(this.lblLastName);
            this.guna2Panel2.Controls.Add(this.txtLastName);
            this.guna2Panel2.Controls.Add(this.lblFirstName);
            this.guna2Panel2.Controls.Add(this.txtFirstName);
            this.guna2Panel2.Controls.Add(this.lblStudentId);
            this.guna2Panel2.Controls.Add(this.txtStudentId);
            this.guna2Panel2.Controls.Add(this.lblRFID);
            this.guna2Panel2.Controls.Add(this.txtRFID);
            this.guna2Panel2.Controls.Add(this.regis);
            this.guna2Panel2.Controls.Add(this.welcome);
            this.guna2Panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.guna2Panel2.Location = new System.Drawing.Point(0, 0);
            this.guna2Panel2.Name = "guna2Panel2";
            this.guna2Panel2.Size = new System.Drawing.Size(1215, 552);
            this.guna2Panel2.TabIndex = 4;
            // 
            // welcome
            // 
            this.welcome.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.welcome.BackColor = System.Drawing.Color.Transparent;
            this.welcome.Font = new System.Drawing.Font("Bodoni MT", 18F, System.Drawing.FontStyle.Bold);
            this.welcome.ForeColor = System.Drawing.Color.White;
            this.welcome.Location = new System.Drawing.Point(323, 67);
            this.welcome.Name = "welcome";
            this.welcome.Size = new System.Drawing.Size(625, 38);
            this.welcome.TabIndex = 0;
            this.welcome.Text = "Welcome to the MMCM.ICpeP Attendance Tracker";
            this.welcome.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnUpdate
            // 
            this.btnUpdate.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnUpdate.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnUpdate.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnUpdate.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnUpdate.FillColor = System.Drawing.Color.White;
            this.btnUpdate.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Bold);
            this.btnUpdate.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(6)))), ((int)(((byte)(30)))), ((int)(((byte)(54)))));
            this.btnUpdate.ImageSize = new System.Drawing.Size(40, 40);
            this.btnUpdate.Location = new System.Drawing.Point(455, 355);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(300, 45);
            this.btnUpdate.TabIndex = 16;
            this.btnUpdate.Text = "Update Student Info";
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            // 
            // lblYearLevel
            // 
            this.lblYearLevel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblYearLevel.BackColor = System.Drawing.Color.Transparent;
            this.lblYearLevel.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Bold);
            this.lblYearLevel.ForeColor = System.Drawing.Color.White;
            this.lblYearLevel.Location = new System.Drawing.Point(436, 298);
            this.lblYearLevel.Name = "lblYearLevel";
            this.lblYearLevel.Size = new System.Drawing.Size(135, 31);
            this.lblYearLevel.TabIndex = 22;
            this.lblYearLevel.Text = "Year Level:";
            this.lblYearLevel.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // cmbYearLevel
            // 
            this.cmbYearLevel.BackColor = System.Drawing.Color.Transparent;
            this.cmbYearLevel.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbYearLevel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbYearLevel.FocusedColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.cmbYearLevel.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.cmbYearLevel.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cmbYearLevel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(112)))));
            this.cmbYearLevel.ItemHeight = 30;
            this.cmbYearLevel.Items.AddRange(new object[] {
            "1st Year",
            "2nd Year",
            "3rd Year",
            "4th Year"});
            this.cmbYearLevel.Location = new System.Drawing.Point(627, 298);
            this.cmbYearLevel.Name = "cmbYearLevel";
            this.cmbYearLevel.Size = new System.Drawing.Size(140, 36);
            this.cmbYearLevel.TabIndex = 21;
            // 
            // lblLastName
            // 
            this.lblLastName.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblLastName.BackColor = System.Drawing.Color.Transparent;
            this.lblLastName.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Bold);
            this.lblLastName.ForeColor = System.Drawing.Color.White;
            this.lblLastName.Location = new System.Drawing.Point(608, 246);
            this.lblLastName.Name = "lblLastName";
            this.lblLastName.Size = new System.Drawing.Size(134, 31);
            this.lblLastName.TabIndex = 20;
            this.lblLastName.Text = "Last Name:";
            this.lblLastName.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txtLastName
            // 
            this.txtLastName.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtLastName.DefaultText = "";
            this.txtLastName.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.txtLastName.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.txtLastName.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtLastName.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtLastName.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtLastName.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtLastName.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtLastName.Location = new System.Drawing.Point(798, 246);
            this.txtLastName.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtLastName.Name = "txtLastName";
            this.txtLastName.PlaceholderText = "";
            this.txtLastName.SelectedText = "";
            this.txtLastName.Size = new System.Drawing.Size(178, 31);
            this.txtLastName.TabIndex = 19;
            // 
            // lblFirstName
            // 
            this.lblFirstName.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblFirstName.BackColor = System.Drawing.Color.Transparent;
            this.lblFirstName.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Bold);
            this.lblFirstName.ForeColor = System.Drawing.Color.White;
            this.lblFirstName.Location = new System.Drawing.Point(608, 198);
            this.lblFirstName.Name = "lblFirstName";
            this.lblFirstName.Size = new System.Drawing.Size(138, 31);
            this.lblFirstName.TabIndex = 18;
            this.lblFirstName.Text = "First Name:";
            this.lblFirstName.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txtFirstName
            // 
            this.txtFirstName.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtFirstName.DefaultText = "";
            this.txtFirstName.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.txtFirstName.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.txtFirstName.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtFirstName.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtFirstName.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtFirstName.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtFirstName.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtFirstName.Location = new System.Drawing.Point(798, 198);
            this.txtFirstName.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtFirstName.Name = "txtFirstName";
            this.txtFirstName.PlaceholderText = "";
            this.txtFirstName.SelectedText = "";
            this.txtFirstName.Size = new System.Drawing.Size(178, 31);
            this.txtFirstName.TabIndex = 17;
            // 
            // lblStudentId
            // 
            this.lblStudentId.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblStudentId.BackColor = System.Drawing.Color.Transparent;
            this.lblStudentId.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Bold);
            this.lblStudentId.ForeColor = System.Drawing.Color.White;
            this.lblStudentId.Location = new System.Drawing.Point(239, 246);
            this.lblStudentId.Name = "lblStudentId";
            this.lblStudentId.Size = new System.Drawing.Size(131, 31);
            this.lblStudentId.TabIndex = 15;
            this.lblStudentId.Text = "Student ID:";
            this.lblStudentId.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txtStudentId
            // 
            this.txtStudentId.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtStudentId.DefaultText = "";
            this.txtStudentId.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.txtStudentId.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.txtStudentId.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtStudentId.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtStudentId.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtStudentId.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtStudentId.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtStudentId.Location = new System.Drawing.Point(419, 246);
            this.txtStudentId.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtStudentId.Name = "txtStudentId";
            this.txtStudentId.PlaceholderText = "";
            this.txtStudentId.SelectedText = "";
            this.txtStudentId.Size = new System.Drawing.Size(165, 31);
            this.txtStudentId.TabIndex = 14;
            // 
            // lblRFID
            // 
            this.lblRFID.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblRFID.BackColor = System.Drawing.Color.Transparent;
            this.lblRFID.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Bold);
            this.lblRFID.ForeColor = System.Drawing.Color.White;
            this.lblRFID.Location = new System.Drawing.Point(239, 198);
            this.lblRFID.Name = "lblRFID";
            this.lblRFID.Size = new System.Drawing.Size(90, 31);
            this.lblRFID.TabIndex = 13;
            this.lblRFID.Text = "RFID# :";
            this.lblRFID.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txtRFID
            // 
            this.txtRFID.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtRFID.DefaultText = "";
            this.txtRFID.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.txtRFID.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.txtRFID.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtRFID.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtRFID.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtRFID.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtRFID.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtRFID.Location = new System.Drawing.Point(378, 198);
            this.txtRFID.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtRFID.Name = "txtRFID";
            this.txtRFID.PlaceholderText = "";
            this.txtRFID.SelectedText = "";
            this.txtRFID.Size = new System.Drawing.Size(206, 31);
            this.txtRFID.TabIndex = 12;
            // 
            // regis
            // 
            this.regis.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.regis.BackColor = System.Drawing.Color.Transparent;
            this.regis.Font = new System.Drawing.Font("Bodoni MT", 18F, System.Drawing.FontStyle.Bold);
            this.regis.ForeColor = System.Drawing.Color.White;
            this.regis.Location = new System.Drawing.Point(490, 125);
            this.regis.Name = "regis";
            this.regis.Size = new System.Drawing.Size(256, 38);
            this.regis.TabIndex = 11;
            this.regis.Text = "Student Information";
            this.regis.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // EditStudentForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1215, 552);
            this.Controls.Add(this.guna2Panel2);
            this.Name = "EditStudentForm";
            this.Text = "EditStudentForm";
            this.guna2Panel2.ResumeLayout(false);
            this.guna2Panel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Guna.UI2.WinForms.Guna2Panel guna2Panel2;
        private Guna.UI2.WinForms.Guna2HtmlLabel welcome;
        private Guna.UI2.WinForms.Guna2Button btnUpdate;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblYearLevel;
        private Guna.UI2.WinForms.Guna2ComboBox cmbYearLevel;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblLastName;
        private Guna.UI2.WinForms.Guna2TextBox txtLastName;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblFirstName;
        private Guna.UI2.WinForms.Guna2TextBox txtFirstName;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblStudentId;
        private Guna.UI2.WinForms.Guna2TextBox txtStudentId;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblRFID;
        private Guna.UI2.WinForms.Guna2TextBox txtRFID;
        private Guna.UI2.WinForms.Guna2HtmlLabel regis;
    }
}