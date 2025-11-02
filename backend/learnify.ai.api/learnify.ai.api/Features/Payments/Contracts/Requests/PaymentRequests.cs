using learnify.ai.api.Domain.Enums;

namespace learnify.ai.api.Features.Payments;

// Request DTOs for Payment endpoints
public record ProcessPaymentRequest(
    int UserId,
    int CourseId,
    decimal Amount,
    string Currency = "USD",
    PaymentMethod PaymentMethod = PaymentMethod.CreditCard,
    string? PaymentMethodDetails = null
);

public record ProcessRefundRequest(
    decimal? RefundAmount = null,
    string? Reason = null
);

public record GetUserPaymentsRequest(
    int? CourseId = null,
    PaymentStatus? Status = null,
    DateTime? FromDate = null,
    DateTime? ToDate = null,
    int Page = 1,
    int PageSize = 10
);

public record GetCoursePaymentsRequest(
    PaymentStatus? Status = null,
    DateTime? FromDate = null,
    DateTime? ToDate = null,
    int Page = 1,
    int PageSize = 10
);

public record GetTransactionsRequest(
    int? UserId = null,
    int? CourseId = null,
    PaymentStatus? Status = null,
    PaymentMethod? PaymentMethod = null,
    DateTime? FromDate = null,
    DateTime? ToDate = null,
    int Page = 1,
    int PageSize = 10
);

public record GetRevenueRequest(
    DateTime? FromDate = null,
    DateTime? ToDate = null,
    string? Currency = null
);

public record GetMonthlyRevenueRequest(
    int Year,
    int? Month = null,
    string? Currency = null
);
