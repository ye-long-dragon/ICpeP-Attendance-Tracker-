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
    public partial class trackAttendance : UserControl
    {
        public trackAttendance()
        {
            InitializeComponent();
        }


        public void LoadAttendances() 
        {


        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            var timeInForm = new popups.AddTimeIn();
            timeInForm.ShowDialog();
        }
    }
}
