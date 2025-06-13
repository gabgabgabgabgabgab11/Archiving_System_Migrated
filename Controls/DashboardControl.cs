using System;
using System.Drawing;
using System.Windows.Forms;
using Archiving_System_Migrated;

namespace Archiving_System_Migrated.Controls
{
    public partial class DashboardControl : UserControl
    {
        public Label lblTotalDocuments;
        public Label lblThesis;
        public Label lblOjt;
        public Label lblOther;

        public Button btnAddNew;
        public Button btnSearch;
        public Button btnManage;

        public DashboardControl()
        {
            InitializeComponent();
            this.BackColor = Color.White;
            this.Dock = DockStyle.Fill;

            // Statistics Panel
            var statsPanel = new Panel()
            {
                Location = new Point(20, 20),
                Size = new Size(900, 120),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            var lblHeader = new Label()
            {
                Text = "Document Stats",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Location = new Point(10, 10),
                AutoSize = true
            };
            statsPanel.Controls.Add(lblHeader);

            lblTotalDocuments = StatLabel("0", 60);
            lblThesis = StatLabel("0", 260);
            lblOjt = StatLabel("0", 460);
            lblOther = StatLabel("0", 660);

            statsPanel.Controls.Add(StatCategoryLabel("Total Documents", 60));
            statsPanel.Controls.Add(lblTotalDocuments);
            statsPanel.Controls.Add(StatCategoryLabel("Thesis", 260));
            statsPanel.Controls.Add(lblThesis);
            statsPanel.Controls.Add(StatCategoryLabel("OJT Report", 460));
            statsPanel.Controls.Add(lblOjt);
            statsPanel.Controls.Add(StatCategoryLabel("Other Documents", 660));
            statsPanel.Controls.Add(lblOther);

            // Quick Actions
            var quickPanel = new Panel()
            {
                Location = new Point(20, 160),
                Size = new Size(900, 160),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            var lblQuick = new Label()
            {
                Text = "Quick Actions",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Location = new Point(10, 10),
                AutoSize = true
            };
            quickPanel.Controls.Add(lblQuick);

            btnAddNew = QuickButton("Add New Document", 30);
            btnSearch = QuickButton("Search Documents", 320);
            btnManage = QuickButton("Manage Documents", 610);

            quickPanel.Controls.Add(btnAddNew);
            quickPanel.Controls.Add(btnSearch);
            quickPanel.Controls.Add(btnManage);

            quickPanel.Controls.Add(QuickDesc("Upload and archive a new document", 30));
            quickPanel.Controls.Add(QuickDesc("Search for and retrieve documents", 320));
            quickPanel.Controls.Add(QuickDesc("Edit or delete existing documents", 610));

            this.Controls.Add(statsPanel);
            this.Controls.Add(quickPanel);

            this.Size = new Size(950, 340);
        }

        private Label StatLabel(string text, int x)
        {
            return new Label()
            {
                Text = text,
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                ForeColor = Color.RoyalBlue,
                Location = new Point(x, 65),
                AutoSize = true
            };
        }

        private Label StatCategoryLabel(string text, int x)
        {
            return new Label()
            {
                Text = text,
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                Location = new Point(x, 40),
                AutoSize = true
            };
        }

        private Button QuickButton(string text, int x)
        {
            var btn = new Button()
            {
                Text = text,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Size = new Size(250, 58),
                Location = new Point(x, 45),
                BackColor = Color.RoyalBlue,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btn.FlatAppearance.BorderSize = 0;
            return btn;
        }

        private Label QuickDesc(string text, int x)
        {
            return new Label()
            {
                Text = text,
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                Location = new Point(x, 110),
                Size = new Size(250, 30),
                TextAlign = ContentAlignment.MiddleCenter
            };
        }

        // Call this after DB change, or on load
        public void LoadStats()
        {
            var repo = new DocumentRepository();
            var stats = repo.GetDocumentStats();
            lblTotalDocuments.Text = stats.total.ToString();
            lblThesis.Text = stats.byType.ContainsKey("RESEARCH/THESIS") ? stats.byType["RESEARCH/THESIS"].ToString() : "0";
            lblOjt.Text = stats.byType.ContainsKey("OJT TERMINAL REPORT") ? stats.byType["OJT TERMINAL REPORT"].ToString() : "0";
            lblOther.Text = (stats.total -
                (stats.byType.ContainsKey("RESEARCH/THESIS") ? stats.byType["RESEARCH/THESIS"] : 0) -
                (stats.byType.ContainsKey("OJT TERMINAL REPORT") ? stats.byType["OJT TERMINAL REPORT"] : 0)
            ).ToString();
        }
    }
}