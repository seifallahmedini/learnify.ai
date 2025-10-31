import { useParams, Link } from 'react-router-dom';
import { 
  ArrowLeft, 
  Edit, 
  Trash2, 
  Users, 
  Clock, 
  Star, 
  BookOpen,
  Globe,
  Calendar,
  DollarSign,
  Award,
  Play,
  FileText,
  GraduationCap,
  BarChart3,
  TrendingUp,
  MoreVertical,
  HelpCircle
} from 'lucide-react';
import { Button } from '@/shared/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '@/shared/components/ui/card';
import { Badge } from '@/shared/components/ui/badge';
import { Separator } from '@/shared/components/ui/separator';
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/shared/components/ui/tabs';
import { Avatar, AvatarFallback, AvatarImage } from '@/shared/components/ui/avatar';
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuLabel,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from '@/shared/components/ui/dropdown-menu';
import { useCourseDetails, useCourseUtils } from '../../hooks';
import { 
  CourseLevelBadge, 
  CourseStatusBadge, 
  CourseFeatureBadge,
  CourseRating,
  CourseDetailsSkeleton 
} from '../shared';
import { useLessonManagement } from '@/features/lessons/hooks';
import { CreateLessonDialog } from '@/features/lessons/components/dialogs';
import { LessonListSkeleton, LessonStatusBadge, LessonAccessBadge } from '@/features/lessons/components/shared';
import { formatLessonDuration, sortLessonsByOrder } from '@/features/lessons/lib';

export function CourseDetailsPage() {
  const { courseId } = useParams<{ courseId: string }>();
  const courseIdNum = courseId ? parseInt(courseId, 10) : null;
  const { course, isLoading, error } = useCourseDetails(courseIdNum);
  const utils = useCourseUtils();
  
  // Load lessons for the course
  const {
    lessons,
    isLoading: lessonsLoading,
    refreshLessons,
  } = useLessonManagement(courseIdNum);

  if (isLoading) {
    return (
      <div className="p-6">
        <CourseDetailsSkeleton />
      </div>
    );
  }

  if (error || !course) {
    return (
      <div className="p-6">
        <div className="flex items-center mb-6">
          <Link to="/courses">
            <Button variant="ghost" size="sm" className="mr-4">
              <ArrowLeft className="h-4 w-4 mr-2" />
              Back to Courses
            </Button>
          </Link>
        </div>
        <Card>
          <CardContent className="p-8 text-center">
            <BookOpen className="h-12 w-12 mx-auto mb-4 text-gray-400" />
            <h3 className="text-lg font-semibold mb-2">Course Not Found</h3>
            <p className="text-gray-600 mb-4">
              {error || 'The course you are looking for could not be found.'}
            </p>
            <Link to="/courses">
              <Button>Return to Courses</Button>
            </Link>
          </CardContent>
        </Card>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-gradient-to-b from-gray-50/50 to-white dark:from-gray-950/50 dark:to-gray-900">
      <div className="p-4 sm:p-6 lg:p-8 space-y-6 lg:space-y-8">
        {/* Header Section with Hero Style */}
        <div className="space-y-4">
          {/* Back Button */}
          <Link to="/courses">
            <Button 
              variant="ghost" 
              size="sm" 
              className="group -ml-2 hover:bg-accent/50 transition-colors"
            >
              <ArrowLeft className="h-4 w-4 mr-2 group-hover:-translate-x-1 transition-transform" />
              Back to Courses
            </Button>
          </Link>
          
          {/* Title Section */}
          <div className="flex flex-col gap-4">
            {/* Badges Row */}
            <div className="flex items-center justify-between gap-4 flex-wrap">
              <div className="flex items-center gap-2.5 sm:gap-3 flex-wrap">
                <CourseStatusBadge isPublished={course.isPublished} />
                <CourseLevelBadge level={course.level} />
                {course.isFeatured && <CourseFeatureBadge isFeatured={course.isFeatured} />}
              </div>
              
              {/* Action Buttons - Enhanced Design */}
              <div className="flex items-center gap-1.5 sm:gap-2 flex-shrink-0">
                <Button 
                  variant="outline" 
                  size="sm"
                  className="gap-1.5 sm:gap-2 h-9 px-3 sm:px-4 hover:bg-primary/5 hover:border-primary/30 hover:text-primary transition-all shadow-sm hover:shadow-md border-border/50"
                  title="Edit Course"
                >
                  <Edit className="h-4 w-4" />
                  <span className="hidden sm:inline">Edit</span>
                </Button>
                <Button 
                  variant="outline" 
                  size="sm"
                  className="gap-1.5 sm:gap-2 h-9 px-3 sm:px-4 text-destructive/70 hover:text-destructive hover:bg-destructive/10 hover:border-destructive/50 transition-all shadow-sm hover:shadow-md border-destructive/20 group"
                  title="Delete Course"
                >
                  <Trash2 className="h-4 w-4 group-hover:scale-110 transition-transform" />
                  <span className="hidden sm:inline">Delete</span>
                </Button>
              </div>
            </div>

            {/* Title Row */}
            <div className="flex items-start justify-between gap-4">
              <div className="flex-1 min-w-0">
                <div className="flex items-start gap-3">
                  <h1 className="text-3xl sm:text-4xl lg:text-5xl font-bold tracking-tight bg-gradient-to-r from-foreground to-foreground/70 bg-clip-text flex-1 break-words">
                    {course.title}
                  </h1>
                  
                  {/* More Actions Dropdown - Positioned next to title */}
                  <DropdownMenu>
                    <DropdownMenuTrigger asChild>
                      <Button 
                        variant="ghost"
                        size="icon"
                        className="h-9 w-9 shrink-0 hover:bg-accent transition-colors mt-1"
                        title="More Actions"
                      >
                        <MoreVertical className="h-4 w-4" />
                      </Button>
                    </DropdownMenuTrigger>
                    <DropdownMenuContent align="end" className="w-52">
                      <DropdownMenuLabel>Quick Actions</DropdownMenuLabel>
                      <DropdownMenuSeparator />
                      <DropdownMenuItem asChild>
                        <Link to={`/courses/${courseId}/lessons`} className="w-full">
                          <BookOpen className="mr-2 h-4 w-4" />
                          Manage Lessons
                        </Link>
                      </DropdownMenuItem>
                      <DropdownMenuItem asChild>
                        <Link to={`/courses/${courseId}/quizzes`} className="w-full">
                          <HelpCircle className="mr-2 h-4 w-4" />
                          Manage Quizzes
                        </Link>
                      </DropdownMenuItem>
                      <DropdownMenuItem className="cursor-pointer">
                        <Play className="mr-2 h-4 w-4" />
                        Preview Course
                      </DropdownMenuItem>
                      <DropdownMenuItem className="cursor-pointer">
                        <Users className="mr-2 h-4 w-4" />
                        Manage Students
                      </DropdownMenuItem>
                    </DropdownMenuContent>
                  </DropdownMenu>
                </div>
              </div>
            </div>
            
            {/* Description and Meta */}
            <div className="space-y-3">
              <p className="text-lg text-muted-foreground max-w-3xl leading-relaxed">
                {course.shortDescription}
              </p>
              <div className="flex items-center gap-4 sm:gap-6 pt-1 text-sm text-muted-foreground flex-wrap">
                <div className="flex items-center gap-1.5">
                  <Calendar className="h-4 w-4 shrink-0" />
                  <span>{new Date(course.createdAt).toLocaleDateString('en-US', { 
                    year: 'numeric', 
                    month: 'long', 
                    day: 'numeric' 
                  })}</span>
                </div>
                <div className="flex items-center gap-1.5">
                  <Globe className="h-4 w-4 shrink-0" />
                  <span>{course.language || 'English'}</span>
                </div>
                <div className="flex items-center gap-1.5">
                  <BookOpen className="h-4 w-4 shrink-0" />
                  <span>{lessons.length} {lessons.length === 1 ? 'Lesson' : 'Lessons'}</span>
                </div>
              </div>
            </div>
          </div>
        </div>

      {/* Course Overview Cards - Enhanced Design */}
      <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4 lg:gap-6">
        <Card className="group hover:shadow-md transition-all duration-300 border-l-4 border-l-blue-500 hover:border-l-blue-600">
          <CardContent className="p-5">
            <div className="flex items-start justify-between">
              <div className="flex-1">
                <p className="text-sm font-medium text-muted-foreground mb-1.5">Total Students</p>
                <p className="text-3xl font-bold text-foreground mb-2">{course.totalStudents}</p>
                {course.maxStudents && (
                  <div className="w-full bg-gray-200 dark:bg-gray-800 rounded-full h-2 mt-3">
                    <div 
                      className="bg-blue-500 h-2 rounded-full transition-all duration-500"
                      style={{ width: `${Math.min((course.totalStudents / course.maxStudents) * 100, 100)}%` }}
                    />
                  </div>
                )}
              </div>
              <div className="p-3 rounded-lg bg-blue-500/10 group-hover:bg-blue-500/20 transition-colors">
                <Users className="h-6 w-6 text-blue-600 dark:text-blue-400" />
              </div>
            </div>
          </CardContent>
        </Card>
        
        <Card className="group hover:shadow-md transition-all duration-300 border-l-4 border-l-green-500 hover:border-l-green-600">
          <CardContent className="p-5">
            <div className="flex items-start justify-between">
              <div className="flex-1">
                <p className="text-sm font-medium text-muted-foreground mb-1.5">Duration</p>
                <p className="text-3xl font-bold text-foreground">{utils.formatDuration(course.durationHours)}</p>
                <p className="text-xs text-muted-foreground mt-2">Total Course Time</p>
              </div>
              <div className="p-3 rounded-lg bg-green-500/10 group-hover:bg-green-500/20 transition-colors">
                <Clock className="h-6 w-6 text-green-600 dark:text-green-400" />
              </div>
            </div>
          </CardContent>
        </Card>
        
        <Card className="group hover:shadow-md transition-all duration-300 border-l-4 border-l-yellow-500 hover:border-l-yellow-600">
          <CardContent className="p-5">
            <div className="flex items-start justify-between">
              <div className="flex-1">
                <p className="text-sm font-medium text-muted-foreground mb-1.5">Rating</p>
                <div className="flex items-baseline gap-2 mb-2">
                  <span className="text-3xl font-bold text-foreground">
                    {course.averageRating?.toFixed(1) || 'N/A'}
                  </span>
                  {course.averageRating && (
                    <span className="text-sm text-muted-foreground">/ 5.0</span>
                  )}
                </div>
                <CourseRating rating={course.averageRating || 0} size="sm" />
              </div>
              <div className="p-3 rounded-lg bg-yellow-500/10 group-hover:bg-yellow-500/20 transition-colors">
                <Star className="h-6 w-6 text-yellow-600 dark:text-yellow-400 fill-yellow-600 dark:fill-yellow-400" />
              </div>
            </div>
          </CardContent>
        </Card>
        
        <Card className="group hover:shadow-md transition-all duration-300 border-l-4 border-l-purple-500 hover:border-l-purple-600">
          <CardContent className="p-5">
            <div className="flex items-start justify-between">
              <div className="flex-1">
                <p className="text-sm font-medium text-muted-foreground mb-1.5">Price</p>
                <p className="text-3xl font-bold text-foreground">{utils.formatPrice(course.price)}</p>
                {course.price === 0 && (
                  <Badge variant="secondary" className="mt-2 text-xs">Free Course</Badge>
                )}
              </div>
              <div className="p-3 rounded-lg bg-purple-500/10 group-hover:bg-purple-500/20 transition-colors">
                <DollarSign className="h-6 w-6 text-purple-600 dark:text-purple-400" />
              </div>
            </div>
          </CardContent>
        </Card>
      </div>

      {/* Main Content */}
      <div className="grid grid-cols-1 lg:grid-cols-3 gap-6 lg:gap-8">
        {/* Course Details */}
        <div className="lg:col-span-2 space-y-6">
          <Tabs defaultValue="overview" className="space-y-6">
            <TabsList className="grid w-full grid-cols-4 bg-muted/50 p-1">
              <TabsTrigger value="overview" className="data-[state=active]:bg-background data-[state=active]:shadow-sm">
                <FileText className="h-4 w-4 mr-2" />
                <span className="hidden sm:inline">Overview</span>
              </TabsTrigger>
              <TabsTrigger value="curriculum" className="data-[state=active]:bg-background data-[state=active]:shadow-sm">
                <BookOpen className="h-4 w-4 mr-2" />
                <span className="hidden sm:inline">Curriculum</span>
              </TabsTrigger>
              <TabsTrigger value="students" className="data-[state=active]:bg-background data-[state=active]:shadow-sm">
                <GraduationCap className="h-4 w-4 mr-2" />
                <span className="hidden sm:inline">Students</span>
              </TabsTrigger>
              <TabsTrigger value="analytics" className="data-[state=active]:bg-background data-[state=active]:shadow-sm">
                <BarChart3 className="h-4 w-4 mr-2" />
                <span className="hidden sm:inline">Analytics</span>
              </TabsTrigger>
            </TabsList>
            
            <TabsContent value="overview" className="space-y-6 mt-6">
              <Card>
                <CardHeader className="pb-3">
                  <CardTitle className="text-xl flex items-center gap-2">
                    <FileText className="h-5 w-5" />
                    Course Description
                  </CardTitle>
                </CardHeader>
                <CardContent className="space-y-6">
                  <div className="prose prose-sm max-w-none dark:prose-invert">
                    <p className="text-foreground leading-relaxed whitespace-pre-line">
                      {course.description}
                    </p>
                  </div>
                  
                  {course.learningObjectives && (
                    <div className="pt-4 border-t">
                      <h4 className="font-semibold text-lg mb-3 flex items-center gap-2">
                        <Award className="h-5 w-5 text-primary" />
                        Learning Objectives
                      </h4>
                      <div className="text-foreground leading-relaxed whitespace-pre-line">
                        {course.learningObjectives}
                      </div>
                    </div>
                  )}
                  
                  {course.prerequisites && (
                    <div className="pt-4 border-t">
                      <h4 className="font-semibold text-lg mb-3 flex items-center gap-2">
                        <TrendingUp className="h-5 w-5 text-primary" />
                        Prerequisites
                      </h4>
                      <p className="text-foreground leading-relaxed">{course.prerequisites}</p>
                    </div>
                  )}
                </CardContent>
              </Card>
            </TabsContent>
            
            <TabsContent value="curriculum" className="space-y-6 mt-6">
              <Card>
                <CardHeader className="pb-3">
                  <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4">
                    <CardTitle className="text-xl flex items-center gap-2">
                      <BookOpen className="h-5 w-5" />
                      Course Curriculum
                      {lessons.length > 0 && (
                        <Badge variant="secondary" className="ml-2">
                          {lessons.length} {lessons.length === 1 ? 'Lesson' : 'Lessons'}
                        </Badge>
                      )}
                    </CardTitle>
                    <div className="flex items-center gap-2 flex-wrap">
                      <Link to={`/courses/${courseId}/lessons`}>
                        <Button variant="outline" size="sm" className="hover:bg-accent transition-colors">
                          <BookOpen className="h-4 w-4 mr-2" />
                          Manage Lessons
                        </Button>
                      </Link>
                      {courseIdNum && (
                        <CreateLessonDialog 
                          courseId={courseIdNum}
                          onLessonCreated={() => {
                            refreshLessons();
                          }}
                        />
                      )}
                    </div>
                  </div>
                </CardHeader>
                <CardContent>
                  {lessonsLoading ? (
                    <LessonListSkeleton count={3} />
                  ) : lessons.length === 0 ? (
                    <div className="text-center py-12">
                      <div className="inline-flex items-center justify-center w-16 h-16 rounded-full bg-muted mb-4">
                        <BookOpen className="h-8 w-8 text-muted-foreground" />
                      </div>
                      <h3 className="text-lg font-semibold mb-2">No lessons added yet</h3>
                      <p className="text-sm text-muted-foreground mb-6 max-w-sm mx-auto">
                        Get started by creating your first lesson for this course.
                      </p>
                      {courseIdNum && (
                        <CreateLessonDialog 
                          courseId={courseIdNum}
                          onLessonCreated={() => {
                            refreshLessons();
                          }}
                        />
                      )}
                    </div>
                  ) : (
                    <div className="space-y-3">
                      {sortLessonsByOrder(lessons as any[]).map((lesson, index) => (
                        <div
                          key={lesson.id}
                          className="group flex items-center justify-between p-4 border rounded-xl hover:border-primary/50 hover:shadow-md bg-card transition-all duration-200 cursor-pointer"
                        >
                          <Link 
                            to={`/lessons/${lesson.id}`}
                            className="flex items-center gap-4 flex-1 min-w-0"
                          >
                            <div className="flex-shrink-0 w-10 h-10 rounded-full bg-primary/10 text-primary flex items-center justify-center font-bold text-sm group-hover:bg-primary/20 transition-colors">
                              {index + 1}
                            </div>
                            <div className="flex-1 min-w-0">
                              <h4 className="font-semibold text-base group-hover:text-primary transition-colors line-clamp-1">
                                {lesson.title}
                              </h4>
                              {lesson.description && (
                                <p className="text-sm text-muted-foreground mt-1 line-clamp-2">
                                  {lesson.description}
                                </p>
                              )}
                              <div className="flex items-center gap-3 mt-3 flex-wrap">
                                <LessonStatusBadge isPublished={lesson.isPublished} size="sm" />
                                <LessonAccessBadge isFree={lesson.isFree} size="sm" />
                                <div className="flex items-center gap-1 text-xs text-muted-foreground">
                                  <Clock className="h-3 w-3" />
                                  {formatLessonDuration(lesson.duration)}
                                </div>
                              </div>
                            </div>
                          </Link>
                          <Link 
                            to={`/lessons/${lesson.id}`}
                            className="flex-shrink-0 ml-2"
                            onClick={(e) => e.stopPropagation()}
                          >
                            <Button 
                              variant="ghost" 
                              size="sm"
                              className="opacity-0 group-hover:opacity-100 transition-opacity"
                            >
                              <Play className="h-4 w-4" />
                              <span className="sr-only">View lesson</span>
                            </Button>
                          </Link>
                        </div>
                      ))}
                      <div className="pt-4 border-t">
                        <Link to={`/courses/${courseId}/lessons`}>
                          <Button variant="outline" className="w-full hover:bg-accent transition-colors">
                            View All Lessons ({lessons.length})
                            <ArrowLeft className="h-4 w-4 ml-2 rotate-180" />
                          </Button>
                        </Link>
                      </div>
                    </div>
                  )}
                </CardContent>
              </Card>
            </TabsContent>
            
            <TabsContent value="students" className="space-y-6 mt-6">
              <Card>
                <CardHeader className="pb-3">
                  <CardTitle className="text-xl flex items-center gap-2">
                    <GraduationCap className="h-5 w-5" />
                    Enrolled Students
                  </CardTitle>
                </CardHeader>
                <CardContent>
                  <div className="text-center py-12">
                    <div className="inline-flex items-center justify-center w-16 h-16 rounded-full bg-muted mb-4">
                      <Users className="h-8 w-8 text-muted-foreground" />
                    </div>
                    <h3 className="text-lg font-semibold mb-2">No students enrolled yet</h3>
                    <p className="text-sm text-muted-foreground max-w-sm mx-auto">
                      Student enrollment information will be displayed here once students start enrolling in this course.
                    </p>
                  </div>
                </CardContent>
              </Card>
            </TabsContent>
            
            <TabsContent value="analytics" className="space-y-6 mt-6">
              <Card>
                <CardHeader className="pb-3">
                  <CardTitle className="text-xl flex items-center gap-2">
                    <BarChart3 className="h-5 w-5" />
                    Course Analytics
                  </CardTitle>
                </CardHeader>
                <CardContent>
                  <div className="text-center py-12">
                    <div className="inline-flex items-center justify-center w-16 h-16 rounded-full bg-muted mb-4">
                      <Award className="h-8 w-8 text-muted-foreground" />
                    </div>
                    <h3 className="text-lg font-semibold mb-2">Analytics coming soon</h3>
                    <p className="text-sm text-muted-foreground max-w-sm mx-auto">
                      Detailed course analytics and performance metrics will be available here.
                    </p>
                  </div>
                </CardContent>
              </Card>
            </TabsContent>
          </Tabs>
        </div>

        {/* Sidebar */}
        <div className="space-y-6">
          {/* Course Info */}
          <Card>
            <CardHeader className="pb-3">
              <CardTitle className="text-lg">Course Information</CardTitle>
            </CardHeader>
            <CardContent className="space-y-4">
              <div className="flex items-center justify-between py-2">
                <span className="text-sm font-medium text-muted-foreground">Status</span>
                <CourseStatusBadge isPublished={course.isPublished} />
              </div>
              
              <div className="flex items-center justify-between py-2">
                <span className="text-sm font-medium text-muted-foreground">Level</span>
                <CourseLevelBadge level={course.level} />
              </div>
              
              <div className="flex items-center justify-between py-2">
                <span className="text-sm font-medium text-muted-foreground">Featured</span>
                <CourseFeatureBadge isFeatured={course.isFeatured} />
              </div>
              
              <Separator className="my-4" />
              
              <div className="flex items-center justify-between py-2">
                <span className="text-sm font-medium text-muted-foreground">Category</span>
                <Badge variant="outline" className="font-normal">
                  {course.categoryName}
                </Badge>
              </div>
              
              <div className="flex items-center justify-between py-2">
                <span className="text-sm font-medium text-muted-foreground">Language</span>
                <div className="flex items-center gap-1.5">
                  <Globe className="h-4 w-4 text-muted-foreground" />
                  <span className="text-sm font-medium">{course.language || 'English'}</span>
                </div>
              </div>
              
              <div className="flex items-center justify-between py-2">
                <span className="text-sm font-medium text-muted-foreground">Created</span>
                <div className="flex items-center gap-1.5">
                  <Calendar className="h-4 w-4 text-muted-foreground" />
                  <span className="text-sm font-medium">
                    {new Date(course.createdAt).toLocaleDateString('en-US', { 
                      month: 'short', 
                      day: 'numeric',
                      year: 'numeric'
                    })}
                  </span>
                </div>
              </div>
              
              {course.maxStudents && (
                <div className="flex items-center justify-between py-2">
                  <span className="text-sm font-medium text-muted-foreground">Capacity</span>
                  <span className="text-sm font-medium">
                    {course.totalStudents}/{course.maxStudents}
                  </span>
                </div>
              )}
            </CardContent>
          </Card>

          {/* Instructor Info */}
          <Card>
            <CardHeader className="pb-3">
              <CardTitle className="text-lg">Instructor</CardTitle>
            </CardHeader>
            <CardContent>
              <div className="flex items-center gap-3 p-3 rounded-lg bg-muted/50 hover:bg-muted transition-colors">
                <Avatar className="h-12 w-12 border-2 border-background">
                  <AvatarImage src={`https://api.dicebear.com/7.x/initials/svg?seed=${course.instructorName}`} />
                  <AvatarFallback className="bg-primary/10 text-primary font-semibold">
                    {utils.getCourseInitials(course.instructorName)}
                  </AvatarFallback>
                </Avatar>
                <div className="flex-1 min-w-0">
                  <p className="font-semibold text-sm truncate">{course.instructorName}</p>
                  <p className="text-xs text-muted-foreground">Course Instructor</p>
                </div>
              </div>
            </CardContent>
          </Card>
        </div>
      </div>
      </div>
    </div>
  );
}