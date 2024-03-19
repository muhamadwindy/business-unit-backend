using System.ComponentModel.DataAnnotations;

namespace BusinessUnitApp.Models.Dtos
{
    public class UploadDocumentDto
    {
        public string? ChunkId { set; get; }
        public string? Id { set; get; }
        public IFormFile? File { get; set; }
    }
}