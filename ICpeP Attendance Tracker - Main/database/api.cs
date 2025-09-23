using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICpeP_Attendance_Tracker___Main.database
{
    public class api
    {

        public dbconnect db = new dbconnect();
        private static readonly string connectionString = db.connectionString;
        public readonly string connectionString = connectionString;



    }
}
