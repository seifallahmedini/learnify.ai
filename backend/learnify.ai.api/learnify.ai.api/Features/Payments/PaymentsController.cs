using Microsoft.AspNetCore.Mvc;
using learnify.ai.api.Common.Controllers;
using learnify.ai.api.Common.Models;

namespace learnify.ai.api.Features.Payments;

[ApiController]
[Route("api/[controller]")]
public class PaymentsController : BaseController
{
    #region Payment Processing

    /// <summary>
    /// Process payment for course
    /// </summary>
    [HttpPost("process")]
    public async Task<ActionResult<ApiResponse<ProcessPaymentResponse>>> ProcessPayment([FromBody] ProcessPaymentRequest request)
    {
        var command = new ProcessPaymentCommand(
            request.UserId,
            request.CourseId,
            request.Amount,
            request.Currency,
            request.PaymentMethod,
            request.PaymentMethodDetails
        );

        try
        {
            var result = await Mediator.Send(command);
            return Ok(result, "Payment processed successfully");
        }
        catch (ArgumentException ex)
        {
            return BadRequest<ProcessPaymentResponse>(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest<ProcessPaymentResponse>(ex.Message);
        }
    }

    /// <summary>
    /// Get payment details
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<PaymentResponse>>> GetPayment(int id)
    {
        var query = new GetPaymentByIdQuery(id);
        var result = await Mediator.Send(query);

        if (result == null)
            return NotFound<PaymentResponse>($"Payment with ID {id} not found");

        return Ok(result, "Payment retrieved successfully");
    }

    /// <summary>
    /// Process refund
    /// </summary>
    [HttpPost("{id:int}/refund")]
    public async Task<ActionResult<ApiResponse<RefundResponse>>> ProcessRefund(int id, [FromBody] ProcessRefundRequest request)
    {
        var command = new ProcessRefundCommand(id, request.RefundAmount, request.Reason);

        try
        {
            var result = await Mediator.Send(command);
            return Ok(result, "Refund processed successfully");
        }
        catch (ArgumentException ex)
        {
            return BadRequest<RefundResponse>(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest<RefundResponse>(ex.Message);
        }
    }

    #endregion

    #region Payment Management

    /// <summary>
    /// Get user payments - matches /api/users/{userId}/payments from features overview
    /// </summary>
    [HttpGet("users/{userId:int}/payments")]
    public async Task<ActionResult<ApiResponse<UserPaymentsResponse>>> GetUserPayments(
        int userId,
        [FromQuery] int? courseId = null,
        [FromQuery] PaymentStatus? status = null,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = new GetUserPaymentsQuery(userId, courseId, status, fromDate, toDate, page, pageSize);

        try
        {
            var result = await Mediator.Send(query);
            return Ok(result, "User payments retrieved successfully");
        }
        catch (ArgumentException)
        {
            return NotFound<UserPaymentsResponse>($"User with ID {userId} not found");
        }
    }

    /// <summary>
    /// Get course payments - matches /api/courses/{courseId}/payments from features overview
    /// </summary>
    [HttpGet("courses/{courseId:int}/payments")]
    public async Task<ActionResult<ApiResponse<CoursePaymentsResponse>>> GetCoursePayments(
        int courseId,
        [FromQuery] PaymentStatus? status = null,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = new GetCoursePaymentsQuery(courseId, status, fromDate, toDate, page, pageSize);

        try
        {
            var result = await Mediator.Send(query);
            return Ok(result, "Course payments retrieved successfully");
        }
        catch (ArgumentException)
        {
            return NotFound<CoursePaymentsResponse>($"Course with ID {courseId} not found");
        }
    }

    /// <summary>
    /// Get all transactions
    /// </summary>
    [HttpGet("transactions")]
    public async Task<ActionResult<ApiResponse<TransactionsResponse>>> GetTransactions(
        [FromQuery] int? userId = null,
        [FromQuery] int? courseId = null,
        [FromQuery] PaymentStatus? status = null,
        [FromQuery] PaymentMethod? paymentMethod = null,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = new GetTransactionsQuery(userId, courseId, status, paymentMethod, fromDate, toDate, page, pageSize);
        var result = await Mediator.Send(query);
        return Ok(result, "Transactions retrieved successfully");
    }

    #endregion

    #region Revenue Analytics

    /// <summary>
    /// Get total revenue
    /// </summary>
    [HttpGet("revenue")]
    public async Task<ActionResult<ApiResponse<TotalRevenueResponse>>> GetTotalRevenue(
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null,
        [FromQuery] string? currency = null)
    {
        var query = new GetTotalRevenueQuery(fromDate, toDate, currency);
        var result = await Mediator.Send(query);
        return Ok(result, "Total revenue retrieved successfully");
    }

    /// <summary>
    /// Get instructor revenue - matches /api/payments/revenue/instructor/{id} from features overview
    /// </summary>
    [HttpGet("revenue/instructor/{instructorId:int}")]
    public async Task<ActionResult<ApiResponse<InstructorRevenueResponse>>> GetInstructorRevenue(
        int instructorId,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null,
        [FromQuery] string? currency = null)
    {
        var query = new GetInstructorRevenueQuery(instructorId, fromDate, toDate, currency);

        try
        {
            var result = await Mediator.Send(query);
            return Ok(result, "Instructor revenue retrieved successfully");
        }
        catch (ArgumentException)
        {
            return NotFound<InstructorRevenueResponse>($"Instructor with ID {instructorId} not found");
        }
    }

    ///// <summary>
    ///// Get course revenue - matches /api/payments/revenue/course/{id} from features overview
    ///// </summary>
    //[HttpGet("revenue/course/{courseId:int}")]
    //public async Task<ActionResult<ApiResponse<CourseRevenueResponse>>> GetCourseRevenue(
    //    int courseId,
    //    [FromQuery] DateTime? fromDate = null,
    //    [FromQuery] DateTime? toDate = null,
    //    [FromQuery] string? currency = null)
    //{
    //    var query = new GetCourseRevenueQuery(courseId, fromDate, toDate, currency);

    //    try
    //    {
    //        var result = await Mediator.Send(query);
    //        return Ok(result, "Course revenue retrieved successfully");
    //    }
    //    catch (ArgumentException)
    //    {
    //        return NotFound<CourseRevenueResponse>($"Course with ID {courseId} not found");
    //    }
    //}

    ///// <summary>
    ///// Get monthly revenue
    ///// </summary>
    //[HttpGet("revenue/monthly")]
    //public async Task<ActionResult<ApiResponse<MonthlyRevenueResponse>>> GetMonthlyRevenue(
    //    [FromQuery] int year = 0,
    //    [FromQuery] int? month = null,
    //    [FromQuery] string? currency = null)
    //{
    //    var currentYear = year == 0 ? DateTime.UtcNow.Year : year;
    //    var query = new GetMonthlyRevenueQuery(currentYear, month, currency);
    //    var result = await Mediator.Send(query);
    //    return Ok(result, "Monthly revenue retrieved successfully");
    //}

    ///// <summary>
    ///// Get payment analytics
    ///// </summary>
    //[HttpGet("analytics")]
    //public async Task<ActionResult<ApiResponse<PaymentAnalyticsResponse>>> GetPaymentAnalytics(
    //    [FromQuery] DateTime? fromDate = null,
    //    [FromQuery] DateTime? toDate = null,
    //    [FromQuery] string? currency = null)
    //{
    //    var query = new GetPaymentAnalyticsQuery(fromDate, toDate, currency);
    //    var result = await Mediator.Send(query);
    //    return Ok(result, "Payment analytics retrieved successfully");
    //}

    #endregion

    #region Additional Payment Operations

    /// <summary>
    /// Get payment by transaction ID
    /// </summary>
    [HttpGet("transaction/{transactionId}")]
    public async Task<ActionResult<ApiResponse<PaymentResponse>>> GetPaymentByTransactionId(string transactionId)
    {
        var query = new GetPaymentByTransactionIdQuery(transactionId);
        var result = await Mediator.Send(query);

        if (result == null)
            return NotFound<PaymentResponse>($"Payment with transaction ID {transactionId} not found");

        return Ok(result, "Payment retrieved successfully");
    }

    /// <summary>
    /// Get user's successful course purchases
    /// </summary>
    [HttpGet("users/{userId:int}/purchases")]
    public async Task<ActionResult<ApiResponse<UserPaymentsResponse>>> GetUserPurchases(int userId)
    {
        var query = new GetUserPaymentsQuery(userId, Status: PaymentStatus.Completed);

        try
        {
            var result = await Mediator.Send(query);
            return Ok(result, "User purchases retrieved successfully");
        }
        catch (ArgumentException)
        {
            return NotFound<UserPaymentsResponse>($"User with ID {userId} not found");
        }
    }

    /// <summary>
    /// Check if user has purchased a specific course
    /// </summary>
    [HttpGet("users/{userId:int}/courses/{courseId:int}/purchased")]
    public async Task<ActionResult<ApiResponse<bool>>> HasUserPurchasedCourse(int userId, int courseId)
    {
        var query = new HasUserPurchasedCourseQuery(userId, courseId);
        var result = await Mediator.Send(query);
        return Ok(result, "Purchase status retrieved successfully");
    }

    #endregion
}