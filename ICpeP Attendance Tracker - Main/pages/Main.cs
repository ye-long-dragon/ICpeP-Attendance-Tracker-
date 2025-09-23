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
    public partial class Main : UserControl
    {
        public Main()
        {
            InitializeComponent();
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            if (this.ParentForm is AttendanceTracker form)
            {
                form.ShowRegisterPage();  // Switches to register
            }
        }
    }
}
