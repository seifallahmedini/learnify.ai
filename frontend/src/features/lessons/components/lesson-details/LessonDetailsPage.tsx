import { useParams, Link, useNavigate } from 'react-router-dom';
import ReactMarkdown from 'react-markdown';
import { 
  ArrowLeft, 
  Clock, 
  BookOpen,
  Play,
  ChevronLeft,
  ChevronRight,
  Target,
  Download,
  ExternalLink,
  Code,
  Link2
} from 'lucide-react';
import { Button } from '@/shared/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '@/shared/components/ui/card';
import { Badge } from '@/shared/components/ui/badge';
import { Separator } from '@/shared/components/ui/separator';
import { useLessonDetails, useLessonNavigation, useLessonOperations, useLessonManagement } from '../../hooks';
import { 
  LessonStatusBadge, 
  LessonAccessBadge, 
  LessonDetailsSkeleton 
} from '../shared';
import { EditLessonDialog, DeleteLessonDialog } from '../dialogs';
import { formatLessonDuration, sortLessonsByOrder } from '../../lib';
import type { LessonResource } from '../../types';

// Helper function to get video embed URL
const getVideoEmbedUrl = (url: string): string | null => {
  // YouTube
  const youtubeRegex = /(?:youtube\.com\/(?:[^\/]+\/.+\/|(?:v|e(?:mbed)?)\/|.*[?&]v=)|youtu\.be\/)([^"&?\/\s]{11})/;
  const youtubeMatch = url.match(youtubeRegex);
  if (youtubeMatch) {
    return `https://www.youtube.com/embed/${youtubeMatch[1]}`;
  }

  // Vimeo
  const vimeoRegex = /(?:vimeo\.com\/)(?:.*\/)?(\d+)/;
  const vimeoMatch = url.match(vimeoRegex);
  if (vimeoMatch) {
    return `https://player.vimeo.com/video/${vimeoMatch[1]}`;
  }

  // Direct video file
  const videoExtensions = ['.mp4', '.webm', '.ogg', '.mov', '.avi'];
  const isVideoFile = videoExtensions.some(ext => url.toLowerCase().includes(ext));
  if (isVideoFile) {
    return url;
  }

  return null;
};

// Helper to check if URL is embeddable
const isEmbeddableVideo = (url: string): boolean => {
  return getVideoEmbedUrl(url) !== null;
};

export function LessonDetailsPage() {
  const { lessonId } = useParams<{ lessonId: string }>();
  const lessonIdNum = lessonId ? parseInt(lessonId, 10) : null;
  const navigate = useNavigate();

  const { lesson, isLoading, error, loadLesson } = useLessonDetails(lessonIdNum);
  const { previousLesson, nextLesson } = useLessonNavigation(lessonIdNum);
  const { publishLesson, unpublishLesson } = useLessonOperations();
  
  // Load all lessons from the same course
  const { lessons: courseLessons, isLoading: lessonsLoading } = useLessonManagement(lesson?.courseId || null);

  const handleLessonUpdated = (updatedLesson: any) => {
    loadLesson(updatedLesson.id);
  };

  const handleLessonDeleted = () => {
    // Navigate back to course if available, otherwise to courses list
    if (lesson?.courseId) {
      navigate(`/courses/${lesson.courseId}`);
    } else {
      navigate('/courses');
    }
  };

  const handleTogglePublish = async () => {
    if (!lesson) return;
    
    if (lesson.isPublished) {
      await unpublishLesson(lesson.id);
    } else {
      await publishLesson(lesson.id);
    }
    loadLesson(lesson.id);
  };

  if (isLoading) {
    return (
      <div className="p-6">
        <LessonDetailsSkeleton />
      </div>
    );
  }

  if (error || !lesson) {
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
            <h3 className="text-lg font-semibold mb-2">Lesson Not Found</h3>
            <p className="text-gray-600 mb-4">
              {error || 'The lesson you are looking for could not be found.'}
            </p>
            <Link to="/courses">
              <Button>Return to Courses</Button>
            </Link>
          </CardContent>
        </Card>
      </div>
    );
  }

  const sortedLessons = sortLessonsByOrder(courseLessons as any[]);

  // Parse learning objectives (comma or newline separated)
  // Handle both string and undefined/null cases
  const learningObjectivesRaw = (lesson as any).learningObjectives;
  const learningObjectives = (learningObjectivesRaw && typeof learningObjectivesRaw === 'string' && learningObjectivesRaw.trim())
    ? learningObjectivesRaw.split(/[,\n]/).map(obj => obj.trim()).filter(obj => obj.length > 0)
    : [];

  // Parse resources (JSON string)
  // Handle both string and undefined/null cases
  let resources: LessonResource[] = [];
  const resourcesRaw = (lesson as any).resources;
  if (resourcesRaw && typeof resourcesRaw === 'string' && resourcesRaw.trim()) {
    try {
      const parsed = JSON.parse(resourcesRaw);
      if (Array.isArray(parsed)) {
        resources = parsed as LessonResource[];
      }
    } catch (e) {
      console.warn('Failed to parse lesson resources:', e);
      resources = [];
    }
  }

  // Helper to get resource icon
  const getResourceIcon = (type: string) => {
    switch (type) {
      case 'download':
        return <Download className="h-4 w-4" />;
      case 'external':
        return <ExternalLink className="h-4 w-4" />;
      case 'code':
        return <Code className="h-4 w-4" />;
      default:
        return <Link2 className="h-4 w-4" />;
    }
  };

  return (
    <div className="min-h-screen bg-gradient-to-b from-gray-50/50 to-white dark:from-gray-950/50 dark:to-gray-900">
      <div className="p-3 sm:p-4 lg:p-6 space-y-3 sm:space-y-4">
        {/* Header - Compact */}
        <div className="space-y-2">
          {/* Top Bar: Back + Actions */}
          <div className="flex items-center justify-between gap-3 flex-wrap">
            <Link to={lesson.courseId ? `/courses/${lesson.courseId}` : '/courses'}>
              <Button 
                variant="ghost" 
                size="sm" 
                className="h-8 px-2 gap-1.5 -ml-2 group hover:bg-accent/50 transition-colors"
              >
                <ArrowLeft className="h-3.5 w-3.5 group-hover:-translate-x-0.5 transition-transform" />
                <span className="hidden sm:inline text-xs">Back</span>
              </Button>
            </Link>
            
            <div className="flex items-center gap-1.5 flex-shrink-0">
              <Button 
                variant="outline" 
                size="sm"
                className="gap-1.5 h-8 px-2.5 text-xs hover:bg-primary/5 hover:border-primary/30 hover:text-primary transition-all"
                onClick={handleTogglePublish}
                title={lesson.isPublished ? 'Unpublish Lesson' : 'Publish Lesson'}
              >
                <Play className="h-3.5 w-3.5" />
                <span className="hidden sm:inline">{lesson.isPublished ? 'Unpublish' : 'Publish'}</span>
              </Button>
              <EditLessonDialog 
                lesson={lesson}
                onLessonUpdated={handleLessonUpdated}
              />
              <DeleteLessonDialog 
                lesson={lesson}
                onLessonDeleted={handleLessonDeleted}
              />
            </div>
          </div>

          {/* Title and Meta - Inline */}
          <div className="flex flex-col sm:flex-row sm:items-start sm:justify-between gap-2">
            <div className="flex-1 min-w-0">
              <div className="flex items-center gap-2 flex-wrap mb-1.5">
                <LessonStatusBadge isPublished={lesson.isPublished} />
                <LessonAccessBadge isFree={lesson.isFree} />
                {lesson.courseTitle && (
                  <Badge variant="outline" className="font-normal text-xs h-5">
                    {lesson.courseTitle}
                  </Badge>
                )}
                <div className="flex items-center gap-1.5 text-xs text-muted-foreground">
                  <Clock className="h-3 w-3" />
                  <span>{formatLessonDuration(lesson.duration)}</span>
                </div>
                <Badge variant="secondary" className="font-normal text-xs h-5">
                  Lesson {lesson.orderIndex} of {sortedLessons.length}
                </Badge>
              </div>
              <h1 className="text-xl sm:text-2xl lg:text-3xl font-bold tracking-tight break-words line-clamp-2">
                {lesson.title}
              </h1>
              {lesson.description && (
                <p className="text-sm text-muted-foreground mt-1.5 line-clamp-2">
                  {lesson.description}
                </p>
              )}
            </div>
          </div>
        </div>

        {/* Navigation - Compact */}
        {(previousLesson || nextLesson) && (
          <div className="flex items-center justify-between gap-2 p-2 bg-card border rounded-lg">
            {previousLesson ? (
              <Link to={`/lessons/${previousLesson.id}`} className="flex-1 min-w-0">
                <Button 
                  variant="ghost" 
                  size="sm"
                  className="h-8 px-2 gap-1.5 w-full justify-start text-xs hover:bg-accent"
                >
                  <ChevronLeft className="h-3.5 w-3.5 shrink-0" />
                  <span className="truncate">{previousLesson.title}</span>
                </Button>
              </Link>
            ) : (
              <div className="flex-1" />
            )}
            
            {nextLesson ? (
              <Link to={`/lessons/${nextLesson.id}`} className="flex-1 min-w-0">
                <Button 
                  variant="ghost" 
                  size="sm"
                  className="h-8 px-2 gap-1.5 w-full justify-end text-xs hover:bg-accent"
                >
                  <span className="truncate">{nextLesson.title}</span>
                  <ChevronRight className="h-3.5 w-3.5 shrink-0" />
                </Button>
              </Link>
            ) : (
              <div className="flex-1" />
            )}
          </div>
        )}

      <div className="grid grid-cols-1 lg:grid-cols-[1fr_280px] xl:grid-cols-[1fr_320px] gap-3 sm:gap-4">
        {/* Main Content */}
        <div className="space-y-3">
          {/* Learning Objectives */}
          <Card>
            <CardHeader className="pb-2 pt-3 px-4">
              <CardTitle className="text-base flex items-center gap-2">
                <Target className="h-4 w-4" />
                Learning Objectives
              </CardTitle>
            </CardHeader>
            <CardContent className="px-4 pb-4">
              {learningObjectives.length > 0 ? (
                <ul className="space-y-2 text-sm">
                  {learningObjectives.map((objective, index) => (
                    <li key={index} className="flex items-start gap-2">
                      <span className="text-primary mt-0.5">•</span>
                      <div className="flex-1 prose prose-sm max-w-none dark:prose-invert">
                        <ReactMarkdown>{objective}</ReactMarkdown>
                      </div>
                    </li>
                  ))}
                </ul>
              ) : (
                <div className="text-center py-6 text-sm text-muted-foreground">
                  <Target className="h-8 w-8 mx-auto mb-2 opacity-50" />
                  <p>No learning objectives defined for this lesson.</p>
                  <p className="text-xs mt-1">Edit the lesson to add learning objectives.</p>
                </div>
              )}
            </CardContent>
          </Card>

          {/* Lesson Content */}
          <Card>
            <CardHeader className="pb-2 pt-3 px-4">
              <CardTitle className="text-base flex items-center gap-2">
                <BookOpen className="h-4 w-4" />
                Content
              </CardTitle>
            </CardHeader>
            <CardContent className="px-4 pb-4 space-y-4">
              {lesson.content && (
                <div className="prose prose-sm max-w-none dark:prose-invert text-sm leading-relaxed">
                  <ReactMarkdown>{lesson.content}</ReactMarkdown>
                </div>
              )}

              {/* Video Section */}
              <div className="border-t pt-4">
                <div className="flex items-center gap-2 mb-3">
                  <Play className="h-4 w-4 text-primary" />
                  <h3 className="font-semibold text-sm">Video Content</h3>
                </div>
                {lesson.videoUrl ? (
                  isEmbeddableVideo(lesson.videoUrl) ? (
                    <div className="space-y-2">
                      <div className="aspect-video rounded-lg overflow-hidden border border-border bg-black shadow-sm">
                        {getVideoEmbedUrl(lesson.videoUrl)?.includes('youtube.com') || getVideoEmbedUrl(lesson.videoUrl)?.includes('vimeo.com') ? (
                          <iframe
                            src={getVideoEmbedUrl(lesson.videoUrl) || ''}
                            className="w-full h-full"
                            allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture; web-share"
                            allowFullScreen
                            title="Lesson Video"
                          />
                        ) : (
                          <video
                            src={getVideoEmbedUrl(lesson.videoUrl) || ''}
                            controls
                            className="w-full h-full"
                            preload="metadata"
                          >
                            Your browser does not support the video tag.
                          </video>
                        )}
                      </div>
                      <a 
                        href={lesson.videoUrl} 
                        target="_blank" 
                        rel="noopener noreferrer"
                        className="block text-xs text-muted-foreground hover:text-primary transition-colors truncate"
                        title={lesson.videoUrl}
                      >
                        {lesson.videoUrl}
                      </a>
                    </div>
                  ) : (
                    <div className="space-y-2">
                      <div className="aspect-video bg-muted rounded-lg flex items-center justify-center overflow-hidden border border-border hover:border-primary/50 transition-colors group">
                        <a 
                          href={lesson.videoUrl} 
                          target="_blank" 
                          rel="noopener noreferrer"
                          className="flex flex-col items-center gap-2 text-primary hover:text-primary/80 transition-colors"
                        >
                          <div className="w-12 h-12 rounded-full bg-primary/10 group-hover:bg-primary/20 flex items-center justify-center transition-colors">
                            <Play className="h-6 w-6 fill-primary text-primary" />
                          </div>
                          <span className="text-sm font-medium">Watch Video</span>
                          <span className="text-xs text-muted-foreground">Opens in new tab</span>
                        </a>
                      </div>
                      <a 
                        href={lesson.videoUrl} 
                        target="_blank" 
                        rel="noopener noreferrer"
                        className="block text-xs text-muted-foreground hover:text-primary transition-colors truncate"
                        title={lesson.videoUrl}
                      >
                        {lesson.videoUrl}
                      </a>
                    </div>
                  )
                ) : (
                  <div className="aspect-video bg-muted/50 rounded-lg flex flex-col items-center justify-center border-2 border-dashed border-muted-foreground/30">
                    <div className="w-16 h-16 rounded-full bg-muted flex items-center justify-center mb-3">
                      <Play className="h-8 w-8 text-muted-foreground" />
                    </div>
                    <p className="text-sm font-medium text-muted-foreground mb-1">No video available</p>
                    <p className="text-xs text-muted-foreground/80">Video URL has not been added to this lesson</p>
                  </div>
                )}
              </div>
            </CardContent>
          </Card>

          {/* Resources Section */}
          <Card>
            <CardHeader className="pb-2 pt-3 px-4">
              <CardTitle className="text-base flex items-center gap-2">
                <Link2 className="h-4 w-4" />
                Resources
                {resources.length > 0 && (
                  <Badge variant="secondary" className="ml-auto text-xs h-5 px-1.5">
                    {resources.length}
                  </Badge>
                )}
              </CardTitle>
            </CardHeader>
            <CardContent className="px-4 pb-4">
              {resources.length > 0 ? (
                <div className="space-y-2">
                  {resources.map((resource, index) => (
                    <a
                      key={index}
                      href={resource.url}
                      target="_blank"
                      rel="noopener noreferrer"
                      className="flex items-center gap-3 p-3 rounded-lg border border-border hover:border-primary/50 hover:bg-accent/50 transition-colors group"
                    >
                      <div className="flex-shrink-0 w-8 h-8 rounded-md bg-primary/10 group-hover:bg-primary/20 flex items-center justify-center text-primary transition-colors">
                        {getResourceIcon(resource.type)}
                      </div>
                      <div className="flex-1 min-w-0">
                        <div className="font-medium text-sm group-hover:text-primary transition-colors truncate">
                          {resource.name}
                        </div>
                        <div className="text-xs text-muted-foreground truncate">
                          {resource.type === 'download' ? 'Download' : resource.type === 'external' ? 'External Link' : 'Code Resource'}
                        </div>
                      </div>
                      {resource.type === 'external' && (
                        <ExternalLink className="h-4 w-4 text-muted-foreground group-hover:text-primary transition-colors flex-shrink-0" />
                      )}
                    </a>
                  ))}
                </div>
              ) : (
                <div className="text-center py-6 text-sm text-muted-foreground">
                  <Link2 className="h-8 w-8 mx-auto mb-2 opacity-50" />
                  <p>No resources available for this lesson.</p>
                  <p className="text-xs mt-1">Edit the lesson to add downloadable files, external links, or code resources.</p>
                </div>
              )}
            </CardContent>
          </Card>
        </div>

        {/* Sidebar - Compact */}
        <div className="space-y-3">
          {/* Course Lessons List - Compact */}
          {lesson.courseId && (
            <Card>
              <CardHeader className="pb-2 pt-3 px-3">
                <CardTitle className="text-sm flex items-center gap-1.5">
                  <BookOpen className="h-3.5 w-3.5" />
                  Lessons
                  <Badge variant="secondary" className="ml-auto text-xs h-5 px-1.5">
                    {sortedLessons.length}
                  </Badge>
                </CardTitle>
              </CardHeader>
              <CardContent className="p-0">
                {lessonsLoading ? (
                  <div className="p-3 text-center text-xs text-muted-foreground">
                    Loading...
                  </div>
                ) : sortedLessons.length === 0 ? (
                  <div className="p-3 text-center text-xs text-muted-foreground">
                    No lessons
                  </div>
                ) : (
                  <div className="max-h-[calc(100vh-400px)] min-h-[300px] overflow-y-auto">
                    <div className="p-1.5 space-y-0.5">
                      {sortedLessons.map((courseLesson, index) => {
                        const isActive = courseLesson.id === lesson.id;
                        
                        return (
                          <Link
                            key={courseLesson.id}
                            to={`/lessons/${courseLesson.id}`}
                            className={`block rounded-md transition-all ${
                              isActive
                                ? 'bg-primary text-primary-foreground shadow-sm'
                                : 'hover:bg-accent hover:text-accent-foreground'
                            }`}
                          >
                            <div className="flex items-center gap-2 p-2">
                              <div className={`flex-shrink-0 w-6 h-6 rounded-full flex items-center justify-center font-bold text-xs ${
                                isActive
                                  ? 'bg-primary-foreground/20 text-primary-foreground'
                                  : 'bg-muted text-muted-foreground'
                              }`}>
                                {index + 1}
                              </div>
                              <div className="flex-1 min-w-0">
                                <div className={`font-medium text-xs line-clamp-1 ${
                                  isActive ? 'text-primary-foreground' : 'text-foreground'
                                }`}>
                                  {courseLesson.title}
                                </div>
                                <div className="flex items-center gap-1.5 mt-0.5">
                                  <div className={`flex items-center gap-0.5 text-[10px] ${
                                    isActive ? 'text-primary-foreground/70' : 'text-muted-foreground'
                                  }`}>
                                    <Clock className="h-2.5 w-2.5" />
                                    {formatLessonDuration(courseLesson.duration)}
                                  </div>
                                  {courseLesson.isPublished && (
                                    <Badge 
                                      variant={isActive ? "secondary" : "outline"} 
                                      className="text-[10px] h-4 px-1"
                                    >
                                      Pub
                                    </Badge>
                                  )}
                                </div>
                              </div>
                              {isActive && (
                                <Play className="h-3 w-3 shrink-0 text-primary-foreground" />
                              )}
                            </div>
                          </Link>
                        );
                      })}
                    </div>
                  </div>
                )}
              </CardContent>
            </Card>
          )}

          {/* Lesson Info & Course Link - Combined Compact */}
          <Card>
            <CardHeader className="pb-2 pt-3 px-3">
              <CardTitle className="text-sm">Info</CardTitle>
            </CardHeader>
            <CardContent className="px-3 pb-3 space-y-2">
              <div className="grid grid-cols-2 gap-2 text-xs">
                <div>
                  <span className="text-muted-foreground">Duration</span>
                  <div className="font-medium mt-0.5">{formatLessonDuration(lesson.duration)}</div>
                </div>
                <div>
                  <span className="text-muted-foreground">Order</span>
                  <div className="mt-0.5">
                    <Badge variant="outline" className="font-normal text-xs h-5">
                      {lesson.orderIndex}
                    </Badge>
                  </div>
                </div>
              </div>
              
              {lesson.courseId && (
                <>
                  <Separator className="my-2" />
                  <Link to={`/courses/${lesson.courseId}`}>
                    <Button variant="outline" size="sm" className="w-full h-8 text-xs hover:bg-accent">
                      <BookOpen className="h-3.5 w-3.5 mr-1.5" />
                      View Course
                    </Button>
                  </Link>
                </>
              )}
            </CardContent>
          </Card>
        </div>
      </div>
      </div>
    </div>
  );
}

