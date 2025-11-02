// Quiz Feature Types

export interface ApiResponse<T> {
  data: T;
  message: string;
  success: boolean;
}

// Question Type Enum
export enum QuestionType {
  MultipleChoice = 1,
  TrueFalse = 2,
  ShortAnswer = 3,
  Essay = 4
}

// Quiz Types
export interface Quiz {
  id: number;
  courseId: number;
  courseTitle: string;
  lessonId?: number;
  lessonTitle?: string;
  title: string;
  description: string;
  timeLimit?: number; // in minutes
  passingScore: number; // percentage (0-100)
  maxAttempts: number;
  isActive: boolean;
  questionCount: number;
  totalPoints: number;
  createdAt: string;
  updatedAt: string;
}

export interface QuizSummary {
  id: number;
  courseId: number;
  lessonId?: number;
  title: string;
  description: string;
  timeLimit?: number;
  passingScore: number;
  isActive: boolean;
  questionCount: number;
  createdAt: string;
}

// Question Types
export interface Question {
  id: number;
  quizId: number;
  questionText: string;
  questionType: QuestionType;
  points: number;
  orderIndex: number;
  isActive: boolean;
  answerCount: number;
  createdAt?: string;
  updatedAt?: string;
}

export interface QuestionSummary {
  id: number;
  quizId: number;
  questionText: string;
  questionType: QuestionType;
  points: number;
  orderIndex: number;
  isActive: boolean;
  answerCount: number;
}

// Answer Types
export interface Answer {
  id: number;
  questionId: number;
  questionText?: string;
  answerText: string;
  isCorrect: boolean;
  orderIndex: number;
  createdAt?: string;
  updatedAt?: string;
}

export interface AnswerSummary {
  id: number;
  questionId: number;
  answerText: string;
  isCorrect: boolean;
  orderIndex: number;
}

// Quiz Attempt Types
export interface QuizAttempt {
  id: number;
  quizId: number;
  quizTitle: string;
  userId: number;
  userName: string;
  startedAt: string;
  completedAt?: string;
  score?: number;
  maxScore: number;
  isCompleted: boolean;
  timeSpentMinutes: number;
  timeRemainingMinutes?: number;
}

// Response Types
export interface QuizListResponse {
  quizzes: QuizSummary[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

export interface CourseQuizzesResponse {
  courseId: number;
  courseTitle: string;
  quizzes: QuizSummary[];
  totalCount: number;
  activeQuizzes: number;
  inactiveQuizzes: number;
}

export interface QuizQuestionsResponse {
  quizId: number;
  quizTitle: string;
  questions: QuestionSummary[];
  totalQuestions: number;
  totalPoints: number;
}

export interface QuizAttemptsResponse {
  quizId: number;
  quizTitle: string;
  attempts: QuizAttempt[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
  averageScore: number;
  completedAttempts: number;
}

export interface StartQuizAttemptResponse {
  attemptId: number;
  quizId: number;
  quizTitle: string;
  userId: number;
  startedAt: string;
  timeLimit?: number;
  expiresAt?: string;
  questions: QuizQuestionResponse[];
}

export interface QuizQuestionResponse {
  id: number;
  questionText: string;
  questionType: QuestionType;
  points: number;
  orderIndex: number;
  answers: QuizAnswerResponse[];
}

export interface QuizAnswerResponse {
  id: number;
  answerText: string;
  orderIndex: number;
  // Note: IsCorrect is not included for student responses
}

// Request Types
export interface CreateQuizRequest {
  courseId: number;
  lessonId?: number;
  title: string;
  description: string;
  timeLimit?: number;
  passingScore?: number;
  maxAttempts?: number;
  isActive?: boolean;
}

export interface UpdateQuizRequest {
  title?: string;
  description?: string;
  timeLimit?: number;
  passingScore?: number;
  maxAttempts?: number;
  isActive?: boolean;
}

export interface GetQuizzesRequest {
  courseId?: number;
  lessonId?: number;
  isActive?: boolean;
  page?: number;
  pageSize?: number;
}

export interface AddQuestionToQuizRequest {
  questionText: string;
  questionType: QuestionType;
  points?: number;
  orderIndex?: number;
}

export interface StartQuizAttemptRequest {
  userId: number;
}

// Answer Response Types
// Response from /api/answers/question/{questionId}
export interface QuestionAnswersResponse {
  questionId: number;
  questionText: string;
  questionType: QuestionType;
  answers: AnswerSummary[];
  totalAnswers: number;
  correctAnswersCount: number;
}
