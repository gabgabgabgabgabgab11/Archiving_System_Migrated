namespace Archiving_System_Migrated
{
    public class Document
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Authors { get; set; }
        public int TypeId { get; set; }
        public string TypeName { get; set; } // For JOIN
        public string Description { get; set; } // Formerly Abstract
        public string FilePath { get; set; }
        public DateTime DateArchived { get; set; }
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; } // For JOIN
    }

    public class Department
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class DocumentType
    {
        public int Id { get; set; }
        public string TypeName { get; set; }
    }
}