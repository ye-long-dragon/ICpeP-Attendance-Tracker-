using System;
using System.Collections.Generic;
using System.Deployment.Application;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICpeP_Attendance_Tracker___Main.models
{
    public class student
    {
        public int rfid { get; set; }
        public int student_id { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public int year_level { get; set; }
        public DateTime date { get; set; }
        public string status { get; set; }

        public student(int RFID, int StudentId, string FirstName, string LastName, int YearLevel)
        {
            this.rfid = RFID;
            this.student_id = StudentId;
            this.first_name = FirstName;
            this.last_name = LastName;
            this.year_level = YearLevel;
            this.status = "Registered";
        }

        public void checkIn(int RFID)
        {
            if (RFID != this.rfid)
            {
                throw new ArgumentException("RFID does not match.");
            }
            this.date = DateTime.Now;
            this.status = "Checked In";
        }

        public void checkOut(int RFID)
        {
            if (RFID != this.rfid)
            {
                throw new ArgumentException("RFID does not match.");
            }
            this.date = DateTime.Now;
            this.status = "Checked Out";
        }
    }
}
