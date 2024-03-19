namespace BusinessUnitApp.Models.Dtos
{
    public class ResponseAPIDto
    {
        public bool status { get; set; }
        public string? message { get; set; }
        public object? data { get; set; }
    }
}