import { useParams, Link } from 'react-router-dom';
import { useState } from 'react';
import { Button } from '@/shared/components/ui/button';
import { Card, CardContent } from '@/shared/components/ui/card';
import { Switch } from '@/shared/components/ui/switch';
import { Badge } from '@/shared/components/ui/badge';
import { 
  BookOpen,
  Clock,
  ArrowLeft,
  ChevronUp,
  ChevronDown,
  Play
} from 'lucide-react';
import { CreateLessonDialog, EditLessonDialog, DeleteLessonDialog } from '../dialogs';
import { LessonListSkeleton, LessonStatusBadge, LessonAccessBadge } from '../shared';
import { useLessonManagement, useLessonOperations } from '../../hooks';
import { formatLessonDuration, sortLessonsByOrder } from '../../lib';
import type { LessonSummary } from '../../types';

export function LessonsListPage() {
  const { courseId } = useParams<{ courseId: string }>();
  const courseIdNum = courseId ? parseInt(courseId, 10) : null;
  const [reordering, setReordering] = useState<number | null>(null);

  const {
    lessons,
    isLoading,
    error,
    showPublishedOnly,
    refreshLessons,
    togglePublishedFilter,
  } = useLessonManagement(courseIdNum);

  const {
    publishLesson,
    unpublishLesson,
    reorderLesson,
  } = useLessonOperations();

  const handleLessonCreated = () => {
    refreshLessons();
  };

  const handleLessonUpdated = () => {
    refreshLessons();
  };

  const handleLessonDeleted = () => {
    refreshLessons();
  };

  const sortedLessons = sortLessonsByOrder(lessons as any[]);

  const handleTogglePublish = async (lesson: LessonSummary) => {
    if (lesson.isPublished) {
      await unpublishLesson(lesson.id);
    } else {
      await publishLesson(lesson.id);
    }
    refreshLessons();
  };

  const handleMoveUp = async (lesson: LessonSummary, index: number) => {
    if (index === 0) return;
    const previousLesson = sortedLessons[index - 1];
    setReordering(lesson.id);
    try {
      await reorderLesson(lesson.id, previousLesson.orderIndex);
      await reorderLesson(previousLesson.id, lesson.orderIndex);
      refreshLessons();
    } catch (error) {
      console.error('Failed to reorder lesson:', error);
    } finally {
      setReordering(null);
    }
  };

  const handleMoveDown = async (lesson: LessonSummary, index: number) => {
    if (index === sortedLessons.length - 1) return;
    const nextLesson = sortedLessons[index + 1];
    setReordering(lesson.id);
    try {
      await reorderLesson(lesson.id, nextLesson.orderIndex);
      await reorderLesson(nextLesson.id, lesson.orderIndex);
      refreshLessons();
    } catch (error) {
      console.error('Failed to reorder lesson:', error);
    } finally {
      setReordering(null);
    }
  };

  if (!courseIdNum) {
    return (
      <div className="p-6">
        <Card>
          <CardContent className="p-8 text-center">
            <BookOpen className="h-12 w-12 mx-auto mb-4 text-gray-400" />
            <h3 className="text-lg font-semibold mb-2">Course ID Required</h3>
            <p className="text-gray-600 mb-4">
              Please navigate to a course to view its lessons.
            </p>
            <Link to="/courses">
              <Button>Go to Courses</Button>
            </Link>
          </CardContent>
        </Card>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-gradient-to-b from-gray-50/50 to-white dark:from-gray-950/50 dark:to-gray-900">
      <div className="p-3 sm:p-4 lg:p-6 space-y-3 sm:space-y-4">
        {/* Header - Compact */}
        <div className="space-y-2">
          <div className="flex items-center justify-between gap-3 flex-wrap">
            <Link to={courseIdNum ? `/courses/${courseIdNum}` : '/courses'}>
              <Button 
                variant="ghost" 
                size="sm" 
                className="h-8 px-2 gap-1.5 -ml-2 group hover:bg-accent/50 transition-colors"
              >
                <ArrowLeft className="h-3.5 w-3.5 group-hover:-translate-x-0.5 transition-transform" />
                <span className="hidden sm:inline text-xs">Back</span>
              </Button>
            </Link>
            
            <div className="flex items-center gap-2">
              <CreateLessonDialog 
                courseId={courseIdNum}
                onLessonCreated={handleLessonCreated}
              />
            </div>
          </div>

          <div className="flex items-center justify-between gap-3 flex-wrap">
            <div>
              <h1 className="text-xl sm:text-2xl font-bold">Course Lessons</h1>
              <p className="text-xs sm:text-sm text-muted-foreground mt-0.5">
                Manage and organize lessons
              </p>
            </div>
            
            <Badge variant="secondary" className="text-xs">
              {sortedLessons.length} {sortedLessons.length === 1 ? 'Lesson' : 'Lessons'}
            </Badge>
          </div>
        </div>

        {/* Filters - Compact */}
        <div className="flex items-center justify-between gap-3 p-2.5 bg-card border rounded-lg">
          <div className="flex items-center gap-2">
            <Switch
              checked={showPublishedOnly}
              onCheckedChange={togglePublishedFilter}
              id="published-filter"
              className="scale-90"
            />
            <label 
              htmlFor="published-filter" 
              className="text-xs font-medium cursor-pointer text-muted-foreground"
            >
              Published only
            </label>
          </div>
          {error && (
            <span className="text-xs text-destructive">{error}</span>
          )}
        </div>

        {/* Loading State */}
        {isLoading ? (
          <LessonListSkeleton />
        ) : sortedLessons.length === 0 ? (
          <Card>
            <CardContent className="p-8 text-center">
              <div className="inline-flex items-center justify-center w-16 h-16 rounded-full bg-muted mb-4">
                <BookOpen className="h-8 w-8 text-muted-foreground" />
              </div>
              <h3 className="text-lg font-semibold mb-2">No Lessons Found</h3>
              <p className="text-sm text-muted-foreground mb-4">
                Get started by creating your first lesson for this course.
              </p>
              <CreateLessonDialog 
                courseId={courseIdNum}
                onLessonCreated={handleLessonCreated}
              />
            </CardContent>
          </Card>
        ) : (
          /* Compact Lessons List */
          <div className="space-y-2">
            {sortedLessons.map((lesson, index) => (
              <Card 
                key={lesson.id} 
                className="group hover:shadow-md transition-all duration-200 border-l-4 border-l-transparent hover:border-l-primary"
              >
                <CardContent className="p-3 sm:p-4">
                  <div className="flex items-start gap-3">
                    {/* Order Controls */}
                    <div className="flex flex-col gap-1 shrink-0">
                      <Button
                        variant="ghost"
                        size="icon"
                        className="h-6 w-6 p-0 opacity-0 group-hover:opacity-100 transition-opacity"
                        onClick={() => handleMoveUp(lesson, index)}
                        disabled={index === 0 || reordering === lesson.id}
                        title="Move up"
                      >
                        <ChevronUp className="h-3.5 w-3.5" />
                      </Button>
                      <div className="flex items-center justify-center w-8 h-8 rounded-full bg-primary/10 text-primary font-bold text-xs shrink-0">
                        {lesson.orderIndex}
                      </div>
                      <Button
                        variant="ghost"
                        size="icon"
                        className="h-6 w-6 p-0 opacity-0 group-hover:opacity-100 transition-opacity"
                        onClick={() => handleMoveDown(lesson, index)}
                        disabled={index === sortedLessons.length - 1 || reordering === lesson.id}
                        title="Move down"
                      >
                        <ChevronDown className="h-3.5 w-3.5" />
                      </Button>
                    </div>

                    {/* Lesson Content */}
                    <Link 
                      to={`/lessons/${lesson.id}`}
                      className="flex-1 min-w-0 group/link"
                    >
                      <div className="space-y-1.5">
                        <div className="flex items-start justify-between gap-2">
                          <h3 className="font-semibold text-sm sm:text-base group-hover/link:text-primary transition-colors line-clamp-1">
                            {lesson.title}
                          </h3>
                          <div className="flex items-center gap-1.5 shrink-0">
                            <LessonStatusBadge isPublished={lesson.isPublished} size="sm" />
                            <LessonAccessBadge isFree={lesson.isFree} size="sm" />
                          </div>
                        </div>
                        {lesson.description && (
                          <p className="text-xs sm:text-sm text-muted-foreground line-clamp-2">
                            {lesson.description}
                          </p>
                        )}
                        <div className="flex items-center gap-3 text-xs text-muted-foreground">
                          <div className="flex items-center gap-1">
                            <Clock className="h-3 w-3" />
                            <span>{formatLessonDuration(lesson.duration)}</span>
                          </div>
                        </div>
                      </div>
                    </Link>

                    {/* Actions */}
                    <div className="flex items-center gap-1.5 shrink-0">
                      <Link to={`/lessons/${lesson.id}`}>
                        <Button 
                          variant="ghost" 
                          size="icon"
                          className="h-8 w-8 opacity-0 group-hover:opacity-100 transition-opacity"
                          title="View Lesson"
                        >
                          <Play className="h-4 w-4" />
                        </Button>
                      </Link>
                      <Switch
                        checked={lesson.isPublished}
                        onCheckedChange={() => handleTogglePublish(lesson)}
                        className="scale-90"
                        title={lesson.isPublished ? 'Unpublish' : 'Publish'}
                      />
                      <EditLessonDialog 
                        lesson={lesson as any}
                        onLessonUpdated={handleLessonUpdated}
                      />
                      <DeleteLessonDialog 
                        lesson={lesson as any}
                        onLessonDeleted={handleLessonDeleted}
                      />
                    </div>
                  </div>
                </CardContent>
              </Card>
            ))}
          </div>
        )}
      </div>
    </div>
  );
}

