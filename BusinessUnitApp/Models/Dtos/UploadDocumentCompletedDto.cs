using System.ComponentModel.DataAnnotations;

namespace BusinessUnitApp.Models.Dtos
{
    public class UploadDocumentCompletedDto
    {
        public string? Id { set; get; }
        public string? FileName { set; get; }
        public string? Description { set; get; }
        public string? UploadBy { set; get; }
    }
}