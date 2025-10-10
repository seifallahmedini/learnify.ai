import { useEffect, useState } from 'react';
import { Button } from '@/shared/components/ui/button';
import { Plus } from 'lucide-react';
import { useCategoryManagement } from '../../hooks';
import type { CategoryFilterRequest } from '../../types';
import { CategoryListSkeleton } from '../shared';
import { CategoryFilters } from './CategoryFilters';
import { CategoryTable } from './CategoryTable';
import { CreateCategoryDialog } from '../dialogs';

export function CategoriesListPage() {
  const {
    categories,
    loading,
    error,
    totalCount,
    currentPage,
    totalPages,
    loadCategories,
    refreshCategories,
  } = useCategoryManagement();

  const [filters, setFilters] = useState<CategoryFilterRequest>({
    page: 1,
    pageSize: 10,
  });
  const [showCreateDialog, setShowCreateDialog] = useState(false);

  // Load categories on mount and when filters change
  useEffect(() => {
    loadCategories(filters);
  }, [loadCategories, filters]);

  const handleFiltersChange = (newFilters: Partial<CategoryFilterRequest>) => {
    setFilters(prev => ({
      ...prev,
      ...newFilters,
      page: 1, // Reset to first page when filters change
    }));
  };

  const handlePageChange = (page: number) => {
    setFilters(prev => ({ ...prev, page }));
  };

  const handleCategoryCreated = () => {
    setShowCreateDialog(false);
    refreshCategories();
  };

  const handleCreateCategory = () => {
    setShowCreateDialog(true);
  };

  if (loading) {
    return (
      <div className="p-6 space-y-6">
        <div className="flex justify-between items-center">
          <div className="space-y-2">
            <div className="h-8 w-48 bg-muted animate-pulse rounded-lg" />
            <div className="h-4 w-64 bg-muted animate-pulse rounded-lg" />
          </div>
          <div className="h-10 w-32 bg-muted animate-pulse rounded-lg" />
        </div>
        <CategoryListSkeleton />
      </div>
    );
  }

  return (
    <div className="p-6 space-y-6">
      {/* Header */}
      <div className="flex justify-between items-center">
        <div className="space-y-1">
          <h1 className="text-3xl font-bold tracking-tight">Categories</h1>
          <p className="text-muted-foreground">
            Manage course categories and organize your learning content
          </p>
        </div>
        <Button onClick={handleCreateCategory} className="gap-2">
          <Plus className="h-4 w-4" />
          Create Category
        </Button>
      </div>

      {/* Filters */}
      <CategoryFilters 
        onFiltersChange={handleFiltersChange}
        totalCount={totalCount}
      />

      {/* Error State */}
      {error && (
        <div className="p-4 bg-destructive/15 border border-destructive/20 rounded-lg">
          <p className="text-destructive font-medium">Error loading categories</p>
          <p className="text-destructive/80 text-sm mt-1">{error}</p>
          <Button 
            variant="outline" 
            size="sm" 
            className="mt-2" 
            onClick={() => refreshCategories()}
          >
            Try Again
          </Button>
        </div>
      )}

      {/* Categories Table */}
      {!error && (
        <CategoryTable
          categories={categories}
          loading={loading}
          currentPage={currentPage}
          totalPages={totalPages}
          onPageChange={handlePageChange}
          onRefresh={refreshCategories}
        />
      )}

      {/* Create Category Dialog */}
      <CreateCategoryDialog
        open={showCreateDialog}
        onOpenChange={setShowCreateDialog}
        onCategoryCreated={handleCategoryCreated}
      />
    </div>
  );
}