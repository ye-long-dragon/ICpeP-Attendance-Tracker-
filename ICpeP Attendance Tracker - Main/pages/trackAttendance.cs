using ICpeP_Attendance_Tracker___Main.database;
using ICpeP_Attendance_Tracker___Main.models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ICpeP_Attendance_Tracker___Main.pages
{
    public partial class trackAttendance : UserControl
    {
        public trackAttendance()
        {
            InitializeComponent();
            this.Load += TrackAttendance_Load;
        }

        private async void TrackAttendance_Load(object sender, EventArgs e)
        {
            await LoadAttendancesAsync();
        }

        // Async method to load attendance records from Supabase REST API
        public async Task LoadAttendancesAsync()
        {
            try
            {
                dgvAttendance.Enabled = false;
                Cursor.Current = Cursors.WaitCursor;

                DataTable dataTable = new DataTable();
                dataTable.Columns.Add("ID", typeof(long));
                dataTable.Columns.Add("Student ID", typeof(long));
                dataTable.Columns.Add("RFID", typeof(string));
                dataTable.Columns.Add("Date", typeof(DateTime));
                dataTable.Columns.Add("Status", typeof(string));

                // Fetch attendance records asynchronously
                var attendanceRecords = await dbconnect.ReadAllAttendanceAsync();

                foreach (var record in attendanceRecords)
                {
                    DataRow row = dataTable.NewRow();
                    row["ID"] = record.ContainsKey("id") ? Convert.ToInt64(record["id"]) : 0L;
                    row["Student ID"] = record.ContainsKey("student_id") ? Convert.ToInt64(record["student_id"]) : 0L;
                    row["RFID"] = record.ContainsKey("rfid") ? record["rfid"]?.ToString() ?? string.Empty : string.Empty;
                    row["Date"] = record.ContainsKey("date") ? Convert.ToDateTime(record["date"]) : DateTime.MinValue;
                    row["Status"] = record.ContainsKey("status") ? record["status"]?.ToString() ?? string.Empty : string.Empty;

                    dataTable.Rows.Add(row);
                }

                dgvAttendance.DataSource = dataTable;
                dgvAttendance.ReadOnly = true;
                dgvAttendance.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dgvAttendance.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading attendance records: {ex.Message}");
                Debug.WriteLine($"Error loading attendance: {ex}");
            }
            finally
            {
                dgvAttendance.Enabled = true;
                Cursor.Current = Cursors.Default;
            }
        }

        private async void btnUpdate_Click(object sender, EventArgs e)
        {
            var timeInForm = new popups.AddTimeIn();
            if (timeInForm.ShowDialog() == DialogResult.OK)
            {
                // After adding/updating time in, refresh attendance list
                await LoadAttendancesAsync();
            }
        }
    }
}
