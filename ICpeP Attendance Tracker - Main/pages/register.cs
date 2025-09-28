using ICpeP_Attendance_Tracker___Main.database;
using ICpeP_Attendance_Tracker___Main.models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ICpeP_Attendance_Tracker___Main.pages
{
    public partial class register : UserControl
    {
        public string firstName;
        public string lastName;
        public long studentid;
        public string rfid;
        public int yearLevel;

        public register()
        {
            InitializeComponent();
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            if (this.ParentForm is AttendanceTracker form)
            {
                form.ShowMainPage();  // Switches back to main
            }
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            //check input
            if (string.IsNullOrWhiteSpace(txtFirstName.Text))
            {
                MessageBox.Show("Input First Name");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtLastName.Text))
            {
                MessageBox.Show("Input Last Name");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtStudentId.Text))
            {
                MessageBox.Show("Input Student ID");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtRFID.Text))
            {
                MessageBox.Show("Input RFID");
                return;
            }

            if (cmbYearLevel.SelectedIndex <= -1) // assuming 0 = "Select Year"
            {
                MessageBox.Show("Select Year Level");
                return;
            }



            firstName = txtFirstName.Text;
            lastName = txtLastName.Text;
            studentid = long.Parse(txtStudentId.Text);
            rfid = txtRFID.Text;
            yearLevel = cmbYearLevel.SelectedIndex;

            student student = new student(rfid, studentid, firstName, lastName, yearLevel);

            
                var existingStudent = dbconnect.ReadStudentById(rfid);
                if (existingStudent != null)
                {
                    MessageBox.Show($"Student ID {studentid} already exists. Use a different ID or update the existing record.",
                                "Duplicate Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                    dbconnect.RegisterStudent(student);
                    MessageBox.Show($"Student '{firstName} {lastName}' (ID: {studentid}) registered successfully!",
                                  "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Clear form for next entry
                    ClearForm();

                    // Optional: Log success
                    Debug.WriteLine($"✅ Student registered: ID {studentid}");
                
           
           


        

        }

        // Helper: Clear form inputs
        private void ClearForm()
        {
            txtRFID?.Clear();
            txtStudentId?.Clear();
            txtFirstName?.Clear();
            txtLastName?.Clear();
            cmbYearLevel.SelectedIndex=0;
            txtStudentId?.Focus();  // Focus back to first field
        }
    }
}
