using System.ComponentModel.DataAnnotations;

namespace BusinessUnitApp.Models.Entities
{
    public class Document
    {
        [Key]
        public Guid Id { get; set; }

        public string? FileName { get; set; }
        public string? Description { set; get; }
        public string? UploadBy { get; set; }
        public DateTime? UploadDate { get; set; }
    }
}