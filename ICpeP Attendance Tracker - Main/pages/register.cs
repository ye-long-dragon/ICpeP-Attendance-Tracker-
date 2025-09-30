using ICpeP_Attendance_Tracker___Main.models;
using ICpeP_Attendance_Tracker___Main.database;
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

        // Make event handler async
        private async void btnRegister_Click(object sender, EventArgs e)
        {
            // Check input
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

            if (cmbYearLevel.SelectedIndex <= 0) // Assuming index 0 is "Select Year"
            {
                MessageBox.Show("Select Year Level");
                return;
            }

            firstName = txtFirstName.Text;
            lastName = txtLastName.Text;

            if (!long.TryParse(txtStudentId.Text, out studentid))
            {
                MessageBox.Show("Student ID must be a valid number");
                return;
            }

            rfid = txtRFID.Text;
            yearLevel = cmbYearLevel.SelectedIndex;

            student student = new student(rfid, studentid, firstName, lastName, yearLevel);

            try
            {
                // Check if student already exists by student ID (long)
                var existingStudent = await dbconnect.ReadStudentByIdAsync(studentid);
                if (existingStudent != null)
                {
                    MessageBox.Show($"Student ID {studentid} already exists. Use a different ID or update the existing record.",
                                "Duplicate Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Register new student asynchronously
                bool success = await dbconnect.CreateStudentAsync(student);
                if (success)
                {
                    MessageBox.Show($"Student '{firstName} {lastName}' (ID: {studentid}) registered successfully!",
                                  "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    ClearForm();

                    Debug.WriteLine($"✅ Student registered: ID {studentid}");
                }
                else
                {
                    MessageBox.Show("Failed to register student. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Debug.WriteLine($"❌ Exception in btnRegister_Click: {ex}");
            }
        }

        // Helper: Clear form inputs
        private void ClearForm()
        {
            txtRFID?.Clear();
            txtStudentId?.Clear();
            txtFirstName?.Clear();
            txtLastName?.Clear();
            cmbYearLevel.SelectedIndex = 0;
            txtStudentId?.Focus();  // Focus back to first field
        }
    }
}
