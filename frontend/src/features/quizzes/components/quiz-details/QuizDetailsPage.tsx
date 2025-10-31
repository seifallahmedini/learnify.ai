import { useParams, Link } from 'react-router-dom';
import { Button } from '@/shared/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '@/shared/components/ui/card';
import { Badge } from '@/shared/components/ui/badge';
import { 
  ArrowLeft,
  Clock,
  Target,
  HelpCircle,
  BookOpen,
  Settings
} from 'lucide-react';
import { QuizDetailsSkeleton, QuizStatusBadge } from '../shared';
import { EditQuizDialog, DeleteQuizDialog } from '../dialogs';
import { useQuizDetails, useQuizOperations } from '../../hooks';
import { formatTimeLimit, formatPassingScore } from '../../lib';

export function QuizDetailsPage() {
  const { quizId } = useParams<{ quizId: string }>();
  const quizIdNum = quizId ? parseInt(quizId, 10) : null;

  const { quiz, isLoading, error, loadQuiz } = useQuizDetails(quizIdNum);
  const { activateQuiz, deactivateQuiz } = useQuizOperations();

  const handleQuizUpdated = (updatedQuiz: any) => {
    loadQuiz();
  };

  const handleToggleActive = async () => {
    if (!quiz) return;
    
    if (quiz.isActive) {
      await deactivateQuiz(quiz.id);
    } else {
      await activateQuiz(quiz.id);
    }
    loadQuiz();
  };

  if (isLoading) {
    return (
      <div className="p-3 sm:p-4 lg:p-6">
        <QuizDetailsSkeleton />
      </div>
    );
  }

  if (error || !quiz) {
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
            <HelpCircle className="h-12 w-12 mx-auto mb-4 text-gray-400" />
            <h3 className="text-lg font-semibold mb-2">Quiz Not Found</h3>
            <p className="text-gray-600 mb-4">
              {error || 'The quiz you are looking for could not be found.'}
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
        {/* Header */}
        <div className="space-y-2">
          <div className="flex items-center justify-between gap-3 flex-wrap">
            <Link to={quiz.courseId ? `/courses/${quiz.courseId}` : '/courses'}>
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
                onClick={handleToggleActive}
                className="gap-1.5 sm:gap-2 h-8 px-3 sm:px-4 hover:bg-primary/5 hover:border-primary/30 hover:text-primary transition-all"
              >
                <Settings className="h-4 w-4" />
                <span className="hidden sm:inline">{quiz.isActive ? 'Deactivate' : 'Activate'}</span>
              </Button>
              <EditQuizDialog 
                quiz={quiz}
                onQuizUpdated={handleQuizUpdated}
              />
              <DeleteQuizDialog 
                quiz={quiz}
                onQuizDeleted={() => {
                  window.location.href = quiz.courseId ? `/courses/${quiz.courseId}` : '/courses';
                }}
              />
            </div>
          </div>

          <div className="flex items-start gap-3">
            <div className="flex-1 min-w-0">
              <div className="flex items-center gap-2 flex-wrap mb-1.5">
                <QuizStatusBadge isActive={quiz.isActive} />
                <Badge variant="outline" className="text-xs h-5 font-normal">
                  ID: {quiz.id}
                </Badge>
                {quiz.courseTitle && (
                  <Badge variant="outline" className="text-xs h-5 font-normal">
                    {quiz.courseTitle}
                  </Badge>
                )}
              </div>
              <h1 className="text-xl sm:text-2xl lg:text-3xl font-bold tracking-tight break-words">
                {quiz.title}
              </h1>
              {quiz.description && (
                <p className="text-xs sm:text-sm text-muted-foreground mt-1.5 line-clamp-2">
                  {quiz.description}
                </p>
              )}
            </div>
          </div>
        </div>

        {/* Main Content */}
        <div className="grid grid-cols-1 lg:grid-cols-[1fr_280px] xl:grid-cols-[1fr_320px] gap-3 sm:gap-4">
          <div className="space-y-3">
            {/* Quiz Information */}
            <Card>
              <CardHeader className="pb-2 pt-3 px-4">
                <CardTitle className="text-base flex items-center gap-2">
                  <BookOpen className="h-4 w-4 text-primary" />
                  Quiz Information
                </CardTitle>
              </CardHeader>
              <CardContent className="px-4 pb-4 space-y-3">
                <div className="grid grid-cols-1 sm:grid-cols-2 gap-3">
                  <div>
                    <div className="flex items-center gap-1.5 text-xs font-medium text-muted-foreground mb-1.5">
                      <HelpCircle className="h-3.5 w-3.5" />
                      Questions
                    </div>
                    <div className="text-sm font-medium">{quiz.questionCount}</div>
                  </div>
                  <div>
                    <div className="flex items-center gap-1.5 text-xs font-medium text-muted-foreground mb-1.5">
                      <Target className="h-3.5 w-3.5" />
                      Passing Score
                    </div>
                    <div className="text-sm font-medium">{formatPassingScore(quiz.passingScore)}</div>
                  </div>
                  {quiz.timeLimit && (
                    <div>
                      <div className="flex items-center gap-1.5 text-xs font-medium text-muted-foreground mb-1.5">
                        <Clock className="h-3.5 w-3.5" />
                        Time Limit
                      </div>
                      <div className="text-sm font-medium">{formatTimeLimit(quiz.timeLimit)}</div>
                    </div>
                  )}
                  <div>
                    <div className="flex items-center gap-1.5 text-xs font-medium text-muted-foreground mb-1.5">
                      <Settings className="h-3.5 w-3.5" />
                      Max Attempts
                    </div>
                    <div className="text-sm font-medium">{quiz.maxAttempts}</div>
                  </div>
                </div>
              </CardContent>
            </Card>

            {/* Questions Section */}
            <Card>
              <CardHeader className="pb-2 pt-3 px-4">
                <CardTitle className="text-base flex items-center gap-2">
                  <HelpCircle className="h-4 w-4 text-primary" />
                  Questions
                </CardTitle>
              </CardHeader>
              <CardContent className="px-4 pb-4">
                {quiz.questionCount === 0 ? (
                  <div className="text-center py-8">
                    <HelpCircle className="h-12 w-12 mx-auto mb-4 text-muted-foreground" />
                    <p className="text-sm text-muted-foreground">No questions yet</p>
                  </div>
                ) : (
                  <p className="text-sm text-muted-foreground">
                    {quiz.questionCount} question{quiz.questionCount !== 1 ? 's' : ''} available
                  </p>
                )}
              </CardContent>
            </Card>
          </div>

          {/* Sidebar */}
          <div className="space-y-3">
            <Card>
              <CardHeader className="pb-2 pt-3 px-3">
                <CardTitle className="text-sm">Quiz Stats</CardTitle>
              </CardHeader>
              <CardContent className="px-3 pb-3 space-y-2">
                <div className="bg-primary/5 border border-primary/20 rounded-lg p-2.5">
                  <div className="flex items-center justify-between">
                    <span className="text-xs font-medium">Total Points</span>
                    <span className="text-lg font-bold text-primary">{quiz.totalPoints}</span>
                  </div>
                </div>
                <div className="bg-muted/50 border rounded-lg p-2.5">
                  <div className="flex items-center justify-between">
                    <span className="text-xs font-medium">Status</span>
                    <QuizStatusBadge isActive={quiz.isActive} size="sm" />
                  </div>
                </div>
              </CardContent>
            </Card>

            {quiz.courseId && (
              <Card>
                <CardHeader className="pb-2 pt-3 px-3">
                  <CardTitle className="text-sm">Course</CardTitle>
                </CardHeader>
                <CardContent className="px-3 pb-3">
                  <Link to={`/courses/${quiz.courseId}`}>
                    <Button variant="outline" size="sm" className="w-full h-8 text-xs hover:bg-accent">
                      <BookOpen className="h-3.5 w-3.5 mr-1.5" />
                      View Course
                    </Button>
                  </Link>
                </CardContent>
              </Card>
            )}
          </div>
        </div>
      </div>
    </div>
  );
}

