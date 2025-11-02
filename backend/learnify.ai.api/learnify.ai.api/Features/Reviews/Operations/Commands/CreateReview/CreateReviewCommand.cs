using FluentValidation;
using MediatR;
using learnify.ai.api.Common.Abstractions;
using learnify.ai.api.Domain.Entities;
using learnify.ai.api.Features.Users;
using learnify.ai.api.Features.Courses;
using learnify.ai.api.Features.Enrollments;

namespace learnify.ai.api.Features.Reviews;

public record CreateReviewCommand(
    int CourseId,
    int UserId,
    int Rating,
    string? Comment = null
) : ICommand<ReviewResponse>;

public class CreateReviewValidator : AbstractValidator<CreateReviewCommand>
{
    public CreateReviewValidator()
    {
        RuleFor(x => x.CourseId)
            .GreaterThan(0)
            .WithMessage("Course ID must be greater than 0");

        RuleFor(x => x.UserId)
            .GreaterThan(0)
            .WithMessage("User ID must be greater than 0");

        RuleFor(x => x.Rating)
            .InclusiveBetween(1, 5)
            .WithMessage("Rating must be between 1 and 5");

        RuleFor(x => x.Comment)
            .MaximumLength(1000)
            .WithMessage("Comment cannot exceed 1000 characters")
            .When(x => !string.IsNullOrEmpty(x.Comment));
    }
}

public class CreateReviewHandler : IRequestHandler<CreateReviewCommand, ReviewResponse>
{
    private readonly IReviewRepository _reviewRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICourseRepository _courseRepository;
    private readonly IEnrollmentRepository _enrollmentRepository;

    public CreateReviewHandler(
        IReviewRepository reviewRepository,
        IUserRepository userRepository,
        ICourseRepository courseRepository,
        IEnrollmentRepository enrollmentRepository)
    {
        _reviewRepository = reviewRepository;
        _userRepository = userRepository;
        _courseRepository = courseRepository;
        _enrollmentRepository = enrollmentRepository;
    }

    public async Task<ReviewResponse> Handle(CreateReviewCommand request, CancellationToken cancellationToken)
    {
        // Verify user exists
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user == null)
            throw new ArgumentException($"User with ID {request.UserId} not found");

        // Verify course exists
        var course = await _courseRepository.GetByIdAsync(request.CourseId, cancellationToken);
        if (course == null)
            throw new ArgumentException($"Course with ID {request.CourseId} not found");

        // Check if user is enrolled in the course
        var enrollment = await _enrollmentRepository.GetByUserAndCourseAsync(request.UserId, request.CourseId, cancellationToken);
        if (enrollment == null)
            throw new InvalidOperationException("User must be enrolled in the course to leave a review");

        // Check if user has already reviewed this course
        var existingReview = await _reviewRepository.GetByUserAndCourseAsync(request.UserId, request.CourseId, cancellationToken);
        if (existingReview != null)
            throw new InvalidOperationException("User has already reviewed this course");

        // Create review
        var review = new Review
        {
            CourseId = request.CourseId,
            UserId = request.UserId,
            Rating = request.Rating,
            Comment = request.Comment,
            IsApproved = false, // Reviews need approval by default
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var createdReview = await _reviewRepository.CreateAsync(review, cancellationToken);

        return new ReviewResponse(
            createdReview.Id,
            createdReview.CourseId,
            course.Title,
            createdReview.UserId,
            user.GetFullName(),
            createdReview.Rating,
            createdReview.Comment,
            createdReview.IsApproved,
            createdReview.CreatedAt,
            createdReview.UpdatedAt
        );
    }
}
