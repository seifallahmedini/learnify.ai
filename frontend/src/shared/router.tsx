import { createBrowserRouter } from "react-router-dom"
import { MainLayout } from "@/shared/layouts/MainLayout"
import { DashboardPage } from "@/features/dashboard"
import { CoursesListPage } from "@/features/courses"
import { ProfilePage } from "@/features/profile"
import { LoginPage } from "@/features/auth"
import { UsersListPage, UserDetailsPage } from "@/features/users"

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