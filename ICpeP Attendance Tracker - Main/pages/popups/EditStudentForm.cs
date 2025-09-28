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

namespace ICpeP_Attendance_Tracker___Main.pages.popups
{
    public partial class EditStudentForm : Form
    {

        student currentStudent;
        public student UpdatedStudent { get; private set; }
        public EditStudentForm(student student)
        {
            InitializeComponent();
            currentStudent = student;
            PopulateFields();
        }

        public void PopulateFields()
        {
            if (currentStudent != null)
            {
                txtRFID.Text = currentStudent.rfid;
                txtStudentId.Text = currentStudent.student_id.ToString();
                txtFirstName.Text = currentStudent.first_name;
                txtLastName.Text = currentStudent.last_name;
                cmbYearLevel.SelectedIndex = currentStudent.year_level;
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            student updatedStudent = new student
            {
                rfid = txtRFID.Text,
                student_id = long.Parse(txtStudentId.Text),
                first_name = txtFirstName.Text,
                last_name = txtLastName.Text,
                year_level = cmbYearLevel.SelectedIndex
            };

            // NEW: Assign to property for access from parent form
            UpdatedStudent = updatedStudent;
            // NEW: Set dialog result and close form
            this.DialogResult = DialogResult.OK;
            this.Close();


        }
    }
}
