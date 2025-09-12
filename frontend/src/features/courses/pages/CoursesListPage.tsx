import { useState } from 'react';
import { Plus } from 'lucide-react';
import { Button } from '@/shared/components/ui/button';
import { CourseTable } from '../components/tables/CourseTable';
import { CourseFilters } from '../components/filters/CourseFilters';
import { CreateCourseDialog } from '../components/dialogs/CreateCourseDialog';
import { DeleteCourseDialog } from '../components/dialogs/DeleteCourseDialog';
import type { CourseSummary, Course } from '../types';

// Local interface for the page-specific filters
interface PageFilters {
  search: string;
  category: string;
  instructor: string;
  level: string;
  priceRange: string;
  status: string;
}

export function CoursesListPage() {
  const [selectedCourses, setSelectedCourses] = useState<number[]>([]);
  const [courseToDelete, setCourseToDelete] = useState<CourseSummary | null>(null);
  const [isCreateDialogOpen, setIsCreateDialogOpen] = useState(false);
  const [filters, setFilters] = useState<PageFilters>({
    search: '',
    category: 'all',
    instructor: 'all',
    level: 'all',
    priceRange: 'all',
    status: 'all',
  });

  // Mock data - replace with actual API call
  const [courses, setCourses] = useState<CourseSummary[]>([
    {
      id: 1,
      title: 'Complete Web Development Bootcamp',
      shortDescription: 'Learn HTML, CSS, JavaScript, React, Node.js and more',
      thumbnailUrl: '/api/placeholder/400/300',
      categoryName: 'Web Development',
      instructorName: 'John Doe',
      level: 1, // Beginner
      price: 199.99,
      discountPrice: 149.99,
      effectivePrice: 149.99,
      isDiscounted: true,
      totalStudents: 1250,
      averageRating: 4.8,
      isPublished: true,
      isFeatured: true,
      createdAt: '2024-01-15T10:00:00Z',
    },
    {
      id: 2,
      title: 'Advanced React Patterns',
      shortDescription: 'Master advanced React concepts and patterns',
      thumbnailUrl: '/api/placeholder/400/300',
      categoryName: 'Web Development',
      instructorName: 'Jane Smith',
      level: 3, // Advanced
      price: 299.99,
      discountPrice: undefined,
      effectivePrice: 299.99,
      isDiscounted: false,
      totalStudents: 380,
      averageRating: 4.9,
      isPublished: true,
      isFeatured: false,
      createdAt: '2024-01-10T09:00:00Z',
    },
    {
      id: 3,
      title: 'Python for Data Science',
      shortDescription: 'Learn Python programming for data analysis and machine learning',
      thumbnailUrl: '/api/placeholder/400/300',
      categoryName: 'Data Science',
      instructorName: 'Mike Johnson',
      level: 2, // Intermediate
      price: 249.99,
      discountPrice: 199.99,
      effectivePrice: 199.99,
      isDiscounted: true,
      totalStudents: 890,
      averageRating: 4.7,
      isPublished: false,
      isFeatured: true,
      createdAt: '2024-01-05T08:00:00Z',
    },
  ]);

  const handleCourseCreated = (newCourse: Course) => {
    // Convert Course to CourseSummary for the list
    const courseSummary: CourseSummary = {
      id: newCourse.id,
      title: newCourse.title,
      shortDescription: newCourse.shortDescription,
      instructorName: newCourse.instructorName,
      categoryName: newCourse.categoryName,
      price: newCourse.price,
      discountPrice: newCourse.discountPrice,
      level: newCourse.level,
      thumbnailUrl: newCourse.thumbnailUrl,
      isPublished: newCourse.isPublished,
      isFeatured: newCourse.isFeatured,
      totalStudents: 0, // New course starts with 0 students
      averageRating: 0, // New course starts with 0 rating
      createdAt: newCourse.createdAt,
      effectivePrice: newCourse.effectivePrice,
      isDiscounted: newCourse.isDiscounted,
    };
    setCourses(prev => [courseSummary, ...prev]);
  };

  const handleCourseDeleted = (deletedCourseId: number) => {
    setCourses(prev => prev.filter(course => course.id !== deletedCourseId));
    setSelectedCourses(prev => prev.filter(id => id !== deletedCourseId));
  };

  const handleBulkDelete = () => {
    setCourses(prev => prev.filter(course => !selectedCourses.includes(course.id)));
    setSelectedCourses([]);
  };

  const handleBulkPublish = () => {
    setCourses(prev => prev.map(course => 
      selectedCourses.includes(course.id) 
        ? { ...course, isPublished: true }
        : course
    ));
    setSelectedCourses([]);
  };

  const handleBulkUnpublish = () => {
    setCourses(prev => prev.map(course => 
      selectedCourses.includes(course.id) 
        ? { ...course, isPublished: false }
        : course
    ));
    setSelectedCourses([]);
  };

  const handleTogglePublished = (courseId: number) => {
    setCourses(prev => prev.map(course => 
      course.id === courseId 
        ? { ...course, isPublished: !course.isPublished }
        : course
    ));
  };

  const handleToggleFeatured = (courseId: number) => {
    setCourses(prev => prev.map(course => 
      course.id === courseId 
        ? { ...course, isFeatured: !course.isFeatured }
        : course
    ));
  };

  // Filter courses based on current filters
  const filteredCourses = courses.filter(course => {
    // Search filter
    if (filters.search && !course.title.toLowerCase().includes(filters.search.toLowerCase()) &&
        !course.shortDescription.toLowerCase().includes(filters.search.toLowerCase()) &&
        !course.instructorName.toLowerCase().includes(filters.search.toLowerCase())) {
      return false;
    }

    // Category filter
    if (filters.category !== 'all' && course.categoryName !== filters.category) {
      return false;
    }

    // Instructor filter
    if (filters.instructor !== 'all' && course.instructorName !== filters.instructor) {
      return false;
    }

    // Level filter
    if (filters.level !== 'all') {
      const levelMap: { [key: string]: number } = {
        'beginner': 1,
        'intermediate': 2,
        'advanced': 3,
        'expert': 4
      };
      if (course.level !== levelMap[filters.level]) {
        return false;
      }
    }

    // Price range filter
    if (filters.priceRange !== 'all') {
      const price = course.effectivePrice;
      switch (filters.priceRange) {
        case 'free':
          if (price > 0) return false;
          break;
        case 'under-50':
          if (price >= 50) return false;
          break;
        case '50-100':
          if (price < 50 || price >= 100) return false;
          break;
        case '100-200':
          if (price < 100 || price >= 200) return false;
          break;
        case 'over-200':
          if (price < 200) return false;
          break;
      }
    }

    // Status filter
    if (filters.status !== 'all') {
      switch (filters.status) {
        case 'published':
          if (!course.isPublished) return false;
          break;
        case 'draft':
          if (course.isPublished) return false;
          break;
        case 'featured':
          if (!course.isFeatured) return false;
          break;
        case 'discounted':
          if (!course.isDiscounted) return false;
          break;
      }
    }

    return true;
  });

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-2xl font-bold tracking-tight">Courses</h1>
          <p className="text-muted-foreground">
            Manage your course catalog and content
          </p>
        </div>
        <Button 
          onClick={() => setIsCreateDialogOpen(true)}
          className="flex items-center gap-2"
        >
          <Plus className="h-4 w-4" />
          Create Course
        </Button>
      </div>

      {/* Filters */}
      <CourseFilters 
        filters={filters}
        onFiltersChange={setFilters}
        resultCount={filteredCourses.length}
        totalCount={courses.length}
      />

      {/* Table */}
      <CourseTable
        courses={filteredCourses}
        selectedCourses={selectedCourses}
        onSelectionChange={setSelectedCourses}
        onDeleteCourse={setCourseToDelete}
        onTogglePublished={handleTogglePublished}
        onToggleFeatured={handleToggleFeatured}
        onBulkDelete={handleBulkDelete}
        onBulkPublish={handleBulkPublish}
        onBulkUnpublish={handleBulkUnpublish}
      />

      {/* Dialogs */}
      <CreateCourseDialog
        open={isCreateDialogOpen}
        onOpenChange={setIsCreateDialogOpen}
        onCourseCreated={handleCourseCreated}
      />

      <DeleteCourseDialog
        course={courseToDelete}
        open={!!courseToDelete}
        onOpenChange={(open) => !open && setCourseToDelete(null)}
        onCourseDeleted={handleCourseDeleted}
      />
    </div>
  );
}
