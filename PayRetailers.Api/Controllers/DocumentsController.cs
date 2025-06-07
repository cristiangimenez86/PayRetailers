using Microsoft.AspNetCore.Mvc;
using PayRetailers.Application.Contracts;
using PayRetailers.Application.DTOs;

namespace PayRetailers.Api.Controllers;

[ApiController]
[Route("accounts/{account}/documents")]
public class DocumentsController(
    IDocumentService documentService,
    ILogger<DocumentsController> logger) : ControllerBase
{
    // GET /accounts/{account}/documents
    [HttpGet]
    public async Task<IActionResult> GetDocumentsByAccount(string account)
    {
        try
        {
            var docs = await documentService.GetByAccountAsync(account);
            return Ok(docs);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, nameof(GetDocumentsByAccount));
            return Problem(statusCode: 500, title: "Internal Server Error");
        }
    }

    // POST /accounts/{account}/documents
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateDocument(string account, [FromBody] DocumentCreateDto dto)
    {
        try
        {
            var id = await documentService.CreateAsync(account, dto);
            return CreatedAtAction(nameof(GetDocumentsByAccount), new { account }, new { documentId = id });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, nameof(CreateDocument));
            return Problem(statusCode: 500, title: "Internal Server Error");
        }
    }

    // PUT /accounts/{account}/documents/{documentId}
    [HttpPut("{documentId:guid}")]
    public async Task<IActionResult> UpdateStatus(string account, Guid documentId, [FromBody] DocumentUpdateDto dto)
    {
        try
        {
            await documentService.UpdateStatusAsync(account, documentId, dto.Status);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, nameof(UpdateStatus));
            return Problem(statusCode: 500, title: "Internal Server Error");
        }
    }
}