using ICpeP_Attendance_Tracker___Main.database;
using ICpeP_Attendance_Tracker___Main.models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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
            LoadStudents();
        }

        public void LoadStudents()
        {
            DataTable dataTable = new DataTable();

            dataTable.Columns.Add("RFID", typeof(string));
            dataTable.Columns.Add("Student ID", typeof(long));
            dataTable.Columns.Add("First Name", typeof(string));
            dataTable.Columns.Add("Last Name", typeof(string));
            dataTable.Columns.Add("Year Level", typeof(int));
            dataTable.Columns.Add("Update");
            dataTable.Columns.Add("Delete");

            List<student> students = dbconnect.ReadAllStudents();
            foreach (student student in students)
            {
                DataRow row = dataTable.NewRow();
                row["RFID"] = student.rfid;
                row["Student ID"] = student.student_id;
                row["First Name"] = student.first_name;
                row["Last Name"] = student.last_name;
                row["Year Level"] = student.year_level;
                row["Update"] = "Update";
                row["Delete"] = "Delete";

                dataTable.Rows.Add(row);
            }

            studentListView.DataSource = dataTable;
        }

    }
}
