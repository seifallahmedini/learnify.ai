import { useState } from 'react';
import { Button } from '@/shared/components/ui/button';
import {
  Sheet,
  SheetContent,
  SheetDescription,
  SheetFooter,
  SheetHeader,
  SheetTitle,
  SheetTrigger,
} from '@/shared/components/ui/sheet';
import { Input } from '@/shared/components/ui/input';
import { Textarea } from '@/shared/components/ui/textarea';
import { Label } from '@/shared/components/ui/label';
import { Switch } from '@/shared/components/ui/switch';
import { Badge } from '@/shared/components/ui/badge';
import { Separator } from '@/shared/components/ui/separator';
import { 
  Plus, 
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
import type { CreateCategoryRequest } from '../../types';

interface CreateCategoryDialogProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  onCategoryCreated?: () => void;
  trigger?: React.ReactNode;
}

export function CreateCategoryDialog({
  open,
  onOpenChange,
  onCategoryCreated,
  trigger,
}: CreateCategoryDialogProps) {
  const { createCategory, isCreating } = useCategoryManagement();
  const [error, setError] = useState<string | null>(null);
  const [formData, setFormData] = useState({
    name: '',
    description: '',
    iconUrl: '',
    parentCategoryId: undefined as number | undefined,
    isActive: true,
  });

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    
    try {
      setError(null);
      
      const categoryData: CreateCategoryRequest = {
        name: formData.name,
        description: formData.description,
        iconUrl: formData.iconUrl || undefined,
        parentCategoryId: formData.parentCategoryId,
        isActive: formData.isActive,
      };

      await createCategory(categoryData);
      
      // Reset form
      setFormData({
        name: '',
        description: '',
        iconUrl: '',
        parentCategoryId: undefined,
        isActive: true,
      });
      
      onCategoryCreated?.();
      onOpenChange(false);
      
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to create category');
    }
  };

  const handleClose = () => {
    if (!isCreating) {
      setError(null);
      setFormData({
        name: '',
        description: '',
        iconUrl: '',
        parentCategoryId: undefined,
        isActive: true,
      });
      onOpenChange(false);
    }
  };

  const defaultTrigger = (
    <Button>
      <Plus className="h-4 w-4 mr-2" />
      Create Category
    </Button>
  );

  return (
    <Sheet open={open} onOpenChange={onOpenChange}>
      <SheetTrigger asChild>
        {trigger}
      </SheetTrigger>
      <SheetContent className="w-[60vw] sm:max-w-3xl overflow-hidden flex flex-col p-0">
        {/* Header with beautiful gradient */}
        <SheetHeader className="relative px-8 py-6 border-b bg-gradient-to-br from-purple-50 via-white to-blue-50">
          <div className="absolute inset-0 bg-gradient-to-br from-purple-500/5 via-transparent to-blue-500/5" />
          
          <div className="relative space-y-4">
            <div className="flex items-center justify-between">
              <div className="flex items-center space-x-4">
                <div className="h-12 w-12 rounded-xl bg-gradient-to-br from-purple-100 to-purple-50 flex items-center justify-center border border-purple-200/50 shadow-sm">
                  <Sparkles className="h-6 w-6 text-purple-600" />
                </div>
                <div>
                  <SheetTitle className="text-xl font-bold text-gray-900 flex items-center space-x-2">
                    <span>Create New Category</span>
                  </SheetTitle>
                  <SheetDescription className="text-sm text-gray-600 mt-1">
                    Build organized learning paths for your courses
                  </SheetDescription>
                </div>
              </div>
              <Badge variant="outline" className="bg-gradient-to-r from-purple-50 to-purple-100 text-purple-700 border-purple-200 shadow-sm">
                New Category
              </Badge>
            </div>
            
            {/* Quick stats preview */}
            <div className="flex items-center space-x-4 p-4 bg-white/60 backdrop-blur-sm rounded-xl border border-white/50 shadow-sm">
              <div className="flex items-center space-x-2">
                <div className="h-2 w-2 rounded-full bg-purple-500 animate-pulse" />
                <span className="text-xs font-medium text-gray-600 uppercase tracking-wider">Ready to Create</span>
              </div>
              <div className="flex items-center space-x-4">
                <div className="flex items-center space-x-1">
                  <Folder className="h-3 w-3 text-purple-600" />
                  <span className="text-xs text-gray-600">Category</span>
                </div>
                <div className="flex items-center space-x-1">
                  <Tag className="h-3 w-3 text-green-600" />
                  <span className="text-xs text-gray-600">Active</span>
                </div>
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
                <div className="h-8 w-8 rounded-lg bg-gradient-to-br from-purple-100 to-purple-50 flex items-center justify-center">
                  <FileText className="h-4 w-4 text-purple-600" />
                </div>
                <div>
                  <h3 className="font-semibold text-gray-900">Basic Information</h3>
                  <p className="text-xs text-gray-500">Essential category details</p>
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
                  placeholder="Enter a clear and descriptive category name..."
                  className="h-11 border-2 focus:border-purple-300 focus:ring-purple-100"
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
                    placeholder="Describe what this category encompasses and what types of courses it will contain..."
                    className="min-h-[100px] resize-none border-2 focus:border-purple-300 focus:ring-purple-100 pr-16"
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
                  className="h-11 border-2 focus:border-purple-300 focus:ring-purple-100"
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
                <div className="h-8 w-8 rounded-lg bg-gradient-to-br from-blue-100 to-blue-50 flex items-center justify-center">
                  <Settings className="h-4 w-4 text-blue-600" />
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
                      {formData.isActive ? '✨ Category will be visible to students' : '📝 Category will be hidden from students'}
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

              {/* Live Preview */}
              <div className="p-4 bg-gradient-to-br from-purple-50 to-blue-50 rounded-xl border border-purple-100">
                <h4 className="text-sm font-semibold text-gray-800 mb-3 flex items-center space-x-2">
                  <Folder className="h-4 w-4 text-purple-600" />
                  <span>Category Preview</span>
                </h4>
                <div className="grid grid-cols-2 gap-3 text-xs">
                  <div className="space-y-1">
                    <span className="text-gray-500">Name:</span>
                    <p className="font-medium text-gray-800 truncate">{formData.name || 'Enter category name'}</p>
                  </div>
                  <div className="space-y-1">
                    <span className="text-gray-500">Status:</span>
                    <div className="flex items-center space-x-1">
                      <div className={`h-1.5 w-1.5 rounded-full ${formData.isActive ? 'bg-green-500' : 'bg-gray-400'}`} />
                      <span className="font-medium">{formData.isActive ? 'Active' : 'Inactive'}</span>
                    </div>
                  </div>
                  <div className="space-y-1">
                    <span className="text-gray-500">Icon:</span>
                    <span className="font-medium">{formData.iconUrl ? '🖼️ Custom' : '📁 Default'}</span>
                  </div>
                  <div className="space-y-1">
                    <span className="text-gray-500">Description:</span>
                    <span className="font-medium">{formData.description ? '✅ Added' : '⏳ Pending'}</span>
                  </div>
                </div>
              </div>
            </div>

            {/* Error Display */}
            {error && (
              <div className="flex items-start space-x-3 p-4 bg-red-50 border border-red-200 rounded-lg">
                <AlertCircle className="h-5 w-5 text-red-500 mt-0.5 flex-shrink-0" />
                <div>
                  <h4 className="text-sm font-medium text-red-800">Error creating category</h4>
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
              <div className="h-1.5 w-1.5 rounded-full bg-purple-500 animate-pulse" />
              <span>Ready to organize your courses</span>
            </div>
            <div className="flex items-center gap-3">
              <Button
                type="button"
                variant="outline"
                onClick={handleClose}
                disabled={isCreating}
                className="h-10 px-6 text-sm"
              >
                <X className="h-4 w-4 mr-2" />
                Cancel
              </Button>
              <Button
                type="submit"
                disabled={isCreating || !formData.name || !formData.description}
                className="h-10 px-6 bg-gradient-to-r from-purple-600 to-purple-500 hover:from-purple-500 hover:to-purple-600 text-sm"
                onClick={handleSubmit}
              >
                {isCreating ? (
                  <>
                    <div className="h-4 w-4 mr-2 animate-spin rounded-full border-2 border-white border-t-transparent" />
                    Creating...
                  </>
                ) : (
                  <>
                    <Save className="h-4 w-4 mr-2" />
                    Create Category
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