import { useState, useEffect } from 'react';
import { Button } from '@/shared/components/ui/button';
import {
  Sheet,
  SheetContent,
  SheetDescription,
  SheetFooter,
  SheetHeader,
  SheetTitle,
} from '@/shared/components/ui/sheet';
import { Input } from '@/shared/components/ui/input';
import { Textarea } from '@/shared/components/ui/textarea';
import { Label } from '@/shared/components/ui/label';
import { Switch } from '@/shared/components/ui/switch';
import { Badge } from '@/shared/components/ui/badge';
import { Separator } from '@/shared/components/ui/separator';
import { 
  Edit, 
  X, 
  Save,
  AlertCircle,
  Sparkles,
  Folder,
  Settings,
  Tag,
  FileText
} from 'lucide-react';
import { useCategoryManagement } from '../../hooks';
import type { CategorySummary, UpdateCategoryRequest } from '../../types';

interface EditCategoryDialogProps {
  category: CategorySummary | null;
  open: boolean;
  onOpenChange: (open: boolean) => void;
  onCategoryUpdated?: () => void;
}

export function EditCategoryDialog({
  category,
  open,
  onOpenChange,
  onCategoryUpdated,
}: EditCategoryDialogProps) {
  const { updateCategory, isUpdating } = useCategoryManagement();
  const [error, setError] = useState<string | null>(null);
  const [formData, setFormData] = useState({
    name: category?.name || '',
    description: category?.description || '',
    iconUrl: category?.iconUrl || '',
    parentCategoryId: category?.parentCategoryId,
    isActive: category?.isActive ?? true,
  });

  // Update form data when category changes
  useEffect(() => {
    if (category) {
      setFormData({
        name: category.name,
        description: category.description,
        iconUrl: category.iconUrl || '',
        parentCategoryId: category.parentCategoryId,
        isActive: category.isActive,
      });
    }
  }, [category]);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    
    if (!category) return;
    
    try {
      setError(null);
      
      const updateData: UpdateCategoryRequest = {
        name: formData.name !== category.name ? formData.name : undefined,
        description: formData.description !== category.description ? formData.description : undefined,
        iconUrl: formData.iconUrl !== (category.iconUrl || '') ? (formData.iconUrl || undefined) : undefined,
        parentCategoryId: formData.parentCategoryId !== category.parentCategoryId ? formData.parentCategoryId : undefined,
        isActive: formData.isActive !== category.isActive ? formData.isActive : undefined,
      };

      // Only send fields that have changed
      const hasChanges = Object.values(updateData).some(value => value !== undefined);
      if (!hasChanges) {
        onOpenChange(false);
        return;
      }

      await updateCategory(category.id, updateData);
      
      onCategoryUpdated?.();
      onOpenChange(false);
      
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to update category');
    }
  };

  const handleClose = () => {
    if (!isUpdating) {
      setError(null);
      // Reset form data to original category values
      if (category) {
        setFormData({
          name: category.name,
          description: category.description,
          iconUrl: category.iconUrl || '',
          parentCategoryId: category.parentCategoryId,
          isActive: category.isActive,
        });
      }
      onOpenChange(false);
    }
  };

  if (!category) return null;

  const hasChanges = 
    formData.name !== category.name ||
    formData.description !== category.description ||
    formData.iconUrl !== (category.iconUrl || '') ||
    formData.parentCategoryId !== category.parentCategoryId ||
    formData.isActive !== category.isActive;

  return (
    <Sheet open={open} onOpenChange={onOpenChange}>
      <SheetContent side="right" className="w-[60vw] sm:max-w-3xl overflow-hidden flex flex-col p-0">
        {/* Header with beautiful gradient */}
        <SheetHeader className="relative px-8 py-6 border-b bg-gradient-to-br from-blue-50 via-white to-purple-50">
          <div className="absolute inset-0 bg-gradient-to-br from-blue-500/5 via-transparent to-purple-500/5" />
          
          <div className="relative space-y-4">
            <div className="flex items-center justify-between">
              <div className="flex items-center space-x-4">
                <div className="h-12 w-12 rounded-xl bg-gradient-to-br from-blue-100 to-blue-50 flex items-center justify-center border border-blue-200/50 shadow-sm">
                  <Edit className="h-6 w-6 text-blue-600" />
                </div>
                <div>
                  <SheetTitle className="text-xl font-bold text-gray-900 flex items-center space-x-2">
                    <span>Edit Category</span>
                  </SheetTitle>
                  <SheetDescription className="text-sm text-gray-600 mt-1">
                    Update "{category.name}" category details
                  </SheetDescription>
                </div>
              </div>
              <Badge variant="outline" className="bg-gradient-to-r from-blue-50 to-blue-100 text-blue-700 border-blue-200 shadow-sm">
                Edit Mode
              </Badge>
            </div>
            
            {/* Quick stats preview */}
            <div className="flex items-center space-x-4 p-4 bg-white/60 backdrop-blur-sm rounded-xl border border-white/50 shadow-sm">
              <div className="flex items-center space-x-2">
                <div className="h-2 w-2 rounded-full bg-blue-500 animate-pulse" />
                <span className="text-xs font-medium text-gray-600 uppercase tracking-wider">Editing Category</span>
              </div>
              <div className="flex items-center space-x-4">
                <div className="flex items-center space-x-1">
                  <Folder className="h-3 w-3 text-blue-600" />
                  <span className="text-xs text-gray-600">ID: {category.id}</span>
                </div>
                <div className="flex items-center space-x-1">
                  <Tag className="h-3 w-3 text-green-600" />
                  <span className="text-xs text-gray-600">{formData.isActive ? 'Active' : 'Inactive'}</span>
                </div>
                {hasChanges && (
                  <div className="flex items-center space-x-1">
                    <AlertCircle className="h-3 w-3 text-orange-600" />
                    <span className="text-xs text-orange-600">Modified</span>
                  </div>
                )}
              </div>
            </div>
          </div>
        </SheetHeader>

        {/* Scrollable Form Content */}
        <div className="flex-1 overflow-y-auto">
          <form onSubmit={handleSubmit} className="p-8 space-y-8">
            
            {/* Basic Information Section */}
            <div className="space-y-6">
              <div className="flex items-center space-x-3 pb-4 border-b border-gray-100">
                <div className="h-8 w-8 rounded-lg bg-gradient-to-br from-blue-100 to-blue-50 flex items-center justify-center">
                  <FileText className="h-4 w-4 text-blue-600" />
                </div>
                <div>
                  <h3 className="font-semibold text-gray-900">Basic Information</h3>
                  <p className="text-xs text-gray-500">Update category details</p>
                </div>
              </div>
              
              {/* Category Name */}
              <div className="space-y-2">
                <Label htmlFor="name" className="text-sm font-medium text-gray-700 flex items-center space-x-1">
                  <span>Category Name</span>
                  <span className="text-red-500">*</span>
                </Label>
                <Input
                  id="name"
                  value={formData.name}
                  onChange={(e) => setFormData(prev => ({ 
                    ...prev, 
                    name: e.target.value
                  }))}
                  placeholder="Enter category name..."
                  className="h-11 border-2 focus:border-blue-300 focus:ring-blue-100"
                  required
                />
              </div>

              {/* Description */}
              <div className="space-y-2">
                <Label htmlFor="description" className="text-sm font-medium text-gray-700 flex items-center space-x-1">
                  <span>Description</span>
                  <span className="text-red-500">*</span>
                </Label>
                <div className="relative">
                  <Textarea
                    id="description"
                    value={formData.description}
                    onChange={(e) => setFormData(prev => ({ ...prev, description: e.target.value }))}
                    placeholder="Describe what this category encompasses..."
                    className="min-h-[100px] resize-none border-2 focus:border-blue-300 focus:ring-blue-100 pr-16"
                    maxLength={500}
                    required
                  />
                  <div className="absolute bottom-3 right-3 bg-gray-50 px-2 py-1 rounded text-xs text-gray-500 border">
                    {formData.description?.length || 0}/500
                  </div>
                </div>
                <p className="text-xs text-gray-500">
                  Help students understand what courses they'll find in this category
                </p>
              </div>

              {/* Icon URL */}
              <div className="space-y-2">
                <Label htmlFor="iconUrl" className="text-sm font-medium text-gray-700">
                  Icon URL (Optional)
                </Label>
                <Input
                  id="iconUrl"
                  value={formData.iconUrl}
                  onChange={(e) => setFormData(prev => ({ ...prev, iconUrl: e.target.value }))}
                  placeholder="https://example.com/category-icon.png"
                  type="url"
                  className="h-11 border-2 focus:border-blue-300 focus:ring-blue-100"
                />
                <p className="text-xs text-gray-500">
                  URL to an icon image that represents this category
                </p>
              </div>
            </div>

            <Separator className="my-6" />

            {/* Category Settings Section */}
            <div className="space-y-6">
              <div className="flex items-center space-x-3 pb-4 border-b border-gray-100">
                <div className="h-8 w-8 rounded-lg bg-gradient-to-br from-purple-100 to-purple-50 flex items-center justify-center">
                  <Settings className="h-4 w-4 text-purple-600" />
                </div>
                <div>
                  <h3 className="font-semibold text-gray-900">Category Settings</h3>
                  <p className="text-xs text-gray-500">Configure visibility and status</p>
                </div>
              </div>
              
              {/* Active Status */}
              <div className="p-4 border-2 border-gray-100 rounded-xl bg-gradient-to-br from-gray-50/50 to-white hover:border-green-200 transition-colors">
                <div className="flex items-center justify-between">
                  <div className="space-y-1">
                    <div className="flex items-center space-x-2">
                      <div className={`h-2 w-2 rounded-full transition-colors ${formData.isActive ? 'bg-green-500' : 'bg-gray-400'}`} />
                      <Label className="text-sm font-medium text-gray-700">Active Status</Label>
                    </div>
                    <p className="text-xs text-gray-500">
                      {formData.isActive ? '✨ Category is visible to students' : '📝 Category is hidden from students'}
                    </p>
                  </div>
                  <Switch
                    id="isActive"
                    checked={formData.isActive}
                    onCheckedChange={(checked) => setFormData(prev => ({ ...prev, isActive: checked }))}
                    className="data-[state=checked]:bg-green-500"
                  />
                </div>
              </div>

              {/* Changes Preview */}
              <div className="p-4 bg-gradient-to-br from-blue-50 to-purple-50 rounded-xl border border-blue-100">
                <h4 className="text-sm font-semibold text-gray-800 mb-3 flex items-center space-x-2">
                  <Sparkles className="h-4 w-4 text-blue-600" />
                  <span>Changes Preview</span>
                </h4>
                {hasChanges ? (
                  <div className="grid grid-cols-1 gap-2 text-xs">
                    {formData.name !== category.name && (
                      <div className="flex items-center justify-between p-2 bg-white/50 rounded border">
                        <span className="text-gray-500">Name:</span>
                        <span className="font-medium text-blue-700">{formData.name}</span>
                      </div>
                    )}
                    {formData.isActive !== category.isActive && (
                      <div className="flex items-center justify-between p-2 bg-white/50 rounded border">
                        <span className="text-gray-500">Status:</span>
                        <span className={`font-medium ${formData.isActive ? 'text-green-700' : 'text-gray-700'}`}>
                          {formData.isActive ? 'Active' : 'Inactive'}
                        </span>
                      </div>
                    )}
                    {formData.iconUrl !== (category.iconUrl || '') && (
                      <div className="flex items-center justify-between p-2 bg-white/50 rounded border">
                        <span className="text-gray-500">Icon:</span>
                        <span className="font-medium text-blue-700">{formData.iconUrl ? 'Updated' : 'Removed'}</span>
                      </div>
                    )}
                  </div>
                ) : (
                  <p className="text-xs text-gray-500 italic">No changes made yet</p>
                )}
              </div>
            </div>

            {/* Error Display */}
            {error && (
              <div className="flex items-start space-x-3 p-4 bg-red-50 border border-red-200 rounded-lg">
                <AlertCircle className="h-5 w-5 text-red-500 mt-0.5 flex-shrink-0" />
                <div>
                  <h4 className="text-sm font-medium text-red-800">Error updating category</h4>
                  <p className="text-sm text-red-700 mt-1">{error}</p>
                </div>
              </div>
            )}
          </form>
        </div>

        {/* Fixed Footer */}
        <SheetFooter className="border-t bg-gray-50/50 px-8 py-4">
          <div className="flex items-center justify-between w-full">
            <div className="flex items-center space-x-2 text-xs text-gray-500">
              <div className={`h-1.5 w-1.5 rounded-full ${hasChanges ? 'bg-orange-500 animate-pulse' : 'bg-blue-500'}`} />
              <span>{hasChanges ? 'Changes detected' : 'No changes made'}</span>
            </div>
            <div className="flex items-center gap-3">
              <Button
                type="button"
                variant="outline"
                onClick={handleClose}
                disabled={isUpdating}
                className="h-10 px-6 text-sm"
              >
                <X className="h-4 w-4 mr-2" />
                Cancel
              </Button>
              <Button
                type="submit"
                disabled={isUpdating || !formData.name || !formData.description || !hasChanges}
                className="h-10 px-6 bg-gradient-to-r from-blue-600 to-blue-500 hover:from-blue-500 hover:to-blue-600 text-sm"
                onClick={handleSubmit}
              >
                {isUpdating ? (
                  <>
                    <div className="h-4 w-4 mr-2 animate-spin rounded-full border-2 border-white border-t-transparent" />
                    Updating...
                  </>
                ) : (
                  <>
                    <Save className="h-4 w-4 mr-2" />
                    Save Changes
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