using System;
using System.Drawing;
using System.Windows.Forms;

namespace Archiving_System_Migrated.Controls
{
    public partial class SidebarNavigation : UserControl
    {
        public Button btnDashboard;
        public Button btnAddDocument;
        public Button btnSearchRetrieve;
        public Button btnManageDocuments;

        public SidebarNavigation()
        {
            InitializeComponent();
            this.BackColor = Color.FromArgb(245, 245, 245);
            this.Dock = DockStyle.Left;
            this.Width = 200;

            btnDashboard = CreateSidebarButton("Dashboard", 0, null);
            btnAddDocument = CreateSidebarButton("Add Document", 50, null);
            btnSearchRetrieve = CreateSidebarButton("Search/Retrieve", 100, null);
            btnManageDocuments = CreateSidebarButton("Manage Documents", 150,null);

            this.Controls.Add(btnDashboard);
            this.Controls.Add(btnAddDocument);
            this.Controls.Add(btnSearchRetrieve);
            this.Controls.Add(btnManageDocuments);
        }

       

        private Button CreateSidebarButton(string text, int top, Image icon)
        {
            var btn = new Button();
            btn.Text = "   " + text;
            btn.TextAlign = ContentAlignment.MiddleLeft;
            btn.Width = this.Width - 1;
            btn.Height = 48;
            btn.Left = 0;
            btn.Top = 20 + top;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btn.BackColor = Color.Transparent;
            btn.ForeColor = Color.Black;
            btn.Cursor = Cursors.Hand;
            return btn;
        }
    }
}