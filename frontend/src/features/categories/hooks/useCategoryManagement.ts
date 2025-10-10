import { useState, useCallback } from 'react';
import { categoriesApi } from '../services';
import type {
  Category,
  CategorySummary,
  CategoryListResponse,
  CategoryDetailResponse,
  CreateCategoryRequest,
  UpdateCategoryRequest,
  CategoryFilterRequest,
} from '../types';

/**
 * Hook for managing category operations and state
 * Provides CRUD operations, loading states, and error handling for categories
 */
export function useCategoryManagement() {
  // State management
  const [categories, setCategories] = useState<CategorySummary[]>([]);
  const [selectedCategory, setSelectedCategory] = useState<Category | null>(null);
  const [loading, setLoading] = useState(true);
  const [isCreating, setIsCreating] = useState(false);
  const [isUpdating, setIsUpdating] = useState(false);
  const [isDeleting, setIsDeleting] = useState(false);
  const [isLoadingCategory, setIsLoadingCategory] = useState(false);
  const [error, setError] = useState<string | null>(null);

  // Pagination and filtering state
  const [totalCount, setTotalCount] = useState(0);
  const [currentPage, setCurrentPage] = useState(1);
  const [pageSize, setPageSize] = useState(10);
  const [totalPages, setTotalPages] = useState(0);

  /**
   * Load categories with optional filtering
   */
  const loadCategories = useCallback(async (filters?: CategoryFilterRequest) => {
    try {
      setLoading(true);
      setError(null);
      
      console.log('Loading categories with filters:', filters);
      
      const response: CategoryListResponse = await categoriesApi.getCategories(filters);
      
      setCategories(response.categories);
      setTotalCount(response.totalCount);
      setCurrentPage(response.page);
      setPageSize(response.pageSize);
      setTotalPages(response.totalPages);
      
      console.log('Categories loaded successfully:', response);
    } catch (err) {
      console.error('Failed to load categories:', err);
      setError(err instanceof Error ? err.message : 'Failed to load categories');
    } finally {
      setLoading(false);
    }
  }, []);

  /**
   * Load a specific category by ID
   */
  const loadCategory = useCallback(async (id: number) => {
    try {
      setIsLoadingCategory(true);
      setError(null);
      
      console.log('Loading category:', id);
      
      const category: CategoryDetailResponse = await categoriesApi.getCategoryById(id);
      setSelectedCategory(category);
      
      console.log('Category loaded successfully:', category);
      return category;
    } catch (err) {
      console.error('Failed to load category:', err);
      setError(err instanceof Error ? err.message : 'Failed to load category');
      throw err;
    } finally {
      setIsLoadingCategory(false);
    }
  }, []);

  /**
   * Create a new category
   */
  const createCategory = useCallback(async (data: CreateCategoryRequest) => {
    try {
      setIsCreating(true);
      setError(null);
      
      console.log('Creating category:', data);
      
      const newCategory: Category = await categoriesApi.createCategory(data);
      
      // Add to local state (optimistic update)
      setCategories(prev => [
        {
          id: newCategory.id,
          name: newCategory.name,
          description: newCategory.description,
          slug: newCategory.slug,
          icon: newCategory.icon,
          color: newCategory.color,
          isActive: newCategory.isActive,
          sortOrder: newCategory.sortOrder,
          courseCount: newCategory.courseCount,
          totalStudents: newCategory.totalStudents,
          parentName: newCategory.parentName,
        },
        ...prev
      ]);
      
      // Update total count
      setTotalCount(prev => prev + 1);
      
      console.log('Category created successfully:', newCategory);
      return newCategory;
    } catch (err) {
      console.error('Failed to create category:', err);
      setError(err instanceof Error ? err.message : 'Failed to create category');
      throw err;
    } finally {
      setIsCreating(false);
    }
  }, []);

  /**
   * Update an existing category
   */
  const updateCategory = useCallback(async (id: number, data: UpdateCategoryRequest) => {
    try {
      setIsUpdating(true);
      setError(null);
      
      console.log('Updating category:', id, data);
      
      const updatedCategory: Category = await categoriesApi.updateCategory(id, data);
      
      // Update local state
      setCategories(prev => 
        prev.map(category => 
          category.id === id 
            ? {
                ...category,
                name: updatedCategory.name,
                description: updatedCategory.description,
                slug: updatedCategory.slug,
                icon: updatedCategory.icon,
                color: updatedCategory.color,
                isActive: updatedCategory.isActive,
                sortOrder: updatedCategory.sortOrder,
                parentName: updatedCategory.parentName,
              }
            : category
        )
      );
      
      // Update selected category if it's the one being updated
      if (selectedCategory?.id === id) {
        setSelectedCategory(updatedCategory);
      }
      
      console.log('Category updated successfully:', updatedCategory);
      return updatedCategory;
    } catch (err) {
      console.error('Failed to update category:', err);
      setError(err instanceof Error ? err.message : 'Failed to update category');
      throw err;
    } finally {
      setIsUpdating(false);
    }
  }, [selectedCategory]);

  /**
   * Delete a category
   */
  const deleteCategory = useCallback(async (id: number) => {
    try {
      setIsDeleting(true);
      setError(null);
      
      console.log('Deleting category:', id);
      
      await categoriesApi.deleteCategory(id);
      
      // Remove from local state
      setCategories(prev => prev.filter(category => category.id !== id));
      
      // Update total count
      setTotalCount(prev => prev - 1);
      
      // Clear selected category if it was deleted
      if (selectedCategory?.id === id) {
        setSelectedCategory(null);
      }
      
      console.log('Category deleted successfully');
    } catch (err) {
      console.error('Failed to delete category:', err);
      setError(err instanceof Error ? err.message : 'Failed to delete category');
      throw err;
    } finally {
      setIsDeleting(false);
    }
  }, [selectedCategory]);

  /**
   * Get parent categories
   */
  const loadParentCategories = useCallback(async () => {
    try {
      setError(null);
      const parentCategories = await categoriesApi.getParentCategories();
      return parentCategories;
    } catch (err) {
      console.error('Failed to load parent categories:', err);
      setError(err instanceof Error ? err.message : 'Failed to load parent categories');
      throw err;
    }
  }, []);

  /**
   * Get subcategories for a parent
   */
  const loadSubcategories = useCallback(async (parentId: number) => {
    try {
      setError(null);
      const subcategories = await categoriesApi.getSubcategories(parentId);
      return subcategories;
    } catch (err) {
      console.error('Failed to load subcategories:', err);
      setError(err instanceof Error ? err.message : 'Failed to load subcategories');
      throw err;
    }
  }, []);

  /**
   * Refresh categories (reload current data)
   */
  const refreshCategories = useCallback(() => {
    return loadCategories();
  }, [loadCategories]);

  /**
   * Clear error state
   */
  const clearError = useCallback(() => {
    setError(null);
  }, []);

  /**
   * Reset state
   */
  const resetState = useCallback(() => {
    setCategories([]);
    setSelectedCategory(null);
    setLoading(true);
    setError(null);
    setTotalCount(0);
    setCurrentPage(1);
    setTotalPages(0);
  }, []);

  return {
    // State
    categories,
    selectedCategory,
    loading,
    isCreating,
    isUpdating,
    isDeleting,
    isLoadingCategory,
    error,
    
    // Pagination
    totalCount,
    currentPage,
    pageSize,
    totalPages,
    
    // Actions
    loadCategories,
    loadCategory,
    createCategory,
    updateCategory,
    deleteCategory,
    loadParentCategories,
    loadSubcategories,
    refreshCategories,
    setSelectedCategory,
    clearError,
    resetState,
  };
}