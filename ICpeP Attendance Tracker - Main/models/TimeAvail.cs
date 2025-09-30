using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace ICpeP_Attendance_Tracker___Main.models
{
    public class TimeAvail
    {
        [JsonPropertyName("time_in")]
        public DateTime TimeIn { get; set; }

        [JsonPropertyName("time_out")]
        public DateTime TimeOut { get; set; }

        [JsonPropertyName("date")]
        public DateTime Date { get; set; }

        // Parameterless constructor required for deserialization
        public TimeAvail() { }

        // Optional convenience constructor
        public TimeAvail(string date, string timeIn, string timeOut)
        {
            this.Date = DateTime.Parse(date);
            this.TimeIn = DateTime.Parse($"{date} {timeIn}");
            this.TimeOut = DateTime.Parse($"{date} {timeOut}");
        }
    }
}

