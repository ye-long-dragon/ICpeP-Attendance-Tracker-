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
    public partial class timeOut : UserControl
    {
        public timeOut()
        {
            InitializeComponent();
        }

        private void btnTimeOut_Click(object sender, EventArgs e)
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
                if (timeAvail.date == now && timeAvail.timeIn > DateTime.Parse("12:00 PM"))
                {
                    MessageBox.Show("You have already timed in for today.");
                    openTime = timeAvail;

                }
            }

            // Record time in

            // Call your Read method (assume it's ReadStudentByRfid for accuracy)
            var row = dbconnect.ReadStudentById(rfid);  // FIXED: Renamed variable to 'row' (Dictionary<string, object>)

            student s = null;  // Start as null

            if (row != null)
            {
                // Existing student found – map safely
                s = new student
                {
                    rfid = row.ContainsKey("rfid") && row["rfid"] != null && row["rfid"] != DBNull.Value
                           ? row["rfid"].ToString() ?? string.Empty
                           : string.Empty,

                    student_id = row.ContainsKey("id") && row["id"] != null && row["id"] != DBNull.Value
                                 ? Convert.ToInt64(row["id"])
                                 : 0L,

                    first_name = row.ContainsKey("first_name") && row["first_name"] != null && row["first_name"] != DBNull.Value
                                 ? row["first_name"].ToString() ?? string.Empty
                                 : string.Empty,

                    last_name = row.ContainsKey("last_name") && row["last_name"] != null && row["last_name"] != DBNull.Value
                                 ? row["last_name"].ToString() ?? string.Empty
                                 : string.Empty,

                    year_level = row.ContainsKey("year_level") && row["year_level"] != null && row["year_level"] != DBNull.Value
                                 ? Convert.ToInt32(row["year_level"])
                                 : 0,

                    date = DateTime.UtcNow,  // FIXED: Use current time (valid for attendance)
                    status = "Present"  // FIXED: Default to "Present" for existing students
                };

                Debug.WriteLine($"✅ Mapped existing student: ID={s.student_id}, RFID={s.rfid}, Name={s.first_name} {s.last_name}");
            }
            else
            {
                // NEW RFID: Don't create empty student – handle separately (see Step 2)
                Debug.WriteLine($"⚠️ New/unregistered RFID: {rfid}. Prompting for registration.");
                s = null;  // Or create a "guest" student if needed (see options below)
            }


            // Now use 's' (e.g., s.rfid, etc.)
            if (s != null)
            {
                // Example: Display in UI
                MessageBox.Show($"Student: {s.first_name} {s.last_name} (ID: {s.student_id})");
            }
            MessageBox.Show(s.first_name);

            s.status = "Checked Out";
            s.date = now;

            dbconnect.CreateAttendance(s);
            MessageBox.Show($"Goodbye, {s.first_name} {s.last_name}! You have successfully timed out at {now.ToString("hh:mm tt")}.");

            txtRFID.Clear();
            s = null; // Clear reference
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
