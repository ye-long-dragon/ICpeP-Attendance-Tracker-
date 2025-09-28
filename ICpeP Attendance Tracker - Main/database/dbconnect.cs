using System;
using System.Data;
using System.Diagnostics;
using Npgsql;
using System.Collections.Generic;
using ICpeP_Attendance_Tracker___Main.models;
using System.Windows.Forms;
using System.Threading.Tasks;  // For returning lists of data

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
                               "Password=SYSDEV.MMCM.ICpeP;" +  // WARNING: Hardcoded – see notes below for secure fix!
                               "SslMode=Require;" +  // Required for Supabase
                               "Trust Server Certificate=true;" + // For self-signed certs (Supabase)
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
            MessageBox.Show("Connection Successfully Established");
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

        // =============================================================================
        // CRUD OPERATIONS FOR ATTENDANCE TABLE
        // =============================================================================
        // Assumptions:
        // - Table: 'attendance' with columns: id (SERIAL PRIMARY KEY), student_id (INTEGER), date (DATE), status (VARCHAR(50) e.g., 'Present', 'Absent')
        // - If your schema differs, adjust the SQL queries and parameters accordingly.
        // - All methods use parameterized queries to prevent SQL injection.
        // - Uses short-lived connections for safety (avoids holding connections open).
        // - Returns bool for success/failure on C/U/D; returns data for R.
        // - Error handling logs to Debug; you can extend to throw exceptions or return custom errors.


        //=========================================================================================
        //==================================ATTENDANCE=============================================
        //=========================================================================================
        // CREATE: Insert a new attendance record
        public static bool CreateAttendance(student student)
        {
            try
            {
                using (var connection = CreateShortLivedConnection())
                {
                    connection.Open();

                    using (var command = new NpgsqlCommand(
                        @"INSERT INTO students_attendance (student_id, rfid, date, status) 
                  VALUES (@student_id, @rfid, @date, @status)", connection))
                    {
                        command.Parameters.AddWithValue("@student_id", student.student_id);
                        command.Parameters.AddWithValue("@rfid", student.rfid);

                        // Store timestamp, or only date part if you prefer
                        command.Parameters.AddWithValue("@date", student.date != default
                            ? student.date
                            : DateTime.UtcNow);

                        command.Parameters.AddWithValue("@status",
                            string.IsNullOrWhiteSpace(student.status)
                            ? (object)DBNull.Value
                            : student.status);

                        int rowsAffected = command.ExecuteNonQuery();
                        Debug.WriteLine($"✅ Attendance created successfully. Rows affected: {rowsAffected}");
                        return rowsAffected > 0;
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                Debug.WriteLine($"❌ Npgsql Error in CreateAttendance: {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error in CreateAttendance: {ex.Message}");
                return false;
            }
        }

        //CREATE: Async version of CreateAttendance
        

        // READ: Get all attendance records (with optional filters)
        public static List<Dictionary<string, object>> ReadAllAttendance(int? studentId = null, DateTime? fromDate = null, DateTime? toDate = null)
        {
            var results = new List<Dictionary<string, object>>();

            try
            {
                using (var connection = CreateShortLivedConnection())
                {
                    connection.Open();
                    string sql = @"SELECT id, student_id, rfid, date, status FROM students_attendance";
                    var parameters = new List<NpgsqlParameter>();

                    // Build WHERE clause with filters
                    bool hasFilter = false;
                    if (studentId.HasValue)
                    {
                        sql += (hasFilter ? " AND" : " WHERE") + " student_id = @student_id";
                        parameters.Add(new NpgsqlParameter("@student_id", studentId.Value));
                        hasFilter = true;
                    }
                    if (fromDate.HasValue)
                    {
                        sql += (hasFilter ? " AND" : " WHERE") + " date >= @from_date";
                        parameters.Add(new NpgsqlParameter("@from_date", fromDate.Value));
                        hasFilter = true;
                    }
                    if (toDate.HasValue)
                    {
                        sql += (hasFilter ? " AND" : " WHERE") + " date <= @to_date";
                        parameters.Add(new NpgsqlParameter("@to_date", toDate.Value));
                        hasFilter = true;
                    }

                    sql += " ORDER BY date DESC, id";  // Recent first

                    using (var command = new NpgsqlCommand(sql, connection))
                    {
                        foreach (var param in parameters)
                        {
                            command.Parameters.Add(param);
                        }

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var row = new Dictionary<string, object>();

                                int idIdx = reader.GetOrdinal("id");
                                int studentIdIdx = reader.GetOrdinal("student_id");
                                int rfidIdx = reader.GetOrdinal("rfid");
                                int dateIdx = reader.GetOrdinal("date");
                                int statusIdx = reader.GetOrdinal("status");

                                row["id"] = reader.IsDBNull(idIdx) ? (object)null : reader.GetValue(idIdx);
                                row["student_id"] = reader.IsDBNull(studentIdIdx) ? (object)null : reader.GetInt32(studentIdIdx);
                                row["rfid"] = reader.IsDBNull(rfidIdx) ? (object)null : reader.GetString(rfidIdx);
                                row["date"] = reader.IsDBNull(dateIdx) ? (object)null : reader.GetDateTime(dateIdx);
                                row["status"] = reader.IsDBNull(statusIdx) ? (object)null : reader.GetString(statusIdx);

                                results.Add(row);
                            }

                        }
                    }
                    Debug.WriteLine($"✅ Retrieved {results.Count} attendance records.");
                }
            }
            catch (NpgsqlException ex)
            {
                Debug.WriteLine($"❌ Npgsql Error in ReadAllAttendance: {ex.Message}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error in ReadAllAttendance: {ex.Message}");
            }

            return results;
        }

        //READ: Async version of ReadAllAttendance
        

        // READ: Get a single attendance record by id
        public static Dictionary<string, object> ReadAttendanceById(long attendanceId)
        {
            try
            {
                using (var connection = CreateShortLivedConnection())
                {
                    connection.Open();
                    using (var command = new NpgsqlCommand(
                        @"SELECT id, student_id, rfid, date, status 
                      FROM students_attendance WHERE id = @id", connection))
                    {
                        command.Parameters.AddWithValue("@id", attendanceId);

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                var row = new Dictionary<string, object>();

                                row["student_id"] = reader.GetInt32(reader.GetOrdinal("student_id"));
                                row["rfid"] = reader.IsDBNull(reader.GetOrdinal("rfid"))
                                    ? null
                                    : reader.GetString(reader.GetOrdinal("rfid"));

                                row["first_name"] = reader.IsDBNull(reader.GetOrdinal("first_name"))
                                    ? null
                                    : reader.GetString(reader.GetOrdinal("first_name"));

                                row["last_name"] = reader.IsDBNull(reader.GetOrdinal("last_name"))
                                    ? null
                                    : reader.GetString(reader.GetOrdinal("last_name"));

                                row["year_level"] = reader.GetInt32(reader.GetOrdinal("year_level"));

                                row["status"] = reader.IsDBNull(reader.GetOrdinal("status"))
                                    ? null
                                    : reader.GetString(reader.GetOrdinal("status"));

                                row["date"] = reader.GetDateTime(reader.GetOrdinal("date"));

                                Debug.WriteLine($"✅ Retrieved attendance record ID {row["student_id"]}.");
                                return row;
                            }

                        }
                    }
                }
                Debug.WriteLine($"⚠️ No attendance record found for ID {attendanceId}.");
                return null;
            }
            catch (NpgsqlException ex)
            {
                Debug.WriteLine($"❌ Npgsql Error in ReadAttendanceById: {ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error in ReadAttendanceById: {ex.Message}");
                return null;
            }
        }

        //READ: Async version of ReadAttendanceById 
        

        // UPDATE: Update an existing attendance record by id (using 'student' object for fields)
        public static bool UpdateAttendance(long attendanceId, student student)
        {
            try
            {
                using (var connection = CreateShortLivedConnection())
                {
                    connection.Open();
                    using (var command = new NpgsqlCommand(
                        @"UPDATE students_attendance 
                      SET student_id = @student_id, rfid = @rfid, date = @date, status = @status 
                      WHERE id = @id", connection))
                    {
                        command.Parameters.AddWithValue("@id", attendanceId);
                        command.Parameters.AddWithValue("@student_id", student.student_id);
                        command.Parameters.AddWithValue("@rfid", string.IsNullOrWhiteSpace(student.rfid) ? (object)DBNull.Value : student.rfid);

                        // Use provided date or current if default
                        command.Parameters.AddWithValue("@date", student.date != default ? student.date : DateTime.UtcNow);

                        command.Parameters.AddWithValue("@status",
                            string.IsNullOrWhiteSpace(student.status) ? (object)DBNull.Value : student.status);

                        int rowsAffected = command.ExecuteNonQuery();
                        Debug.WriteLine($"✅ Attendance updated successfully. Rows affected: {rowsAffected}");
                        return rowsAffected > 0;
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                Debug.WriteLine($"❌ Npgsql Error in UpdateAttendance: {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error in UpdateAttendance: {ex.Message}");
                return false;
            }
        }

        //UPDATE: Async version of UpdateAttendance
       

        // DELETE: Delete an attendance record by id
        public static bool DeleteAttendance(long attendanceId)
        {
            try
            {
                using (var connection = CreateShortLivedConnection())
                {
                    connection.Open();
                    using (var command = new NpgsqlCommand(
                        "DELETE FROM students_attendance WHERE id = @id", connection))
                    {
                        command.Parameters.AddWithValue("@id", attendanceId);

                        int rowsAffected = command.ExecuteNonQuery();
                        Debug.WriteLine($"✅ Attendance deleted successfully. Rows affected: {rowsAffected}");
                        return rowsAffected > 0;
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                Debug.WriteLine($"❌ Npgsql Error in DeleteAttendance: {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error in DeleteAttendance: {ex.Message}");
                return false;
            }
        }

        //CREATE: Async version of DeleteAttendance
        


        //=============================================================================================
        //===================================STUDENTS=================================================
        //============================================================================================
        //CREATE: Insert a new student
        public static void RegisterStudent(student student)
        {
            try
            {
                using(var connection = CreateShortLivedConnection())
                {
                    connection.Open();
                    MessageBox.Show("Connection Established");
                    using(var command = new NpgsqlCommand(
                        "INSERT INTO studentsinformation (id, rfid, first_name, last_name, year_level) VALUES (@student_id, @rfid, @first_name, @last_name, @year_level)", connection))
                    {
                        command.Parameters.AddWithValue("@student_id", student.student_id);
                        command.Parameters.AddWithValue("@rfid", student.rfid);
                        command.Parameters.AddWithValue("@first_name", student.first_name);
                        command.Parameters.AddWithValue("@last_name", student.last_name);
                        command.Parameters.AddWithValue("@year_level", student.year_level);

                        int rowsAffected = command.ExecuteNonQuery();
                        Debug.WriteLine($"Student Registered Successfully. Rows affected: {rowsAffected}");
                        MessageBox.Show("Student Registered Successfully");
                        
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                Debug.WriteLine($"Npgsql Error in CreateAttendance: {ex.Message}");
                MessageBox.Show(ex.Message);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in CreateAttendance: {ex.Message}");
                MessageBox.Show(ex.Message);

            }
        }

        //CREATE: Async version of RegisterStudent
        public static async Task<bool> CreateStudentAsync(student student)
        {
            try
            {
                using (var connection = CreateShortLivedConnection())
                {
                    await connection.OpenAsync();
                    using (var command = new NpgsqlCommand(
                        @"INSERT INTO studentsinformation (id, rfid, first_name, last_name, year_level) 
                  VALUES (@student_id, @rfid, @first_name, @last_name, @year_level)", connection))
                    {
                        command.Parameters.AddWithValue("@student_id", student.student_id);
                        command.Parameters.AddWithValue("@rfid", string.IsNullOrWhiteSpace(student.rfid) ? (object)DBNull.Value : student.rfid);
                        command.Parameters.AddWithValue("@first_name", string.IsNullOrWhiteSpace(student.first_name) ? (object)DBNull.Value : student.first_name);
                        command.Parameters.AddWithValue("@last_name", string.IsNullOrWhiteSpace(student.last_name) ? (object)DBNull.Value : student.last_name);
                        command.Parameters.AddWithValue("@year_level", student.year_level > 0 ? (object)student.year_level : DBNull.Value);

                        int rowsAffected = await command.ExecuteNonQueryAsync();
                        Debug.WriteLine($"✅ Student created successfully. Rows affected: {rowsAffected}");
                        return rowsAffected > 0;
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                Debug.WriteLine($"❌ Npgsql Error in CreateStudent: {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error in CreateStudent: {ex.Message}");
                return false;
            }
        }


        public static List<student> ReadAllStudents()  // Return List<Student> (typed)
        {
            var students = new List<student>();

            try
            {
                using (var connection = CreateShortLivedConnection())
                {
                    connection.Open();
                    using (var command = new NpgsqlCommand(
                        "SELECT id, rfid, first_name, last_name, year_level " +
                        "FROM studentsinformation " +
                        "ORDER BY last_name, first_name",  // Alphabetical for UX
                        connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                students.Add(new student()
                                {
                                    student_id = reader.IsDBNull(reader.GetOrdinal("id"))
                                        ? 0L  // Use 0L for long
                                        : reader.GetInt64(reader.GetOrdinal("id")),  // Changed to GetInt64
                                    rfid = reader.IsDBNull(reader.GetOrdinal("rfid"))
                                        ? string.Empty
                                        : reader.GetString(reader.GetOrdinal("rfid")),
                                    first_name = reader.IsDBNull(reader.GetOrdinal("first_name"))
                                        ? string.Empty
                                        : reader.GetString(reader.GetOrdinal("first_name")),
                                    last_name = reader.IsDBNull(reader.GetOrdinal("last_name"))
                                        ? string.Empty
                                        : reader.GetString(reader.GetOrdinal("last_name")),
                                    year_level = reader.IsDBNull(reader.GetOrdinal("year_level"))
                                        ? 0
                                        : reader.GetInt32(reader.GetOrdinal("year_level"))  // Keep Int32 if small; change to GetInt64 if needed
                                });
                            }
                        }
                        MessageBox.Show("All Students Retrieved");  // Consider moving this outside or checking if students.Count > 0
                    }
                }
                Debug.WriteLine($"✅ Retrieved {students.Count} students.");
            }
            catch (NpgsqlException ex)
            {
                Debug.WriteLine($"❌ Npgsql Error in ReadAllStudents: {ex.Message}");
                MessageBox.Show($"Database error: {ex.Message}");  // More user-friendly
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error in ReadAllStudents: {ex.Message}");
                MessageBox.Show($"Unexpected error: {ex.Message}");
            }

            return students;
        }

        //READ: Async version of ReadAllStudents
        public static async Task<List<student>> ReadAllStudentsAsync()
        {
            var students = new List<student>();

            try
            {
                using (var connection = CreateShortLivedConnection())
                {
                    await connection.OpenAsync();
                    using (var command = new NpgsqlCommand(
                        @"SELECT id, rfid, first_name, last_name, year_level 
                  FROM studentsinformation 
                  ORDER BY last_name, first_name", connection))
                    {
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                students.Add(new student()
                                {
                                    student_id = reader.IsDBNull(reader.GetOrdinal("id")) ? 0L : reader.GetInt64(reader.GetOrdinal("id")),
                                    rfid = reader.IsDBNull(reader.GetOrdinal("rfid")) ? string.Empty : reader.GetString(reader.GetOrdinal("rfid")),
                                    first_name = reader.IsDBNull(reader.GetOrdinal("first_name")) ? string.Empty : reader.GetString(reader.GetOrdinal("first_name")),
                                    last_name = reader.IsDBNull(reader.GetOrdinal("last_name")) ? string.Empty : reader.GetString(reader.GetOrdinal("last_name")),
                                    year_level = reader.IsDBNull(reader.GetOrdinal("year_level")) ? 0 : reader.GetInt32(reader.GetOrdinal("year_level"))
                                });
                            }
                        }
                        Debug.WriteLine($"✅ Retrieved {students.Count} students.");
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                Debug.WriteLine($"❌ Npgsql Error in ReadAllStudents: {ex.Message}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error in ReadAllStudents: {ex.Message}");
            }

            return students;
        }

        // READ: Get a single student by id
        public static Dictionary<string, object> ReadStudentById(string rfid)
        {
            try
            {
                using (var connection = CreateShortLivedConnection())
                {
                    connection.Open();
                    using (var command = new NpgsqlCommand(
                        "SELECT id, rfid, first_name, last_name, year_level FROM studentsinformation WHERE id = @id", connection))
                    {
                        command.Parameters.AddWithValue("@rfid", rfid);

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var row = new Dictionary<string, object>();

                                // Using column name with reader["column"]
                                row["id"] = reader["id"] == DBNull.Value ? null : reader["id"];
                                row["rfid"] = reader["rfid"] == DBNull.Value ? null : reader["rfid"].ToString();
                                row["first_name"] = reader["first_name"] == DBNull.Value ? null : reader["first_name"].ToString();
                                row["last_name"] = reader["last_name"] == DBNull.Value ? null : reader["last_name"].ToString();
                                row["year_level"] = reader["year_level"] == DBNull.Value ? null : reader["year_level"].ToString();

                                return row; 
                            }

                        }
                    }
                }
                Debug.WriteLine($"⚠️ No student found for ID {rfid}.");
                return null;  // Or throw NotFoundException if preferred
            }
            catch (NpgsqlException ex)
            {
                Debug.WriteLine($"❌ Npgsql Error in ReadStudentById: {ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error in ReadStudentById: {ex.Message}");
                return null;
            }
        }

        //READ: Async version of ReadStudentById
        public static async Task<student> ReadStudentByIdAsync(long studentId)
        {
            try
            {
                using (var connection = CreateShortLivedConnection())
                {
                    await connection.OpenAsync();
                    using (var command = new NpgsqlCommand(
                        @"SELECT id, rfid, first_name, last_name, year_level 
                  FROM studentsinformation WHERE id = @id", connection))
                    {
                        command.Parameters.AddWithValue("@id", studentId);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                var student = new student()
                                {
                                    student_id = reader.GetInt64(reader.GetOrdinal("id")),
                                    rfid = reader.IsDBNull(reader.GetOrdinal("rfid")) ? string.Empty : reader.GetString(reader.GetOrdinal("rfid")),
                                    first_name = reader.IsDBNull(reader.GetOrdinal("first_name")) ? string.Empty : reader.GetString(reader.GetOrdinal("first_name")),
                                    last_name = reader.IsDBNull(reader.GetOrdinal("last_name")) ? string.Empty : reader.GetString(reader.GetOrdinal("last_name")),
                                    year_level = reader.IsDBNull(reader.GetOrdinal("year_level")) ? 0 : reader.GetInt32(reader.GetOrdinal("year_level"))
                                };

                                Debug.WriteLine($"✅ Retrieved student ID {studentId}.");
                                return student;
                            }
                        }
                    }
                }
                Debug.WriteLine($"⚠️ No student found for ID {studentId}.");
                return null;
            }
            catch (NpgsqlException ex)
            {
                Debug.WriteLine($"❌ Npgsql Error in ReadStudentById: {ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error in ReadStudentById: {ex.Message}");
                return null;
            }
        }

        // UPDATE: Update an existing student by id (using full 'student' object)
        public static bool UpdateStudent(student student)
        {
            try
            {
                using (var connection = CreateShortLivedConnection())
                {
                    connection.Open();

                    using (var command = new NpgsqlCommand(@"
                                                    UPDATE studentsinformation
                                                    SET rfid = @rfid,
                                                        first_name = @first_name,
                                                        last_name = @last_name,
                                                        year_level = @year_level
                                                    WHERE id = @id", connection))
                    {
                        // Always required
                        command.Parameters.AddWithValue("@id", student.student_id);

                        // Nullable / optional fields
                        command.Parameters.AddWithValue("@rfid",
                            string.IsNullOrWhiteSpace(student.rfid) ? (object)DBNull.Value : student.rfid);

                        command.Parameters.AddWithValue("@first_name",
                            string.IsNullOrWhiteSpace(student.first_name) ? (object)DBNull.Value : student.first_name);

                        command.Parameters.AddWithValue("@last_name",
                            string.IsNullOrWhiteSpace(student.last_name) ? (object)DBNull.Value : student.last_name);

                        // Year level as int (only insert if > 0)
                        command.Parameters.AddWithValue("@year_level",
                            student.year_level > 0 ? (object)student.year_level : DBNull.Value);

                        int rowsAffected = command.ExecuteNonQuery();
                        Debug.WriteLine($"✅ Student updated successfully. Rows affected: {rowsAffected}");
                        return rowsAffected > 0;
                    }
                }

            }
            catch (NpgsqlException ex)
            {
                Debug.WriteLine($"❌ Npgsql Error in UpdateStudent: {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error in UpdateStudent: {ex.Message}");
                return false;
            }
        }

        //UPDATE: Async version of UpdateStudent
        public static async Task<bool> UpdateStudentAsync(student student)
        {
            try
            {
                using (var connection = CreateShortLivedConnection())
                {
                    await connection.OpenAsync();
                    using (var command = new NpgsqlCommand(
                        @"UPDATE studentsinformation 
                  SET rfid = @rfid, first_name = @first_name, last_name = @last_name, year_level = @year_level 
                  WHERE id = @id", connection))
                    {
                        command.Parameters.AddWithValue("@id", student.student_id);
                        command.Parameters.AddWithValue("@rfid", string.IsNullOrWhiteSpace(student.rfid) ? (object)DBNull.Value : student.rfid);
                        command.Parameters.AddWithValue("@first_name", string.IsNullOrWhiteSpace(student.first_name) ? (object)DBNull.Value : student.first_name);
                        command.Parameters.AddWithValue("@last_name", string.IsNullOrWhiteSpace(student.last_name) ? (object)DBNull.Value : student.last_name);
                        command.Parameters.AddWithValue("@year_level", student.year_level > 0 ? (object)student.year_level : DBNull.Value);

                        int rowsAffected = await command.ExecuteNonQueryAsync();
                        Debug.WriteLine($"✅ Student updated successfully. Rows affected: {rowsAffected}");
                        return rowsAffected > 0;
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                Debug.WriteLine($"❌ Npgsql Error in UpdateStudent: {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error in UpdateStudent: {ex.Message}");
                return false;
            }
        }

        // DELETE: Delete a student by id
        public static bool DeleteStudent(long studentId)
        {
            try
            {
                using (var connection = CreateShortLivedConnection())
                {
                    connection.Open();
                    using (var command = new NpgsqlCommand(
                        "DELETE FROM studentsinformation WHERE id = @id", connection))
                    {
                        command.Parameters.AddWithValue("@id", studentId);

                        int rowsAffected = command.ExecuteNonQuery();
                        Debug.WriteLine($"✅ Student deleted successfully. Rows affected: {rowsAffected}");
                        return rowsAffected > 0;
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                Debug.WriteLine($"❌ Npgsql Error in DeleteStudent: {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error in DeleteStudent: {ex.Message}");
                return false;
            }
        }

        //DELETE: Async version of DeleteStudent
        public static async Task<bool> DeleteStudentAsync(long studentId)
        {
            try
            {
                using (var connection = CreateShortLivedConnection())
                {
                    await connection.OpenAsync();
                    using (var command = new NpgsqlCommand(
                        "DELETE FROM studentsinformation WHERE id = @id", connection))
                    {
                        command.Parameters.AddWithValue("@id", studentId);

                        int rowsAffected = await command.ExecuteNonQueryAsync();
                        Debug.WriteLine($"✅ Student deleted successfully. Rows affected: {rowsAffected}");
                        return rowsAffected > 0;
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                Debug.WriteLine($"❌ Npgsql Error in DeleteStudent: {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error in DeleteStudent: {ex.Message}");
                return false;
            }
        }


        //=========================================================================================
        //==================================TIME IN/OUT============================================
        //=========================================================================================
        //CREATE: Insert a new time in record
        public static bool CreateTimeAvail(TimeAvail timeAvail)
        {
            try
            {

                using (var connection = CreateShortLivedConnection())
                {
                    connection.Open();
                    using (var command = new NpgsqlCommand(
                        "INSERT INTO time_avail  (time_in, time_out) VALUES (@time_in, @time_out)", connection))
                    {
                        command.Parameters.AddWithValue("@time_in", timeAvail.timeIn);
                        command.Parameters.AddWithValue("@time_out", timeAvail.timeOut); // Initially null

                        int rowsAffected = command.ExecuteNonQuery();
                        Debug.WriteLine($"Time In Created Successfully. Rows affected: {rowsAffected}");

                    }
                }
                return true;
            }
            catch (NpgsqlException ex)
            {
                Debug.WriteLine($"❌ Npgsql Error in CreateTimeIn: {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error in CreateTimeIn: {ex.Message}");
                return false;
            }
        }

        //READ: Get all time in records
        public static List<TimeAvail> ReadAllTimeIn()
        {
            var timeAvails = new List<TimeAvail>();
            try
            {
                using (var connection = CreateShortLivedConnection())
                {
                    connection.Open();
                    using (var command = new NpgsqlCommand(
                        "SELECT id, time_in, time_out FROM time_avail ORDER BY time_in DESC", connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                timeAvails.Add(new TimeAvail(
                                    reader.GetDateTime(reader.GetOrdinal("time_in")).ToString("yyyy-MM-dd"),
                                    reader.GetDateTime(reader.GetOrdinal("time_in")).ToString("HH:mm"),
                                    reader.IsDBNull(reader.GetOrdinal("time_out")) ? "N/A" : reader.GetDateTime(reader.GetOrdinal("time_out")).ToString("HH:mm")
                                ));
                            }
                        }
                    }
                }
                Debug.WriteLine($"✅ Retrieved {timeAvails.Count} time in records.");
            }
            catch (NpgsqlException ex)
            {
                Debug.WriteLine($"❌ Npgsql Error in ReadAllTimeIn: {ex.Message}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error in ReadAllTimeIn: {ex.Message}");
            }
            return timeAvails;
        }

        //READ: Get a single time in record by date
        public static DateTime ReadTimeInbyDate(string date)
        {
            try
            {
                using (var connection = CreateShortLivedConnection())
                {
                    connection.Open();
                    using (var command = new NpgsqlCommand(
                        "SELECT id, time_in, time_out FROM time_avail WHERE time_in::date = @date", connection))
                    {
                        command.Parameters.AddWithValue("@date", DateTime.Parse(date).Date);
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                var timeIn = reader.GetDateTime(reader.GetOrdinal("time_in"));
                                return timeIn;
                            }
                        }
                    }
                }
                Debug.WriteLine($"⚠️ No time in record found for date {date}.");
                return default;  // Or throw NotFoundException if preferred
            }
            catch (NpgsqlException ex)
            {
                Debug.WriteLine($"❌ Npgsql Error in ReadTimeInByDate: {ex.Message}");
                return default;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error in ReadTimeInByDate: {ex.Message}");
                return default;
            }
        }



    }
}
