import { Avatar, AvatarFallback, AvatarImage } from "@/shared/components/ui/avatar"
import { TableCell, TableRow } from "@/shared/components/ui/table"
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuLabel,
  DropdownMenuSeparator,
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

  return (
    <TableRow 
      className={`
        group hover:bg-muted/50 transition-all duration-200 border-b border-border/40
        ${isSelected ? 'bg-primary/5 border-primary/20' : ''}
      `}
    >
      <TableCell className="w-12">
        <div className="flex items-center justify-center">
          <label className="relative inline-flex items-center cursor-pointer">
            <input
              type="checkbox"
              checked={isSelected}
              onChange={(e) => onSelectUser(user.id, e.target.checked)}
              className="sr-only peer"
              aria-label={`Select ${user.fullName}`}
            />
            <div className={`
              relative w-4 h-4 rounded border-2 transition-all duration-200
              ${isSelected 
                ? 'bg-primary border-primary' 
                : 'border-input hover:border-primary/50 peer-focus:ring-2 peer-focus:ring-primary/20'
              }
            `}>
              {isSelected && (
                <svg className="absolute inset-0 w-3 h-3 text-primary-foreground m-auto" fill="currentColor" viewBox="0 0 16 16">
                  <path d="M13.854 3.646a.5.5 0 0 1 0 .708l-7 7a.5.5 0 0 1-.708 0l-3.5-3.5a.5.5 0 1 1 .708-.708L6.5 10.293l6.646-6.647a.5.5 0 0 1 .708 0z"/>
                </svg>
              )}
            </div>
          </label>
        </div>
      </TableCell>
      
      <TableCell className="py-4">
        <div className="flex items-center space-x-4">
          <Avatar className="h-12 w-12 border-2 border-background shadow-md transition-all duration-200 group-hover:shadow-lg">
            <AvatarImage 
              src={`https://api.dicebear.com/7.x/initials/svg?seed=${user.fullName}`} 
              alt={user.fullName}
              className="object-cover"
            />
            <AvatarFallback className="text-sm font-semibold bg-gradient-to-br from-primary/10 to-primary/20 text-primary">
              {user.fullName.split(' ').map(n => n[0]).join('').slice(0, 2)}
            </AvatarFallback>
          </Avatar>
          <div className="space-y-1 min-w-0 flex-1">
            <div 
              className="font-semibold text-lg line-clamp-1 text-foreground hover:text-primary transition-colors cursor-pointer group-hover:text-primary"
              onClick={() => onView?.(user.id)}
              role="button"
              tabIndex={0}
              onKeyDown={(e) => e.key === 'Enter' && onView?.(user.id)}
            >
              {user.fullName}
            </div>
            <div className="text-sm text-muted-foreground line-clamp-1">
              {user.email}
            </div>
          </div>
        </div>
      </TableCell>
      
      <TableCell className="py-4">
        <UserRoleBadge role={user.role} />
      </TableCell>
      
      <TableCell className="py-4">
        <UserStatusBadge isActive={user.isActive} />
      </TableCell>
      
      <TableCell className="py-4">
        <div className="text-sm font-medium text-foreground">
          {new Date(user.createdAt).toLocaleDateString('en-US', {
            year: 'numeric',
            month: 'short',
            day: 'numeric'
          })}
        </div>
      </TableCell>
      
      <TableCell>
        <DropdownMenu>
          <DropdownMenuTrigger asChild>
            <Button variant="ghost" className="h-8 w-8 p-0 hover:bg-muted transition-colors">
              <MoreHorizontal className="h-4 w-4" />
              <span className="sr-only">Open menu</span>
            </Button>
          </DropdownMenuTrigger>
          <DropdownMenuContent align="end" className="w-48">
            <DropdownMenuLabel className="text-xs font-semibold text-muted-foreground uppercase tracking-wider">
              User Actions
            </DropdownMenuLabel>
            <DropdownMenuSeparator />
            
            {onView && (
              <DropdownMenuItem 
                onClick={() => onView(user.id)}
                className="flex items-center gap-2 cursor-pointer"
              >
                <Eye className="h-4 w-4 text-blue-600" />
                <span>View Details</span>
              </DropdownMenuItem>
            )}
            
            <DropdownMenuItem 
              onClick={() => onEdit?.(user.id)}
              className="flex items-center gap-2 cursor-pointer"
            >
              <Edit className="h-4 w-4 text-amber-600" />
              <span>Edit User</span>
            </DropdownMenuItem>
            
            <DropdownMenuSeparator />
            
            {user.isActive ? (
              <DropdownMenuItem
                onClick={() => onDeactivate(user.id)}
                className="flex items-center gap-2 cursor-pointer"
              >
                <UserX className="h-4 w-4 text-orange-600" />
                <span>Deactivate</span>
              </DropdownMenuItem>
            ) : (
              <DropdownMenuItem
                onClick={() => onActivate(user.id)}
                className="flex items-center gap-2 cursor-pointer"
              >
                <UserCheck className="h-4 w-4 text-green-600" />
                <span>Activate</span>
              </DropdownMenuItem>
            )}
            
            <DropdownMenuSeparator />
            
            <DropdownMenuItem
              onClick={() => onDelete(user.id)}
              className="flex items-center gap-2 cursor-pointer text-red-600 focus:text-red-600 focus:bg-red-50"
            >
              <Trash2 className="h-4 w-4" />
              <span>Delete User</span>
            </DropdownMenuItem>
          </DropdownMenuContent>
        </DropdownMenu>
      </TableCell>
    </TableRow>
  )
}
