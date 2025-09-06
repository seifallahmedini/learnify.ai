import { Users, GraduationCap, Shield } from "lucide-react"
import { UserRole } from "../types"

export function useUserUtils() {
  const getRoleIcon = (role: UserRole) => {
    switch (role) {
      case UserRole.Student:
        return <Users className="h-4 w-4" />
      case UserRole.Instructor:
        return <GraduationCap className="h-4 w-4" />
      case UserRole.Admin:
        return <Shield className="h-4 w-4" />
      default:
        return <Users className="h-4 w-4" />
    }
  }

  const getRoleColor = (role: UserRole) => {
    switch (role) {
      case UserRole.Student:
        return "bg-blue-100 text-blue-800"
      case UserRole.Instructor:
        return "bg-green-100 text-green-800"
      case UserRole.Admin:
        return "bg-red-100 text-red-800"
      default:
        return "bg-gray-100 text-gray-800"
    }
  }

  const getRoleName = (role: UserRole) => {
    switch (role) {
      case UserRole.Student:
        return "Student"
      case UserRole.Instructor:
        return "Instructor"
      case UserRole.Admin:
        return "Admin"
      default:
        return "Unknown"
    }
  }

  return {
    getRoleIcon,
    getRoleColor,
    getRoleName,
  }
}
