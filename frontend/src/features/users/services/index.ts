import {
  UserRole,
  EnrollmentStatus
} from '../types';
import type {
  UserListResponse,
  UserResponse,
  UserFilterRequest,
  CreateUserRequest,
  UpdateUserRequest,
  UserStatisticsResponse,
  UserDashboardResponse,
  GetUserEnrollmentsResponse,
  GetUserQuizAttemptsResponse,
  GetUserInstructedCoursesResponse,
  User,
} from '../types';
import ApiClient from '../../../lib/api-client';

const API_BASE_URL = ApiClient.getUrl('users');

interface ApiResponse<T> {
  data: T;
  message: string;
  success: boolean;
}

class UsersApiService {
  private async request<T>(endpoint: string, options?: RequestInit): Promise<T> {
    const url = `${API_BASE_URL}${endpoint}`;
    const response = await fetch(url, {
      headers: {
        'Content-Type': 'application/json',
        ...options?.headers,
      },
      ...options,
    });

    if (!response.ok) {
      throw new Error(`API request failed: ${response.statusText}`);
    }

    const result: ApiResponse<T> = await response.json();
    return result.data;
  }

  // User CRUD Operations
  async getUsers(filters?: UserFilterRequest): Promise<UserListResponse> {
    const params = new URLSearchParams();
    if (filters?.role) params.append('role', filters.role.toString());
    if (filters?.isActive !== undefined) params.append('isActive', filters.isActive.toString());
    if (filters?.searchTerm) params.append('searchTerm', filters.searchTerm);
    if (filters?.page) params.append('page', filters.page.toString());
    if (filters?.pageSize) params.append('pageSize', filters.pageSize.toString());

    const queryString = params.toString();
    const endpoint = queryString ? `?${queryString}` : '';
    return this.request<UserListResponse>(endpoint);
  }

  async getUserById(id: number): Promise<User> {
    return this.request<User>(`/${id}`);
  }

  async createUser(userData: CreateUserRequest): Promise<UserResponse> {
    return this.request<UserResponse>('', {
      method: 'POST',
      body: JSON.stringify(userData),
    });
  }

  async updateUser(id: number, userData: UpdateUserRequest): Promise<UserResponse> {
    return this.request<UserResponse>(`/${id}`, {
      method: 'PUT',
      body: JSON.stringify(userData),
    });
  }

  async deleteUser(id: number): Promise<{ success: boolean }> {
    return this.request<{ success: boolean }>(`/${id}`, {
      method: 'DELETE',
    });
  }

  // User Role Management
  async getInstructors(filters?: Omit<UserFilterRequest, 'role'>): Promise<UserListResponse> {
    const params = new URLSearchParams();
    if (filters?.isActive !== undefined) params.append('isActive', filters.isActive.toString());
    if (filters?.searchTerm) params.append('searchTerm', filters.searchTerm);
    if (filters?.page) params.append('page', filters.page.toString());
    if (filters?.pageSize) params.append('pageSize', filters.pageSize.toString());

    const queryString = params.toString();
    const endpoint = `/instructors${queryString ? `?${queryString}` : ''}`;
    return this.request<UserListResponse>(endpoint);
  }

  async getStudents(filters?: Omit<UserFilterRequest, 'role'>): Promise<UserListResponse> {
    const params = new URLSearchParams();
    if (filters?.isActive !== undefined) params.append('isActive', filters.isActive.toString());
    if (filters?.searchTerm) params.append('searchTerm', filters.searchTerm);
    if (filters?.page) params.append('page', filters.page.toString());
    if (filters?.pageSize) params.append('pageSize', filters.pageSize.toString());

    const queryString = params.toString();
    const endpoint = `/students${queryString ? `?${queryString}` : ''}`;
    return this.request<UserListResponse>(endpoint);
  }

  async getAdmins(filters?: Omit<UserFilterRequest, 'role'>): Promise<UserListResponse> {
    const params = new URLSearchParams();
    if (filters?.isActive !== undefined) params.append('isActive', filters.isActive.toString());
    if (filters?.searchTerm) params.append('searchTerm', filters.searchTerm);
    if (filters?.page) params.append('page', filters.page.toString());
    if (filters?.pageSize) params.append('pageSize', filters.pageSize.toString());

    const queryString = params.toString();
    const endpoint = `/admins${queryString ? `?${queryString}` : ''}`;
    return this.request<UserListResponse>(endpoint);
  }

  async activateUser(id: number): Promise<UserResponse> {
    return this.request<UserResponse>(`/${id}/activate`, {
      method: 'PUT',
    });
  }

  async deactivateUser(id: number): Promise<UserResponse> {
    return this.request<UserResponse>(`/${id}/deactivate`, {
      method: 'PUT',
    });
  }

  // User Learning Data
  async getUserEnrollments(
    id: number, 
    page = 1, 
    pageSize = 10, 
    status?: EnrollmentStatus
  ): Promise<GetUserEnrollmentsResponse> {
    const params = new URLSearchParams();
    params.append('page', page.toString());
    params.append('pageSize', pageSize.toString());
    if (status) params.append('status', status);

    return this.request<GetUserEnrollmentsResponse>(`/${id}/enrollments?${params.toString()}`);
  }

  async getUserQuizAttempts(
    id: number, 
    courseId?: number, 
    isPassed?: boolean, 
    page = 1, 
    pageSize = 10
  ): Promise<GetUserQuizAttemptsResponse> {
    const params = new URLSearchParams();
    if (courseId) params.append('courseId', courseId.toString());
    if (isPassed !== undefined) params.append('isPassed', isPassed.toString());
    params.append('page', page.toString());
    params.append('pageSize', pageSize.toString());

    return this.request<GetUserQuizAttemptsResponse>(`/${id}/quiz-attempts?${params.toString()}`);
  }

  async getUserInstructedCourses(
    id: number, 
    isPublished?: boolean, 
    page = 1, 
    pageSize = 10
  ): Promise<GetUserInstructedCoursesResponse> {
    const params = new URLSearchParams();
    if (isPublished !== undefined) params.append('isPublished', isPublished.toString());
    params.append('page', page.toString());
    params.append('pageSize', pageSize.toString());

    return this.request<GetUserInstructedCoursesResponse>(`/${id}/instructed-courses?${params.toString()}`);
  }

  // User Profile & Dashboard
  async getUserProfile(id: number): Promise<UserResponse> {
    return this.request<UserResponse>(`/${id}/profile`);
  }

  async updateUserProfile(id: number, userData: UpdateUserRequest): Promise<UserResponse> {
    return this.request<UserResponse>(`/${id}/profile`, {
      method: 'PUT',
      body: JSON.stringify(userData),
    });
  }

  async getUserDashboard(id: number): Promise<UserDashboardResponse> {
    return this.request<UserDashboardResponse>(`/${id}/dashboard`);
  }

  // User Search & Discovery
  async searchUsers(
    searchTerm: string, 
    role?: UserRole, 
    isActive?: boolean, 
    page = 1, 
    pageSize = 10
  ): Promise<UserListResponse> {
    const params = new URLSearchParams();
    params.append('searchTerm', searchTerm);
    if (role) params.append('role', role.toString());
    if (isActive !== undefined) params.append('isActive', isActive.toString());
    params.append('page', page.toString());
    params.append('pageSize', pageSize.toString());

    return this.request<UserListResponse>(`/search?${params.toString()}`);
  }

  async getActiveUsersCount(): Promise<{ count: number }> {
    return this.request<{ count: number }>('/count/active');
  }

  async getUsersStatistics(): Promise<UserStatisticsResponse> {
    return this.request<UserStatisticsResponse>('/statistics');
  }
}

export const usersApi = new UsersApiService();