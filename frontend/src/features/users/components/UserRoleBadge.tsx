import { Badge } from "@/shared/components/ui/badge"
import { Shield } from "lucide-react"
import type { UserRole } from "../types"
import { UserRole as UserRoleEnum } from "../types"
import { useUserUtils } from "../hooks/useUserUtils"

interface UserRoleBadgeProps {
  role: UserRole
  showIcon?: boolean
  variant?: "default" | "secondary" | "destructive" | "outline"
  className?: string
}

export function UserRoleBadge({ 
  role, 
  showIcon = true, 
  variant,
  className = "" 
}: UserRoleBadgeProps) {
  const { getRoleIcon, getRoleName } = useUserUtils()

  const getRoleBadgeVariant = (role: UserRole) => {
    switch (role) {
      case UserRoleEnum.Student:
        return "default"
      case UserRoleEnum.Instructor:
        return "secondary"
      case UserRoleEnum.Admin:
        return "destructive"
      default:
        return "outline"
    }
  }

  const badgeVariant = variant || getRoleBadgeVariant(role)

  return (
    <Badge variant={badgeVariant} className={className}>
      <span className="flex items-center gap-1">
        {showIcon && (role === UserRoleEnum.Admin ? <Shield className="h-3 w-3" /> : getRoleIcon(role))}
        {getRoleName(role)}
      </span>
    </Badge>
  )
}
