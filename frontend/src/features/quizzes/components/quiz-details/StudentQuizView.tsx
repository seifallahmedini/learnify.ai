import { Card, CardContent } from '@/shared/components/ui/card';
import { Badge } from '@/shared/components/ui/badge';
import { HelpCircle } from 'lucide-react';
import { type QuestionSummary, type QuestionAnswersResponse, QuestionType } from '../../types';

interface StudentQuizViewProps {
  questions: QuestionSummary[];
  questionsWithAnswers: Map<number, QuestionAnswersResponse>;
  isLoadingAnswers: boolean;
  onAnswerSelect?: (questionId: number, answerId: number) => void;
  selectedAnswers?: Map<number, number>; // questionId -> answerId
  showResults?: boolean; // If true, shows correct answers after submission
}

/**
 * Reusable component for student quiz view
 * Hides correct answers during quiz taking, shows them only if showResults is true
 */
export function StudentQuizView({
  questions,
  questionsWithAnswers,
  isLoadingAnswers,
  onAnswerSelect,
  selectedAnswers = new Map(),
  showResults = false,
}: StudentQuizViewProps) {
  if (questions.length === 0) {
    return (
      <div className="text-center py-8">
        <HelpCircle className="h-12 w-12 mx-auto mb-4 text-muted-foreground" />
        <p className="text-sm text-muted-foreground">No questions available</p>
      </div>
    );
  }

  if (isLoadingAnswers) {
    return (
      <div className="space-y-4">
        {questions.map((question) => (
          <Card key={question.id} className="animate-pulse">
            <CardContent className="p-4">
              <div className="h-4 bg-gray-200 rounded w-3/4 mb-2"></div>
              <div className="h-3 bg-gray-200 rounded w-1/2"></div>
            </CardContent>
          </Card>
        ))}
      </div>
    );
  }

  return (
    <div className="space-y-4">
      {questions.map((question) => {
        const questionAnswers = questionsWithAnswers.get(question.id);
        const selectedAnswerId = selectedAnswers.get(question.id);

        return (
          <Card key={question.id} className="overflow-hidden">
            <CardContent className="p-4 space-y-3">
              {/* Question Header */}
              <div className="flex items-start justify-between gap-2">
                <div className="flex-1">
                  <div className="flex items-center gap-2 mb-2">
                    <Badge variant="outline" className="text-xs">
                      Q{question.orderIndex}
                    </Badge>
                    <Badge variant="secondary" className="text-xs">
                      {QuestionType[question.questionType]}
                    </Badge>
                    <Badge variant="outline" className="text-xs">
                      {question.points} {question.points === 1 ? 'point' : 'points'}
                    </Badge>
                  </div>
                  <p className="text-sm font-medium">{question.questionText}</p>
                </div>
              </div>

              {/* Answers */}
              {questionAnswers && questionAnswers.answers.length > 0 ? (
                <div className="mt-3 space-y-2">
                  <div className="text-xs font-medium text-muted-foreground mb-2">
                    Select your answer:
                  </div>
                  <div className="space-y-1.5">
                    {questionAnswers.answers.map((answer) => {
                      const isSelected = selectedAnswerId === answer.id;
                      const isCorrect = showResults && answer.isCorrect;
                      const isWrong = showResults && isSelected && !answer.isCorrect;

                      return (
                        <div
                          key={answer.id}
                          onClick={() => !showResults && onAnswerSelect?.(question.id, answer.id)}
                          className={`flex items-start gap-2 p-3 rounded-md text-sm cursor-pointer transition-colors ${
                            isSelected
                              ? 'bg-primary/10 border-2 border-primary'
                              : 'bg-muted/50 border-2 border-transparent hover:bg-muted'
                          } ${
                            isCorrect
                              ? 'bg-green-50 dark:bg-green-950/20 border-green-200 dark:border-green-800'
                              : ''
                          } ${
                            isWrong
                              ? 'bg-red-50 dark:bg-red-950/20 border-red-200 dark:border-red-800'
                              : ''
                          }`}
                        >
                          <div className="flex-shrink-0 mt-0.5">
                            {isSelected ? (
                              <div className="h-4 w-4 rounded-full border-2 border-primary bg-primary flex items-center justify-center">
                                <div className="h-2 w-2 rounded-full bg-white"></div>
                              </div>
                            ) : (
                              <div className="h-4 w-4 rounded-full border-2 border-muted-foreground/30"></div>
                            )}
                          </div>
                          <div className="flex-1">
                            <span className={isSelected ? 'font-medium' : ''}>
                              {answer.answerText}
                            </span>
                          </div>
                          {showResults && isCorrect && (
                            <Badge variant="default" className="text-xs bg-green-600">
                              Correct
                            </Badge>
                          )}
                          {showResults && isWrong && (
                            <Badge variant="destructive" className="text-xs">
                              Wrong
                            </Badge>
                          )}
                        </div>
                      );
                    })}
                  </div>
                </div>
              ) : (
                <div className="text-xs text-muted-foreground py-2">
                  No answers available
                </div>
              )}
            </CardContent>
          </Card>
        );
      })}
    </div>
  );
}

