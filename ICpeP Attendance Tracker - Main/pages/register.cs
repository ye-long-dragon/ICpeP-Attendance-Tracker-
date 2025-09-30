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

        private async void btnRegister_Click(object sender, EventArgs e)
        {
            // Validate inputs
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

            // Parse student ID safely
            if (!long.TryParse(txtStudentId.Text, out long studentid))
            {
                MessageBox.Show("Student ID must be a valid number");
                return;
            }

            string firstName = txtFirstName.Text.Trim();
            string lastName = txtLastName.Text.Trim();
            string rfid = txtRFID.Text.Trim();
            int yearLevel = cmbYearLevel.SelectedIndex;  // Assuming year levels start at 1

            // Create student object
            student student = new student(rfid, studentid, firstName, lastName, yearLevel);

            try
            {
                // Check if student with this ID already exists (async)
                var existingStudent = await dbconnect.ReadStudentByIdAsync(studentid);
                if (existingStudent != null)
                {
                    MessageBox.Show($"Student ID {studentid} already exists. Use a different ID or update the existing record.",
                                "Duplicate Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Register student (async)
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
                Debug.WriteLine($"❌ Error during registration: {ex.Message}");
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Helper: Clear form inputs
        private void ClearForm()
        {
            txtRFID?.Clear();
            txtStudentId?.Clear();
            txtFirstName?.Clear();
            txtLastName?.Clear();
            cmbYearLevel.SelectedIndex = 0;  // Reset to "Select Year" or default
            txtStudentId?.Focus();  // Focus back to first field
        }
    }
}
