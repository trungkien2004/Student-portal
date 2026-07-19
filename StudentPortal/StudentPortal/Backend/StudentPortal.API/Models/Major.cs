namespace StudentPortal.API.Models;

public class Major
{
    public int MajorId { get; set; }
    public string MajorCode { get; set; } = string.Empty;
    public string MajorName { get; set; } = string.Empty;
    public string Faculty { get; set; } = string.Empty;
    public string ClassCode { get; set; } = string.Empty;
    public string Course { get; set; } = string.Empty;

    public ICollection<Student> Students { get; set; } = new List<Student>();
    public ICollection<CurriculumItem> CurriculumItems { get; set; } = new List<CurriculumItem>();
}
