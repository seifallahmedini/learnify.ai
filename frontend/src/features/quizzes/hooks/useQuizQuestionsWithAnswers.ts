import { useState, useEffect, useCallback } from 'react';
import { quizzesApi } from '../services';
import { answersApi } from '../services/answersApi';
import type { QuizQuestionsResponse, QuestionAnswersResponse, QuestionSummary } from '../types';

/**
 * Hook to fetch quiz questions and their answers
 */
export const useQuizQuestionsWithAnswers = (quizId: number | null) => {
  const [questions, setQuestions] = useState<QuestionSummary[]>([]);
  const [questionsWithAnswers, setQuestionsWithAnswers] = useState<Map<number, QuestionAnswersResponse>>(new Map());
  const [isLoading, setIsLoading] = useState(false);
  const [isLoadingAnswers, setIsLoadingAnswers] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const loadQuestions = useCallback(async () => {
    if (!quizId) return;
    
    setIsLoading(true);
    setError(null);
    try {
      const data = await quizzesApi.getQuizQuestions(quizId);
      setQuestions(data.questions);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to load questions');
      setQuestions([]);
    } finally {
      setIsLoading(false);
    }
  }, [quizId]);

  const loadAnswersForQuestion = useCallback(async (questionId: number) => {
    setIsLoadingAnswers(true);
    try {
      const answerData = await answersApi.getQuestionAnswers(questionId);
      setQuestionsWithAnswers(prev => {
        const newMap = new Map(prev);
        newMap.set(questionId, answerData);
        return newMap;
      });
    } catch (err) {
      console.error(`Failed to load answers for question ${questionId}:`, err);
    } finally {
      setIsLoadingAnswers(false);
    }
  }, []);

  const loadAllAnswers = useCallback(async (questionsToLoad: QuestionSummary[]) => {
    if (questionsToLoad.length === 0) return;
    
    setIsLoadingAnswers(true);
    try {
      const answerPromises = questionsToLoad.map(question => 
        answersApi.getQuestionAnswers(question.id).catch(err => {
          console.error(`Failed to load answers for question ${question.id}:`, err);
          return null;
        })
      );
      
      const answersResults = await Promise.all(answerPromises);
      const answersMap = new Map<number, QuestionAnswersResponse>();
      
      answersResults.forEach((answerData, index) => {
        if (answerData) {
          answersMap.set(questionsToLoad[index].id, answerData);
        }
      });
      
      setQuestionsWithAnswers(answersMap);
    } catch (err) {
      console.error('Failed to load answers:', err);
    } finally {
      setIsLoadingAnswers(false);
    }
  }, []);

  useEffect(() => {
    loadQuestions();
  }, [loadQuestions]);

  useEffect(() => {
    if (questions.length > 0) {
      loadAllAnswers(questions);
    }
  }, [questions, loadAllAnswers]);

  return {
    questions,
    questionsWithAnswers,
    isLoading,
    isLoadingAnswers,
    error,
    loadQuestions,
    loadAnswersForQuestion,
    loadAllAnswers,
  };
};

