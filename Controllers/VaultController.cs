using Microsoft.AspNetCore.Mvc;
using PassXYZ.Server.DTOs.Vault;
using PassXYZ.Server.Services;

namespace PassXYZ.Server.Controllers;

[ApiController]
[Route("api/vault")]
public class VaultController : ControllerBase
{
    private readonly IVaultService _vaultService;

    public VaultController(IVaultService vaultService)
    {
        _vaultService = vaultService;
    }

    [HttpGet("groups/{groupId}/items")]
    public async Task<IActionResult> GetItems(string groupId)
    {
        var username = HttpContext.Items["Username"] as string;
        if (string.IsNullOrEmpty(username)) return Unauthorized();

        var items = await _vaultService.GetItems(username, groupId);
        return Ok(items);
    }

    [HttpGet("items/{itemId}")]
    public async Task<IActionResult> GetItem(string itemId)
    {
        var username = HttpContext.Items["Username"] as string;
        if (string.IsNullOrEmpty(username)) return Unauthorized();

        var item = await _vaultService.GetItem(username, itemId);
        if (item == null) return NotFound();

        return Ok(item);
    }

    [HttpGet("entries/{entryId}")]
    public async Task<IActionResult> GetEntry(string entryId)
    {
        var username = HttpContext.Items["Username"] as string;
        if (string.IsNullOrEmpty(username)) return Unauthorized();

        var entry = await _vaultService.GetEntry(username, entryId);
        if (entry == null) return NotFound();

        return Ok(entry);
    }

    [HttpGet("groups/{groupId}")]
    public async Task<IActionResult> GetGroup(string groupId)
    {
        var username = HttpContext.Items["Username"] as string;
        if (string.IsNullOrEmpty(username)) return Unauthorized();

        var group = await _vaultService.GetGroup(username, groupId);
        if (group == null) return NotFound();

        return Ok(group);
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchEntries([FromQuery] string? keyword)
    {
        var username = HttpContext.Items["Username"] as string;
        if (string.IsNullOrEmpty(username)) return Unauthorized();

        var entries = await _vaultService.SearchEntries(username, keyword ?? string.Empty);
        return Ok(entries);
    }

    [HttpPost("groups/{groupId}/entries")]
    public async Task<IActionResult> CreateEntry(string groupId, [FromBody] NewEntryRequest request)
    {
        var username = HttpContext.Items["Username"] as string;
        if (string.IsNullOrEmpty(username)) return Unauthorized();

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var entryId = await _vaultService.CreateEntry(username, groupId, request);
            return CreatedAtAction(nameof(GetEntry), new { entryId }, new { Id = entryId });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("groups/{parentGroupId}/groups")]
    public async Task<IActionResult> CreateGroup(string parentGroupId, [FromBody] NewGroupRequest request)
    {
        var username = HttpContext.Items["Username"] as string;
        if (string.IsNullOrEmpty(username)) return Unauthorized();

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var groupId = await _vaultService.CreateGroup(username, parentGroupId, request);
            return CreatedAtAction(nameof(GetGroup), new { groupId }, new { Id = groupId });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("entries/{entryId}")]
    public async Task<IActionResult> UpdateEntry(string entryId, [FromBody] EntryDto request)
    {
        var username = HttpContext.Items["Username"] as string;
        if (string.IsNullOrEmpty(username)) return Unauthorized();

        var result = await _vaultService.UpdateEntry(username, entryId, request);
        if (!result) return NotFound();

        return Ok();
    }

    [HttpPut("groups/{groupId}")]
    public async Task<IActionResult> UpdateGroup(string groupId, [FromBody] GroupDto request)
    {
        var username = HttpContext.Items["Username"] as string;
        if (string.IsNullOrEmpty(username)) return Unauthorized();

        var result = await _vaultService.UpdateGroup(username, groupId, request);
        if (!result) return NotFound();

        return Ok();
    }

    [HttpDelete("entries/{entryId}")]
    public async Task<IActionResult> DeleteEntry(string entryId)
    {
        var username = HttpContext.Items["Username"] as string;
        if (string.IsNullOrEmpty(username)) return Unauthorized();

        var result = await _vaultService.DeleteEntry(username, entryId);
        if (!result) return NotFound();

        return Ok();
    }

    [HttpDelete("groups/{groupId}")]
    public async Task<IActionResult> DeleteGroup(string groupId)
    {
        var username = HttpContext.Items["Username"] as string;
        if (string.IsNullOrEmpty(username)) return Unauthorized();

        var result = await _vaultService.DeleteGroup(username, groupId);
        if (!result) return NotFound();

        return Ok();
    }

    [HttpPost("change-password")]
    public async Task<IActionResult> ChangeMasterPassword([FromBody] ChangePasswordRequest request)
    {
        var username = HttpContext.Items["Username"] as string;
        if (string.IsNullOrEmpty(username)) return Unauthorized();

        var result = await _vaultService.ChangeMasterPassword(username, request.NewPassword);
        if (!result) return BadRequest("Failed to change password");

        return Ok();
    }

    [HttpGet("icons")]
    public async Task<IActionResult> GetIcons()
    {
        var icons = await _vaultService.GetIcons();
        return Ok(icons);
    }

    [HttpGet("entries/{entryId}/attachments")]
    public async Task<IActionResult> GetAttachments(string entryId)
    {
        var username = HttpContext.Items["Username"] as string;
        if (string.IsNullOrEmpty(username)) return Unauthorized();

        var attachments = await _vaultService.GetAttachments(username, entryId);
        return Ok(attachments);
    }

    [HttpGet("entries/{entryId}/attachments/{attachmentId}")]
    public async Task<IActionResult> DownloadAttachment(string entryId, string attachmentId)
    {
        var username = HttpContext.Items["Username"] as string;
        if (string.IsNullOrEmpty(username)) return Unauthorized();

        var data = await _vaultService.DownloadAttachment(username, entryId, attachmentId);
        if (data == null) return NotFound();

        var attachments = await _vaultService.GetAttachments(username, entryId);
        var attachment = attachments.FirstOrDefault(a => a.Id == attachmentId);

        return File(data, attachment?.ContentType ?? "application/octet-stream", attachment?.Name ?? attachmentId);
    }

    [HttpPost("entries/{entryId}/attachments")]
    public async Task<IActionResult> UploadAttachment(string entryId, IFormFile file)
    {
        var username = HttpContext.Items["Username"] as string;
        if (string.IsNullOrEmpty(username)) return Unauthorized();

        if (file == null || file.Length == 0)
        {
            return BadRequest("No file uploaded");
        }

        try
        {
            var attachmentId = await _vaultService.UploadAttachment(username, entryId, file);
            return CreatedAtAction(nameof(DownloadAttachment), new { entryId, attachmentId }, new { Id = attachmentId });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("entries/{entryId}/attachments/{attachmentId}")]
    public async Task<IActionResult> DeleteAttachment(string entryId, string attachmentId)
    {
        var username = HttpContext.Items["Username"] as string;
        if (string.IsNullOrEmpty(username)) return Unauthorized();

        var result = await _vaultService.DeleteAttachment(username, entryId, attachmentId);
        if (!result) return NotFound();

        return Ok();
    }
}