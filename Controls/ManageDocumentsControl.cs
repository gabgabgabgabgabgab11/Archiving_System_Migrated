using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Collections.Generic;

namespace Archiving_System_Migrated.Controls
{
    public partial class ManageDocumentsControl : UserControl
    {
        DataGridView dgv;
        TextBox txtSearch, txtTitle, txtAuthors, txtFilePath, txtDescription;
        ComboBox cmbType, cmbDept, cmbFilterType, cmbFilterDept;
        Button btnSearch, btnRefresh, btnSave, btnDelete, btnClear;
        List<Document> allDocs;
        Document selectedDoc;

        public ManageDocumentsControl()
        {
            InitializeComponent();
            this.Dock = DockStyle.Fill;
            this.BackColor = Color.White;

            int y = 20;

            // Search controls
            Controls.Add(new Label() { Text = "Search:", Location = new Point(20, y + 4), Width = 60 });
            txtSearch = new TextBox() { Location = new Point(80, y), Width = 200 }; Controls.Add(txtSearch);

            Controls.Add(new Label() { Text = "Type:", Location = new Point(300, y + 4), Width = 40 });
            cmbFilterType = new ComboBox() { Location = new Point(340, y), Width = 130, DropDownStyle = ComboBoxStyle.DropDownList }; Controls.Add(cmbFilterType);

            Controls.Add(new Label() { Text = "Department:", Location = new Point(480, y + 4), Width = 80 });
            cmbFilterDept = new ComboBox() { Location = new Point(570, y), Width = 130, DropDownStyle = ComboBoxStyle.DropDownList }; Controls.Add(cmbFilterDept);

            btnSearch = new Button() { Text = "Filter", Location = new Point(710, y), Width = 70, BackColor = Color.RoyalBlue, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnRefresh = new Button() { Text = "Refresh", Location = new Point(790, y), Width = 70, BackColor = Color.Gray, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            Controls.Add(btnSearch); Controls.Add(btnRefresh);

            y += 40;

            // DataGridView
            dgv = new DataGridView()
            {
                Location = new Point(20, y),
                Size = new Size(950, 230),
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoGenerateColumns = false
            };
            Controls.Add(dgv);

            dgv.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "ID", DataPropertyName = "Id", Width = 40 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "Title", DataPropertyName = "Title", Width = 180 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "Authors", DataPropertyName = "Authors", Width = 120 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "Type", DataPropertyName = "TypeName", Width = 100 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "Dept.", DataPropertyName = "DepartmentName", Width = 110 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "Description", DataPropertyName = "Description", Width = 200 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "File Path", DataPropertyName = "FilePath", Width = 120 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "Date Archived", DataPropertyName = "DateArchived", Width = 110, DefaultCellStyle = new DataGridViewCellStyle() { Format = "yyyy-MM-dd" } });

            dgv.SelectionChanged += (s, e) => LoadSelectedDocument();

            y += 240;

            // Edit controls
            int formX = 20, formY = y, labelW = 70, fieldW = 220, spacing = 10, lineH = 30;
            Controls.Add(new Label() { Text = "Title:", Location = new Point(formX, formY + 4), Width = labelW });
            txtTitle = new TextBox() { Location = new Point(formX + labelW, formY), Width = fieldW }; Controls.Add(txtTitle);

            Controls.Add(new Label() { Text = "Authors:", Location = new Point(formX + labelW + fieldW + spacing, formY + 4), Width = labelW });
            txtAuthors = new TextBox() { Location = new Point(formX + 2 * (labelW + fieldW) + spacing, formY), Width = fieldW }; Controls.Add(txtAuthors);

            formY += lineH;
            Controls.Add(new Label() { Text = "Type:", Location = new Point(formX, formY + 4), Width = labelW });
            cmbType = new ComboBox() { Location = new Point(formX + labelW, formY), Width = 140, DropDownStyle = ComboBoxStyle.DropDownList }; Controls.Add(cmbType);

            Controls.Add(new Label() { Text = "Department:", Location = new Point(formX + labelW + 150, formY + 4), Width = 80 });
            cmbDept = new ComboBox() { Location = new Point(formX + labelW + 240, formY), Width = 140, DropDownStyle = ComboBoxStyle.DropDownList }; Controls.Add(cmbDept);

            formY += lineH;
            Controls.Add(new Label() { Text = "Description:", Location = new Point(formX, formY + 4), Width = labelW });
            txtDescription = new TextBox() { Location = new Point(formX + labelW, formY), Width = fieldW, Height = 30, Multiline = true }; Controls.Add(txtDescription);

            Controls.Add(new Label() { Text = "File Path:", Location = new Point(formX + labelW + fieldW + spacing, formY + 4), Width = labelW });
            txtFilePath = new TextBox() { Location = new Point(formX + 2 * (labelW + fieldW) + spacing, formY), Width = fieldW }; Controls.Add(txtFilePath);

            btnSave = new Button() { Text = "Save", Location = new Point(formX + 2 * (labelW + fieldW) + spacing + 250, formY), Width = 80, BackColor = Color.RoyalBlue, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnDelete = new Button() { Text = "Delete", Location = new Point(formX + 2 * (labelW + fieldW) + spacing + 340, formY), Width = 80, BackColor = Color.Red, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnClear = new Button() { Text = "Clear", Location = new Point(formX + 2 * (labelW + fieldW) + spacing + 430, formY), Width = 80, BackColor = Color.Gray, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            Controls.Add(btnSave); Controls.Add(btnDelete); Controls.Add(btnClear);

            // Events
            btnSearch.Click += (s, e) => ApplyFilter();
            btnRefresh.Click += (s, e) => LoadDocuments();
            btnSave.Click += (s, e) => SaveDocument();
            btnDelete.Click += (s, e) => DeleteDocument();
            btnClear.Click += (s, e) => ClearEditFields();

            dgv.DoubleClick += (s, e) => LoadSelectedDocument();
        }

        public void LoadDropdowns()
        {
            var repo = new DocumentRepository();

            // Filter dropdowns
            var types = repo.GetAllDocumentTypes();
            types.Insert(0, new DocumentType() { Id = 0, TypeName = "All" });
            cmbFilterType.DataSource = types.ToList();
            cmbFilterType.DisplayMember = "TypeName";
            cmbFilterType.ValueMember = "Id";

            var deps = repo.GetAllDepartments();
            deps.Insert(0, new Department() { Id = 0, Name = "All" });
            cmbFilterDept.DataSource = deps.ToList();
            cmbFilterDept.DisplayMember = "Name";
            cmbFilterDept.ValueMember = "Id";

            // Edit dropdowns
            cmbType.DataSource = repo.GetAllDocumentTypes();
            cmbType.DisplayMember = "TypeName";
            cmbType.ValueMember = "Id";
            cmbDept.DataSource = repo.GetAllDepartments();
            cmbDept.DisplayMember = "Name";
            cmbDept.ValueMember = "Id";
        }

        public void LoadDocuments()
        {
            var repo = new DocumentRepository();
            allDocs = repo.GetAllDocuments();
            dgv.DataSource = allDocs;
            ClearEditFields();
        }

        private void ApplyFilter()
        {
            if (allDocs == null) return;
            string search = txtSearch.Text.ToLower().Trim();
            int typeId = (int)cmbFilterType.SelectedValue;
            int deptId = (int)cmbFilterDept.SelectedValue;

            var filtered = allDocs.Where(d =>
                (string.IsNullOrEmpty(search) ||
                 (d.Title != null && d.Title.ToLower().Contains(search)) ||
                 (d.Authors != null && d.Authors.ToLower().Contains(search))
                ) &&
                (typeId == 0 || d.TypeId == typeId) &&
                (deptId == 0 || d.DepartmentId == deptId)
            ).ToList();
            dgv.DataSource = filtered;
        }

        private void LoadSelectedDocument()
        {
            if (dgv.SelectedRows.Count == 0)
            {
                selectedDoc = null;
                ClearEditFields();
                return;
            }
            selectedDoc = dgv.SelectedRows[0].DataBoundItem as Document;
            if (selectedDoc == null) return;
            txtTitle.Text = selectedDoc.Title;
            txtAuthors.Text = selectedDoc.Authors;
            cmbType.SelectedValue = selectedDoc.TypeId;
            cmbDept.SelectedValue = selectedDoc.DepartmentId;
            txtDescription.Text = selectedDoc.Description;
            txtFilePath.Text = selectedDoc.FilePath;
        }

        private void SaveDocument()
        {
            if (selectedDoc == null)
            {
                MessageBox.Show("Select a document to edit.", "Notice");
                return;
            }
            try
            {
                selectedDoc.Title = txtTitle.Text;
                selectedDoc.Authors = txtAuthors.Text;
                selectedDoc.TypeId = (int)cmbType.SelectedValue;
                selectedDoc.DepartmentId = (int)cmbDept.SelectedValue;
                selectedDoc.Description = txtDescription.Text;
                selectedDoc.FilePath = txtFilePath.Text;

                var repo = new DocumentRepository();
                repo.UpdateDocument(selectedDoc);

                MessageBox.Show("Document updated!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadDocuments();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving document: " + ex.Message);
            }
        }

        private void DeleteDocument()
        {
            if (selectedDoc == null)
            {
                MessageBox.Show("Select a document to delete.", "Notice");
                return;
            }
            if (MessageBox.Show("Delete selected document?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                var repo = new DocumentRepository();
                repo.DeleteDocument(selectedDoc.Id);
                LoadDocuments();
            }
        }

        private void ClearEditFields()
        {
            txtTitle.Text = "";
            txtAuthors.Text = "";
            cmbType.SelectedIndex = 0;
            cmbDept.SelectedIndex = 0;
            txtDescription.Text = "";
            txtFilePath.Text = "";
            selectedDoc = null;
        }
    }
}