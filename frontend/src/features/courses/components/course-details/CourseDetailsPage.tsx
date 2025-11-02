import { useParams, Link, useNavigate } from 'react-router-dom';
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
  CourseDetailsSkeleton 
} from '../shared';
import { useLessonManagement } from '@/features/lessons/hooks';
import { CreateLessonDialog } from '@/features/lessons/components/dialogs';
import { LessonListSkeleton, LessonStatusBadge, LessonAccessBadge } from '@/features/lessons/components/shared';
import { formatLessonDuration, sortLessonsByOrder } from '@/features/lessons/lib';
import { useQuizManagement } from '@/features/quizzes/hooks';
import { CreateQuizDialog } from '@/features/quizzes/components/dialogs';

export function CourseDetailsPage() {
  const { courseId } = useParams<{ courseId: string }>();
  const navigate = useNavigate();
  const courseIdNum = courseId ? parseInt(courseId, 10) : null;
  const { course, isLoading, error } = useCourseDetails(courseIdNum);
  const utils = useCourseUtils();
  
  // Load lessons for the course
  const {
    lessons,
    isLoading: lessonsLoading,
    refreshLessons,
  } = useLessonManagement(courseIdNum);

  // Load quizzes for the course
  const {
    quizzes,
    refreshQuizzes,
  } = useQuizManagement({ courseId: courseIdNum });

  const handleQuizCreated = () => {
    refreshQuizzes();
  };

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
      <div className="p-3 sm:p-4 lg:p-6 space-y-3 sm:space-y-4">
        {/* Header Section - More Compact */}
        <div className="space-y-2">
          {/* Back Button */}
          <Button 
            variant="ghost" 
            size="sm" 
            className="group -ml-2 hover:bg-accent/50 transition-colors"
            onClick={() => navigate(-1)}
          >
            <ArrowLeft className="h-4 w-4 mr-2 group-hover:-translate-x-1 transition-transform" />
            Back
          </Button>
          
          {/* Title Section */}
          <div className="flex flex-col gap-2">
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
                  <h1 className="text-xl sm:text-2xl font-bold tracking-tight flex-1 break-words">
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
            <div className="space-y-2">
              <p className="text-sm text-muted-foreground max-w-3xl leading-relaxed">
                {course.shortDescription}
              </p>
              <div className="flex items-center gap-3 sm:gap-4 pt-1 text-xs sm:text-sm text-muted-foreground flex-wrap">
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
                <div className="flex items-center gap-1.5">
                  <HelpCircle className="h-4 w-4 shrink-0" />
                  <span>{quizzes.length} {quizzes.length === 1 ? 'Quiz' : 'Quizzes'}</span>
                </div>
              </div>
            </div>
          </div>
        </div>

      {/* Course Statistics Cards */}
      <div className="grid grid-cols-2 lg:grid-cols-4 gap-2 sm:gap-3">
        <Card className="hover:shadow-md transition-all duration-200 border-l-4 border-l-blue-500">
          <CardContent className="p-3">
            <div className="flex items-center gap-2.5">
              <div className="p-2 bg-blue-500/10 rounded-lg shrink-0">
                <Users className="h-4 w-4 text-blue-600 dark:text-blue-400" />
              </div>
              <div className="flex-1 min-w-0">
                <p className="text-xs font-medium text-muted-foreground mb-0.5">Students</p>
                <p className="text-lg sm:text-xl font-bold">{course.totalStudents || 0}</p>
                {course.maxStudents && (
                  <p className="text-xs text-muted-foreground mt-0.5">
                    of {course.maxStudents} capacity
                  </p>
                )}
              </div>
            </div>
          </CardContent>
        </Card>
        
        <Card className="hover:shadow-md transition-all duration-200 border-l-4 border-l-green-500">
          <CardContent className="p-3">
            <div className="flex items-center gap-2.5">
              <div className="p-2 bg-green-500/10 rounded-lg shrink-0">
                <Clock className="h-4 w-4 text-green-600 dark:text-green-400" />
              </div>
              <div className="flex-1 min-w-0">
                <p className="text-xs font-medium text-muted-foreground mb-0.5">Duration</p>
                <p className="text-lg sm:text-xl font-bold">{utils.formatDuration(course.durationHours)}</p>
                <p className="text-xs text-muted-foreground mt-0.5">
                  {course.durationHours} {course.durationHours === 1 ? 'hour' : 'hours'}
                </p>
              </div>
            </div>
          </CardContent>
        </Card>
        
        <Card className="hover:shadow-md transition-all duration-200 border-l-4 border-l-yellow-500">
          <CardContent className="p-3">
            <div className="flex items-center gap-2.5">
              <div className="p-2 bg-yellow-500/10 rounded-lg shrink-0">
                <Star className="h-4 w-4 text-yellow-600 dark:text-yellow-400 fill-yellow-600 dark:fill-yellow-400" />
              </div>
              <div className="flex-1 min-w-0">
                <p className="text-xs font-medium text-muted-foreground mb-0.5">Rating</p>
                <div className="flex items-baseline gap-1">
                  <span className="text-lg sm:text-xl font-bold">
                    {course.averageRating?.toFixed(1) || 'N/A'}
                  </span>
                  {course.averageRating && (
                    <span className="text-xs text-muted-foreground">/5</span>
                  )}
                </div>
                {course.totalReviews > 0 && (
                  <p className="text-xs text-muted-foreground mt-0.5">
                    {course.totalReviews} {course.totalReviews === 1 ? 'review' : 'reviews'}
                  </p>
                )}
              </div>
            </div>
          </CardContent>
        </Card>
        
        <Card className="hover:shadow-md transition-all duration-200 border-l-4 border-l-purple-500">
          <CardContent className="p-3">
            <div className="flex items-center gap-2.5">
              <div className="p-2 bg-purple-500/10 rounded-lg shrink-0">
                <DollarSign className="h-4 w-4 text-purple-600 dark:text-purple-400" />
              </div>
              <div className="flex-1 min-w-0">
                <p className="text-xs font-medium text-muted-foreground mb-0.5">Price</p>
                <div className="space-y-0.5">
                  {course.discountPrice && course.discountPrice < course.price ? (
                    <>
                      <div className="flex items-baseline gap-1.5">
                        <p className="text-lg sm:text-xl font-bold">{utils.formatPrice(course.discountPrice)}</p>
                        <span className="text-xs line-through text-muted-foreground">
                          {utils.formatPrice(course.price)}
                        </span>
                      </div>
                      <p className="text-xs text-green-600 dark:text-green-400 font-medium">
                        {Math.round(((course.price - course.discountPrice) / course.price) * 100)}% off
                      </p>
                    </>
                  ) : (
                    <p className="text-lg sm:text-xl font-bold">{utils.formatPrice(course.price)}</p>
                  )}
                </div>
              </div>
            </div>
          </CardContent>
        </Card>
      </div>

      {/* Main Content */}
      <div className="grid grid-cols-1 lg:grid-cols-3 gap-3 sm:gap-4">
        {/* Course Details */}
        <div className="lg:col-span-2 space-y-3">
          <Tabs defaultValue="overview" className="space-y-3">
            <TabsList className="grid w-full grid-cols-4 h-11 bg-muted/50 p-1 rounded-lg border border-border/50">
              <TabsTrigger 
                value="overview" 
                className="text-sm font-medium data-[state=active]:bg-background data-[state=active]:text-primary data-[state=active]:shadow-sm transition-all duration-200 rounded-md"
              >
                <FileText className="h-4 w-4 mr-2" />
                <span className="hidden sm:inline">Overview</span>
              </TabsTrigger>
              <TabsTrigger 
                value="curriculum" 
                className="text-sm font-medium data-[state=active]:bg-background data-[state=active]:text-primary data-[state=active]:shadow-sm transition-all duration-200 rounded-md relative"
              >
                <BookOpen className="h-4 w-4 mr-2" />
                <span className="hidden sm:inline">Curriculum</span>
                {(lessons.length > 0 || quizzes.length > 0) && (
                  <Badge variant="secondary" className="ml-1.5 h-5 px-1.5 text-xs font-semibold">
                    {lessons.length + quizzes.length}
                  </Badge>
                )}
              </TabsTrigger>
              <TabsTrigger 
                value="students" 
                className="text-sm font-medium data-[state=active]:bg-background data-[state=active]:text-primary data-[state=active]:shadow-sm transition-all duration-200 rounded-md"
              >
                <GraduationCap className="h-4 w-4 mr-2" />
                <span className="hidden sm:inline">Students</span>
              </TabsTrigger>
              <TabsTrigger 
                value="analytics" 
                className="text-sm font-medium data-[state=active]:bg-background data-[state=active]:text-primary data-[state=active]:shadow-sm transition-all duration-200 rounded-md"
              >
                <BarChart3 className="h-4 w-4 mr-2" />
                <span className="hidden sm:inline">Analytics</span>
              </TabsTrigger>
            </TabsList>
            
            <TabsContent value="overview" className="space-y-3 mt-3">
              <Card>
                <CardHeader className="pb-2 pt-3 px-4">
                  <CardTitle className="text-base flex items-center gap-2">
                    <FileText className="h-4 w-4" />
                    Course Description
                  </CardTitle>
                </CardHeader>
                <CardContent className="px-4 pb-4 space-y-3">
                  <div className="prose prose-sm max-w-none dark:prose-invert">
                    <p className="text-sm text-foreground leading-relaxed whitespace-pre-line">
                      {course.description}
                    </p>
                  </div>
                  
                  {course.learningObjectives && (
                    <div className="pt-3 border-t">
                      <h4 className="font-semibold text-sm mb-2 flex items-center gap-2">
                        <Award className="h-4 w-4 text-primary" />
                        Learning Objectives
                      </h4>
                      <div className="text-sm text-foreground leading-relaxed whitespace-pre-line">
                        {course.learningObjectives}
                      </div>
                    </div>
                  )}
                  
                  {course.prerequisites && (
                    <div className="pt-3 border-t">
                      <h4 className="font-semibold text-sm mb-2 flex items-center gap-2">
                        <TrendingUp className="h-4 w-4 text-primary" />
                        Prerequisites
                      </h4>
                      <p className="text-sm text-foreground leading-relaxed">{course.prerequisites}</p>
                    </div>
                  )}
                </CardContent>
              </Card>
            </TabsContent>
            
            <TabsContent value="curriculum" className="space-y-3 mt-3">
              {/* Lessons Section */}
              <Card>
                <CardHeader className="pb-2 pt-3 px-4">
                  <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-3">
                    <CardTitle className="text-base flex items-center gap-2">
                      <BookOpen className="h-4 w-4" />
                      Course Curriculum
                      {lessons.length > 0 && (
                        <Badge variant="secondary" className="text-xs">
                          {lessons.length}
                        </Badge>
                      )}
                    </CardTitle>
                    <div className="flex items-center gap-2 flex-wrap">
                      <Link to={`/courses/${courseId}/lessons`}>
                        <Button variant="outline" size="sm" className="h-8 text-xs hover:bg-accent">
                          <BookOpen className="h-3.5 w-3.5 mr-1.5" />
                          Manage
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
                <CardContent className="px-4 pb-4">
                  {lessonsLoading ? (
                    <LessonListSkeleton count={3} />
                  ) : lessons.length === 0 ? (
                    <div className="text-center py-8">
                      <div className="inline-flex items-center justify-center w-12 h-12 rounded-full bg-muted mb-3">
                        <BookOpen className="h-6 w-6 text-muted-foreground" />
                      </div>
                      <h3 className="text-sm font-semibold mb-1">No lessons added yet</h3>
                      <p className="text-xs text-muted-foreground mb-4">
                        Get started by creating your first lesson.
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
                    <div className="space-y-2">
                      {sortLessonsByOrder(lessons as any[]).slice(0, 5).map((lesson, index) => (
                        <Link
                          key={lesson.id}
                          to={`/lessons/${lesson.id}`}
                          className="group flex items-center justify-between p-2.5 border rounded-lg hover:border-primary/50 hover:bg-accent/50 bg-card transition-all cursor-pointer"
                        >
                          <div className="flex items-center gap-3 flex-1 min-w-0">
                            <div className="flex-shrink-0 w-8 h-8 rounded-full bg-primary/10 text-primary flex items-center justify-center font-bold text-xs group-hover:bg-primary/20 transition-colors">
                              {index + 1}
                            </div>
                            <div className="flex-1 min-w-0">
                              <h4 className="font-semibold text-sm group-hover:text-primary transition-colors truncate">
                                {lesson.title}
                              </h4>
                              <div className="flex items-center gap-2 mt-1 flex-wrap">
                                <LessonStatusBadge isPublished={lesson.isPublished} size="sm" />
                                <LessonAccessBadge isFree={lesson.isFree} size="sm" />
                                <div className="flex items-center gap-1 text-xs text-muted-foreground">
                                  <Clock className="h-3 w-3" />
                                  {formatLessonDuration(lesson.duration)}
                                </div>
                              </div>
                            </div>
                          </div>
                          <Play className="h-4 w-4 text-muted-foreground opacity-0 group-hover:opacity-100 transition-opacity shrink-0" />
                        </Link>
                      ))}
                      {lessons.length > 5 && (
                        <Link to={`/courses/${courseId}/lessons`}>
                          <Button variant="outline" className="w-full h-8 text-xs mt-2">
                            View All Lessons ({lessons.length})
                          </Button>
                        </Link>
                      )}
                    </div>
                  )}
                </CardContent>
              </Card>

              {/* Quizzes Section */}
              <Card>
                <CardHeader className="pb-2 pt-3 px-4">
                  <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-3">
                    <CardTitle className="text-base flex items-center gap-2">
                      <HelpCircle className="h-4 w-4" />
                      Quizzes
                      {quizzes.length > 0 && (
                        <Badge variant="secondary" className="text-xs">
                          {quizzes.length}
                        </Badge>
                      )}
                    </CardTitle>
                    <div className="flex items-center gap-2 flex-wrap">
                      <Link to={`/courses/${courseId}/quizzes`}>
                        <Button variant="outline" size="sm" className="h-8 text-xs hover:bg-accent">
                          <HelpCircle className="h-3.5 w-3.5 mr-1.5" />
                          Manage
                        </Button>
                      </Link>
                      {courseIdNum && (
                        <CreateQuizDialog 
                          courseId={courseIdNum}
                          onQuizCreated={handleQuizCreated}
                        />
                      )}
                    </div>
                  </div>
                </CardHeader>
                <CardContent className="px-4 pb-4">
                  {quizzes.length === 0 ? (
                    <div className="text-center py-8">
                      <div className="inline-flex items-center justify-center w-12 h-12 rounded-full bg-muted mb-3">
                        <HelpCircle className="h-6 w-6 text-muted-foreground" />
                      </div>
                      <h3 className="text-sm font-semibold mb-1">No quizzes added yet</h3>
                      <p className="text-xs text-muted-foreground mb-4">
                        Get started by creating your first quiz.
                      </p>
                      {courseIdNum && (
                        <CreateQuizDialog 
                          courseId={courseIdNum}
                          onQuizCreated={handleQuizCreated}
                        />
                      )}
                    </div>
                  ) : (
                    <div className="space-y-2">
                      {quizzes.slice(0, 5).map((quiz, index) => (
                        <Link
                          key={quiz.id}
                          to={`/quizzes/${quiz.id}`}
                          className="group flex items-center justify-between p-2.5 border rounded-lg hover:border-primary/50 hover:bg-accent/50 bg-card transition-all cursor-pointer"
                        >
                          <div className="flex items-center gap-3 flex-1 min-w-0">
                            <div className="flex-shrink-0 w-8 h-8 rounded-full bg-primary/10 text-primary flex items-center justify-center font-bold text-xs group-hover:bg-primary/20 transition-colors">
                              {index + 1}
                            </div>
                            <div className="flex-1 min-w-0">
                              <h4 className="font-semibold text-sm group-hover:text-primary transition-colors truncate">
                                {quiz.title}
                              </h4>
                              <div className="flex items-center gap-2 mt-1 flex-wrap">
                                <Badge variant={quiz.isActive ? "default" : "secondary"} className="text-xs">
                                  {quiz.isActive ? "Active" : "Inactive"}
                                </Badge>
                                {quiz.questionCount > 0 && (
                                  <span className="text-xs text-muted-foreground">
                                    {quiz.questionCount} {quiz.questionCount === 1 ? 'question' : 'questions'}
                                  </span>
                                )}
                              </div>
                            </div>
                          </div>
                          <Play className="h-4 w-4 text-muted-foreground opacity-0 group-hover:opacity-100 transition-opacity shrink-0" />
                        </Link>
                      ))}
                      {quizzes.length > 5 && (
                        <Link to={`/courses/${courseId}/quizzes`}>
                          <Button variant="outline" className="w-full h-8 text-xs mt-2">
                            View All Quizzes ({quizzes.length})
                          </Button>
                        </Link>
                      )}
                    </div>
                  )}
                </CardContent>
              </Card>
            </TabsContent>
            
            <TabsContent value="students" className="space-y-3 mt-3">
              <Card>
                <CardHeader className="pb-2 pt-3 px-4">
                  <CardTitle className="text-base flex items-center gap-2">
                    <GraduationCap className="h-4 w-4" />
                    Enrolled Students
                  </CardTitle>
                </CardHeader>
                <CardContent className="px-4 pb-4">
                  <div className="text-center py-8">
                    <div className="inline-flex items-center justify-center w-12 h-12 rounded-full bg-muted mb-3">
                      <Users className="h-6 w-6 text-muted-foreground" />
                    </div>
                    <h3 className="text-sm font-semibold mb-1">No students enrolled yet</h3>
                    <p className="text-xs text-muted-foreground">
                      Student enrollment information will be displayed here once students start enrolling.
                    </p>
                  </div>
                </CardContent>
              </Card>
            </TabsContent>
            
            <TabsContent value="analytics" className="space-y-3 mt-3">
              <Card>
                <CardHeader className="pb-2 pt-3 px-4">
                  <CardTitle className="text-base flex items-center gap-2">
                    <BarChart3 className="h-4 w-4" />
                    Course Analytics
                  </CardTitle>
                </CardHeader>
                <CardContent className="px-4 pb-4">
                  <div className="text-center py-8">
                    <div className="inline-flex items-center justify-center w-12 h-12 rounded-full bg-muted mb-3">
                      <Award className="h-6 w-6 text-muted-foreground" />
                    </div>
                    <h3 className="text-sm font-semibold mb-1">Analytics coming soon</h3>
                    <p className="text-xs text-muted-foreground">
                      Detailed course analytics and performance metrics will be available here.
                    </p>
                  </div>
                </CardContent>
              </Card>
            </TabsContent>
          </Tabs>
        </div>

        {/* Sidebar */}
        <div className="space-y-3">
          {/* Course Info */}
          <Card>
            <CardHeader className="pb-2 pt-3 px-3">
              <CardTitle className="text-sm">Course Information</CardTitle>
            </CardHeader>
            <CardContent className="px-3 pb-3 space-y-2">
              <div className="flex items-center justify-between py-1.5">
                <span className="text-xs font-medium text-muted-foreground">Status</span>
                <CourseStatusBadge isPublished={course.isPublished} />
              </div>
              
              <div className="flex items-center justify-between py-1.5">
                <span className="text-xs font-medium text-muted-foreground">Level</span>
                <CourseLevelBadge level={course.level} />
              </div>
              
              <div className="flex items-center justify-between py-1.5">
                <span className="text-xs font-medium text-muted-foreground">Featured</span>
                <CourseFeatureBadge isFeatured={course.isFeatured} />
              </div>
              
              <Separator className="my-2" />
              
              <div className="flex items-center justify-between py-1.5">
                <span className="text-xs font-medium text-muted-foreground">Category</span>
                <Badge variant="outline" className="text-xs font-normal">
                  {course.categoryName}
                </Badge>
              </div>
              
              <div className="flex items-center justify-between py-1.5">
                <span className="text-xs font-medium text-muted-foreground">Language</span>
                <div className="flex items-center gap-1">
                  <Globe className="h-3 w-3 text-muted-foreground" />
                  <span className="text-xs font-medium">{course.language || 'English'}</span>
                </div>
              </div>
              
              <div className="flex items-center justify-between py-1.5">
                <span className="text-xs font-medium text-muted-foreground">Created</span>
                <div className="flex items-center gap-1">
                  <Calendar className="h-3 w-3 text-muted-foreground" />
                  <span className="text-xs font-medium">
                    {new Date(course.createdAt).toLocaleDateString('en-US', { 
                      month: 'short', 
                      day: 'numeric',
                      year: 'numeric'
                    })}
                  </span>
                </div>
              </div>
              
              {course.maxStudents && (
                <div className="flex items-center justify-between py-1.5">
                  <span className="text-xs font-medium text-muted-foreground">Capacity</span>
                  <span className="text-xs font-medium">
                    {course.totalStudents}/{course.maxStudents}
                  </span>
                </div>
              )}
            </CardContent>
          </Card>

          {/* Instructor Info */}
          <Card>
            <CardHeader className="pb-2 pt-3 px-3">
              <CardTitle className="text-sm">Instructor</CardTitle>
            </CardHeader>
            <CardContent className="px-3 pb-3">
              <div className="flex items-center gap-2.5 p-2 rounded-lg bg-muted/50 hover:bg-muted transition-colors">
                <Avatar className="h-10 w-10 border-2 border-background">
                  <AvatarImage src={`https://api.dicebear.com/7.x/initials/svg?seed=${course.instructorName}`} />
                  <AvatarFallback className="bg-primary/10 text-primary font-semibold text-xs">
                    {utils.getCourseInitials(course.instructorName)}
                  </AvatarFallback>
                </Avatar>
                <div className="flex-1 min-w-0">
                  <p className="font-semibold text-xs truncate">{course.instructorName}</p>
                  <p className="text-xs text-muted-foreground">Instructor</p>
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