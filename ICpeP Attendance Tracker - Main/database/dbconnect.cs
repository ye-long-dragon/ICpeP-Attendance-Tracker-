using System;
using System.Data;
using System.Diagnostics;
using Npgsql;

namespace ICpeP_Attendance_Tracker___Main.database
{
    public class dbconnect
    {
        // Static fields: Accessible from static methods
        private static NpgsqlConnection conn;
        private static readonly string connectionString;  // readonly for immutability

        // Static constructor: Runs once to init connectionString (securely)
        static dbconnect()
        {
            // Your URI: postgresql://postgres:SYSDEV.MMCM.ICpeP@db.axwduoswuwxdqimchpvs.supabase.co:5432/postgres
            // Convert to Npgsql format (Host;Port; etc.) – Npgsql parses URIs, but this is explicit/safer
            connectionString = "Host=db.axwduoswuwxdqimchpvs.supabase.co;" +
                               "Port=5432;" +
                               "Database=postgres;" +
                               "Username=postgres;" +
                               "Password=SYSDEV.MMCM.ICpeP;" +  // WARNING: Hardcoded – see Step 2 for secure fix!
                               "SslMode=Require;" +  // Required for Supabase
                               "Pooling=true;" +     // Enable connection pooling for performance
                               "Timeout=30;";        // Connection timeout

            // Alternative: If you prefer URI format (Npgsql supports it directly)
            // connectionString = "postgresql://postgres:SYSDEV.MMCM.ICpeP@db.axwduoswuwxdqimchpvs.supabase.co:5432/postgres?sslmode=require";
        }

        public static bool StartConnection()
        {
            try
            {
                if (conn != null && conn.State == ConnectionState.Open)
                {
                    Debug.WriteLine("DB connection already open.");
                    return true;
                }

                conn = new NpgsqlConnection(connectionString);
                conn.Open();
                Debug.WriteLine("Supabase connection established successfully.");
                return true;
            }
            catch (NpgsqlException ex)
            {
                Debug.WriteLine($"Npgsql Error: {ex.Message}");  // DB-specific
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"DB Connection Error: {ex.Message}");
                return false;
            }
        }

        public static void StopConnection()
        {
            if (conn != null)
            {
                try
                {
                    if (conn.State == ConnectionState.Open)
                    {
                        conn.Close();
                    }
                    conn.Dispose();
                    Debug.WriteLine("Supabase connection closed successfully.");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error closing DB: {ex.Message}");
                }
                finally
                {
                    conn = null;
                }
            }
        }

        public static NpgsqlConnection GetConnection()
        {
            if (conn == null || conn.State != ConnectionState.Open)
            {
                if (!StartConnection())
                {
                    throw new InvalidOperationException("Failed to establish DB connection. Check credentials.");
                }
            }
            return conn;
        }

        // Short-lived connection factory (recommended for queries – avoids persistent issues)
        public static NpgsqlConnection CreateShortLivedConnection()
        {
            return new NpgsqlConnection(connectionString);
        }

        // Optional: Test connection (call this in your app startup)
        public static bool TestConnection()
        {
            try
            {
                using (var testConn = CreateShortLivedConnection())
                {
                    testConn.Open();
                    using (var cmd = new NpgsqlCommand("SELECT 1", testConn))
                    {
                        cmd.ExecuteScalar();
                    }
                    Debug.WriteLine("Connection test successful.");
                    return true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Connection test failed: {ex.Message}");
                return false;
            }
        }
    }
}
