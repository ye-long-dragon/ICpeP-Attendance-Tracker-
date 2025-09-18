using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace ICpeP_Attendance_Tracker___Main.database
{
    public class dbconnect
    {
        public string connectionString = "postgresql://postgres:SYSDEV.MMCM.ICpeP@db.axwduoswuwxdqimchpvs.supabase.co:5432/postgres";
        private NpgsqlConnection conn;


        public void startDBConnection()
        {
            conn = new NpgsqlConnection(connectionString);
            conn.Open();
            Console.WriteLine("Database connection established successfully.");
        }

        public void stopDBConnection()
        {
            if (conn != null)
            {
                conn.Close();
                conn.Dispose();
                conn = null;
                Console.WriteLine("Database connection closed successfully.");
            }
        }



    }
}
