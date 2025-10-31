import { useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { Button } from '@/shared/components/ui/button';
import { Badge } from '@/shared/components/ui/badge';
import { Separator } from '@/shared/components/ui/separator';
import { 
  ArrowLeft, 
  Edit, 
  Trash2, 
  BookOpen, 
  Calendar, 
  Tag, 
  Users, 
  Folder,
  Eye,
  Settings,
  TrendingUp,
  Activity,
  AlertCircle,
  RotateCcw,
  BarChart3,
  Clock,
  Star,
  GraduationCap
} from 'lucide-react';
import { useCategoryManagement } from '../../hooks';
import { CategoryIcon } from '../shared/CategoryIcon';
import { CategoryStatusBadge } from '../shared/CategoryStatusBadge';
import { EditCategoryDialog, DeleteCategoryDialog } from '../dialogs';
import { coursesApi } from '@/features/courses/services';
import { 
  getCourseLevelLabel, 
  getCourseLevelColor, 
  formatCoursePrice, 
  formatCourseDuration
} from '@/features/courses/lib';
import type { CourseSummary } from '@/features/courses/types';

export function CategoryDetailsPage() {
  const { categoryId } = useParams<{ categoryId: string }>();
  const navigate = useNavigate();
  const { selectedCategory, isLoadingCategory, error, loadCategory } = useCategoryManagement();
  const [showEditDialog, setShowEditDialog] = useState(false);
  const [showDeleteDialog, setShowDeleteDialog] = useState(false);
  
  // Course state management
  const [categoryCourses, setCategoryCourses] = useState<CourseSummary[]>([]);
  const [isLoadingCourses, setIsLoadingCourses] = useState(false);
  const [coursesError, setCoursesError] = useState<string | null>(null);
  const [showAllCourses, setShowAllCourses] = useState(false);

  useEffect(() => {
    if (categoryId) {
      const id = parseInt(categoryId, 10);
      if (!isNaN(id)) {
        loadCategory(id);
        loadCategoryCourses(id);
      }
    }
  }, [categoryId, loadCategory]);

  // Reload courses when showAllCourses changes
  useEffect(() => {
    if (categoryId && selectedCategory) {
      const id = parseInt(categoryId, 10);
      if (!isNaN(id)) {
        loadCategoryCourses(id);
      }
    }
  }, [showAllCourses]);

  const loadCategoryCourses = async (categoryId: number) => {
    try {
      setIsLoadingCourses(true);
      setCoursesError(null);
      const response = await coursesApi.getCoursesByCategory(categoryId, { 
        pageSize: showAllCourses ? 100 : 6,
        page: 1
        // Removed isPublished filter to show all courses
      });
      setCategoryCourses(response.courses);
    } catch (err) {
      setCoursesError(err instanceof Error ? err.message : 'Failed to load courses');
    } finally {
      setIsLoadingCourses(false);
    }
  };

  const handleShowAllCourses = () => {
    setShowAllCourses(true);
  };

  const handleBack = () => {
    navigate('/categories');
  };

  const handleEdit = () => {
    setShowEditDialog(true);
  };

  const handleDelete = () => {
    setShowDeleteDialog(true);
  };

  const handleCategoryUpdated = () => {
    // Reload category data after update
    if (categoryId) {
      const id = parseInt(categoryId, 10);
      if (!isNaN(id)) {
        loadCategory(id);
      }
    }
  };

  const handleCategoryDeleted = () => {
    // Navigate back to categories list after deletion
    navigate('/categories');
  };

  if (isLoadingCategory) {
    return (
      <div className="min-h-screen bg-gradient-to-br from-gray-50 via-white to-gray-50">
        <div className="p-8 space-y-8">
          {/* Header Skeleton */}
          <div className="flex items-center justify-between">
            <div className="flex items-center gap-6">
              <div className="h-10 w-20 bg-gray-200 animate-pulse rounded-lg" />
              <div className="flex items-center gap-4">
                <div className="h-16 w-16 bg-gray-200 animate-pulse rounded-xl" />
                <div className="space-y-2">
                  <div className="h-8 w-64 bg-gray-200 animate-pulse rounded-lg" />
                  <div className="h-4 w-32 bg-gray-200 animate-pulse rounded-lg" />
                </div>
              </div>
            </div>
            <div className="flex items-center gap-3">
              <div className="h-9 w-20 bg-gray-200 animate-pulse rounded-lg" />
              <div className="h-9 w-24 bg-gray-200 animate-pulse rounded-lg" />
            </div>
          </div>

          {/* Content Skeleton */}
          <div className="grid grid-cols-1 lg:grid-cols-3 gap-8">
            <div className="lg:col-span-2 space-y-6">
              <div className="bg-white border rounded-2xl p-8 shadow-sm">
                <div className="h-6 w-48 bg-gray-200 animate-pulse rounded-lg mb-6" />
                <div className="space-y-4">
                  <div className="h-4 w-full bg-gray-200 animate-pulse rounded-lg" />
                  <div className="h-4 w-3/4 bg-gray-200 animate-pulse rounded-lg" />
                  <div className="h-4 w-1/2 bg-gray-200 animate-pulse rounded-lg" />
                </div>
              </div>
              <div className="bg-white border rounded-2xl p-8 shadow-sm">
                <div className="h-6 w-56 bg-gray-200 animate-pulse rounded-lg mb-6" />
                <div className="h-32 bg-gray-200 animate-pulse rounded-lg" />
              </div>
            </div>
            <div className="space-y-6">
              <div className="bg-white border rounded-2xl p-6 shadow-sm">
                <div className="h-5 w-32 bg-gray-200 animate-pulse rounded-lg mb-4" />
                <div className="h-16 bg-gray-200 animate-pulse rounded-lg" />
              </div>
              <div className="bg-white border rounded-2xl p-6 shadow-sm">
                <div className="h-5 w-40 bg-gray-200 animate-pulse rounded-lg mb-4" />
                <div className="h-24 bg-gray-200 animate-pulse rounded-lg" />
              </div>
            </div>
          </div>
        </div>
      </div>
    );
  }

  if (error || !selectedCategory) {
    return (
      <div className="min-h-screen bg-gradient-to-br from-red-50 via-white to-red-50 flex items-center justify-center">
        <div className="bg-white border border-red-200 rounded-2xl p-12 shadow-xl max-w-md w-full mx-4">
          <div className="text-center space-y-6">
            <div className="flex justify-center">
              <div className="p-4 bg-red-100 rounded-full">
                <AlertCircle className="h-12 w-12 text-red-600" />
              </div>
            </div>
            <div className="space-y-3">
              <h2 className="text-2xl font-bold text-gray-900">
                Oops! Something went wrong
              </h2>
              <p className="text-gray-600">
                We couldn't load the category details. This might be a temporary issue.
              </p>
              <div className="bg-red-50 border border-red-200 rounded-xl p-4 text-left">
                <p className="text-sm text-red-700 font-medium">Error details:</p>
                <p className="text-sm text-red-600 mt-1">{error || 'Category not found'}</p>
              </div>
            </div>
            <div className="flex flex-col sm:flex-row gap-3">
              <Button
                variant="outline"
                onClick={() => navigate('/categories')}
                className="flex-1"
              >
                <ArrowLeft className="h-4 w-4 mr-2" />
                Back to Categories
              </Button>
              <Button
                onClick={() => window.location.reload()}
                className="flex-1 bg-red-600 hover:bg-red-700"
              >
                <RotateCcw className="h-4 w-4 mr-2" />
                Try Again
              </Button>
            </div>
          </div>
        </div>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-gradient-to-br from-gray-50 via-white to-gray-50">
      <div className="p-4 sm:p-6 lg:p-8 space-y-4 sm:space-y-6">
        {/* Header */}
        <div className="flex items-center justify-between">
          <div className="flex items-center gap-4 sm:gap-6">
            <Button variant="outline" onClick={handleBack} className="h-9 sm:h-10">
              <ArrowLeft className="h-4 w-4 mr-2" />
              Back
            </Button>
            <div className="flex items-center gap-3 sm:gap-4">
              <div className="relative">
                <CategoryIcon 
                  iconUrl={selectedCategory.iconUrl}
                  className="h-14 w-14 sm:h-16 sm:w-16 rounded-xl shadow-lg"
                />
                <div className="absolute -bottom-1 -right-1 p-1 bg-white rounded-full shadow-md">
                  <div className={`h-3 w-3 rounded-full ${selectedCategory.isActive ? 'bg-green-500' : 'bg-gray-400'}`} />
                </div>
              </div>
              <div>
                <h1 className="text-2xl sm:text-3xl lg:text-4xl font-bold bg-gradient-to-r from-gray-900 to-gray-600 bg-clip-text text-transparent">
                  {selectedCategory.name}
                </h1>
                <div className="flex items-center gap-2 sm:gap-3 mt-1.5 sm:mt-2">
                  <Badge variant="outline" className="text-xs">
                    ID: {selectedCategory.id}
                  </Badge>
                  <CategoryStatusBadge isActive={selectedCategory.isActive} />
                </div>
              </div>
            </div>
          </div>
          <div className="flex items-center gap-3">
            <Button 
              variant="outline" 
              size="sm" 
              onClick={handleEdit}
              className="h-9 px-4 hover:bg-blue-50 hover:border-blue-200 hover:text-blue-700 transition-colors"
            >
              <Edit className="h-4 w-4 mr-2" />
              Edit
            </Button>
            <Button 
              variant="outline" 
              size="sm" 
              onClick={handleDelete}
              className="h-9 px-4 hover:bg-red-50 hover:border-red-200 hover:text-red-700 transition-colors"
            >
              <Trash2 className="h-4 w-4 mr-2" />
              Delete
            </Button>
          </div>
        </div>

        <Separator className="bg-gradient-to-r from-transparent via-gray-200 to-transparent" />

        {/* Main Content */}
        <div className="grid grid-cols-1 lg:grid-cols-3 gap-4 sm:gap-6 lg:gap-8">
          {/* Left Column - Main Info */}
          <div className="lg:col-span-2 space-y-4 sm:space-y-6">
            {/* Category Information Card */}
            <div className="bg-white border rounded-2xl p-4 sm:p-6 lg:p-8 shadow-sm hover:shadow-md transition-shadow">
              <div className="flex items-center gap-3 mb-4 sm:mb-6">
                <div className="p-2 bg-blue-100 rounded-lg">
                  <Folder className="h-5 w-5 text-blue-600" />
                </div>
                <h2 className="text-xl sm:text-2xl font-bold text-gray-900">Category Information</h2>
              </div>
              
              <div className="space-y-4 sm:space-y-6">
                <div>
                  <label className="flex items-center gap-2 text-sm font-semibold text-gray-700 mb-3">
                    <Tag className="h-4 w-4" />
                    Description
                  </label>
                  <div className="bg-gray-50 border rounded-xl p-4">
                    <p className="text-gray-800 leading-relaxed">
                      {selectedCategory.description || 'No description available'}
                    </p>
                  </div>
                </div>

                {selectedCategory.parentCategoryName && (
                  <div>
                    <label className="flex items-center gap-2 text-sm font-semibold text-gray-700 mb-3">
                      <Folder className="h-4 w-4" />
                      Parent Category
                    </label>
                    <Badge 
                      variant="secondary" 
                      className="px-4 py-2 text-sm bg-purple-100 text-purple-800 hover:bg-purple-200 transition-colors"
                    >
                      {selectedCategory.parentCategoryName}
                    </Badge>
                  </div>
                )}

                <div>
                  <label className="flex items-center gap-2 text-sm font-semibold text-gray-700 mb-3">
                    <Calendar className="h-4 w-4" />
                    Created Date
                  </label>
                  <div className="flex items-center gap-2 text-gray-600">
                    <span>{new Date(selectedCategory.createdAt).toLocaleDateString('en-US', {
                      weekday: 'long',
                      year: 'numeric',
                      month: 'long',
                      day: 'numeric'
                    })}</span>
                  </div>
                </div>
              </div>
            </div>

            {/* Course List Card */}
            <div className="bg-white border rounded-2xl p-4 sm:p-6 lg:p-8 shadow-sm hover:shadow-md transition-shadow">
              <div className="flex items-center justify-between mb-4 sm:mb-6">
                <div className="flex items-center gap-3">
                  <div className="p-2 bg-green-100 rounded-lg">
                    <BookOpen className="h-5 w-5 text-green-600" />
                  </div>
                  <h2 className="text-xl sm:text-2xl font-bold text-gray-900">Courses in this Category</h2>
                </div>
                {!showAllCourses && selectedCategory.courseCount > 6 && (
                  <Button 
                    variant="outline" 
                    size="sm"
                    onClick={handleShowAllCourses}
                    className="hover:bg-green-50 hover:border-green-200"
                  >
                    View All ({selectedCategory.courseCount})
                  </Button>
                )}
                {showAllCourses && (
                  <Badge variant="secondary" className="bg-green-100 text-green-800">
                    Showing all {categoryCourses.length} courses
                  </Badge>
                )}
              </div>
              
              {/* Course Status Legend */}
              {categoryCourses.length > 0 && (
                <div className="flex items-center gap-3 sm:gap-4 mb-3 sm:mb-4 text-sm">
                  <span className="text-gray-600">Status:</span>
                  <div className="flex items-center gap-1">
                    <Badge className="bg-green-100 text-green-800 text-xs">Published</Badge>
                    <span className="text-gray-500">- Live courses</span>
                  </div>
                  <div className="flex items-center gap-1">
                    <Badge className="bg-yellow-100 text-yellow-800 text-xs">Draft</Badge>
                    <span className="text-gray-500">- Work in progress</span>
                  </div>
                </div>
              )}
              
              {isLoadingCourses ? (
                <div className="space-y-4">
                  {[...Array(3)].map((_, index) => (
                    <div key={index} className="border rounded-xl p-4">
                      <div className="flex gap-4">
                        <div className="h-16 w-24 bg-gray-200 animate-pulse rounded-lg" />
                        <div className="flex-1 space-y-2">
                          <div className="h-5 w-3/4 bg-gray-200 animate-pulse rounded" />
                          <div className="h-4 w-1/2 bg-gray-200 animate-pulse rounded" />
                          <div className="flex gap-2">
                            <div className="h-3 w-16 bg-gray-200 animate-pulse rounded" />
                            <div className="h-3 w-12 bg-gray-200 animate-pulse rounded" />
                          </div>
                        </div>
                      </div>
                    </div>
                  ))}
                </div>
              ) : coursesError ? (
                <div className="bg-red-50 border border-red-200 rounded-xl p-6 text-center">
                  <div className="flex justify-center mb-3">
                    <AlertCircle className="h-8 w-8 text-red-600" />
                  </div>
                  <p className="text-red-700 font-medium mb-2">Failed to load courses</p>
                  <p className="text-red-600 text-sm">{coursesError}</p>
                </div>
              ) : categoryCourses.length === 0 ? (
                <div className="bg-gradient-to-r from-gray-50 to-blue-50 border border-gray-200 rounded-xl p-8 text-center">
                  <div className="flex justify-center mb-4">
                    <div className="p-3 bg-gray-100 rounded-full">
                      <BookOpen className="h-8 w-8 text-gray-600" />
                    </div>
                  </div>
                  <h3 className="text-lg font-semibold text-gray-900 mb-2">No courses yet</h3>
                  <p className="text-gray-600 mb-4">
                    This category doesn't have any courses yet. Create some courses and assign them to this category!
                  </p>
                  <Badge variant="secondary" className="bg-gray-100 text-gray-800">
                    Coming Soon
                  </Badge>
                </div>
              ) : (
                <div className="space-y-3 sm:space-y-4">
                  {categoryCourses.map((course) => (
                    <div 
                      key={course.id} 
                      className={`border rounded-xl p-4 sm:p-6 hover:shadow-md transition-shadow cursor-pointer ${
                        course.isPublished ? '' : 'bg-yellow-50 border-yellow-200'
                      }`}
                      onClick={() => navigate(`/courses/${course.id}`)}
                    >
                      <div className="flex gap-4 sm:gap-6">
                        {/* Course Thumbnail */}
                        <div className="flex-shrink-0">
                          <div className="h-16 w-24 sm:h-20 sm:w-32 bg-gradient-to-br from-blue-100 to-purple-100 rounded-lg flex items-center justify-center border">
                            {course.thumbnailUrl ? (
                              <img 
                                src={course.thumbnailUrl} 
                                alt={course.title}
                                className="h-full w-full object-cover rounded-lg"
                              />
                            ) : (
                              <BookOpen className="h-8 w-8 text-blue-600" />
                            )}
                          </div>
                        </div>

                        {/* Course Info */}
                        <div className="flex-1 min-w-0">
                          <div className="flex items-start justify-between mb-2 sm:mb-3">
                            <div className="flex-1 min-w-0">
                              <div className="flex items-center gap-2 mb-1">
                                <h3 className="text-base sm:text-lg font-semibold text-gray-900 truncate">
                                  {course.title}
                                </h3>
                                <Badge 
                                  variant={course.isPublished ? "default" : "secondary"}
                                  className={`text-xs ${course.isPublished 
                                    ? 'bg-green-100 text-green-800' 
                                    : 'bg-yellow-100 text-yellow-800'}`}
                                >
                                  {course.isPublished ? 'Published' : 'Draft'}
                                </Badge>
                              </div>
                              <p className="text-gray-600 text-sm">
                                {course.shortDescription.length > 100 
                                  ? `${course.shortDescription.substring(0, 100)}...` 
                                  : course.shortDescription}
                              </p>
                            </div>
                            <div className="flex flex-col items-end gap-1 ml-4">
                              <div className="text-lg font-bold text-green-600">
                                {formatCoursePrice(course.effectivePrice)}
                              </div>
                              {course.discountPrice && course.discountPrice < course.price && (
                                <div className="text-sm text-gray-500 line-through">
                                  {formatCoursePrice(course.price)}
                                </div>
                              )}
                            </div>
                          </div>

                          {/* Course Meta */}
                          <div className="flex items-center gap-3 sm:gap-4 text-xs sm:text-sm text-gray-600 flex-wrap">
                            <div className="flex items-center gap-1">
                              <Clock className="h-4 w-4" />
                              <span>{formatCourseDuration(course.durationHours)}</span>
                            </div>
                            
                            <div className="flex items-center gap-1">
                              <GraduationCap className="h-4 w-4" />
                              <Badge 
                                variant="secondary" 
                                className={`text-xs ${getCourseLevelColor(course.level)}`}
                              >
                                {getCourseLevelLabel(course.level)}
                              </Badge>
                            </div>

                            <div className="flex items-center gap-1">
                              <Users className="h-4 w-4" />
                              <span>{course.totalStudents} students</span>
                            </div>

                            {course.averageRating && (
                              <div className="flex items-center gap-1">
                                <Star className="h-4 w-4 fill-yellow-400 text-yellow-400" />
                                <span>{course.averageRating.toFixed(1)} ({course.totalReviews})</span>
                              </div>
                            )}
                          </div>

                          {/* Instructor */}
                          <div className="mt-3 text-sm text-gray-700">
                            <span className="font-medium">Instructor:</span> {course.instructorName}
                          </div>

                          {/* Featured Badge */}
                          {course.isFeatured && (
                            <div className="mt-2">
                              <Badge className="bg-yellow-100 text-yellow-800 border-yellow-200">
                                ⭐ Featured
                              </Badge>
                            </div>
                          )}
                        </div>
                      </div>
                    </div>
                  ))}

                  {/* Show More Button */}
                  {!showAllCourses && categoryCourses.length >= 6 && selectedCategory.courseCount > categoryCourses.length && (
                    <div className="text-center pt-4">
                      <Button 
                        variant="outline" 
                        onClick={handleShowAllCourses}
                        className="hover:bg-green-50 hover:border-green-200"
                      >
                        <Eye className="h-4 w-4 mr-2" />
                        Show All {selectedCategory.courseCount} Courses
                      </Button>
                    </div>
                  )}
                  {showAllCourses && categoryCourses.length > 6 && (
                    <div className="text-center pt-4">
                      <Button 
                        variant="outline" 
                        onClick={() => setShowAllCourses(false)}
                        className="hover:bg-gray-50 hover:border-gray-200"
                      >
                        Show Less
                      </Button>
                    </div>
                  )}
                </div>
              )}
            </div>
          </div>

          {/* Right Column - Statistics and Details */}
          <div className="space-y-4 sm:space-y-6">
            {/* Statistics Card */}
            <div className="bg-white border rounded-2xl p-4 sm:p-6 shadow-sm hover:shadow-md transition-shadow">
              <div className="flex items-center gap-3 mb-3 sm:mb-4">
                <div className="p-2 bg-orange-100 rounded-lg">
                  <BarChart3 className="h-5 w-5 text-orange-600" />
                </div>
                <h3 className="text-base sm:text-lg font-bold text-gray-900">Statistics</h3>
              </div>
              
              <div className="space-y-3 sm:space-y-4">
                <div className="bg-gradient-to-r from-blue-50 to-blue-100 border border-blue-200 rounded-xl p-3 sm:p-4">
                  <div className="flex items-center justify-between">
                    <div className="flex items-center gap-2 sm:gap-3">
                      <div className="p-1.5 sm:p-2 bg-blue-200 rounded-lg">
                        <BookOpen className="h-3.5 w-3.5 sm:h-4 sm:w-4 text-blue-700" />
                      </div>
                      <div>
                        <span className="text-xs sm:text-sm font-medium text-blue-900">Total Courses</span>
                        <p className="text-[10px] sm:text-xs text-blue-700">Courses in category</p>
                      </div>
                    </div>
                    <span className="text-xl sm:text-2xl font-bold text-blue-800">
                      {selectedCategory.courseCount}
                    </span>
                  </div>
                </div>

                <div className="bg-gradient-to-r from-green-50 to-green-100 border border-green-200 rounded-xl p-3 sm:p-4">
                  <div className="flex items-center justify-between">
                    <div className="flex items-center gap-2 sm:gap-3">
                      <div className="p-1.5 sm:p-2 bg-green-200 rounded-lg">
                        <Users className="h-3.5 w-3.5 sm:h-4 sm:w-4 text-green-700" />
                      </div>
                      <div>
                        <span className="text-xs sm:text-sm font-medium text-green-900">Active Status</span>
                        <p className="text-[10px] sm:text-xs text-green-700">Category visibility</p>
                      </div>
                    </div>
                    <div className={`px-3 py-1 rounded-full text-xs font-medium ${
                      selectedCategory.isActive 
                        ? 'bg-green-200 text-green-800' 
                        : 'bg-gray-200 text-gray-700'
                    }`}>
                      {selectedCategory.isActive ? 'Active' : 'Inactive'}
                    </div>
                  </div>
                </div>
              </div>
            </div>

            {/* Quick Actions Card */}
            <div className="bg-white border rounded-2xl p-4 sm:p-6 shadow-sm hover:shadow-md transition-shadow">
              <div className="flex items-center gap-3 mb-3 sm:mb-4">
                <div className="p-2 bg-purple-100 rounded-lg">
                  <Settings className="h-5 w-5 text-purple-600" />
                </div>
                <h3 className="text-base sm:text-lg font-bold text-gray-900">Quick Actions</h3>
              </div>
              
              <div className="space-y-2 sm:space-y-3">
                <Button 
                  variant="outline" 
                  className="w-full justify-start hover:bg-blue-50 hover:border-blue-200 transition-colors"
                  onClick={handleEdit}
                >
                  <Edit className="h-4 w-4 mr-3" />
                  Edit Category Details
                </Button>
                
                <Button 
                  variant="outline" 
                  className="w-full justify-start hover:bg-green-50 hover:border-green-200 transition-colors"
                >
                  <Eye className="h-4 w-4 mr-3" />
                  View All Courses
                </Button>
                
                <Button 
                  variant="outline" 
                  className="w-full justify-start hover:bg-orange-50 hover:border-orange-200 transition-colors"
                >
                  <TrendingUp className="h-4 w-4 mr-3" />
                  View Analytics
                </Button>
                
                <Separator className="my-3" />
                
                <Button 
                  variant="outline" 
                  className="w-full justify-start hover:bg-red-50 hover:border-red-200 hover:text-red-600 transition-colors"
                  onClick={handleDelete}
                >
                  <Trash2 className="h-4 w-4 mr-3" />
                  Delete Category
                </Button>
              </div>
            </div>

            {/* Activity Card */}
            <div className="bg-white border rounded-2xl p-4 sm:p-6 shadow-sm hover:shadow-md transition-shadow">
              <div className="flex items-center gap-3 mb-3 sm:mb-4">
                <div className="p-2 bg-indigo-100 rounded-lg">
                  <Activity className="h-5 w-5 text-indigo-600" />
                </div>
                <h3 className="text-base sm:text-lg font-bold text-gray-900">Recent Activity</h3>
              </div>
              
              <div className="space-y-2 sm:space-y-3">
                <div className="flex items-center gap-2 sm:gap-3 p-2 sm:p-3 bg-gray-50 rounded-lg">
                  <div className="p-1 bg-blue-100 rounded">
                    <Calendar className="h-3 w-3 text-blue-600" />
                  </div>
                  <div className="flex-1 min-w-0">
                    <p className="text-sm font-medium text-gray-900 truncate">
                      Category created
                    </p>
                    <p className="text-xs text-gray-500">
                      {new Date(selectedCategory.createdAt).toLocaleDateString()}
                    </p>
                  </div>
                </div>
                
                <div className="text-center py-4">
                  <Badge variant="secondary" className="bg-indigo-100 text-indigo-800">
                    Activity tracking coming soon
                  </Badge>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      {/* Dialog Components */}
      <EditCategoryDialog
        category={selectedCategory}
        open={showEditDialog}
        onOpenChange={setShowEditDialog}
        onCategoryUpdated={handleCategoryUpdated}
      />

      <DeleteCategoryDialog
        category={selectedCategory}
        open={showDeleteDialog}
        onOpenChange={setShowDeleteDialog}
        onCategoryDeleted={handleCategoryDeleted}
      />
    </div>
  );
}