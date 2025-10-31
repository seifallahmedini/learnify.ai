import { useParams, Link } from 'react-router-dom';
import { Button } from '@/shared/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '@/shared/components/ui/card';
import { Badge } from '@/shared/components/ui/badge';
import { Switch } from '@/shared/components/ui/switch';
import { 
  ArrowLeft,
  Plus,
  Clock,
  BookOpen,
  Target,
  HelpCircle
} from 'lucide-react';
import { CreateQuizDialog } from '../dialogs';
import { QuizListSkeleton, QuizStatusBadge } from '../shared';
import { useQuizManagement } from '../../hooks';
import { formatTimeLimit, formatPassingScore } from '../../lib';

export function QuizzesListPage() {
  const { courseId } = useParams<{ courseId: string }>();
  const courseIdNum = courseId ? parseInt(courseId, 10) : null;

  const {
    quizzes,
    isLoading,
    error,
    showActiveOnly,
    refreshQuizzes,
    toggleActiveFilter,
  } = useQuizManagement({ courseId: courseIdNum });

  const handleQuizCreated = () => {
    refreshQuizzes();
  };

  return (
    <div className="min-h-screen bg-gradient-to-b from-gray-50/50 to-white dark:from-gray-950/50 dark:to-gray-900">
      <div className="p-3 sm:p-4 lg:p-6 space-y-3 sm:space-y-4">
        {/* Header */}
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
            
            <CreateQuizDialog 
              courseId={courseIdNum}
              onQuizCreated={handleQuizCreated}
            />
          </div>

          <div className="flex items-center justify-between gap-3 flex-wrap">
            <div>
              <h1 className="text-xl sm:text-2xl font-bold">Course Quizzes</h1>
              <p className="text-xs sm:text-sm text-muted-foreground mt-0.5">
                Manage and organize quizzes
              </p>
            </div>
            
            <Badge variant="secondary" className="text-xs">
              {quizzes.length} {quizzes.length === 1 ? 'Quiz' : 'Quizzes'}
            </Badge>
          </div>
        </div>

        {/* Filters */}
        <div className="flex items-center justify-between gap-3 p-2.5 bg-card border rounded-lg">
          <div className="flex items-center gap-2">
            <Switch
              checked={showActiveOnly}
              onCheckedChange={toggleActiveFilter}
              id="active-filter"
              className="scale-90"
            />
            <label 
              htmlFor="active-filter" 
              className="text-xs font-medium cursor-pointer text-muted-foreground"
            >
              Active only
            </label>
          </div>
          {error && (
            <span className="text-xs text-destructive">{error}</span>
          )}
        </div>

        {/* Loading State */}
        {isLoading ? (
          <QuizListSkeleton />
        ) : quizzes.length === 0 ? (
          <Card>
            <CardContent className="p-8 text-center">
              <div className="inline-flex items-center justify-center w-16 h-16 rounded-full bg-muted mb-4">
                <HelpCircle className="h-8 w-8 text-muted-foreground" />
              </div>
              <h3 className="text-lg font-semibold mb-2">No Quizzes Found</h3>
              <p className="text-sm text-muted-foreground mb-4">
                Get started by creating your first quiz for this course.
              </p>
              <CreateQuizDialog 
                courseId={courseIdNum}
                onQuizCreated={handleQuizCreated}
              />
            </CardContent>
          </Card>
        ) : (
          /* Quizzes List */
          <div className="space-y-2">
            {quizzes.map((quiz) => (
              <Card 
                key={quiz.id} 
                className="group hover:shadow-md transition-all duration-200 border-l-4 border-l-transparent hover:border-l-primary"
              >
                <CardContent className="p-3 sm:p-4">
                  <div className="flex items-start justify-between gap-3">
                    <Link 
                      to={`/quizzes/${quiz.id}`}
                      className="flex-1 min-w-0 group/link"
                    >
                      <div className="space-y-1.5">
                        <div className="flex items-start justify-between gap-2">
                          <h3 className="font-semibold text-sm sm:text-base group-hover/link:text-primary transition-colors line-clamp-1">
                            {quiz.title}
                          </h3>
                          <QuizStatusBadge isActive={quiz.isActive} size="sm" />
                        </div>
                        {quiz.description && (
                          <p className="text-xs sm:text-sm text-muted-foreground line-clamp-2">
                            {quiz.description}
                          </p>
                        )}
                        <div className="flex items-center gap-3 text-xs text-muted-foreground flex-wrap">
                          <div className="flex items-center gap-1">
                            <HelpCircle className="h-3 w-3" />
                            <span>{quiz.questionCount} questions</span>
                          </div>
                          {quiz.timeLimit && (
                            <div className="flex items-center gap-1">
                              <Clock className="h-3 w-3" />
                              <span>{formatTimeLimit(quiz.timeLimit)}</span>
                            </div>
                          )}
                          <div className="flex items-center gap-1">
                            <Target className="h-3 w-3" />
                            <span>Pass: {formatPassingScore(quiz.passingScore)}</span>
                          </div>
                        </div>
                      </div>
                    </Link>
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

