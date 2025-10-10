import { useState } from 'react';
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from '@/shared/components/ui/table';
import { Button } from '@/shared/components/ui/button';
import { Badge } from '@/shared/components/ui/badge';
import { 
  MoreHorizontal, 
  Edit, 
  Trash2, 
  Eye,
  ChevronLeft,
  ChevronRight,
  Users,
  BookOpen,
} from 'lucide-react';
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from '@/shared/components/ui/dropdown-menu';
import { useCategoryManagement } from '../../hooks';
import type { CategorySummary } from '../../types';
import { CategoryIcon, CategoryStatusBadge } from '../shared';
import { DeleteCategoryDialog, EditCategoryDialog } from '../dialogs';

interface CategoryTableProps {
  categories: CategorySummary[];
  loading: boolean;
  currentPage: number;
  totalPages: number;
  onPageChange: (page: number) => void;
  onRefresh: () => void;
}

export function CategoryTable({
  categories,
  loading,
  currentPage,
  totalPages,
  onPageChange,
  onRefresh,
}: CategoryTableProps) {
  const { deleteCategory } = useCategoryManagement();
  const [selectedCategory, setSelectedCategory] = useState<CategorySummary | null>(null);
  const [showEditDialog, setShowEditDialog] = useState(false);
  const [showDeleteDialog, setShowDeleteDialog] = useState(false);

  const handleEdit = (category: CategorySummary) => {
    setSelectedCategory(category);
    setShowEditDialog(true);
  };

  const handleDelete = (category: CategorySummary) => {
    setSelectedCategory(category);
    setShowDeleteDialog(true);
  };

  const handleView = (category: CategorySummary) => {
    // Navigate to category details page
    window.location.href = `/categories/${category.id}`;
  };

  const handleCategoryUpdated = () => {
    setShowEditDialog(false);
    setSelectedCategory(null);
    onRefresh();
  };

  const handleCategoryDeleted = () => {
    setShowDeleteDialog(false);
    setSelectedCategory(null);
    onRefresh();
  };

  if (loading) {
    return (
      <div className="space-y-4">
        {[...Array(5)].map((_, i) => (
          <div key={i} className="h-16 bg-muted animate-pulse rounded-lg" />
        ))}
      </div>
    );
  }

  if (categories.length === 0) {
    return (
      <div className="text-center py-12 bg-muted/50 rounded-lg border-2 border-dashed border-muted">
        <BookOpen className="h-12 w-12 text-muted-foreground mx-auto mb-4" />
        <h3 className="text-lg font-semibold text-muted-foreground mb-2">No categories found</h3>
        <p className="text-sm text-muted-foreground">
          Create your first category to organize your courses
        </p>
      </div>
    );
  }

  return (
    <div className="space-y-4">
      {/* Table */}
      <div className="border rounded-lg overflow-hidden">
        <Table>
          <TableHeader>
            <TableRow>
              <TableHead className="w-[50px]">Icon</TableHead>
              <TableHead>Name</TableHead>
              <TableHead>Description</TableHead>
              <TableHead>Parent</TableHead>
              <TableHead>Status</TableHead>
              <TableHead className="text-center">Courses</TableHead>
              <TableHead className="text-center">Students</TableHead>
              <TableHead>Sort Order</TableHead>
              <TableHead className="w-[100px]">Actions</TableHead>
            </TableRow>
          </TableHeader>
          <TableBody>
            {categories.map((category) => (
              <TableRow key={category.id}>
                <TableCell>
                  <CategoryIcon 
                    icon={category.icon} 
                    color={category.color}
                    className="h-8 w-8"
                  />
                </TableCell>
                <TableCell>
                  <div className="space-y-1">
                    <p className="font-medium">{category.name}</p>
                    <p className="text-xs text-muted-foreground">#{category.slug}</p>
                  </div>
                </TableCell>
                <TableCell>
                  <p className="text-sm text-muted-foreground max-w-xs truncate">
                    {category.description}
                  </p>
                </TableCell>
                <TableCell>
                  {category.parentName ? (
                    <Badge variant="secondary" className="text-xs">
                      {category.parentName}
                    </Badge>
                  ) : (
                    <span className="text-xs text-muted-foreground">Root</span>
                  )}
                </TableCell>
                <TableCell>
                  <CategoryStatusBadge isActive={category.isActive} />
                </TableCell>
                <TableCell className="text-center">
                  <div className="flex items-center justify-center space-x-1">
                    <BookOpen className="h-4 w-4 text-muted-foreground" />
                    <span className="font-medium">{category.courseCount}</span>
                  </div>
                </TableCell>
                <TableCell className="text-center">
                  <div className="flex items-center justify-center space-x-1">
                    <Users className="h-4 w-4 text-muted-foreground" />
                    <span className="font-medium">{category.totalStudents}</span>
                  </div>
                </TableCell>
                <TableCell>
                  <Badge variant="outline" className="text-xs">
                    {category.sortOrder}
                  </Badge>
                </TableCell>
                <TableCell>
                  <DropdownMenu>
                    <DropdownMenuTrigger asChild>
                      <Button variant="ghost" size="sm" className="h-8 w-8 p-0">
                        <MoreHorizontal className="h-4 w-4" />
                      </Button>
                    </DropdownMenuTrigger>
                    <DropdownMenuContent align="end" className="w-48">
                      <DropdownMenuItem onClick={() => handleView(category)}>
                        <Eye className="h-4 w-4 mr-2" />
                        View Details
                      </DropdownMenuItem>
                      <DropdownMenuItem onClick={() => handleEdit(category)}>
                        <Edit className="h-4 w-4 mr-2" />
                        Edit Category
                      </DropdownMenuItem>
                      <DropdownMenuItem 
                        onClick={() => handleDelete(category)}
                        className="text-destructive focus:text-destructive"
                      >
                        <Trash2 className="h-4 w-4 mr-2" />
                        Delete Category
                      </DropdownMenuItem>
                    </DropdownMenuContent>
                  </DropdownMenu>
                </TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </div>

      {/* Pagination */}
      {totalPages > 1 && (
        <div className="flex items-center justify-between">
          <p className="text-sm text-muted-foreground">
            Page {currentPage} of {totalPages}
          </p>
          <div className="flex items-center space-x-2">
            <Button
              variant="outline"
              size="sm"
              onClick={() => onPageChange(currentPage - 1)}
              disabled={currentPage <= 1}
            >
              <ChevronLeft className="h-4 w-4 mr-1" />
              Previous
            </Button>
            <Button
              variant="outline"
              size="sm"
              onClick={() => onPageChange(currentPage + 1)}
              disabled={currentPage >= totalPages}
            >
              Next
              <ChevronRight className="h-4 w-4 ml-1" />
            </Button>
          </div>
        </div>
      )}

      {/* Edit Dialog */}
      <EditCategoryDialog
        category={selectedCategory}
        open={showEditDialog}
        onOpenChange={setShowEditDialog}
        onCategoryUpdated={handleCategoryUpdated}
      />

      {/* Delete Dialog */}
      <DeleteCategoryDialog
        category={selectedCategory}
        open={showDeleteDialog}
        onOpenChange={setShowDeleteDialog}
        onCategoryDeleted={handleCategoryDeleted}
      />
    </div>
  );
}