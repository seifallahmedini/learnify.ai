import { useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { Button } from '@/shared/components/ui/button';
import { Badge } from '@/shared/components/ui/badge';
import { ArrowLeft, Edit, Trash2, BookOpen } from 'lucide-react';
import { useCategoryManagement } from '../../hooks';
import { CategoryIcon } from '../shared/CategoryIcon';
import { CategoryStatusBadge } from '../shared/CategoryStatusBadge';

export function CategoryDetailsPage() {
  const { categoryId } = useParams<{ categoryId: string }>();
  const navigate = useNavigate();
  const { selectedCategory, isLoadingCategory, error, loadCategory } = useCategoryManagement();

  useEffect(() => {
    if (categoryId) {
      const id = parseInt(categoryId, 10);
      if (!isNaN(id)) {
        loadCategory(id);
      }
    }
  }, [categoryId, loadCategory]);

  const handleBack = () => {
    navigate('/categories');
  };

  if (isLoadingCategory) {
    return (
      <div className="p-6 space-y-6">
        <div className="flex items-center gap-4">
          <div className="h-10 w-24 bg-muted animate-pulse rounded-lg" />
          <div className="h-8 w-48 bg-muted animate-pulse rounded-lg" />
        </div>
        <div className="space-y-4">
          <div className="h-32 bg-muted animate-pulse rounded-lg" />
          <div className="h-64 bg-muted animate-pulse rounded-lg" />
        </div>
      </div>
    );
  }

  if (error || !selectedCategory) {
    return (
      <div className="p-6">
        <div className="text-center py-12">
          <h2 className="text-2xl font-bold text-destructive mb-2">Error Loading Category</h2>
          <p className="text-muted-foreground mb-4">
            {error || 'Category not found'}
          </p>
          <Button onClick={handleBack}>
            <ArrowLeft className="h-4 w-4 mr-2" />
            Back to Categories
          </Button>
        </div>
      </div>
    );
  }

  return (
    <div className="p-6 space-y-6">
      {/* Header */}
      <div className="flex items-center justify-between">
        <div className="flex items-center gap-4">
          <Button variant="outline" onClick={handleBack}>
            <ArrowLeft className="h-4 w-4 mr-2" />
            Back
          </Button>
          <div className="flex items-center gap-3">
            <CategoryIcon 
              iconUrl={selectedCategory.iconUrl}
              className="h-12 w-12"
            />
            <div>
              <h1 className="text-3xl font-bold">{selectedCategory.name}</h1>
              <p className="text-muted-foreground">ID: {selectedCategory.id}</p>
            </div>
          </div>
        </div>
        <div className="flex items-center gap-2">
          <Button variant="outline" size="sm">
            <Edit className="h-4 w-4 mr-2" />
            Edit
          </Button>
          <Button variant="destructive" size="sm">
            <Trash2 className="h-4 w-4 mr-2" />
            Delete
          </Button>
        </div>
      </div>

      {/* Category Info */}
      <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
        {/* Main Info */}
        <div className="lg:col-span-2 space-y-6">
          <div className="bg-card border rounded-lg p-6">
            <h2 className="text-xl font-semibold mb-4">Category Information</h2>
            <div className="space-y-4">
              <div>
                <label className="text-sm font-medium text-muted-foreground">Description</label>
                <p className="text-sm mt-1">{selectedCategory.description}</p>
              </div>
              <div className="grid grid-cols-1 gap-4">
                <div>
                  <label className="text-sm font-medium text-muted-foreground">Status</label>
                  <div className="mt-1">
                    <CategoryStatusBadge isActive={selectedCategory.isActive} />
                  </div>
                </div>
              </div>
              {selectedCategory.parentCategoryName && (
                <div>
                  <label className="text-sm font-medium text-muted-foreground">Parent Category</label>
                  <div className="mt-1">
                    <Badge variant="secondary">{selectedCategory.parentCategoryName}</Badge>
                  </div>
                </div>
              )}
            </div>
          </div>

          {/* Placeholder for courses */}
          <div className="bg-card border rounded-lg p-6">
            <h2 className="text-xl font-semibold mb-4">Courses in this Category</h2>
            <p className="text-muted-foreground">Course list will be implemented here.</p>
          </div>
        </div>

        {/* Stats Sidebar */}
        <div className="space-y-6">
          <div className="bg-card border rounded-lg p-6">
            <h3 className="text-lg font-semibold mb-4">Statistics</h3>
            <div className="space-y-4">
              <div className="flex items-center justify-between p-3 bg-muted/50 rounded-lg">
                <div className="flex items-center gap-2">
                  <BookOpen className="h-4 w-4 text-muted-foreground" />
                  <span className="text-sm font-medium">Courses</span>
                </div>
                <span className="font-bold">{selectedCategory.courseCount}</span>
              </div>
            </div>
          </div>

          <div className="bg-card border rounded-lg p-6">
            <h3 className="text-lg font-semibold mb-4">Category Details</h3>
            <div className="space-y-3 text-sm">
              <div className="flex justify-between">
                <span className="text-muted-foreground">Created</span>
                <span>{new Date(selectedCategory.createdAt).toLocaleDateString()}</span>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}