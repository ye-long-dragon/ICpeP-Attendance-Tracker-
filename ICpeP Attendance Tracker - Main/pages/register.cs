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
    public partial class register : UserControl
    {
        public register()
        {
            InitializeComponent();
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            if (this.ParentForm is AttendanceTracker form)
            {
                form.ShowMainPage();  // Switches back to main
            }
        }
    }
}
