import { Link } from 'react-router-dom';
import { Button } from '@/shared/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '@/shared/components/ui/card';
import { Input } from '@/shared/components/ui/input';
import { 
  BookOpen, 
  Search,
  ArrowRight,
  Clock,
  Play,
} from 'lucide-react';
import { LessonListSkeleton, LessonStatusBadge, LessonAccessBadge } from '../shared';
import { formatLessonDuration, sortLessonsByOrder } from '../../lib';
import { useState, useEffect } from 'react';
import type { LessonSummary } from '../../types';

export function AllLessonsPage() {
  const [allLessons, setAllLessons] = useState<Array<LessonSummary & { courseId: number; courseTitle: string }>>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [searchTerm, setSearchTerm] = useState('');
  const [selectedCourseId, setSelectedCourseId] = useState<number | null>(null);

  // This would ideally fetch from a backend endpoint that returns all lessons
  // For now, we'll show a message directing users to courses
  useEffect(() => {
    setIsLoading(false);
  }, []);

  const filteredLessons = allLessons.filter(lesson => {
    if (searchTerm && !lesson.title.toLowerCase().includes(searchTerm.toLowerCase())) {
      return false;
    }
    if (selectedCourseId && lesson.courseId !== selectedCourseId) {
      return false;
    }
    return true;
  });

  return (
    <div className="p-6 space-y-6">
      {/* Header */}
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-3xl font-bold">All Lessons</h1>
          <p className="text-gray-600 mt-1">Browse and manage lessons across all courses</p>
        </div>
      </div>

      {/* Search and Filters */}
      <Card>
        <CardContent className="p-4">
          <div className="flex items-center gap-4">
            <div className="relative flex-1">
              <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 h-4 w-4 text-gray-400" />
              <Input
                placeholder="Search lessons..."
                value={searchTerm}
                onChange={(e) => setSearchTerm(e.target.value)}
                className="pl-10"
              />
            </div>
          </div>
        </CardContent>
      </Card>

      {/* Info Card */}
      <Card className="border-blue-200 bg-blue-50/50">
        <CardContent className="p-6">
          <div className="flex items-start gap-4">
            <BookOpen className="h-6 w-6 text-blue-600 mt-1" />
            <div className="flex-1">
              <h3 className="font-semibold text-lg mb-2">Manage Lessons by Course</h3>
              <p className="text-gray-700 mb-4">
                To manage lessons, navigate to a specific course and access its lessons page. 
                You can create, edit, and organize lessons from there.
              </p>
              <Link to="/courses">
                <Button>
                  Browse Courses
                  <ArrowRight className="h-4 w-4 ml-2" />
                </Button>
              </Link>
            </div>
          </div>
        </CardContent>
      </Card>

      {/* Lessons List */}
      {isLoading ? (
        <LessonListSkeleton />
      ) : filteredLessons.length === 0 ? (
        <Card>
          <CardContent className="p-8 text-center">
            <BookOpen className="h-12 w-12 mx-auto mb-4 text-gray-400" />
            <h3 className="text-lg font-semibold mb-2">No Lessons Found</h3>
            <p className="text-gray-600 mb-4">
              {searchTerm 
                ? 'No lessons match your search criteria.'
                : 'Lessons are organized by courses. Navigate to a course to view its lessons.'}
            </p>
            <Link to="/courses">
              <Button variant="outline">Go to Courses</Button>
            </Link>
          </CardContent>
        </Card>
      ) : (
        <div className="space-y-4">
          {sortLessonsByOrder(filteredLessons as any[]).map((lesson) => (
            <Card key={lesson.id} className="hover:shadow-md transition-shadow">
              <CardHeader>
                <div className="flex items-start justify-between">
                  <div className="flex-1">
                    <div className="flex items-center gap-3 mb-2">
                      <CardTitle className="text-xl">{lesson.title}</CardTitle>
                      <span className="text-sm text-gray-500">#{lesson.orderIndex}</span>
                    </div>
                    <p className="text-gray-600 text-sm mb-3">{lesson.description}</p>
                    <div className="flex items-center gap-3 mb-2">
                      <LessonStatusBadge isPublished={lesson.isPublished} size="sm" />
                      <LessonAccessBadge isFree={lesson.isFree} size="sm" />
                      <div className="flex items-center gap-1 text-sm text-gray-500">
                        <Clock className="h-4 w-4" />
                        {formatLessonDuration(lesson.duration)}
                      </div>
                    </div>
                    <Link 
                      to={`/courses/${lesson.courseId}`}
                      className="text-sm text-blue-600 hover:underline flex items-center gap-1"
                    >
                      <BookOpen className="h-3 w-3" />
                      {(lesson as any).courseTitle || `Course ${lesson.courseId}`}
                    </Link>
                  </div>
                  
                  <div className="flex items-center gap-2">
                    <Link to={`/lessons/${lesson.id}`}>
                      <Button variant="outline" size="sm">
                        <Play className="h-4 w-4 mr-2" />
                        View
                      </Button>
                    </Link>
                  </div>
                </div>
              </CardHeader>
            </Card>
          ))}
        </div>
      )}
    </div>
  );
}

