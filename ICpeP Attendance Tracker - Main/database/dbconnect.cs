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
    public class DbConnect
    {
        private static readonly HttpClient httpClient = new HttpClient();
        private static readonly string supabaseUrl = "https://axwduoswuwxdqimchpvs.supabase.co/rest/v1";
        private static readonly string supabaseApiKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6ImF4d2R1b3N3dXd4ZHFpbWNocHZzIiwicm9sZSI6InNlcnZpY2Vfcm9sZSIsImlhdCI6MTc1ODE5MzcwNywiZXhwIjoyMDczNzY5NzA3fQ.qLuyjji-hGGBBUUJ-tdC9EHzoXk3pmEBnbz-mb9bNjw";

        static DbConnect()
        {
            httpClient.BaseAddress = new Uri(supabaseUrl);
            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Add("apikey", supabaseApiKey);
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {supabaseApiKey}");
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        // ===========================
        // 🔹 Helper Methods
        // ===========================

        private static StringContent ToJsonContent(object obj) =>
            new StringContent(JsonSerializer.Serialize(obj), Encoding.UTF8, "application/json");

        private static async Task<T> SendRequestAsync<T>(Func<Task<HttpResponseMessage>> httpAction)
        {
            try
            {
                var response = await httpAction().ConfigureAwait(false);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    Debug.WriteLine($"❌ Request failed: {error}");
                    return default;
                }

                var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                return JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Exception: {ex.Message}");
                return default;
            }
        }

        private static async Task<bool> SendNonQueryAsync(Func<Task<HttpResponseMessage>> httpAction, string successMessage)
        {
            try
            {
                var response = await httpAction().ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {
                    Debug.WriteLine($"✅ {successMessage}");
                    return true;
                }

                var error = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                Debug.WriteLine($"❌ Request failed: {error}");
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Exception: {ex.Message}");
                return false;
            }
        }

        // ===========================
        // 🔹 Attendance CRUD
        // ===========================

        public static Task<bool> CreateAttendanceAsync(student student)
        {
            var attendanceRecord = new
            {
                student_id = student.student_id,
                rfid = student.rfid,
                date = student.date != default ? student.date.ToString("yyyy-MM-dd") : DateTime.UtcNow.ToString("yyyy-MM-dd"),
                status = string.IsNullOrWhiteSpace(student.status) ? null : student.status
            };

            return SendNonQueryAsync(
                () => httpClient.PostAsync("students_attendance", ToJsonContent(attendanceRecord)),
                "Attendance created successfully."
            );
        }

        public static Task<List<Dictionary<string, object>>> ReadAllAttendanceAsync(int? studentId = null, DateTime? fromDate = null, DateTime? toDate = null)
        {
            var queryParams = new List<string>();

            if (studentId.HasValue) queryParams.Add($"student_id=eq.{studentId.Value}");
            if (fromDate.HasValue) queryParams.Add($"date=gte.{fromDate.Value:yyyy-MM-dd}");
            if (toDate.HasValue) queryParams.Add($"date=lte.{toDate.Value:yyyy-MM-dd}");
            queryParams.Add("order=date.desc,id.asc");

            var url = $"students_attendance?{string.Join("&", queryParams)}";

            return SendRequestAsync<List<Dictionary<string, object>>>(() => httpClient.GetAsync(url));
        }

        public static async Task<Dictionary<string, object>> ReadAttendanceByIdAsync(long attendanceId)
        {
            var list = await SendRequestAsync<List<Dictionary<string, object>>>(() =>
                httpClient.GetAsync($"students_attendance?id=eq.{attendanceId}")
            ).ConfigureAwait(false);

            return list != null && list.Count > 0 ? list[0] : null;
        }

        public static Task<bool> UpdateAttendanceAsync(long attendanceId, student student)
        {
            var attendanceRecord = new
            {
                student_id = student.student_id,
                rfid = string.IsNullOrWhiteSpace(student.rfid) ? null : student.rfid,
                date = student.date != default ? student.date.ToString("yyyy-MM-dd") : DateTime.UtcNow.ToString("yyyy-MM-dd"),
                status = string.IsNullOrWhiteSpace(student.status) ? null : student.status
            };

            var request = new HttpRequestMessage(new HttpMethod("PATCH"), $"students_attendance?id=eq.{attendanceId}")
            {
                Content = ToJsonContent(attendanceRecord)
            };

            return SendNonQueryAsync(() => httpClient.SendAsync(request), "Attendance updated successfully.");
        }

        public static Task<bool> DeleteAttendanceAsync(long attendanceId) =>
            SendNonQueryAsync(
                () => httpClient.DeleteAsync($"students_attendance?id=eq.{attendanceId}"),
                "Attendance deleted successfully."
            );
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
