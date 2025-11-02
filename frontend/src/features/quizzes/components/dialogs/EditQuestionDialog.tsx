import { useState, useEffect } from 'react';
import { Button } from '@/shared/components/ui/button';
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from '@/shared/components/ui/dialog';
import { Input } from '@/shared/components/ui/input';
import { Textarea } from '@/shared/components/ui/textarea';
import { Label } from '@/shared/components/ui/label';
import { Checkbox } from '@/shared/components/ui/checkbox';
import { Badge } from '@/shared/components/ui/badge';
import { Plus, Trash2, GripVertical } from 'lucide-react';
import { answersApi, type CreateAnswerRequest, type UpdateAnswerRequest } from '../../services/answersApi';
import { type QuestionSummary, type QuestionAnswersResponse, type AnswerSummary, QuestionType } from '../../types';

interface EditQuestionDialogProps {
  question: QuestionSummary;
  questionAnswers: QuestionAnswersResponse | null;
  open: boolean;
  onOpenChange: (open: boolean) => void;
  onQuestionUpdated?: () => void;
}

interface AnswerFormData {
  id?: number;
  answerText: string;
  isCorrect: boolean;
  orderIndex: number;
}

export function EditQuestionDialog({
  question,
  questionAnswers,
  open,
  onOpenChange,
  onQuestionUpdated,
}: EditQuestionDialogProps) {
  const [questionText, setQuestionText] = useState(question.questionText);
  const [answers, setAnswers] = useState<AnswerFormData[]>([]);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  // Initialize form data when dialog opens
  useEffect(() => {
    if (open && questionAnswers) {
      setQuestionText(question.questionText);
      const sortedAnswers = [...questionAnswers.answers].sort((a, b) => a.orderIndex - b.orderIndex);
      setAnswers(
        sortedAnswers.map((answer) => ({
          id: answer.id,
          answerText: answer.answerText,
          isCorrect: answer.isCorrect,
          orderIndex: answer.orderIndex,
        }))
      );
      setError(null);
    }
  }, [open, question, questionAnswers]);

  const handleAddAnswer = () => {
    const newOrderIndex = answers.length > 0 ? Math.max(...answers.map((a) => a.orderIndex)) + 1 : 1;
    setAnswers([
      ...answers,
      {
        answerText: '',
        isCorrect: false,
        orderIndex: newOrderIndex,
      },
    ]);
  };

  const handleRemoveAnswer = (index: number) => {
    setAnswers(answers.filter((_, i) => i !== index));
  };

  const handleAnswerChange = (index: number, field: keyof AnswerFormData, value: string | boolean) => {
    const newAnswers = [...answers];
    newAnswers[index] = { ...newAnswers[index], [field]: value };
    
    // If marking as correct and it's multiple choice, uncheck others
    if (field === 'isCorrect' && value === true && question.questionType === QuestionType.MultipleChoice) {
      newAnswers.forEach((answer, i) => {
        if (i !== index) {
          answer.isCorrect = false;
        }
      });
    }
    
    setAnswers(newAnswers);
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);
    setIsLoading(true);

    try {
      // Validate form
      if (!questionText.trim()) {
        throw new Error('Question text is required');
      }

      const validAnswers = answers.filter((a) => a.answerText.trim());
      if (validAnswers.length === 0) {
        throw new Error('At least one answer is required');
      }

      // Check if there's at least one correct answer
      const hasCorrectAnswer = validAnswers.some((a) => a.isCorrect);
      if (!hasCorrectAnswer && question.questionType !== QuestionType.Essay) {
        throw new Error('At least one answer must be marked as correct');
      }

      // Update existing answers and create new ones
      const existingAnswers = answers.filter((a) => a.id !== undefined);
      const newAnswers = answers.filter((a) => a.id === undefined && a.answerText.trim());

      // Update existing answers
      for (const answer of existingAnswers) {
        if (answer.id) {
          const originalAnswer = questionAnswers?.answers.find((a) => a.id === answer.id);
          if (originalAnswer) {
            const hasChanges =
              answer.answerText !== originalAnswer.answerText ||
              answer.isCorrect !== originalAnswer.isCorrect ||
              answer.orderIndex !== originalAnswer.orderIndex;

            if (hasChanges) {
              await answersApi.updateAnswer(answer.id, {
                answerText: answer.answerText,
                isCorrect: answer.isCorrect,
                orderIndex: answer.orderIndex,
              });
            }
          }
        }
      }

      // Create new answers
      for (const answer of newAnswers) {
        await answersApi.createAnswer({
          questionId: question.id,
          answerText: answer.answerText,
          isCorrect: answer.isCorrect,
          orderIndex: answer.orderIndex,
        });
      }

      // Delete removed answers
      const currentAnswerIds = answers.filter((a) => a.id !== undefined).map((a) => a.id!);
      const originalAnswerIds = questionAnswers?.answers.map((a) => a.id) || [];
      const answersToDelete = originalAnswerIds.filter((id) => !currentAnswerIds.includes(id));

      for (const answerId of answersToDelete) {
        await answersApi.deleteAnswer(answerId);
      }

      onQuestionUpdated?.();
      onOpenChange(false);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to update question');
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="sm:max-w-[700px] max-h-[90vh] overflow-y-auto">
        <form onSubmit={handleSubmit}>
          <DialogHeader>
            <DialogTitle>Edit Question</DialogTitle>
            <DialogDescription>
              Update question text and manage answer options.
            </DialogDescription>
          </DialogHeader>

          <div className="space-y-4 py-4">
            {error && (
              <div className="p-3 bg-destructive/10 border border-destructive/20 rounded-lg text-sm text-destructive">
                {error}
              </div>
            )}

            {/* Question Text */}
            <div className="space-y-2">
              <Label htmlFor="questionText">Question Text *</Label>
              <Textarea
                id="questionText"
                value={questionText}
                onChange={(e) => setQuestionText(e.target.value)}
                placeholder="Enter question text"
                rows={3}
                required
                disabled
                className="bg-muted"
              />
              <p className="text-xs text-muted-foreground">
                Question text editing is not yet available via API
              </p>
            </div>

            {/* Question Type Badge */}
            <div className="flex items-center gap-2">
              <Label className="text-sm">Question Type:</Label>
              <Badge variant="secondary">{QuestionType[question.questionType]}</Badge>
              <Badge variant="outline">{question.points} points</Badge>
            </div>

            {/* Answers Section */}
            <div className="space-y-3">
              <div className="flex items-center justify-between">
                <Label>Answers *</Label>
                <Button
                  type="button"
                  variant="outline"
                  size="sm"
                  onClick={handleAddAnswer}
                  className="gap-1.5"
                >
                  <Plus className="h-3.5 w-3.5" />
                  Add Answer
                </Button>
              </div>

              {answers.length === 0 ? (
                <div className="text-center py-6 border border-dashed rounded-lg">
                  <p className="text-sm text-muted-foreground mb-2">No answers yet</p>
                  <Button type="button" variant="outline" size="sm" onClick={handleAddAnswer}>
                    <Plus className="h-3.5 w-3.5 mr-1.5" />
                    Add First Answer
                  </Button>
                </div>
              ) : (
                <div className="space-y-2">
                  {answers.map((answer, index) => (
                    <div
                      key={index}
                      className="flex items-start gap-2 p-3 border rounded-lg bg-card"
                    >
                      <div className="flex-shrink-0 mt-2">
                        <GripVertical className="h-4 w-4 text-muted-foreground" />
                      </div>
                      <div className="flex-1 space-y-2">
                        <div className="flex items-center gap-2">
                          <Input
                            value={answer.answerText}
                            onChange={(e) => handleAnswerChange(index, 'answerText', e.target.value)}
                            placeholder="Enter answer text"
                            required
                            className="flex-1"
                          />
                          <div className="flex items-center space-x-2">
                            <Checkbox
                              id={`correct-${index}`}
                              checked={answer.isCorrect}
                              onCheckedChange={(checked) =>
                                handleAnswerChange(index, 'isCorrect', checked === true)
                              }
                            />
                            <Label
                              htmlFor={`correct-${index}`}
                              className="text-xs cursor-pointer whitespace-nowrap"
                            >
                              Correct
                            </Label>
                          </div>
                          <Button
                            type="button"
                            variant="ghost"
                            size="sm"
                            className="h-8 w-8 p-0 text-destructive hover:text-destructive"
                            onClick={() => handleRemoveAnswer(index)}
                          >
                            <Trash2 className="h-4 w-4" />
                          </Button>
                        </div>
                        {answer.isCorrect && (
                          <Badge variant="default" className="text-xs bg-green-600 w-fit">
                            Correct Answer
                          </Badge>
                        )}
                      </div>
                    </div>
                  ))}
                </div>
              )}
            </div>
          </div>

          <DialogFooter>
            <Button type="button" variant="outline" onClick={() => onOpenChange(false)}>
              Cancel
            </Button>
            <Button type="submit" disabled={isLoading}>
              {isLoading ? 'Saving...' : 'Save Changes'}
            </Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  );
}

