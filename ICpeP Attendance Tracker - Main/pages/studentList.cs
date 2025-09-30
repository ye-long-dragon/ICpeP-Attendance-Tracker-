using ICpeP_Attendance_Tracker___Main.database;
using ICpeP_Attendance_Tracker___Main.models;
using ICpeP_Attendance_Tracker___Main.pages.popups;
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
            this.Load += StudentList_Load;  // Async load event
        }

        // Async event handler for control load
        private async void StudentList_Load(object sender, EventArgs e)
        {
            await LoadStudentsAsync();
        }

        private void SetupButtonEvents()
        {
            studentListView.CellContentClick += StudentListView_CellContentClick;
        }

        // Async method to load students and bind to DataGridView
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

                // Await async DB call
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
                MessageBox.Show($"Error loading students: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Debug.WriteLine($"Error loading students: {ex}");
            }
            finally
            {
                studentListView.Enabled = true;
                Cursor.Current = Cursors.Default;
            }
        }

        // Async event handler for button clicks in DataGridView
        private async void StudentListView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            var dgv = (DataGridView)sender;

            if (!(dgv.Rows[e.RowIndex].Cells["Student ID"].Value is long studentId))
            {
                MessageBox.Show("Invalid Student ID.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            dgv.Enabled = false;
            Cursor.Current = Cursors.WaitCursor;

            try
            {
                string columnName = dgv.Columns[e.ColumnIndex].Name;

                if (columnName == "Delete")
                {
                    var confirm = MessageBox.Show($"Delete student with ID {studentId}?\nThis action cannot be undone.", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (confirm == DialogResult.Yes)
                    {
                        // Await async delete
                        bool deleted = await dbconnect.DeleteStudentAsync(studentId);
                        if (deleted)
                        {
                            MessageBox.Show("Student deleted successfully.", "Deleted", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            await LoadStudentsAsync();  // Refresh list async
                        }
                        else
                        {
                            MessageBox.Show("Failed to delete student.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                else if (columnName == "Update")
                {
                    string rfid = dgv.Rows[e.RowIndex].Cells["RFID"].Value?.ToString() ?? string.Empty;
                    string firstName = dgv.Rows[e.RowIndex].Cells["First Name"].Value?.ToString() ?? string.Empty;
                    string lastName = dgv.Rows[e.RowIndex].Cells["Last Name"].Value?.ToString() ?? string.Empty;
                    int yearLevel = 0;
                    int.TryParse(dgv.Rows[e.RowIndex].Cells["Year Level"].Value?.ToString(), out yearLevel);

                    var currentStudent = new student
                    {
                        student_id = studentId,
                        rfid = rfid,
                        first_name = firstName,
                        last_name = lastName,
                        year_level = yearLevel
                    };

                    using (var editForm = new EditStudentForm(currentStudent))
                    {
                        if (editForm.ShowDialog() == DialogResult.OK)
                        {
                            // Await async update
                            bool updated = await dbconnect.UpdateStudentAsync(editForm.UpdatedStudent);
                            if (updated)
                            {
                                MessageBox.Show("Student updated successfully.", "Updated", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                await LoadStudentsAsync();  // Refresh list async
                            }
                            else
                            {
                                MessageBox.Show("Failed to update student.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Debug.WriteLine($"Error in CellContentClick: {ex}");
            }
            finally
            {
                dgv.Enabled = true;
                Cursor.Current = Cursors.Default;
            }
        }
    }
}
