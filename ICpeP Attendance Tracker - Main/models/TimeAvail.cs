using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICpeP_Attendance_Tracker___Main.models
{
    public class TimeAvail
    {
        public DateTime timeIn { get; set; }

        public DateTime timeOut { get; set; }
        public DateTime date { get; set; }

        public TimeAvail(string Date,string timeIn, string timeOut)
        {
            //add date to timein and timeout
            this.timeIn = DateTime.Parse(Date + " " + timeIn);
            this.timeOut = DateTime.Parse(Date + " " + timeOut);
            this.date = DateTime.Parse(Date);
        }

    }
}
