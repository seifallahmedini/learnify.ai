import { Avatar, AvatarFallback, AvatarImage } from "@/shared/components/ui/avatar"
import { Checkbox } from "@/shared/components/ui/checkbox"
import { Badge } from "@/shared/components/ui/badge"
import { TableCell, TableRow } from "@/shared/components/ui/table"
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from "@/shared/components/ui/dropdown-menu"
import { Button } from "@/shared/components/ui/button"
import { MoreHorizontal, Eye, Edit, UserCheck, UserX, Trash2 } from "lucide-react"
import type { UserSummary } from "../../types"
import { UserRoleBadge } from "../shared/UserRoleBadge"
import { UserStatusBadge } from "../shared/UserStatusBadge"

interface UserTableRowProps {
  user: UserSummary
  selectedUsers: number[]
  onSelectUser: (userId: number, checked: boolean) => void
  onActivate: (userId: number) => void
  onDeactivate: (userId: number) => void
  onDelete: (userId: number) => void
  onEdit?: (userId: number) => void
  onView?: (userId: number) => void
}

export function UserTableRow({ 
  user, 
  selectedUsers,
  onSelectUser,
  onActivate, 
  onDeactivate, 
  onDelete, 
  onEdit, 
  onView 
}: UserTableRowProps) {
  const isSelected = selectedUsers.includes(user.id)
  
  // Generate initials from full name
  const getInitials = (name: string) => {
    return name
      .split(' ')
      .map(n => n[0])
      .join('')
      .toUpperCase()
      .slice(0, 2)
  }

  const initials = getInitials(user.fullName)

  return (
    <TableRow className="hover:bg-muted/50">
      <TableCell className="py-4">
        <Checkbox
          checked={isSelected}
          onCheckedChange={(checked) => onSelectUser(user.id, checked as boolean)}
          aria-label={`Select user ${user.fullName}`}
        />
      </TableCell>
      
      <TableCell className="py-4">
        <div className="flex items-center gap-3 max-w-[300px]">
          <Avatar className="h-10 w-10 shrink-0">
            <AvatarImage 
              src={(user as any).profilePicture} 
              alt={user.fullName}
            />
            <AvatarFallback className="bg-primary/10 text-primary font-medium text-sm">
              {initials}
            </AvatarFallback>
          </Avatar>
          <div className="space-y-1 min-w-0 flex-1">
            <h3 
              className="font-medium truncate hover:text-primary transition-colors cursor-pointer"
              onClick={() => onView?.(user.id)}
            >
              {user.fullName}
            </h3>
            <p className="text-sm text-muted-foreground truncate">
              {user.email}
            </p>
          </div>
        </div>
      </TableCell>
      
      <TableCell>
        <UserRoleBadge role={user.role} />
      </TableCell>
      
      <TableCell>
        <UserStatusBadge isActive={user.isActive} />
      </TableCell>
      
      <TableCell>
        <span className="text-sm">
          {new Date(user.createdAt).toLocaleDateString('en-US', {
            year: 'numeric',
            month: 'short',
            day: 'numeric'
          })}
        </span>
      </TableCell>
      
      <TableCell>
        <DropdownMenu modal={false}>
          <DropdownMenuTrigger asChild>
            <Button variant="ghost" size="sm" className="h-8 w-8 p-0">
              <MoreHorizontal className="h-4 w-4" />
              <span className="sr-only">Open menu</span>
            </Button>
          </DropdownMenuTrigger>
          <DropdownMenuContent align="end">
            <DropdownMenuItem onClick={() => onView?.(user.id)}>
              <Eye className="mr-2 h-4 w-4" />
              View Details
            </DropdownMenuItem>
            <DropdownMenuItem 
              onClick={(e) => {
                e.preventDefault();
                onEdit?.(user.id);
              }}
            >
              <Edit className="mr-2 h-4 w-4" />
              Edit User
            </DropdownMenuItem>
            {user.isActive ? (
              <DropdownMenuItem
                onClick={() => onDeactivate(user.id)}
              >
                <UserX className="mr-2 h-4 w-4" />
                Deactivate
              </DropdownMenuItem>
            ) : (
              <DropdownMenuItem
                onClick={() => onActivate(user.id)}
              >
                <UserCheck className="mr-2 h-4 w-4" />
                Activate
              </DropdownMenuItem>
            )}
            <DropdownMenuItem 
              onClick={() => onDelete(user.id)} 
              className="text-destructive"
            >
              <Trash2 className="mr-2 h-4 w-4" />
              Delete User
            </DropdownMenuItem>
          </DropdownMenuContent>
        </DropdownMenu>
      </TableCell>
    </TableRow>
  )
}
