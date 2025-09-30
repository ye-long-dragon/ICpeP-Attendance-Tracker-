using ICpeP_Attendance_Tracker___Main.models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ICpeP_Attendance_Tracker___Main.database
{
    public class dbconnect
    {
        private static readonly HttpClient httpClient = new HttpClient();
        private static readonly string supabaseUrl = "https://your-project-ref.supabase.co/rest/v1";
        private static readonly string supabaseApiKey = "your-anon-or-service-role-api-key";

        static dbconnect()
        {
            httpClient.BaseAddress = new Uri(supabaseUrl);
            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Add("apikey", supabaseApiKey);
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {supabaseApiKey}");
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        // Helper method to serialize object to JSON content
        private static StringContent ToJsonContent(object obj)
        {
            var json = JsonSerializer.Serialize(obj);
            return new StringContent(json, Encoding.UTF8, "application/json");
        }

        // ===========================
        // ATTENDANCE CRUD OPERATIONS
        // ===========================

        // CREATE attendance record
        public static async Task<bool> CreateAttendanceAsync(student student)
        {
            try
            {
                var attendanceRecord = new
                {
                    student_id = student.student_id,
                    rfid = student.rfid,
                    date = student.date != default ? student.date.ToString("yyyy-MM-dd") : DateTime.UtcNow.ToString("yyyy-MM-dd"),
                    status = string.IsNullOrWhiteSpace(student.status) ? null : student.status
                };

                var content = ToJsonContent(attendanceRecord);

                // POST to attendance table
                var response = await httpClient.PostAsync("students_attendance", content);
                if (response.IsSuccessStatusCode)
                {
                    Debug.WriteLine("✅ Attendance created successfully.");
                    return true;
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine($"❌ Failed to create attendance: {error}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Exception in CreateAttendanceAsync: {ex.Message}");
                return false;
            }
        }

        // READ all attendance records with optional filters
        public static async Task<List<Dictionary<string, object>>> ReadAllAttendanceAsync(int? studentId = null, DateTime? fromDate = null, DateTime? toDate = null)
        {
            var results = new List<Dictionary<string, object>>();
            try
            {
                var queryParams = new List<string>();

                if (studentId.HasValue)
                    queryParams.Add($"student_id=eq.{studentId.Value}");

                if (fromDate.HasValue)
                    queryParams.Add($"date=gte.{fromDate.Value:yyyy-MM-dd}");

                if (toDate.HasValue)
                    queryParams.Add($"date=lte.{toDate.Value:yyyy-MM-dd}");

                // Order by date descending, id ascending
                queryParams.Add("order=date.desc,id.asc");

                var queryString = string.Join("&", queryParams);
                var url = $"students_attendance?{queryString}";

                var response = await httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    results = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    Debug.WriteLine($"✅ Retrieved {results.Count} attendance records.");
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine($"❌ Failed to read attendance: {error}");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Exception in ReadAllAttendanceAsync: {ex.Message}");
            }
            return results;
        }

        // READ attendance by id
        public static async Task<Dictionary<string, object>> ReadAttendanceByIdAsync(long attendanceId)
        {
            try
            {
                var url = $"students_attendance?id=eq.{attendanceId}";
                var response = await httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var list = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    if (list != null && list.Count > 0)
                    {
                        Debug.WriteLine($"✅ Retrieved attendance record ID {attendanceId}.");
                        return list[0];
                    }
                    else
                    {
                        Debug.WriteLine($"⚠️ No attendance record found for ID {attendanceId}.");
                        return null;
                    }
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine($"❌ Failed to read attendance by ID: {error}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Exception in ReadAttendanceByIdAsync: {ex.Message}");
                return null;
            }
        }

        // UPDATE attendance by id
        public static async Task<bool> UpdateAttendanceAsync(long attendanceId, student student)
        {
            try
            {
                var attendanceRecord = new
                {
                    student_id = student.student_id,
                    rfid = string.IsNullOrWhiteSpace(student.rfid) ? null : student.rfid,
                    date = student.date != default ? student.date.ToString("yyyy-MM-dd") : DateTime.UtcNow.ToString("yyyy-MM-dd"),
                    status = string.IsNullOrWhiteSpace(student.status) ? null : student.status
                };

                var content = ToJsonContent(attendanceRecord);

                // PATCH request to update record by id
                var request = new HttpRequestMessage(new HttpMethod("PATCH"), $"students_attendance?id=eq.{attendanceId}")
                {
                    Content = content
                };

                var response = await httpClient.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    Debug.WriteLine("✅ Attendance updated successfully.");
                    return true;
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine($"❌ Failed to update attendance: {error}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Exception in UpdateAttendanceAsync: {ex.Message}");
                return false;
            }
        }

        // DELETE attendance by id
        public static async Task<bool> DeleteAttendanceAsync(long attendanceId)
        {
            try
            {
                var response = await httpClient.DeleteAsync($"students_attendance?id=eq.{attendanceId}");
                if (response.IsSuccessStatusCode)
                {
                    Debug.WriteLine("✅ Attendance deleted successfully.");
                    return true;
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine($"❌ Failed to delete attendance: {error}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Exception in DeleteAttendanceAsync: {ex.Message}");
                return false;
            }
        }

        // ===========================
        // STUDENTS CRUD OPERATIONS
        // ===========================

        // CREATE student
        public static async Task<bool> CreateStudentAsync(student student)
        {
            try
            {
                var studentRecord = new
                {
                    id = student.student_id,
                    rfid = string.IsNullOrWhiteSpace(student.rfid) ? null : student.rfid,
                    first_name = string.IsNullOrWhiteSpace(student.first_name) ? null : student.first_name,
                    last_name = string.IsNullOrWhiteSpace(student.last_name) ? null : student.last_name,
                    year_level = student.year_level > 0 ? student.year_level : (int?)null
                };

                var content = ToJsonContent(studentRecord);
                var response = await httpClient.PostAsync("studentsinformation", content);
                if (response.IsSuccessStatusCode)
                {
                    Debug.WriteLine("✅ Student created successfully.");
                    return true;
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine($"❌ Failed to create student: {error}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Exception in CreateStudentAsync: {ex.Message}");
                return false;
            }
        }

        // READ all students
        public static async Task<List<student>> ReadAllStudentsAsync()
        {
            var students = new List<student>();

            try
            {
                // Check the full URL formed
                var requestUrl = "studentsinformation?order=last_name.asc,first_name.asc";
                Debug.WriteLine($"Requesting URL: {httpClient.BaseAddress}{requestUrl}");

                var response = await httpClient.GetAsync(requestUrl);

                Debug.WriteLine($"Response Status Code: {response.StatusCode}");

                var content = await response.Content.ReadAsStringAsync();
                Debug.WriteLine($"Response Content: {content}");

                if (response.IsSuccessStatusCode)
                {
                    students = JsonSerializer.Deserialize<List<student>>(content, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    Debug.WriteLine($"✅ Retrieved {students.Count} students.");
                }
                else
                {
                    Debug.WriteLine($"❌ Failed to read students: {content}");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Exception in ReadAllStudentsAsync: {ex}");
            }

            return students;
        }

        // READ student by id
        public static async Task<student> ReadStudentByIdAsync(long studentId)
        {
            try
            {
                var response = await httpClient.GetAsync($"studentsinformation?id=eq.{studentId}");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var list = JsonSerializer.Deserialize<List<student>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    if (list != null && list.Count > 0)
                    {
                        Debug.WriteLine($"✅ Retrieved student ID {studentId}.");
                        return list[0];
                    }
                    else
                    {
                        Debug.WriteLine($"⚠️ No student found for ID {studentId}.");
                        return null;
                    }
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine($"❌ Failed to read student by ID: {error}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Exception in ReadStudentByIdAsync: {ex.Message}");
                return null;
            }
        }

        public static async Task<student> ReadStudentByRfidAsync(string rfid)
        {
            try
            {
                // URL-encode the RFID value to be safe in the query string
                var encodedRfid = Uri.EscapeDataString(rfid);

                // Query Supabase REST API filtering by rfid
                var response = await httpClient.GetAsync($"studentsinformation?rfid=eq.{encodedRfid}");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();

                    // Deserialize JSON array to List<student>
                    var students = JsonSerializer.Deserialize<List<student>>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (students != null && students.Count > 0)
                    {
                        return students[0]; // Return the first matching student
                    }
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine($"❌ Failed to read student by RFID: {error}");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Exception in ReadStudentByRfidAsync: {ex.Message}");
            }

            return null; // Not found or error
        }

        // UPDATE student
        public static async Task<bool> UpdateStudentAsync(student student)
        {
            try
            {
                var studentRecord = new
                {
                    rfid = string.IsNullOrWhiteSpace(student.rfid) ? null : student.rfid,
                    first_name = string.IsNullOrWhiteSpace(student.first_name) ? null : student.first_name,
                    last_name = string.IsNullOrWhiteSpace(student.last_name) ? null : student.last_name,
                    year_level = student.year_level > 0 ? student.year_level : (int?)null
                };

                var content = ToJsonContent(studentRecord);

                var request = new HttpRequestMessage(new HttpMethod("PATCH"), $"studentsinformation?id=eq.{student.student_id}")
                {
                    Content = content
                };

                var response = await httpClient.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    Debug.WriteLine("✅ Student updated successfully.");
                    return true;
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine($"❌ Failed to update student: {error}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Exception in UpdateStudentAsync: {ex.Message}");
                return false;
            }
        }

        // DELETE student
        public static async Task<bool> DeleteStudentAsync(long studentId)
        {
            try
            {
                var response = await httpClient.DeleteAsync($"studentsinformation?id=eq.{studentId}");
                if (response.IsSuccessStatusCode)
                {
                    Debug.WriteLine("✅ Student deleted successfully.");
                    return true;
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine($"❌ Failed to delete student: {error}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Exception in DeleteStudentAsync: {ex.Message}");
                return false;
            }
        }

        // You can similarly implement TimeAvail CRUD using the same pattern.

        // ===========================
        // time_avail CRUD OPERATIONS
        // ===========================

        //CREATE all time_avails
        public static async Task<bool> CreateTimeAvailAsync(TimeAvail time)
        {
            try
            {
                var timeAvailRecord = new
                {
                   
                    time_in = time.timeIn != default ? time.timeIn.ToString("yyyy-MM-ddTHH:mm:ssZ") : null,
                    time_out = time.timeOut != null ? time.timeOut.ToString("yyyy-MM-ddTHH:mm:ssZ") : null,
                    date = time.date,
                };

                var content = ToJsonContent(timeAvailRecord);

                var response = await httpClient.PostAsync("time_avail", content);
                if (response.IsSuccessStatusCode)
                {
                    Debug.WriteLine("✅ TimeAvail created successfully.");
                    return true;
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine($"❌ Failed to create TimeAvail: {error}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Exception in CreateTimeAvailAsync: {ex.Message}");
                return false;
            }
        }

        public static async Task<List<TimeAvail>> ReadAllTimeAvailAsync()
        {
            var results = new List<TimeAvail>();
            try
            {
                var response = await httpClient.GetAsync("time_avail?order=time_in.desc");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    results = JsonSerializer.Deserialize<List<TimeAvail>>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    Debug.WriteLine($"✅ Retrieved {results.Count} time_avail records.");
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine($"❌ Failed to read time_avail: {error}");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Exception in ReadAllTimeAvailAsync: {ex.Message}");
            }
            return results;
        }



    }
}
