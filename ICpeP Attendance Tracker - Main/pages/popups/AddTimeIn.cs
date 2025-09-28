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

namespace ICpeP_Attendance_Tracker___Main.pages.popups
{
    public partial class AddTimeIn : Form
    {
        public AddTimeIn()
        {
            InitializeComponent();
        }

        private void welcome_Click(object sender, EventArgs e)
        {

        }

        private void btnAddTime_Click(object sender, EventArgs e)
        {
            string timeIn = txtStart.Text;
            string timeOut = txtEnd.Text;
            DateTime date = dtpDate.Value;

            //validate time format
            DateTime temp;
            if (!DateTime.TryParse(timeIn, out temp) || !DateTime.TryParse(timeOut, out temp))
            {
                MessageBox.Show("Invalid time format. Please use HH:mm format.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            //validate timein is before timeout
            if (DateTime.Parse(timeIn) >= DateTime.Parse(timeOut))
            {
                MessageBox.Show("Time In must be before Time Out.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            //add date to timein and timeout
            DateTime fullTimeIn = DateTime.Parse(date.ToString("yyyy-MM-dd") + " " + timeIn);
            DateTime fullTimeOut = DateTime.Parse(date.ToString("yyyy-MM-dd") + " " + timeOut);
            //send to database
            TimeAvail setTime = new TimeAvail(date.ToString("yyyy-MM-dd"), timeIn, timeOut);
            var sendTime = dbconnect.CreateTimeAvail(setTime);
            if (sendTime)
            {
                MessageBox.Show("Time Added Successfully", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            else
            {
                MessageBox.Show("Failed to add time. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }


        }
    }
}
