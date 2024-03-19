namespace BusinessUnitApp.Services;

using BusinessUnitApp.Models.DbContext;
using BusinessUnitApp.Models.Dtos;
using BusinessUnitApp.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text;

public interface IDocumentService
{
    Task<ResponseAPIDto> UploadLargeFile(HttpContext httpContext, UploadDocumentDto model, string uploadBy);

    Task<ResponseAPIDto> UploadLargeFileCompleted(UploadDocumentCompletedDto document);

    Task<ResponseAPIDto> Download(string id);

    Task<ResponseAPIDto> GetDocument();
}

public class DocumentService : IDocumentService
{
    private readonly AppDbContext _dbContext;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IConfiguration _configuration;

    public DocumentService(AppDbContext dbContext, UserManager<ApplicationUser> userManager, IConfiguration configuration)
    {
        _userManager = userManager;
        _dbContext = dbContext;
        _configuration = configuration;
    }

    public async Task<ResponseAPIDto> Download(string id)
    {
        string message = "";
        bool checkId = Guid.TryParse(id, out var resultId);
        DocumentDto data = new DocumentDto();
        if (checkId)
        {
            Document dataFromDB = await _dbContext.Documents.FirstOrDefaultAsync(x => x.Id == resultId);

            if (dataFromDB == null)
            {
                message = "Document Not Found";
            }

            data.Id = dataFromDB.Id;
            data.FileName = dataFromDB.FileName;
            data.Description = dataFromDB.Description;
            data.UploadBy = dataFromDB.UploadBy;
            data.UploadDate = dataFromDB.UploadDate;

            string[] splittedNameDoc = data.FileName.Split(".");
            string fileExt = splittedNameDoc[splittedNameDoc.Length - 1];
            string fileProcessed = (data.Id + "." + fileExt).ToUpper();
            string pathSource = _configuration["TargetFolder"] + "/" + fileProcessed;

            byte[] fileBlob = null;
            FileStream fs = new FileStream(pathSource, FileMode.Open, FileAccess.Read);
            fileBlob = new byte[fs.Length];

            using (var memoryStream = new MemoryStream())
            {
                await fs.CopyToAsync(memoryStream);
                data.Data = memoryStream.ToArray();
            }
        }

        var response = new ResponseAPIDto
        {
            status = (message == "" ? true : false),
            message = message == "" ? "Download success" : message,
            data = data
        };

        return response;
    }

    public async Task<ResponseAPIDto> GetDocument()
    {
        var data = await _dbContext.Documents.ToListAsync();

        string message = "";
        var response = new ResponseAPIDto
        {
            status = (message == "" ? true : false),
            message = message == "" ? "Get Document success" : message,
            data = data
        };

        return response;
    }

    public async Task<ResponseAPIDto> UploadLargeFile(HttpContext httpContext, UploadDocumentDto uploadDocument, string uploadBy)
    {
        string message = "";

        try
        {
            int chunkSize; chunkSize = 1048576 * Convert.ToInt32(_configuration["ChunkSize"]);

            string tempFolder = _configuration["TargetFolder"];
            var chunkNumber = uploadDocument.ChunkId;
            string newpath = Path.Combine(tempFolder + "/Temp", uploadDocument.Id.ToUpper() + chunkNumber);
            using (FileStream fs = System.IO.File.Create(newpath))
            {
                byte[] bytes = new byte[chunkSize];
                int bytesRead = 0;

                Stream stream = uploadDocument.File.OpenReadStream();
                while ((bytesRead = await stream.ReadAsync(bytes, 0, bytes.Length)) > 0)
                {
                    fs.Write(bytes, 0, bytesRead);
                }
            }
        }
        catch (Exception ex)
        {
            message = ex.Message;
        }

        var response = new ResponseAPIDto
        {
            status = (message == "" ? true : false),
            message = message == "" ? "Upload success" : message,
            data = uploadDocument.File?.FileName
        };

        return response;
    }

    public async Task<ResponseAPIDto> UploadLargeFileCompleted(UploadDocumentCompletedDto document)
    {
        string message = "";
        try
        {
            string[] splittedNameDoc = document.FileName.Split(".");
            string fileExt = splittedNameDoc[splittedNameDoc.Length - 1];
            string fileProcessed = (document.Id + "." + fileExt).ToUpper();

            string tempFolder = _configuration["TargetFolder"];
            string tempPath = tempFolder + "/Temp";
            string newPath = Path.Combine(tempPath, fileProcessed);

            string[] filePaths = Directory.GetFiles(tempPath).Where(p => p.Contains(fileProcessed))
                .OrderBy(p => Int32.Parse(p.Replace(fileProcessed, "$").Split('$')[1])).ToArray();
            foreach (string filePath in filePaths)
            {
                MergeChunks(newPath, filePath);
            }
            System.IO.File.Move(Path.Combine(tempPath, fileProcessed), Path.Combine(tempFolder, fileProcessed));

            // kene iki urusane karo DB

            var doc = new Document
            {
                Id = Guid.Parse(document.Id),
                Description = document.Description,
                FileName = document.FileName,
                UploadBy = document.UploadBy,
                UploadDate = DateTime.Now
            };

            await _dbContext.Documents.AddAsync(doc);
            await _dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            message = ex.Message;
        }

        var response = new ResponseAPIDto
        {
            status = (message == "" ? true : false),
            message = message == "" ? "Upload success" : message,
            data = null
        };

        return response;
    }

    private static void MergeChunks(string chunk1, string chunk2)
    {
        FileStream fs1 = null;
        FileStream fs2 = null;
        try
        {
            fs1 = System.IO.File.Open(chunk1, FileMode.Append);
            fs2 = System.IO.File.Open(chunk2, FileMode.Open);
            byte[] fs2Content = new byte[fs2.Length];
            fs2.Read(fs2Content, 0, (int)fs2.Length);
            fs1.Write(fs2Content, 0, (int)fs2.Length);
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            if (fs1 != null) fs1.Close();
            if (fs2 != null) fs2.Close();
            System.IO.File.Delete(chunk2);
        }
    }
}