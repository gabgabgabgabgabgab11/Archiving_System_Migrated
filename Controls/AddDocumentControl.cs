using System;
using System.Drawing;
using System.Windows.Forms;

namespace Archiving_System_Migrated.Controls
{
    public partial class AddDocumentControl : UserControl
    {
        TextBox txtTitle, txtAuthors, txtFilePath, txtDescription;
        ComboBox cmbCategory, cmbDepartment;
        Button btnBrowse, btnSubmit, btnClear;
        string pdfFilePath = "";

        public AddDocumentControl()
        {
            InitializeComponent();
            this.Dock = DockStyle.Fill;
            this.BackColor = Color.White;

            int y = 20, spacing = 38, labelW = 140, fieldW = 500;

            Controls.Add(HeaderLabel("Add New Document", 10, y)); y += 36;
            Controls.Add(L("Document Title:", 10, y)); txtTitle = TB(160, y, fieldW); Controls.Add(txtTitle); y += spacing;
            Controls.Add(L("Author(s):", 10, y)); txtAuthors = TB(160, y, fieldW); Controls.Add(txtAuthors); y += spacing;
            Controls.Add(L("Document Category:", 10, y));
            cmbCategory = new ComboBox()
            {
                Location = new Point(160, y),
                Width = fieldW,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbCategory.Items.AddRange(new string[] { "RESEARCH/THESIS", "OJT TERMINAL REPORT", "OTHERS" });
            cmbCategory.SelectedIndex = 0;
            Controls.Add(cmbCategory);
            y += spacing;

            Controls.Add(L("Department:", 10, y));
            cmbDepartment = new ComboBox()
            {
                Location = new Point(160, y),
                Width = fieldW,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbDepartment.Items.AddRange(new string[] { "IT", "CompEng", "EDUC", "DOMT", "HM", "BSA" });
            cmbDepartment.SelectedIndex = 0;
            Controls.Add(cmbDepartment);
            y += spacing;

            Controls.Add(L("Upload Document (PDF):", 10, y));
            txtFilePath = TB(160, y, fieldW - 110); txtFilePath.ReadOnly = true; Controls.Add(txtFilePath);
            btnBrowse = new Button() { Text = "Choose File", Location = new Point(160 + fieldW - 100, y), Width = 100 };
            Controls.Add(btnBrowse);
            y += spacing;

            Controls.Add(L("Description (Optional):", 10, y));
            txtDescription = new TextBox()
            {
                Location = new Point(160, y),
                Width = fieldW,
                Height = 60,
                Multiline = true
            };
            Controls.Add(txtDescription);
            y += 70;

            btnSubmit = new Button()
            {
                Text = "Submit Document",
                Location = new Point(160, y),
                Width = 150,
                BackColor = Color.RoyalBlue,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnClear = new Button()
            {
                Text = "Clear Form",
                Location = new Point(320, y),
                Width = 110,
                BackColor = Color.LightGray,
                ForeColor = Color.Black,
                FlatStyle = FlatStyle.Flat
            };
            Controls.Add(btnSubmit); Controls.Add(btnClear);

            btnBrowse.Click += BtnBrowse_Click;
            btnSubmit.Click += BtnSubmit_Click;
            btnClear.Click += (s, e) => ClearForm();
        }

        Label HeaderLabel(string t, int x, int y)
            => new Label() { Text = t, Location = new Point(x, y), Font = new Font("Segoe UI", 14, FontStyle.Bold), AutoSize = true };
        Label L(string t, int x, int y)
            => new Label() { Text = t, Location = new Point(x, y + 4), Width = 140, Font = new Font("Segoe UI", 10) };
        TextBox TB(int x, int y, int w)
            => new TextBox() { Location = new Point(x, y), Width = w };

        private void BtnBrowse_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.Filter = "PDF files (*.pdf)|*.pdf";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    pdfFilePath = ofd.FileName;
                    txtFilePath.Text = pdfFilePath;
                }
            }
        }

        private void BtnSubmit_Click(object sender, EventArgs e)
        {
            // Validation
            if (string.IsNullOrWhiteSpace(txtTitle.Text)
                || string.IsNullOrWhiteSpace(txtAuthors.Text)
                || string.IsNullOrWhiteSpace(pdfFilePath)
                || !pdfFilePath.ToLower().EndsWith(".pdf"))
            {
                MessageBox.Show("Please fill in all required fields and select a PDF file.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Find type_id and department_id from names
            var repo = new DocumentRepository();
            int typeId = repo.GetTypeIdByName(cmbCategory.SelectedItem.ToString());
            int deptId = repo.GetDepartmentIdByName(cmbDepartment.SelectedItem.ToString());

            // Save or copy PDF file to archive folder (optional)
            string destFolder = @"C:\ArchivePDFs\"; // Change to your desired storage folder
            if (!System.IO.Directory.Exists(destFolder)) System.IO.Directory.CreateDirectory(destFolder);
            string fileName = System.IO.Path.GetFileName(pdfFilePath);
            string destPath = System.IO.Path.Combine(destFolder, fileName);
            try
            {
                System.IO.File.Copy(pdfFilePath, destPath, true);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to copy PDF file: " + ex.Message, "File Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Insert into database
            var doc = new Document()
            {
                Title = txtTitle.Text,
                Authors = txtAuthors.Text,
                TypeId = typeId,
                DepartmentId = deptId,
                FilePath = destPath,
                Description = txtDescription.Text,
                DateArchived = DateTime.Now
            };
            try
            {
                repo.InsertDocument(doc);
                MessageBox.Show("Document submitted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearForm();

                
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to save document: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearForm()
        {
            txtTitle.Text = "";
            txtAuthors.Text = "";
            cmbCategory.SelectedIndex = 0;
            cmbDepartment.SelectedIndex = 0;
            txtFilePath.Text = "";
            txtDescription.Text = "";
            pdfFilePath = "";
        }
    }
}