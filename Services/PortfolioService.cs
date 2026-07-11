using Microsoft.EntityFrameworkCore;
using PassXYZ.Server.Data;
using PassXYZ.Server.DTOs.Portfolio;
using PassXYZ.Server.Models;

namespace PassXYZ.Server.Services;

public class PortfolioService : IPortfolioService
{
    private readonly IDashboardDbContextFactory _dbContextFactory;

    public PortfolioService(IDashboardDbContextFactory dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    private DashboardDbContext GetDbContext(string userId)
    {
        return _dbContextFactory.Create(userId);
    }

    public async Task<IEnumerable<StockDto>> GetStocksAsync(string userId)
    {
        using var db = GetDbContext(userId);

        var stocks = await db.Stocks
            .Where(s => s.UserId == userId)
            .ToListAsync();

        return stocks.Select(ConvertToStockDto);
    }

    public async Task<StockDto> GetStockAsync(string userId, string symbol)
    {
        using var db = GetDbContext(userId);

        var stock = await db.Stocks
            .FirstOrDefaultAsync(s => s.UserId == userId && s.Symbol == symbol);

        return stock != null ? ConvertToStockDto(stock) : null!;
    }

    public async Task<StockDto> CreateStockAsync(string userId, StockCreateRequest request)
    {
        using var db = GetDbContext(userId);

        var now = DateTime.UtcNow.ToString("o");

        var stock = new Stock
        {
            Symbol = request.Symbol,
            UserId = userId,
            Name = request.Name ?? request.Symbol,
            AvgCost = request.AvgCost,
            Quantity = request.Quantity,
            TotalValue = request.TotalValue,
            CurrentPrice = request.CurrentPrice,
            FiftyTwoWeekLow = request.FiftyTwoWeekLow,
            FiftyTwoWeekHigh = request.FiftyTwoWeekHigh,
            DividendYield = request.DividendYield,
            LatestDividend = request.LatestDividend,
            Strategy = request.Strategy,
            TradingView = request.TradingView,
            CreatedAt = now,
            UpdatedAt = now
        };

        db.Stocks.Add(stock);
        await db.SaveChangesAsync();

        return ConvertToStockDto(stock);
    }

    public async Task<StockDto> UpdateStockAsync(string userId, string symbol, StockUpdateRequest request)
    {
        using var db = GetDbContext(userId);

        var stock = await db.Stocks
            .FirstOrDefaultAsync(s => s.UserId == userId && s.Symbol == symbol);

        if (stock == null)
        {
            return null!;
        }

        if (!string.IsNullOrEmpty(request.Name))
        {
            stock.Name = request.Name;
        }

        if (request.AvgCost.HasValue)
        {
            stock.AvgCost = request.AvgCost.Value;
        }

        if (request.Quantity.HasValue)
        {
            stock.Quantity = request.Quantity.Value;
        }

        if (request.TotalValue.HasValue)
        {
            stock.TotalValue = request.TotalValue.Value;
        }

        stock.UpdatedAt = DateTime.UtcNow.ToString("o");

        await db.SaveChangesAsync();

        return ConvertToStockDto(stock);
    }

    public async Task DeleteStockAsync(string userId, string symbol)
    {
        using var db = GetDbContext(userId);

        var stock = await db.Stocks
            .FirstOrDefaultAsync(s => s.UserId == userId && s.Symbol == symbol);

        if (stock != null)
        {
            db.Stocks.Remove(stock);
            await db.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<TransactionDto>> GetTransactionsAsync(string userId, string? symbol, string? startDate, string? endDate)
    {
        using var db = GetDbContext(userId);

        var query = db.Transactions.AsQueryable().Where(t => t.UserId == userId);

        if (!string.IsNullOrEmpty(symbol))
        {
            query = query.Where(t => t.Symbol == symbol);
        }

        if (!string.IsNullOrEmpty(startDate))
        {
            query = query.Where(t => string.Compare(t.Date, startDate) >= 0);
        }

        if (!string.IsNullOrEmpty(endDate))
        {
            query = query.Where(t => string.Compare(t.Date, endDate) <= 0);
        }

        var transactions = await query.OrderByDescending(t => t.Date).ToListAsync();

        return transactions.Select(ConvertToTransactionDto);
    }

    public async Task<TransactionDto> GetTransactionAsync(string userId, int transactionId)
    {
        using var db = GetDbContext(userId);

        var transaction = await db.Transactions
            .FirstOrDefaultAsync(t => t.UserId == userId && t.Id == transactionId);

        return transaction != null ? ConvertToTransactionDto(transaction) : null!;
    }

    public async Task<TransactionDto> CreateTransactionAsync(string userId, TransactionCreateRequest request)
    {
        using var db = GetDbContext(userId);

        var now = DateTime.UtcNow.ToString("o");

        var transaction = new Transaction
        {
            UserId = userId,
            Symbol = request.Symbol,
            Name = request.Name,
            Date = request.Date ?? DateTime.UtcNow.ToString("yyyy-MM-dd"),
            Price = request.Price,
            Quantity = request.Quantity,
            TransactionType = request.TransactionType,
            TotalValue = request.TotalValue ?? request.Price * request.Quantity,
            BaseValue = request.BaseValue ?? request.Price * request.Quantity,
            TransactionFee = request.TransactionFee ?? 0,
            CreatedAt = now,
            UpdatedAt = now
        };

        db.Transactions.Add(transaction);
        await db.SaveChangesAsync();

        return ConvertToTransactionDto(transaction);
    }

    public async Task<TransactionDto> UpdateTransactionAsync(string userId, int transactionId, TransactionUpdateRequest request)
    {
        using var db = GetDbContext(userId);

        var transaction = await db.Transactions
            .FirstOrDefaultAsync(t => t.UserId == userId && t.Id == transactionId);

        if (transaction == null)
        {
            return null!;
        }

        if (!string.IsNullOrEmpty(request.Date))
        {
            transaction.Date = request.Date;
        }

        if (!string.IsNullOrEmpty(request.Symbol))
        {
            transaction.Symbol = request.Symbol;
        }

        if (!string.IsNullOrEmpty(request.Name))
        {
            transaction.Name = request.Name;
        }

        if (request.Price.HasValue)
        {
            transaction.Price = request.Price.Value;
        }

        if (request.Quantity.HasValue)
        {
            transaction.Quantity = request.Quantity.Value;
        }

        if (!string.IsNullOrEmpty(request.TransactionType))
        {
            transaction.TransactionType = request.TransactionType;
        }

        if (request.TotalValue.HasValue)
        {
            transaction.TotalValue = request.TotalValue.Value;
        }

        if (request.BaseValue.HasValue)
        {
            transaction.BaseValue = request.BaseValue.Value;
        }

        if (request.TransactionFee.HasValue)
        {
            transaction.TransactionFee = request.TransactionFee.Value;
        }

        transaction.UpdatedAt = DateTime.UtcNow.ToString("o");

        await db.SaveChangesAsync();

        return ConvertToTransactionDto(transaction);
    }

    public async Task DeleteTransactionAsync(string userId, int transactionId)
    {
        using var db = GetDbContext(userId);

        var transaction = await db.Transactions
            .FirstOrDefaultAsync(t => t.UserId == userId && t.Id == transactionId);

        if (transaction != null)
        {
            db.Transactions.Remove(transaction);
            await db.SaveChangesAsync();
        }
    }

    private StockDto ConvertToStockDto(Stock stock)
    {
        return new StockDto
        {
            Symbol = stock.Symbol,
            Name = stock.Name,
            AvgCost = stock.AvgCost,
            Quantity = stock.Quantity,
            TotalValue = stock.TotalValue,
            CurrentPrice = stock.CurrentPrice,
            FiftyTwoWeekLow = stock.FiftyTwoWeekLow,
            FiftyTwoWeekHigh = stock.FiftyTwoWeekHigh,
            DividendYield = stock.DividendYield,
            LatestDividend = stock.LatestDividend,
            Strategy = stock.Strategy,
            TradingView = stock.TradingView,
            CreatedAt = stock.CreatedAt,
            UpdatedAt = stock.UpdatedAt
        };
    }

    private TransactionDto ConvertToTransactionDto(Transaction transaction)
    {
        return new TransactionDto
        {
            Id = transaction.Id,
            Symbol = transaction.Symbol,
            Name = transaction.Name,
            Date = transaction.Date,
            Price = transaction.Price,
            Quantity = transaction.Quantity,
            TransactionType = transaction.TransactionType,
            TotalValue = transaction.TotalValue,
            BaseValue = transaction.BaseValue,
            TransactionFee = transaction.TransactionFee,
            CreatedAt = transaction.CreatedAt,
            UpdatedAt = transaction.UpdatedAt
        };
    }
}