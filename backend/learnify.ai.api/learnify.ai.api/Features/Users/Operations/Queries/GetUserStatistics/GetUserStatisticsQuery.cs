using FluentValidation;
using MediatR;
using learnify.ai.api.Common.Interfaces;
using learnify.ai.api.Features.Users.Data;
using learnify.ai.api.Features.Users.Contracts.Responses;
using learnify.ai.api.Features.Users.Models;

namespace learnify.ai.api.Features.Users.Operations.Queries.GetUserStatistics;

public record GetUserStatisticsQuery() : IQuery<UserStatisticsResponse>;

public class GetUserStatisticsValidator : AbstractValidator<GetUserStatisticsQuery>
{
    public GetUserStatisticsValidator()
    {
        // No validation needed for this query as it has no parameters
    }
}

public class GetUserStatisticsHandler : IRequestHandler<GetUserStatisticsQuery, UserStatisticsResponse>
{
    private readonly IUserRepository _userRepository;

    public GetUserStatisticsHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserStatisticsResponse> Handle(GetUserStatisticsQuery request, CancellationToken cancellationToken)
    {
        // Get all users for analysis
        var allUsers = await _userRepository.GetAllAsync(cancellationToken);
        var usersList = allUsers.ToList();

        // Calculate basic counts
        var totalUsers = usersList.Count;
        var activeUsers = usersList.Count(u => u.IsActive);
        var inactiveUsers = totalUsers - activeUsers;

        // Calculate role counts
        var instructorCount = usersList.Count(u => u.Role == UserRole.Instructor);
        var studentCount = usersList.Count(u => u.Role == UserRole.Student);
        var adminCount = usersList.Count(u => u.Role == UserRole.Admin);

        // Calculate role distribution percentages
        var roleDistribution = CalculateRoleDistribution(totalUsers, studentCount, instructorCount, adminCount);

        // Calculate activity stats
        var activityStats = CalculateActivityStats(usersList, activeUsers, totalUsers);

        // Calculate growth stats
        var growthStats = CalculateGrowthStats(usersList);

        return new UserStatisticsResponse(
            totalUsers,
            activeUsers,
            inactiveUsers,
            instructorCount,
            studentCount,
            adminCount,
            roleDistribution,
            activityStats,
            growthStats,
            DateTime.UtcNow
        );
    }

    private static UserRoleDistribution CalculateRoleDistribution(int totalUsers, int studentCount, int instructorCount, int adminCount)
    {
        if (totalUsers == 0)
        {
            return new UserRoleDistribution(0, 0, 0);
        }

        var studentPercentage = Math.Round((decimal)studentCount / totalUsers * 100, 2);
        var instructorPercentage = Math.Round((decimal)instructorCount / totalUsers * 100, 2);
        var adminPercentage = Math.Round((decimal)adminCount / totalUsers * 100, 2);

        return new UserRoleDistribution(studentPercentage, instructorPercentage, adminPercentage);
    }

    private static UserActivityStats CalculateActivityStats(List<User> usersList, int activeUsers, int totalUsers)
    {
        var now = DateTime.UtcNow;
        var oneWeekAgo = now.AddDays(-7);
        var oneMonthAgo = now.AddDays(-30);

        // Calculate users with recent activity (based on UpdatedAt as a proxy for recent activity)
        var usersWithRecentActivity = usersList.Count(u => u.UpdatedAt >= oneWeekAgo);
        var usersActiveThisWeek = usersList.Count(u => u.IsActive && u.UpdatedAt >= oneWeekAgo);
        var usersActiveThisMonth = usersList.Count(u => u.IsActive && u.UpdatedAt >= oneMonthAgo);

        var activeUserPercentage = totalUsers > 0 ? Math.Round((decimal)activeUsers / totalUsers * 100, 2) : 0;

        return new UserActivityStats(
            usersWithRecentActivity,
            usersActiveThisWeek,
            usersActiveThisMonth,
            activeUserPercentage
        );
    }

    private static UserGrowthStats CalculateGrowthStats(List<User> usersList)
    {
        var now = DateTime.UtcNow;
        var startOfThisMonth = new DateTime(now.Year, now.Month, 1);
        var startOfLastMonth = startOfThisMonth.AddMonths(-1);
        var oneWeekAgo = now.AddDays(-7);
        var startOfToday = now.Date;

        // Calculate new users for different periods
        var newUsersThisMonth = usersList.Count(u => u.CreatedAt >= startOfThisMonth);
        var newUsersLastMonth = usersList.Count(u => u.CreatedAt >= startOfLastMonth && u.CreatedAt < startOfThisMonth);
        var newUsersThisWeek = usersList.Count(u => u.CreatedAt >= oneWeekAgo);
        var newUsersToday = usersList.Count(u => u.CreatedAt >= startOfToday);

        // Calculate growth rate
        var growthRate = newUsersLastMonth > 0 
            ? Math.Round((decimal)(newUsersThisMonth - newUsersLastMonth) / newUsersLastMonth * 100, 2)
            : newUsersThisMonth > 0 ? 100m : 0m;

        return new UserGrowthStats(
            newUsersThisMonth,
            newUsersLastMonth,
            growthRate,
            newUsersThisWeek,
            newUsersToday
        );
    }
}