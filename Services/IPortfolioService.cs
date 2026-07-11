using PassXYZ.Server.DTOs.Portfolio;

namespace PassXYZ.Server.Services;

public interface IPortfolioService
{
    Task<IEnumerable<StockDto>> GetStocksAsync(string userId);
    Task<StockDto> GetStockAsync(string userId, string symbol);
    Task<StockDto> CreateStockAsync(string userId, StockCreateRequest request);
    Task<StockDto> UpdateStockAsync(string userId, string symbol, StockUpdateRequest request);
    Task DeleteStockAsync(string userId, string symbol);
    Task<IEnumerable<TransactionDto>> GetTransactionsAsync(string userId, string? symbol, string? startDate, string? endDate);
    Task<TransactionDto> GetTransactionAsync(string userId, int transactionId);
    Task<TransactionDto> CreateTransactionAsync(string userId, TransactionCreateRequest request);
    Task<TransactionDto> UpdateTransactionAsync(string userId, int transactionId, TransactionUpdateRequest request);
    Task DeleteTransactionAsync(string userId, int transactionId);
}