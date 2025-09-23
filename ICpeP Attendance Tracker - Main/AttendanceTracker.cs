using System;
using System.Windows.Forms;

namespace ICpeP_Attendance_Tracker___Main
{
    public partial class AttendanceTracker : Form
    {
        private UserControl currentControl;

        // Instantiate UserControls once (for reuse - efficient)
        private UserControl registerControl = new pages.register();
        private UserControl mainControl = new pages.Main();  // Renamed for clarity (was 'main')

        // If you have more, add here e.g., private UserControl settingsControl = new pages.Settings();

        public AttendanceTracker()
        {
            InitializeComponent();
            InitializeNavigation();  // Load initial setup
            ShowUserControl(mainControl);  // Start with the main page (change as needed)
        }

        // Method: One-time setup for all UserControls (add to container once)
        private void InitializeNavigation()
        {
            // Assume mainPanel is a Panel on your form (designer-added)
            // If no panel, replace 'mainPanel' with 'this' to add directly to form

            // Add all controls to the panel/form but hide them initially
            mainPanel.Controls.Add(registerControl);
            mainPanel.Controls.Add(mainControl);
            // Add more: mainPanel.Controls.Add(settingsControl);

            // Configure docking and initial visibility
            registerControl.Dock = DockStyle.Fill;
            registerControl.Visible = false;

            mainControl.Dock = DockStyle.Fill;
            mainControl.Visible = false;

            // Suspend layout to prevent flicker during init
            mainPanel.SuspendLayout();
            mainPanel.ResumeLayout();
        }

        // Core Method: Switch to a new UserControl (fast, smooth, single source of truth)
        // Replaces LoadControl, showUser Control, and HideControl
        public void ShowUserControl(UserControl newControl)
        {
            if (newControl == null)
            {
                throw new ArgumentNullException(nameof(newControl), "Cannot show null control.");
            }

            if (currentControl != null && currentControl != newControl)
            {
                // Hide and optionally dispose if not reusing (but here we reuse, so just hide)
                currentControl.Visible = false;
                // If one-time use: currentControl.Dispose(); but avoid for navigation
            }

            currentControl = newControl;
            currentControl.Visible = true;
            currentControl.BringToFront();  // Ensure it's on top

            // Refresh for any layout updates (only if needed; remove for max speed)
            mainPanel?.Refresh();  // Safe if mainPanel might be null

            // Optional: Focus first control (e.g., for accessibility)
            newControl.Focus();
        }

        // Convenience Methods (for easy calling from buttons/menus)
        private void ShowRegisterPage()
        {
            ShowUserControl(registerControl);
        }

        private void ShowMainPage()
        {
            ShowUserControl(mainControl);
        }

        // Example: Hide all (e.g., for a "home" or logout state)
        private void HideCurrentControl()
        {
            if (currentControl != null)
            {
                currentControl.Visible = false;
                currentControl = null;
                mainPanel.Refresh();
            }
        }

        // Form Closing: Clean up resources
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // Dispose UserControls to free memory (e.g., images, DB connections)
            registerControl?.Dispose();
            mainControl?.Dispose();
            base.OnFormClosing(e);
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            ShowRegisterPage();
        }

        private void btnMain_Click(object sender, EventArgs e)
        {
            ShowMainPage();
        }
    }
}