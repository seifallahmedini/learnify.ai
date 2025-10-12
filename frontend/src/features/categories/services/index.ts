import { ApiClient } from '@/lib/api-client';
import type {
  ApiResponse,
  Category,
  CategorySummary,
  CategoryListResponse,
  CategoryDetailResponse,
  CreateCategoryRequest,
  UpdateCategoryRequest,
  CategoryFilterRequest,
} from '../types';

/**
 * API service for categories management
 * Handles all category-related HTTP operations with centralized error handling
 */
class CategoriesApiService {
  private baseUrl = ApiClient.getUrl('categories');

  /**
   * Private method to handle all HTTP requests with consistent error handling
   * @param endpoint - The API endpoint relative to baseUrl
   * @param options - Fetch options (method, body, headers, etc.)
   * @returns Promise with the API response data
   */
  private async request<T>(endpoint: string, options?: RequestInit): Promise<T> {
    const url = `${this.baseUrl}${endpoint}`;
    
    const response = await fetch(url, {
      headers: {
        'Content-Type': 'application/json',
        ...options?.headers,
      },
      ...options,
    });

    const result: ApiResponse<T> = await response.json();
    
    if (!result.success) {
      throw new Error(result.message || 'API request failed');
    }

    return result.data;
  }

  /**
   * Retrieve all categories with optional filtering
   * @param filters - Optional filtering criteria
   * @returns Promise with category list response
   */
  async getCategories(filters?: CategoryFilterRequest): Promise<CategoryListResponse> {
    const params = new URLSearchParams();
    
    if (filters?.parentId !== undefined) {
      params.append('parentId', filters.parentId?.toString() || 'null');
    }
    if (filters?.isActive !== undefined) {
      params.append('isActive', filters.isActive.toString());
    }
    if (filters?.searchTerm) {
      params.append('searchTerm', filters.searchTerm);
    }
    if (filters?.sortBy) {
      params.append('sortBy', filters.sortBy);
    }
    if (filters?.sortDirection) {
      params.append('sortDirection', filters.sortDirection);
    }
    if (filters?.page) {
      params.append('page', filters.page.toString());
    }
    if (filters?.pageSize) {
      params.append('pageSize', filters.pageSize.toString());
    }

    const queryString = params.toString();
    const endpoint = queryString ? `?${queryString}` : '';
    
    return this.request<CategoryListResponse>(endpoint);
  }

  /**
   * Retrieve a specific category by ID with detailed information
   * @param id - Category ID
   * @returns Promise with detailed category information
   */
  async getCategoryById(id: number): Promise<CategoryDetailResponse> {
    return this.request<CategoryDetailResponse>(`/${id}`);
  }

  /**
   * Create a new category
   * @param data - Category creation data
   * @returns Promise with the created category
   */
  async createCategory(data: CreateCategoryRequest): Promise<Category> {
    return this.request<Category>('', {
      method: 'POST',
      body: JSON.stringify(data),
    });
  }

  /**
   * Update an existing category
   * @param id - Category ID
   * @param data - Category update data
   * @returns Promise with the updated category
   */
  async updateCategory(id: number, data: UpdateCategoryRequest): Promise<Category> {
    return this.request<Category>(`/${id}`, {
      method: 'PUT',
      body: JSON.stringify(data),
    });
  }

  /**
   * Delete a category
   * @param id - Category ID
   * @returns Promise with deletion confirmation
   */
  async deleteCategory(id: number): Promise<{ success: boolean, message?: string }> {
    return this.request<{ success: boolean }>(`/${id}`, {
      method: 'DELETE',
    });
  }

  /**
   * Get all parent categories (categories without a parent)
   * @returns Promise with list of parent categories
   */
  async getParentCategories(): Promise<CategorySummary[]> {
    return this.request<CategorySummary[]>('/parents');
  }

  /**
   * Get subcategories for a specific parent category
   * @param parentId - Parent category ID
   * @returns Promise with list of subcategories
   */
  async getSubcategories(parentId: number): Promise<CategorySummary[]> {
    return this.request<CategorySummary[]>(`/${parentId}/subcategories`);
  }

  /**
   * Get category statistics for analytics
   * @returns Promise with category statistics
   */
  async getCategoryStatistics(): Promise<{
    totalCategories: number;
    activeCategories: number;
    categoriesWithCourses: number;
    avgCoursesPerCategory: number;
  }> {
    return this.request(`/statistics`);
  }

  /**
   * Reorder categories (update sort order)
   * @param updates - Array of category ID and new sort order pairs
   * @returns Promise with success confirmation
   */
  async reorderCategories(updates: { id: number; sortOrder: number }[]): Promise<{ success: boolean }> {
    return this.request<{ success: boolean }>('/reorder', {
      method: 'PUT',
      body: JSON.stringify({ updates }),
    });
  }
}

// Export singleton instance
export const categoriesApi = new CategoriesApiService();