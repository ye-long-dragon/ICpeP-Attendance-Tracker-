using ICpeP_Attendance_Tracker___Main.database;
using ICpeP_Attendance_Tracker___Main.models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ICpeP_Attendance_Tracker___Main.pages
{
    public partial class timeOut : UserControl
    {
        public timeOut()
        {
            InitializeComponent();
        }

        private async void btnTimeOut_Click(object sender, EventArgs e)
        {
            string rfid = txtRFID.Text.Trim();

            if (string.IsNullOrEmpty(rfid))
            {
                MessageBox.Show("Please enter an RFID.");
                return;
            }

            try
            {
                // Assume you have this method implemented to get a student by RFID:
                var studentRecord = await dbconnect.ReadStudentByRfidAsync(rfid);

                if (studentRecord == null)
                {
                    MessageBox.Show("RFID not recognized. Please register first.");
                    return;
                }

                student s = new student
                {
                    rfid = studentRecord.rfid ?? string.Empty,
                    student_id = studentRecord.student_id,
                    first_name = studentRecord.first_name ?? string.Empty,
                    last_name = studentRecord.last_name ?? string.Empty,
                    year_level = studentRecord.year_level
                };


                DateTime today = DateTime.Now.Date;

                // Get all attendance records for this student today
                var timeAvails = await dbconnect.ReadAllAttendanceAsync();


                //check what attendance is 
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during time out: {ex.Message}");
                Debug.WriteLine($"Exception in btnTimeOut_Click: {ex}");
            }
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            if (this.ParentForm is AttendanceTracker form)
            {
                form.ShowMainPage();  // Switches back to main page
            }
        }
    }
}
