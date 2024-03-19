namespace BusinessUnitApp.Controllers;

using BusinessUnitApp.Core.OtherObjects;
using BusinessUnitApp.Models.Dtos;
using BusinessUnitApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[ApiController]
[Route("[controller]")]
public class DocumentController : ControllerBase
{
    private IDocumentService _documentService;

    public DocumentController(IDocumentService userService)
    {
        _documentService = userService;
    }

    [HttpPost("upload")]
    [Authorize(Roles = StaticUserRoles.CUSTOMER)]
    public async Task<IActionResult> Upload([FromForm] UploadDocumentDto model)
    {
        var userId = User.Claims.Where(c => c.Type == "UserName").FirstOrDefault().Value;
        var response = await _documentService.UploadLargeFile(HttpContext, model, userId);
        return Ok(response);
    }

    [HttpPost("uploadcompleted")]
    [Authorize(Roles = StaticUserRoles.CUSTOMER)]
    public async Task<IActionResult> UploadCompleted([FromForm] UploadDocumentCompletedDto document)
    {
        var userId = User.Claims.Where(c => c.Type == "UserName").FirstOrDefault().Value;
        document.UploadBy = userId;
        var response = await _documentService.UploadLargeFileCompleted(document);
        return Ok(response);
    }

    [HttpGet("download/{id}")]
    [Authorize(Roles = "CUSTOMER, ADMIN")]
    public async Task<IActionResult> Download(string id)
    {
        var response = await _documentService.Download(id);
        return Ok(response);
    }

    [HttpGet("get")]
    [Authorize(Roles = StaticUserRoles.ADMIN)]
    public async Task<IActionResult> Get()
    {
        var response = await _documentService.GetDocument();
        return Ok(response);
    }
}