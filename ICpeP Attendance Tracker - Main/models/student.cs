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
        public string rfid { get; set; }
        public long student_id { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public int year_level { get; set; }
        public DateTime date { get; set; }
        public string status { get; set; }

        public student(string RFID, long StudentId, string FirstName, string LastName, int YearLevel)
        {
            this.rfid = RFID;
            this.student_id = StudentId;
            this.first_name = FirstName;
            this.last_name = LastName;
            this.year_level = YearLevel;
            this.status = "Registered";
        }

        public student()
        {
        }

        public void checkIn(string RFID)
        {
            if (RFID != this.rfid)
            {
                throw new ArgumentException("RFID does not match.");
            }
            this.date = DateTime.Now;
            this.status = "Checked In";
        }

        public void checkOut(string RFID)
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
