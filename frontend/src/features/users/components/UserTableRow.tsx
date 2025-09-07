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
import type { UserSummary } from "../types"
import { UserRoleBadge } from "./UserRoleBadge"
import { UserStatusBadge } from "./UserStatusBadge"

interface UserTableRowProps {
  user: UserSummary
  onActivate: (userId: number) => void
  onDeactivate: (userId: number) => void
  onDelete: (userId: number) => void
  onEdit?: (userId: number) => void
  onView?: (userId: number) => void
}

export function UserTableRow({ 
  user, 
  onActivate, 
  onDeactivate, 
  onDelete, 
  onEdit, 
  onView 
}: UserTableRowProps) {
  return (
    <TableRow>
      <TableCell>
        <div className="flex items-center space-x-3">
          <Avatar className="h-8 w-8">
            <AvatarImage 
              src={`https://api.dicebear.com/7.x/initials/svg?seed=${user.fullName}`} 
              alt={user.fullName} 
            />
            <AvatarFallback>
              {user.fullName.split(' ').map(n => n[0]).join('').slice(0, 2)}
            </AvatarFallback>
          </Avatar>
          <div>
            <div className="font-medium">{user.fullName}</div>
            <div className="text-sm text-muted-foreground">{user.email}</div>
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
        {new Date(user.createdAt).toLocaleDateString()}
      </TableCell>
      <TableCell className="text-right">
        <DropdownMenu>
          <DropdownMenuTrigger asChild>
            <Button variant="ghost" className="h-8 w-8 p-0">
              <MoreHorizontal className="h-4 w-4" />
            </Button>
          </DropdownMenuTrigger>
          <DropdownMenuContent align="end">
            <DropdownMenuLabel>Actions</DropdownMenuLabel>
            <DropdownMenuItem onClick={() => onView?.(user.id)}>
              <Eye className="mr-2 h-4 w-4" />
              View Details
            </DropdownMenuItem>
            <DropdownMenuItem onClick={() => onEdit?.(user.id)}>
              <Edit className="mr-2 h-4 w-4" />
              Edit User
            </DropdownMenuItem>
            <DropdownMenuSeparator />
            {user.isActive ? (
              <DropdownMenuItem
                onClick={() => onDeactivate(user.id)}
                className="text-orange-600"
              >
                <UserX className="mr-2 h-4 w-4" />
                Deactivate
              </DropdownMenuItem>
            ) : (
              <DropdownMenuItem
                onClick={() => onActivate(user.id)}
                className="text-green-600"
              >
                <UserCheck className="mr-2 h-4 w-4" />
                Activate
              </DropdownMenuItem>
            )}
            <DropdownMenuSeparator />
            <DropdownMenuItem
              onClick={() => onDelete(user.id)}
              className="text-red-600"
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
