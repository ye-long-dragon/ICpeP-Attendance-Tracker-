using ICpeP_Attendance_Tracker___Main.database;
using ICpeP_Attendance_Tracker___Main.models;
using ICpeP_Attendance_Tracker___Main.pages.popups;
using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ICpeP_Attendance_Tracker___Main.pages
{
    public partial class studentList : UserControl
    {
        public studentList()
        {
            InitializeComponent();
            SetupButtonEvents();
            // Load async data after control is loaded (use Load event for better timing)
            this.Load += StudentList_Load;  // NEW: Async load on form/control load
        }

        private async void StudentList_Load(object sender, EventArgs e)  // NEW: Async load event
        {
            await LoadStudentsAsync();
        }

        private void SetupButtonEvents()
        {
            studentListView.CellContentClick += StudentListView_CellContentClick;  // Handle button clicks
        }

        // ASYNC: Load students (non-blocking)
        public async Task LoadStudentsAsync()
        {
            try
            {
                // UI Feedback: Show loading
                studentListView.Enabled = false;
                Cursor.Current = Cursors.WaitCursor;

                DataTable dataTable = new DataTable();

                // Add only data columns (no button columns here)
                dataTable.Columns.Add("RFID", typeof(string));
                dataTable.Columns.Add("Student ID", typeof(long));
                dataTable.Columns.Add("First Name", typeof(string));
                dataTable.Columns.Add("Last Name", typeof(string));
                dataTable.Columns.Add("Year Level", typeof(int));

                // ASYNC: Fetch from DB
                List<student> students = await dbconnect.ReadAllStudentsAsync();
                foreach (student stu in students)  // Renamed 'student' to 'stu' to avoid keyword confusion
                {
                    DataRow row = dataTable.NewRow();
                    row["RFID"] = stu.rfid ?? string.Empty;  // Use null-coalescing for safety
                    row["Student ID"] = stu.student_id;
                    row["First Name"] = stu.first_name ?? string.Empty;
                    row["Last Name"] = stu.last_name ?? string.Empty;
                    row["Year Level"] = stu.year_level;

                    dataTable.Rows.Add(row);
                }

                // Bind data to DataGridView
                studentListView.DataSource = dataTable;

                // Configure DataGridView for better UX
                studentListView.ReadOnly = true;  // Prevent accidental edits (allow via buttons)
                studentListView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                studentListView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                // Add button columns AFTER binding (they don't bind to data)
                if (studentListView.Columns["Update"] == null)
                {
                    var updateColumn = new DataGridViewButtonColumn
                    {
                        Name = "Update",
                        HeaderText = "Edit",  // More intuitive text
                        Text = "Edit",
                        UseColumnTextForButtonValue = true,
                        Width = 80
                    };
                    studentListView.Columns.Insert(5, updateColumn);  // Insert after "Year Level" (index 4, so 5)
                }

                if (studentListView.Columns["Delete"] == null)
                {
                    var deleteColumn = new DataGridViewButtonColumn
                    {
                        Name = "Delete",
                        HeaderText = "Delete",
                        Text = "Delete",
                        UseColumnTextForButtonValue = true,
                        Width = 80
                    };
                    studentListView.Columns.Insert(6, deleteColumn);  // Insert after Update (index 5, so 6)
                }

                // Optional: Hide "Student ID" if not needed for display (but keep for internal use)
                // studentListView.Columns["Student ID"].Visible = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading students: {ex.Message}");
            }
            finally
            {
                // UI Feedback: Restore
                studentListView.Enabled = true;
                Cursor.Current = Cursors.Default;
            }
        }

        // ASYNC: Handle cell clicks (non-blocking for DB ops)
        private async void StudentListView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;  // Ignore headers/empty

            DataGridView dgv = (DataGridView)sender;
            long studentId = (long)dgv.Rows[e.RowIndex].Cells["Student ID"].Value;  // Get ID from row

            // UI Feedback: Disable during op
            dgv.Enabled = false;
            Cursor.Current = Cursors.WaitCursor;

            try
            {
                if (dgv.Columns[e.ColumnIndex].Name == "Delete")
                {
                    // Confirm deletion (sync, as it's quick)
                    var result = MessageBox.Show($"Delete student with ID {studentId}?\nThis cannot be undone.", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (result == DialogResult.Yes)
                    {
                        // ASYNC: Delete from DB
                        await dbconnect.DeleteStudentAsync(studentId);
                        MessageBox.Show("Student deleted successfully.");
                        await LoadStudentsAsync();  // ASYNC: Refresh grid
                    }
                }
                else if (dgv.Columns[e.ColumnIndex].Name == "Update")
                {
                    // Get current student data from row (or fetch fresh from DB for accuracy)
                    string rfid = dgv.Rows[e.RowIndex].Cells["RFID"].Value?.ToString() ?? string.Empty;
                    string firstName = dgv.Rows[e.RowIndex].Cells["First Name"].Value?.ToString() ?? string.Empty;
                    string lastName = dgv.Rows[e.RowIndex].Cells["Last Name"].Value?.ToString() ?? string.Empty;
                    int yearLevel = (int)dgv.Rows[e.RowIndex].Cells["Year Level"].Value;

                    student currentStudent = new student
                    {
                        student_id = studentId,
                        rfid = rfid,
                        first_name = firstName,
                        last_name = lastName,
                        year_level = yearLevel
                    };

                    // Sync: Open edit form (modal, blocks until closed)
                    var editForm = new EditStudentForm(currentStudent);
                    if (editForm.ShowDialog() == DialogResult.OK)
                    {
                        // ASYNC: Save changes after form closes
                        await dbconnect.UpdateStudentAsync(editForm.UpdatedStudent);
                        MessageBox.Show("Student updated successfully.");
                        await LoadStudentsAsync();  // ASYNC: Refresh grid
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
            finally
            {
                // UI Feedback: Restore
                dgv.Enabled = true;
                Cursor.Current = Cursors.Default;
            }
        }
    }
}
