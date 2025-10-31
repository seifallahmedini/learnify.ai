import { createBrowserRouter } from "react-router-dom"
import { MainLayout } from "@/shared/layouts/MainLayout"
import { DashboardPage } from "@/features/dashboard"
import { CoursesListPage, CourseDetailsPage } from "@/features/courses"
import { CategoriesListPage, CategoryDetailsPage } from "@/features/categories"
import { ProfilePage } from "@/features/profile"
import { LoginPage } from "@/features/auth"
import { UsersListPage, UserDetailsPage } from "@/features/users"
import { LessonsListPage, LessonDetailsPage, AllLessonsPage } from "@/features/lessons"
import { QuizzesListPage, QuizDetailsPage } from "@/features/quizzes"

export const router = createBrowserRouter([
  {
    path: "/login",
    element: <LoginPage />,
  },
  {
    path: "/",
    element: <MainLayout />,
    children: [
      {
        index: true,
        element: <DashboardPage />,
      },
      {
        path: "courses",
        element: <CoursesListPage />,
      },
      {
        path: "courses/:courseId",
        element: <CourseDetailsPage />,
      },
      {
        path: "courses/:courseId/lessons",
        element: <LessonsListPage />,
      },
      {
        path: "courses/:courseId/quizzes",
        element: <QuizzesListPage />,
      },
      {
        path: "lessons",
        element: <AllLessonsPage />,
      },
      {
        path: "lessons/:lessonId",
        element: <LessonDetailsPage />,
      },
      {
        path: "quizzes/:quizId",
        element: <QuizDetailsPage />,
      },
      {
        path: "categories",
        element: <CategoriesListPage />,
      },
      {
        path: "categories/:categoryId",
        element: <CategoryDetailsPage />,
      },
      {
        path: "profile",
        element: <ProfilePage />,
      },
      {
        path: "users",
        element: <UsersListPage />,
      },
      {
        path: "users/:userId",
        element: <UserDetailsPage />,
      },
    ],
  },
])