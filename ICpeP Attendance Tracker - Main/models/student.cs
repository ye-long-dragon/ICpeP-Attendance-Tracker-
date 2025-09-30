using System;
using System.Collections.Generic;
using System.Deployment.Application;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace ICpeP_Attendance_Tracker___Main.models
{
    

    public class student
    {
        [JsonPropertyName("id")]
        public long student_id { get; set; }

        [JsonPropertyName("rfid")]
        public string rfid { get; set; }

        [JsonPropertyName("first_name")]
        public string first_name { get; set; }

        [JsonPropertyName("last_name")]
        public string last_name { get; set; }

        [JsonPropertyName("year_level")]
        public int year_level { get; set; }
    }

}
