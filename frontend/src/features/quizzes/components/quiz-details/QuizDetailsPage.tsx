import { useState } from 'react';
import { useParams, Link, useNavigate } from 'react-router-dom';
import { Button } from '@/shared/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '@/shared/components/ui/card';
import { Badge } from '@/shared/components/ui/badge';
import { Tabs, TabsList, TabsTrigger, TabsContent } from '@/shared/components/ui/tabs';
import { 
  ArrowLeft,
  Clock,
  Target,
  HelpCircle,
  BookOpen,
  Settings,
  Eye,
  Edit as EditIcon,
  Award,
  RotateCw
} from 'lucide-react';
import { QuizDetailsSkeleton, QuizStatusBadge } from '../shared';
import { EditQuizDialog, DeleteQuizDialog } from '../dialogs';
import { useQuizDetails, useQuizOperations, useQuizQuestionsWithAnswers } from '../../hooks';
import { formatTimeLimit, formatPassingScore } from '../../lib';
import { InstructorQuizView } from './InstructorQuizView';
import { StudentQuizView } from './StudentQuizView';

export function QuizDetailsPage() {
  const { quizId } = useParams<{ quizId: string }>();
  const navigate = useNavigate();
  const quizIdNum = quizId ? parseInt(quizId, 10) : null;
  const [viewMode, setViewMode] = useState<'instructor' | 'student'>('instructor');
  const [selectedAnswers, setSelectedAnswers] = useState<Map<number, number>>(new Map());

  const { quiz, isLoading, error, loadQuiz } = useQuizDetails(quizIdNum);
  const { activateQuiz, deactivateQuiz } = useQuizOperations();
  const { questions, questionsWithAnswers, isLoadingAnswers, loadQuestions } = useQuizQuestionsWithAnswers(quizIdNum);

  const handleQuizUpdated = () => {
    loadQuiz();
    loadQuestions();
  };

  const handleAnswerSelect = (questionId: number, answerId: number) => {
    setSelectedAnswers(prev => {
      const newMap = new Map(prev);
      newMap.set(questionId, answerId);
      return newMap;
    });
  };

  const handleQuestionDeleted = (questionId: number) => {
    // TODO: Implement question deletion API call
    console.log('Delete question:', questionId);
    loadQuestions();
    loadQuiz();
  };

  const handleQuestionUpdated = () => {
    loadQuestions();
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
            <Button 
              variant="ghost" 
              size="sm" 
              className="group -ml-2 hover:bg-accent/50 transition-colors"
              onClick={() => navigate(-1)}
            >
              <ArrowLeft className="h-4 w-4 mr-2 group-hover:-translate-x-1 transition-transform" />
              Back
            </Button>
            
            <div className="flex items-center gap-1.5 sm:gap-2 flex-shrink-0">
              <Button 
                variant="outline" 
                size="sm"
                onClick={handleToggleActive}
                className="gap-1.5 sm:gap-2 h-9 px-3 sm:px-4 hover:bg-primary/5 hover:border-primary/30 hover:text-primary transition-all shadow-sm hover:shadow-md border-border/50"
                title={quiz.isActive ? 'Deactivate Quiz' : 'Activate Quiz'}
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
                  if (quiz.courseId) {
                    navigate(`/courses/${quiz.courseId}`);
                  } else {
                    navigate('/courses');
                  }
                }}
              />
            </div>
          </div>

          <div className="flex flex-col gap-2">
            <div className="flex items-center justify-between gap-4 flex-wrap">
              <div className="flex items-center gap-2.5 sm:gap-3 flex-wrap">
                <QuizStatusBadge isActive={quiz.isActive} />
                {quiz.courseTitle && (
                  <Badge variant="outline" className="text-xs h-5 font-normal">
                    {quiz.courseTitle}
                  </Badge>
                )}
              </div>
            </div>

            <div className="flex items-start justify-between gap-4">
              <div className="flex-1 min-w-0">
                <h1 className="text-xl sm:text-2xl font-bold tracking-tight break-words">
                  {quiz.title}
                </h1>
                {quiz.description && (
                  <p className="text-sm text-muted-foreground mt-1.5 max-w-3xl leading-relaxed">
                    {quiz.description}
                  </p>
                )}
              </div>
            </div>
          </div>
        </div>

        {/* Quiz Statistics Cards */}
        <div className="grid grid-cols-2 lg:grid-cols-4 gap-2 sm:gap-3">
          <Card className="hover:shadow-md transition-all duration-200 border-l-4 border-l-blue-500">
            <CardContent className="p-3">
              <div className="flex items-center gap-2.5">
                <div className="p-2 bg-blue-500/10 rounded-lg shrink-0">
                  <HelpCircle className="h-4 w-4 text-blue-600 dark:text-blue-400" />
                </div>
                <div className="flex-1 min-w-0">
                  <p className="text-xs font-medium text-muted-foreground mb-0.5">Questions</p>
                  <p className="text-lg sm:text-xl font-bold">{quiz.questionCount || 0}</p>
                </div>
              </div>
            </CardContent>
          </Card>
          
          <Card className="hover:shadow-md transition-all duration-200 border-l-4 border-l-purple-500">
            <CardContent className="p-3">
              <div className="flex items-center gap-2.5">
                <div className="p-2 bg-purple-500/10 rounded-lg shrink-0">
                  <Award className="h-4 w-4 text-purple-600 dark:text-purple-400" />
                </div>
                <div className="flex-1 min-w-0">
                  <p className="text-xs font-medium text-muted-foreground mb-0.5">Total Points</p>
                  <p className="text-lg sm:text-xl font-bold">{quiz.totalPoints || 0}</p>
                </div>
              </div>
            </CardContent>
          </Card>
          
          <Card className="hover:shadow-md transition-all duration-200 border-l-4 border-l-green-500">
            <CardContent className="p-3">
              <div className="flex items-center gap-2.5">
                <div className="p-2 bg-green-500/10 rounded-lg shrink-0">
                  <Target className="h-4 w-4 text-green-600 dark:text-green-400" />
                </div>
                <div className="flex-1 min-w-0">
                  <p className="text-xs font-medium text-muted-foreground mb-0.5">Passing Score</p>
                  <p className="text-lg sm:text-xl font-bold">{formatPassingScore(quiz.passingScore)}</p>
                </div>
              </div>
            </CardContent>
          </Card>
          
          <Card className="hover:shadow-md transition-all duration-200 border-l-4 border-l-orange-500">
            <CardContent className="p-3">
              <div className="flex items-center gap-2.5">
                <div className="p-2 bg-orange-500/10 rounded-lg shrink-0">
                  {quiz.timeLimit ? (
                    <Clock className="h-4 w-4 text-orange-600 dark:text-orange-400" />
                  ) : (
                    <RotateCw className="h-4 w-4 text-orange-600 dark:text-orange-400" />
                  )}
                </div>
                <div className="flex-1 min-w-0">
                  <p className="text-xs font-medium text-muted-foreground mb-0.5">
                    {quiz.timeLimit ? 'Time Limit' : 'Max Attempts'}
                  </p>
                  <p className="text-lg sm:text-xl font-bold">
                    {quiz.timeLimit ? formatTimeLimit(quiz.timeLimit) : quiz.maxAttempts || 'Unlimited'}
                  </p>
                </div>
              </div>
            </CardContent>
          </Card>
        </div>

        {/* Main Content */}
        <div className="grid grid-cols-1 lg:grid-cols-3 gap-3 sm:gap-4">
          <div className="lg:col-span-2 space-y-3">
            {/* Questions Section */}
            <Card>
              <CardHeader className="pb-2 pt-3 px-4">
                <div className="flex items-center justify-between gap-3 flex-wrap">
                  <CardTitle className="text-base flex items-center gap-2">
                    <HelpCircle className="h-4 w-4" />
                    Questions
                    {questions.length > 0 && (
                      <Badge variant="secondary" className="text-xs">
                        {questions.length}
                      </Badge>
                    )}
                  </CardTitle>
                  <Tabs value={viewMode} onValueChange={(value) => setViewMode(value as 'instructor' | 'student')}>
                    <TabsList className="h-8">
                      <TabsTrigger value="instructor" className="text-xs px-2 gap-1.5">
                        <EditIcon className="h-3.5 w-3.5" />
                        <span className="hidden sm:inline">Instructor</span>
                      </TabsTrigger>
                      <TabsTrigger value="student" className="text-xs px-2 gap-1.5">
                        <Eye className="h-3.5 w-3.5" />
                        <span className="hidden sm:inline">Preview</span>
                      </TabsTrigger>
                    </TabsList>
                  </Tabs>
                </div>
              </CardHeader>
              <CardContent className="px-4 pb-4">
                <Tabs value={viewMode} onValueChange={(value) => setViewMode(value as 'instructor' | 'student')}>
                  <TabsContent value="instructor" className="mt-0">
                    <InstructorQuizView
                      quizId={quizIdNum!}
                      questions={questions}
                      questionsWithAnswers={questionsWithAnswers}
                      isLoadingAnswers={isLoadingAnswers}
                      onQuestionDeleted={handleQuestionDeleted}
                      onQuestionUpdated={handleQuestionUpdated}
                    />
                  </TabsContent>
                  <TabsContent value="student" className="mt-0">
                    <StudentQuizView
                      questions={questions}
                      questionsWithAnswers={questionsWithAnswers}
                      isLoadingAnswers={isLoadingAnswers}
                      onAnswerSelect={handleAnswerSelect}
                      selectedAnswers={selectedAnswers}
                      showResults={false}
                    />
                  </TabsContent>
                </Tabs>
              </CardContent>
            </Card>
          </div>

          {/* Sidebar */}
          <div className="space-y-3">
            {/* Quiz Information */}
            <Card>
              <CardHeader className="pb-2 pt-3 px-3">
                <CardTitle className="text-sm">Quiz Information</CardTitle>
              </CardHeader>
              <CardContent className="px-3 pb-3 space-y-2">
                <div className="flex items-center justify-between py-1.5">
                  <span className="text-xs font-medium text-muted-foreground">Status</span>
                  <QuizStatusBadge isActive={quiz.isActive} size="sm" />
                </div>
                
                <div className="flex items-center justify-between py-1.5">
                  <span className="text-xs font-medium text-muted-foreground">Questions</span>
                  <span className="text-xs font-medium">{quiz.questionCount || 0}</span>
                </div>
                
                <div className="flex items-center justify-between py-1.5">
                  <span className="text-xs font-medium text-muted-foreground">Total Points</span>
                  <span className="text-xs font-medium">{quiz.totalPoints || 0}</span>
                </div>
                
                <div className="flex items-center justify-between py-1.5">
                  <span className="text-xs font-medium text-muted-foreground">Passing Score</span>
                  <span className="text-xs font-medium">{formatPassingScore(quiz.passingScore)}</span>
                </div>
                
                {quiz.timeLimit && (
                  <div className="flex items-center justify-between py-1.5">
                    <span className="text-xs font-medium text-muted-foreground">Time Limit</span>
                    <span className="text-xs font-medium">{formatTimeLimit(quiz.timeLimit)}</span>
                  </div>
                )}
                
                <div className="flex items-center justify-between py-1.5">
                  <span className="text-xs font-medium text-muted-foreground">Max Attempts</span>
                  <span className="text-xs font-medium">{quiz.maxAttempts || 'Unlimited'}</span>
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

