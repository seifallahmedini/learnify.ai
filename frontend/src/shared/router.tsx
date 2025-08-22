import { createBrowserRouter } from "react-router-dom"
import { MainLayout } from "@/shared/layouts/MainLayout"
import { DashboardPage } from "@/features/dashboard"
import { CoursesPage } from "@/features/courses"
import { ProfilePage } from "@/features/profile"
import { LoginPage } from "@/features/auth"

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
        element: <CoursesPage />,
      },
      {
        path: "profile",
        element: <ProfilePage />,
      },
    ],
  },
])