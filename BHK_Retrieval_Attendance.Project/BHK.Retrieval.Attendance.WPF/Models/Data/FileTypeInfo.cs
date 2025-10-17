namespace BHK.Retrieval.Attendance.WPF.Models.Data
{
    /// <summary>
    /// Thông tin loại file xuất dữ liệu
    /// </summary>
    public class FileTypeInfo
    {
        public string Name { get; set; } = string.Empty;
        public string Extension { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public FileTypeInfo() { }

        public FileTypeInfo(string name, string extension, string description)
        {
            Name = name;
            Extension = extension;
            Description = description;
        }

        public override string ToString()
        {
            return $"{Name} (*.{Extension})";
        }
    }
}