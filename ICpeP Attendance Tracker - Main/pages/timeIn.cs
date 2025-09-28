using ICpeP_Attendance_Tracker___Main.database;
using ICpeP_Attendance_Tracker___Main.models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ICpeP_Attendance_Tracker___Main.pages
{
    public partial class timeIn : UserControl
    {
        public timeIn()
        {
            InitializeComponent();
        }

        private void btnTimeIn_Click(object sender, EventArgs e)
        {
            string rfid = txtRFID.Text.Trim();
            if (string.IsNullOrEmpty(rfid))
            {
                MessageBox.Show("Please enter an RFID.");
                return;
            }

            // Look into each student for same rfid
            var student = dbconnect.ReadStudentById(rfid);
            if (student == null)
            {
                MessageBox.Show("RFID not recognized. Please register first.");
                return;
            }

            //check timeAvail for existing time in today
            DateTime now = DateTime.Now;
            var timeAvails = dbconnect.ReadAllTimeIn();
            TimeAvail openTime;
            

            foreach (TimeAvail timeAvail in timeAvails)
            {
                //change to DateTime


                if (now == DateTime.MinValue)
                {
                    MessageBox.Show("Date is empty.");
                    return;
                }
                //check if time avail is for today and the time in is in the morning
                if (timeAvail.date == now && timeAvail.timeIn < DateTime.Parse("12:00 PM"))
                {
                    MessageBox.Show("You have already timed in for today.");
                    openTime = timeAvail;
                    
                }
            }

            // Record time in
            var s = new student
            {
                rfid = student.ContainsKey("rfid") && student["rfid"] != null ? student["rfid"].ToString() : string.Empty,
                student_id = student.ContainsKey("id") && student["id"] != null ? (long)student["id"] : 0L,  // Map "id" to "student_id"
                first_name = student.ContainsKey("first_name") && student["first_name"] != null ? student["first_name"].ToString() : string.Empty,
                last_name = student.ContainsKey("last_name") && student["last_name"] != null ? student["last_name"].ToString() : string.Empty,
                year_level = student.ContainsKey("year_level") && student["year_level"] != null ? (int)student["year_level"] : 0,
            };

            s.status = "Checked In";
            s.date = now;

            dbconnect.CreateAttendance(s);
            MessageBox.Show($"Welcome, {s.first_name} {s.last_name}! You have successfully timed in at {now.ToString("hh:mm tt")}.");

            txtRFID.Clear();


        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            if (this.ParentForm is AttendanceTracker form)
            {
                form.ShowMainPage();  // Switches back to main
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    }
}
