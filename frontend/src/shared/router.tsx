import { createBrowserRouter, Navigate } from "react-router-dom"
import { Suspense, lazy } from "react"
import { MainLayout } from "@/shared/layouts/MainLayout"

// Lazy-loaded pages for faster initial load
const DashboardPage = lazy(() => import("@/features/dashboard").then(m => ({ default: m.DashboardPage })))
const LoginPage = lazy(() => import("@/features/auth").then(m => ({ default: m.LoginPage })))

const CoursesListPage = lazy(() => import("@/features/courses").then(m => ({ default: m.CoursesListPage })))
const CourseDetailsPage = lazy(() => import("@/features/courses").then(m => ({ default: m.CourseDetailsPage })))

const LessonsListPage = lazy(() => import("@/features/lessons").then(m => ({ default: m.LessonsListPage })))
const AllLessonsPage = lazy(() => import("@/features/lessons").then(m => ({ default: m.AllLessonsPage })))
const LessonDetailsPage = lazy(() => import("@/features/lessons").then(m => ({ default: m.LessonDetailsPage })))

const QuizzesListPage = lazy(() => import("@/features/quizzes").then(m => ({ default: m.QuizzesListPage })))
const QuizDetailsPage = lazy(() => import("@/features/quizzes").then(m => ({ default: m.QuizDetailsPage })))

const CategoriesListPage = lazy(() => import("@/features/categories").then(m => ({ default: m.CategoriesListPage })))
const CategoryDetailsPage = lazy(() => import("@/features/categories").then(m => ({ default: m.CategoryDetailsPage })))

const UsersListPage = lazy(() => import("@/features/users").then(m => ({ default: m.UsersListPage })))
const UserDetailsPage = lazy(() => import("@/features/users").then(m => ({ default: m.UserDetailsPage })))

const ProfilePage = lazy(() => import("@/features/profile").then(m => ({ default: m.ProfilePage })))
const AssistantPage = lazy(() => import("@/features/assistant").then(m => ({ default: m.AssistantPage })))

// Simple error and 404 elements
function RouteError() {
  return <div className="p-6">Something went wrong loading this page.</div>
}
function NotFoundPage() {
  return <div className="p-6">Page not found.</div>
}

const withSuspense = (el: JSX.Element) => (
  <Suspense fallback={<div className="p-6">Loading…</div>}>{el}</Suspense>
)

export const router = createBrowserRouter([
  {
    path: "/login",
    element: withSuspense(<LoginPage />),
    errorElement: <RouteError />,
  },
  {
    path: "/",
    element: <MainLayout />,
    errorElement: <RouteError />,
    children: [
      {
        index: true,
        element: withSuspense(<DashboardPage />),
      },
      {
        path: "courses",
        children: [
          {
            index: true,
            element: withSuspense(<CoursesListPage />),
          },
          {
            path: ":courseId",
            children: [
              {
                index: true,
                element: withSuspense(<CourseDetailsPage />),
              },
              {
                path: "lessons",
                element: withSuspense(<LessonsListPage />),
              },
              {
                path: "quizzes",
                element: withSuspense(<QuizzesListPage />),
              },
            ],
          },
        ],
      },
      {
        path: "lessons",
        children: [
          {
            index: true,
            element: withSuspense(<AllLessonsPage />),
          },
          {
            path: ":lessonId",
            element: withSuspense(<LessonDetailsPage />),
          },
        ],
      },
      {
        path: "quizzes/:quizId",
        element: withSuspense(<QuizDetailsPage />),
      },
      {
        path: "categories",
        children: [
          {
            index: true,
            element: withSuspense(<CategoriesListPage />),
          },
          {
            path: ":categoryId",
            element: withSuspense(<CategoryDetailsPage />),
          },
        ],
      },
      {
        path: "profile",
        element: withSuspense(<ProfilePage />),
      },
      {
        path: "assistant",
        element: withSuspense(<AssistantPage />),
      },
      {
        path: "users",
        children: [
          {
            index: true,
            element: withSuspense(<UsersListPage />),
          },
          {
            path: ":userId",
            element: withSuspense(<UserDetailsPage />),
          },
        ],
      },
      {
        path: "404",
        element: <NotFoundPage />,
      },
      {
        path: "*",
        element: <NotFoundPage />,
      },
    ],
  },
  // Top-level catch-all redirect to layout 404
  { path: "*", element: <Navigate to="/404" replace /> },
])