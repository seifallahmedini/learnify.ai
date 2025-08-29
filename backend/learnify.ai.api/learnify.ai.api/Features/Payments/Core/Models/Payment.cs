namespace learnify.ai.api.Features.Payments.Core.Models;

public class Payment
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int CourseId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";
    public PaymentMethod PaymentMethod { get; set; }
    public string TransactionId { get; set; } = string.Empty;
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
    public DateTime PaymentDate { get; set; }
    public DateTime? RefundDate { get; set; }
    public decimal? RefundAmount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Business methods
    public bool IsCompleted() => Status == PaymentStatus.Completed;
    public bool IsRefunded() => Status == PaymentStatus.Refunded;
    public bool CanBeRefunded() => Status == PaymentStatus.Completed && RefundDate == null;
    
    public void ProcessRefund(decimal refundAmount)
    {
        if (!CanBeRefunded()) throw new InvalidOperationException("Payment cannot be refunded");
        
        RefundAmount = refundAmount;
        RefundDate = DateTime.UtcNow;
        Status = PaymentStatus.Refunded;
        UpdatedAt = DateTime.UtcNow;
    }

    public string GetFormattedAmount() => $"{Amount:C} {Currency}";
}

public enum PaymentMethod
{
    CreditCard = 1,
    PayPal = 2,
    BankTransfer = 3,
    Cryptocurrency = 4,
    Cash = 5
}

public enum PaymentStatus
{
    Pending = 1,
    Completed = 2,
    Failed = 3,
    Refunded = 4,
    Cancelled = 5
}