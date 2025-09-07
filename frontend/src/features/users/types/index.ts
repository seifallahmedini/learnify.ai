// User-related types and interfaces based on the UsersController.cs

// API Response wrapper
export interface ApiResponse<T> {
  success: boolean;
  data: T;
  message: string;
  errors?: string[];
}

export interface User {
  id: number;
  firstName: string;
  lastName: string;
  fullName: string;
  email: string;
  role: UserRole;
  isActive: boolean;
  profilePicture?: string;
  bio?: string;
  dateOfBirth?: string;
  phoneNumber?: string;
  createdAt: string;
  updatedAt: string;
}

export const UserRole = {
  Student: 1,
  Instructor: 2,
  Admin: 3
} as const;

export type UserRole = typeof UserRole[keyof typeof UserRole];

export interface UserSummary {
  id: number;
  fullName: string;
  email: string;
  role: UserRole;
  isActive: boolean;
  createdAt: string;
}

export interface UserListResponse {
  users: UserSummary[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
  hasNextPage: boolean;
  hasPreviousPage: boolean;
}

export interface UserResponse {
  user: User;
}

export interface UserFilterRequest {
  role?: UserRole;
  isActive?: boolean;
  searchTerm?: string;
  page?: number;
  pageSize?: number;
}

export interface CreateUserRequest {
  firstName: string;
  lastName: string;
  email: string;
  password: string;
  role: UserRole;
  bio?: string;
  dateOfBirth?: string;
  phoneNumber?: string;
}

export interface UpdateUserRequest {
  firstName?: string;
  lastName?: string;
  bio?: string;
  dateOfBirth?: string;
  phoneNumber?: string;
  isActive?: boolean;
}

export interface UserStatisticsResponse {
  totalUsers: number;
  activeUsers: number;
  inactiveUsers: number;
  roleDistribution: {
    students: number;
    instructors: number;
    admins: number;
  };
  recentActivity: {
    newUsersThisMonth: number;
    newUsersThisWeek: number;
    activatedUsers: number;
    deactivatedUsers: number;
  };
}

export interface UserDashboardResponse {
  user: User;
  enrollmentStats: {
    totalEnrollments: number;
    completedCourses: number;
    inProgressCourses: number;
    averageProgress: number;
  };
  quizPerformance: {
    totalAttempts: number;
    averageScore: number;
    passedQuizzes: number;
    failedQuizzes: number;
  };
  recentActivity: {
    enrollments: any[];
    quizAttempts: any[];
    courseCreations: any[];
  };
  courseAnalytics?: {
    totalCourses: number;
    publishedCourses: number;
    draftCourses: number;
    totalStudents: number;
    averageRating: number;
  };
}

// Enrollment related types
export const EnrollmentStatus = {
  Active: 'Active',
  Completed: 'Completed',
  Dropped: 'Dropped',
  Suspended: 'Suspended'
} as const;

export type EnrollmentStatus = typeof EnrollmentStatus[keyof typeof EnrollmentStatus];

export interface UserEnrollment {
  id: number;
  courseId: number;
  courseName: string;
  courseTitle: string;
  instructorName: string;
  status: EnrollmentStatus;
  progress: number;
  enrolledAt: string;
  completedAt?: string;
  lastAccessedAt?: string;
  totalLessons: number;
  completedLessons: number;
}

export interface GetUserEnrollmentsResponse {
  enrollments: UserEnrollment[];
  totalCount: number;
  page: number;
  pageSize: number;
}

export interface UserQuizAttempt {
  id: number;
  quizId: number;
  quizTitle: string;
  courseId: number;
  courseName: string;
  score: number;
  maxScore: number;
  percentage: number;
  isPassed: boolean;
  attemptedAt: string;
  completedAt?: string;
  timeSpent: number;
  totalQuestions: number;
  correctAnswers: number;
}

export interface GetUserQuizAttemptsResponse {
  quizAttempts: UserQuizAttempt[];
  totalCount: number;
  page: number;
  pageSize: number;
}

export interface UserInstructedCourse {
  id: number;
  title: string;
  description: string;
  isPublished: boolean;
  totalStudents: number;
  averageRating: number;
  totalLessons: number;
  createdAt: string;
  updatedAt: string;
  category: string;
  price: number;
}

export interface GetUserInstructedCoursesResponse {
  courses: UserInstructedCourse[];
  totalCount: number;
  page: number;
  pageSize: number;
}