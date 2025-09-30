using ICpeP_Attendance_Tracker___Main.models;
using ICpeP_Attendance_Tracker___Main.pages.popups;
using ICpeP_Attendance_Tracker___Main.database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
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
            this.Load += StudentList_Load;  // Async load on control load
        }

        private async void StudentList_Load(object sender, EventArgs e)
        {
            await LoadStudentsAsync();
        }

        private void SetupButtonEvents()
        {
            studentListView.CellContentClick += StudentListView_CellContentClick;
        }

        // Async load students from Supabase REST API
        public async Task LoadStudentsAsync()
        {
            try
            {
                studentListView.Enabled = false;
                Cursor.Current = Cursors.WaitCursor;

                DataTable dataTable = new DataTable();
                dataTable.Columns.Add("RFID", typeof(string));
                dataTable.Columns.Add("Student ID", typeof(long));
                dataTable.Columns.Add("First Name", typeof(string));
                dataTable.Columns.Add("Last Name", typeof(string));
                dataTable.Columns.Add("Year Level", typeof(int));

                List<student> students = await dbconnect.ReadAllStudentsAsync();

                foreach (student stu in students)
                {
                    DataRow row = dataTable.NewRow();
                    row["RFID"] = stu.rfid ?? string.Empty;
                    row["Student ID"] = stu.student_id;
                    row["First Name"] = stu.first_name ?? string.Empty;
                    row["Last Name"] = stu.last_name ?? string.Empty;
                    row["Year Level"] = stu.year_level;
                    dataTable.Rows.Add(row);
                }

                studentListView.DataSource = dataTable;
                studentListView.ReadOnly = true;
                studentListView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                studentListView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                // Add Edit button column if not present
                if (studentListView.Columns["Update"] == null)
                {
                    var updateColumn = new DataGridViewButtonColumn
                    {
                        Name = "Update",
                        HeaderText = "Edit",
                        Text = "Edit",
                        UseColumnTextForButtonValue = true,
                        Width = 80
                    };
                    studentListView.Columns.Insert(5, updateColumn);
                }

                // Add Delete button column if not present
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
                    studentListView.Columns.Insert(6, deleteColumn);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading students: {ex.Message}");
            }
            finally
            {
                studentListView.Enabled = true;
                Cursor.Current = Cursors.Default;
            }
        }

        // Async handler for Edit/Delete button clicks
        private async void StudentListView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            DataGridView dgv = (DataGridView)sender;
            long studentId = (long)dgv.Rows[e.RowIndex].Cells["Student ID"].Value;

            dgv.Enabled = false;
            Cursor.Current = Cursors.WaitCursor;

            try
            {
                if (dgv.Columns[e.ColumnIndex].Name == "Delete")
                {
                    var result = MessageBox.Show($"Delete student with ID {studentId}?\nThis cannot be undone.", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (result == DialogResult.Yes)
                    {
                        bool deleted = await dbconnect.DeleteStudentAsync(studentId);
                        if (deleted)
                        {
                            MessageBox.Show("Student deleted successfully.");
                            await LoadStudentsAsync();
                        }
                        else
                        {
                            MessageBox.Show("Failed to delete student.");
                        }
                    }
                }
                else if (dgv.Columns[e.ColumnIndex].Name == "Update")
                {
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

                    var editForm = new EditStudentForm(currentStudent);
                    if (editForm.ShowDialog() == DialogResult.OK)
                    {
                        bool updated = await dbconnect.UpdateStudentAsync(editForm.UpdatedStudent);
                        if (updated)
                        {
                            MessageBox.Show("Student updated successfully.");
                            await LoadStudentsAsync();
                        }
                        else
                        {
                            MessageBox.Show("Failed to update student.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
            finally
            {
                dgv.Enabled = true;
                Cursor.Current = Cursors.Default;
            }
        }
    }
}
