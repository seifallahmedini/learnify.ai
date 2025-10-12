import { useState } from 'react';
import { Button } from '@/shared/components/ui/button';
import {
  Sheet,
  SheetContent,
  SheetDescription,
  SheetFooter,
  SheetHeader,
  SheetTitle,
} from '@/shared/components/ui/sheet';
import { Badge } from '@/shared/components/ui/badge';
import { Separator } from '@/shared/components/ui/separator';
import { 
  Trash2, 
  X, 
  AlertTriangle,
  AlertCircle,
  Folder,
  BookOpen,
  Shield,
  Info
} from 'lucide-react';
import { useCategoryManagement } from '../../hooks';
import type { CategorySummary } from '../../types';

interface DeleteCategoryDialogProps {
  category: CategorySummary | null;
  open: boolean;
  onOpenChange: (open: boolean) => void;
  onCategoryDeleted?: () => void;
}

export function DeleteCategoryDialog({
  category,
  open,
  onOpenChange,
  onCategoryDeleted,
}: DeleteCategoryDialogProps) {
  const { deleteCategory, isDeleting } = useCategoryManagement();
  const [error, setError] = useState<{
    message: string;
    success?: boolean;
    data?: any;
    errors?: any;
  } | null>(null);
  const [confirmationText, setConfirmationText] = useState('');

  const handleDelete = async () => {
    if (!category) return;
    
    try {
      setError(null);
      await deleteCategory(category.id);
      
      onCategoryDeleted?.();
      onOpenChange(false);
      setConfirmationText(''); // Reset confirmation
      
    } catch (err: any) {
      // Handle API error response format
      if (err.response?.data) {
        const apiError = err.response.data;
        setError({
          message: apiError.message || 'Failed to delete category',
          success: apiError.success,
          data: apiError.data,
          errors: apiError.errors
        });
      } else if (err instanceof Error) {
        setError({ message: err.message });
      } else {
        setError({ message: 'Failed to delete category' });
      }
    }
  };

  const handleClose = () => {
    if (!isDeleting) {
      setError(null);
      setConfirmationText('');
      onOpenChange(false);
    }
  };

  if (!category) return null;

  const isConfirmed = confirmationText.toLowerCase() === category.name.toLowerCase();

  return (
    <Sheet open={open} onOpenChange={onOpenChange}>
      <SheetContent side="right" className="w-[60vw] sm:max-w-2xl overflow-hidden flex flex-col p-0">
        {/* Header with danger gradient */}
        <SheetHeader className="relative px-8 py-6 border-b bg-gradient-to-br from-red-50 via-white to-orange-50">
          <div className="absolute inset-0 bg-gradient-to-br from-red-500/5 via-transparent to-orange-500/5" />
          
          <div className="relative space-y-4">
            <div className="flex items-center justify-between">
              <div className="flex items-center space-x-4">
                <div className="h-12 w-12 rounded-xl bg-gradient-to-br from-red-100 to-red-50 flex items-center justify-center border border-red-200/50 shadow-sm">
                  <Trash2 className="h-6 w-6 text-red-600" />
                </div>
                <div>
                  <SheetTitle className="text-xl font-bold text-gray-900 flex items-center space-x-2">
                    <span>Delete Category</span>
                  </SheetTitle>
                  <SheetDescription className="text-sm text-gray-600 mt-1">
                    Permanently remove "{category.name}" category
                  </SheetDescription>
                </div>
              </div>
              <Badge variant="outline" className="bg-gradient-to-r from-red-50 to-red-100 text-red-700 border-red-200 shadow-sm">
                Danger Zone
              </Badge>
            </div>
            
            {/* Quick stats preview */}
            <div className="flex items-center space-x-4 p-4 bg-white/60 backdrop-blur-sm rounded-xl border border-red-100 shadow-sm">
              <div className="flex items-center space-x-2">
                <div className="h-2 w-2 rounded-full bg-red-500 animate-pulse" />
                <span className="text-xs font-medium text-gray-600 uppercase tracking-wider">Deletion Warning</span>
              </div>
              <div className="flex items-center space-x-4">
                <div className="flex items-center space-x-1">
                  <Folder className="h-3 w-3 text-red-600" />
                  <span className="text-xs text-gray-600">ID: {category.id}</span>
                </div>
                <div className="flex items-center space-x-1">
                  <BookOpen className="h-3 w-3 text-orange-600" />
                  <span className="text-xs text-gray-600">{category.courseCount} Courses</span>
                </div>
                <div className="flex items-center space-x-1">
                  <AlertTriangle className="h-3 w-3 text-red-600" />
                  <span className="text-xs text-red-600">Irreversible</span>
                </div>
              </div>
            </div>
          </div>
        </SheetHeader>

        {/* Scrollable Content */}
        <div className="flex-1 overflow-y-auto">
          <div className="p-8 space-y-8">
            
            {/* Danger Warning Section */}
            <div className="space-y-6">
              <div className="flex items-center space-x-3 pb-4 border-b border-red-100">
                <div className="h-8 w-8 rounded-lg bg-gradient-to-br from-red-100 to-red-50 flex items-center justify-center">
                  <AlertTriangle className="h-4 w-4 text-red-600" />
                </div>
                <div>
                  <h3 className="font-semibold text-gray-900">Deletion Warning</h3>
                  <p className="text-xs text-gray-500">This action cannot be undone</p>
                </div>
              </div>
              
              {/* Main Warning */}
              <div className="p-6 bg-gradient-to-br from-red-50 to-orange-50 rounded-xl border-2 border-red-200">
                <div className="flex items-start space-x-4">
                  <div className="h-10 w-10 rounded-lg bg-red-100 flex items-center justify-center flex-shrink-0">
                    <AlertTriangle className="h-5 w-5 text-red-600" />
                  </div>
                  <div className="space-y-3">
                    <h4 className="font-semibold text-red-800">Are you absolutely sure?</h4>
                    <p className="text-sm text-red-700">
                      This will permanently delete the <strong>"{category.name}"</strong> category. 
                      This action cannot be undone and will:
                    </p>
                    <ul className="text-sm text-red-700 space-y-1 ml-4">
                      <li className="flex items-center space-x-2">
                        <span className="h-1 w-1 bg-red-500 rounded-full"></span>
                        <span>Remove the category from all systems</span>
                      </li>
                      <li className="flex items-center space-x-2">
                        <span className="h-1 w-1 bg-red-500 rounded-full"></span>
                        <span>Affect {category.courseCount} associated courses</span>
                      </li>
                      <li className="flex items-center space-x-2">
                        <span className="h-1 w-1 bg-red-500 rounded-full"></span>
                        <span>Break existing category relationships</span>
                      </li>
                    </ul>
                  </div>
                </div>
              </div>

              {/* Confirmation Input */}
              <div className="space-y-3">
                <div className="flex items-center space-x-2">
                  <Shield className="h-4 w-4 text-gray-500" />
                  <span className="text-sm font-medium text-gray-700">Confirmation Required</span>
                </div>
                <p className="text-sm text-gray-600">
                  Type <strong>{category.name}</strong> to confirm deletion:
                </p>
                <input
                  type="text"
                  value={confirmationText}
                  onChange={(e) => setConfirmationText(e.target.value)}
                  placeholder={`Type "${category.name}" here`}
                  className="w-full h-11 px-3 border-2 border-gray-200 rounded-lg focus:border-red-300 focus:ring-red-100 focus:outline-none"
                />
                {confirmationText && !isConfirmed && (
                  <p className="text-xs text-red-600 flex items-center space-x-1">
                    <AlertCircle className="h-3 w-3" />
                    <span>Category name doesn't match</span>
                  </p>
                )}
                {isConfirmed && (
                  <p className="text-xs text-green-600 flex items-center space-x-1">
                    <Info className="h-3 w-3" />
                    <span>Confirmation successful - deletion enabled</span>
                  </p>
                )}
              </div>
            </div>

            <Separator className="my-6 border-red-100" />

            {/* Category Details Section */}
            <div className="space-y-6">
              <div className="flex items-center space-x-3 pb-4 border-b border-gray-100">
                <div className="h-8 w-8 rounded-lg bg-gradient-to-br from-gray-100 to-gray-50 flex items-center justify-center">
                  <Info className="h-4 w-4 text-gray-600" />
                </div>
                <div>
                  <h3 className="font-semibold text-gray-900">Category Details</h3>
                  <p className="text-xs text-gray-500">Information about the category being deleted</p>
                </div>
              </div>
              
              {/* Category Info Card */}
              <div className="p-4 bg-gray-50 rounded-xl border border-gray-200">
                <div className="grid grid-cols-2 gap-4 text-sm">
                  <div className="space-y-1">
                    <span className="text-gray-500">Name:</span>
                    <p className="font-medium text-gray-800">{category.name}</p>
                  </div>
                  <div className="space-y-1">
                    <span className="text-gray-500">Status:</span>
                    <div className="flex items-center space-x-1">
                      <div className={`h-1.5 w-1.5 rounded-full ${category.isActive ? 'bg-green-500' : 'bg-gray-400'}`} />
                      <span className="font-medium">{category.isActive ? 'Active' : 'Inactive'}</span>
                    </div>
                  </div>
                  <div className="space-y-1">
                    <span className="text-gray-500">Courses:</span>
                    <span className="font-medium text-gray-800">{category.courseCount}</span>
                  </div>
                  <div className="space-y-1">
                    <span className="text-gray-500">Created:</span>
                    <span className="font-medium text-gray-800">{new Date(category.createdAt).toLocaleDateString()}</span>
                  </div>
                </div>
              </div>
            </div>

            {/* Error Display */}
            {error && (
              <div className="space-y-4">
                <div className="flex items-center space-x-3 pb-4 border-b border-red-100">
                  <div className="h-8 w-8 rounded-lg bg-gradient-to-br from-red-100 to-red-50 flex items-center justify-center">
                    <AlertCircle className="h-4 w-4 text-red-600" />
                  </div>
                  <div>
                    <h3 className="font-semibold text-gray-900">Deletion Failed</h3>
                    <p className="text-xs text-gray-500">The operation could not be completed</p>
                  </div>
                </div>
                
                <div className="p-4 bg-red-50 border-2 border-red-200 rounded-xl">
                  <div className="flex items-start space-x-3">
                    <AlertCircle className="h-5 w-5 text-red-500 mt-0.5 flex-shrink-0" />
                    <div className="space-y-2">
                      <h4 className="text-sm font-medium text-red-800">Cannot Delete Category</h4>
                      <p className="text-sm text-red-700">{error.message}</p>
                      
                      {/* Show additional error details if available */}
                      {error.success === false && (
                        <div className="mt-3 p-3 bg-red-100 rounded-lg border border-red-200">
                          <p className="text-xs text-red-600 font-medium">API Response Details:</p>
                          <div className="mt-1 text-xs text-red-600 space-y-1">
                            <div>Success: {error.success ? 'true' : 'false'}</div>
                            {error.data !== undefined && <div>Data: {JSON.stringify(error.data)}</div>}
                            {error.errors && <div>Errors: {JSON.stringify(error.errors)}</div>}
                          </div>
                        </div>
                      )}
                    </div>
                  </div>
                </div>
              </div>
            )}
          </div>
        </div>

        {/* Fixed Footer */}
        <SheetFooter className="border-t bg-gray-50/50 px-8 py-4">
          <div className="flex items-center justify-between w-full">
            <div className="flex items-center space-x-2 text-xs text-gray-500">
              <div className={`h-1.5 w-1.5 rounded-full ${isConfirmed ? 'bg-red-500 animate-pulse' : 'bg-gray-400'}`} />
              <span>{isConfirmed ? 'Ready to delete' : 'Confirmation required'}</span>
            </div>
            <div className="flex items-center gap-3">
              <Button
                type="button"
                variant="outline"
                onClick={handleClose}
                disabled={isDeleting}
                className="h-10 px-6 text-sm"
              >
                <X className="h-4 w-4 mr-2" />
                Cancel
              </Button>
              <Button
                type="button"
                variant="destructive"
                disabled={isDeleting || !isConfirmed}
                className="h-10 px-6 bg-gradient-to-r from-red-600 to-red-500 hover:from-red-500 hover:to-red-600 text-sm"
                onClick={handleDelete}
              >
                {isDeleting ? (
                  <>
                    <div className="h-4 w-4 mr-2 animate-spin rounded-full border-2 border-white border-t-transparent" />
                    Deleting...
                  </>
                ) : (
                  <>
                    <Trash2 className="h-4 w-4 mr-2" />
                    Delete Category
                  </>
                )}
              </Button>
            </div>
          </div>
        </SheetFooter>
      </SheetContent>
    </Sheet>
  );
}