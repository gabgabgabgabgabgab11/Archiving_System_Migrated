using System;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace Archiving_System_Migrated.Controls
{
    public partial class SearchRetrieveControl : UserControl
    {
        public TextBox txtSearch;
        public ComboBox cmbCategory;
        public Button btnSearch, btnClear;
        public DataGridView dgvResults;

        private List<Document> allDocs = new List<Document>();

        public SearchRetrieveControl()
        {
            InitializeComponent();
            BuildUI();
            LoadCategories();
            LoadDocuments();
        }

        private void BuildUI()
        {
            this.BackColor = Color.White;
            this.Dock = DockStyle.Fill;

            var lblHeader = new Label() { Text = "Search Documents", Font = new Font("Segoe UI", 12, FontStyle.Bold), Location = new Point(30, 20) };
            this.Controls.Add(lblHeader);

            this.Controls.Add(new Label() { Text = "Search Term:", Location = new Point(30, 60), Size = new Size(120, 24) });
            txtSearch = new TextBox() { Location = new Point(150, 60), Size = new Size(260, 24) };
            this.Controls.Add(txtSearch);

            this.Controls.Add(new Label() { Text = "Category:", Location = new Point(430, 60), Size = new Size(70, 24) });
            cmbCategory = new ComboBox() { Location = new Point(500, 60), Size = new Size(220, 24), DropDownStyle = ComboBoxStyle.DropDownList };
            this.Controls.Add(cmbCategory);

            btnSearch = new Button() { Text = "Search", Location = new Point(750, 60), Size = new Size(90, 26), BackColor = Color.RoyalBlue, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnClear = new Button() { Text = "Clear", Location = new Point(850, 60), Size = new Size(90, 26), BackColor = Color.LightGray, FlatStyle = FlatStyle.Flat };
            this.Controls.Add(btnSearch); this.Controls.Add(btnClear);

            dgvResults = new DataGridView()
            {
                Location = new Point(30, 110),
                Size = new Size(910, 340),
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            dgvResults.Columns.Add("Title", "Title");
            dgvResults.Columns.Add("Author", "Author");
            dgvResults.Columns.Add("Category", "Category");
            dgvResults.Columns.Add("DateAdded", "Date Added");
            dgvResults.Columns.Add(new DataGridViewButtonColumn() { Name = "View", Text = "View", UseColumnTextForButtonValue = true });
            dgvResults.Columns.Add(new DataGridViewButtonColumn() { Name = "Download", Text = "Download", UseColumnTextForButtonValue = true });

            dgvResults.CellContentClick += DgvResults_CellContentClick;

            this.Controls.Add(dgvResults);

            btnSearch.Click += (s, ev) => ApplyFilter();
            btnClear.Click += (s, ev) => { txtSearch.Text = ""; cmbCategory.SelectedIndex = 0; LoadDocuments(); };
        }

        private void LoadCategories()
        {
            // Populate category dropdown from document types
            var repo = new DocumentRepository();
            var types = repo.GetAllDocumentTypes();
            types.Insert(0, new DocumentType() { Id = 0, TypeName = "All" });
            cmbCategory.DataSource = types;
            cmbCategory.DisplayMember = "TypeName";
            cmbCategory.ValueMember = "Id";
        }

        public void LoadDocuments()
        {
            var repo = new DocumentRepository();
            allDocs = repo.GetAllDocuments();
            ApplyFilter();
        }

        private void ApplyFilter()
        {
            if (allDocs == null) return;
            if (txtSearch == null) return; // Defensive: should never happen if BuildUI is called
            string search = txtSearch.Text.ToLower().Trim();
            int typeId = 0;
            if (cmbCategory.SelectedValue is int)
                typeId = (int)cmbCategory.SelectedValue;
            else if (int.TryParse(cmbCategory.SelectedValue?.ToString(), out int parsed))
                typeId = parsed;

            var filtered = allDocs.Where(d =>
                (string.IsNullOrEmpty(search) ||
                 (d.Title != null && d.Title.ToLower().Contains(search)) ||
                 (d.Authors != null && d.Authors.ToLower().Contains(search))
                ) &&
                (typeId == 0 || d.TypeId == typeId)
            ).ToList();

            dgvResults.Rows.Clear();
            foreach (var doc in filtered)
            {
                dgvResults.Rows.Add(doc.Title, doc.Authors, doc.TypeName, doc.DateArchived.ToString("yyyy-MM-dd"));
            }
        }

        private void DgvResults_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            // Get filtered list for current view
            string search = txtSearch.Text.ToLower().Trim();
            int typeId = 0;
            if (cmbCategory.SelectedValue is int)
                typeId = (int)cmbCategory.SelectedValue;
            else if (int.TryParse(cmbCategory.SelectedValue?.ToString(), out int parsed))
                typeId = parsed;

            var filtered = allDocs.Where(d =>
                (string.IsNullOrEmpty(search) ||
                 (d.Title != null && d.Title.ToLower().Contains(search)) ||
                 (d.Authors != null && d.Authors.ToLower().Contains(search))
                ) &&
                (typeId == 0 || d.TypeId == typeId)
            ).ToList();

            if (e.RowIndex >= filtered.Count) return;

            if (e.ColumnIndex == dgvResults.Columns["View"].Index)
            {
                var doc = filtered[e.RowIndex];
                ViewDocument(doc.FilePath);
            }
            else if (e.ColumnIndex == dgvResults.Columns["Download"].Index)
            {
                var doc = filtered[e.RowIndex];
                DownloadDocument(doc.FilePath, doc.Title);
            }
        }

        private void ViewDocument(string filePath)
        {
            try
            {
                // Open with default PDF viewer
                Process.Start(new ProcessStartInfo(filePath) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not open file: " + ex.Message);
            }
        }

        private void DownloadDocument(string filePath, string title)
        {
            try
            {
                using (var sfd = new SaveFileDialog()
                {
                    FileName = title + ".pdf",
                    Filter = "PDF Files (*.pdf)|*.pdf|All files (*.*)|*.*"
                })
                {
                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        System.IO.File.Copy(filePath, sfd.FileName, true);
                        MessageBox.Show("File downloaded successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not download file: " + ex.Message);
            }
        }
    }
}