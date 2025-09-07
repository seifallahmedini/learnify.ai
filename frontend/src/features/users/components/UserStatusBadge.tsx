import { Badge } from "@/shared/components/ui/badge"
import { CheckCircle, Clock } from "lucide-react"

interface UserStatusBadgeProps {
  isActive: boolean
  showIcon?: boolean
  className?: string
}

export function UserStatusBadge({ 
  isActive, 
  showIcon = true, 
  className = "" 
}: UserStatusBadgeProps) {
  return (
    <Badge 
      variant={isActive ? "default" : "secondary"} 
      className={className}
    >
      {showIcon && (
        isActive ? (
          <CheckCircle className="mr-1 h-3 w-3" />
        ) : (
          <Clock className="mr-1 h-3 w-3" />
        )
      )}
      {isActive ? "Active" : "Inactive"}
    </Badge>
  )
}
