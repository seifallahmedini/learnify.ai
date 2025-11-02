import { useState } from 'react';
import { Button } from '@/shared/components/ui/button';
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from '@/shared/components/ui/dialog';
import { Input } from '@/shared/components/ui/input';
import { Textarea } from '@/shared/components/ui/textarea';
import { Label } from '@/shared/components/ui/label';
import { Checkbox } from '@/shared/components/ui/checkbox';
import { Badge } from '@/shared/components/ui/badge';
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/shared/components/ui/select';
import { Plus, Trash2, GripVertical } from 'lucide-react';
import { quizzesApi } from '../../services';
import { answersApi, type CreateAnswerRequest } from '../../services/answersApi';
import { QuestionType } from '../../types';

interface CreateQuestionDialogProps {
  quizId: number;
  onQuestionCreated?: () => void;
  trigger?: React.ReactNode;
}

interface AnswerFormData {
  answerText: string;
  isCorrect: boolean;
  orderIndex: number;
}

const getQuestionTypeLabel = (type: QuestionType): string => {
  switch (type) {
    case QuestionType.MultipleChoice:
      return 'Multiple Choice';
    case QuestionType.TrueFalse:
      return 'True/False';
    case QuestionType.ShortAnswer:
      return 'Short Answer';
    case QuestionType.Essay:
      return 'Essay';
    default:
      return 'Unknown';
  }
};

export function CreateQuestionDialog({
  quizId,
  onQuestionCreated,
  trigger,
}: CreateQuestionDialogProps) {
  const [open, setOpen] = useState(false);
  const [questionText, setQuestionText] = useState('');
  const [questionType, setQuestionType] = useState<QuestionType>(QuestionType.MultipleChoice);
  const [points, setPoints] = useState<number>(1);
  const [answers, setAnswers] = useState<AnswerFormData[]>([]);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

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
    if (field === 'isCorrect' && value === true && questionType === QuestionType.MultipleChoice) {
      newAnswers.forEach((answer, i) => {
        if (i !== index) {
          answer.isCorrect = false;
        }
      });
    }
    
    setAnswers(newAnswers);
  };

  const handleOpenChange = (newOpen: boolean) => {
    setOpen(newOpen);
    if (!newOpen) {
      // Reset form when dialog closes
      setQuestionText('');
      setQuestionType(QuestionType.MultipleChoice);
      setPoints(1);
      setAnswers([]);
      setError(null);
    }
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

      if (points < 0) {
        throw new Error('Points must be 0 or greater');
      }

      const validAnswers = answers.filter((a) => a.answerText.trim());
      
      // For non-essay questions, require at least one answer
      if (questionType !== QuestionType.Essay && validAnswers.length === 0) {
        throw new Error('At least one answer is required');
      }

      // Check if there's at least one correct answer (for non-essay types)
      const hasCorrectAnswer = validAnswers.some((a) => a.isCorrect);
      if (questionType !== QuestionType.Essay && !hasCorrectAnswer) {
        throw new Error('At least one answer must be marked as correct');
      }

      // For multiple choice, ensure only one correct answer
      if (questionType === QuestionType.MultipleChoice) {
        const correctCount = validAnswers.filter((a) => a.isCorrect).length;
        if (correctCount !== 1) {
          throw new Error('Multiple choice questions must have exactly one correct answer');
        }
      }

      // Create the question
      const questionResponse = await quizzesApi.addQuestionToQuiz(quizId, {
        questionText: questionText.trim(),
        questionType,
        points,
        orderIndex: undefined, // Will be set by backend
      });

      const questionId = questionResponse?.id;
      if (!questionId) {
        throw new Error('Failed to create question - invalid response');
      }

      // Create answers for the question
      for (const answer of validAnswers) {
        await answersApi.createAnswer({
          questionId,
          answerText: answer.answerText.trim(),
          isCorrect: answer.isCorrect,
          orderIndex: answer.orderIndex,
        });
      }

      onQuestionCreated?.();
      handleOpenChange(false);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to create question');
    } finally {
      setIsLoading(false);
    }
  };

  const defaultTrigger = (
    <Button size="sm" className="gap-1.5">
      <Plus className="h-4 w-4" />
      <span className="hidden sm:inline">Add Question</span>
    </Button>
  );

  return (
    <Dialog open={open} onOpenChange={handleOpenChange}>
      <DialogTrigger asChild>
        {trigger || defaultTrigger}
      </DialogTrigger>
      <DialogContent className="sm:max-w-[700px] max-h-[90vh] overflow-y-auto">
        <form onSubmit={handleSubmit}>
          <DialogHeader>
            <DialogTitle>Add New Question</DialogTitle>
            <DialogDescription>
              Create a new question for this quiz. You can add multiple answer options.
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
              />
            </div>

            {/* Question Type and Points */}
            <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
              <div className="space-y-2">
                <Label htmlFor="questionType">Question Type *</Label>
                <Select
                  value={questionType.toString()}
                  onValueChange={(value) => {
                    const newType = parseInt(value) as QuestionType;
                    setQuestionType(newType);
                    // Reset answers when type changes (except if switching between similar types)
                    if (newType === QuestionType.Essay) {
                      setAnswers([]);
                    } else if (answers.length === 0) {
                      handleAddAnswer();
                    }
                  }}
                >
                  <SelectTrigger id="questionType">
                    <SelectValue />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value={QuestionType.MultipleChoice.toString()}>
                      Multiple Choice
                    </SelectItem>
                    <SelectItem value={QuestionType.TrueFalse.toString()}>
                      True/False
                    </SelectItem>
                    <SelectItem value={QuestionType.ShortAnswer.toString()}>
                      Short Answer
                    </SelectItem>
                    <SelectItem value={QuestionType.Essay.toString()}>
                      Essay
                    </SelectItem>
                  </SelectContent>
                </Select>
              </div>

              <div className="space-y-2">
                <Label htmlFor="points">Points *</Label>
                <Input
                  id="points"
                  type="number"
                  min="0"
                  step="0.5"
                  value={points}
                  onChange={(e) => setPoints(parseFloat(e.target.value) || 0)}
                  required
                />
              </div>
            </div>

            {/* Question Type Badge */}
            <div className="flex items-center gap-2">
              <Label className="text-sm">Question Type:</Label>
              <Badge variant="secondary">{getQuestionTypeLabel(questionType)}</Badge>
              <Badge variant="outline">{points} {points === 1 ? 'point' : 'points'}</Badge>
            </div>

            {/* Answers Section - Hide for Essay type */}
            {questionType !== QuestionType.Essay && (
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
                                disabled={questionType === QuestionType.MultipleChoice && !answer.isCorrect && answers.some(a => a.isCorrect)}
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

                {questionType === QuestionType.TrueFalse && answers.length < 2 && (
                  <p className="text-xs text-muted-foreground">
                    True/False questions typically have 2 answers (True and False)
                  </p>
                )}
              </div>
            )}

            {/* Essay Type Info */}
            {questionType === QuestionType.Essay && (
              <div className="p-3 bg-blue-50 dark:bg-blue-950/20 border border-blue-200 dark:border-blue-800 rounded-lg">
                <p className="text-sm text-blue-900 dark:text-blue-100">
                  Essay questions don't require predefined answers. Students will provide a written response.
                </p>
              </div>
            )}
          </div>

          <DialogFooter>
            <Button type="button" variant="outline" onClick={() => handleOpenChange(false)} disabled={isLoading}>
              Cancel
            </Button>
            <Button type="submit" disabled={isLoading}>
              {isLoading ? 'Creating...' : 'Create Question'}
            </Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  );
}

