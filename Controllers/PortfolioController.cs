using Microsoft.AspNetCore.Mvc;
using PassXYZ.Server.DTOs.Portfolio;
using PassXYZ.Server.Services;

namespace PassXYZ.Server.Controllers;

[ApiController]
[Route("api/v1")]
public class PortfolioController : ControllerBase
{
    private readonly IPortfolioService _portfolioService;

    public PortfolioController(IPortfolioService portfolioService)
    {
        _portfolioService = portfolioService;
    }

    private string? GetUserId()
    {
        return HttpContext.Items["Username"] as string;
    }

    [HttpGet("portfolio/stocks")]
    public async Task<IActionResult> GetStocks()
    {
        var userId = GetUserId();
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var stocks = await _portfolioService.GetStocksAsync(userId);
        return Ok(stocks);
    }

    [HttpGet("portfolio/stocks/{symbol}")]
    public async Task<IActionResult> GetStock(string symbol)
    {
        var userId = GetUserId();
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var stock = await _portfolioService.GetStockAsync(userId, symbol);
        if (stock == null) return NotFound();

        return Ok(stock);
    }

    [HttpPost("portfolio/stocks")]
    public async Task<IActionResult> CreateStock([FromBody] StockCreateRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var userId = GetUserId();
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var stock = await _portfolioService.CreateStockAsync(userId, request);
        return Ok(stock);
    }

    [HttpPut("portfolio/stocks/{symbol}")]
    public async Task<IActionResult> UpdateStock(string symbol, [FromBody] StockUpdateRequest request)
    {
        var userId = GetUserId();
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var stock = await _portfolioService.UpdateStockAsync(userId, symbol, request);
        if (stock == null) return NotFound();

        return Ok(stock);
    }

    [HttpDelete("portfolio/stocks/{symbol}")]
    public async Task<IActionResult> DeleteStock(string symbol)
    {
        var userId = GetUserId();
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        await _portfolioService.DeleteStockAsync(userId, symbol);
        return Ok();
    }

    [HttpGet("portfolio/transactions")]
    public async Task<IActionResult> GetTransactions(
        [FromQuery] string? symbol,
        [FromQuery] string? start_date,
        [FromQuery] string? end_date)
    {
        var userId = GetUserId();
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var transactions = await _portfolioService.GetTransactionsAsync(userId, symbol, start_date, end_date);
        return Ok(transactions);
    }

    [HttpGet("portfolio/transactions/{transaction_id}")]
    public async Task<IActionResult> GetTransaction(int transaction_id)
    {
        var userId = GetUserId();
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var transaction = await _portfolioService.GetTransactionAsync(userId, transaction_id);
        if (transaction == null) return NotFound();

        return Ok(transaction);
    }

    [HttpPost("portfolio/transactions")]
    public async Task<IActionResult> CreateTransaction([FromBody] TransactionCreateRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var userId = GetUserId();
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var transaction = await _portfolioService.CreateTransactionAsync(userId, request);
        return Ok(transaction);
    }

    [HttpPut("portfolio/transactions/{transaction_id}")]
    public async Task<IActionResult> UpdateTransaction(int transaction_id, [FromBody] TransactionUpdateRequest request)
    {
        var userId = GetUserId();
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var transaction = await _portfolioService.UpdateTransactionAsync(userId, transaction_id, request);
        if (transaction == null) return NotFound();

        return Ok(transaction);
    }

    [HttpDelete("portfolio/transactions/{transaction_id}")]
    public async Task<IActionResult> DeleteTransaction(int transaction_id)
    {
        var userId = GetUserId();
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        await _portfolioService.DeleteTransactionAsync(userId, transaction_id);
        return Ok();
    }
}