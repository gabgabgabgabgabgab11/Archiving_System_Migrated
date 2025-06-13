using System;
using System.Drawing;
using System.Windows.Forms;
using Archiving_System_Migrated.Controls;

namespace Archiving_System_Migrated
{
    public partial class Form1 : Form
    {
        private Panel mainPanel;
        private SidebarNavigation sidebarNavigation;
        private DashboardControl dashboardControl;
        private AddDocumentControl addDocumentControl;
        private SearchRetrieveControl searchRetrieveControl;
        private ManageDocumentsControl manageDocumentsControl;

        public Form1()
        {
            InitializeComponent();

            // --- MAIN CONTENT PANEL (Add after sidebar, DockFill) ---
            mainPanel = new Panel();
            mainPanel.Dock = DockStyle.Fill;
            mainPanel.BackColor = Color.White;
            this.Controls.Add(mainPanel);

            // --- SIDEBAR NAVIGATION (Always add first!) ---
            sidebarNavigation = new SidebarNavigation();
            sidebarNavigation.Dock = DockStyle.Left;
            sidebarNavigation.Width = 200;
            this.Controls.Add(sidebarNavigation);

            // --- USER CONTROLS (All DockFill, add to mainPanel only!) ---
            dashboardControl = new DashboardControl();
            dashboardControl.Dock = DockStyle.Fill;

            addDocumentControl = new AddDocumentControl();
            addDocumentControl.Dock = DockStyle.Fill;

            searchRetrieveControl = new SearchRetrieveControl();
            searchRetrieveControl.Dock = DockStyle.Fill;

            manageDocumentsControl = new ManageDocumentsControl();
            manageDocumentsControl.Dock = DockStyle.Fill;

            mainPanel.Controls.Add(dashboardControl);
            mainPanel.Controls.Add(addDocumentControl);
            mainPanel.Controls.Add(searchRetrieveControl);
            mainPanel.Controls.Add(manageDocumentsControl);

            addDocumentControl.Hide();
            searchRetrieveControl.Hide();
            manageDocumentsControl.Hide();

            // --- SIDEBAR BUTTON EVENTS ---
            sidebarNavigation.btnDashboard.Click += (s, e) => ShowDashboard();
            sidebarNavigation.btnAddDocument.Click += (s, e) => ShowAddDocument();
            sidebarNavigation.btnSearchRetrieve.Click += (s, e) => ShowSearchRetrieve();
            sidebarNavigation.btnManageDocuments.Click += (s, e) => ShowManageDocuments();

            // --- QUICK ACTION BUTTON EVENTS (DASHBOARD) ---
            dashboardControl.btnAddNew.Click += (s, e) => ShowAddDocument();
            dashboardControl.btnSearch.Click += (s, e) => ShowSearchRetrieve();
            dashboardControl.btnManage.Click += (s, e) => ShowManageDocuments();

            // --- INITIALIZE DATA ON STARTUP ---
            dashboardControl.LoadStats();
            manageDocumentsControl.LoadDropdowns();
            manageDocumentsControl.LoadDocuments();
            searchRetrieveControl.LoadDocuments();

            // --- INITIAL VIEW ---
            ShowDashboard();
        }

        private void ShowDashboard()
        {
            dashboardControl.LoadStats();
            dashboardControl.Show();
            addDocumentControl.Hide();
            searchRetrieveControl.Hide();
            manageDocumentsControl.Hide();
        }
        private void ShowAddDocument()
        {
            addDocumentControl.Show();
            dashboardControl.Hide();
            searchRetrieveControl.Hide();
            manageDocumentsControl.Hide();
        }
        private void ShowSearchRetrieve()
        {
            searchRetrieveControl.LoadDocuments();
            searchRetrieveControl.Show();
            dashboardControl.Hide();
            addDocumentControl.Hide();
            manageDocumentsControl.Hide();
        }
        private void ShowManageDocuments()
        {
            manageDocumentsControl.LoadDropdowns();
            manageDocumentsControl.LoadDocuments();
            manageDocumentsControl.Show();
            dashboardControl.Hide();
            addDocumentControl.Hide();
            searchRetrieveControl.Hide();
        }
    }
}