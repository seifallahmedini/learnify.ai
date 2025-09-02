namespace learnify.ai.api.Features.Payments;

// Response DTOs for Payment endpoints
public record PaymentResponse(
    int Id,
    int UserId,
    string UserName,
    int CourseId,
    string CourseTitle,
    decimal Amount,
    string Currency,
    string FormattedAmount,
    PaymentMethod PaymentMethod,
    string TransactionId,
    PaymentStatus Status,
    DateTime PaymentDate,
    DateTime? RefundDate,
    decimal? RefundAmount,
    string? FormattedRefundAmount,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

public record PaymentSummaryResponse(
    int Id,
    int UserId,
    string UserName,
    int CourseId,
    string CourseTitle,
    decimal Amount,
    string FormattedAmount,
    PaymentStatus Status,
    DateTime PaymentDate
);

public record ProcessPaymentResponse(
    int PaymentId,
    string TransactionId,
    PaymentStatus Status,
    decimal Amount,
    string FormattedAmount,
    DateTime PaymentDate,
    string Message
);

public record RefundResponse(
    int PaymentId,
    string TransactionId,
    decimal RefundAmount,
    string FormattedRefundAmount,
    DateTime RefundDate,
    PaymentStatus Status,
    string Message
);

public record UserPaymentsResponse(
    int UserId,
    string UserName,
    IEnumerable<PaymentSummaryResponse> Payments,
    int TotalCount,
    int Page,
    int PageSize,
    int TotalPages,
    PaymentStatsResponse Stats
);

public record CoursePaymentsResponse(
    int CourseId,
    string CourseTitle,
    IEnumerable<PaymentSummaryResponse> Payments,
    int TotalCount,
    int Page,
    int PageSize,
    int TotalPages,
    decimal TotalRevenue,
    string FormattedTotalRevenue
);

public record TransactionsResponse(
    IEnumerable<PaymentSummaryResponse> Transactions,
    int TotalCount,
    int Page,
    int PageSize,
    int TotalPages,
    TransactionStatsResponse Stats
);

public record PaymentStatsResponse(
    int TotalPayments,
    int CompletedPayments,
    int RefundedPayments,
    int FailedPayments,
    decimal TotalSpent,
    string FormattedTotalSpent,
    decimal TotalRefunded,
    string FormattedTotalRefunded
);

public record TransactionStatsResponse(
    int TotalTransactions,
    decimal TotalRevenue,
    string FormattedTotalRevenue,
    decimal AverageTransactionValue,
    string FormattedAverageTransactionValue,
    IDictionary<PaymentStatus, int> StatusBreakdown,
    IDictionary<PaymentMethod, int> MethodBreakdown
);

public record TotalRevenueResponse(
    decimal TotalRevenue,
    string FormattedTotalRevenue,
    decimal ThisMonth,
    string FormattedThisMonth,
    decimal LastMonth,
    string FormattedLastMonth,
    double GrowthRate,
    string Currency,
    DateTime CalculatedAt
);

public record InstructorRevenueResponse(
    int InstructorId,
    string InstructorName,
    decimal TotalRevenue,
    string FormattedTotalRevenue,
    decimal ThisMonth,
    string FormattedThisMonth,
    decimal LastMonth,
    string FormattedLastMonth,
    double GrowthRate,
    int TotalSales,
    IEnumerable<CourseRevenueBreakdownResponse> CourseBreakdown
);

public record CourseRevenueResponse(
    int CourseId,
    string CourseTitle,
    int InstructorId,
    string InstructorName,
    decimal TotalRevenue,
    string FormattedTotalRevenue,
    decimal ThisMonth,
    string FormattedThisMonth,
    decimal LastMonth,
    string FormattedLastMonth,
    double GrowthRate,
    int TotalSales,
    decimal AveragePrice,
    string FormattedAveragePrice
);

public record CourseRevenueBreakdownResponse(
    int CourseId,
    string CourseTitle,
    decimal Revenue,
    string FormattedRevenue,
    int Sales
);

public record MonthlyRevenueResponse(
    int Year,
    int? Month,
    IEnumerable<MonthlyRevenueItemResponse> MonthlyData,
    decimal TotalRevenue,
    string FormattedTotalRevenue,
    string Currency
);

public record MonthlyRevenueItemResponse(
    int Year,
    int Month,
    string MonthName,
    decimal Revenue,
    string FormattedRevenue,
    int TransactionCount,
    decimal AverageTransactionValue,
    string FormattedAverageTransactionValue
);

public record PaymentAnalyticsResponse(
    TotalRevenueResponse RevenueOverview,
    TransactionStatsResponse TransactionStats,
    IEnumerable<MonthlyRevenueItemResponse> Last12Months,
    IEnumerable<TopCourseRevenueResponse> TopCourses,
    IEnumerable<TopInstructorRevenueResponse> TopInstructors,
    PaymentMethodAnalyticsResponse PaymentMethodStats
);

public record TopCourseRevenueResponse(
    int CourseId,
    string CourseTitle,
    decimal Revenue,
    string FormattedRevenue,
    int Sales
);

public record TopInstructorRevenueResponse(
    int InstructorId,
    string InstructorName,
    decimal Revenue,
    string FormattedRevenue,
    int Sales
);

public record PaymentMethodAnalyticsResponse(
    IDictionary<PaymentMethod, int> MethodUsage,
    IDictionary<PaymentMethod, decimal> MethodRevenue,
    PaymentMethod MostPopularMethod,
    PaymentMethod HighestRevenueMethod
);