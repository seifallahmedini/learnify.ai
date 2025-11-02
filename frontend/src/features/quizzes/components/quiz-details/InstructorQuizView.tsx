import { useState } from 'react';
import { Card, CardContent } from '@/shared/components/ui/card';
import { Badge } from '@/shared/components/ui/badge';
import { Button } from '@/shared/components/ui/button';
import { 
  HelpCircle, 
  Edit, 
  Trash2, 
  CheckCircle2, 
  XCircle
} from 'lucide-react';
import { type QuestionSummary, type QuestionAnswersResponse, QuestionType } from '../../types';
import { EditQuestionDialog, CreateQuestionDialog } from '../dialogs';

interface InstructorQuizViewProps {
  quizId: number;
  questions: QuestionSummary[];
  questionsWithAnswers: Map<number, QuestionAnswersResponse>;
  isLoadingAnswers: boolean;
  onQuestionUpdated?: () => void;
  onQuestionDeleted?: (questionId: number) => void;
}

/**
 * Instructor view component for quiz questions
 * Shows correct answers and allows editing
 */
export function InstructorQuizView({
  quizId,
  questions,
  questionsWithAnswers,
  isLoadingAnswers,
  onQuestionDeleted,
  onQuestionUpdated,
}: InstructorQuizViewProps) {
  const [editingQuestionId, setEditingQuestionId] = useState<number | null>(null);

  if (questions.length === 0) {
    return (
      <div className="text-center py-8">
        <div className="inline-flex items-center justify-center w-16 h-16 rounded-full bg-muted mb-4">
          <HelpCircle className="h-8 w-8 text-muted-foreground" />
        </div>
        <h3 className="text-sm font-semibold mb-1">No questions yet</h3>
        <p className="text-xs text-muted-foreground mb-4">
          Get started by creating your first question for this quiz.
        </p>
        <CreateQuestionDialog 
          quizId={quizId}
          onQuestionCreated={onQuestionUpdated}
        />
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
    <div className="space-y-3">
      <div className="flex justify-end">
        <CreateQuestionDialog 
          quizId={quizId}
          onQuestionCreated={onQuestionUpdated}
        />
      </div>
      
      {questions.map((question) => {
        const questionAnswers = questionsWithAnswers.get(question.id);

        return (
          <Card key={question.id} className="overflow-hidden">
            <CardContent className="p-4 space-y-3">
              {/* Question Header */}
              <div className="flex items-start justify-between gap-2">
                <div className="flex-1">
                  <div className="flex items-center gap-2 mb-2 flex-wrap">
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
                <div className="flex items-center gap-1">
                  <Button
                    variant="ghost"
                    size="sm"
                    className="h-7 w-7 p-0"
                    onClick={() => setEditingQuestionId(question.id)}
                  >
                    <Edit className="h-3.5 w-3.5" />
                  </Button>
                  {onQuestionDeleted && (
                    <Button
                      variant="ghost"
                      size="sm"
                      className="h-7 w-7 p-0 text-destructive hover:text-destructive"
                      onClick={() => {
                        if (confirm('Are you sure you want to delete this question?')) {
                          onQuestionDeleted(question.id);
                        }
                      }}
                    >
                      <Trash2 className="h-3.5 w-3.5" />
                    </Button>
                  )}
                </div>
              </div>

              {/* Answers */}
              {questionAnswers && questionAnswers.answers.length > 0 ? (
                <div className="mt-3 space-y-2">
                  <div className="text-xs font-medium text-muted-foreground mb-2">
                    Answers ({questionAnswers.totalAnswers}):
                  </div>
                  <div className="space-y-1.5">
                    {questionAnswers.answers.map((answer) => (
                      <div
                        key={answer.id}
                        className={`flex items-start gap-2 p-2 rounded-md text-sm ${
                          answer.isCorrect
                            ? 'bg-green-50 dark:bg-green-950/20 border border-green-200 dark:border-green-800'
                            : 'bg-muted/50 border border-transparent'
                        }`}
                      >
                        <div className="flex-shrink-0 mt-0.5">
                          {answer.isCorrect ? (
                            <CheckCircle2 className="h-4 w-4 text-green-600 dark:text-green-400" />
                          ) : (
                            <XCircle className="h-4 w-4 text-gray-400" />
                          )}
                        </div>
                        <div className="flex-1">
                          <span className={answer.isCorrect ? 'font-medium' : ''}>
                            {answer.answerText}
                          </span>
                        </div>
                        {answer.isCorrect && (
                          <Badge variant="default" className="text-xs bg-green-600">
                            Correct
                          </Badge>
                        )}
                      </div>
                    ))}
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

      {/* Edit Question Dialog */}
      {editingQuestionId !== null && (
        <EditQuestionDialog
          question={questions.find((q) => q.id === editingQuestionId)!}
          questionAnswers={questionsWithAnswers.get(editingQuestionId) || null}
          open={editingQuestionId !== null}
          onOpenChange={(open) => {
            if (!open) setEditingQuestionId(null);
          }}
          onQuestionUpdated={() => {
            setEditingQuestionId(null);
            onQuestionUpdated?.();
          }}
        />
      )}
    </div>
  );
}

