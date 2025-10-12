import { useEffect, useState, useCallback, memo } from 'react';
import { Button } from '@/shared/components/ui/button';
import { Input } from '@/shared/components/ui/input';
import { Plus, Search } from 'lucide-react';
import { useCategoryManagement } from '../../hooks';
import type { CategoryFilterRequest } from '../../types';
import { CategoryListSkeleton } from '../shared';
import { CategoryTable } from './CategoryTable';
import { CreateCategoryDialog } from '../dialogs';

// Memoized search component to prevent unnecessary re-renders
const SearchSection = memo(({ 
  searchTerm, 
  isSearching, 
  onSearchChange 
}: {
  searchTerm: string;
  isSearching: boolean;
  onSearchChange: (value: string) => void;
}) => (
  <div className="flex flex-col sm:flex-row gap-4">
    <div className="relative flex-1">
      <Search className="absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-muted-foreground" />
      <Input
        placeholder="Search categories by name or description..."
        value={searchTerm}
        onChange={(e) => onSearchChange(e.target.value)}
        className="pl-10 pr-10"
      />
      {/* Search loading indicator */}
      {isSearching && (
        <div className="absolute right-3 top-1/2 -translate-y-1/2">
          <div className="h-4 w-4 animate-spin rounded-full border-2 border-primary border-t-transparent" />
        </div>
      )}
    </div>
  </div>
));

export function CategoriesListPage() {
  const {
    categories,
    loading,
    error,
    totalCount,
    currentPage,
    totalPages,
    searchTerm,
    isSearching,
    loadCategories,
    refreshCategories,
    handleSearchChange,
    clearSearch,
  } = useCategoryManagement();

  const [filters, setFilters] = useState<CategoryFilterRequest>({
    page: 1,
    pageSize: 10,
  });
  const [showCreateDialog, setShowCreateDialog] = useState(false);

  // Load categories on mount and when filters change
  useEffect(() => {
    loadCategories(filters);
  }, [filters.page, filters.pageSize, filters.searchTerm, filters.isActive, filters.rootOnly, filters.parentCategoryId]);

  const handlePageChange = useCallback((page: number) => {
    setFilters(prev => ({ ...prev, page }));
  }, []);

  const handleCategoryCreated = useCallback(() => {
    setShowCreateDialog(false);
    refreshCategories();
  }, [refreshCategories]);

  const handleCreateCategory = useCallback(() => {
    setShowCreateDialog(true);
  }, []);

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

      {/* Search and Filters */}
      <SearchSection 
        searchTerm={searchTerm}
        isSearching={isSearching}
        onSearchChange={handleSearchChange}
      />

      {/* Results Summary */}
      <div className="flex items-center justify-between text-sm text-muted-foreground">
        <span>
          {totalCount} {totalCount === 1 ? 'category' : 'categories'} found
          {searchTerm && (
            <span className="ml-1">
              for "<span className="text-foreground font-medium">{searchTerm}</span>"
            </span>
          )}
        </span>
        {searchTerm && (
          <Button variant="ghost" size="sm" onClick={clearSearch} className="h-auto p-1 text-xs">
            Clear search
          </Button>
        )}
      </div>

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
        <>
          {categories.length === 0 && !loading ? (
            <div className="text-center py-12 space-y-4">
              <div className="text-lg font-medium text-muted-foreground">
                {searchTerm ? 'No categories found' : 'No categories yet'}
              </div>
              <div className="text-sm text-muted-foreground">
                {searchTerm 
                  ? `No categories match "${searchTerm}". Try adjusting your search or browse all categories.`
                  : 'Get started by creating your first category to organize your courses.'
                }
              </div>
              <div className="flex justify-center gap-2">
                {searchTerm && (
                  <Button variant="outline" onClick={clearSearch}>
                    Clear Search
                  </Button>
                )}
                <Button onClick={handleCreateCategory} className="gap-2">
                  <Plus className="h-4 w-4" />
                  {searchTerm ? 'Create Category' : 'Create Your First Category'}
                </Button>
              </div>
            </div>
          ) : (
            <CategoryTable
              categories={categories}
              loading={loading}
              currentPage={currentPage}
              totalPages={totalPages}
              onPageChange={handlePageChange}
              onRefresh={refreshCategories}
            />
          )}
        </>
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