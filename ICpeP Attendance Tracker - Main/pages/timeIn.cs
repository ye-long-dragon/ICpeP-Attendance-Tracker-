using ICpeP_Attendance_Tracker___Main.models;
using ICpeP_Attendance_Tracker___Main.database;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Globalization;

namespace ICpeP_Attendance_Tracker___Main.pages
{
    public partial class timeIn : UserControl
    {
        public timeIn()
        {
            InitializeComponent();
        }

        private async void btnTimeIn_Click(object sender, EventArgs e)
        {
            string rfid = txtRFID.Text.Trim();
            if (string.IsNullOrEmpty(rfid))
            {
                MessageBox.Show("Please enter an RFID.");
                return;
            }

            try
            {
                // Get student by RFID (returns a student object)
                var student = await DbConnect.ReadStudentByRfidAsync(rfid);
                if (student == null)
                {
                    MessageBox.Show("RFID not recognized. Please register first.");
                    return;
                }

                DateTime today = DateTime.Now.Date;

                

                // Prepare attendance record
                student.status = "Checked In";
                student.date = DateTime.Now;

                bool created = await DbConnect.CreateAttendanceAsync(student);
                if (created)
                {
                    MessageBox.Show($"Welcome, {student.first_name} {student.last_name}! You have successfully timed in at {student.date.ToString("hh:mm tt")}");
                }
                else
                {
                    MessageBox.Show("Failed to record time in. Please try again.");
                }

                txtRFID.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during time in: {ex.Message}");
            }
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            if (this.ParentForm is AttendanceTracker form)
            {
                form.ShowMainPage();  // Switches back to main
            }
        }
    }
}
